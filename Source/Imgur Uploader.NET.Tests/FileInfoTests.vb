
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

#End Region

#Region " FileInfoTests "

Namespace Tests

    ''' <summary>
    ''' Tests for <see cref="ByteSizeFormatter"/>, <see cref="ImageInfoFormatter"/> and <see cref="ImagePreviewLoader.TryGetResolution"/>.
    ''' </summary>
    <TestClass>
    Public Class FileInfoTests

        Private tempDirectory As String

        <TestInitialize>
        Public Sub Setup()

            Me.tempDirectory = Path.Combine(Path.GetTempPath(), $"ImgurUploaderTests_{Guid.NewGuid():N}")
            Directory.CreateDirectory(Me.tempDirectory)

        End Sub

        <TestCleanup>
        Public Sub Cleanup()

            If Directory.Exists(Me.tempDirectory) Then
                Directory.Delete(Me.tempDirectory, recursive:=True)
            End If

        End Sub

#Region " ByteSizeFormatter "

        <TestMethod>
        Public Sub ByteSize_Zero_IsFormattedInBytes()

            StringAssert.StartsWith(ByteSizeFormatter.Format(0L), "0")

        End Sub

        <TestMethod>
        Public Sub ByteSize_UsesTheWindowsUnits()

            StringAssert.Contains(ByteSizeFormatter.Format(1024L), "KB")
            StringAssert.Contains(ByteSizeFormatter.Format(5L * 1024L * 1024L), "MB")
            StringAssert.Contains(ByteSizeFormatter.Format(3L * 1024L * 1024L * 1024L), "GB")

        End Sub

        <TestMethod>
        Public Sub ByteSize_StartsWithTheScaledNumber()

            StringAssert.StartsWith(ByteSizeFormatter.Format(5L * 1024L * 1024L), "5")

        End Sub

        <TestMethod>
        Public Sub ByteSize_Negative_Throws()

            Assert.Throws(Of ArgumentOutOfRangeException)(Function() ByteSizeFormatter.Format(-1L))

        End Sub

#End Region

#Region " ImageInfoFormatter "

        <TestMethod>
        Public Sub Details_IncludeExtensionSizeAndResolution()

            Dim path As String = System.IO.Path.Combine(Me.tempDirectory, "photo.png")
            File.WriteAllBytes(path, New Byte(2047) {})

            Dim details As String = ImageInfoFormatter.GetDetails(New FileInfo(path), New Size(1920, 1080))

            StringAssert.StartsWith(details, "PNG")
            StringAssert.Contains(details, "KB")
            StringAssert.EndsWith(details, "1920x1080")

        End Sub

        <TestMethod>
        Public Sub Details_WithoutResolution_OmitTheResolutionPart()

            Dim path As String = System.IO.Path.Combine(Me.tempDirectory, "document.pdf")
            File.WriteAllBytes(path, New Byte(99) {})

            Dim details As String = ImageInfoFormatter.GetDetails(New FileInfo(path), Nothing)

            StringAssert.StartsWith(details, "PDF")
            Assert.AreEqual(2, details.Split(New String() {"|"}, StringSplitOptions.None).Length)

        End Sub

        <TestMethod>
        Public Sub Details_NullFile_Throws()

            Assert.Throws(Of ArgumentNullException)(Function() ImageInfoFormatter.GetDetails(Nothing, Nothing))

        End Sub

#End Region

#Region " ImagePreviewLoader.TryGetResolution "

        <TestMethod>
        Public Sub TryGetResolution_ValidImage_ReturnsItsSize()

            Dim path As String = System.IO.Path.Combine(Me.tempDirectory, "image.png")
            Using bmp As New Bitmap(320, 200)
                bmp.Save(path, ImageFormat.Png)
            End Using

            Dim resolution As Size? = ImagePreviewLoader.TryGetResolution(path)

            Assert.IsTrue(resolution.HasValue)
            Assert.AreEqual(New Size(320, 200), resolution.Value)

        End Sub

        <TestMethod>
        Public Sub TryGetResolution_NotAnImageOrMissing_ReturnsNothing()

            Dim path As String = System.IO.Path.Combine(Me.tempDirectory, "text.pdf")
            File.WriteAllText(path, "not an image")

            Assert.IsFalse(ImagePreviewLoader.TryGetResolution(path).HasValue)
            Assert.IsFalse(ImagePreviewLoader.TryGetResolution(System.IO.Path.Combine(Me.tempDirectory, "missing.png")).HasValue)

        End Sub

        <TestMethod>
        Public Sub TryGetResolution_EmptyPath_Throws()

            Assert.Throws(Of ArgumentException)(Function() ImagePreviewLoader.TryGetResolution(String.Empty))

        End Sub

#End Region

    End Class

End Namespace

#End Region
