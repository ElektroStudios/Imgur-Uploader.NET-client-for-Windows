
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

#Region " ImageToolsTests "

Namespace Tests

    ''' <summary>
    ''' Tests for <see cref="ImageTools"/> and <see cref="ImagePreviewLoader"/>.
    ''' </summary>
    <TestClass>
    Public Class ImageToolsTests

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

        Private Shared Function CreateSolidBitmap(width As Integer, height As Integer, color As Color, pixelFormat As PixelFormat) As Bitmap

            Dim bmp As New Bitmap(width, height, pixelFormat)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(color)
            End Using
            Return bmp

        End Function

#Region " ImageTools.ResizeImage "

        <TestMethod>
        Public Sub ResizeImage_50Percent_HalvesTheSize()

            Using source As Bitmap = CreateSolidBitmap(100, 50, Color.Red, PixelFormat.Format24bppRgb)
                Using result As Bitmap = ImageTools.ResizeImage(source, 50.0R)
                    Assert.AreEqual(50, result.Width)
                    Assert.AreEqual(25, result.Height)
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub ResizeImage_100Percent_KeepsTheSize()

            Using source As Bitmap = CreateSolidBitmap(64, 32, Color.Blue, PixelFormat.Format24bppRgb)
                Using result As Bitmap = ImageTools.ResizeImage(source, 100.0R)
                    Assert.AreEqual(64, result.Width)
                    Assert.AreEqual(32, result.Height)
                End Using
            End Using

        End Sub

#End Region

#Region " ImageTools.Resize "

        <TestMethod>
        Public Sub Resize_WithoutAspectRatio_StretchesToTheRequestedSize()

            Using source As Bitmap = CreateSolidBitmap(200, 100, Color.Red, PixelFormat.Format24bppRgb)
                Using result As Image = ImageTools.Resize(source, New Size(50, 50), keepAspectRatio:=False)
                    Assert.AreEqual(50, result.Width)
                    Assert.AreEqual(50, result.Height)
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub Resize_WithAspectRatio_CentersTheImageAndAddsBarsOnTheShortSide()

            Using source As Bitmap = CreateSolidBitmap(200, 100, Color.Red, PixelFormat.Format24bppRgb)
                Using result As Bitmap = DirectCast(ImageTools.Resize(source, New Size(100, 100), keepAspectRatio:=True), Bitmap)

                    Assert.AreEqual(100, result.Width)
                    Assert.AreEqual(100, result.Height)

                    Dim center As Color = result.GetPixel(50, 50)
                    Assert.IsTrue(center.R > 200 AndAlso center.G < 50 AndAlso center.B < 50, "The center must be red.")

                    Dim bar As Color = result.GetPixel(50, 5)
                    Assert.IsTrue(bar.R = 0 AndAlso bar.G = 0 AndAlso bar.B = 0, "The bar must be black.")

                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub Resize_NonPositiveWidth_Throws()

            Using source As Bitmap = CreateSolidBitmap(10, 10, Color.Red, PixelFormat.Format24bppRgb)
                Assert.Throws(Of ArgumentException)(Function() ImageTools.Resize(source, New Size(0, 10), False))
            End Using

        End Sub

        <TestMethod>
        Public Sub Resize_NonPositiveHeight_Throws()

            Using source As Bitmap = CreateSolidBitmap(10, 10, Color.Red, PixelFormat.Format24bppRgb)
                Assert.Throws(Of ArgumentException)(Function() ImageTools.Resize(source, New Size(10, 0), False))
            End Using

        End Sub

#End Region

#Region " ImageTools.CompressImage "

        <TestMethod>
        Public Sub CompressImage_ProducesAFileNotBiggerThanTheTarget()

            Dim inputFile As String = Path.Combine(Me.tempDirectory, "input.jpg")
            Dim outputFile As String = Path.Combine(Me.tempDirectory, "output.jpg")
            Const targetSize As Long = 100L * 1024L

            Using bmp As New Bitmap(400, 400, PixelFormat.Format24bppRgb)
                Dim random As New Random(1234)
                For y As Integer = 0 To (bmp.Height - 1)
                    For x As Integer = 0 To (bmp.Width - 1)
                        Dim noise As Integer = random.Next(-12, 13)
                        bmp.SetPixel(x, y, Color.FromArgb(Clamp(x \ 2 + noise), Clamp(y \ 2 + noise), Clamp(128 + noise)))
                    Next
                Next
                bmp.Save(inputFile, ImageFormat.Jpeg)
            End Using

            ImageTools.CompressImage(inputFile, outputFile, ImageFormat.Jpeg, targetSize)

            Assert.IsTrue(File.Exists(outputFile))
            Assert.IsTrue(New FileInfo(outputFile).Length <= targetSize)

        End Sub

        Private Shared Function Clamp(value As Integer) As Integer
            Return Math.Max(0, Math.Min(255, value))
        End Function

#End Region

#Region " ImagePreviewLoader.GetScaledSize "

        <TestMethod>
        Public Sub GetScaledSize_ImageThatFits_IsNotEnlarged()

            Assert.AreEqual(New Size(100, 50), ImagePreviewLoader.GetScaledSize(100, 50, 1920))

        End Sub

        <TestMethod>
        Public Sub GetScaledSize_WideImage_IsLimitedByWidth()

            Assert.AreEqual(New Size(1000, 500), ImagePreviewLoader.GetScaledSize(4000, 2000, 1000))

        End Sub

        <TestMethod>
        Public Sub GetScaledSize_TallImage_IsLimitedByHeight()

            Assert.AreEqual(New Size(500, 1000), ImagePreviewLoader.GetScaledSize(2000, 4000, 1000))

        End Sub

        <TestMethod>
        Public Sub GetScaledSize_ExtremeAspectRatio_NeverReturnsZero()

            Dim result As Size = ImagePreviewLoader.GetScaledSize(100000, 1, 100)

            Assert.AreEqual(100, result.Width)
            Assert.AreEqual(1, result.Height)

        End Sub

        <TestMethod>
        Public Sub GetScaledSize_InvalidArguments_Throw()

            Assert.Throws(Of ArgumentOutOfRangeException)(Function() ImagePreviewLoader.GetScaledSize(0, 10, 10))
            Assert.Throws(Of ArgumentOutOfRangeException)(Function() ImagePreviewLoader.GetScaledSize(10, 0, 10))
            Assert.Throws(Of ArgumentOutOfRangeException)(Function() ImagePreviewLoader.GetScaledSize(10, 10, 0))

        End Sub

#End Region

#Region " ImagePreviewLoader.TryLoad "

        <TestMethod>
        Public Sub TryLoad_ValidImage_ReturnsAPreviewWithTheSameAspectRatio()

            Dim file As String = Path.Combine(Me.tempDirectory, "wide.png")
            Using source As Bitmap = CreateSolidBitmap(400, 200, Color.Green, PixelFormat.Format32bppArgb)
                source.Save(file, ImageFormat.Png)
            End Using

            Using preview As Bitmap = ImagePreviewLoader.TryLoad(file, 100)
                Assert.IsNotNull(preview)
                Assert.AreEqual(100, preview.Width)
                Assert.AreEqual(50, preview.Height)
            End Using

        End Sub

        <TestMethod>
        Public Sub TryLoad_DoesNotKeepTheFileLocked()

            Dim file As String = Path.Combine(Me.tempDirectory, "locked.png")
            Using source As Bitmap = CreateSolidBitmap(20, 20, Color.Green, PixelFormat.Format32bppArgb)
                source.Save(file, ImageFormat.Png)
            End Using

            Using preview As Bitmap = ImagePreviewLoader.TryLoad(file, 100)
                Assert.IsNotNull(preview)
                IO.File.Delete(file) ' Throws if the file is still locked.
            End Using

            Assert.IsFalse(IO.File.Exists(file))

        End Sub

        <TestMethod>
        Public Sub TryLoad_NotAnImage_ReturnsNothing()

            Dim file As String = Path.Combine(Me.tempDirectory, "document.pdf")
            IO.File.WriteAllText(file, "%PDF-1.4 this is not an image")

            Assert.IsNull(ImagePreviewLoader.TryLoad(file, 100))

        End Sub

        <TestMethod>
        Public Sub TryLoad_MissingFile_ReturnsNothing()

            Assert.IsNull(ImagePreviewLoader.TryLoad(Path.Combine(Me.tempDirectory, "missing.png"), 100))

        End Sub

        <TestMethod>
        Public Sub TryLoad_InvalidArguments_Throw()

            Assert.Throws(Of ArgumentException)(Function() ImagePreviewLoader.TryLoad(String.Empty, 100))
            Assert.Throws(Of ArgumentOutOfRangeException)(Function() ImagePreviewLoader.TryLoad("any.png", 0))

        End Sub

#End Region

    End Class

End Namespace

#End Region
