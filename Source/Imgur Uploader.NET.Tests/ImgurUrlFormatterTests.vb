
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports ImgurUploader.Tools
Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " ImgurUrlFormatterTests "

Namespace Tests

    ''' <summary>
    ''' Tests for <see cref="ImgurUrlFormatter"/>.
    ''' </summary>
    <TestClass>
    Public Class ImgurUrlFormatterTests

        Private Const Base As String = "https://i.imgur.com/HVGDIyx"
        Private Const ShareLink As String = "https://imgur.com/HVGDIyx"

        Private Shared Function CreateImage() As ImgurImage
            Return New ImgurImage(New Uri($"{Base}.jpg"))
        End Function

        Private Shared Function Fmt(size As ImgurUrlSize, urlFormat As ImgurUrlFormat) As String
            Return ImgurUrlFormatter.Format(CreateImage(), size, urlFormat)
        End Function

#Region " GetUrl "

        <TestMethod>
        Public Sub GetUrl_ReturnsTheUrlsListedByImgur()

            Dim image As ImgurImage = CreateImage()

            Assert.AreEqual($"{Base}.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.Normal))
            Assert.AreEqual($"{Base}s.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.SmallSquare))
            Assert.AreEqual($"{Base}b.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.BigSquare))
            Assert.AreEqual($"{Base}t.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.SmallThumbnail))
            Assert.AreEqual($"{Base}m.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.MediumThumbnail))
            Assert.AreEqual($"{Base}l.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.LargeThumbnail))
            Assert.AreEqual($"{Base}h.jpg", ImgurUrlFormatter.GetUrl(image, ImgurUrlSize.HugeThumbnail))

        End Sub

        <TestMethod>
        Public Sub GetUrl_NullImage_Throws()

            Assert.ThrowsException(Of ArgumentNullException)(Function() ImgurUrlFormatter.GetUrl(Nothing, ImgurUrlSize.Normal))

        End Sub

        <TestMethod>
        Public Sub GetUrl_InvalidSize_Throws()

            Assert.ThrowsException(Of InvalidEnumArgumentException)(Function() ImgurUrlFormatter.GetUrl(CreateImage(), CType(99, ImgurUrlSize)))

        End Sub

#End Region

#Region " Format: original size "

        <TestMethod>
        Public Sub Format_Original_DirectUrl()
            Assert.AreEqual($"{Base}.jpg", Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.DirectUrl))
        End Sub

        <TestMethod>
        Public Sub Format_Original_ShareLink()
            Assert.AreEqual(ShareLink, Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.ShareLink))
        End Sub

        <TestMethod>
        Public Sub Format_Original_Css()
            Assert.AreEqual($"background-image: url('{Base}.jpg');", Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.Css))
        End Sub

        <TestMethod>
        Public Sub Format_Original_Html()
            Assert.AreEqual($"<img src=""{Base}.jpg"" alt=""Imgur"" />", Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.Html))
        End Sub

        <TestMethod>
        Public Sub Format_Original_HtmlEmbed_MatchesTheOfficialImgurCode()

            Dim expected As String = "<blockquote class=""imgur-embed-pub"" lang=""en"" data-id=""HVGDIyx""><a href=""https://imgur.com/HVGDIyx"">View post on imgur.com</a></blockquote><script async src=""//s.imgur.com/min/embed.js"" charset=""utf-8""></script>"

            Assert.AreEqual(expected, Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.HtmlEmbed))

        End Sub

        <TestMethod>
        Public Sub Format_Original_BBCode()
            Assert.AreEqual($"[img]{Base}.jpg[/img]", Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.BBCode))
        End Sub

        <TestMethod>
        Public Sub Format_Original_Markdown_MatchesTheOfficialImgurCode()
            Assert.AreEqual($"[Imgur]({ShareLink})", Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.Markdown))
        End Sub

        <TestMethod>
        Public Sub Format_Original_MarkdownImage()
            Assert.AreEqual($"![Imgur]({Base}.jpg)", Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.MarkdownImage))
        End Sub

#End Region

#Region " Format: thumbnails "

        <TestMethod>
        Public Sub Format_Thumbnail_DirectUrlAndCss_UseTheThumbnailUrl()

            Assert.AreEqual($"{Base}m.jpg", Fmt(ImgurUrlSize.MediumThumbnail, ImgurUrlFormat.DirectUrl))
            Assert.AreEqual($"background-image: url('{Base}t.jpg');", Fmt(ImgurUrlSize.SmallThumbnail, ImgurUrlFormat.Css))

        End Sub

        <TestMethod>
        Public Sub Format_Thumbnail_Html_LinksToTheOriginalImage()

            Assert.AreEqual($"<a href=""{Base}.jpg""><img src=""{Base}l.jpg"" alt=""Imgur"" /></a>",
                            Fmt(ImgurUrlSize.LargeThumbnail, ImgurUrlFormat.Html))

        End Sub

        <TestMethod>
        Public Sub Format_Thumbnail_BBCode_LinksToTheOriginalImage()

            Assert.AreEqual($"[url={Base}.jpg][img]{Base}h.jpg[/img][/url]",
                            Fmt(ImgurUrlSize.HugeThumbnail, ImgurUrlFormat.BBCode))

        End Sub

        <TestMethod>
        Public Sub Format_Thumbnail_MarkdownImage_LinksToTheOriginalImage()

            Assert.AreEqual($"[![Imgur]({Base}s.jpg)]({Base}.jpg)",
                            Fmt(ImgurUrlSize.SmallSquare, ImgurUrlFormat.MarkdownImage))

        End Sub

        <TestMethod>
        Public Sub Format_SizeIndependentFormats_IgnoreTheSize()

            For Each size As ImgurUrlSize In [Enum].GetValues(GetType(ImgurUrlSize))
                Assert.AreEqual(Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.ShareLink), Fmt(size, ImgurUrlFormat.ShareLink))
                Assert.AreEqual(Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.HtmlEmbed), Fmt(size, ImgurUrlFormat.HtmlEmbed))
                Assert.AreEqual(Fmt(ImgurUrlSize.Normal, ImgurUrlFormat.Markdown), Fmt(size, ImgurUrlFormat.Markdown))
            Next

        End Sub

        <TestMethod>
        Public Sub Format_EveryCombination_IsNotEmptyAndUsesHttps()

            For Each size As ImgurUrlSize In [Enum].GetValues(GetType(ImgurUrlSize))
                For Each urlFormat As ImgurUrlFormat In [Enum].GetValues(GetType(ImgurUrlFormat))
                    Dim result As String = Fmt(size, urlFormat)
                    Assert.IsFalse(String.IsNullOrWhiteSpace(result), $"{size} / {urlFormat}")
                    Assert.IsTrue(result.Contains("HVGDIyx"), $"{size} / {urlFormat}")
                    Assert.IsFalse(result.Contains("http://"), $"{size} / {urlFormat}")
                Next
            Next

        End Sub

        <TestMethod>
        Public Sub Format_HttpImage_ProducesHttpsLinks()

            Dim image As New ImgurImage(New Uri("http://i.imgur.com/HVGDIyx.jpg"))

            Assert.AreEqual($"{Base}.jpg", ImgurUrlFormatter.Format(image, ImgurUrlSize.Normal, ImgurUrlFormat.DirectUrl))
            Assert.AreEqual(ShareLink, ImgurUrlFormatter.Format(image, ImgurUrlSize.Normal, ImgurUrlFormat.ShareLink))

        End Sub

#End Region

#Region " Format: errors "

        <TestMethod>
        Public Sub Format_NullImage_Throws()

            Assert.ThrowsException(Of ArgumentNullException)(Function() ImgurUrlFormatter.Format(Nothing, ImgurUrlSize.Normal, ImgurUrlFormat.DirectUrl))

        End Sub

        <TestMethod>
        Public Sub Format_InvalidFormat_Throws()

            Assert.ThrowsException(Of InvalidEnumArgumentException)(Function() Fmt(ImgurUrlSize.Normal, CType(99, ImgurUrlFormat)))

        End Sub

        <TestMethod>
        Public Sub Format_InvalidSize_Throws()

            Assert.ThrowsException(Of InvalidEnumArgumentException)(Function() Fmt(CType(99, ImgurUrlSize), ImgurUrlFormat.DirectUrl))

        End Sub

#End Region

#Region " Helpers "

        <TestMethod>
        Public Sub IsOpenable_OnlyForUrlFormats()

            Assert.IsTrue(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.DirectUrl))
            Assert.IsTrue(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.ShareLink))

            Assert.IsFalse(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.Css))
            Assert.IsFalse(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.Html))
            Assert.IsFalse(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.HtmlEmbed))
            Assert.IsFalse(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.BBCode))
            Assert.IsFalse(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.Markdown))
            Assert.IsFalse(ImgurUrlFormatter.IsOpenable(ImgurUrlFormat.MarkdownImage))

        End Sub

        <TestMethod>
        Public Sub UsesSize_IsFalseOnlyForPostLevelFormats()

            Assert.IsFalse(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.ShareLink))
            Assert.IsFalse(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.HtmlEmbed))
            Assert.IsFalse(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.Markdown))

            Assert.IsTrue(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.DirectUrl))
            Assert.IsTrue(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.Css))
            Assert.IsTrue(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.Html))
            Assert.IsTrue(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.BBCode))
            Assert.IsTrue(ImgurUrlFormatter.UsesSize(ImgurUrlFormat.MarkdownImage))

        End Sub

        <TestMethod>
        Public Sub EnumOrder_MatchesTheSelectorItemOrder()

            ' Main.vb converts the selector indexes with CType, so the order of these enums is part of the contract.
            Assert.AreEqual(0, CInt(ImgurUrlSize.Normal))
            Assert.AreEqual(1, CInt(ImgurUrlSize.SmallSquare))
            Assert.AreEqual(2, CInt(ImgurUrlSize.BigSquare))
            Assert.AreEqual(3, CInt(ImgurUrlSize.SmallThumbnail))
            Assert.AreEqual(4, CInt(ImgurUrlSize.MediumThumbnail))
            Assert.AreEqual(5, CInt(ImgurUrlSize.LargeThumbnail))
            Assert.AreEqual(6, CInt(ImgurUrlSize.HugeThumbnail))

            Assert.AreEqual(0, CInt(ImgurUrlFormat.DirectUrl))
            Assert.AreEqual(1, CInt(ImgurUrlFormat.ShareLink))
            Assert.AreEqual(2, CInt(ImgurUrlFormat.Css))
            Assert.AreEqual(3, CInt(ImgurUrlFormat.Html))
            Assert.AreEqual(4, CInt(ImgurUrlFormat.HtmlEmbed))
            Assert.AreEqual(5, CInt(ImgurUrlFormat.BBCode))
            Assert.AreEqual(6, CInt(ImgurUrlFormat.Markdown))
            Assert.AreEqual(7, CInt(ImgurUrlFormat.MarkdownImage))

        End Sub

#End Region

    End Class

End Namespace

#End Region
