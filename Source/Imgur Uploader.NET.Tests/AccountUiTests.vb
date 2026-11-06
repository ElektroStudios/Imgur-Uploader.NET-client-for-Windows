
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports ImgurUploader.UserInterface
Imports ImgurUploader.UserInterface.CustomControls
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Reflection
Imports System.Windows.Forms

#End Region

#Region " AccountUiTests "

Namespace Tests

    ''' <summary>
    ''' Tests for the user interface of the account support: the new options of <see cref="DropDownActionButton"/>
    ''' (arrow width and button color) and the <see cref="AuthorizationDialog"/>.
    ''' The private members are invoked by reflection, as they can only be raised by real mouse input or by Windows.
    ''' </summary>
    <TestClass>
    Public Class AccountUiTests

        Private Const NonPublicInstance As BindingFlags = BindingFlags.Instance Or BindingFlags.NonPublic

        ''' <summary>
        ''' Returns the name of the zone (None, MainArea or ArrowArea) of a button that is located at the specified point.
        ''' </summary>
        Private Shared Function GetZoneName(button As DropDownActionButton, x As Integer, y As Integer) As String

            Dim method As MethodInfo = GetType(DropDownActionButton).GetMethod("GetZoneAt", NonPublicInstance)
            Return method.Invoke(button, New Object() {New Point(x, y)}).ToString()

        End Function

        ''' <summary>
        ''' Paints a button on a bitmap (without showing it) and returns the bitmap.
        ''' </summary>
        Private Shared Function Paint(button As DropDownActionButton) As Bitmap

            Dim bmp As New Bitmap(button.Width, button.Height, PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(bmp)
                Dim args As New PaintEventArgs(g, New Rectangle(0, 0, button.Width, button.Height))
                GetType(DropDownActionButton).GetMethod("OnPaint", NonPublicInstance).Invoke(button, New Object() {args})
            End Using

            Return bmp

        End Function

        ''' <summary>
        ''' Returns every control (recursively) of the specified type.
        ''' </summary>
        Private Shared Function FindControls(Of T As Control)(parent As Control) As List(Of T)

            Dim result As New List(Of T)

            For Each child As Control In parent.Controls
                If TypeOf child Is T Then
                    result.Add(DirectCast(child, T))
                End If
                result.AddRange(FindControls(Of T)(child))
            Next

            Return result

        End Function

#Region " DropDownActionButton: arrow width "

        <TestMethod>
        Public Sub Button_ArrowAreaWidth_DefaultsTo26()

            Using button As New DropDownActionButton()
                Assert.AreEqual(26, button.ArrowAreaWidth)
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_ArrowAreaWidth_TooSmall_Throws()

            Using button As New DropDownActionButton()
                Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() button.ArrowAreaWidth = 11)
                Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() button.ArrowAreaWidth = 0)
                Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() button.ArrowAreaWidth = -40)
                Assert.AreEqual(26, button.ArrowAreaWidth)
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_ArrowAreaWidth_MovesTheBoundaryBetweenTheZones()

            Using button As New DropDownActionButton() With {.Size = New Size(200, 30)}

                ' Default width (26): the arrow starts at x = 174.
                Assert.AreEqual("MainArea", GetZoneName(button, 173, 10))
                Assert.AreEqual("ArrowArea", GetZoneName(button, 174, 10))

                ' A wider arrow area (40): the arrow starts at x = 160.
                button.ArrowAreaWidth = 40
                Assert.AreEqual("MainArea", GetZoneName(button, 159, 10))
                Assert.AreEqual("ArrowArea", GetZoneName(button, 160, 10))
                Assert.AreEqual("ArrowArea", GetZoneName(button, 199, 10))

            End Using

        End Sub

        <TestMethod>
        Public Sub Button_Zones_OutsideTheButton_AreNone()

            Using button As New DropDownActionButton() With {.Size = New Size(200, 30)}
                Assert.AreEqual("None", GetZoneName(button, -1, 10))
                Assert.AreEqual("None", GetZoneName(button, 200, 10))
                Assert.AreEqual("None", GetZoneName(button, 10, 30))
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_WithoutDropDown_IsAllMainArea()

            Using button As New DropDownActionButton() With {.Size = New Size(200, 30), .HasDropDown = False, .ArrowAreaWidth = 40}
                Assert.AreEqual("MainArea", GetZoneName(button, 199, 10))
            End Using

        End Sub

        <TestMethod>
        Public Sub Selector_IsAllArrowArea_WhateverTheArrowWidth()

            Using selector As New DropDownSelector() With {.Size = New Size(200, 30)}
                selector.ArrowAreaWidth = 12
                Assert.AreEqual("ArrowArea", GetZoneName(selector, 5, 10))
            End Using

        End Sub

#End Region

#Region " DropDownActionButton: button color "

        <TestMethod>
        Public Sub Button_ButtonColor_DefaultsToEmpty()

            Using button As New DropDownActionButton()
                Assert.IsTrue(button.ButtonColor.IsEmpty)
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_ButtonColor_IsSerializedOnlyWhenItIsSet()

            Using button As New DropDownActionButton()

                Dim shouldSerialize As MethodInfo = GetType(DropDownActionButton).GetMethod("ShouldSerializeButtonColor", NonPublicInstance)
                Dim reset As MethodInfo = GetType(DropDownActionButton).GetMethod("ResetButtonColor", NonPublicInstance)

                Assert.IsFalse(CBool(shouldSerialize.Invoke(button, Nothing)))

                button.ButtonColor = Color.Green
                Assert.IsTrue(CBool(shouldSerialize.Invoke(button, Nothing)))

                reset.Invoke(button, Nothing)
                Assert.IsTrue(button.ButtonColor.IsEmpty)
                Assert.IsFalse(CBool(shouldSerialize.Invoke(button, Nothing)))

            End Using

        End Sub

        <TestMethod>
        Public Sub Button_Paint_WithoutButtonColor_UsesTheNeutralDarkFill()

            Using button As New DropDownActionButton() With {.Size = New Size(200, 30), .Text = "Test"}
                Using bmp As Bitmap = Paint(button)
                    Assert.AreEqual(Color.FromArgb(45, 45, 48).ToArgb(), bmp.GetPixel(3, 15).ToArgb())
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_Paint_WithButtonColor_FillsWithThatColor()

            Dim green As Color = Color.FromArgb(134, 197, 31)

            Using button As New DropDownActionButton() With {.Size = New Size(200, 30), .Text = "Test", .ButtonColor = green}
                Using bmp As Bitmap = Paint(button)
                    Assert.AreEqual(green.ToArgb(), bmp.GetPixel(3, 15).ToArgb())
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_Paint_Disabled_IgnoresTheButtonColor()

            Using button As New DropDownActionButton() With {.Size = New Size(200, 30), .Text = "Test", .ButtonColor = Color.FromArgb(134, 197, 31), .Enabled = False}
                Using bmp As Bitmap = Paint(button)
                    Assert.AreEqual(Color.FromArgb(34, 34, 36).ToArgb(), bmp.GetPixel(3, 15).ToArgb())
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub Button_Paint_WithAWideArrowArea_DoesNotThrow()

            Using button As New DropDownActionButton() With {.Size = New Size(265, 32), .Text = "Upload to my account", .CenterText = True, .ArrowAreaWidth = 40, .ButtonColor = Color.FromArgb(134, 197, 31), .ForeColor = Color.Black}
                Using bmp As Bitmap = Paint(button)
                    Assert.AreEqual(265, bmp.Width)
                End Using
            End Using

        End Sub

#End Region

#Region " AuthorizationDialog "

        <TestMethod>
        Public Sub Dialog_NullClient_Throws()

            Assert.ThrowsException(Of ArgumentNullException)(Function() New AuthorizationDialog(Nothing))

        End Sub

        <TestMethod>
        Public Sub Dialog_WithoutClientId_Throws()

            Dim client As New ImgurAPI(String.Empty, "clientSecret")

            Assert.ThrowsException(Of ArgumentException)(Function() New AuthorizationDialog(client))

        End Sub

        <TestMethod>
        Public Sub Dialog_IsAFixedModalDialog()

            Dim client As New ImgurAPI("dc3e850cd310751", "clientSecret")

            Using dialog As New AuthorizationDialog(client)
                Assert.AreEqual(FormBorderStyle.FixedDialog, dialog.FormBorderStyle)
                Assert.IsFalse(dialog.MaximizeBox)
                Assert.IsFalse(dialog.MinimizeBox)
                Assert.IsFalse(dialog.ShowInTaskbar)
            End Using

        End Sub

        <TestMethod>
        Public Sub Dialog_ShowsTheAuthorizationUrlOfTheClient()

            Dim client As New ImgurAPI("dc3e850cd310751", "clientSecret")

            Using dialog As New AuthorizationDialog(client)

                Dim urlBoxes As List(Of TextBox) = FindControls(Of TextBox)(dialog).Where(Function(t As TextBox) t.ReadOnly).ToList()

                Assert.AreEqual(1, urlBoxes.Count)
                Assert.AreEqual("https://api.imgur.com/oauth2/authorize?client_id=dc3e850cd310751&response_type=pin", urlBoxes(0).Text)

            End Using

        End Sub

        <TestMethod>
        Public Sub Dialog_HasAnEditablePinBoxAndThreeButtons()

            Dim client As New ImgurAPI("dc3e850cd310751", "clientSecret")

            Using dialog As New AuthorizationDialog(client)

                Assert.AreEqual(1, FindControls(Of TextBox)(dialog).Where(Function(t As TextBox) Not t.ReadOnly).Count())

                Dim captions As List(Of String) = FindControls(Of DropDownActionButton)(dialog).Select(Function(b As DropDownActionButton) b.Text).ToList()
                CollectionAssert.AreEquivalent(New String() {"Open page", "Authorize", "Cancel"}, captions)

                ' None of them has a drop-down arrow.
                Assert.IsTrue(FindControls(Of DropDownActionButton)(dialog).All(Function(b As DropDownActionButton) Not b.HasDropDown))

            End Using

        End Sub

        <TestMethod>
        Public Sub Dialog_DoesNotAuthorizeTheClientByItself()

            Dim client As New ImgurAPI("dc3e850cd310751", "clientSecret")

            Using dialog As New AuthorizationDialog(client)
                Assert.AreEqual(DialogResult.None, dialog.DialogResult)
            End Using

            Assert.IsFalse(client.IsAuthorized)

        End Sub

#End Region

    End Class

End Namespace

#End Region
