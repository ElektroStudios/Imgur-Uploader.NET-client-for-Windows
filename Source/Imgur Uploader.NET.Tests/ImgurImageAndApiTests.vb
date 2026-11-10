
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " ImgurImageAndApiTests "

Namespace Tests

    ''' <summary>
    ''' Offline tests for <see cref="ImgurImage"/> and <see cref="ImgurAPI"/> (no network access is performed).
    ''' </summary>
    <TestClass>
    Public Class ImgurImageAndApiTests

        <TestMethod>
        Public Sub ImgurImage_HttpUrl_IsConvertedToHttps()

            Dim image As New ImgurImage(New Uri("http://i.imgur.com/AbCd123.png"))

            Assert.AreEqual("https://i.imgur.com/AbCd123.png", image.Normal)

        End Sub

        <TestMethod>
        Public Sub ImgurImage_HttpUrlWithExplicitPort_DropsThePort()

            Dim image As New ImgurImage(New Uri("http://i.imgur.com:80/AbCd123.png"))

            Assert.AreEqual("https://i.imgur.com/AbCd123.png", image.Normal)

        End Sub

        <TestMethod>
        Public Sub ImgurImage_HttpsUrl_IsKept()

            Dim image As New ImgurImage(New Uri("https://i.imgur.com/AbCd123.jpg"))

            Assert.AreEqual("https://i.imgur.com/AbCd123.jpg", image.Normal)

        End Sub

        <TestMethod>
        Public Sub ImgurImage_NullUri_Throws()

            Assert.Throws(Of ArgumentNullException)(Function() New ImgurImage(Nothing))

        End Sub

        <TestMethod>
        Public Sub ImgurImage_Thumbnails_UseTheImgurSuffixesAndKeepTheExtension()

            Dim image As New ImgurImage(New Uri("http://i.imgur.com/AbCd123.jpg"))

            Assert.AreEqual("https://i.imgur.com/AbCd123s.jpg", image.SmallSquare)
            Assert.AreEqual("https://i.imgur.com/AbCd123b.jpg", image.BigSquare)
            Assert.AreEqual("https://i.imgur.com/AbCd123t.jpg", image.SmallThumbnail)
            Assert.AreEqual("https://i.imgur.com/AbCd123m.jpg", image.MediumThumbnail)
            Assert.AreEqual("https://i.imgur.com/AbCd123l.jpg", image.LargeThumbnail)
            Assert.AreEqual("https://i.imgur.com/AbCd123h.jpg", image.HugeThumbnail)

        End Sub

        <TestMethod>
        Public Sub ImgurImage_IdAndShareLink_AreDerivedFromTheDirectUrl()

            Dim image As New ImgurImage(New Uri("http://i.imgur.com/BRAw18S.jpeg"))

            Assert.AreEqual("BRAw18S", image.Id)
            Assert.AreEqual("https://imgur.com/BRAw18S", image.ShareLink)

        End Sub

        <TestMethod>
        Public Sub ImgurApi_GetThumbnail_AlwaysReturnsHttps()

            Dim result As String = ImgurAPI.GetThumbnail(New Uri("http://i.imgur.com/XyZ789.gif"), ImgurAPI.ThumbnailType.MediumThumbnail)

            Assert.AreEqual("https://i.imgur.com/XyZ789m.gif", result)

        End Sub

        <TestMethod>
        Public Sub ImgurApi_StatusValues_MatchTheHttpStatusCodes()

            Assert.AreEqual(200, CInt(ImgurAPI.ImgurStatus.Success))
            Assert.AreEqual(400, CInt(ImgurAPI.ImgurStatus.BadImageFormat))
            Assert.AreEqual(401, CInt(ImgurAPI.ImgurStatus.AuthorizationFailed))
            Assert.AreEqual(403, CInt(ImgurAPI.ImgurStatus.AccessForbidden))
            Assert.AreEqual(404, CInt(ImgurAPI.ImgurStatus.PageIsNotFound))
            Assert.AreEqual(429, CInt(ImgurAPI.ImgurStatus.UploadRateLimitError))
            Assert.AreEqual(500, CInt(ImgurAPI.ImgurStatus.InternalServerError))

        End Sub

        <TestMethod>
        Public Sub ImgurApi_UploadImage_WithoutClientId_Throws()

            Dim client As New ImgurAPI()

            Dim ex As Exception = Assert.Throws(Of Exception)(Function() client.UploadImage("any.png"))
            StringAssert.Contains(ex.Message, "ClientId")

        End Sub

        <TestMethod>
        Public Sub ImgurApi_UploadImage_WithoutClientSecret_Throws()

            Dim client As New ImgurAPI("clientId", String.Empty)

            Dim ex As Exception = Assert.Throws(Of Exception)(Function() client.UploadImage("any.png"))
            StringAssert.Contains(ex.Message, "ClientSecret")

        End Sub

        <TestMethod>
        Public Sub ImgurApi_UploadImageAsync_WithoutClientId_Throws()

            Dim client As New ImgurAPI()

            Dim ex As Exception = Assert.Throws(Of Exception)(Function() client.UploadImageAsync("any.png"))
            StringAssert.Contains(ex.Message, "ClientId")

        End Sub

        <TestMethod>
        Public Sub ImgurApi_UploadImage_MissingFile_RaisesUnknownErrorAndReturnsNothing()

            Dim client As New ImgurAPI("clientId", "clientSecret")
            Dim raised As Integer = 0
            AddHandler client.OnUnknownError, Sub(sender As Object, e As ImgurAPI.ImgurStatus) raised += 1

            Dim result As ImgurImage = client.UploadImage(IO.Path.Combine(IO.Path.GetTempPath(), $"{Guid.NewGuid()}.png"))

            Assert.IsNull(result)
            Assert.AreEqual(1, raised)

        End Sub

        <TestMethod>
        Public Sub ImgurApi_UploadImageAsync_MissingFile_RaisesUnknownErrorAndReturnsNothing()

            Dim client As New ImgurAPI("clientId", "clientSecret")
            Dim raised As Integer = 0
            AddHandler client.OnUnknownError, Sub(sender As Object, e As ImgurAPI.ImgurStatus) raised += 1

            Dim result As ImgurImage = client.UploadImageAsync(IO.Path.Combine(IO.Path.GetTempPath(), $"{Guid.NewGuid()}.png"))

            Assert.IsNull(result)
            Assert.AreEqual(1, raised)

        End Sub

    End Class

End Namespace

#End Region
