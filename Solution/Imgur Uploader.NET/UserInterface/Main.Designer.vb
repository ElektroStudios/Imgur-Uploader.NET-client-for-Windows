Imports ImgurUploader.UserInterface

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
            Dim DesignerRectTracker1 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
            Dim CBlendItems1 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
            Dim DesignerRectTracker2 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
            Dim DesignerRectTracker3 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
            Dim CBlendItems2 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
            Dim DesignerRectTracker4 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
            Me.TextBox_Url_Normal = New System.Windows.Forms.TextBox()
            Me.TextBox_Url_SmallThumbnail = New System.Windows.Forms.TextBox()
            Me.TextBox_Url_MediumThumbnail = New System.Windows.Forms.TextBox()
            Me.TextBox_Url_LargeThumbnail = New System.Windows.Forms.TextBox()
            Me.Label_Normal = New System.Windows.Forms.Label()
            Me.Label_SmallThumbnail = New System.Windows.Forms.Label()
            Me.Label_MediumThumbnail = New System.Windows.Forms.Label()
            Me.Label_LargeThumbnail = New System.Windows.Forms.Label()
            Me.Label_HugeThumbnail = New System.Windows.Forms.Label()
            Me.TextBox_Url_HugeThumbnail = New System.Windows.Forms.TextBox()
            Me.Panel_Urls = New System.Windows.Forms.Panel()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.Label_BBCode_Thumbnail = New System.Windows.Forms.Label()
            Me.Button_Url_Normal = New System.Windows.Forms.Button()
            Me.Button_Clip_Normal = New System.Windows.Forms.Button()
            Me.Button_Clip_SmallThumbnail = New System.Windows.Forms.Button()
            Me.Button_Url_SmallThumbnail = New System.Windows.Forms.Button()
            Me.Button_Clip_MediumThumbnail = New System.Windows.Forms.Button()
            Me.Button_Url_MediumThumbnail = New System.Windows.Forms.Button()
            Me.Button_Url_LargeThumbnail = New System.Windows.Forms.Button()
            Me.Button_Url_HugeThumbnail = New System.Windows.Forms.Button()
            Me.Button_Clip_LargeThumbnail = New System.Windows.Forms.Button()
            Me.Button_Clip_HugeThumbnail = New System.Windows.Forms.Button()
            Me.Label_BBCode_Normal = New System.Windows.Forms.Label()
            Me.Button_BBCode_Normal = New System.Windows.Forms.Button()
            Me.Button_BBCode_Thumbnail = New System.Windows.Forms.Button()
            Me.TextBox_BBCode_Normal = New System.Windows.Forms.TextBox()
            Me.TextBox_BBCode_Thumbnail = New System.Windows.Forms.TextBox()
            Me.OpenFileDialog_BrowseImage = New System.Windows.Forms.OpenFileDialog()
            Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
            Me.ToolStripStatusLabel_File = New System.Windows.Forms.ToolStripStatusLabel()
            Me.CButton_BrowseImage = New CButtonLib.CButton()
            Me.PictureBox_Logo = New System.Windows.Forms.PictureBox()
            Me.CButton_Abort = New CButtonLib.CButton()
            Me.Panel_Urls.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.StatusStrip1.SuspendLayout()
            CType(Me.PictureBox_Logo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TextBox_Url_Normal
            '
            Me.TextBox_Url_Normal.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_Url_Normal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_Url_Normal.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_Url_Normal.Location = New System.Drawing.Point(108, 3)
            Me.TextBox_Url_Normal.Name = "TextBox_Url_Normal"
            Me.TextBox_Url_Normal.ReadOnly = True
            Me.TextBox_Url_Normal.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_Url_Normal.TabIndex = 0
            '
            'TextBox_Url_SmallThumbnail
            '
            Me.TextBox_Url_SmallThumbnail.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_Url_SmallThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_Url_SmallThumbnail.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_Url_SmallThumbnail.Location = New System.Drawing.Point(108, 28)
            Me.TextBox_Url_SmallThumbnail.Name = "TextBox_Url_SmallThumbnail"
            Me.TextBox_Url_SmallThumbnail.ReadOnly = True
            Me.TextBox_Url_SmallThumbnail.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_Url_SmallThumbnail.TabIndex = 3
            '
            'TextBox_Url_MediumThumbnail
            '
            Me.TextBox_Url_MediumThumbnail.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_Url_MediumThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_Url_MediumThumbnail.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_Url_MediumThumbnail.Location = New System.Drawing.Point(108, 53)
            Me.TextBox_Url_MediumThumbnail.Name = "TextBox_Url_MediumThumbnail"
            Me.TextBox_Url_MediumThumbnail.ReadOnly = True
            Me.TextBox_Url_MediumThumbnail.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_Url_MediumThumbnail.TabIndex = 4
            '
            'TextBox_Url_LargeThumbnail
            '
            Me.TextBox_Url_LargeThumbnail.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_Url_LargeThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_Url_LargeThumbnail.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_Url_LargeThumbnail.Location = New System.Drawing.Point(108, 78)
            Me.TextBox_Url_LargeThumbnail.Name = "TextBox_Url_LargeThumbnail"
            Me.TextBox_Url_LargeThumbnail.ReadOnly = True
            Me.TextBox_Url_LargeThumbnail.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_Url_LargeThumbnail.TabIndex = 5
            '
            'Label_Normal
            '
            Me.Label_Normal.BackColor = System.Drawing.Color.Transparent
            Me.Label_Normal.ForeColor = System.Drawing.Color.Silver
            Me.Label_Normal.Location = New System.Drawing.Point(3, 0)
            Me.Label_Normal.Name = "Label_Normal"
            Me.Label_Normal.Size = New System.Drawing.Size(99, 20)
            Me.Label_Normal.TabIndex = 0
            Me.Label_Normal.Text = "Normal"
            Me.Label_Normal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label_SmallThumbnail
            '
            Me.Label_SmallThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Label_SmallThumbnail.ForeColor = System.Drawing.Color.Silver
            Me.Label_SmallThumbnail.Location = New System.Drawing.Point(3, 25)
            Me.Label_SmallThumbnail.Name = "Label_SmallThumbnail"
            Me.Label_SmallThumbnail.Size = New System.Drawing.Size(99, 20)
            Me.Label_SmallThumbnail.TabIndex = 3
            Me.Label_SmallThumbnail.Text = "Small Thumbnail"
            Me.Label_SmallThumbnail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label_MediumThumbnail
            '
            Me.Label_MediumThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Label_MediumThumbnail.ForeColor = System.Drawing.Color.Silver
            Me.Label_MediumThumbnail.Location = New System.Drawing.Point(3, 50)
            Me.Label_MediumThumbnail.Name = "Label_MediumThumbnail"
            Me.Label_MediumThumbnail.Size = New System.Drawing.Size(99, 20)
            Me.Label_MediumThumbnail.TabIndex = 4
            Me.Label_MediumThumbnail.Text = "Medium Thumbnail"
            Me.Label_MediumThumbnail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label_LargeThumbnail
            '
            Me.Label_LargeThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Label_LargeThumbnail.ForeColor = System.Drawing.Color.Silver
            Me.Label_LargeThumbnail.Location = New System.Drawing.Point(3, 75)
            Me.Label_LargeThumbnail.Name = "Label_LargeThumbnail"
            Me.Label_LargeThumbnail.Size = New System.Drawing.Size(99, 20)
            Me.Label_LargeThumbnail.TabIndex = 5
            Me.Label_LargeThumbnail.Text = "Large Thumbnail"
            Me.Label_LargeThumbnail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label_HugeThumbnail
            '
            Me.Label_HugeThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Label_HugeThumbnail.ForeColor = System.Drawing.Color.Silver
            Me.Label_HugeThumbnail.Location = New System.Drawing.Point(3, 100)
            Me.Label_HugeThumbnail.Name = "Label_HugeThumbnail"
            Me.Label_HugeThumbnail.Size = New System.Drawing.Size(99, 20)
            Me.Label_HugeThumbnail.TabIndex = 6
            Me.Label_HugeThumbnail.Text = "Huge Thumbnail"
            Me.Label_HugeThumbnail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'TextBox_Url_HugeThumbnail
            '
            Me.TextBox_Url_HugeThumbnail.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_Url_HugeThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_Url_HugeThumbnail.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_Url_HugeThumbnail.Location = New System.Drawing.Point(108, 103)
            Me.TextBox_Url_HugeThumbnail.Name = "TextBox_Url_HugeThumbnail"
            Me.TextBox_Url_HugeThumbnail.ReadOnly = True
            Me.TextBox_Url_HugeThumbnail.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_Url_HugeThumbnail.TabIndex = 6
            '
            'Panel_Urls
            '
            Me.Panel_Urls.BackColor = System.Drawing.Color.Transparent
            Me.Panel_Urls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.Panel_Urls.Controls.Add(Me.TableLayoutPanel1)
            Me.Panel_Urls.Enabled = False
            Me.Panel_Urls.Location = New System.Drawing.Point(12, 287)
            Me.Panel_Urls.Name = "Panel_Urls"
            Me.Panel_Urls.Size = New System.Drawing.Size(358, 185)
            Me.Panel_Urls.TabIndex = 4
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.ColumnCount = 4
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.10247!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.89753!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24.0!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38.0!))
            Me.TableLayoutPanel1.Controls.Add(Me.Label_BBCode_Thumbnail, 0, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.Label_Normal, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Url_Normal, 3, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Clip_Normal, 2, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Url_Normal, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Label_SmallThumbnail, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Label_MediumThumbnail, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.Label_LargeThumbnail, 0, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.Label_HugeThumbnail, 0, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Url_SmallThumbnail, 1, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Url_MediumThumbnail, 1, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Url_HugeThumbnail, 1, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Clip_SmallThumbnail, 2, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Url_SmallThumbnail, 3, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Clip_MediumThumbnail, 2, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Url_MediumThumbnail, 3, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Url_LargeThumbnail, 3, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Url_HugeThumbnail, 3, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Clip_LargeThumbnail, 2, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_Clip_HugeThumbnail, 2, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.Label_BBCode_Normal, 0, 5)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_BBCode_Normal, 2, 5)
            Me.TableLayoutPanel1.Controls.Add(Me.Button_BBCode_Thumbnail, 2, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_BBCode_Normal, 1, 5)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_BBCode_Thumbnail, 1, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Url_LargeThumbnail, 1, 3)
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(6, 4)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 7
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571!))
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(346, 176)
            Me.TableLayoutPanel1.TabIndex = 16
            '
            'Label_BBCode_Thumbnail
            '
            Me.Label_BBCode_Thumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Label_BBCode_Thumbnail.ForeColor = System.Drawing.Color.Silver
            Me.Label_BBCode_Thumbnail.Location = New System.Drawing.Point(3, 150)
            Me.Label_BBCode_Thumbnail.Name = "Label_BBCode_Thumbnail"
            Me.Label_BBCode_Thumbnail.Size = New System.Drawing.Size(99, 20)
            Me.Label_BBCode_Thumbnail.TabIndex = 22
            Me.Label_BBCode_Thumbnail.Text = "BBCode Thumbnail"
            Me.Label_BBCode_Thumbnail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Button_Url_Normal
            '
            Me.Button_Url_Normal.BackColor = System.Drawing.Color.Transparent
            Me.Button_Url_Normal.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Go_Gray
            Me.Button_Url_Normal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Url_Normal.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Url_Normal.FlatAppearance.BorderSize = 0
            Me.Button_Url_Normal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Url_Normal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Url_Normal.Location = New System.Drawing.Point(310, 3)
            Me.Button_Url_Normal.Name = "Button_Url_Normal"
            Me.Button_Url_Normal.Size = New System.Drawing.Size(19, 19)
            Me.Button_Url_Normal.TabIndex = 7
            Me.Button_Url_Normal.Tag = "Url_Normal"
            Me.Button_Url_Normal.UseVisualStyleBackColor = False
            '
            'Button_Clip_Normal
            '
            Me.Button_Clip_Normal.BackColor = System.Drawing.Color.Transparent
            Me.Button_Clip_Normal.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_Clip_Normal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Clip_Normal.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Clip_Normal.FlatAppearance.BorderSize = 0
            Me.Button_Clip_Normal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Clip_Normal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Clip_Normal.Location = New System.Drawing.Point(286, 3)
            Me.Button_Clip_Normal.Name = "Button_Clip_Normal"
            Me.Button_Clip_Normal.Size = New System.Drawing.Size(18, 19)
            Me.Button_Clip_Normal.TabIndex = 14
            Me.Button_Clip_Normal.Tag = "Url_Normal"
            Me.Button_Clip_Normal.UseVisualStyleBackColor = False
            '
            'Button_Clip_SmallThumbnail
            '
            Me.Button_Clip_SmallThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Clip_SmallThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_Clip_SmallThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Clip_SmallThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Clip_SmallThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Clip_SmallThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Clip_SmallThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Clip_SmallThumbnail.Location = New System.Drawing.Point(286, 28)
            Me.Button_Clip_SmallThumbnail.Name = "Button_Clip_SmallThumbnail"
            Me.Button_Clip_SmallThumbnail.Size = New System.Drawing.Size(18, 19)
            Me.Button_Clip_SmallThumbnail.TabIndex = 17
            Me.Button_Clip_SmallThumbnail.Tag = "Url_SmallThumbnail"
            Me.Button_Clip_SmallThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Url_SmallThumbnail
            '
            Me.Button_Url_SmallThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Url_SmallThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Go_Gray
            Me.Button_Url_SmallThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Url_SmallThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Url_SmallThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Url_SmallThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Url_SmallThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Url_SmallThumbnail.Location = New System.Drawing.Point(310, 28)
            Me.Button_Url_SmallThumbnail.Name = "Button_Url_SmallThumbnail"
            Me.Button_Url_SmallThumbnail.Size = New System.Drawing.Size(19, 19)
            Me.Button_Url_SmallThumbnail.TabIndex = 10
            Me.Button_Url_SmallThumbnail.Tag = "Url_SmallThumbnail"
            Me.Button_Url_SmallThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Clip_MediumThumbnail
            '
            Me.Button_Clip_MediumThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Clip_MediumThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_Clip_MediumThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Clip_MediumThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Clip_MediumThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Clip_MediumThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Clip_MediumThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Clip_MediumThumbnail.Location = New System.Drawing.Point(286, 53)
            Me.Button_Clip_MediumThumbnail.Name = "Button_Clip_MediumThumbnail"
            Me.Button_Clip_MediumThumbnail.Size = New System.Drawing.Size(18, 19)
            Me.Button_Clip_MediumThumbnail.TabIndex = 20
            Me.Button_Clip_MediumThumbnail.Tag = "Url_MediumThumbnail"
            Me.Button_Clip_MediumThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Url_MediumThumbnail
            '
            Me.Button_Url_MediumThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Url_MediumThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Go_Gray
            Me.Button_Url_MediumThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Url_MediumThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Url_MediumThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Url_MediumThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Url_MediumThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Url_MediumThumbnail.Location = New System.Drawing.Point(310, 53)
            Me.Button_Url_MediumThumbnail.Name = "Button_Url_MediumThumbnail"
            Me.Button_Url_MediumThumbnail.Size = New System.Drawing.Size(19, 19)
            Me.Button_Url_MediumThumbnail.TabIndex = 11
            Me.Button_Url_MediumThumbnail.Tag = "Url_MediumThumbnail"
            Me.Button_Url_MediumThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Url_LargeThumbnail
            '
            Me.Button_Url_LargeThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Url_LargeThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Go_Gray
            Me.Button_Url_LargeThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Url_LargeThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Url_LargeThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Url_LargeThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Url_LargeThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Url_LargeThumbnail.Location = New System.Drawing.Point(310, 78)
            Me.Button_Url_LargeThumbnail.Name = "Button_Url_LargeThumbnail"
            Me.Button_Url_LargeThumbnail.Size = New System.Drawing.Size(19, 19)
            Me.Button_Url_LargeThumbnail.TabIndex = 12
            Me.Button_Url_LargeThumbnail.Tag = "Url_LargeThumbnail"
            Me.Button_Url_LargeThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Url_HugeThumbnail
            '
            Me.Button_Url_HugeThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Url_HugeThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Go_Gray
            Me.Button_Url_HugeThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Url_HugeThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Url_HugeThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Url_HugeThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Url_HugeThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Url_HugeThumbnail.Location = New System.Drawing.Point(310, 103)
            Me.Button_Url_HugeThumbnail.Name = "Button_Url_HugeThumbnail"
            Me.Button_Url_HugeThumbnail.Size = New System.Drawing.Size(19, 19)
            Me.Button_Url_HugeThumbnail.TabIndex = 13
            Me.Button_Url_HugeThumbnail.Tag = "Url_HugeThumbnail"
            Me.Button_Url_HugeThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Clip_LargeThumbnail
            '
            Me.Button_Clip_LargeThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Clip_LargeThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_Clip_LargeThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Clip_LargeThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Clip_LargeThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Clip_LargeThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Clip_LargeThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Clip_LargeThumbnail.Location = New System.Drawing.Point(286, 78)
            Me.Button_Clip_LargeThumbnail.Name = "Button_Clip_LargeThumbnail"
            Me.Button_Clip_LargeThumbnail.Size = New System.Drawing.Size(18, 19)
            Me.Button_Clip_LargeThumbnail.TabIndex = 19
            Me.Button_Clip_LargeThumbnail.Tag = "Url_LargeThumbnail"
            Me.Button_Clip_LargeThumbnail.UseVisualStyleBackColor = False
            '
            'Button_Clip_HugeThumbnail
            '
            Me.Button_Clip_HugeThumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_Clip_HugeThumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_Clip_HugeThumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Clip_HugeThumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Clip_HugeThumbnail.FlatAppearance.BorderSize = 0
            Me.Button_Clip_HugeThumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_Clip_HugeThumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_Clip_HugeThumbnail.Location = New System.Drawing.Point(286, 103)
            Me.Button_Clip_HugeThumbnail.Name = "Button_Clip_HugeThumbnail"
            Me.Button_Clip_HugeThumbnail.Size = New System.Drawing.Size(18, 19)
            Me.Button_Clip_HugeThumbnail.TabIndex = 18
            Me.Button_Clip_HugeThumbnail.Tag = "Url_HugeThumbnail"
            Me.Button_Clip_HugeThumbnail.UseVisualStyleBackColor = False
            '
            'Label_BBCode_Normal
            '
            Me.Label_BBCode_Normal.BackColor = System.Drawing.Color.Transparent
            Me.Label_BBCode_Normal.ForeColor = System.Drawing.Color.Silver
            Me.Label_BBCode_Normal.Location = New System.Drawing.Point(3, 125)
            Me.Label_BBCode_Normal.Name = "Label_BBCode_Normal"
            Me.Label_BBCode_Normal.Size = New System.Drawing.Size(99, 20)
            Me.Label_BBCode_Normal.TabIndex = 21
            Me.Label_BBCode_Normal.Text = "BBCode Normal"
            Me.Label_BBCode_Normal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Button_BBCode_Normal
            '
            Me.Button_BBCode_Normal.BackColor = System.Drawing.Color.Transparent
            Me.Button_BBCode_Normal.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_BBCode_Normal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_BBCode_Normal.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_BBCode_Normal.FlatAppearance.BorderSize = 0
            Me.Button_BBCode_Normal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_BBCode_Normal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_BBCode_Normal.Location = New System.Drawing.Point(286, 128)
            Me.Button_BBCode_Normal.Name = "Button_BBCode_Normal"
            Me.Button_BBCode_Normal.Size = New System.Drawing.Size(18, 19)
            Me.Button_BBCode_Normal.TabIndex = 23
            Me.Button_BBCode_Normal.Tag = "BBCode_Normal"
            Me.Button_BBCode_Normal.UseVisualStyleBackColor = False
            '
            'Button_BBCode_Thumbnail
            '
            Me.Button_BBCode_Thumbnail.BackColor = System.Drawing.Color.Transparent
            Me.Button_BBCode_Thumbnail.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.ClipboardGray
            Me.Button_BBCode_Thumbnail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_BBCode_Thumbnail.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_BBCode_Thumbnail.FlatAppearance.BorderSize = 0
            Me.Button_BBCode_Thumbnail.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(58, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(56, Byte), Integer))
            Me.Button_BBCode_Thumbnail.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Button_BBCode_Thumbnail.Location = New System.Drawing.Point(286, 153)
            Me.Button_BBCode_Thumbnail.Name = "Button_BBCode_Thumbnail"
            Me.Button_BBCode_Thumbnail.Size = New System.Drawing.Size(18, 19)
            Me.Button_BBCode_Thumbnail.TabIndex = 24
            Me.Button_BBCode_Thumbnail.Tag = "BBCode_Thumbnail"
            Me.Button_BBCode_Thumbnail.UseVisualStyleBackColor = False
            '
            'TextBox_BBCode_Normal
            '
            Me.TextBox_BBCode_Normal.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_BBCode_Normal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_BBCode_Normal.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_BBCode_Normal.Location = New System.Drawing.Point(108, 128)
            Me.TextBox_BBCode_Normal.Name = "TextBox_BBCode_Normal"
            Me.TextBox_BBCode_Normal.ReadOnly = True
            Me.TextBox_BBCode_Normal.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_BBCode_Normal.TabIndex = 25
            '
            'TextBox_BBCode_Thumbnail
            '
            Me.TextBox_BBCode_Thumbnail.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.TextBox_BBCode_Thumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBox_BBCode_Thumbnail.ForeColor = System.Drawing.Color.YellowGreen
            Me.TextBox_BBCode_Thumbnail.Location = New System.Drawing.Point(108, 153)
            Me.TextBox_BBCode_Thumbnail.Name = "TextBox_BBCode_Thumbnail"
            Me.TextBox_BBCode_Thumbnail.ReadOnly = True
            Me.TextBox_BBCode_Thumbnail.Size = New System.Drawing.Size(172, 20)
            Me.TextBox_BBCode_Thumbnail.TabIndex = 26
            '
            'OpenFileDialog_BrowseImage
            '
            Me.OpenFileDialog_BrowseImage.FileName = "Image.png"
            Me.OpenFileDialog_BrowseImage.Filter = resources.GetString("OpenFileDialog_BrowseImage.Filter")
            Me.OpenFileDialog_BrowseImage.ReadOnlyChecked = True
            Me.OpenFileDialog_BrowseImage.RestoreDirectory = True
            Me.OpenFileDialog_BrowseImage.SupportMultiDottedExtensions = True
            Me.OpenFileDialog_BrowseImage.Title = "Browse an image to upload"
            '
            'StatusStrip1
            '
            Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel_File})
            Me.StatusStrip1.Location = New System.Drawing.Point(0, 480)
            Me.StatusStrip1.Name = "StatusStrip1"
            Me.StatusStrip1.Size = New System.Drawing.Size(382, 22)
            Me.StatusStrip1.SizingGrip = False
            Me.StatusStrip1.TabIndex = 7
            Me.StatusStrip1.Text = "StatusStrip1"
            '
            'ToolStripStatusLabel_File
            '
            Me.ToolStripStatusLabel_File.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripStatusLabel_File.Name = "ToolStripStatusLabel_File"
            Me.ToolStripStatusLabel_File.Size = New System.Drawing.Size(39, 17)
            Me.ToolStripStatusLabel_File.Text = "Ready"
            '
            'CButton_BrowseImage
            '
            Me.CButton_BrowseImage.BorderColor = System.Drawing.Color.Black
            DesignerRectTracker1.IsActive = False
            DesignerRectTracker1.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker1.TrackerRectangle"), System.Drawing.RectangleF)
            Me.CButton_BrowseImage.CenterPtTracker = DesignerRectTracker1
            CBlendItems1.iColor = New System.Drawing.Color() {System.Drawing.Color.FromArgb(CType(CType(134, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(31, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(134, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(31, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(134, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(31, Byte), Integer))}
            CBlendItems1.iPoint = New Single() {0.0!, 0.5!, 1.0!}
            Me.CButton_BrowseImage.ColorFillBlend = CBlendItems1
            Me.CButton_BrowseImage.Corners.All = 2
            Me.CButton_BrowseImage.Corners.LowerLeft = 2
            Me.CButton_BrowseImage.Corners.LowerRight = 2
            Me.CButton_BrowseImage.Corners.UpperLeft = 2
            Me.CButton_BrowseImage.Corners.UpperRight = 2
            Me.CButton_BrowseImage.Cursor = System.Windows.Forms.Cursors.Hand
            DesignerRectTracker2.IsActive = False
            DesignerRectTracker2.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker2.TrackerRectangle"), System.Drawing.RectangleF)
            Me.CButton_BrowseImage.FocusPtTracker = DesignerRectTracker2
            Me.CButton_BrowseImage.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Bold)
            Me.CButton_BrowseImage.ForeColor = System.Drawing.Color.Black
            Me.CButton_BrowseImage.ImageIndex = 0
            Me.CButton_BrowseImage.Location = New System.Drawing.Point(12, 246)
            Me.CButton_BrowseImage.Name = "CButton_BrowseImage"
            Me.CButton_BrowseImage.Size = New System.Drawing.Size(358, 35)
            Me.CButton_BrowseImage.TabIndex = 8
            Me.CButton_BrowseImage.Text = "Browse an image..."
            Me.CButton_BrowseImage.TextShadow = System.Drawing.Color.Black
            Me.CButton_BrowseImage.TextShadowShow = False
            '
            'PictureBox_Logo
            '
            Me.PictureBox_Logo.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Logo
            Me.PictureBox_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.PictureBox_Logo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.PictureBox_Logo.Location = New System.Drawing.Point(12, 12)
            Me.PictureBox_Logo.Name = "PictureBox_Logo"
            Me.PictureBox_Logo.Size = New System.Drawing.Size(358, 226)
            Me.PictureBox_Logo.TabIndex = 15
            Me.PictureBox_Logo.TabStop = False
            '
            'CButton_Abort
            '
            Me.CButton_Abort.BorderColor = System.Drawing.Color.Black
            DesignerRectTracker3.IsActive = False
            DesignerRectTracker3.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker3.TrackerRectangle"), System.Drawing.RectangleF)
            Me.CButton_Abort.CenterPtTracker = DesignerRectTracker3
            CBlendItems2.iColor = New System.Drawing.Color() {System.Drawing.Color.Crimson, System.Drawing.Color.Crimson, System.Drawing.Color.Crimson}
            CBlendItems2.iPoint = New Single() {0.0!, 0.5!, 1.0!}
            Me.CButton_Abort.ColorFillBlend = CBlendItems2
            Me.CButton_Abort.Corners.All = 2
            Me.CButton_Abort.Corners.LowerLeft = 2
            Me.CButton_Abort.Corners.LowerRight = 2
            Me.CButton_Abort.Corners.UpperLeft = 2
            Me.CButton_Abort.Corners.UpperRight = 2
            Me.CButton_Abort.Cursor = System.Windows.Forms.Cursors.Hand
            DesignerRectTracker4.IsActive = False
            DesignerRectTracker4.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker4.TrackerRectangle"), System.Drawing.RectangleF)
            Me.CButton_Abort.FocusPtTracker = DesignerRectTracker4
            Me.CButton_Abort.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Bold)
            Me.CButton_Abort.ForeColor = System.Drawing.Color.WhiteSmoke
            Me.CButton_Abort.ImageIndex = 0
            Me.CButton_Abort.Location = New System.Drawing.Point(12, 246)
            Me.CButton_Abort.Name = "CButton_Abort"
            Me.CButton_Abort.Size = New System.Drawing.Size(358, 35)
            Me.CButton_Abort.TabIndex = 17
            Me.CButton_Abort.Text = "Abort Upload"
            Me.CButton_Abort.TextShadow = System.Drawing.Color.Black
            Me.CButton_Abort.TextShadowShow = False
            Me.CButton_Abort.Visible = False
            '
            'Main
            '
            Me.AllowDrop = True
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(18, Byte), Integer), CType(CType(18, Byte), Integer), CType(CType(16, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(382, 502)
            Me.Controls.Add(Me.StatusStrip1)
            Me.Controls.Add(Me.Panel_Urls)
            Me.Controls.Add(Me.PictureBox_Logo)
            Me.Controls.Add(Me.CButton_BrowseImage)
            Me.Controls.Add(Me.CButton_Abort)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MaximizeBox = False
            Me.Name = "Main"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Imgur Uploader.Net"
            Me.Panel_Urls.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.StatusStrip1.ResumeLayout(False)
            Me.StatusStrip1.PerformLayout()
            CType(Me.PictureBox_Logo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TextBox_Url_Normal As System.Windows.Forms.TextBox
        Friend WithEvents TextBox_Url_SmallThumbnail As System.Windows.Forms.TextBox
        Friend WithEvents TextBox_Url_MediumThumbnail As System.Windows.Forms.TextBox
        Friend WithEvents TextBox_Url_LargeThumbnail As System.Windows.Forms.TextBox
        Friend WithEvents Label_Normal As System.Windows.Forms.Label
        Friend WithEvents Label_SmallThumbnail As System.Windows.Forms.Label
        Friend WithEvents Label_MediumThumbnail As System.Windows.Forms.Label
        Friend WithEvents Label_LargeThumbnail As System.Windows.Forms.Label
        Friend WithEvents Label_HugeThumbnail As System.Windows.Forms.Label
        Friend WithEvents TextBox_Url_HugeThumbnail As System.Windows.Forms.TextBox
        Friend WithEvents PictureBox_Logo As System.Windows.Forms.PictureBox
        Friend WithEvents Panel_Urls As System.Windows.Forms.Panel
        Friend WithEvents OpenFileDialog_BrowseImage As System.Windows.Forms.OpenFileDialog
        Friend WithEvents Button_Url_Normal As System.Windows.Forms.Button
        Friend WithEvents Button_Url_MediumThumbnail As System.Windows.Forms.Button
        Friend WithEvents Button_Url_LargeThumbnail As System.Windows.Forms.Button
        Friend WithEvents Button_Url_HugeThumbnail As System.Windows.Forms.Button
        Friend WithEvents Button_Url_SmallThumbnail As System.Windows.Forms.Button
        Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
        Friend WithEvents ToolStripStatusLabel_File As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents CButton_BrowseImage As CButtonLib.CButton
        Friend WithEvents Button_Clip_Normal As System.Windows.Forms.Button
        Friend WithEvents Button_Clip_MediumThumbnail As System.Windows.Forms.Button
        Friend WithEvents Button_Clip_LargeThumbnail As System.Windows.Forms.Button
        Friend WithEvents Button_Clip_HugeThumbnail As System.Windows.Forms.Button
        Friend WithEvents Button_Clip_SmallThumbnail As System.Windows.Forms.Button
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Label_BBCode_Thumbnail As System.Windows.Forms.Label
        Friend WithEvents Label_BBCode_Normal As System.Windows.Forms.Label
        Friend WithEvents Button_BBCode_Normal As System.Windows.Forms.Button
        Friend WithEvents Button_BBCode_Thumbnail As System.Windows.Forms.Button
        Friend WithEvents TextBox_BBCode_Normal As System.Windows.Forms.TextBox
        Friend WithEvents TextBox_BBCode_Thumbnail As System.Windows.Forms.TextBox
        Friend WithEvents CButton_Abort As CButtonLib.CButton

    End Class

End Namespace
