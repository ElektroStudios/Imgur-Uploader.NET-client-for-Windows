
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

#End Region

#Region " DropDownSelector "

Namespace UserInterface.CustomControls

    ''' <summary>
    ''' A dark, flat, combo-box-like selector. Clicking anywhere on the control opens a drop-down list of items.
    ''' </summary>
    <DefaultEvent("SelectedIndexChanged")>
    Public Class DropDownSelector
        Inherits DropDownActionButton

#Region " Private Fields "

        ''' <summary>
        ''' The internal menu that displays the items.
        ''' </summary>
        Private ReadOnly menu As ContextMenuStrip

        ''' <summary>
        ''' The text displayed before the selected item (for example, "Size").
        ''' </summary>
        Private prefixValue As String = String.Empty

        ''' <summary>
        ''' The items of the selector.
        ''' </summary>
        Private itemsValue As String() = New String() {}

        ''' <summary>
        ''' The index of the selected item, or -1 if there is no selection.
        ''' </summary>
        Private selectedIndexValue As Integer = -1

        ''' <summary>
        ''' The accumulated mouse wheel delta that has not been converted into a selection step yet.
        ''' </summary>
        Private wheelDeltaAccumulator As Integer

#End Region

#Region " Events "

        ''' <summary>
        ''' Occurs when the value of <see cref="DropDownSelector.SelectedIndex"/> changes.
        ''' </summary>
        <Category("Behavior")>
        <Description("Occurs when the selected item changes.")>
        Public Event SelectedIndexChanged As EventHandler

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the text displayed before the selected item.
        ''' </summary>
        <Category("Appearance")>
        <Description("The text displayed before the selected item.")>
        <DefaultValue("")>
        Public Property Prefix As String
            Get
                Return Me.prefixValue
            End Get
            Set(value As String)
                Me.prefixValue = If(value, String.Empty)
                Me.UpdateDisplayText()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the index of the selected item (-1 for no selection).
        ''' </summary>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectedIndex As Integer
            Get
                Return Me.selectedIndexValue
            End Get
            Set(value As Integer)
                If (value < -1) OrElse (value >= Me.itemsValue.Length) Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), value, "Index is out of the items range.")
                End If
                If Me.selectedIndexValue <> value Then
                    Me.selectedIndexValue = value
                    Me.UpdateDisplayText()
                    RaiseEvent SelectedIndexChanged(Me, EventArgs.Empty)
                End If
            End Set
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DropDownSelector"/> class.
        ''' </summary>
        Public Sub New()

            MyBase.New()

            Me.menu = New ContextMenuStrip() With {
                .Renderer = New DarkMenuRenderer(),
                .ShowImageMargin = False,
                .ForeColor = Color.Gainsboro,
                .BackColor = Color.FromArgb(37, 37, 38)
            }

            AddHandler Me.menu.Opening, AddressOf Me.Menu_Opening

            Me.OpensMenuOnMainClick = True
            Me.DropDownMenu = Me.menu

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Replaces the items of the selector. The selection is reset to the first item (if any), without raising an event.
        ''' </summary>
        ''' <param name="items">The item captions.</param>
        ''' <exception cref="ArgumentNullException">
        ''' <paramref name="items"/> is <see langword="Nothing"/>.
        ''' </exception>
        Public Sub SetItems(items As String())

            If items Is Nothing Then
                Throw New ArgumentNullException(NameOf(items))
            End If

            Me.itemsValue = DirectCast(items.Clone(), String())
            Me.menu.Items.Clear()

            For i As Integer = 0 To (Me.itemsValue.Length - 1)
                Dim item As New ToolStripMenuItem(Me.itemsValue(i)) With {.Tag = i}
                AddHandler item.Click, AddressOf Me.MenuItem_Click
                Me.menu.Items.Add(item)
            Next

            Me.selectedIndexValue = If(Me.itemsValue.Length > 0, 0, -1)
            Me.UpdateDisplayText()

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Updates the text shown by the control.
        ''' </summary>
        Private Sub UpdateDisplayText()

            Dim itemText As String = If(Me.selectedIndexValue >= 0, Me.itemsValue(Me.selectedIndexValue), String.Empty)
            Me.Text = If(Me.prefixValue.Length > 0, $"{Me.prefixValue}: {itemText}", itemText)

        End Sub

        ''' <summary>
        ''' Handles the Opening event of the internal menu.
        ''' Makes every item as wide as the menu, so the whole drop-down area is clickable.
        ''' </summary>
        Private Sub Menu_Opening(sender As Object, e As CancelEventArgs)

            Dim contentWidth As Integer = Me.Width - Me.menu.Padding.Horizontal

            For Each item As ToolStripItem In Me.menu.Items
                contentWidth = Math.Max(contentWidth, item.GetPreferredSize(Size.Empty).Width)
            Next

            For Each item As ToolStripItem In Me.menu.Items
                item.AutoSize = False
                item.Width = contentWidth
            Next

        End Sub

        ''' <summary>
        ''' Handles the click on a menu item.
        ''' </summary>
        Private Sub MenuItem_Click(sender As Object, e As EventArgs)

            Me.SelectedIndex = CInt(DirectCast(DirectCast(sender, ToolStripMenuItem).Tag, Integer))

        End Sub

#End Region

#Region " Overrides "

        ''' <summary>
        ''' Gives the keyboard focus to the control when the mouse enters it,
        ''' so the mouse wheel is always delivered to the hovered selector.
        ''' </summary>
        Protected Overrides Sub OnMouseEnter(e As EventArgs)

            MyBase.OnMouseEnter(e)

            Dim parentForm As Form = Me.FindForm()
            If (parentForm IsNot Nothing) AndAlso (Form.ActiveForm Is parentForm) AndAlso Not Me.Focused Then
                Me.Focus()
            End If

        End Sub

        ''' <summary>
        ''' Changes the selected item with the mouse wheel (wheel up: previous item; wheel down: next item).
        ''' The selection does not wrap around.
        ''' </summary>
        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)

            MyBase.OnMouseWheel(e)

            Dim handledArgs As HandledMouseEventArgs = TryCast(e, HandledMouseEventArgs)
            If handledArgs IsNot Nothing Then
                handledArgs.Handled = True
            End If

            If Me.itemsValue.Length = 0 Then
                Return
            End If

            Me.wheelDeltaAccumulator += e.Delta
            Dim steps As Integer = Me.wheelDeltaAccumulator \ SystemInformation.MouseWheelScrollDelta
            Me.wheelDeltaAccumulator -= (steps * SystemInformation.MouseWheelScrollDelta)

            If steps <> 0 Then
                Dim newIndex As Integer = Math.Max(0, Math.Min(Me.itemsValue.Length - 1, Me.selectedIndexValue - steps))
                Me.SelectedIndex = newIndex
            End If

        End Sub

        ''' <summary>
        ''' The internal menu is owned by this control, so it is never serialized by the designer.
        ''' </summary>
        Protected Overrides Function ShouldSerializeDropDownMenu() As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Releases the resources used by the control.
        ''' </summary>
        ''' <param name="disposing"><see langword="True"/> to release managed resources.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)

            If disposing Then
                Me.menu.Dispose()
            End If
            MyBase.Dispose(disposing)

        End Sub

#End Region

    End Class

End Namespace

#End Region
