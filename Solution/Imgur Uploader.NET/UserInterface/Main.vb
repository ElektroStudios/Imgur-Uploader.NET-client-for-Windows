
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports System.Threading.Tasks
Imports CButtonLib
Imports System.Threading
Imports System.IO
Imports System.Drawing.Imaging

#End Region

Namespace UserInterface

    ''' <summary>
    ''' The Main UserInterface Form.
    ''' </summary>
    Public NotInheritable Class Main

#Region " Objects "

        ''' <summary>
        ''' The <see cref="WindowSticker"/> instance that sticks the Form on the Desktop screen.
        ''' </summary>
        Private windowSticker As New WindowSticker(ClientForm:=Me) With {.SnapMargin = 35}

        ''' <summary>
        ''' Indicates the imgurAPI instance to upload images.
        ''' </summary>
        Private WithEvents imgurClient As New ImgurAPI()

        ''' <summary>
        ''' Indicates the supported formats by Imgur.
        ''' </summary>
        Private ReadOnly supportedFormats As String() =
            ".BMP .JPG .JPEG .GIF .PNG .APNG .TIF .TIFF .PDF .XCF".Split(" "c)

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

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Main"/> class.
        ''' </summary>
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.SetClientIds()

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the Shown event of the Main form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Main_Shown(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Me.Shown

            If My.Application.CommandLineArgs.Count <> 0 Then

                Me.LoadImage(New FileInfo(My.Application.CommandLineArgs.First))

            End If

        End Sub

        ''' <summary>
        ''' Handles the DragEnter event of the Main Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        Private Sub Main_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) _
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
        Private Sub Main_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) _
        Handles Me.DragDrop

            If e.Data.GetDataPresent(DataFormats.FileDrop) Then

                Me.LoadImage(New FileInfo(DirectCast(e.Data.GetData(DataFormats.FileDrop), IEnumerable(Of String))(0)))

            End If

        End Sub

        ''' <summary>
        ''' Handles the ClickButtonArea event of the CButton_BrowseImage control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Private Sub CButton_BrowseImage_ClickButtonArea(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles CButton_BrowseImage.ClickButtonArea

            If Me.OpenFileDialog_BrowseImage.ShowDialog() = Windows.Forms.DialogResult.OK Then

                Me.LoadImage(New FileInfo(Me.OpenFileDialog_BrowseImage.FileName))

            End If

        End Sub

        ''' <summary>
        ''' Handles the ClickButtonArea event of the CButton_Abort control.
        ''' </summary>
        ''' <param name="Sender">The source of the event.</param>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Private Sub CButton_Abort_ClickButtonArea(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles CButton_Abort.ClickButtonArea

            Me.uploadTaskCTSource.Cancel(True)
            Me.uploadTask.Wait()
            Me.uploadTask.Dispose()
            Me.uploadTaskCTSource.Dispose()

            Me.CButton_Abort.Hide()
            Me.CButton_BrowseImage.Show()
            Me.Panel_Urls.Enabled = False
            Me.Enabled = True
            Me.AllowDrop = True

        End Sub

        ''' <summary>
        ''' Handles the EnabledChanged event of the Buttons Clipboard controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Buttons_Clip_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Clip_Normal.EnabledChanged,
                Button_Clip_HugeThumbnail.EnabledChanged,
                Button_Clip_LargeThumbnail.EnabledChanged,
                Button_Clip_MediumThumbnail.EnabledChanged,
                Button_Clip_SmallThumbnail.EnabledChanged,
                Button_BBCode_Normal.EnabledChanged,
                Button_BBCode_Thumbnail.EnabledChanged

            If DirectCast(sender, Button).Enabled Then
                DirectCast(sender, Button).BackgroundImage = My.Resources.Clipboard
            Else
                DirectCast(sender, Button).BackgroundImage = My.Resources.ClipboardGray
            End If

        End Sub

        ''' <summary>
        ''' Handles the Click event of the Button controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Buttons_Clip_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Clip_Normal.Click,
                Button_Clip_HugeThumbnail.Click,
                Button_Clip_LargeThumbnail.Click,
                Button_Clip_MediumThumbnail.Click,
                Button_Clip_SmallThumbnail.Click,
                Button_BBCode_Normal.Click,
                Button_BBCode_Thumbnail.Click

            Clipboard.SetText(DirectCast(Me.TableLayoutPanel1.Controls("TextBox_" & CStr(DirectCast(sender, Button).Tag)), TextBox).Text)

        End Sub

        ''' <summary>
        ''' Handles the EnabledChanged event of the Buttons Url controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Buttons_Url_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Url_Normal.EnabledChanged,
                Button_Url_HugeThumbnail.EnabledChanged,
                Button_Url_LargeThumbnail.EnabledChanged,
                Button_Url_MediumThumbnail.EnabledChanged,
                Button_Url_SmallThumbnail.EnabledChanged

            If DirectCast(sender, Button).Enabled Then
                DirectCast(sender, Button).BackgroundImage = My.Resources.Go
            Else
                DirectCast(sender, Button).BackgroundImage = My.Resources.Go_Gray
            End If

        End Sub

        ''' <summary>
        ''' Handles the Click event of the Buttons Url controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Buttons_Url_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Url_Normal.Click,
                Button_Url_HugeThumbnail.Click,
                Button_Url_LargeThumbnail.Click,
                Button_Url_MediumThumbnail.Click,
                Button_Url_SmallThumbnail.Click

            Dim tb As TextBox = DirectCast(Me.TableLayoutPanel1.Controls("TextBox_" & CStr(DirectCast(sender, Button).Tag)), TextBox)

            If Not String.IsNullOrEmpty(tb.Text) Then
                Process.Start(tb.Text)
            End If

        End Sub

        ''' <summary>
        ''' Handles the EnabledChanged event of the TextBox controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TextBoxes_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TextBox_Url_Normal.EnabledChanged,
                TextBox_Url_SmallThumbnail.EnabledChanged,
                TextBox_Url_MediumThumbnail.EnabledChanged,
                TextBox_Url_LargeThumbnail.EnabledChanged,
                TextBox_Url_HugeThumbnail.EnabledChanged,
                TextBox_BBCode_Normal.EnabledChanged,
                TextBox_BBCode_Thumbnail.EnabledChanged

            Dim tb As TextBox = DirectCast(sender, TextBox)

            If Not tb.Enabled Then
                tb.Clear()
            End If

        End Sub

        ''' <summary>
        ''' Handles the MouseDown and MouseMove events of the TextBox Urls controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TextBox_Urls_MouseDown_MouseMove(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TextBox_Url_Normal.MouseDown,
                TextBox_Url_SmallThumbnail.MouseDown,
                TextBox_Url_MediumThumbnail.MouseDown,
                TextBox_Url_LargeThumbnail.MouseDown,
                TextBox_Url_HugeThumbnail.MouseDown,
                TextBox_BBCode_Normal.MouseDown,
                TextBox_BBCode_Thumbnail.MouseDown,
                TextBox_Url_Normal.MouseMove,
                TextBox_Url_SmallThumbnail.MouseMove,
                TextBox_Url_MediumThumbnail.MouseMove,
                TextBox_Url_LargeThumbnail.MouseMove,
                TextBox_Url_HugeThumbnail.MouseMove,
                TextBox_BBCode_Normal.MouseMove,
                TextBox_BBCode_Thumbnail.MouseMove

            DirectCast(sender, TextBox).SelectAll()

        End Sub

        ' ''' <summary>
        ' ''' Handles the OnBadImageFormat event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnBadImageFormat(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnBadImageFormat

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show(String.Format("Imgur service has thrown a 'Bad Image Format' error. (StatusCode: {0})", CStr(e)),
        '                    Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ' ''' <summary>
        ' ''' Handles the OnAccessForbidden event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnAccessForbidden(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnAccessForbidden

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show(String.Format("Imgur service has thrown an 'Access Forbidden' error. (StatusCode: {0})", CStr(e)),
        '                    Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ' ''' <summary>
        ' ''' Handles the OnAuthorizationFailed event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnAuthorizationFailed(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnAuthorizationFailed

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show(String.Format("Imgur service has thrown an 'Authorization Failed' error. (StatusCode: {0})", CStr(e)),
        '                    Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ' ''' <summary>
        ' ''' Handles the OnInternalServerError event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnInternalServerError(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnInternalServerError

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show(String.Format("Imgur service has thrown an Internal Server error. (StatusCode: {0})", CStr(e)),
        '                    Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ' ''' <summary>
        ' ''' Handles the OnBadImageFormat event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnPageIsNotFound(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnPageIsNotFound

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show(String.Format("Imgur service has thrown a 'Page Not Found' error. (StatusCode: {0})", CStr(e)),
        '                    Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ' ''' <summary>
        ' ''' Handles the OnUploadRateLimitError event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnUploadRateLimitError(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnUploadRateLimitError

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show(String.Format("Imgur service has thrown an 'Upload Rate-Limit' error. (StatusCode: {0})", CStr(e)),
        '                    Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ' ''' <summary>
        ' ''' Handles the OnUnknownError event of the imgurClient.
        ' ''' </summary>
        ' ''' <param name="sender">The source of the event.</param>
        ' ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        'Private Sub ImgurClient_OnUnknownError(ByVal sender As Object, ByVal e As ImgurAPI.ImgurStatus) _
        'Handles imgurClient.OnUnknownError

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

        '    MessageBox.Show("Imgur service has thrown an unknown error.", Me.Text,
        '                    MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        'End Sub

        ''' <summary>
        ''' Handles the OnAsyncError event of the imgurClient.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="imgurAPI.imgurStatus"/> instance containing the event data.</param>
        Private Sub ImgurClient_OnAsyncError(ByVal sender As Object, ByVal e As Exception) _
        Handles imgurClient.OnAsyncError

            Me.InvokeControl(Me, Sub(x As Form) x.Enabled = False)

            MessageBox.Show(e.Message, Me.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Error)

            Me.InvokeControl(Me.CButton_Abort, Sub(x As CButton) x.Hide())
            Me.InvokeControl(Me.CButton_BrowseImage, Sub(x As CButton) x.Show())
            Me.InvokeControl(Me, Sub(x As Form) x.Enabled = True)

        End Sub

        ''' <summary>
        ''' Handles the OnAsyncSuccess event of the imgurClient.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="ImgurImage"/> instance containing the event data.</param>
        Private Sub ImgurClient_OnAsyncSuccess(ByVal sender As Object, ByVal e As ImgurImage) _
        Handles imgurClient.OnAsyncSuccess

            Me.InvokeControl(Me.CButton_BrowseImage, Sub(x As CButton)
                                                         x.Show()
                                                     End Sub)

            Me.InvokeControl(Me.CButton_Abort, Sub(x As CButton)
                                                   x.Hide()
                                               End Sub)

            Me.InvokeControl(Me.Panel_Urls, Sub(x As Panel) x.Enabled = True)

        End Sub

#End Region

#Region " Methods "

        ''' <summary>
        ''' Reads the INI file to retrieve and set the 'ClientId' and 'ClientSecret'.
        ''' </summary>
        Private Sub SetClientIds()

            Select Case IniFileManager.File.Exist()

                Case True
                    Me.imgurClient.ClientId = CStr(IniFileManager.Key.Get("ClientId", "", "[API]"))
                    Me.imgurClient.ClientSecret = CStr(IniFileManager.Key.Get("ClientSecret", "", "[API]"))

                Case False
                    MessageBox.Show(String.Format("'{0}' file not found",
                                                  Path.GetFileName(IniFileManager.FilePath)),
                                    Me.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End

            End Select

            If String.IsNullOrEmpty(Me.imgurClient.ClientId) Then
                MessageBox.Show("ClientId is empty",
                                Me.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                End

            ElseIf String.IsNullOrEmpty(Me.imgurClient.ClientSecret) Then
                MessageBox.Show("ClientSecret is empty",
                                Me.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                End

            End If

        End Sub

        ''' <summary>
        ''' Gets a thumbnail from the specified image.
        ''' </summary>
        ''' <param name="filePath">The image filepath.</param>
        ''' <param name="width">The thumbnail width.</param>
        ''' <param name="height">The thumbnail height.</param>
        ''' <returns>The thumbnail.</returns>
        Private Function GetThumbnail(ByVal filePath As String,
                                      ByVal width As Integer,
                                      ByVal height As Integer) As Image


            Using bmp As New Bitmap(filePath)
                Return ImageTools.Resize(bmp, New Size(width, height), keepAspectRatio:=True)
                ' Return bmp.GetThumbnailImage(width, height, Nothing, IntPtr.Zero)
            End Using

        End Function

        ''' <summary>
        ''' Loads an image from commandline or Drag-Drop.
        ''' </summary>
        ''' <param name="ImageFile">The image file.</param>
        Private Sub LoadImage(ByVal imageFile As FileInfo)

            Me.Enabled = False

            If File.Exists(imageFile.FullName) _
            AndAlso supportedFormats.Contains(imageFile.Extension.ToUpper) Then

                Try
                    Me.PictureBox_Logo.BackgroundImage = Me.GetThumbnail(imageFile.FullName,
                                                         Me.PictureBox_Logo.Width,
                                                         Me.PictureBox_Logo.Height)

                Catch ex As Exception
                    ' Do nothing with corrupted images, also because PDF is not an image.
                    Me.PictureBox_Logo.BackgroundImage = My.Resources.Logo

                End Try

                Dim compressFlag As Boolean = False
                Dim imgFormat As ImageFormat = Nothing
                Dim targetsize As Long = 0

                Select Case imageFile.Extension.ToLower

                    Case "gif", "png"
                        If imageFile.Length > 2097152L Then
                            If MessageBox.Show("Maximum filesize for GIf and PNG is 2 MB." &
                                            Environment.NewLine &
                                            Environment.NewLine &
                                            "Would you like to let ImgurUploader to compress and/or resize the image automatically?", Me.Text,
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.OK Then

                                compressFlag = True
                                targetsize = 2097152L

                                Select Case imageFile.Extension.ToLower

                                    Case "gif"
                                        imgFormat = ImageFormat.Gif

                                    Case "png"
                                        imgFormat = ImageFormat.Png

                                End Select

                            Else
                                Exit Sub

                            End If

                        End If

                    Case Else
                        If imageFile.Length > 10485760L Then
                            If MessageBox.Show("Maximum filesize for this format is 10 MB." &
                                            Environment.NewLine &
                                            Environment.NewLine &
                                            "Would you like to let ImgurUploader to compress and/or resize the image automatically?", Me.Text,
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.OK Then

                                compressFlag = True
                                targetsize = 10485760L

                                Select Case imageFile.Extension.ToLower

                                    Case "bmp"
                                        imgFormat = ImageFormat.Bmp

                                    Case "jpg", "jpeg"
                                        imgFormat = ImageFormat.Jpeg

                                    Case "tif", "tiff"
                                        imgFormat = ImageFormat.Tiff

                                    Case "apng", "pdf", "xcf"
                                        MessageBox.Show("Image compression for this format is not supported.",
                                                        Me.Text,
                                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        Exit Sub

                                End Select

                            Else
                                Exit Sub

                            End If

                        End If

                End Select

                Dim compressedFile As String = Path.ChangeExtension(Path.GetTempFileName, imageFile.Extension)
                If compressFlag Then

                    WorkingForm.Show()

                    Try
                        ImageTools.CompressImage(imageFile.FullName, compressedFile, imgFormat, targetsize)

                    Catch ex As Exception
                        MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub

                    Finally
                        WorkingForm.Hide()
                        Me.Enabled = True

                    End Try

                End If

                Me.Enabled = True
                Me.CButton_BrowseImage.Hide()
                Me.CButton_Abort.Show()

                Me.uploadTaskCTSource = New CancellationTokenSource
                Me.uploadTaskCT = Me.uploadTaskCTSource.Token
                Me.uploadTask = Task.Factory.StartNew(Sub()
                                                          Me.UploadImage(If(Not compressFlag, imageFile.FullName, compressedFile), uploadTaskCT)
                                                      End Sub, uploadTaskCT)

            Else
                MessageBox.Show("Unsupported file format.",
                                Me.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.Enabled = True

            End If

        End Sub

        ''' <summary>
        ''' Uploads the image to imgur and refreshes the textboxes content.
        ''' </summary>
        ''' <param name="filename">The filename.</param>
        ''' <param name="ct">The Task CancellationToken.</param>
        Private Sub UploadImage(ByVal filename As String, ByVal ct As CancellationToken)

            Me.InvokeControl(Me, Sub(x As Form) x.AllowDrop = False)
            Me.InvokeControl(Me.Panel_Urls, Sub(x As Panel) x.Enabled = False)

            Me.ToolStripStatusLabel_File.Text = String.Format("Uploading: '{0}'",
                                                              Path.GetFileName(filename))

            ' Dim flag As Boolean = False
            Dim urls As ImgurImage = Nothing

            Using uploadTaskCTSource.Token.Register(Sub() Me.imgurClient.UploadImageAsyncCancel())
                urls = Me.imgurClient.UploadImageAsync(filename)
            End Using

            If ct.IsCancellationRequested Then
                Me.ToolStripStatusLabel_File.Text = String.Format("Aborted: '{0}'", Path.GetFileName(filename))
                Exit Sub

            ElseIf urls IsNot Nothing Then

                With urls

                    Me.InvokeControl(Me.TextBox_Url_Normal, Sub(x As TextBox) x.Text = .Normal)
                    Me.InvokeControl(Me.TextBox_Url_SmallThumbnail, Sub(x As TextBox) x.Text = .SmallThumbnail)
                    Me.InvokeControl(Me.TextBox_Url_MediumThumbnail, Sub(x As TextBox) x.Text = .MediumThumbnail)
                    Me.InvokeControl(Me.TextBox_Url_LargeThumbnail, Sub(x As TextBox) x.Text = .LargeThumbnail)
                    Me.InvokeControl(Me.TextBox_Url_HugeThumbnail, Sub(x As TextBox) x.Text = .HugeThumbnail)
                    Me.InvokeControl(Me.TextBox_BBCode_Normal, Sub(x As TextBox) x.Text = String.Format("[img]{0}[/img]", .Normal))
                    Me.InvokeControl(Me.TextBox_BBCode_Thumbnail, Sub(x As TextBox) x.Text = String.Format("[url={0}][img]{1}[/img][/url]", .Normal, .LargeThumbnail))

                End With '/ urls

                Me.ToolStripStatusLabel_File.Text = String.Format("Uploaded: '{0}'",
                                                                  Path.GetFileName(filename))
                Me.ToolStripStatusLabel_File.ForeColor = Color.Gainsboro

            Else

                Me.ToolStripStatusLabel_File.Text = String.Format("Error: '{0}'",
                                                                  Path.GetFileName(filename))
                Me.ToolStripStatusLabel_File.ForeColor = Color.IndianRed

            End If '/ urls IsNot Nothing

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
        Public Sub InvokeControl(Of T As Control)(ByVal control As T, ByVal action As Action(Of T))

            If control.InvokeRequired Then
                control.Invoke(New Action(Of T, Action(Of T))(AddressOf InvokeControl),
                                    New Object() {control, action})

            Else
                action(control)

            End If

        End Sub

#End Region

    End Class

End Namespace