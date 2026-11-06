
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

#End Region

#Region " DropDownActionButton "

Namespace UserInterface.CustomControls

    ''' <summary>
    ''' A flat, dark-themed split button.
    ''' <para></para>
    ''' Clicking the main area raises <see cref="DropDownActionButton.MainAreaClick"/>,
    ''' while clicking the arrow area (optional) shows the assigned <see cref="DropDownActionButton.DropDownMenu"/>.
    ''' </summary>
    <DefaultEvent("MainAreaClick")>
    Public Class DropDownActionButton
        Inherits Control

#Region " Constants "

        ''' <summary>
        ''' The default width of the arrow area, in pixels.
        ''' </summary>
        Private Const DefaultArrowAreaWidth As Integer = 26

        ''' <summary>
        ''' The corner radius of the button, in pixels.
        ''' </summary>
        Private Const CornerRadius As Integer = 4

        ''' <summary>
        ''' The time window, in milliseconds, in which a click on the arrow area is ignored
        ''' after the menu has been closed by clicking that very same arrow area.
        ''' </summary>
        Private Const ReopenGuardMilliseconds As Double = 400.0R

#End Region

#Region " Colors "

        Private Shared ReadOnly BaseFillColor As Color = Color.FromArgb(45, 45, 48)
        Private Shared ReadOnly HoverFillColor As Color = Color.FromArgb(66, 66, 70)
        Private Shared ReadOnly PressedFillColor As Color = Color.FromArgb(30, 30, 32)
        Private Shared ReadOnly DisabledFillColor As Color = Color.FromArgb(34, 34, 36)
        Private Shared ReadOnly BorderColor As Color = Color.FromArgb(70, 70, 76)
        Private Shared ReadOnly HoverBorderColor As Color = Color.FromArgb(96, 96, 102)
        Private Shared ReadOnly DisabledTextColor As Color = Color.FromArgb(110, 110, 110)
        Private Shared ReadOnly AccentColor As Color = Color.FromArgb(134, 197, 31)

#End Region

#Region " Enumerations "

        ''' <summary>
        ''' Specifies the area of the button that is under the mouse cursor.
        ''' </summary>
        Private Enum HitZone As Integer

            ''' <summary>
            ''' The cursor is outside the button.
            ''' </summary>
            None = 0

            ''' <summary>
            ''' The cursor is over the main (text) area.
            ''' </summary>
            MainArea = 1

            ''' <summary>
            ''' The cursor is over the drop-down arrow area.
            ''' </summary>
            ArrowArea = 2

        End Enum

#End Region

#Region " Fields "

        ''' <summary>
        ''' The area currently under the mouse cursor.
        ''' </summary>
        Private hoverZone As HitZone = HitZone.None

        ''' <summary>
        ''' The area where the left mouse button was pressed.
        ''' </summary>
        Private pressedZone As HitZone = HitZone.None

        ''' <summary>
        ''' Indicates whether the drop-down menu is currently shown.
        ''' </summary>
        Private isMenuOpen As Boolean

        ''' <summary>
        ''' The moment when the menu was closed by a click on the arrow area.
        ''' </summary>
        Private menuClosedByArrowClickUtc As Date = Date.MinValue

        ''' <summary>
        ''' Backing field of <see cref="DropDownActionButton.HasDropDown"/>.
        ''' </summary>
        Private hasDropDownValue As Boolean = True

        ''' <summary>
        ''' Backing field of <see cref="DropDownActionButton.DropDownMenu"/>.
        ''' </summary>
        Private dropDownMenuValue As ContextMenuStrip

        ''' <summary>
        ''' Backing field of <see cref="DropDownActionButton.OpensMenuOnMainClick"/>.
        ''' </summary>
        Private opensMenuOnMainClickValue As Boolean

        ''' <summary>
        ''' Backing field of <see cref="DropDownActionButton.CenterText"/>.
        ''' </summary>
        Private centerTextValue As Boolean

        ''' <summary>
        ''' Backing field of <see cref="DropDownActionButton.ArrowAreaWidth"/>.
        ''' </summary>
        Private arrowAreaWidthValue As Integer = DefaultArrowAreaWidth

        ''' <summary>
        ''' Backing field of <see cref="DropDownActionButton.ButtonColor"/>.
        ''' </summary>
        Private buttonColorValue As Color = Color.Empty

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets a value indicating whether the arrow area (and its menu) is available.
        ''' </summary>
        <Category("Behavior")>
        <Description("Indicates whether the arrow area that shows the drop-down menu is available.")>
        <DefaultValue(True)>
        Public Property HasDropDown As Boolean
            Get
                Return Me.hasDropDownValue
            End Get
            Set(value As Boolean)
                If Me.hasDropDownValue <> value Then
                    Me.hasDropDownValue = value
                    Me.hoverZone = HitZone.None
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the menu that is shown when the arrow area is clicked.
        ''' </summary>
        <Category("Behavior")>
        <Description("The menu that is shown when the arrow area is clicked.")>
        <DefaultValue(CType(Nothing, Object))>
        Public Property DropDownMenu As ContextMenuStrip
            Get
                Return Me.dropDownMenuValue
            End Get
            Set(value As ContextMenuStrip)
                If Me.dropDownMenuValue IsNot Nothing Then
                    RemoveHandler Me.dropDownMenuValue.Closed, AddressOf Me.DropDownMenu_Closed
                End If

                Me.dropDownMenuValue = value

                If Me.dropDownMenuValue IsNot Nothing Then
                    AddHandler Me.dropDownMenuValue.Closed, AddressOf Me.DropDownMenu_Closed
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the whole control (not only the arrow) opens the menu.
        ''' This turns the control into a combo-box-like selector.
        ''' </summary>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Protected Property OpensMenuOnMainClick As Boolean
            Get
                Return Me.opensMenuOnMainClickValue
            End Get
            Set(value As Boolean)
                If Me.opensMenuOnMainClickValue <> value Then
                    Me.opensMenuOnMainClickValue = value
                    Me.hoverZone = HitZone.None
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the text is horizontally centered in the main area.
        ''' </summary>
        <Category("Appearance")>
        <Description("Indicates whether the text is horizontally centered in the main area.")>
        <DefaultValue(False)>
        Public Property CenterText As Boolean
            Get
                Return Me.centerTextValue
            End Get
            Set(value As Boolean)
                If Me.centerTextValue <> value Then
                    Me.centerTextValue = value
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the width, in pixels, of the arrow area.
        ''' A wide arrow area makes it harder to click it by mistake when the main area is clicked frequently.
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' The value is less than 12.
        ''' </exception>
        <Category("Appearance")>
        <Description("The width, in pixels, of the arrow area.")>
        <DefaultValue(DefaultArrowAreaWidth)>
        Public Property ArrowAreaWidth As Integer
            Get
                Return Me.arrowAreaWidthValue
            End Get
            Set(value As Integer)
                If value < 12 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), value, "The arrow area must be at least 12 pixels wide.")
                End If
                If Me.arrowAreaWidthValue <> value Then
                    Me.arrowAreaWidthValue = value
                    Me.hoverZone = HitZone.None
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the fill color of the button. If it is <see cref="Color.Empty"/> (the default),
        ''' the neutral dark theme colors are used; otherwise the hover, pressed and border colors are derived from it.
        ''' Set <see cref="Control.ForeColor"/> to a color that contrasts with it.
        ''' </summary>
        <Category("Appearance")>
        <Description("The fill color of the button. Empty uses the neutral dark theme.")>
        Public Property ButtonColor As Color
            Get
                Return Me.buttonColorValue
            End Get
            Set(value As Color)
                If Me.buttonColorValue <> value Then
                    Me.buttonColorValue = value
                    Me.Invalidate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Determines whether the designer must serialize the <see cref="DropDownActionButton.ButtonColor"/> property.
        ''' </summary>
        ''' <returns><see langword="True"/> if the property must be serialized; otherwise, <see langword="False"/>.</returns>
        Private Function ShouldSerializeButtonColor() As Boolean
            Return Not Me.buttonColorValue.IsEmpty
        End Function

        ''' <summary>
        ''' Resets the <see cref="DropDownActionButton.ButtonColor"/> property to its default value.
        ''' </summary>
        Private Sub ResetButtonColor()
            Me.ButtonColor = Color.Empty
        End Sub

        ''' <summary>
        ''' Determines whether the designer must serialize the <see cref="DropDownActionButton.DropDownMenu"/> property.
        ''' </summary>
        ''' <returns><see langword="True"/> if the property must be serialized; otherwise, <see langword="False"/>.</returns>
        Protected Overridable Function ShouldSerializeDropDownMenu() As Boolean
            Return (Me.dropDownMenuValue IsNot Nothing)
        End Function

        ''' <summary>
        ''' Gets the default size of the control.
        ''' </summary>
        Protected Overrides ReadOnly Property DefaultSize As Size
            Get
                Return New Size(134, 30)
            End Get
        End Property

#End Region

#Region " Events "

        ''' <summary>
        ''' Occurs when the main area (not the arrow) is clicked, or when the button is activated with the keyboard.
        ''' </summary>
        <Category("Action")>
        <Description("Occurs when the main area (not the arrow) is clicked.")>
        Public Event MainAreaClick As EventHandler

        ''' <summary>
        ''' Raises the <see cref="DropDownActionButton.MainAreaClick"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnMainAreaClick(e As EventArgs)

            RaiseEvent MainAreaClick(Me, e)

        End Sub

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DropDownActionButton"/> class.
        ''' </summary>
        Public Sub New()

            MyBase.New()

            Me.SetStyle(ControlStyles.UserPaint Or
                        ControlStyles.AllPaintingInWmPaint Or
                        ControlStyles.OptimizedDoubleBuffer Or
                        ControlStyles.ResizeRedraw Or
                        ControlStyles.Selectable, True)

            Me.BackColor = Color.FromArgb(26, 26, 26)
            Me.ForeColor = Color.Gainsboro

        End Sub

#End Region

#Region " Painting "

        ''' <summary>
        ''' Raises the <see cref="Control.Paint"/> event.
        ''' </summary>
        ''' <param name="e">A <see cref="PaintEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            Dim g As Graphics = e.Graphics
            g.Clear(Me.BackColor)
            g.SmoothingMode = SmoothingMode.AntiAlias

            Dim isEnabledNow As Boolean = Me.Enabled
            Dim arrowBounds As Rectangle = Rectangle.Empty

            If Me.hasDropDownValue Then
                arrowBounds = New Rectangle(Me.Width - Me.arrowAreaWidthValue, 0, Me.arrowAreaWidthValue, Me.Height)
            End If

            Dim mainBounds As New Rectangle(0, 0, Me.Width - arrowBounds.Width, Me.Height)
            Dim isSelectorMode As Boolean = Me.hasDropDownValue AndAlso Me.opensMenuOnMainClickValue
            Dim mainZoneBounds As Rectangle = mainBounds
            Dim arrowZoneBounds As Rectangle = arrowBounds

            If isSelectorMode Then
                mainZoneBounds = Rectangle.Empty
                arrowZoneBounds = New Rectangle(0, 0, Me.Width, Me.Height)
            End If
            Dim outline As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)

            Using path As GraphicsPath = DropDownActionButton.CreateRoundedPath(outline, CornerRadius)

                ' Base fill.
                Using baseBrush As New SolidBrush(If(isEnabledNow, Me.GetBaseFillColor(), DisabledFillColor))
                    g.FillPath(baseBrush, path)
                End Using

                ' Hover / pressed overlays, clipped to the rounded shape.
                If isEnabledNow Then
                    g.SetClip(path)
                    Me.FillZone(g, mainZoneBounds, HitZone.MainArea)
                    Me.FillZone(g, arrowZoneBounds, HitZone.ArrowArea)
                    g.ResetClip()
                End If

                ' Divider between the main area and the arrow area.
                If Me.hasDropDownValue AndAlso Not isSelectorMode Then
                    Using dividerPen As New Pen(Me.GetBorderColor())
                        g.DrawLine(dividerPen, arrowBounds.X, 5, arrowBounds.X, Me.Height - 6)
                    End Using
                End If

                ' Border.
                Dim borderToUse As Color = Me.GetBorderColor()
                If isEnabledNow AndAlso Me.Focused AndAlso Me.ShowFocusCues Then
                    borderToUse = If(Me.HasButtonColor, MixColors(Me.buttonColorValue, Color.White, 0.6F), AccentColor)
                ElseIf isEnabledNow AndAlso (Me.hoverZone <> HitZone.None OrElse Me.isMenuOpen) Then
                    borderToUse = If(Me.HasButtonColor, MixColors(Me.buttonColorValue, Color.White, 0.35F), HoverBorderColor)
                End If

                Using borderPen As New Pen(borderToUse)
                    g.DrawPath(borderPen, path)
                End Using

            End Using

            ' Text.
            Dim textBounds As New Rectangle(mainBounds.X + 8, 0, Math.Max(mainBounds.Width - 12, 1), Me.Height)
            TextRenderer.DrawText(g, Me.Text, Me.Font, textBounds,
                                  If(isEnabledNow, Me.ForeColor, DisabledTextColor),
                                  If(Me.centerTextValue, TextFormatFlags.HorizontalCenter, TextFormatFlags.Left) Or
                                  TextFormatFlags.VerticalCenter Or
                                  TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)

            ' Arrow glyph (chevron).
            If Me.hasDropDownValue Then
                Dim glyphColor As Color = DisabledTextColor
                If isEnabledNow Then
                    If Me.HasButtonColor Then
                        glyphColor = Me.ForeColor
                    Else
                        glyphColor = If(Me.hoverZone = HitZone.ArrowArea OrElse Me.isMenuOpen, AccentColor, Color.Gainsboro)
                    End If
                End If

                Dim cx As Single = arrowBounds.X + (arrowBounds.Width / 2.0F)
                Dim cy As Single = Me.Height / 2.0F

                Using glyphPen As New Pen(glyphColor, 1.8F)
                    glyphPen.StartCap = LineCap.Round
                    glyphPen.EndCap = LineCap.Round
                    glyphPen.LineJoin = LineJoin.Round
                    g.DrawLines(glyphPen, New PointF() {New PointF(cx - 4.0F, cy - 2.0F),
                                                        New PointF(cx, cy + 2.0F),
                                                        New PointF(cx + 4.0F, cy - 2.0F)})
                End Using
            End If

        End Sub

        ''' <summary>
        ''' Fills the specified zone with the hover or pressed color, when applicable.
        ''' </summary>
        ''' <param name="g">The graphics surface.</param>
        ''' <param name="zoneBounds">The bounds of the zone.</param>
        ''' <param name="zone">The zone kind.</param>
        Private Sub FillZone(g As Graphics, zoneBounds As Rectangle, zone As HitZone)

            If zoneBounds.IsEmpty Then
                Return
            End If

            Dim hasFill As Boolean = True
            Dim fillColor As Color = Me.GetHoverFillColor()

            If Me.pressedZone = zone AndAlso Me.hoverZone = zone Then
                fillColor = Me.GetPressedFillColor()
            ElseIf zone = HitZone.ArrowArea AndAlso Me.isMenuOpen Then
                fillColor = Me.GetPressedFillColor()
            ElseIf Me.hoverZone = zone Then
                fillColor = Me.GetHoverFillColor()
            Else
                hasFill = False
            End If

            If hasFill Then
                Using brush As New SolidBrush(fillColor)
                    g.FillRectangle(brush, zoneBounds)
                End Using
            End If

        End Sub

        ''' <summary>
        ''' Creates a rounded rectangle path.
        ''' </summary>
        ''' <param name="bounds">The rectangle bounds.</param>
        ''' <param name="radius">The corner radius.</param>
        ''' <returns>The resulting <see cref="GraphicsPath"/>. The caller must dispose it.</returns>
        Private Shared Function CreateRoundedPath(bounds As Rectangle, radius As Integer) As GraphicsPath

            Dim diameter As Integer = radius * 2
            Dim path As New GraphicsPath()

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180.0F, 90.0F)
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270.0F, 90.0F)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0.0F, 90.0F)
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90.0F, 90.0F)
            path.CloseFigure()

            Return path

        End Function

#End Region

#Region " Mouse And Keyboard "

        ''' <summary>
        ''' Raises the <see cref="Control.MouseMove"/> event.
        ''' </summary>
        ''' <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

            MyBase.OnMouseMove(e)
            Me.SetHoverZone(Me.GetZoneAt(e.Location))

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseLeave"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnMouseLeave(e As EventArgs)

            MyBase.OnMouseLeave(e)
            Me.SetHoverZone(HitZone.None)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseDown"/> event.
        ''' </summary>
        ''' <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)

            MyBase.OnMouseDown(e)

            If e.Button = MouseButtons.Left Then
                Me.pressedZone = Me.GetZoneAt(e.Location)
                Me.Focus()
                Me.Invalidate()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseUp"/> event.
        ''' </summary>
        ''' <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)

            Dim releasedZone As HitZone = Me.GetZoneAt(e.Location)
            Dim previouslyPressedZone As HitZone = Me.pressedZone
            Me.pressedZone = HitZone.None

            MyBase.OnMouseUp(e)

            If (e.Button = MouseButtons.Left) AndAlso
               (releasedZone <> HitZone.None) AndAlso
               (releasedZone = previouslyPressedZone) Then

                Select Case releasedZone

                    Case HitZone.MainArea
                        Me.OnMainAreaClick(EventArgs.Empty)

                    Case HitZone.ArrowArea
                        Dim elapsed As Double = (Date.UtcNow - Me.menuClosedByArrowClickUtc).TotalMilliseconds
                        If elapsed >= ReopenGuardMilliseconds Then
                            Me.ShowDropDown()
                        End If

                End Select

            End If

            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.KeyDown"/> event.
        ''' </summary>
        ''' <param name="e">A <see cref="KeyEventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

            MyBase.OnKeyDown(e)

            If Me.hasDropDownValue AndAlso Me.opensMenuOnMainClickValue AndAlso
               ((e.KeyCode = Keys.Space) OrElse (e.KeyCode = Keys.Enter) OrElse (e.KeyCode = Keys.Down)) Then
                e.Handled = True
                Me.ShowDropDown()

            ElseIf (e.KeyCode = Keys.Space) OrElse (e.KeyCode = Keys.Enter) Then
                e.Handled = True
                Me.OnMainAreaClick(EventArgs.Empty)

            ElseIf Me.hasDropDownValue AndAlso
                   ((e.KeyCode = Keys.F4) OrElse (e.Alt AndAlso e.KeyCode = Keys.Down)) Then
                e.Handled = True
                Me.ShowDropDown()

            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.GotFocus"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnGotFocus(e As EventArgs)

            MyBase.OnGotFocus(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.LostFocus"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnLostFocus(e As EventArgs)

            MyBase.OnLostFocus(e)
            Me.pressedZone = HitZone.None
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.EnabledChanged"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnEnabledChanged(e As EventArgs)

            MyBase.OnEnabledChanged(e)
            Me.hoverZone = HitZone.None
            Me.pressedZone = HitZone.None
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.TextChanged"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnTextChanged(e As EventArgs)

            MyBase.OnTextChanged(e)
            Me.Invalidate()

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Gets the zone located at the specified client point.
        ''' </summary>
        ''' <param name="location">The client-relative point.</param>
        ''' <returns>The zone located at the specified point.</returns>
        Private Function GetZoneAt(location As Point) As HitZone

            If Not Me.ClientRectangle.Contains(location) Then
                Return HitZone.None
            End If

            If Me.hasDropDownValue AndAlso Me.opensMenuOnMainClickValue Then
                Return HitZone.ArrowArea
            End If

            If Me.hasDropDownValue AndAlso (location.X >= (Me.Width - Me.arrowAreaWidthValue)) Then
                Return HitZone.ArrowArea
            End If

            Return HitZone.MainArea

        End Function

        ''' <summary>
        ''' Gets a value indicating whether a custom <see cref="DropDownActionButton.ButtonColor"/> is set.
        ''' </summary>
        Private ReadOnly Property HasButtonColor As Boolean
            Get
                Return Not Me.buttonColorValue.IsEmpty
            End Get
        End Property

        ''' <summary>
        ''' Gets the fill color of the enabled button.
        ''' </summary>
        Private Function GetBaseFillColor() As Color
            Return If(Me.HasButtonColor, Me.buttonColorValue, BaseFillColor)
        End Function

        ''' <summary>
        ''' Gets the fill color of the zone under the mouse cursor.
        ''' </summary>
        Private Function GetHoverFillColor() As Color
            Return If(Me.HasButtonColor, MixColors(Me.buttonColorValue, Color.White, 0.18F), HoverFillColor)
        End Function

        ''' <summary>
        ''' Gets the fill color of the pressed zone.
        ''' </summary>
        Private Function GetPressedFillColor() As Color
            Return If(Me.HasButtonColor, MixColors(Me.buttonColorValue, Color.Black, 0.22F), PressedFillColor)
        End Function

        ''' <summary>
        ''' Gets the border (and divider) color of the button, when it is not hovered nor focused.
        ''' </summary>
        Private Function GetBorderColor() As Color
            Return If(Me.HasButtonColor AndAlso Me.Enabled, MixColors(Me.buttonColorValue, Color.Black, 0.35F), BorderColor)
        End Function

        ''' <summary>
        ''' Mixes two colors.
        ''' </summary>
        ''' <param name="baseColor">The base color.</param>
        ''' <param name="mixColor">The color that is mixed in.</param>
        ''' <param name="amount">The proportion of <paramref name="mixColor"/>, from 0 to 1.</param>
        ''' <returns>The resulting opaque color.</returns>
        Private Shared Function MixColors(baseColor As Color, mixColor As Color, amount As Single) As Color

            Return Color.FromArgb(CInt(CInt(baseColor.R) + ((CInt(mixColor.R) - CInt(baseColor.R)) * amount)),
                                  CInt(CInt(baseColor.G) + ((CInt(mixColor.G) - CInt(baseColor.G)) * amount)),
                                  CInt(CInt(baseColor.B) + ((CInt(mixColor.B) - CInt(baseColor.B)) * amount)))

        End Function

        ''' <summary>
        ''' Updates the hover zone and repaints the control if it changed.
        ''' </summary>
        ''' <param name="zone">The new hover zone.</param>
        Private Sub SetHoverZone(zone As HitZone)

            If Me.hoverZone <> zone Then
                Me.hoverZone = zone
                Me.Invalidate()
            End If

        End Sub

        ''' <summary>
        ''' Shows the drop-down menu below the button.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">
        ''' The <see cref="DropDownActionButton.DropDownMenu"/> property is not set.
        ''' </exception>
        Private Sub ShowDropDown()

            If Me.dropDownMenuValue Is Nothing Then
                Throw New InvalidOperationException($"The {NameOf(DropDownActionButton.DropDownMenu)} property must be set to use the arrow area.")
            End If

            Me.isMenuOpen = True
            Me.Invalidate()

            Me.dropDownMenuValue.MinimumSize = New Size(Me.Width, 0)
            Me.dropDownMenuValue.Show(Me, New Point(0, Me.Height + 1))

        End Sub

        ''' <summary>
        ''' Handles the Closed event of the drop-down menu.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="ToolStripDropDownClosedEventArgs"/> instance containing the event data.</param>
        Private Sub DropDownMenu_Closed(sender As Object, e As ToolStripDropDownClosedEventArgs)

            Me.isMenuOpen = False

            ' If the user closed the menu by clicking the arrow again, don't re-open it on mouse up.
            If (e.CloseReason = ToolStripDropDownCloseReason.AppClicked) AndAlso
               (Me.GetZoneAt(Me.PointToClient(Cursor.Position)) = HitZone.ArrowArea) Then

                Me.menuClosedByArrowClickUtc = Date.UtcNow
            End If

            Me.Invalidate()

        End Sub

#End Region

#Region " Dispose "

        ''' <summary>
        ''' Releases the unmanaged resources used by the control and optionally releases the managed resources.
        ''' </summary>
        ''' <param name="disposing">
        ''' <see langword="True"/> to release both managed and unmanaged resources;
        ''' <see langword="False"/> to release only unmanaged resources.
        ''' </param>
        Protected Overrides Sub Dispose(disposing As Boolean)

            If disposing AndAlso (Me.dropDownMenuValue IsNot Nothing) Then
                RemoveHandler Me.dropDownMenuValue.Closed, AddressOf Me.DropDownMenu_Closed
            End If

            MyBase.Dispose(disposing)

        End Sub

#End Region

    End Class

End Namespace

#End Region
