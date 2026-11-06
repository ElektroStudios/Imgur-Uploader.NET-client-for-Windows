
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices

#End Region

#Region " ImagePreviewLoader "

Namespace Tools

    ''' <summary>
    ''' Loads preview bitmaps of image files without keeping the file locked
    ''' and without adding borders (the aspect ratio is preserved exactly).
    ''' </summary>
    Public NotInheritable Class ImagePreviewLoader

        ''' <summary>
        ''' Prevents instantiation of the class.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Calculates the size of an image scaled down to fit in a square of the specified side. Images that already fit are not enlarged.
        ''' </summary>
        ''' <param name="width">The original width.</param>
        ''' <param name="height">The original height.</param>
        ''' <param name="maxDimension">The maximum width and height.</param>
        ''' <returns>The scaled size, with a minimum of 1x1.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' <paramref name="width"/>, <paramref name="height"/> or <paramref name="maxDimension"/> is not greater than zero.
        ''' </exception>
        Public Shared Function GetScaledSize(width As Integer, height As Integer, maxDimension As Integer) As Size

            If width <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(width), width, "Value must be greater than zero.")
            End If
            If height <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(height), height, "Value must be greater than zero.")
            End If
            If maxDimension <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(maxDimension), maxDimension, "Value must be greater than zero.")
            End If

            If (width <= maxDimension) AndAlso (height <= maxDimension) Then
                Return New Size(width, height)
            End If

            Dim scale As Double = Math.Min(maxDimension / width, maxDimension / height)
            Return New Size(Math.Max(1, CInt(Math.Round(width * scale))),
                            Math.Max(1, CInt(Math.Round(height * scale))))

        End Function

        ''' <summary>
        ''' Tries to read the resolution of an image file (without decoding the whole image).
        ''' </summary>
        ''' <param name="filePath">The image file path.</param>
        ''' <returns>
        ''' The resolution in pixels, or <see langword="Nothing"/> if the file does not exist or is not a decodable image.
        ''' </returns>
        ''' <exception cref="ArgumentException"><paramref name="filePath"/> is null or empty.</exception>
        Public Shared Function TryGetResolution(filePath As String) As Size?

            If String.IsNullOrEmpty(filePath) Then
                Throw New ArgumentException("The file path is null or empty.", NameOf(filePath))
            End If

            Try
                Using stream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    Using source As Image = Image.FromStream(stream, useEmbeddedColorManagement:=False, validateImageData:=False)
                        Return New Size(source.Width, source.Height)
                    End Using
                End Using

            Catch ex As ArgumentException
                Return Nothing

            Catch ex As OutOfMemoryException
                Return Nothing ' GDI+ reports an invalid image format with this exception.

            Catch ex As ExternalException
                Return Nothing

            Catch ex As IOException
                Return Nothing

            Catch ex As UnauthorizedAccessException
                Return Nothing

            End Try

        End Function

        ''' <summary>
        ''' Tries to load an image file as a preview bitmap.
        ''' </summary>
        ''' <param name="filePath">The image file path.</param>
        ''' <param name="maxDimension">The maximum width and height of the preview.</param>
        ''' <returns>
        ''' The preview bitmap (the caller must dispose it), or <see langword="Nothing"/> if the file
        ''' does not exist, is not a decodable image (for example a PDF) or cannot be read.
        ''' </returns>
        ''' <exception cref="ArgumentException"><paramref name="filePath"/> is null or empty.</exception>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="maxDimension"/> is not greater than zero.</exception>
        Public Shared Function TryLoad(filePath As String, maxDimension As Integer) As Bitmap

            If String.IsNullOrEmpty(filePath) Then
                Throw New ArgumentException("The file path is null or empty.", NameOf(filePath))
            End If
            If maxDimension <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(maxDimension), maxDimension, "Value must be greater than zero.")
            End If

            Try
                Using stream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    Using source As Image = Image.FromStream(stream, useEmbeddedColorManagement:=False, validateImageData:=False)

                        Dim previewSize As Size = ImagePreviewLoader.GetScaledSize(source.Width, source.Height, maxDimension)
                        Dim preview As New Bitmap(previewSize.Width, previewSize.Height, PixelFormat.Format32bppPArgb)

                        Using g As Graphics = Graphics.FromImage(preview)
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality
                            g.DrawImage(source, 0, 0, previewSize.Width, previewSize.Height)
                        End Using

                        Return preview

                    End Using
                End Using

            Catch ex As ArgumentException
                Return Nothing

            Catch ex As OutOfMemoryException
                Return Nothing ' GDI+ reports an invalid image format with this exception.

            Catch ex As ExternalException
                Return Nothing

            Catch ex As IOException
                Return Nothing

            Catch ex As UnauthorizedAccessException
                Return Nothing

            End Try

        End Function

    End Class

End Namespace

#End Region
