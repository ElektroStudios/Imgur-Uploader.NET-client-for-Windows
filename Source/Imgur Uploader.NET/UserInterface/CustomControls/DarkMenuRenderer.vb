
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Drawing2D

#End Region

#Region " DarkMenuRenderer "

Namespace UserInterface.CustomControls

    ''' <summary>
    ''' Renders <see cref="ToolStrip"/> drop-down menus with a flat dark theme,
    ''' without the white borders and light image margin of the default renderer.
    ''' </summary>
    Public NotInheritable Class DarkMenuRenderer
        Inherits ToolStripProfessionalRenderer

#Region " Colors "

        Private Shared ReadOnly MenuBackColor As Color = Color.FromArgb(37, 37, 38)
        Private Shared ReadOnly MenuBorderColor As Color = Color.FromArgb(70, 70, 76)
        Private Shared ReadOnly ItemHoverColor As Color = Color.FromArgb(66, 66, 70)
        Private Shared ReadOnly ItemTextColor As Color = Color.Gainsboro
        Private Shared ReadOnly ItemDisabledTextColor As Color = Color.FromArgb(110, 110, 110)
        Private Shared ReadOnly CheckMarkColor As Color = Color.FromArgb(134, 197, 31)

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DarkMenuRenderer"/> class.
        ''' </summary>
        Public Sub New()

            MyBase.New(New DarkColorTable())
            Me.RoundedEdges = False

        End Sub

#End Region

#Region " Render Overrides "

        ''' <summary>
        ''' Draws the background of a <see cref="ToolStrip"/>.
        ''' </summary>
        ''' <param name="e">A <see cref="ToolStripRenderEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)

            Using brush As New SolidBrush(MenuBackColor)
                e.Graphics.FillRectangle(brush, e.AffectedBounds)
            End Using

        End Sub

        ''' <summary>
        ''' Draws the border of a <see cref="ToolStrip"/>.
        ''' </summary>
        ''' <param name="e">A <see cref="ToolStripRenderEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnRenderToolStripBorder(e As ToolStripRenderEventArgs)

            Using pen As New Pen(MenuBorderColor)
                e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1)
            End Using

        End Sub

        ''' <summary>
        ''' Draws the background of a <see cref="ToolStripMenuItem"/>.
        ''' </summary>
        ''' <param name="e">A <see cref="ToolStripItemRenderEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnRenderMenuItemBackground(e As ToolStripItemRenderEventArgs)

            If e.Item.Selected AndAlso e.Item.Enabled Then
                Using brush As New SolidBrush(ItemHoverColor)
                    e.Graphics.FillRectangle(brush, 1, 0, e.Item.Width - 2, e.Item.Height)
                End Using
            End If

        End Sub

        ''' <summary>
        ''' Draws the text of a <see cref="ToolStripItem"/>.
        ''' </summary>
        ''' <param name="e">A <see cref="ToolStripItemTextRenderEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)

            e.TextColor = If(e.Item.Enabled, ItemTextColor, ItemDisabledTextColor)
            MyBase.OnRenderItemText(e)

        End Sub

        ''' <summary>
        ''' Draws the check mark of a checked <see cref="ToolStripMenuItem"/>
        ''' (only visible when the menu shows its check margin).
        ''' </summary>
        ''' <param name="e">A <see cref="ToolStripItemImageRenderEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)

            Dim g As Graphics = e.Graphics
            Dim previousMode As SmoothingMode = g.SmoothingMode
            g.SmoothingMode = SmoothingMode.AntiAlias

            Dim cx As Single = e.ImageRectangle.X + (e.ImageRectangle.Width / 2.0F)
            Dim cy As Single = e.ImageRectangle.Y + (e.ImageRectangle.Height / 2.0F)

            Using pen As New Pen(If(e.Item.Enabled, CheckMarkColor, ItemDisabledTextColor), 2.0F)
                pen.StartCap = LineCap.Round
                pen.EndCap = LineCap.Round
                pen.LineJoin = LineJoin.Round
                g.DrawLines(pen, New PointF() {New PointF(cx - 4.0F, cy),
                                               New PointF(cx - 1.0F, cy + 3.0F),
                                               New PointF(cx + 4.5F, cy - 3.5F)})
            End Using

            g.SmoothingMode = previousMode

        End Sub

#End Region

#Region " Color Table "

        ''' <summary>
        ''' A dark color table used as a fallback for the parts that are not custom-rendered.
        ''' </summary>
        Private NotInheritable Class DarkColorTable
            Inherits ProfessionalColorTable

            Public Overrides ReadOnly Property MenuBorder As Color
                Get
                    Return MenuBorderColor
                End Get
            End Property

            Public Overrides ReadOnly Property MenuItemBorder As Color
                Get
                    Return ItemHoverColor
                End Get
            End Property

            Public Overrides ReadOnly Property MenuItemSelected As Color
                Get
                    Return ItemHoverColor
                End Get
            End Property

            Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
                Get
                    Return ItemHoverColor
                End Get
            End Property

            Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
                Get
                    Return ItemHoverColor
                End Get
            End Property

            Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
                Get
                    Return MenuBackColor
                End Get
            End Property

            Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
                Get
                    Return MenuBackColor
                End Get
            End Property

            Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
                Get
                    Return MenuBackColor
                End Get
            End Property

            Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
                Get
                    Return MenuBackColor
                End Get
            End Property

            Public Overrides ReadOnly Property SeparatorDark As Color
                Get
                    Return MenuBorderColor
                End Get
            End Property

            Public Overrides ReadOnly Property SeparatorLight As Color
                Get
                    Return MenuBorderColor
                End Get
            End Property

        End Class

#End Region

    End Class

End Namespace

#End Region
