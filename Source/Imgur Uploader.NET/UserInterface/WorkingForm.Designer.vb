Imports ImgurUploader.UserInterface

Namespace UserInterface

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WorkingForm
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WorkingForm))
            Me.Label_Status = New System.Windows.Forms.Label()
            Me.PictureBox_Status = New System.Windows.Forms.PictureBox()
            Me.Timer_StatusLabel = New System.Windows.Forms.Timer(Me.components)
            Me.Panel1 = New System.Windows.Forms.Panel()
            CType(Me.PictureBox_Status, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Panel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'Label_Status
            '
            Me.Label_Status.AutoSize = True
            Me.Label_Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!)
            Me.Label_Status.Location = New System.Drawing.Point(43, 9)
            Me.Label_Status.Name = "Label_Status"
            Me.Label_Status.Size = New System.Drawing.Size(171, 24)
            Me.Label_Status.TabIndex = 0
            Me.Label_Status.Text = "Optimizing image..."
            '
            'PictureBox_Status
            '
            Me.PictureBox_Status.BackgroundImage = Global.ImgurUploader.My.Resources.Resources.Tool
            Me.PictureBox_Status.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.PictureBox_Status.Location = New System.Drawing.Point(3, 3)
            Me.PictureBox_Status.Name = "PictureBox_Status"
            Me.PictureBox_Status.Size = New System.Drawing.Size(32, 32)
            Me.PictureBox_Status.TabIndex = 1
            Me.PictureBox_Status.TabStop = False
            '
            'Timer_StatusLabel
            '
            Me.Timer_StatusLabel.Interval = 500
            '
            'Panel1
            '
            Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.Panel1.Controls.Add(Me.PictureBox_Status)
            Me.Panel1.Controls.Add(Me.Label_Status)
            Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(222, 40)
            Me.Panel1.TabIndex = 2
            '
            'Working
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(18, Byte), Integer), CType(CType(18, Byte), Integer), CType(CType(16, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(222, 40)
            Me.Controls.Add(Me.Panel1)
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "Working"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Imgur Uploader.Net"
            CType(Me.PictureBox_Status, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Panel1.ResumeLayout(False)
            Me.Panel1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents Label_Status As System.Windows.Forms.Label
        Friend WithEvents PictureBox_Status As System.Windows.Forms.PictureBox
        Friend WithEvents Timer_StatusLabel As System.Windows.Forms.Timer
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
    End Class

End Namespace
