Imports System.IO
Imports System.Drawing.Imaging

Namespace Tools

    ''' <summary>
    ''' Class ImageTools.
    ''' </summary>
    Public NotInheritable Class ImageTools

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

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Resizes an <see cref="Drawing.Image"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="size">
        ''' The new size.
        ''' </param>
        ''' 
        ''' <param name="keepAspectRatio">
        ''' <see langword="True"/> to preserve original image's aspect ratio; <see langword="False"/> otherwise.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' The resized <see cref="Drawing.Image"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Value greater than zero is required.;width
        ''' or
        ''' Value greater than zero is required.;height
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        <DebuggerStepThrough>
        Public Shared Function Resize(img As Drawing.Image,
                               size As Drawing.Size,
                               keepAspectRatio As Boolean) As Drawing.Image

            If (size.Width <= 0) Then
                Throw New ArgumentException(message:="Width bigger than zero is reqired.", paramName:=NameOf(size))

            ElseIf (size.Height <= 0) Then
                Throw New ArgumentException(message:="Height bigger than zero is reqired.", paramName:=NameOf(size))

            Else
                Dim bmp As New Drawing.Bitmap(size.Width, size.Height, img.PixelFormat)
                Using g As Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
                    If keepAspectRatio Then
                        ' Compute aspect ratio (our scaling factor)
                        Dim scaleHeight As Single = CSng(size.Height) / CSng(img.Height)
                        Dim scaleWidth As Single = CSng(size.Width) / CSng(img.Width)
                        Dim scale As Single = Math.Min(scaleHeight, scaleWidth)

                        ' Center image.
                        Dim shiftX As Single = 0, shiftY As Single = 0
                        If (size.Width > (img.Width * scale)) Then
                            shiftX = CSng(size.Width - (img.Width * scale)) / 2
                        End If
                        If (size.Height > (img.Height * scale)) Then
                            shiftY = CSng(size.Height - (img.Height * scale)) / 2
                        End If

                        g.DrawImage(img, New System.Drawing.RectangleF(shiftX, shiftY, img.Width * scale, img.Height * scale))
                    Else
                        g.DrawImage(img, New System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height))

                    End If
                End Using

                Return bmp

            End If

        End Function

    End Class

End Namespace