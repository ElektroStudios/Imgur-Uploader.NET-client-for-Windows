
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports ImgurUploader.UserInterface.CustomControls
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.ComponentModel
Imports System.Reflection
Imports System.Windows.Forms

#End Region

#Region " CustomControlsTests "

Namespace Tests

    ''' <summary>
    ''' Tests for <see cref="DropDownSelector"/>, <see cref="DropDownActionButton"/>,
    ''' <see cref="DarkMenuRenderer"/> and <see cref="DarkTitleBar"/>.
    ''' The protected and private members are invoked by reflection, as they can only be raised by real mouse/keyboard input.
    ''' </summary>
    <TestClass>
    Public Class CustomControlsTests

        Private Const NonPublicInstance As BindingFlags = BindingFlags.Instance Or BindingFlags.NonPublic

        Private Shared Function CreateSelector() As DropDownSelector

            Dim selector As New DropDownSelector() With {.Prefix = "Size"}
            selector.SetItems(New String() {"Normal", "Small", "Medium", "Large", "Huge"})
            Return selector

        End Function

        Private Shared Sub RaiseMouseWheel(selector As DropDownSelector, delta As Integer)

            Dim method As MethodInfo = GetType(DropDownSelector).GetMethod("OnMouseWheel", NonPublicInstance)
            method.Invoke(selector, New Object() {New HandledMouseEventArgs(MouseButtons.None, 0, 0, 0, delta)})

        End Sub

#Region " DropDownSelector: items and selection "

        <TestMethod>
        Public Sub Selector_SetItems_SelectsTheFirstItemWithoutRaisingTheEvent()

            Dim selector As New DropDownSelector() With {.Prefix = "Size"}
            Dim raised As Integer = 0
            AddHandler selector.SelectedIndexChanged, Sub(sender As Object, e As EventArgs) raised += 1

            selector.SetItems(New String() {"Normal", "Small"})

            Assert.AreEqual(0, selector.SelectedIndex)
            Assert.AreEqual("Size: Normal", selector.Text)
            Assert.AreEqual(0, raised)

        End Sub

        <TestMethod>
        Public Sub Selector_SetItems_EmptyList_HasNoSelection()

            Dim selector As New DropDownSelector()

            selector.SetItems(New String() {})

            Assert.AreEqual(-1, selector.SelectedIndex)
            Assert.AreEqual(String.Empty, selector.Text)

        End Sub

        <TestMethod>
        Public Sub Selector_SetItems_Null_Throws()

            Dim selector As New DropDownSelector()

            Assert.ThrowsException(Of ArgumentNullException)(Sub() selector.SetItems(Nothing))

        End Sub

        <TestMethod>
        Public Sub Selector_SelectedIndex_RaisesTheEventOnlyWhenItChanges()

            Dim selector As DropDownSelector = CreateSelector()
            Dim raised As Integer = 0
            AddHandler selector.SelectedIndexChanged, Sub(sender As Object, e As EventArgs) raised += 1

            selector.SelectedIndex = 2
            selector.SelectedIndex = 2

            Assert.AreEqual(1, raised)
            Assert.AreEqual("Size: Medium", selector.Text)

        End Sub

        <TestMethod>
        Public Sub Selector_SelectedIndex_OutOfRange_Throws()

            Dim selector As DropDownSelector = CreateSelector()

            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() selector.SelectedIndex = 5)
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() selector.SelectedIndex = -2)

        End Sub

        <TestMethod>
        Public Sub Selector_Prefix_ChangesTheDisplayedText()

            Dim selector As DropDownSelector = CreateSelector()

            selector.Prefix = "Format"
            Assert.AreEqual("Format: Normal", selector.Text)

            selector.Prefix = String.Empty
            Assert.AreEqual("Normal", selector.Text)

        End Sub

        <TestMethod>
        Public Sub Selector_MenuItemClick_SelectsTheClickedItem()

            Dim selector As DropDownSelector = CreateSelector()
            Dim menu As ContextMenuStrip = DirectCast(GetType(DropDownSelector).GetField("menu", NonPublicInstance).GetValue(selector), ContextMenuStrip)

            Assert.AreEqual(5, menu.Items.Count)

            DirectCast(menu.Items(3), ToolStripMenuItem).PerformClick()

            Assert.AreEqual(3, selector.SelectedIndex)

        End Sub

#End Region

#Region " DropDownSelector: mouse wheel "

        <TestMethod>
        Public Sub Selector_WheelDown_SelectsTheNextItem()

            Dim selector As DropDownSelector = CreateSelector()

            RaiseMouseWheel(selector, -120)

            Assert.AreEqual(1, selector.SelectedIndex)

        End Sub

        <TestMethod>
        Public Sub Selector_WheelUp_SelectsThePreviousItem()

            Dim selector As DropDownSelector = CreateSelector()
            selector.SelectedIndex = 3

            RaiseMouseWheel(selector, 120)

            Assert.AreEqual(2, selector.SelectedIndex)

        End Sub

        <TestMethod>
        Public Sub Selector_Wheel_DoesNotWrapAround()

            Dim selector As DropDownSelector = CreateSelector()

            RaiseMouseWheel(selector, 120)
            Assert.AreEqual(0, selector.SelectedIndex)

            selector.SelectedIndex = 4
            RaiseMouseWheel(selector, -120)
            Assert.AreEqual(4, selector.SelectedIndex)

        End Sub

        <TestMethod>
        Public Sub Selector_Wheel_SmallDeltasAreAccumulated()

            Dim selector As DropDownSelector = CreateSelector()

            RaiseMouseWheel(selector, -60)
            Assert.AreEqual(0, selector.SelectedIndex, "Half a notch must not change the selection yet.")

            RaiseMouseWheel(selector, -60)
            Assert.AreEqual(1, selector.SelectedIndex)

        End Sub

        <TestMethod>
        Public Sub Selector_Wheel_MultipleNotchesMoveSeveralItems()

            Dim selector As DropDownSelector = CreateSelector()

            RaiseMouseWheel(selector, -360)

            Assert.AreEqual(3, selector.SelectedIndex)

        End Sub

        <TestMethod>
        Public Sub Selector_Wheel_WithoutItems_DoesNothing()

            Dim selector As New DropDownSelector()
            selector.SetItems(New String() {})

            RaiseMouseWheel(selector, -120)

            Assert.AreEqual(-1, selector.SelectedIndex)

        End Sub

        <TestMethod>
        Public Sub Selector_Wheel_MarksTheEventAsHandled()

            Dim selector As DropDownSelector = CreateSelector()
            Dim args As New HandledMouseEventArgs(MouseButtons.None, 0, 0, 0, -120)

            GetType(DropDownSelector).GetMethod("OnMouseWheel", NonPublicInstance).Invoke(selector, New Object() {args})

            Assert.IsTrue(args.Handled, "The parent must not scroll when the wheel is used on the selector.")

        End Sub

#End Region

#Region " DropDownSelector: menu width "

        <TestMethod>
        Public Sub Selector_MenuOpening_MakesEveryItemAsWideAsTheControl()

            Dim selector As DropDownSelector = CreateSelector()
            selector.Width = 300

            Dim menu As ContextMenuStrip = DirectCast(GetType(DropDownSelector).GetField("menu", NonPublicInstance).GetValue(selector), ContextMenuStrip)
            GetType(DropDownSelector).GetMethod("Menu_Opening", NonPublicInstance).Invoke(selector, New Object() {menu, New CancelEventArgs()})

            Dim expectedWidth As Integer = 300 - menu.Padding.Horizontal

            For Each item As ToolStripItem In menu.Items
                Assert.IsFalse(item.AutoSize)
                Assert.IsTrue(item.Width >= expectedWidth, $"'{item.Text}' is {item.Width}px wide, expected at least {expectedWidth}px.")
            Next

        End Sub

        <TestMethod>
        Public Sub Selector_MenuOpening_AllItemsHaveTheSameWidth()

            Dim selector As DropDownSelector = CreateSelector()
            selector.Width = 50 ' Narrower than the longest caption.

            Dim menu As ContextMenuStrip = DirectCast(GetType(DropDownSelector).GetField("menu", NonPublicInstance).GetValue(selector), ContextMenuStrip)
            GetType(DropDownSelector).GetMethod("Menu_Opening", NonPublicInstance).Invoke(selector, New Object() {menu, New CancelEventArgs()})

            Dim widths As Integer() = menu.Items.Cast(Of ToolStripItem)().Select(Function(item As ToolStripItem) item.Width).Distinct().ToArray()

            Assert.AreEqual(1, widths.Length)

        End Sub

#End Region

#Region " DropDownSelector: designer behavior "

        <TestMethod>
        Public Sub Selector_DoesNotSerializeItsInternalMenu()

            Dim selector As DropDownSelector = CreateSelector()

            Dim method As MethodInfo = GetType(DropDownSelector).GetMethod("ShouldSerializeDropDownMenu", NonPublicInstance)

            Assert.IsNotNull(selector.DropDownMenu)
            Assert.IsFalse(CBool(method.Invoke(selector, Nothing)))

        End Sub

#End Region

#Region " DropDownActionButton "

        <TestMethod>
        Public Sub Button_Defaults_AreSplitButtonValues()

            Dim button As New DropDownActionButton()

            Assert.IsTrue(button.HasDropDown)
            Assert.IsFalse(button.CenterText)
            Assert.IsNull(button.DropDownMenu)

        End Sub

        <TestMethod>
        Public Sub Button_ShouldSerializeDropDownMenu_OnlyWhenAMenuIsAssigned()

            Dim button As New DropDownActionButton()
            Dim method As MethodInfo = GetType(DropDownActionButton).GetMethod("ShouldSerializeDropDownMenu", NonPublicInstance)

            Assert.IsFalse(CBool(method.Invoke(button, Nothing)))

            button.DropDownMenu = New ContextMenuStrip()

            Assert.IsTrue(CBool(method.Invoke(button, Nothing)))

        End Sub

        <TestMethod>
        Public Sub Button_SpaceKey_RaisesMainAreaClick()

            Dim button As New DropDownActionButton() With {.HasDropDown = False}
            Dim raised As Integer = 0
            AddHandler button.MainAreaClick, Sub(sender As Object, e As EventArgs) raised += 1

            GetType(DropDownActionButton).GetMethod("OnKeyDown", NonPublicInstance).Invoke(button, New Object() {New KeyEventArgs(Keys.Space)})

            Assert.AreEqual(1, raised)

        End Sub

        <TestMethod>
        Public Sub Button_OpeningTheDropDownWithoutAMenu_Throws()

            Dim button As New DropDownActionButton()

            Dim ex As TargetInvocationException = Assert.ThrowsException(Of TargetInvocationException)(
                Sub() GetType(DropDownActionButton).GetMethod("OnKeyDown", NonPublicInstance).Invoke(button, New Object() {New KeyEventArgs(Keys.F4)}))

            Assert.IsInstanceOfType(ex.InnerException, GetType(InvalidOperationException))

        End Sub

#End Region

#Region " DarkMenuRenderer "

        <TestMethod>
        Public Sub Renderer_CanBeAssignedToAMenuAndUsesSquareEdges()

            Dim renderer As New DarkMenuRenderer()
            Dim menu As New ContextMenuStrip() With {.Renderer = renderer}

            Assert.IsFalse(renderer.RoundedEdges)
            Assert.AreSame(renderer, menu.Renderer)

        End Sub

#End Region

#Region " DarkTitleBar "

        <TestMethod>
        Public Sub DarkTitleBar_IsAppsDarkThemeEnabled_DoesNotThrow()

            Dim unused As Boolean = DarkTitleBar.IsAppsDarkThemeEnabled()

        End Sub

        <TestMethod>
        Public Sub DarkTitleBar_ZeroHandle_Throws()

            Assert.ThrowsException(Of ArgumentException)(Function() DarkTitleBar.SetDarkTitleBar(IntPtr.Zero, True))
            Assert.ThrowsException(Of ArgumentException)(Function() DarkTitleBar.ApplyCurrentTheme(IntPtr.Zero))

        End Sub

        <TestMethod>
        Public Sub DarkTitleBar_RealWindow_DoesNotThrow()

            Using window As New Form()
                Dim handle As IntPtr = window.Handle ' Forces the creation of the window handle.

                ' The result depends on the Windows version; the call must never throw.
                Dim unusedEnable As Boolean = DarkTitleBar.SetDarkTitleBar(handle, True)
                Dim unusedDisable As Boolean = DarkTitleBar.SetDarkTitleBar(handle, False)
                Dim unusedTheme As Boolean = DarkTitleBar.ApplyCurrentTheme(handle)
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
