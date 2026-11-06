
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

#End Region

#Region " IniFileManagerTests "

Namespace Tests

    ''' <summary>
    ''' Tests for <see cref="IniFileManager"/>.
    ''' The class stores its state in shared fields, so every test uses its own temporary file
    ''' and the tests of this class must not run in parallel (MSTest does not by default).
    ''' </summary>
    <TestClass>
    Public Class IniFileManagerTests

        Private iniPath As String

        <TestInitialize>
        Public Sub Setup()

            Me.iniPath = Path.Combine(Path.GetTempPath(), $"ImgurUploaderTests_{Guid.NewGuid():N}.ini")
            IniFileManager.FilePath = Me.iniPath

        End Sub

        <TestCleanup>
        Public Sub Cleanup()

            If File.Exists(Me.iniPath) Then
                IniFileManager.File.Delete()
            End If

        End Sub

        Private Shared Sub AssertThrowsSectionNameError(action As Action)

            Try
                action()
            Catch ex As Exception
                Assert.AreEqual("SectionNameInvalidFormatException", ex.GetType().Name)
                Return
            End Try

            Assert.Fail("A section-name format exception was expected.")

        End Sub

        <TestMethod>
        Public Sub File_CreateAndDelete_ChangeTheFileExistence()

            Assert.IsFalse(IniFileManager.File.Exist())

            Assert.IsTrue(IniFileManager.File.Create())
            Assert.IsTrue(IniFileManager.File.Exist())

            Assert.IsTrue(IniFileManager.File.Delete())
            Assert.IsFalse(IniFileManager.File.Exist())

        End Sub

        <TestMethod>
        Public Sub Key_Get_WithoutFile_ReturnsTheDefaultValue()

            Assert.AreEqual("fallback", CStr(IniFileManager.Key.Get("ClientId", "fallback")))

        End Sub

        <TestMethod>
        Public Sub Key_SetThenGet_RoundTripsAValueWithoutSection()

            Assert.IsTrue(IniFileManager.Key.Set("ClientId", "abc123"))

            Assert.AreEqual("abc123", CStr(IniFileManager.Key.Get("ClientId")))
            Assert.IsTrue(IniFileManager.Key.Exist("ClientId"))

        End Sub

        <TestMethod>
        Public Sub Key_Set_ExistingKey_ReplacesTheValue()

            IniFileManager.Key.Set("ClientId", "old")
            IniFileManager.Key.Set("ClientId", "new")

            Assert.AreEqual("new", CStr(IniFileManager.Key.Get("ClientId")))
            Assert.AreEqual(1, File.ReadAllLines(Me.iniPath).Count(Function(line As String) line.StartsWith("ClientId=")))

        End Sub

        <TestMethod>
        Public Sub Key_Get_ValueContainingEqualsSign_ReturnsEverythingAfterTheFirstOne()

            IniFileManager.Key.Set("Token", "a=b=c")

            Assert.AreEqual("a=b=c", CStr(IniFileManager.Key.Get("Token")))

        End Sub

        <TestMethod>
        Public Sub Key_Get_MissingKey_ReturnsTheDefaultValue()

            IniFileManager.Key.Set("ClientId", "abc")

            Assert.AreEqual("none", CStr(IniFileManager.Key.Get("Missing", "none")))
            Assert.IsFalse(IniFileManager.Key.Exist("Missing"))

        End Sub

        <TestMethod>
        Public Sub Key_Get_IsCaseInsensitiveForTheKeyName()

            IniFileManager.Key.Set("ClientId", "abc")

            Assert.AreEqual("abc", CStr(IniFileManager.Key.Get("clientid")))

        End Sub

        <TestMethod>
        Public Sub Key_Remove_DeletesTheKey()

            IniFileManager.Key.Set("ClientId", "abc")

            Assert.IsTrue(IniFileManager.Key.Remove("ClientId"))
            Assert.IsFalse(IniFileManager.Key.Exist("ClientId"))
            Assert.IsFalse(IniFileManager.Key.Remove("ClientId"))

        End Sub

        <TestMethod>
        Public Sub Section_Add_CreatesTheSectionOnlyOnce()

            Assert.IsTrue(IniFileManager.Section.Add("[API]"))
            Assert.IsFalse(IniFileManager.Section.Add("[api]"))

            IniFileManager.File.Get()
            Assert.IsTrue(IniFileManager.Section.Has())
            CollectionAssert.AreEqual(New String() {"[API]"}, IniFileManager.Section.GetNames())

        End Sub

        <TestMethod>
        Public Sub Key_SetThenGet_RoundTripsAValueInsideASection()

            IniFileManager.Section.Add("[API]")
            IniFileManager.Key.Set("ClientId", "abc", "[API]")
            IniFileManager.Key.Set("ClientSecret", "def", "[API]")

            Assert.AreEqual("abc", CStr(IniFileManager.Key.Get("ClientId", String.Empty, "[API]")))
            Assert.AreEqual("def", CStr(IniFileManager.Key.Get("ClientSecret", String.Empty, "[API]")))
            Assert.IsTrue(IniFileManager.Key.Exist("ClientId", "[API]"))

        End Sub

        <TestMethod>
        Public Sub Key_Set_InANewSection_AppendsTheSectionAndTheKey()

            IniFileManager.Key.Set("First", "1", "[One]")
            IniFileManager.Key.Set("Second", "2", "[Two]")

            Assert.AreEqual("1", CStr(IniFileManager.Key.Get("First", String.Empty, "[One]")))
            Assert.AreEqual("2", CStr(IniFileManager.Key.Get("Second", String.Empty, "[Two]")))

        End Sub

        <TestMethod>
        Public Sub Section_Remove_DeletesTheSectionAndItsKeys()

            IniFileManager.Key.Set("First", "1", "[One]")
            IniFileManager.Key.Set("Second", "2", "[Two]")

            Assert.IsTrue(IniFileManager.Section.Remove("[One]"))

            IniFileManager.File.Get()
            CollectionAssert.AreEqual(New String() {"[Two]"}, IniFileManager.Section.GetNames())
            Assert.IsFalse(IniFileManager.Section.Remove("[Missing]"))

        End Sub

        <TestMethod>
        Public Sub Section_GetNames_AreSortedAlphabetically()

            IniFileManager.Section.Add("[Zeta]")
            IniFileManager.Section.Add("[Alpha]")

            IniFileManager.File.Get()
            CollectionAssert.AreEqual(New String() {"[Alpha]", "[Zeta]"}, IniFileManager.Section.GetNames())

        End Sub

        <TestMethod>
        Public Sub InvalidSectionName_Throws()

            IniFileManager.File.Create()

            AssertThrowsSectionNameError(Sub() IniFileManager.Section.Add("API"))
            AssertThrowsSectionNameError(Sub() IniFileManager.Section.Remove("API"))
            AssertThrowsSectionNameError(Sub() IniFileManager.Key.Get("ClientId", String.Empty, "API"))

        End Sub

    End Class

End Namespace

#End Region
