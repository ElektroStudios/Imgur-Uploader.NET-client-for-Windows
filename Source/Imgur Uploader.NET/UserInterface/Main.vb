
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports CButtonLib
Imports ImgurUploader.Tools
Imports ImgurUploader.UserInterface.CustomControls
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks

#End Region

Namespace UserInterface

    ''' <summary>
    ''' The Main UserInterface Form.
    ''' </summary>
    Public NotInheritable Class Main

#Region " Private Fields "

        ''' <summary>
        ''' The <see cref="WindowSticker"/> instance that sticks the Form on the Desktop screen.
        ''' </summary>
        Private windowSticker As New WindowSticker(clientForm:=Me) With {.SnapMargin = 35}

        ''' <summary>
        ''' Indicates the imgurAPI instance to upload images.
        ''' </summary>
        Private WithEvents ImgurClient As New ImgurAPI()

        ''' <summary>
        ''' Indicates the supported formats by Imgur.
        ''' </summary>
        Private ReadOnly supportedFormats As String() =
            ".BMP .JPG .JPEG .GIF .PNG .APNG .TIF .TIFF .PDF .XCF".Split(" "c)

        ''' <summary>
        ''' The maximum width and height, in pixels, of the preview that is shown in the picture box.
        ''' </summary>
        Private Const PreviewMaxDimension As Integer = 1920

        ''' <summary>
        ''' The Windows message that notifies a change of system settings.
        ''' </summary>
        Private Const WmSettingChange As Integer = &H1A

        ''' <summary>
        ''' The image file that is selected and ready to be uploaded, or <see langword="Nothing"/> if there is none.
        ''' </summary>
        Private pendingFile As FileInfo

        ''' <summary>
        ''' The default image (logo) of the picture box. It is a shared resource, so it must never be disposed.
        ''' </summary>
        Private logoImage As Image

        ''' <summary>
        ''' The last successfully uploaded image, or <see langword="Nothing"/> if there is none.
        ''' </summary>
        Private currentImage As ImgurImage

        ''' <summary>
        ''' The way the next image will be uploaded: anonymously, or to the account of the user.
        ''' </summary>
        Private uploadMode As ImgurUploadMode = ImgurUploadMode.Account

        ''' <summary>
        ''' The drop-down menu of the upload button, which allows to choose the <see cref="Main.uploadMode"/>.
        ''' </summary>
        Private uploadMenu As ContextMenuStrip

        ''' <summary>
        ''' The menu item that selects <see cref="ImgurUploadMode.Account"/>.
        ''' </summary>
        Private menuItemUploadToAccount As ToolStripMenuItem

        ''' <summary>
        ''' The menu item that selects <see cref="ImgurUploadMode.Anonymous"/>.
        ''' </summary>
        Private menuItemUploadAnonymously As ToolStripMenuItem

        ''' <summary>
        ''' The menu item that signs in to (or signs out of) the imgur account.
        ''' </summary>
        Private menuItemAccountAction As ToolStripMenuItem

        ''' <summary>
        ''' The persistent status-bar text (the one that is restored after a transient message).
        ''' </summary>
        Private statusText As String = "Ready"

        ''' <summary>
        ''' The persistent status-bar color (the one that is restored after a transient message).
        ''' </summary>
        Private statusColor As Color = Color.Gainsboro

        ''' <summary>
        ''' The uploading background task.
        ''' </summary>
        Private uploadTask As Task

        ''' <summary>
        ''' The uploading task cancellation token source.
        ''' </summary>
        Private uploadTaskCTSource As CancellationTokenSource

        ''' <summary>
        ''' The uploading task cancellation token.
        ''' </summary>
        Private uploadTaskCT As CancellationToken

        ''' <summary>
        ''' The maximum file size, in bytes, that Imgur accepts for non-animated images (50 MB).
        ''' </summary>
        Private Const MaxImageFileSize As Long = 52428800L

        ''' <summary>
        ''' The maximum file size, in bytes, that Imgur accepts for animated images (200 MB).
        ''' </summary>
        Private Const MaxAnimatedFileSize As Long = 209715200L

        ''' <summary>
        ''' The url of the project repository.
        ''' </summary>
        Private Const RepositoryUrl As String = "https://github.com/ElektroStudios/Imgur-Uploader.NET-client-for-Windows"

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Main"/> class.
        ''' </summary>
        Public Sub New()

            Dim english As Globalization.CultureInfo = Globalization.CultureInfo.GetCultureInfo("en-US")
            Globalization.CultureInfo.DefaultThreadCurrentUICulture = english
            Threading.Thread.CurrentThread.CurrentUICulture = english

            ' This call is required by the designer.
            Me.InitializeComponent()

            Me.Text = $"{My.Application.Info.Title} {My.Application.Info.Version.Major}.{My.Application.Info.Version.Minor}"

            ' Globe emoji (U+1F310) as a UTF-16 surrogate pair.
            Me.ToolStripStatusLabel_GitHub.Text = ChrW(&HD83C) & ChrW(&HDF10) & " GitHub"

            ' Add any initialization after the InitializeComponent() call.
            Me.logoImage = Me.PictureBox_Logo.Image
            Me.InitializeSelectors()
            Me.SetToolTips()
            Me.SetClientIds()
            Me.InitializeUploadMenu()
            Me.LoadAccount()

        End Sub

#End Region

#Region " Overrides "

        ''' <summary>
        ''' Raises the HandleCreated event.
        ''' The dark title bar must be applied here, because it has no effect when applied later.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnHandleCreated(e As EventArgs)

            MyBase.OnHandleCreated(e)
            DarkTitleBar.ApplyCurrentTheme(Me.Handle)

        End Sub

        ''' <summary>
        ''' Processes Windows messages.
        ''' Re-applies the title bar theme when the user changes the Windows theme while the app is running.
        ''' </summary>
        ''' <param name="m">The Windows <see cref="Message"/> to process.</param>
        Protected Overrides Sub WndProc(ByRef m As Message)

            MyBase.WndProc(m)

            If (m.Msg = WmSettingChange) AndAlso (m.LParam <> IntPtr.Zero) AndAlso
               (Marshal.PtrToStringUni(m.LParam) = "ImmersiveColorSet") AndAlso Me.IsHandleCreated Then

                DarkTitleBar.ApplyCurrentTheme(Me.Handle)
            End If

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the Shown event of the Main form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Main_Shown(sender As Object, e As EventArgs) _
        Handles Me.Shown

            Me.MinimumSize = Me.Size

            If My.Application.CommandLineArgs.Count <> 0 Then
                Me.SelectFile(New FileInfo(My.Application.CommandLineArgs.First))
            End If

        End Sub

        ''' <summary>
        ''' Handles the DragEnter event of the Main Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        Private Sub Main_DragEnter(sender As Object, e As DragEventArgs) _
        Handles Me.DragEnter

            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.All
            End If

        End Sub

        ''' <summary>
        ''' Handles the DragDrop event of the Main Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        Private Sub Main_DragDrop(sender As Object, e As DragEventArgs) _
        Handles Me.DragDrop

            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Me.SelectFile(New FileInfo(DirectCast(e.Data.GetData(DataFormats.FileDrop), String())(0)))
            End If

        End Sub

        ''' <summary>
        ''' Handles the ClickButtonArea event of the CButton_BrowseImage control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Private Sub CButton_BrowseImage_ClickButtonArea(sender As Object, e As MouseEventArgs) _
        Handles CButton_BrowseImage.ClickButtonArea

            If Me.OpenFileDialog_BrowseImage.ShowDialog() = DialogResult.OK Then
                Me.SelectFile(New FileInfo(Me.OpenFileDialog_BrowseImage.FileName))
            End If

        End Sub

        ''' <summary>
        ''' Handles the MainAreaClick event of the Button_Upload control.
        ''' Uploads the selected image using the current upload mode (the arrow of the button changes it).
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Button_Upload_MainAreaClick(sender As Object, e As EventArgs) _
        Handles Button_Upload.MainAreaClick

            Me.StartUpload()

        End Sub

        ''' <summary>
        ''' Handles the Opening event of the upload menu.
        ''' Refreshes the check marks and the texts that depend on the account.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        Private Sub UploadMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs)

            Dim isSignedIn As Boolean = Me.ImgurClient.IsAuthorized

            Me.menuItemUploadToAccount.Checked = (Me.uploadMode = ImgurUploadMode.Account)
            Me.menuItemUploadAnonymously.Checked = (Me.uploadMode = ImgurUploadMode.Anonymous)

            Me.menuItemUploadToAccount.Text = If(isSignedIn,
                                                 $"Upload to my account ({Me.AccountName})",
                                                 "Upload to my account (sign-in required)")

            Me.menuItemAccountAction.Text = If(isSignedIn,
                                               $"Sign out ({Me.AccountName})",
                                               "Sign in to Imgur...")

        End Sub

        ''' <summary>
        ''' Handles the Click event of the "Upload to my account" menu item.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub MenuItemUploadToAccount_Click(sender As Object, e As EventArgs)

            Me.SetUploadMode(ImgurUploadMode.Account)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the "Upload anonymously" menu item.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub MenuItemUploadAnonymously_Click(sender As Object, e As EventArgs)

            Me.SetUploadMode(ImgurUploadMode.Anonymous)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the "Sign in / Sign out" menu item.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub MenuItemAccountAction_Click(sender As Object, e As EventArgs)

            If Me.ImgurClient.IsAuthorized Then
                Me.SignOutAccount()

            ElseIf Me.SignIn() Then
                Me.SetUploadMode(ImgurUploadMode.Account)

            End If

        End Sub

        ''' <summary>
        ''' Handles the OnAccountTokensChanged event of the imgurClient.
        ''' Stores the account, so the user does not have to authorize the application again.
        ''' This event can be raised from a background thread.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ImgurClient_OnAccountTokensChanged(sender As Object, e As EventArgs) _
        Handles ImgurClient.OnAccountTokensChanged

            Me.StoreAccount()

        End Sub

        ''' <summary>
        ''' Handles the ClickButtonArea event of the CButton_Abort control.
        ''' Requests the cancellation of the upload; the upload task restores the user interface when it finishes.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Private Sub CButton_Abort_ClickButtonArea(sender As Object, e As MouseEventArgs) _
        Handles CButton_Abort.ClickButtonArea

            ' Never block the UI thread waiting for the task: the task invokes the UI thread to finish its work.
            Me.CButton_Abort.Enabled = False
            Me.uploadTaskCTSource.Cancel()

        End Sub

        ''' <summary>
        ''' Handles the SelectedIndexChanged event of both selectors.
        ''' Refreshes the formatted result.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Selectors_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles Selector_Size.SelectedIndexChanged,
                Selector_Format.SelectedIndexChanged

            Me.UpdateResult()

        End Sub

        ''' <summary>
        ''' Handles the MouseDown event of the TextBox_Result control.
        ''' Selects all the text, so it can be copied manually.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Private Sub TextBox_Result_MouseDown(sender As Object, e As MouseEventArgs) _
        Handles TextBox_Result.MouseDown

            Me.TextBox_Result.SelectAll()

        End Sub

        ''' <summary>
        ''' Handles the MainAreaClick event of the Button_Copy control.
        ''' Copies the formatted result to the clipboard.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ''' <exception cref="InvalidOperationException">
        ''' There is no result to copy.
        ''' </exception>
        Private Sub Button_Copy_MainAreaClick(sender As Object, e As EventArgs) _
        Handles Button_Copy.MainAreaClick

            If String.IsNullOrEmpty(Me.TextBox_Result.Text) Then
                Throw New InvalidOperationException("There is no result to copy.")
            End If

            Clipboard.SetText(Me.TextBox_Result.Text)
            Me.SetStatus("Copied to clipboard.", Color.YellowGreen, transient:=True)

        End Sub

        ''' <summary>
        ''' Handles the MainAreaClick event of the Button_Open control.
        ''' Opens the URL (direct URL or share link) in the default browser.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ''' <exception cref="InvalidOperationException">
        ''' The selected format is not a URL, or there is no uploaded image.
        ''' </exception>
        Private Sub Button_Open_MainAreaClick(sender As Object, e As EventArgs) _
        Handles Button_Open.MainAreaClick

            If (Me.currentImage Is Nothing) OrElse Not ImgurUrlFormatter.IsOpenable(Me.SelectedFormat) Then
                Throw New InvalidOperationException("Only a URL of an uploaded image can be opened.")
            End If

            Dim url As String = ImgurUrlFormatter.Format(Me.currentImage, Me.SelectedSize, Me.SelectedFormat)
            Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})

        End Sub

        ''' <summary>
        ''' Handles the Tick event of the Timer_StatusRestore control.
        ''' Restores the persistent status-bar message.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Timer_StatusRestore_Tick(sender As Object, e As EventArgs) _
        Handles Timer_StatusRestore.Tick

            Me.Timer_StatusRestore.Stop()
            Me.ToolStripStatusLabel_File.Text = Me.statusText
            Me.ToolStripStatusLabel_File.ForeColor = Me.statusColor

        End Sub

        ''' <summary>
        ''' Handles the OnAsyncError event of the imgurClient.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="Exception"/> instance containing the error.</param>
        Private Sub ImgurClient_OnAsyncError(sender As Object, e As Exception) _
        Handles ImgurClient.OnAsyncError

            Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

            MessageBox.Show(e.Message, Me.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Error)

            Me.InvokeControl(Me, Sub(x As Form)
                                     x.Enabled = True
                                     Me.RestoreIdleState()
                                 End Sub)

        End Sub

        ''' <summary>
        ''' Handles the Imgur status error events of the imgurClient.
        ''' Informs the user about the reason of the failed upload.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The Imgur status.</param>
        Private Sub ImgurClient_OnStatusError(sender As Object, e As ImgurAPI.ImgurStatus) _
        Handles ImgurClient.OnAccessForbidden,
                ImgurClient.OnAuthorizationFailed,
                ImgurClient.OnBadImageFormat,
                ImgurClient.OnInternalServerError,
                ImgurClient.OnPageIsNotFound,
                ImgurClient.OnUploadRateLimitError,
                ImgurClient.OnUnknownError

            Me.SetStatus($"Imgur error: {e} ({CInt(e)})", Color.IndianRed)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripStatusLabel_GitHub control.
        ''' Opens the project repository in the default browser.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripStatusLabel_GitHub_Click(sender As Object, e As EventArgs) _
        Handles ToolStripStatusLabel_GitHub.Click

            Process.Start(New ProcessStartInfo(RepositoryUrl) With {.UseShellExecute = True})

        End Sub

#End Region

#Region " Methods "

        ''' <summary>
        ''' Gets the size currently selected in <see cref="Main.Selector_Size"/>.
        ''' </summary>
        Private ReadOnly Property SelectedSize As ImgurUrlSize
            Get
                Return CType(Me.Selector_Size.SelectedIndex, ImgurUrlSize)
            End Get
        End Property

        ''' <summary>
        ''' Gets the format currently selected in <see cref="Main.Selector_Format"/>.
        ''' </summary>
        Private ReadOnly Property SelectedFormat As ImgurUrlFormat
            Get
                Return CType(Me.Selector_Format.SelectedIndex, ImgurUrlFormat)
            End Get
        End Property

        ''' <summary>
        ''' Fills the size and format selectors.
        ''' The item order must match the order of <see cref="ImgurUrlSize"/> and <see cref="ImgurUrlFormat"/>.
        ''' </summary>
        Private Sub InitializeSelectors()

            Me.Selector_Size.SetItems(New String() {"Original", "Small Square", "Big Square", "Small Thumbnail", "Medium Thumbnail", "Large Thumbnail", "Huge Thumbnail"})
            Me.Selector_Format.SetItems(New String() {"Direct URL", "Share Link", "CSS", "HTML", "HTML Embed", "BBCode", "Markdown", "Markdown (Image)"})
            Me.UpdateResult()

        End Sub

        ''' <summary>
        ''' Refreshes the result text box and the state of the copy and open buttons,
        ''' according to the uploaded image and the selected size and format.
        ''' This method must be called from the UI thread.
        ''' </summary>
        Private Sub UpdateResult()

            Dim hasImage As Boolean = (Me.currentImage IsNot Nothing)

            If hasImage Then
                Me.TextBox_Result.Text = ImgurUrlFormatter.Format(Me.currentImage, Me.SelectedSize, Me.SelectedFormat)
            Else
                Me.TextBox_Result.Clear()
            End If

            ' Some formats (share link, HTML embed, official Markdown) do not depend on the size.
            Me.Selector_Size.Enabled = ImgurUrlFormatter.UsesSize(Me.SelectedFormat)

            Me.Button_Copy.Enabled = hasImage
            Me.Button_Open.Enabled = hasImage AndAlso ImgurUrlFormatter.IsOpenable(Me.SelectedFormat)

        End Sub

        ''' <summary>
        ''' Sets the tooltips of the controls.
        ''' </summary>
        Private Sub SetToolTips()

            Me.ToolTip_Actions.SetToolTip(Me.CButton_BrowseImage, "Select an image file (you can also drag and drop it onto the window).")
            ' (The tooltip of the upload button depends on the upload mode: see UpdateUploadModeUi.)
            Me.ToolTip_Actions.SetToolTip(Me.Selector_Size, "Select which image size to use (not used by the Share Link, HTML Embed and Markdown formats).")
            Me.ToolTip_Actions.SetToolTip(Me.Selector_Format, "Select the output format.")
            Me.ToolTip_Actions.SetToolTip(Me.Button_Copy, "Copy the result to the clipboard.")
            Me.ToolTip_Actions.SetToolTip(Me.Button_Open, "Open the URL in the default browser (Direct URL and Share Link formats only).")

        End Sub

        ''' <summary>
        ''' Gets the Imgur user name of the authorized account, or a generic text if it is unknown.
        ''' </summary>
        Private ReadOnly Property AccountName As String
            Get
                Dim name As String = Me.ImgurClient.AccountUsername
                Return If(String.IsNullOrEmpty(name), "unknown user", name)
            End Get
        End Property

        ''' <summary>
        ''' Creates the drop-down menu of the upload button.
        ''' </summary>
        Private Sub InitializeUploadMenu()

            Me.menuItemUploadToAccount = New ToolStripMenuItem("Upload to my account")
            Me.menuItemUploadAnonymously = New ToolStripMenuItem("Upload anonymously")
            Me.menuItemAccountAction = New ToolStripMenuItem("Sign in to Imgur...")

            AddHandler Me.menuItemUploadToAccount.Click, AddressOf Me.MenuItemUploadToAccount_Click
            AddHandler Me.menuItemUploadAnonymously.Click, AddressOf Me.MenuItemUploadAnonymously_Click
            AddHandler Me.menuItemAccountAction.Click, AddressOf Me.MenuItemAccountAction_Click

            Me.uploadMenu = New ContextMenuStrip(Me.components) With {
                .BackColor = Color.FromArgb(37, 37, 38),
                .ForeColor = Color.Gainsboro,
                .Renderer = New DarkMenuRenderer(),
                .ShowCheckMargin = True,
                .ShowImageMargin = False
            }

            Me.uploadMenu.Items.AddRange(New ToolStripItem() {Me.menuItemUploadToAccount,
                                                              Me.menuItemUploadAnonymously,
                                                              New ToolStripSeparator(),
                                                              Me.menuItemAccountAction})

            AddHandler Me.uploadMenu.Opening, AddressOf Me.UploadMenu_Opening

            Me.Button_Upload.DropDownMenu = Me.uploadMenu

        End Sub

        ''' <summary>
        ''' Loads the preferred upload mode and the account that authorized the application in a previous session.
        ''' </summary>
        Private Sub LoadAccount()

            Me.uploadMode = UserAccountStore.LoadUploadMode()

            Dim refreshToken As String = Nothing
            Dim username As String = Nothing

            If UserAccountStore.TryLoadAccount(refreshToken, username) Then
                Me.ImgurClient.RestoreAccount(refreshToken, username)
            End If

            Me.UpdateUploadModeUi()

        End Sub

        ''' <summary>
        ''' Stores the authorized account in the profile of the user.
        ''' This method is thread-safe.
        ''' </summary>
        Private Sub StoreAccount()

            If Not Me.ImgurClient.IsAuthorized Then
                Return
            End If

            If Not UserAccountStore.SaveAccount(Me.ImgurClient.RefreshToken, Me.ImgurClient.AccountUsername) Then
                Me.SetStatus("The account could not be saved on this computer: you will have to sign in again the next time.", Color.Orange)
            End If

        End Sub

        ''' <summary>
        ''' Changes the upload mode and remembers it.
        ''' </summary>
        ''' <param name="mode">The new upload mode.</param>
        Private Sub SetUploadMode(mode As ImgurUploadMode)

            Me.uploadMode = mode
            UserAccountStore.SaveUploadMode(mode)
            Me.UpdateUploadModeUi()

        End Sub

        ''' <summary>
        ''' Updates the text and the tooltip of the upload button, according to the upload mode and the account.
        ''' This method must be called from the UI thread.
        ''' </summary>
        Private Sub UpdateUploadModeUi()

            Dim tip As String

            If Me.uploadMode = ImgurUploadMode.Account Then

                Me.Button_Upload.Text = "Upload to my account"

                If Me.ImgurClient.IsAuthorized Then
                    tip = $"Upload the selected image to your Imgur account ({Me.AccountName}). Use the arrow to change the mode or to sign out."
                Else
                    tip = "Upload the selected image to your Imgur account. You will be asked to authorize the application first. Use the arrow to change the mode."
                End If

            Else
                Me.Button_Upload.Text = "Upload anonymously"
                tip = "Upload the selected image anonymously: it will not be linked to any Imgur account. Use the arrow to change the mode."

            End If

            Me.ToolTip_Actions.SetToolTip(Me.Button_Upload, tip)

        End Sub

        ''' <summary>
        ''' Shows the authorization dialog, so the user allows the application to upload images to his Imgur account.
        ''' </summary>
        ''' <returns>
        ''' <see langword="True"/> if the application was authorized; <see langword="False"/> if the user canceled.
        ''' </returns>
        Private Function SignIn() As Boolean

            Using dialog As New AuthorizationDialog(Me.ImgurClient)

                If dialog.ShowDialog(Me) <> DialogResult.OK Then
                    Return False
                End If

            End Using

            ' (The account is stored when the tokens change, but this makes sure it is also stored if they did not.)
            Me.StoreAccount()
            Me.UpdateUploadModeUi()
            Me.SetStatus($"Signed in to Imgur as {Me.AccountName}.", Color.YellowGreen, transient:=True)

            Return True

        End Function

        ''' <summary>
        ''' Forgets the authorized account.
        ''' </summary>
        Private Sub SignOutAccount()

            Dim name As String = Me.AccountName

            Me.ImgurClient.SignOut()
            UserAccountStore.ClearAccount()
            Me.UpdateUploadModeUi()

            MessageBox.Show($"The account '{name}' was removed from this computer." & Environment.NewLine & Environment.NewLine &
                            "To revoke the access of this application completely, remove it from the applications connected to your account in the Imgur settings.",
                            Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)

        End Sub

        ''' <summary>
        ''' Describes where an image is uploaded to, for the status bar.
        ''' </summary>
        ''' <param name="mode">The upload mode.</param>
        ''' <returns>The description.</returns>
        Private Function DescribeDestination(mode As ImgurUploadMode) As String

            If mode = ImgurUploadMode.Account Then
                Return $"to your account ({Me.AccountName})"
            End If

            Return "anonymously"

        End Function

        ''' <summary>
        ''' Reads the INI file to retrieve and set the 'ClientId' and 'ClientSecret'.
        ''' </summary>
        Private Sub SetClientIds()

            Select Case IniFileManager.File.Exist()

                Case True
                    Me.ImgurClient.ClientId = CStr(IniFileManager.Key.Get("ClientId", "", "[API]"))
                    Me.ImgurClient.ClientSecret = CStr(IniFileManager.Key.Get("ClientSecret", "", "[API]"))

                Case False
                    Dim iniPath As String = IniFileManager.FilePath

                    Dim message As String = String.Join(Environment.NewLine, New String() {
                        $"The configuration file '{Path.GetFileName(iniPath)}' was not found.",
                        "This is normal the first time you run the application: it needs your own Imgur API credentials in order to upload images.",
                        String.Empty,
                        "How to fix it:",
                        String.Empty,
                        "1. Register an application on Imgur to get a Client ID and a Client Secret:",
                        "    https://apidocs.imgur.com/#register",
                        String.Empty,
                        "2. Create a text file with this exact name and location:",
                        $"    {iniPath}",
                        String.Empty,
                        "3. Write these lines in it, replacing the values with your credentials:",
                        String.Empty,
                        "    [API]",
                        "    ClientId=your client id",
                        "    ClientSecret=your client secret",
                        String.Empty,
                        "4. Save the file and start the application again.",
                        String.Empty,
                        "The application will close now."
                    })

                    MessageBox.Show(message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End

            End Select

            If String.IsNullOrEmpty(Me.ImgurClient.ClientId) Then
                MessageBox.Show("Client Id. value is empty in the INI file", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End If

            If String.IsNullOrEmpty(Me.ImgurClient.ClientSecret) Then
                MessageBox.Show("Client Secret value is empty in the INI file.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End If

            If Not Me.ImgurClient.ClientId Like "[aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9]*" Then
                MessageBox.Show("Ivalid Client Id. value specified in the INI file", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End If

            If Not Me.ImgurClient.ClientSecret Like "[aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9][aA0-zZ9]*" Then
                MessageBox.Show("Ivalid Client Secret value specified in the INI file", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End If

        End Sub

        ''' <summary>
        ''' Sets the status-bar message. This method is thread-safe.
        ''' </summary>
        ''' <param name="text">The message.</param>
        ''' <param name="textColor">The text color.</param>
        ''' <param name="transient">
        ''' <see langword="True"/> to show the message for a few seconds and then restore the previous persistent message;
        ''' <see langword="False"/> to make it the new persistent message.
        ''' </param>
        Private Sub SetStatus(text As String, textColor As Color, Optional transient As Boolean = False)

            Me.InvokeControl(Me.StatusStrip1,
                             Sub(x As StatusStrip)
                                 If Not transient Then
                                     Me.statusText = text
                                     Me.statusColor = textColor
                                 End If

                                 Me.ToolStripStatusLabel_File.Text = text
                                 Me.ToolStripStatusLabel_File.ForeColor = textColor

                                 Me.Timer_StatusRestore.Stop()
                                 If transient Then
                                     Me.Timer_StatusRestore.Start()
                                 End If
                             End Sub)

        End Sub

        ''' <summary>
        ''' Shows the specified image in the picture box and releases the previous preview.
        ''' </summary>
        ''' <param name="image">The image to show.</param>
        Private Sub SetPreview(image As Image)

            Dim previous As Image = Me.PictureBox_Logo.Image
            Me.PictureBox_Logo.Image = image

            If (previous IsNot Nothing) AndAlso Not Object.ReferenceEquals(previous, Me.logoImage) Then
                previous.Dispose()
            End If

        End Sub

        ''' <summary>
        ''' Selects an image file (from the browse dialog, drag-drop or command line):
        ''' shows its preview and enables the upload button. The image is not uploaded until the user confirms it.
        ''' </summary>
        ''' <param name="imageFile">The image file.</param>
        Private Sub SelectFile(imageFile As FileInfo)

            ' Files cannot be selected while an upload is in progress.
            If Not Me.CButton_BrowseImage.Enabled Then
                Return
            End If

            If Not File.Exists(imageFile.FullName) _
            OrElse Not Me.supportedFormats.Contains(imageFile.Extension.ToUpperInvariant()) Then

                MessageBox.Show("Unsupported file format.",
                                Me.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return

            End If

            ' PDF and corrupted images have no preview: the logo is shown instead.
            Dim preview As Bitmap = ImagePreviewLoader.TryLoad(imageFile.FullName, PreviewMaxDimension)
            Me.SetPreview(If(preview IsNot Nothing, DirectCast(preview, Image), Me.logoImage))

            Me.ShowFileInfo(imageFile)

            Me.pendingFile = imageFile
            Me.currentImage = Nothing
            Me.UpdateResult()
            Me.UpdateUploadButton()
            Me.SetStatus($"Ready to upload: {imageFile.Name}", Color.Gainsboro)

        End Sub

        ''' <summary>
        ''' Shows the name, extension, size and resolution of the selected image file.
        ''' </summary>
        ''' <param name="imageFile">The image file.</param>
        Private Sub ShowFileInfo(imageFile As FileInfo)

            Dim resolution As Size? = ImagePreviewLoader.TryGetResolution(imageFile.FullName)

            Me.Label_FileName.Text = imageFile.Name
            Me.Label_FileDetails.Text = ImageInfoFormatter.GetDetails(imageFile, resolution)
            Me.ToolTip_Actions.SetToolTip(Me.Label_FileName, imageFile.FullName)

        End Sub

        ''' <summary>
        ''' Enables the upload button only when there is a selected file.
        ''' </summary>
        Private Sub UpdateUploadButton()

            Me.Button_Upload.Enabled = (Me.pendingFile IsNot Nothing)

        End Sub

        ''' <summary>
        ''' Uploads the selected file.
        ''' </summary>
        ''' 
        ''' <remarks>
        ''' Imgur limits (see https://help.imgur.com/hc/en-us/articles/26511665959579-What-files-can-I-upload-Is-there-a-size-limit):
        ''' 50 MB for non-animated images and 200 MB for animated images (GIF). Below these limits Imgur itself converts
        ''' big PNG files to JPEG and recompresses non-animated images, so no client-side compression is needed.
        ''' Images over the limit are compressed (as JPEG) by the upload task, so the user interface never freezes.
        ''' </remarks>
        ''' 
        ''' <exception cref="InvalidOperationException">
        ''' There is no selected file to upload.
        ''' </exception>
        Private Sub StartUpload()

            If Me.pendingFile Is Nothing Then
                Throw New InvalidOperationException("There is no selected file to upload.")
            End If

            Dim imageFile As FileInfo = Me.pendingFile
            imageFile.Refresh()

            If Not imageFile.Exists Then
                MessageBox.Show("The selected file no longer exists.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.pendingFile = Nothing
                Me.UpdateUploadButton()
                Return
            End If

            Dim extension As String = imageFile.Extension.ToLowerInvariant()
            Dim isAnimated As Boolean = (extension = ".gif") OrElse (extension = ".apng")
            Dim maxFileSize As Long = If(isAnimated, MaxAnimatedFileSize, MaxImageFileSize)
            Dim compress As Boolean = False

            If imageFile.Length > maxFileSize Then

                Dim limitText As String = $"The maximum file size accepted by Imgur for this format is {ByteSizeFormatter.Format(maxFileSize)}."

                Select Case extension

                    Case ".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff"
                        If MessageBox.Show($"{limitText}{Environment.NewLine}{Environment.NewLine}Would you like to let ImgurUploader to compress the image automatically? (it will be converted to JPEG)",
                                           Me.Text,
                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) <> DialogResult.OK Then
                            Return
                        End If
                        compress = True

                    Case ".gif", ".apng", ".pdf", ".xcf"
                        MessageBox.Show($"{limitText}{Environment.NewLine}{Environment.NewLine}Automatic compression is not supported for this format.",
                                        Me.Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return

                    Case Else
                        Throw New InvalidOperationException($"Unexpected file extension: '{extension}'.")

                End Select

            End If

            ' Uploading to the account requires that the user has authorized the application.
            ' This is asked after the checks above, so the user is not asked to authorize an image that cannot be uploaded.
            Dim mode As ImgurUploadMode = Me.uploadMode

            If (mode = ImgurUploadMode.Account) AndAlso Not Me.ImgurClient.IsAuthorized Then
                If Not Me.SignIn() Then
                    Return
                End If
            End If

            Me.currentImage = Nothing
            Me.UpdateResult()
            Me.CButton_BrowseImage.Enabled = False
            Me.Button_Upload.Hide()
            Me.CButton_Abort.Enabled = True
            Me.CButton_Abort.Show()

            ' Release the resources of the previous (already finished) upload.
            If (Me.uploadTask IsNot Nothing) AndAlso Me.uploadTask.IsCompleted Then
                Me.uploadTask.Dispose()
            End If
            Me.uploadTaskCTSource?.Dispose()

            Dim sourcePath As String = imageFile.FullName

            Me.uploadTaskCTSource = New CancellationTokenSource
            Me.uploadTaskCT = Me.uploadTaskCTSource.Token
            Me.uploadTask = Task.Factory.StartNew(Sub()
                                                      Me.UploadImage(sourcePath, compress, maxFileSize, mode, Me.uploadTaskCT)
                                                  End Sub, Me.uploadTaskCT)

        End Sub

        ''' <summary>
        ''' Creates a unique temporary file path (the file itself is not created) with the specified extension.
        ''' </summary>
        ''' <param name="extension">The extension, including the dot.</param>
        ''' <returns>The temporary file path.</returns>
        Private Shared Function CreateTempFilePath(extension As String) As String

            Dim tempFile As String = Path.GetTempFileName()

            Try
                Return Path.ChangeExtension(tempFile, extension)

            Finally
                File.Delete(tempFile)

            End Try

        End Function

        ''' <summary>
        ''' Compresses the image if needed, uploads it to imgur and shows the formatted result.
        ''' This method runs in the upload task (not in the UI thread).
        ''' </summary>
        ''' <param name="filename">The filename.</param>
        ''' <param name="compress"><see langword="True"/> to compress the image (as JPEG) before uploading it.</param>
        ''' <param name="maxFileSize">The maximum file size, in bytes, of the compressed image.</param>
        ''' <param name="mode">The upload mode: anonymously, or to the account of the user.</param>
        ''' <param name="ct">The Task CancellationToken.</param>
        Private Sub UploadImage(filename As String, compress As Boolean, maxFileSize As Long, mode As ImgurUploadMode, ct As CancellationToken)

            Me.InvokeControl(Me, Sub(x As Form) x.AllowDrop = False)

            Dim displayName As String = Path.GetFileName(filename)
            Dim compressedFile As String = Nothing

            Try
                Dim fileToUpload As String = filename

                If compress Then
                    Me.SetStatus($"Compressing: {displayName}...", Color.Gainsboro)
                    compressedFile = CreateTempFilePath(".jpg")
                    ImageTools.CompressImage(filename, compressedFile, ImageFormat.Jpeg, maxFileSize)
                    fileToUpload = compressedFile
                End If

                Dim urls As ImgurImage = Nothing

                If Not ct.IsCancellationRequested Then
                    Me.SetStatus($"Uploading {Me.DescribeDestination(mode)}: {displayName}...", Color.Gainsboro)

                    Using ct.Register(Sub() Me.ImgurClient.UploadImageAsyncCancel())
                        urls = Me.ImgurClient.UploadImageAsync(fileToUpload, mode)
                    End Using
                End If

                If ct.IsCancellationRequested Then
                    Me.SetStatus($"Aborted: {displayName}", Color.Gainsboro)
                    Me.InvokeControl(Me, Sub(x As Form) Me.RestoreIdleState())

                ElseIf urls IsNot Nothing Then
                    Me.InvokeControl(Me, Sub(x As Form) Me.ShowResults(urls))
                    Me.SetStatus($"Success: {displayName} (uploaded {Me.DescribeDestination(mode)})", Color.Gainsboro)

                ElseIf Me.statusColor <> Color.IndianRed Then
                    ' (When Imgur reported a specific error, its message is already in the status bar.)
                    Me.InvokeControl(Me, Sub(x As Form) Me.RestoreIdleState())
                    Me.SetStatus($"Error: {displayName}", Color.IndianRed)

                Else
                    Me.InvokeControl(Me, Sub(x As Form) Me.RestoreIdleState())

                End If

            Catch ex As ImgurAuthorizationException
                ' Imgur no longer accepts the saved authorization (the user revoked it): the user must authorize again.
                Me.ImgurClient.SignOut()
                UserAccountStore.ClearAccount()
                Me.SetStatus($"Authorization lost: {displayName}", Color.IndianRed)
                Me.InvokeControl(Me, Sub(x As Form)
                                         MessageBox.Show(x,
                                                         $"Imgur no longer accepts the saved authorization of your account.{Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.NewLine}Click Upload again to authorize the application again.",
                                                         x.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                         Me.UpdateUploadModeUi()
                                         Me.RestoreIdleState()
                                     End Sub)

            Catch ex As Exception
                ' An exception must never escape from a background task: the UI would remain in the "uploading" state forever.
                Me.SetStatus($"Error: {displayName}", Color.IndianRed)
                Me.InvokeControl(Me, Sub(x As Form)
                                         MessageBox.Show(x, ex.Message, x.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                         Me.RestoreIdleState()
                                     End Sub)

            Finally
                If (compressedFile IsNot Nothing) AndAlso File.Exists(compressedFile) Then
                    File.Delete(compressedFile)
                End If

            End Try

        End Sub

        ''' <summary>
        ''' Stores the result of a successful upload and refreshes the result controls.
        ''' This method must be called from the UI thread.
        ''' </summary>
        ''' <param name="urls">The uploaded image urls.</param>
        Private Sub ShowResults(urls As ImgurImage)

            Me.currentImage = urls
            Me.pendingFile = Nothing
            Me.UpdateResult()
            Me.RestoreIdleState()

        End Sub

        ''' <summary>
        ''' Restores the browse and upload buttons and the drag-drop support after an upload has finished or has been aborted.
        ''' This method must be called from the UI thread.
        ''' </summary>
        Private Sub RestoreIdleState()

            Me.CButton_Abort.Hide()
            Me.Button_Upload.Show()
            Me.CButton_BrowseImage.Enabled = True
            Me.UpdateUploadButton()
            Me.AllowDrop = True

        End Sub

        ' Invoke Control
        ' ( By Elektro )
        '
        ' Usage Examples :
        ' InvokeControl(TextBox1, Sub(x As TextBox) x.AppendText("Hello"))
        ' InvokeControl(CheckBox1, Sub(x As CheckBox) x.Checked = True)
        '
        ''' <summary>
        ''' Invokes an <see cref="T:Action"/> delegate on the specified control.
        ''' This method avoids cross-threading exceptions.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="control">The control to invoke.</param>
        ''' <param name="action">The encapsulated method.</param>
        Public Sub InvokeControl(Of T As Control)(control As T, action As Action(Of T))

            If control.InvokeRequired Then
                control.Invoke(New Action(Of T, Action(Of T))(AddressOf Me.InvokeControl),
                                    New Object() {control, action})

            Else
                action(control)

            End If

        End Sub

#End Region

    End Class

End Namespace
