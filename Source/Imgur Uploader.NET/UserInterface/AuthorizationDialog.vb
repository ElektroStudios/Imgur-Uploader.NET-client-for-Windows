
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports ImgurUploader.UserInterface.CustomControls
Imports System.Threading.Tasks

#End Region

#Region " AuthorizationDialog "

Namespace UserInterface

    ''' <summary>
    ''' A dark dialog that guides the user through the Imgur authorization (OAuth 2, PIN flow):
    ''' the user opens the Imgur page, allows the application, and pastes the PIN that Imgur shows.
    ''' <para></para>
    ''' The dialog returns <see cref="DialogResult.OK"/> only when the application was authorized;
    ''' in that case, <see cref="ImgurAPI.RefreshToken"/> of the client is set.
    ''' </summary>
    Public NotInheritable Class AuthorizationDialog
        Inherits Form

#Region " Constants "

        ''' <summary>
        ''' The width, in pixels, of the content of the dialog.
        ''' </summary>
        Private Const ContentWidth As Integer = 440

        ''' <summary>
        ''' The width, in pixels, of the buttons.
        ''' </summary>
        Private Const ButtonWidth As Integer = 110

#End Region

#Region " Fields "

        ''' <summary>
        ''' The client that performs the authorization.
        ''' </summary>
        Private ReadOnly client As ImgurAPI

        ''' <summary>
        ''' The url of the Imgur page where the user allows the application.
        ''' </summary>
        Private ReadOnly authorizationUrl As String

        ''' <summary>
        ''' The text box where the user pastes the PIN.
        ''' </summary>
        Private ReadOnly textBoxPin As TextBox

        ''' <summary>
        ''' The label that shows the errors.
        ''' </summary>
        Private ReadOnly labelError As Label

        ''' <summary>
        ''' The button that opens the Imgur authorization page.
        ''' </summary>
        Private ReadOnly buttonOpenPage As DropDownActionButton

        ''' <summary>
        ''' The button that sends the PIN to Imgur.
        ''' </summary>
        Private ReadOnly buttonAuthorize As DropDownActionButton

        ''' <summary>
        ''' The button that closes the dialog without authorizing.
        ''' </summary>
        Private ReadOnly buttonCancel As DropDownActionButton

        ''' <summary>
        ''' Indicates whether the PIN is being sent to Imgur (the dialog cannot be closed meanwhile).
        ''' </summary>
        Private isBusy As Boolean

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AuthorizationDialog"/> class.
        ''' </summary>
        ''' <param name="client">The client that performs the authorization. Its application credentials must be set.</param>
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="client"/> is <see langword="Nothing"/>.
        ''' </exception>
        Public Sub New(client As ImgurAPI)

            MyBase.New()

            If client Is Nothing Then
                Throw New ArgumentNullException(NameOf(client))
            End If

            Me.client = client
            Me.authorizationUrl = ImgurAPI.GetAuthorizationUrl(client.ClientId)

            Dim textBoxBackColor As Color = Color.FromArgb(37, 37, 38)

            Me.SuspendLayout()

            Me.Font = New Font("Segoe UI", 9.0F)
            Me.BackColor = Color.FromArgb(26, 26, 26)
            Me.ForeColor = Color.Gainsboro
            Me.Text = "Sign in to Imgur"
            Me.FormBorderStyle = FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.ShowInTaskbar = False
            Me.ShowIcon = False
            Me.StartPosition = FormStartPosition.CenterParent
            Me.KeyPreview = True
            Me.AutoSize = True
            Me.AutoSizeMode = AutoSizeMode.GrowAndShrink
            Me.Padding = New Padding(16)

            ' Page url (read-only, so the user can copy it if the browser cannot be opened) and the button to open it.
            Dim textBoxUrl As New TextBox() With {
                .BackColor = textBoxBackColor,
                .BorderStyle = BorderStyle.FixedSingle,
                .ForeColor = Color.Gainsboro,
                .Location = New Point(0, 3),
                .ReadOnly = True,
                .TabStop = False,
                .Text = Me.authorizationUrl,
                .Width = ContentWidth - ButtonWidth - 6
            }

            Me.buttonOpenPage = AuthorizationDialog.CreateButton("Open page", ButtonWidth)
            Me.buttonOpenPage.Location = New Point(ContentWidth - ButtonWidth, 0)
            Me.buttonOpenPage.TabIndex = 0

            Dim panelUrl As New Panel() With {
                .BackColor = Color.Transparent,
                .Margin = New Padding(0, 0, 0, 12),
                .Size = New Size(ContentWidth, 30)
            }
            panelUrl.Controls.Add(textBoxUrl)
            panelUrl.Controls.Add(Me.buttonOpenPage)

            ' PIN.
            Me.textBoxPin = New TextBox() With {
                .BackColor = textBoxBackColor,
                .BorderStyle = BorderStyle.FixedSingle,
                .ForeColor = Color.Gainsboro,
                .Margin = New Padding(0, 0, 0, 8),
                .MaxLength = 64,
                .TabIndex = 1,
                .Width = ContentWidth
            }

            ' Error message.
            Me.labelError = AuthorizationDialog.CreateLabel(String.Empty)
            Me.labelError.ForeColor = Color.IndianRed
            Me.labelError.Margin = New Padding(0, 0, 0, 8)
            Me.labelError.Visible = False

            ' Buttons.
            Me.buttonAuthorize = AuthorizationDialog.CreateButton("Authorize", ButtonWidth)
            Me.buttonAuthorize.ButtonColor = Color.FromArgb(134, 197, 31)
            Me.buttonAuthorize.ForeColor = Color.Black
            Me.buttonAuthorize.Margin = New Padding(8, 0, 0, 0)
            Me.buttonAuthorize.TabIndex = 2

            Me.buttonCancel = AuthorizationDialog.CreateButton("Cancel", ButtonWidth)
            Me.buttonCancel.Margin = New Padding(0)
            Me.buttonCancel.TabIndex = 3

            Dim panelButtons As New FlowLayoutPanel() With {
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                .BackColor = Color.Transparent,
                .FlowDirection = FlowDirection.RightToLeft,
                .Margin = New Padding(0)
            }
            panelButtons.Controls.Add(Me.buttonCancel)
            panelButtons.Controls.Add(Me.buttonAuthorize)

            ' Layout: one auto-sized row per element.
            Dim table As New TableLayoutPanel() With {
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .BackColor = Color.Transparent,
                .ColumnCount = 1,
                .Dock = DockStyle.Fill
            }
            table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))

            Dim introLabel As Label = AuthorizationDialog.CreateLabel(
                "To upload images to your Imgur account, Imgur Uploader.NET needs your permission. " &
                "You only have to do this once.")
            introLabel.Margin = New Padding(0, 0, 0, 12)

            Dim step1Label As Label = AuthorizationDialog.CreateLabel(
                "1. Open the Imgur page below, sign in if needed and click ""Allow"". Imgur will then show you a PIN.")
            step1Label.Margin = New Padding(0, 0, 0, 6)

            Dim step2Label As Label = AuthorizationDialog.CreateLabel("2. Paste the PIN here and click ""Authorize"":")
            step2Label.Margin = New Padding(0, 0, 0, 6)

            AuthorizationDialog.AddRow(table, introLabel)
            AuthorizationDialog.AddRow(table, step1Label)
            AuthorizationDialog.AddRow(table, panelUrl)
            AuthorizationDialog.AddRow(table, step2Label)
            AuthorizationDialog.AddRow(table, Me.textBoxPin)
            AuthorizationDialog.AddRow(table, Me.labelError)
            AuthorizationDialog.AddRow(table, panelButtons)

            Me.Controls.Add(table)

            AddHandler Me.buttonOpenPage.MainAreaClick, AddressOf Me.ButtonOpenPage_Click
            AddHandler Me.buttonAuthorize.MainAreaClick, AddressOf Me.ButtonAuthorize_Click
            AddHandler Me.buttonCancel.MainAreaClick, AddressOf Me.ButtonCancel_Click
            AddHandler Me.textBoxPin.KeyDown, AddressOf Me.TextBoxPin_KeyDown

            Me.ResumeLayout(False)
            Me.PerformLayout()

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
        ''' Raises the Shown event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnShown(e As EventArgs)

            MyBase.OnShown(e)
            Me.buttonOpenPage.Focus()

        End Sub

        ''' <summary>
        ''' Raises the KeyDown event. The Escape key cancels the dialog.
        ''' </summary>
        ''' <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

            MyBase.OnKeyDown(e)

            If (e.KeyCode = Keys.Escape) AndAlso Not Me.isBusy Then
                e.Handled = True
                Me.DialogResult = DialogResult.Cancel
            End If

        End Sub

        ''' <summary>
        ''' Raises the FormClosing event. The dialog cannot be closed while the PIN is being sent to Imgur.
        ''' </summary>
        ''' <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

            If Me.isBusy Then
                e.Cancel = True
            End If

            MyBase.OnFormClosing(e)

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the MainAreaClick event of the "Open page" button.
        ''' Opens the Imgur authorization page in the default browser.
        ''' </summary>
        Private Sub ButtonOpenPage_Click(sender As Object, e As EventArgs)

            Try
                Process.Start(New ProcessStartInfo(Me.authorizationUrl) With {.UseShellExecute = True})
                Me.textBoxPin.Focus()

            Catch ex As Exception When TypeOf ex Is System.ComponentModel.Win32Exception OrElse TypeOf ex Is InvalidOperationException
                Me.ShowError("The browser could not be opened. Copy the address above and open it manually.")

            End Try

        End Sub

        ''' <summary>
        ''' Handles the MainAreaClick event of the "Authorize" button.
        ''' </summary>
        Private Sub ButtonAuthorize_Click(sender As Object, e As EventArgs)

            Me.Authorize()

        End Sub

        ''' <summary>
        ''' Handles the MainAreaClick event of the "Cancel" button.
        ''' </summary>
        Private Sub ButtonCancel_Click(sender As Object, e As EventArgs)

            Me.DialogResult = DialogResult.Cancel

        End Sub

        ''' <summary>
        ''' Handles the KeyDown event of the PIN text box. The Enter key sends the PIN.
        ''' </summary>
        Private Sub TextBoxPin_KeyDown(sender As Object, e As KeyEventArgs)

            If e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
                Me.Authorize()
            End If

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Sends the PIN to Imgur without blocking the user interface.
        ''' If it succeeds, the dialog is closed with <see cref="DialogResult.OK"/>;
        ''' otherwise, the error is shown and the user can try again.
        ''' </summary>
        Private Async Sub Authorize()

            If Me.isBusy Then
                Return
            End If

            Dim pin As String = Me.textBoxPin.Text.Trim()

            If pin.Length = 0 Then
                Me.ShowError("Paste the PIN that Imgur showed you.")
                Me.textBoxPin.Focus()
                Return
            End If

            Me.SetBusy(True)

            Try
                Await Task.Run(Sub() Me.client.AuthorizeWithPin(pin))
                Me.SetBusy(False)
                Me.DialogResult = DialogResult.OK

            Catch ex As ImgurAuthorizationException
                Me.SetBusy(False)
                Me.ShowError($"{ex.Message} Open the page again to get a new PIN (a PIN can be used only once).")
                Me.textBoxPin.SelectAll()
                Me.textBoxPin.Focus()

            Catch ex As Exception
                Me.SetBusy(False)
                Me.ShowError(ex.Message)
                Me.textBoxPin.Focus()

            End Try

        End Sub

        ''' <summary>
        ''' Enables or disables the controls while the PIN is being sent to Imgur.
        ''' </summary>
        ''' <param name="busy"><see langword="True"/> while the PIN is being sent.</param>
        Private Sub SetBusy(busy As Boolean)

            Me.isBusy = busy

            If busy Then
                Me.labelError.Visible = False
            End If

            Me.textBoxPin.Enabled = Not busy
            Me.buttonOpenPage.Enabled = Not busy
            Me.buttonCancel.Enabled = Not busy
            Me.buttonAuthorize.Enabled = Not busy
            Me.buttonAuthorize.Text = If(busy, "Authorizing...", "Authorize")
            Me.UseWaitCursor = busy

        End Sub

        ''' <summary>
        ''' Shows an error message in the dialog.
        ''' </summary>
        ''' <param name="message">The message.</param>
        Private Sub ShowError(message As String)

            Me.labelError.Text = message
            Me.labelError.Visible = True

        End Sub

        ''' <summary>
        ''' Creates a label whose text wraps at the width of the content of the dialog.
        ''' </summary>
        ''' <param name="text">The text.</param>
        ''' <returns>The label.</returns>
        Private Shared Function CreateLabel(text As String) As Label

            Return New Label() With {
                .AutoSize = True,
                .BackColor = Color.Transparent,
                .ForeColor = Color.Gainsboro,
                .MaximumSize = New Size(ContentWidth, 0),
                .Text = text
            }

        End Function

        ''' <summary>
        ''' Creates a flat button that has no drop-down arrow.
        ''' </summary>
        ''' <param name="text">The text.</param>
        ''' <param name="width">The width, in pixels.</param>
        ''' <returns>The button.</returns>
        Private Shared Function CreateButton(text As String, width As Integer) As DropDownActionButton

            Return New DropDownActionButton() With {
                .BackColor = Color.FromArgb(26, 26, 26),
                .CenterText = True,
                .Cursor = Cursors.Hand,
                .ForeColor = Color.Gainsboro,
                .HasDropDown = False,
                .Size = New Size(width, 30),
                .Text = text
            }

        End Function

        ''' <summary>
        ''' Adds a control in a new auto-sized row at the end of the table.
        ''' </summary>
        ''' <param name="table">The table.</param>
        ''' <param name="control">The control.</param>
        Private Shared Sub AddRow(table As TableLayoutPanel, control As Control)

            table.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            table.RowCount = table.RowStyles.Count
            table.Controls.Add(control, 0, table.RowCount - 1)

        End Sub

#End Region

    End Class

End Namespace

#End Region
