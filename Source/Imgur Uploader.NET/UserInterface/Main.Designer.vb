Namespace UserInterface

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Main
    Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
            Dim CBlendItems1 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
            Dim CBlendItems2 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
            Dim CBlendItems3 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
            Me.OpenFileDialog_BrowseImage = New System.Windows.Forms.OpenFileDialog()
            Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
            Me.ToolStripStatusLabel_File = New System.Windows.Forms.ToolStripStatusLabel()
            Me.ToolStripStatusLabel_GitHub = New System.Windows.Forms.ToolStripStatusLabel()
            Me.CButton_BrowseImage = New CButtonLib.CButton()
            Me.CButton_Upload = New CButtonLib.CButton()
            Me.CButton_Abort = New CButtonLib.CButton()
            Me.TableLayoutPanel_Buttons = New System.Windows.Forms.TableLayoutPanel()
            Me.Panel_Upload = New System.Windows.Forms.Panel()
            Me.PictureBox_Logo = New System.Windows.Forms.PictureBox()
            Me.Label_FileName = New System.Windows.Forms.Label()
            Me.Label_FileDetails = New System.Windows.Forms.Label()
            Me.TableLayoutPanel_Selectors = New System.Windows.Forms.TableLayoutPanel()
            Me.Selector_Size = New ImgurUploader.UserInterface.CustomControls.DropDownSelector()
            Me.Selector_Format = New ImgurUploader.UserInterface.CustomControls.DropDownSelector()
            Me.TableLayoutPanel_Result = New System.Windows.Forms.TableLayoutPanel()
            Me.TextBox_Result = New System.Windows.Forms.TextBox()
            Me.Button_Copy = New ImgurUploader.UserInterface.CustomControls.DropDownActionButton()
            Me.Button_Open = New ImgurUploader.UserInterface.CustomControls.DropDownActionButton()
            Me.ToolTip_Actions = New System.Windows.Forms.ToolTip(Me.components)
            Me.Timer_StatusRestore = New System.Windows.Forms.Timer(Me.components)
            Me.StatusStrip1.SuspendLayout()
            Me.TableLayoutPanel_Buttons.SuspendLayout()
            Me.Panel_Upload.SuspendLayout()
            CType(Me.PictureBox_Logo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel_Selectors.SuspendLayout()
            Me.TableLayoutPanel_Result.SuspendLayout()
            Me.SuspendLayout()
            '
            'OpenFileDialog_BrowseImage
            '
            Me.OpenFileDialog_BrowseImage.Filter = resources.GetString("OpenFileDialog_BrowseImage.Filter")
            Me.OpenFileDialog_BrowseImage.ReadOnlyChecked = True
            Me.OpenFileDialog_BrowseImage.RestoreDirectory = True
            Me.OpenFileDialog_BrowseImage.SupportMultiDottedExtensions = True
            Me.OpenFileDialog_BrowseImage.Title = "Browse an image to upload"
            '
            'StatusStrip1
            '
            Me.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel_File, Me.ToolStripStatusLabel_GitHub})
            Me.StatusStrip1.Location = New System.Drawing.Point(0, 407)
            Me.StatusStrip1.Name = "StatusStrip1"
            Me.StatusStrip1.Size = New System.Drawing.Size(560, 22)
            Me.StatusStrip1.SizingGrip = False
            Me.StatusStrip1.TabIndex = 7
            Me.StatusStrip1.Text = "StatusStrip1"
            '
            'ToolStripStatusLabel_File
            '
            Me.ToolStripStatusLabel_File.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripStatusLabel_File.ForeColor = System.Drawing.Color.Gainsboro
            Me.ToolStripStatusLabel_File.Name = "ToolStripStatusLabel_File"
            Me.ToolStripStatusLabel_File.Size = New System.Drawing.Size(500, 17)
            Me.ToolStripStatusLabel_File.Spring = True
            Me.ToolStripStatusLabel_File.Text = "Ready"
            Me.ToolStripStatusLabel_File.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'ToolStripStatusLabel_GitHub
            '
            Me.ToolStripStatusLabel_GitHub.ActiveLinkColor = System.Drawing.Color.White
            Me.ToolStripStatusLabel_GitHub.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.ToolStripStatusLabel_GitHub.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripStatusLabel_GitHub.IsLink = True
            Me.ToolStripStatusLabel_GitHub.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
            Me.ToolStripStatusLabel_GitHub.LinkColor = System.Drawing.Color.Gainsboro
            Me.ToolStripStatusLabel_GitHub.Name = "ToolStripStatusLabel_GitHub"
            Me.ToolStripStatusLabel_GitHub.Size = New System.Drawing.Size(45, 17)
            Me.ToolStripStatusLabel_GitHub.Text = "GitHub"
            Me.ToolStripStatusLabel_GitHub.ToolTipText = "https://github.com/ElektroStudios/Imgur-Uploader.NET-client-for-Windows"
            Me.ToolStripStatusLabel_GitHub.VisitedLinkColor = System.Drawing.Color.Gainsboro
            '
            'CButton_BrowseImage
            '
            Me.CButton_BrowseImage.BorderColor = System.Drawing.Color.Black
            CBlendItems1.iColor = New System.Drawing.Color() {System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))}
            CBlendItems1.iPoint = New Single() {0!, 0.5!, 1.0!}
            Me.CButton_BrowseImage.ColorFillBlend = CBlendItems1
            Me.CButton_BrowseImage.Corners.All = 2
            Me.CButton_BrowseImage.Corners.LowerLeft = 2
            Me.CButton_BrowseImage.Corners.LowerRight = 2
            Me.CButton_BrowseImage.Corners.UpperLeft = 2
            Me.CButton_BrowseImage.Corners.UpperRight = 2
            Me.CButton_BrowseImage.Cursor = System.Windows.Forms.Cursors.Hand
            Me.CButton_BrowseImage.DesignerSelected = False
            Me.CButton_BrowseImage.Dock = System.Windows.Forms.DockStyle.Fill
            Me.CButton_BrowseImage.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Bold)
            Me.CButton_BrowseImage.ImageIndex = 0
            Me.CButton_BrowseImage.Location = New System.Drawing.Point(3, 3)
            Me.CButton_BrowseImage.Name = "CButton_BrowseImage"
            Me.CButton_BrowseImage.Size = New System.Drawing.Size(265, 32)
            Me.CButton_BrowseImage.TabIndex = 0
            Me.CButton_BrowseImage.Text = "Browse an image..."
            Me.CButton_BrowseImage.TextShadow = System.Drawing.Color.Black
            Me.CButton_BrowseImage.TextShadowShow = False
            '
            'CButton_Upload
            '
            Me.CButton_Upload.BorderColor = System.Drawing.Color.Black
            CBlendItems2.iColor = New System.Drawing.Color() {System.Drawing.Color.FromArgb(CType(CType(134, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(31, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(134, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(31, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(134, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(31, Byte), Integer))}
            CBlendItems2.iPoint = New Single() {0!, 0.5!, 1.0!}
            Me.CButton_Upload.ColorFillBlend = CBlendItems2
            Me.CButton_Upload.Corners.All = 2
            Me.CButton_Upload.Corners.LowerLeft = 2
            Me.CButton_Upload.Corners.LowerRight = 2
            Me.CButton_Upload.Corners.UpperLeft = 2
            Me.CButton_Upload.Corners.UpperRight = 2
            Me.CButton_Upload.Cursor = System.Windows.Forms.Cursors.Hand
            Me.CButton_Upload.DesignerSelected = False
            Me.CButton_Upload.Dock = System.Windows.Forms.DockStyle.Fill
            Me.CButton_Upload.Enabled = False
            Me.CButton_Upload.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Bold)
            Me.CButton_Upload.ForeColor = System.Drawing.Color.Black
            Me.CButton_Upload.ImageIndex = 0
            Me.CButton_Upload.Location = New System.Drawing.Point(3, 3)
            Me.CButton_Upload.Name = "CButton_Upload"
            Me.CButton_Upload.Size = New System.Drawing.Size(265, 32)
            Me.CButton_Upload.TabIndex = 1
            Me.CButton_Upload.Text = "Upload image"
            Me.CButton_Upload.TextShadow = System.Drawing.Color.Black
            Me.CButton_Upload.TextShadowShow = False
            '
            'CButton_Abort
            '
            Me.CButton_Abort.BorderColor = System.Drawing.Color.Black
            CBlendItems3.iColor = New System.Drawing.Color() {System.Drawing.Color.Crimson, System.Drawing.Color.Crimson, System.Drawing.Color.Crimson}
            CBlendItems3.iPoint = New Single() {0!, 0.5!, 1.0!}
            Me.CButton_Abort.ColorFillBlend = CBlendItems3
            Me.CButton_Abort.Corners.All = 2
            Me.CButton_Abort.Corners.LowerLeft = 2
            Me.CButton_Abort.Corners.LowerRight = 2
            Me.CButton_Abort.Corners.UpperLeft = 2
            Me.CButton_Abort.Corners.UpperRight = 2
            Me.CButton_Abort.Cursor = System.Windows.Forms.Cursors.Hand
            Me.CButton_Abort.DesignerSelected = False
            Me.CButton_Abort.Dock = System.Windows.Forms.DockStyle.Fill
            Me.CButton_Abort.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Bold)
            Me.CButton_Abort.ForeColor = System.Drawing.Color.WhiteSmoke
            Me.CButton_Abort.ImageIndex = 0
            Me.CButton_Abort.Location = New System.Drawing.Point(3, 3)
            Me.CButton_Abort.Name = "CButton_Abort"
            Me.CButton_Abort.Size = New System.Drawing.Size(265, 32)
            Me.CButton_Abort.TabIndex = 1
            Me.CButton_Abort.Text = "Abort Upload"
            Me.CButton_Abort.TextShadow = System.Drawing.Color.Black
            Me.CButton_Abort.TextShadowShow = False
            Me.CButton_Abort.Visible = False
            '
            'TableLayoutPanel_Buttons
            '
            Me.TableLayoutPanel_Buttons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TableLayoutPanel_Buttons.BackColor = System.Drawing.Color.Transparent
            Me.TableLayoutPanel_Buttons.ColumnCount = 2
            Me.TableLayoutPanel_Buttons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel_Buttons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel_Buttons.Controls.Add(Me.CButton_BrowseImage, 0, 0)
            Me.TableLayoutPanel_Buttons.Controls.Add(Me.Panel_Upload, 1, 0)
            Me.TableLayoutPanel_Buttons.Location = New System.Drawing.Point(9, 285)
            Me.TableLayoutPanel_Buttons.Name = "TableLayoutPanel_Buttons"
            Me.TableLayoutPanel_Buttons.RowCount = 1
            Me.TableLayoutPanel_Buttons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel_Buttons.Size = New System.Drawing.Size(542, 38)
            Me.TableLayoutPanel_Buttons.TabIndex = 3
            '
            'Panel_Upload
            '
            Me.Panel_Upload.BackColor = System.Drawing.Color.Transparent
            Me.Panel_Upload.Controls.Add(Me.CButton_Upload)
            Me.Panel_Upload.Controls.Add(Me.CButton_Abort)
            Me.Panel_Upload.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel_Upload.Location = New System.Drawing.Point(271, 0)
            Me.Panel_Upload.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel_Upload.Name = "Panel_Upload"
            Me.Panel_Upload.Padding = New System.Windows.Forms.Padding(3)
            Me.Panel_Upload.Size = New System.Drawing.Size(271, 38)
            Me.Panel_Upload.TabIndex = 1
            '
            'PictureBox_Logo
            '
            Me.PictureBox_Logo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PictureBox_Logo.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
            Me.PictureBox_Logo.Image = Global.ImgurUploader.My.Resources.Resources.Logo
            Me.PictureBox_Logo.Location = New System.Drawing.Point(12, 12)
            Me.PictureBox_Logo.Name = "PictureBox_Logo"
            Me.PictureBox_Logo.Size = New System.Drawing.Size(536, 229)
            Me.PictureBox_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.PictureBox_Logo.TabIndex = 0
            Me.PictureBox_Logo.TabStop = False
            '
            'Label_FileName
            '
            Me.Label_FileName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label_FileName.AutoEllipsis = True
            Me.Label_FileName.BackColor = System.Drawing.Color.Transparent
            Me.Label_FileName.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
            Me.Label_FileName.ForeColor = System.Drawing.Color.Gainsboro
            Me.Label_FileName.Location = New System.Drawing.Point(12, 245)
            Me.Label_FileName.Name = "Label_FileName"
            Me.Label_FileName.Size = New System.Drawing.Size(536, 18)
            Me.Label_FileName.TabIndex = 4
            Me.Label_FileName.Text = "No image selected"
            Me.Label_FileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label_FileDetails
            '
            Me.Label_FileDetails.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label_FileDetails.AutoEllipsis = True
            Me.Label_FileDetails.BackColor = System.Drawing.Color.Transparent
            Me.Label_FileDetails.Font = New System.Drawing.Font("Segoe UI", 9.0!)
            Me.Label_FileDetails.ForeColor = System.Drawing.Color.DarkGray
            Me.Label_FileDetails.Location = New System.Drawing.Point(12, 263)
            Me.Label_FileDetails.Name = "Label_FileDetails"
            Me.Label_FileDetails.Size = New System.Drawing.Size(536, 18)
            Me.Label_FileDetails.TabIndex = 5
            Me.Label_FileDetails.Text = "Drag and drop an image onto the window, or click Browse."
            Me.Label_FileDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'TableLayoutPanel_Selectors
            '
            Me.TableLayoutPanel_Selectors.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TableLayoutPanel_Selectors.BackColor = System.Drawing.Color.Transparent
            Me.TableLayoutPanel_Selectors.ColumnCount = 2
            Me.TableLayoutPanel_Selectors.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel_Selectors.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel_Selectors.Controls.Add(Me.Selector_Size, 0, 0)
            Me.TableLayoutPanel_Selectors.Controls.Add(Me.Selector_Format, 1, 0)
            Me.TableLayoutPanel_Selectors.Location = New System.Drawing.Point(9, 326)
            Me.TableLayoutPanel_Selectors.Name = "TableLayoutPanel_Selectors"
            Me.TableLayoutPanel_Selectors.RowCount = 1
            Me.TableLayoutPanel_Selectors.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel_Selectors.Size = New System.Drawing.Size(542, 36)
            Me.TableLayoutPanel_Selectors.TabIndex = 1
            '
            'Selector_Size
            '
            Me.Selector_Size.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
            Me.Selector_Size.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Selector_Size.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Selector_Size.ForeColor = System.Drawing.Color.Gainsboro
            Me.Selector_Size.Location = New System.Drawing.Point(3, 3)
            Me.Selector_Size.Name = "Selector_Size"
            Me.Selector_Size.Prefix = "Size"
            Me.Selector_Size.Size = New System.Drawing.Size(265, 30)
            Me.Selector_Size.TabIndex = 0
            Me.Selector_Size.Text = "Size: "
            '
            'Selector_Format
            '
            Me.Selector_Format.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
            Me.Selector_Format.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Selector_Format.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Selector_Format.ForeColor = System.Drawing.Color.Gainsboro
            Me.Selector_Format.Location = New System.Drawing.Point(274, 3)
            Me.Selector_Format.Name = "Selector_Format"
            Me.Selector_Format.Prefix = "Format"
            Me.Selector_Format.Size = New System.Drawing.Size(265, 30)
            Me.Selector_Format.TabIndex = 1
            Me.Selector_Format.Text = "Format: "
            '
            'TableLayoutPanel_Result
            '
            Me.TableLayoutPanel_Result.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TableLayoutPanel_Result.BackColor = System.Drawing.Color.Transparent
            Me.TableLayoutPanel_Result.ColumnCount = 3
            Me.TableLayoutPanel_Result.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel_Result.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
            Me.TableLayoutPanel_Result.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
            Me.TableLayoutPanel_Result.Controls.Add(Me.TextBox_Result, 0, 0)
            Me.TableLayoutPanel_Result.Controls.Add(Me.Button_Copy, 1, 0)
            Me.TableLayoutPanel_Result.Controls.Add(Me.Button_Open, 2, 0)
            Me.TableLayoutPanel_Result.Location = New System.Drawing.Point(9, 363)
            Me.TableLayoutPanel_Result.Name = "TableLayoutPanel_Result"
            Me.TableLayoutPanel_Result.RowCount = 1
            Me.TableLayoutPanel_Result.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel_Result.Size = New System.Drawing.Size(542, 34)
            Me.TableLayoutPanel_Result.TabIndex = 2
            '
            'TextBox_Result
            '
            Me.TextBox_Result.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBox_Result.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(38, Byte), Integer))
            Me.TextBox_Result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_Result.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_Result.Location = New System.Drawing.Point(3, 7)
            Me.TextBox_Result.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
            Me.TextBox_Result.Name = "TextBox_Result"
            Me.TextBox_Result.ReadOnly = True
            Me.TextBox_Result.Size = New System.Drawing.Size(396, 20)
            Me.TextBox_Result.TabIndex = 0
            '
            'Button_Copy
            '
            Me.Button_Copy.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
            Me.Button_Copy.CenterText = True
            Me.Button_Copy.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Copy.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Button_Copy.Enabled = False
            Me.Button_Copy.ForeColor = System.Drawing.Color.Gainsboro
            Me.Button_Copy.HasDropDown = False
            Me.Button_Copy.Location = New System.Drawing.Point(405, 3)
            Me.Button_Copy.Name = "Button_Copy"
            Me.Button_Copy.Size = New System.Drawing.Size(64, 28)
            Me.Button_Copy.TabIndex = 1
            Me.Button_Copy.Text = "Copy"
            '
            'Button_Open
            '
            Me.Button_Open.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
            Me.Button_Open.CenterText = True
            Me.Button_Open.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Open.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Button_Open.Enabled = False
            Me.Button_Open.ForeColor = System.Drawing.Color.Gainsboro
            Me.Button_Open.HasDropDown = False
            Me.Button_Open.Location = New System.Drawing.Point(475, 3)
            Me.Button_Open.Name = "Button_Open"
            Me.Button_Open.Size = New System.Drawing.Size(64, 28)
            Me.Button_Open.TabIndex = 2
            Me.Button_Open.Text = "Open"
            '
            'Timer_StatusRestore
            '
            Me.Timer_StatusRestore.Interval = 2500
            '
            'Main
            '
            Me.AllowDrop = True
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(560, 429)
            Me.Controls.Add(Me.StatusStrip1)
            Me.Controls.Add(Me.TableLayoutPanel_Result)
            Me.Controls.Add(Me.TableLayoutPanel_Selectors)
            Me.Controls.Add(Me.Label_FileDetails)
            Me.Controls.Add(Me.Label_FileName)
            Me.Controls.Add(Me.PictureBox_Logo)
            Me.Controls.Add(Me.TableLayoutPanel_Buttons)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "Main"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.StatusStrip1.ResumeLayout(False)
            Me.StatusStrip1.PerformLayout()
            Me.TableLayoutPanel_Buttons.ResumeLayout(False)
            Me.Panel_Upload.ResumeLayout(False)
            CType(Me.PictureBox_Logo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel_Selectors.ResumeLayout(False)
            Me.TableLayoutPanel_Result.ResumeLayout(False)
            Me.TableLayoutPanel_Result.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents PictureBox_Logo As System.Windows.Forms.PictureBox
        Friend WithEvents OpenFileDialog_BrowseImage As System.Windows.Forms.OpenFileDialog
        Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
        Friend WithEvents ToolStripStatusLabel_File As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents ToolStripStatusLabel_GitHub As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents CButton_BrowseImage As CButtonLib.CButton
        Friend WithEvents CButton_Upload As CButtonLib.CButton
        Friend WithEvents CButton_Abort As CButtonLib.CButton
        Friend WithEvents TableLayoutPanel_Buttons As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Panel_Upload As System.Windows.Forms.Panel
        Friend WithEvents TableLayoutPanel_Selectors As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Selector_Size As Global.ImgurUploader.UserInterface.CustomControls.DropDownSelector
        Friend WithEvents Label_FileName As System.Windows.Forms.Label
        Friend WithEvents Label_FileDetails As System.Windows.Forms.Label
        Friend WithEvents Selector_Format As Global.ImgurUploader.UserInterface.CustomControls.DropDownSelector
        Friend WithEvents TableLayoutPanel_Result As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents TextBox_Result As System.Windows.Forms.TextBox
        Friend WithEvents Button_Copy As Global.ImgurUploader.UserInterface.CustomControls.DropDownActionButton
        Friend WithEvents Button_Open As Global.ImgurUploader.UserInterface.CustomControls.DropDownActionButton
        Friend WithEvents ToolTip_Actions As System.Windows.Forms.ToolTip
        Friend WithEvents Timer_StatusRestore As System.Windows.Forms.Timer
    End Class

End Namespace
