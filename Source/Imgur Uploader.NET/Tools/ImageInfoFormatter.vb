
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO

#End Region

#Region " ImageInfoFormatter "

Namespace Tools

    ''' <summary>
    ''' Builds the short descriptions of an image file that are shown in the user interface.
    ''' </summary>
    Public NotInheritable Class ImageInfoFormatter

        ''' <summary>
        ''' Separates the parts of the details text.
        ''' </summary>
        Private Const Separator As String = "  |  "

        ''' <summary>
        ''' Prevents instantiation of the class.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Gets the details text of an image file: extension, size and (if known) resolution.
        ''' For example: "PNG  |  1.25 MB  |  1920x1080".
        ''' </summary>
        ''' <param name="imageFile">The image file.</param>
        ''' <param name="resolution">The image resolution, or <see langword="Nothing"/> if it is unknown (for example, a PDF).</param>
        ''' <returns>The details text.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="imageFile"/> is <see langword="Nothing"/>.</exception>
        Public Shared Function GetDetails(imageFile As FileInfo, resolution As Size?) As String

            If imageFile Is Nothing Then
                Throw New ArgumentNullException(NameOf(imageFile))
            End If

            Dim parts As New List(Of String)

            Dim extension As String = imageFile.Extension.TrimStart("."c).ToUpperInvariant()
            If extension.Length > 0 Then
                parts.Add(extension)
            End If

            parts.Add(ByteSizeFormatter.Format(imageFile.Length))

            If resolution.HasValue Then
                parts.Add($"{resolution.Value.Width}x{resolution.Value.Height}")
            End If

            Return String.Join(Separator, parts)

        End Function

    End Class

End Namespace

#End Region
