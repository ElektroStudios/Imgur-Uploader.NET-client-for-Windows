
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports ImgurUploader.Tools
Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " ImgurAccountTests "

Namespace Tests

    ''' <summary>
    ''' Offline tests for the account (OAuth) support of <see cref="ImgurAPI"/> and for <see cref="ImgurTokens"/>
    ''' (no network access is performed).
    ''' </summary>
    <TestClass>
    Public Class ImgurAccountTests

#Region " ImgurTokens.Parse "

        <TestMethod>
        Public Sub Tokens_Parse_FullResponse_ReadsEveryField()

            Dim json As String =
                "{""access_token"":""acc123"",""expires_in"":3600,""token_type"":""bearer""," &
                """refresh_token"":""ref456"",""account_username"":""elektro"",""account_id"":42}"

            Dim tokens As ImgurTokens = ImgurTokens.Parse(json)

            Assert.AreEqual("acc123", tokens.AccessToken)
            Assert.AreEqual("ref456", tokens.RefreshToken)
            Assert.AreEqual(3600, tokens.ExpiresInSeconds)
            Assert.AreEqual("elektro", tokens.Username)

        End Sub

        <TestMethod>
        Public Sub Tokens_Parse_OnlyAccessToken_UsesDefaults()

            Dim tokens As ImgurTokens = ImgurTokens.Parse("{""access_token"":""acc123""}")

            Assert.AreEqual("acc123", tokens.AccessToken)
            Assert.AreEqual(String.Empty, tokens.RefreshToken)
            Assert.AreEqual(String.Empty, tokens.Username)
            Assert.AreEqual(3600, tokens.ExpiresInSeconds)

        End Sub

        <TestMethod>
        Public Sub Tokens_Parse_InvalidExpiresIn_UsesTheDefaultLifetime()

            Assert.AreEqual(3600, ImgurTokens.Parse("{""access_token"":""a"",""expires_in"":0}").ExpiresInSeconds)
            Assert.AreEqual(3600, ImgurTokens.Parse("{""access_token"":""a"",""expires_in"":-5}").ExpiresInSeconds)
            Assert.AreEqual(3600, ImgurTokens.Parse("{""access_token"":""a"",""expires_in"":""soon""}").ExpiresInSeconds)
            Assert.AreEqual(60, ImgurTokens.Parse("{""access_token"":""a"",""expires_in"":60}").ExpiresInSeconds)

        End Sub

        <TestMethod>
        Public Sub Tokens_Parse_IgnoresTheCaseOfTheKeys()

            Dim tokens As ImgurTokens = ImgurTokens.Parse("{""Access_Token"":""acc123"",""REFRESH_TOKEN"":""ref456""}")

            Assert.AreEqual("acc123", tokens.AccessToken)
            Assert.AreEqual("ref456", tokens.RefreshToken)

        End Sub

        <TestMethod>
        Public Sub Tokens_Parse_TrimsTheValues()

            Dim tokens As ImgurTokens = ImgurTokens.Parse("{""access_token"":""  acc123 "",""refresh_token"":"" ref456  ""}")

            Assert.AreEqual("acc123", tokens.AccessToken)
            Assert.AreEqual("ref456", tokens.RefreshToken)

        End Sub

        <TestMethod>
        Public Sub Tokens_Parse_WithoutAccessToken_Throws()

            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("{""refresh_token"":""ref456""}"))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("{""access_token"":""   ""}"))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("{""access_token"":123}"))

        End Sub

        <TestMethod>
        Public Sub Tokens_Parse_EmptyOrInvalidJson_Throws()

            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse(Nothing))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse(String.Empty))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("   "))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("not json at all"))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("{""access_token"":"))
            Assert.Throws(Of FormatException)(Function() ImgurTokens.Parse("[1,2,3]"))

        End Sub

        <TestMethod>
        Public Sub Tokens_Constructor_EmptyAccessToken_Throws()

            Assert.Throws(Of ArgumentException)(Function() New ImgurTokens(String.Empty, "r", 10, "u"))
            Assert.Throws(Of ArgumentException)(Function() New ImgurTokens(Nothing, "r", 10, "u"))

        End Sub

        <TestMethod>
        Public Sub Tokens_Constructor_NullOptionalValues_BecomeEmpty()

            Dim tokens As New ImgurTokens("acc", Nothing, 10, Nothing)

            Assert.AreEqual(String.Empty, tokens.RefreshToken)
            Assert.AreEqual(String.Empty, tokens.Username)

        End Sub

#End Region

#Region " ImgurTokens.GetErrorMessage "

        <TestMethod>
        Public Sub Tokens_GetErrorMessage_ImgurDataError_ReturnsTheText()

            Dim json As String = "{""data"":{""error"":""Invalid pin."",""request"":""/oauth2/token"",""method"":""POST""},""success"":false,""status"":400}"

            Assert.AreEqual("Invalid pin.", ImgurTokens.GetErrorMessage(json))

        End Sub

        <TestMethod>
        Public Sub Tokens_GetErrorMessage_ErrorObject_ReturnsItsMessage()

            Dim json As String = "{""data"":{""error"":{""code"":1003,""message"":""File type invalid"",""type"":""ImgurException""}},""success"":false,""status"":400}"

            Assert.AreEqual("File type invalid", ImgurTokens.GetErrorMessage(json))

        End Sub

        <TestMethod>
        Public Sub Tokens_GetErrorMessage_OAuthStyle_PrefersTheDescription()

            Assert.AreEqual("The grant is invalid.",
                            ImgurTokens.GetErrorMessage("{""error"":""invalid_grant"",""error_description"":""The grant is invalid.""}"))
            Assert.AreEqual("invalid_grant", ImgurTokens.GetErrorMessage("{""error"":""invalid_grant""}"))

        End Sub

        <TestMethod>
        Public Sub Tokens_GetErrorMessage_UnusableInput_ReturnsEmpty()

            Assert.AreEqual(String.Empty, ImgurTokens.GetErrorMessage(Nothing))
            Assert.AreEqual(String.Empty, ImgurTokens.GetErrorMessage(String.Empty))
            Assert.AreEqual(String.Empty, ImgurTokens.GetErrorMessage("<html>Forbidden</html>"))
            Assert.AreEqual(String.Empty, ImgurTokens.GetErrorMessage("{""data"":{}}"))
            Assert.AreEqual(String.Empty, ImgurTokens.GetErrorMessage("[]"))

        End Sub

#End Region

#Region " Authorization url and header "

        <TestMethod>
        Public Sub Api_GetAuthorizationUrl_UsesThePinFlow()

            Assert.AreEqual("https://api.imgur.com/oauth2/authorize?client_id=dc3e850cd310751&response_type=pin",
                            ImgurAPI.GetAuthorizationUrl("dc3e850cd310751"))

        End Sub

        <TestMethod>
        Public Sub Api_GetAuthorizationUrl_EscapesAndTrimsTheClientId()

            Assert.AreEqual("https://api.imgur.com/oauth2/authorize?client_id=a%26b%3Dc&response_type=pin",
                            ImgurAPI.GetAuthorizationUrl("  a&b=c  "))

        End Sub

        <TestMethod>
        Public Sub Api_GetAuthorizationUrl_EmptyClientId_Throws()

            Assert.Throws(Of ArgumentException)(Function() ImgurAPI.GetAuthorizationUrl(Nothing))
            Assert.Throws(Of ArgumentException)(Function() ImgurAPI.GetAuthorizationUrl("   "))

        End Sub

        <TestMethod>
        Public Sub Api_GetAuthorizationHeaderValue_Anonymous_UsesTheClientId()

            Assert.AreEqual("Client-ID abc123",
                            ImgurAPI.GetAuthorizationHeaderValue(ImgurUploadMode.Anonymous, "abc123", "ignored-token"))

            ' The access token is not needed for anonymous uploads.
            Assert.AreEqual("Client-ID abc123",
                            ImgurAPI.GetAuthorizationHeaderValue(ImgurUploadMode.Anonymous, "abc123", Nothing))

        End Sub

        <TestMethod>
        Public Sub Api_GetAuthorizationHeaderValue_Account_UsesTheBearerToken()

            Assert.AreEqual("Bearer tok456",
                            ImgurAPI.GetAuthorizationHeaderValue(ImgurUploadMode.Account, "abc123", "tok456"))

            ' The client id is not needed for account uploads.
            Assert.AreEqual("Bearer tok456",
                            ImgurAPI.GetAuthorizationHeaderValue(ImgurUploadMode.Account, Nothing, "tok456"))

        End Sub

        <TestMethod>
        Public Sub Api_GetAuthorizationHeaderValue_MissingCredential_Throws()

            Assert.Throws(Of ArgumentException)(
                Function() ImgurAPI.GetAuthorizationHeaderValue(ImgurUploadMode.Anonymous, String.Empty, "tok"))

            Assert.Throws(Of ArgumentException)(
                Function() ImgurAPI.GetAuthorizationHeaderValue(ImgurUploadMode.Account, "abc", String.Empty))

        End Sub

        <TestMethod>
        Public Sub Api_GetAuthorizationHeaderValue_InvalidMode_Throws()

            Assert.Throws(Of ArgumentOutOfRangeException)(
                Function() ImgurAPI.GetAuthorizationHeaderValue(CType(99, ImgurUploadMode), "abc", "tok"))

        End Sub

        <TestMethod>
        Public Sub UploadMode_Values_AreStable()

            ' The values are persisted by name, but they are also part of the public API.
            Assert.AreEqual(0, CInt(ImgurUploadMode.Anonymous))
            Assert.AreEqual(1, CInt(ImgurUploadMode.Account))

        End Sub

#End Region

#Region " Account state "

        <TestMethod>
        Public Sub Api_NewInstance_IsNotAuthorized()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            Assert.IsFalse(client.IsAuthorized)
            Assert.AreEqual(String.Empty, client.RefreshToken)
            Assert.AreEqual(String.Empty, client.AccountUsername)

        End Sub

        <TestMethod>
        Public Sub Api_RestoreAccount_SetsTheAccountWithoutRaisingTheEvent()

            Dim client As New ImgurAPI("clientId", "clientSecret")
            Dim raised As Integer = 0
            AddHandler client.OnAccountTokensChanged, Sub(sender As Object, e As EventArgs) raised += 1

            client.RestoreAccount("  ref456 ", " elektro ")

            Assert.IsTrue(client.IsAuthorized)
            Assert.AreEqual("ref456", client.RefreshToken)
            Assert.AreEqual("elektro", client.AccountUsername)
            Assert.AreEqual(0, raised)

        End Sub

        <TestMethod>
        Public Sub Api_RestoreAccount_NullValues_LeaveTheClientUnauthorized()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            client.RestoreAccount(Nothing, Nothing)

            Assert.IsFalse(client.IsAuthorized)
            Assert.AreEqual(String.Empty, client.AccountUsername)

        End Sub

        <TestMethod>
        Public Sub Api_SignOut_ForgetsTheAccount()

            Dim client As New ImgurAPI("clientId", "clientSecret")
            client.RestoreAccount("ref456", "elektro")

            client.SignOut()

            Assert.IsFalse(client.IsAuthorized)
            Assert.AreEqual(String.Empty, client.RefreshToken)
            Assert.AreEqual(String.Empty, client.AccountUsername)

        End Sub

        <TestMethod>
        Public Sub Api_WhitespaceRefreshToken_IsNotAuthorized()

            Dim client As New ImgurAPI("clientId", "clientSecret") With {.RefreshToken = "   "}

            Assert.IsFalse(client.IsAuthorized)

        End Sub

#End Region

#Region " Guard clauses (no network access) "

        <TestMethod>
        Public Sub Api_UploadImage_AccountModeWithoutAuthorization_Throws()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            Dim ex As InvalidOperationException =
                Assert.Throws(Of InvalidOperationException)(Function() client.UploadImage("any.png", ImgurUploadMode.Account))

            StringAssert.Contains(ex.Message, "not authorized")

        End Sub

        <TestMethod>
        Public Sub Api_UploadImageAsync_AccountModeWithoutAuthorization_Throws()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            Dim ex As InvalidOperationException =
                Assert.Throws(Of InvalidOperationException)(Function() client.UploadImageAsync("any.png", ImgurUploadMode.Account))

            StringAssert.Contains(ex.Message, "not authorized")

        End Sub

        <TestMethod>
        Public Sub Api_UploadImage_AccountModeWithoutClientSecret_StillReportsTheMissingCredential()

            Dim client As New ImgurAPI("clientId", String.Empty)
            client.RestoreAccount("ref456", "elektro")

            Dim ex As Exception = Assert.Throws(Of Exception)(Function() client.UploadImage("any.png", ImgurUploadMode.Account))

            StringAssert.Contains(ex.Message, "ClientSecret")

        End Sub

        <TestMethod>
        Public Sub Api_AuthorizeWithPin_EmptyPin_Throws()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            Assert.Throws(Of ArgumentException)(Sub() client.AuthorizeWithPin(Nothing))
            Assert.Throws(Of ArgumentException)(Sub() client.AuthorizeWithPin("   "))

        End Sub

        <TestMethod>
        Public Sub Api_AuthorizeWithPin_WithoutCredentials_Throws()

            Dim withoutId As New ImgurAPI(String.Empty, "clientSecret")
            Dim withoutSecret As New ImgurAPI("clientId", String.Empty)

            Dim ex1 As InvalidOperationException = Assert.Throws(Of InvalidOperationException)(Sub() withoutId.AuthorizeWithPin("1234"))
            Dim ex2 As InvalidOperationException = Assert.Throws(Of InvalidOperationException)(Sub() withoutSecret.AuthorizeWithPin("1234"))

            StringAssert.Contains(ex1.Message, "ClientId")
            StringAssert.Contains(ex2.Message, "ClientSecret")

        End Sub

        <TestMethod>
        Public Sub Api_RefreshAccessToken_WithoutAuthorization_Throws()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            Dim ex As InvalidOperationException = Assert.Throws(Of InvalidOperationException)(Sub() client.RefreshAccessToken())

            StringAssert.Contains(ex.Message, "not authorized")

        End Sub

        <TestMethod>
        Public Sub Api_UploadImageAsyncCancel_BeforeAnyUpload_DoesNotThrow()

            Dim client As New ImgurAPI("clientId", "clientSecret")

            client.UploadImageAsyncCancel()

        End Sub

        <TestMethod>
        Public Sub AuthorizationException_KeepsTheMessageAndTheInnerException()

            Dim inner As New InvalidOperationException("inner")
            Dim ex As New ImgurAuthorizationException("outer", inner)

            Assert.AreEqual("outer", ex.Message)
            Assert.AreSame(inner, ex.InnerException)
            Assert.AreEqual("only message", New ImgurAuthorizationException("only message").Message)

        End Sub

#End Region

    End Class

End Namespace

#End Region
