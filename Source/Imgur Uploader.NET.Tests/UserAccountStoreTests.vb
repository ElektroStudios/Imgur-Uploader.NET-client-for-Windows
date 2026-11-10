
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

#Region " UserAccountStoreTests "

Namespace Tests

    ''' <summary>
    ''' Tests for <see cref="UserAccountStore"/>. Every test uses its own temporary file,
    ''' so the real account of the user is never touched.
    ''' </summary>
    <TestClass>
    Public Class UserAccountStoreTests

        Private originalFilePath As String
        Private testFolder As String

        <TestInitialize>
        Public Sub Setup()

            Me.originalFilePath = UserAccountStore.FilePath
            Me.testFolder = Path.Combine(Path.GetTempPath(), $"ImgurUploaderTests_{Guid.NewGuid():N}")
            UserAccountStore.FilePath = Path.Combine(Me.testFolder, "Sub", "Account.dat")

        End Sub

        <TestCleanup>
        Public Sub Cleanup()

            UserAccountStore.FilePath = Me.originalFilePath

            If Directory.Exists(Me.testFolder) Then
                Directory.Delete(Me.testFolder, True)
            End If

        End Sub

#Region " Default location "

        <TestMethod>
        Public Sub DefaultFilePath_IsInTheRoamingProfileOfTheUser()

            ' Setup replaced the path, so check the original one.
            Dim appData As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)

            Assert.IsTrue(Me.originalFilePath.StartsWith(appData, StringComparison.OrdinalIgnoreCase))
            Assert.AreEqual("Account.dat", Path.GetFileName(Me.originalFilePath))

        End Sub

#End Region

#Region " Account "

        <TestMethod>
        Public Sub TryLoadAccount_WithoutFile_ReturnsFalse()

            Dim token As String = "unchanged"
            Dim name As String = "unchanged"

            Assert.IsFalse(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual(String.Empty, token)
            Assert.AreEqual(String.Empty, name)

        End Sub

        <TestMethod>
        Public Sub SaveAccount_ThenLoad_RoundTripsTheValues()

            Assert.IsTrue(UserAccountStore.SaveAccount("refresh-token-123", "elektro"))

            Dim token As String = Nothing
            Dim name As String = Nothing

            Assert.IsTrue(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual("refresh-token-123", token)
            Assert.AreEqual("elektro", name)

        End Sub

        <TestMethod>
        Public Sub SaveAccount_CreatesTheFolder()

            Assert.IsFalse(Directory.Exists(Path.GetDirectoryName(UserAccountStore.FilePath)))

            Assert.IsTrue(UserAccountStore.SaveAccount("refresh-token-123", "elektro"))

            Assert.IsTrue(File.Exists(UserAccountStore.FilePath))

        End Sub

        <TestMethod>
        Public Sub SaveAccount_NeverWritesTheTokenInPlainText()

            UserAccountStore.SaveAccount("super-secret-refresh-token", "elektro")

            Dim content As String = File.ReadAllText(UserAccountStore.FilePath)

            Assert.IsFalse(content.Contains("super-secret-refresh-token"))
            StringAssert.Contains(content, "RefreshToken=")
            StringAssert.Contains(content, "Username=elektro")

        End Sub

        <TestMethod>
        Public Sub SaveAccount_UsernameWithEqualsSign_RoundTrips()

            UserAccountStore.SaveAccount("tok", "a=b=c")

            Dim token As String = Nothing
            Dim name As String = Nothing

            Assert.IsTrue(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual("a=b=c", name)

        End Sub

        <TestMethod>
        Public Sub SaveAccount_Twice_KeepsTheLastAccount()

            UserAccountStore.SaveAccount("first-token", "first")
            UserAccountStore.SaveAccount("second-token", "second")

            Dim token As String = Nothing
            Dim name As String = Nothing

            Assert.IsTrue(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual("second-token", token)
            Assert.AreEqual("second", name)
            Assert.AreEqual(1, File.ReadAllLines(UserAccountStore.FilePath).Count(Function(line As String) line.StartsWith("RefreshToken=", StringComparison.Ordinal)))

        End Sub

        <TestMethod>
        Public Sub SaveAccount_NullUsername_IsStoredAsEmpty()

            UserAccountStore.SaveAccount("tok", Nothing)

            Dim token As String = Nothing
            Dim name As String = "unchanged"

            Assert.IsTrue(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual(String.Empty, name)

        End Sub

        <TestMethod>
        Public Sub SaveAccount_EmptyToken_Throws()

            Assert.Throws(Of ArgumentException)(Function() UserAccountStore.SaveAccount(String.Empty, "elektro"))
            Assert.Throws(Of ArgumentException)(Function() UserAccountStore.SaveAccount("   ", "elektro"))
            Assert.Throws(Of ArgumentException)(Function() UserAccountStore.SaveAccount(Nothing, "elektro"))

        End Sub

        <TestMethod>
        Public Sub TryLoadAccount_CorruptToken_ReturnsFalse()

            Directory.CreateDirectory(Path.GetDirectoryName(UserAccountStore.FilePath))

            ' Not a base64 string, and a base64 string that is not a valid protected blob
            ' (this is what happens when the file is copied from another user or computer).
            File.WriteAllLines(UserAccountStore.FilePath, New String() {"RefreshToken=this is not base64!!", "Username=elektro"})
            Dim token As String = Nothing
            Dim name As String = Nothing
            Assert.IsFalse(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual(String.Empty, token)

            File.WriteAllLines(UserAccountStore.FilePath, New String() {"RefreshToken=" & Convert.ToBase64String(New Byte() {1, 2, 3, 4, 5, 6, 7, 8}), "Username=elektro"})
            Assert.IsFalse(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual(String.Empty, token)

        End Sub

        <TestMethod>
        Public Sub TryLoadAccount_EmptyTokenValue_ReturnsFalse()

            Directory.CreateDirectory(Path.GetDirectoryName(UserAccountStore.FilePath))
            File.WriteAllLines(UserAccountStore.FilePath, New String() {"RefreshToken=", "Username=elektro"})

            Dim token As String = Nothing
            Dim name As String = Nothing

            Assert.IsFalse(UserAccountStore.TryLoadAccount(token, name))

        End Sub

        <TestMethod>
        Public Sub ClearAccount_RemovesTheAccountButKeepsTheUploadMode()

            UserAccountStore.SaveUploadMode(ImgurUploadMode.Anonymous)
            UserAccountStore.SaveAccount("tok", "elektro")

            Assert.IsTrue(UserAccountStore.ClearAccount())

            Dim token As String = Nothing
            Dim name As String = Nothing
            Assert.IsFalse(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual(ImgurUploadMode.Anonymous, UserAccountStore.LoadUploadMode())

            Dim content As String = File.ReadAllText(UserAccountStore.FilePath)
            Assert.IsFalse(content.Contains("RefreshToken"))
            Assert.IsFalse(content.Contains("Username"))

        End Sub

        <TestMethod>
        Public Sub ClearAccount_WithoutFile_DoesNothingAndSucceeds()

            Assert.IsTrue(UserAccountStore.ClearAccount())
            Assert.IsFalse(File.Exists(UserAccountStore.FilePath))

        End Sub

        <TestMethod>
        Public Sub SaveAccount_UnwritableLocation_ReturnsFalse()

            ' A file where a folder is needed: the folder cannot be created.
            Directory.CreateDirectory(Me.testFolder)
            Dim blocker As String = Path.Combine(Me.testFolder, "blocker")
            File.WriteAllText(blocker, "x")
            UserAccountStore.FilePath = Path.Combine(blocker, "Account.dat")

            Assert.IsFalse(UserAccountStore.SaveAccount("tok", "elektro"))
            Assert.IsFalse(UserAccountStore.SaveUploadMode(ImgurUploadMode.Anonymous))

        End Sub

#End Region

#Region " Upload mode "

        <TestMethod>
        Public Sub LoadUploadMode_WithoutFile_DefaultsToAccount()

            Assert.AreEqual(ImgurUploadMode.Account, UserAccountStore.LoadUploadMode())

        End Sub

        <TestMethod>
        Public Sub SaveUploadMode_ThenLoad_RoundTrips()

            Assert.IsTrue(UserAccountStore.SaveUploadMode(ImgurUploadMode.Anonymous))
            Assert.AreEqual(ImgurUploadMode.Anonymous, UserAccountStore.LoadUploadMode())

            Assert.IsTrue(UserAccountStore.SaveUploadMode(ImgurUploadMode.Account))
            Assert.AreEqual(ImgurUploadMode.Account, UserAccountStore.LoadUploadMode())

        End Sub

        <TestMethod>
        Public Sub SaveUploadMode_KeepsTheStoredAccount()

            UserAccountStore.SaveAccount("tok", "elektro")

            UserAccountStore.SaveUploadMode(ImgurUploadMode.Anonymous)

            Dim token As String = Nothing
            Dim name As String = Nothing
            Assert.IsTrue(UserAccountStore.TryLoadAccount(token, name))
            Assert.AreEqual("tok", token)

        End Sub

        <TestMethod>
        Public Sub LoadUploadMode_InvalidValue_DefaultsToAccount()

            Directory.CreateDirectory(Path.GetDirectoryName(UserAccountStore.FilePath))

            File.WriteAllLines(UserAccountStore.FilePath, New String() {"UploadMode=Banana"})
            Assert.AreEqual(ImgurUploadMode.Account, UserAccountStore.LoadUploadMode())

            ' A number that is not a member of the enumeration.
            File.WriteAllLines(UserAccountStore.FilePath, New String() {"UploadMode=99"})
            Assert.AreEqual(ImgurUploadMode.Account, UserAccountStore.LoadUploadMode())

        End Sub

        <TestMethod>
        Public Sub LoadUploadMode_IgnoresTheCaseOfKeyAndValue()

            Directory.CreateDirectory(Path.GetDirectoryName(UserAccountStore.FilePath))
            File.WriteAllLines(UserAccountStore.FilePath, New String() {"uploadmode=anonymous"})

            Assert.AreEqual(ImgurUploadMode.Anonymous, UserAccountStore.LoadUploadMode())

        End Sub

        <TestMethod>
        Public Sub Store_FileWithUnrelatedOrMalformedLines_IsTolerated()

            Directory.CreateDirectory(Path.GetDirectoryName(UserAccountStore.FilePath))
            File.WriteAllLines(UserAccountStore.FilePath, New String() {"", "garbage without separator", "=novalue", "UploadMode=Anonymous"})

            Assert.AreEqual(ImgurUploadMode.Anonymous, UserAccountStore.LoadUploadMode())

        End Sub

#End Region

    End Class

End Namespace

#End Region
