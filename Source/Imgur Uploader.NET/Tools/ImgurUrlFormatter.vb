
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

#End Region

#Region " ImgurUrlSize "

    ''' <summary>
    ''' Specifies which size of an uploaded image is used.
    ''' The order of the values must match the order of the items of the size selector.
    ''' </summary>
    Public Enum ImgurUrlSize As Integer

        ''' <summary>Original image.</summary>
        Normal = 0

        ''' <summary>Small square thumbnail (90x90, suffix 's').</summary>
        SmallSquare = 1

        ''' <summary>Big square thumbnail (160x160, suffix 'b').</summary>
        BigSquare = 2

        ''' <summary>Small thumbnail (160x160 keeping proportions, suffix 't').</summary>
        SmallThumbnail = 3

        ''' <summary>Medium thumbnail (320x320 keeping proportions, suffix 'm').</summary>
        MediumThumbnail = 4

        ''' <summary>Large thumbnail (640x640 keeping proportions, suffix 'l').</summary>
        LargeThumbnail = 5

        ''' <summary>Huge thumbnail (1024x1024 keeping proportions, suffix 'h').</summary>
        HugeThumbnail = 6

    End Enum

#End Region

#Region " ImgurUrlFormat "

Namespace Tools

    ''' <summary>
    ''' Specifies the output format of an image URL.
    ''' The order of the values must match the order of the items of the format selector.
    ''' </summary>
    Public Enum ImgurUrlFormat As Integer

        ''' <summary>The direct URL of the image file.</summary>
        DirectUrl = 0

        ''' <summary>The URL of the Imgur page of the image.</summary>
        ShareLink = 1

        ''' <summary>A CSS background-image declaration.</summary>
        Css = 2

        ''' <summary>An HTML img tag.</summary>
        Html = 3

        ''' <summary>The official Imgur HTML embed code.</summary>
        HtmlEmbed = 4

        ''' <summary>A BBCode img tag (forums).</summary>
        BBCode = 5

        ''' <summary>The official Imgur Markdown link (Reddit).</summary>
        Markdown = 6

        ''' <summary>A Markdown image.</summary>
        MarkdownImage = 7

    End Enum

#End Region

#Region " ImgurUrlFormatter "

    ''' <summary>
    ''' Builds formatted URLs and embed codes (direct URL, share link, CSS, HTML, HTML embed, BBCode, Markdown)
    ''' from an <see cref="ImgurImage"/>.
    ''' </summary>
    Public NotInheritable Class ImgurUrlFormatter

        ''' <summary>
        ''' Prevents instantiation of the class.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Gets the URL of the image file for the specified size.
        ''' </summary>
        ''' <param name="image">The uploaded image.</param>
        ''' <param name="size">The size to retrieve.</param>
        ''' <returns>The URL.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="size"/> is not a valid value.</exception>
        Public Shared Function GetUrl(image As ImgurImage, size As ImgurUrlSize) As String

            If image Is Nothing Then
                Throw New ArgumentNullException(NameOf(image))
            End If

            Select Case size
                Case ImgurUrlSize.Normal
                    Return image.Normal
                Case ImgurUrlSize.SmallSquare
                    Return image.SmallSquare
                Case ImgurUrlSize.BigSquare
                    Return image.BigSquare
                Case ImgurUrlSize.SmallThumbnail
                    Return image.SmallThumbnail
                Case ImgurUrlSize.MediumThumbnail
                    Return image.MediumThumbnail
                Case ImgurUrlSize.LargeThumbnail
                    Return image.LargeThumbnail
                Case ImgurUrlSize.HugeThumbnail
                    Return image.HugeThumbnail
                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(size), CInt(size), GetType(ImgurUrlSize))
            End Select

        End Function

        ''' <summary>
        ''' Gets the image URL of the specified size, formatted as requested.
        ''' Thumbnails are wrapped in a link to the original image in the formats that support links.
        ''' </summary>
        ''' <param name="image">The uploaded image.</param>
        ''' <param name="size">The size to use.</param>
        ''' <param name="urlFormat">The output format.</param>
        ''' <returns>The formatted text.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="size"/> or <paramref name="urlFormat"/> is not a valid value.</exception>
        Public Shared Function Format(image As ImgurImage, size As ImgurUrlSize, urlFormat As ImgurUrlFormat) As String

            Dim url As String = ImgurUrlFormatter.GetUrl(image, size)
            Dim isThumbnail As Boolean = (size <> ImgurUrlSize.Normal)

            Select Case urlFormat
                Case ImgurUrlFormat.DirectUrl
                    Return url

                Case ImgurUrlFormat.ShareLink
                    Return image.ShareLink

                Case ImgurUrlFormat.Css
                    Return $"background-image: url('{url}');"

                Case ImgurUrlFormat.Html
                    Dim tag As String = $"<img src=""{url}"" alt=""Imgur"" />"
                    Return If(isThumbnail, $"<a href=""{image.Normal}"">{tag}</a>", tag)

                Case ImgurUrlFormat.HtmlEmbed
                    Return $"<blockquote class=""imgur-embed-pub"" lang=""en"" data-id=""{image.Id}""><a href=""{image.ShareLink}"">View post on imgur.com</a></blockquote><script async src=""//s.imgur.com/min/embed.js"" charset=""utf-8""></script>"

                Case ImgurUrlFormat.BBCode
                    Dim tag As String = $"[img]{url}[/img]"
                    Return If(isThumbnail, $"[url={image.Normal}]{tag}[/url]", tag)

                Case ImgurUrlFormat.Markdown
                    Return $"[Imgur]({image.ShareLink})"

                    ' Official Imgur code for the original size; a clickable thumbnail (linking to the Imgur post) otherwise.
                    Return If(isThumbnail, $"[![Imgur]({url})]({image.ShareLink})", $"[Imgur]({image.ShareLink})")

                Case ImgurUrlFormat.MarkdownImage
                    Dim tag As String = $"![Imgur]({url})"
                    Return If(isThumbnail, $"[{tag}]({image.Normal})", tag)

                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(urlFormat), CInt(urlFormat), GetType(ImgurUrlFormat))
            End Select

        End Function

        ''' <summary>
        ''' Determines whether the specified format depends on the selected image size.
        ''' The share link, the HTML embed code and the official Markdown link always refer to the whole Imgur post.
        ''' </summary>
        ''' <param name="urlFormat">The output format.</param>
        ''' <returns><see langword="True"/> if the size is used by the format; otherwise, <see langword="False"/>.</returns>
        Public Shared Function UsesSize(urlFormat As ImgurUrlFormat) As Boolean

            Return Not ((urlFormat = ImgurUrlFormat.ShareLink) OrElse
                        (urlFormat = ImgurUrlFormat.HtmlEmbed) OrElse
                        (urlFormat = ImgurUrlFormat.Markdown))

        End Function

        ''' <summary>
        ''' Determines whether the text produced for the specified format is a URL that can be opened in a web browser.
        ''' </summary>
        ''' <param name="urlFormat">The output format.</param>
        ''' <returns><see langword="True"/> for <see cref="ImgurUrlFormat.DirectUrl"/> and <see cref="ImgurUrlFormat.ShareLink"/>; otherwise, <see langword="False"/>.</returns>
        Public Shared Function IsOpenable(urlFormat As ImgurUrlFormat) As Boolean

            Return (urlFormat = ImgurUrlFormat.DirectUrl) OrElse (urlFormat = ImgurUrlFormat.ShareLink)

        End Function

    End Class

End Namespace

#End Region
