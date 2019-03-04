Imports System.IO
Imports System.Drawing.Imaging

' ***********************************************************************
' Author   : Elektro
' Modified : 19-January-2015
' ***********************************************************************
' <copyright file="ImageTools.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Namespace Tools

    ''' <summary>
    ''' Class ImageTools.
    ''' </summary>
    Public NotInheritable Class ImageTools

        ' Resize Image
        ' By Elektro
        '
        ' Usage Examples:
        '
        ' PictureBox1.BackgroundImage = ResizeImage(System.Drawing.Image.FromFile("C:\Image.png"), Width:=256,  Height:=256)
        ' PictureBox1.BackgroundImage = ResizeImage(System.Drawing.ImageBitmap.FromFile("C:\Image.png"), Width:=256,  Height:=256)
        ' PictureBox1.BackgroundImage = ResizeImage(System.Drawing.Image.FromFile("C:\Image.png"), Percent:=50.0R)
        ' PictureBox1.BackgroundImage = ResizeImage(System.Drawing.Bitmap.FromFile("C:\Image.png"), Percent:=50.0R)

        ''' <summary>
        ''' Resizes an image.
        ''' </summary>
        ''' <param name="Bitmap">Indicates the image.</param>
        ''' <param name="Width">Indicates the new width.</param>
        ''' <param name="Height">Indicates the new height.</param>
        ''' <returns>Bitmap.</returns>
        Public Shared Function ResizeImage(ByVal bitmap As Drawing.Bitmap,
                                    ByVal width As Integer,
                                    ByVal height As Integer,
                                    Optional ByVal quality As Drawing2D.InterpolationMode =
                                                              Drawing2D.InterpolationMode.HighQualityBicubic,
                                    Optional ByVal pixelFormat As Imaging.PixelFormat =
                                                                  Imaging.PixelFormat.Format24bppRgb) As Drawing.Bitmap

            Dim newBitmap As New Bitmap(width, height, pixelFormat)

            Using g As Graphics = Graphics.FromImage(newBitmap)
                g.InterpolationMode = quality
                g.DrawImage(bitmap, 0I, 0I, newBitmap.Width, newBitmap.Height)
            End Using

            Return newBitmap

        End Function

        ''' <summary>
        ''' Resizes an image by a percentage.
        ''' </summary>
        ''' <param name="Bitmap">Indicates the image to resize.</param>
        ''' <param name="Percent">Indicates the percent size.</param>
        ''' <returns>Bitmap.</returns>
        Public Shared Function ResizeImage(ByVal bitmap As Drawing.Bitmap,
                                    ByVal percent As Double,
                                    Optional ByVal quality As Drawing2D.InterpolationMode =
                                                              Drawing2D.InterpolationMode.HighQualityBicubic,
                                    Optional ByVal pixelFormat As Imaging.PixelFormat =
                                                                  Imaging.PixelFormat.Format24bppRgb) As Drawing.Bitmap

            Dim width As Integer = (bitmap.Width \ (100I / percent))
            Dim height As Integer = (bitmap.Height \ (100I / percent))

            Dim newBitmap As New Bitmap(width, height, pixelFormat)

            Using g As Graphics = Graphics.FromImage(newBitmap)
                g.InterpolationMode = quality
                g.DrawImage(bitmap, 0, 0, width, height)
            End Using

            Return newBitmap

        End Function

        ''' <summary>
        ''' Resizes an image.
        ''' </summary>
        ''' <param name="Image">Indicates the image to resize.</param>
        ''' <param name="Width">Indicates the new width.</param>
        ''' <param name="Height">Indicates the new height.</param>
        ''' <returns>Bitmap.</returns>
        Public Shared Function ResizeImage(ByVal image As Drawing.Image,
                                    ByVal width As Integer,
                                    ByVal height As Integer,
                                    Optional ByVal quality As Drawing2D.InterpolationMode =
                                                              Drawing2D.InterpolationMode.HighQualityBicubic,
                                    Optional ByVal pixelFormat As Imaging.PixelFormat =
                                                                  Imaging.PixelFormat.Format24bppRgb) As Drawing.Image

            Return DirectCast(ResizeImage(DirectCast(image, Drawing.Bitmap), width, height, quality, pixelFormat), 
                              Drawing.Image)

        End Function

        ''' <summary>
        ''' Resize an image by the specified percentage.
        ''' </summary>
        ''' <param name="Image">Indicates the image.</param>
        ''' <param name="Percent">Indicates the percentage.</param>
        ''' <returns>Bitmap.</returns>
        Public Shared Function ResizeImage(ByVal image As Drawing.Image,
                                    ByVal percent As Double,
                                    Optional ByVal quality As Drawing2D.InterpolationMode =
                                                              Drawing2D.InterpolationMode.HighQualityBicubic,
                                    Optional ByVal pixelFormat As Imaging.PixelFormat =
                                                                  Imaging.PixelFormat.Format24bppRgb) As Drawing.Image

            Return DirectCast(ResizeImage(DirectCast(image, Drawing.Bitmap), percent, quality, pixelFormat), 
                              Drawing.Image)

        End Function

        ''' <summary>
        ''' Compress an image to the specified target filesize.
        ''' </summary>
        ''' <param name="inputFile">The input image file.</param>
        ''' <param name="targettFile">The target image file.</param>
        ''' <param name="targetImageFormat">The target image format.</param>
        ''' <param name="targetFileSize">The target filesize, in bytes.</param>
        ''' <exception cref="System.NotImplementedException">Resize Image to -1% and reset quality compression...</exception>
        Public Shared Sub CompressImage(ByVal inputFile As String,
                                        ByVal targettFile As String,
                                        ByVal targetImageFormat As ImageFormat,
                                        ByVal targetFileSize As Long)

            Dim qualityPercent As Integer = 100
            Dim resizePercent As Integer = 100
            Dim bmp As New Bitmap(inputFile)
            Dim codecInfo As ImageCodecInfo = (From codec As ImageCodecInfo In ImageCodecInfo.GetImageDecoders
                                               Where codec.FormatID = targetImageFormat.Guid).First
            Dim encoder As Imaging.Encoder = Imaging.Encoder.Quality
            Dim encoderParameters As New EncoderParameters(1)

            Using encoderParameter As New EncoderParameter(encoder, qualityPercent)
                encoderParameters.Param(0) = encoderParameter
                bmp.Save(targettFile, codecInfo, encoderParameters)
            End Using

            Dim fInfo As New FileInfo(targettFile)

            Do Until fInfo.Length <= targetFileSize

                qualityPercent -= 1

                If qualityPercent = 50 Then
                    resizePercent -= 2
                    bmp = ResizeImage(bmp, resizePercent)
                    qualityPercent = 90
                End If

                Using encoderParameter As New EncoderParameter(encoder, qualityPercent)
                    encoderParameters.Param(0) = encoderParameter
                    bmp.Save(targettFile, codecInfo, encoderParameters)
                End Using
                fInfo = New FileInfo(targettFile)

            Loop

            encoderParameters.Dispose()
            bmp.Dispose()

        End Sub

    End Class

End Namespace