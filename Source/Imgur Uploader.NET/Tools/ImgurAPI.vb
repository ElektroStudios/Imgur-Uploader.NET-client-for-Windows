
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Collections.Specialized ' Reference: System.Web.Extensions
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading

#End Region

Namespace Tools

    ''' <summary>
    ''' Indicates how an image is uploaded to imgur.
    ''' </summary>
    <Description("Indicates how an image is uploaded to imgur.")>
    Public Enum ImgurUploadMode As Integer

        ''' <summary>
        ''' The image is uploaded anonymously: it is not linked to any imgur account.
        ''' Only the application credentials (Client ID) are used.
        ''' </summary>
        Anonymous = 0

        ''' <summary>
        ''' The image is uploaded to the account of the user who authorized the application (OAuth 2).
        ''' </summary>
        Account = 1

    End Enum

    ''' <summary>
    ''' Uploads images to imgur using the imgur API, anonymously or to the account of the user.
    ''' </summary>
    Public NotInheritable Class ImgurAPI

#Region " Properties "

        ''' <summary>
        ''' Gets or set the unique client identifier that is provided with the imgur API application registration.
        ''' Example ID => "dc3e850cd310751"
        ''' </summary>
        Friend Property ClientId As String = String.Empty

        ''' <summary>
        ''' Gets or set the unique client secret that is provided with the imgur API registrationapplication registration.
        ''' Example ID => "c2b80785e393928644b54d6d970338744e764103"
        ''' </summary>
        Friend Property ClientSecret As String = String.Empty

        ''' <summary>
        ''' Gets or sets the OAuth refresh token of the user account that authorized the application.
        ''' It does not expire, and it is used to get the (short-lived) access tokens that authorize the uploads
        ''' made with <see cref="ImgurUploadMode.Account"/>.
        ''' It is empty until the user authorizes the application (see <see cref="ImgurAPI.AuthorizeWithPin"/>).
        ''' </summary>
        Public Property RefreshToken As String = String.Empty

        ''' <summary>
        ''' Gets the Imgur user name of the account that authorized the application,
        ''' or an empty string if it is unknown.
        ''' </summary>
        Public ReadOnly Property AccountUsername As String
            Get
                Return Me.accountUsernameValue
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether a user account has authorized the application,
        ''' that is, whether images can be uploaded with <see cref="ImgurUploadMode.Account"/>.
        ''' </summary>
        Public ReadOnly Property IsAuthorized As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(Me.RefreshToken)
            End Get
        End Property

#End Region

#Region " Enums "

        ''' <summary>
        ''' Indicates one of the possible imgur status result when uploading an image.
        ''' </summary>
        <Description("Indicates one of the possible imgur status result when uploading an image.")>
        Public Enum ImgurStatus As Integer

            ''' <summary>
            ''' Indicates an unknown status.
            ''' </summary>
            UnknownError = -0

            ''' <summary>
            ''' Indicates a success status.
            ''' </summary>
            Success = 200

            ''' <summary>
            ''' Indicates a bad image file-format.
            ''' </summary>
            BadImageFormat = 400

            ''' <summary>
            ''' Indicates that the authorization to upload the image has failed.
            ''' Note: It's unsure that this status code could raise in an anonym upload.
            ''' </summary>
            AuthorizationFailed = 401

            ''' <summary>
            ''' Indicates that the access is forbidden.
            ''' </summary>
            AccessForbidden = 403

            ''' <summary>
            ''' Indicates that the page is not found.
            ''' </summary>
            PageIsNotFound = 404

            ''' <summary>
            ''' Indicates that the user (Anonym IP) raised the image upload limit.
            ''' Note: It's unsure that this status code could raise in an anonym upload.
            ''' </summary>
            UploadRateLimitError = 429

            ''' <summary>
            ''' Indicates an internal imgur service error.
            ''' </summary>
            InternalServerError = 500

        End Enum

        ''' <summary>
        ''' Indicates one of the possible imgur thumbnail types.
        ''' For more info see here:
        ''' https://api.imgur.com/models/image
        ''' </summary>
        <Description("Indicates one of the possible imgur thumbnail types.")>
        Public Enum ThumbnailType As Integer

            ''' <summary>
            ''' The image resized to 90x90 (without preserving image proportions).
            ''' </summary>
            SmallSquare = 0

            ''' <summary>
            ''' The image resized to 160x160 (without preserving image proportions).
            ''' </summary>
            BigSquare = 1

            ''' <summary>
            ''' The image resized to 160x160 (preserving image proportions).
            ''' </summary>
            SmallThumbnail = 2

            ''' <summary>
            ''' The image resized to 320x320 (without preserving image proportions).
            ''' </summary>
            MediumThumbnail = 3

            ''' <summary>
            ''' The image resized to 640x640 (without preserving image proportions).
            ''' </summary>
            LargeThumbnail = 4

            ''' <summary>
            ''' The image resized to 1024x1024 (without preserving image proportions).
            ''' </summary>
            HugeThumbnail = 5

        End Enum

#End Region

#Region " Other objects "

        ''' <summary>
        ''' Specifies a thumbnail key to retrieve the thumbnail url.
        ''' </summary>
        Private Shared ReadOnly thumbnailKeys As New Dictionary(Of String, Char) From
        {
            {"SmallSquare", "s"c},
            {"BigSquare", "b"c},
            {"SmallThumbnail", "t"c},
            {"MediumThumbnail", "m"c},
            {"LargeThumbnail", "l"c},
            {"HugeThumbnail", "h"c}
        }

        ''' <summary>
        ''' 
        ''' </summary>
        Dim cts As CancellationTokenSource

        ''' <summary>
        ''' 
        ''' </summary>
        Dim token As CancellationToken

        ''' <summary>
        ''' 
        ''' </summary>
        Dim responseAsync As Byte()

        ''' <summary>
        '''
        ''' </summary>
        Dim asyncError As Boolean = False

        ''' <summary>
        ''' The current access token (short-lived), or an empty string if there is none.
        ''' </summary>
        Private accessToken As String = String.Empty

        ''' <summary>
        ''' The moment (UTC) from which the current access token must be renewed.
        ''' </summary>
        Private accessTokenExpiresUtc As Date = Date.MinValue

        ''' <summary>
        ''' Backing field of <see cref="ImgurAPI.AccountUsername"/>.
        ''' </summary>
        Private accountUsernameValue As String = String.Empty

        ''' <summary>
        ''' The security margin, in seconds, that is subtracted from the lifetime of an access token,
        ''' so a token never expires while an upload is starting.
        ''' </summary>
        Private Const AccessTokenSafetyMarginSeconds As Integer = 120

#End Region

#Region " Events "

        ''' <summary>
        ''' Occurs when imgur service throws an 'access forbidden' response.
        ''' </summary>
        Public Event OnAccessForbidden(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'Authorization Failed' response.
        ''' </summary>
        Public Event OnAuthorizationFailed(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws a 'Bad Image Format' response.
        ''' </summary>
        Public Event OnBadImageFormat(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'Internal Server Error' response.
        ''' </summary>
        Public Event OnInternalServerError(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'Page Is Not Found' response.
        ''' </summary>
        Public Event OnPageIsNotFound(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'access forbidden' response.
        ''' </summary>
        Public Event OnUploadRateLimitError(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an unknown (unhandled) response.
        ''' </summary>
        Public Event OnUnknownError(sender As Object, e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an unknown (unhandled) response on an Asynchronous upload.
        ''' </summary>
        Public Event OnAsyncError(sender As Object, ex As Exception)

        ''' <summary>
        ''' Occurs when imgur service throws sucessful response on an Asynchronous upload.
        ''' </summary>
        Public Event OnAsyncSuccess(sender As Object, e As ImgurImage)

        ''' <summary>
        ''' Occurs when the refresh token or the user name of the account have changed,
        ''' so the new values must be stored. This event can be raised from a background thread.
        ''' </summary>
        Public Event OnAccountTokensChanged(sender As Object, e As EventArgs)

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="imgurAPI"/> class.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="imgurAPI"/> class.
        ''' </summary>
        ''' <param name="ClientId">
        ''' Indicates the unique client identifier that is provided with the imgur API application registration.
        ''' </param>
        ''' <param name="ClientSecret">
        ''' Indicates the unique client secret that is provided with the imgur API registrationapplication registration.
        ''' </param>
        Public Sub New(clientId As String, clientSecret As String)

            Me.ClientId = clientId
            Me.ClientSecret = clientSecret

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the UploadValuesCompleted event of the WC <see cref="Webclient"/>.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="UploadValuesCompletedEventArgs"/> instance containing the event data.</param>
        Private Sub WC_UploadValuesCompleted(sender As Object, e As UploadValuesCompletedEventArgs)

            ' A user abort is not an error: the upload loop detects the cancellation and returns.
            Dim wasCanceled As Boolean =
                e.Cancelled OrElse
                (TypeOf e.Error Is WebException AndAlso
                 DirectCast(e.Error, WebException).Status = WebExceptionStatus.RequestCanceled)

            If wasCanceled Then
                Exit Sub
            End If

            If e.Error IsNot Nothing Then

                If TypeOf (e.Error.InnerException) Is ObjectDisposedException Then
                    ' Do Nothing

                Else 'If TypeOf (e.Error.InnerException) Is WebException Then
                    Me.asyncError = True
                    RaiseEvent OnAsyncError(Me, e.Error)
                    Exit Sub

                End If

            End If

            If Not e.Cancelled Then
                Me.responseAsync = e.Result
            End If

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Uploads an image to imgur, anonymously or to the account of the user.
        ''' </summary>
        ''' <param name="img">
        ''' Indicates a filename that points to an existing image.
        ''' </param>
        ''' <param name="mode">
        ''' Indicates whether the image is uploaded anonymously (default) or to the account of the user.
        ''' </param>
        ''' <returns>An instance of the imgurImage Class that contains the url and it's thumbnail urls.</returns>
        ''' <exception cref="InvalidOperationException">
        ''' <paramref name="mode"/> is <see cref="ImgurUploadMode.Account"/> and the application is not authorized,
        ''' or the access token could not be renewed.
        ''' </exception>
        ''' <exception cref="ImgurAuthorizationException">
        ''' Imgur rejected the refresh token (it is invalid or the user revoked the authorization).
        ''' </exception>
        Public Function UploadImage(img As String,
                                    Optional mode As ImgurUploadMode = ImgurUploadMode.Anonymous) As ImgurImage

            If String.IsNullOrEmpty(Me.ClientId) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientId) Then
                Throw New Exception("'ClientId' property is empty.")
            End If

            If String.IsNullOrEmpty(Me.ClientSecret) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientSecret) Then
                Throw New Exception("'ClientSecret' property is empty.")
            End If

            Dim authorization As String = Me.GetAuthorizationHeader(mode)

            Try

                ' Create a WebClient. 
                Using wc As New WebClient()

                    ' Read the image.
                    Dim values As New NameValueCollection() From
                        {
                            {"image", Convert.ToBase64String(File.ReadAllBytes(img))}
                        }

                    ' Set the Headers.
                    Dim headers As New NameValueCollection() From
                        {
                            {"Authorization", authorization}
                        }

                    ' Add the headers.
                    wc.Headers.Add(headers)

                    ' Upload the image, and get the response.
                    Dim response As Byte()
                    Try
                        response = wc.UploadValues("https://api.imgur.com/3/upload.xml", values)

                    Catch ex As WebException
                        Using br As New BinaryReader(ex.Response.GetResponseStream)
                            response = br.ReadBytes(CInt(ex.Response.GetResponseStream.Length))
                        End Using

                    Catch ex As Exception
                        RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                        Return Nothing

                    End Try

                    ' Read the response (Converting Byte-Array to Stream).
                    Using sr As New StreamReader(New MemoryStream(response))

                        Dim serverResponse As String = sr.ReadToEnd
                        Dim xdoc As New XDocument(XDocument.Parse(serverResponse))
                        Dim status As ImgurStatus = Nothing

                        status = Me.GetResultFromStatus(Convert.ToInt32(xdoc.Root.LastAttribute.Value.ToString))

                        Select Case status

                            Case ImgurStatus.Success ' 200
                                Return New ImgurImage(New Uri(xdoc.Descendants("link").Value))

                            Case ImgurStatus.AccessForbidden ' 403
                                RaiseEvent OnAccessForbidden(Me, ImgurStatus.AccessForbidden)

                            Case ImgurStatus.AuthorizationFailed ' 401
                                RaiseEvent OnAuthorizationFailed(Me, ImgurStatus.AuthorizationFailed)

                            Case ImgurStatus.BadImageFormat ' 400
                                RaiseEvent OnBadImageFormat(Me, ImgurStatus.BadImageFormat)

                            Case ImgurStatus.InternalServerError ' 500
                                RaiseEvent OnInternalServerError(Me, ImgurStatus.InternalServerError)

                            Case ImgurStatus.PageIsNotFound ' 404
                                RaiseEvent OnPageIsNotFound(Me, ImgurStatus.PageIsNotFound)

                            Case ImgurStatus.UploadRateLimitError ' 429
                                RaiseEvent OnUploadRateLimitError(Me, ImgurStatus.UploadRateLimitError)

                            Case ImgurStatus.UnknownError ' -0
                                RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                                Return Nothing

                        End Select

                    End Using '/ sr As New StreamReader

                End Using '/  wc As New WebClient()

            Catch ex As Exception
                RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                Return Nothing

            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Uploads asynchronous an image to imgur, anonymously or to the account of the user.
        ''' </summary>
        ''' <param name="img">
        ''' Indicates a filename that points to an existing image.
        ''' </param>
        ''' <param name="mode">
        ''' Indicates whether the image is uploaded anonymously (default) or to the account of the user.
        ''' </param>
        ''' <returns>
        ''' An instance of the imgurImage Class that contains the url and it's thumbnail urls.
        ''' </returns>
        ''' <exception cref="InvalidOperationException">
        ''' <paramref name="mode"/> is <see cref="ImgurUploadMode.Account"/> and the application is not authorized,
        ''' or the access token could not be renewed.
        ''' </exception>
        ''' <exception cref="ImgurAuthorizationException">
        ''' Imgur rejected the refresh token (it is invalid or the user revoked the authorization).
        ''' </exception>
        Public Function UploadImageAsync(img As String,
                                         Optional mode As ImgurUploadMode = ImgurUploadMode.Anonymous) As ImgurImage
            Me.cts = New CancellationTokenSource
            Me.token = Me.cts.Token

            If String.IsNullOrEmpty(Me.ClientId) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientId) Then
                Throw New Exception("'ClientId' property is empty.")
            End If

            If String.IsNullOrEmpty(Me.ClientSecret) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientSecret) Then
                Throw New Exception("'ClientSecret' property is empty.")
            End If

            ' This call can renew the access token (a blocking request), so the upload may have been canceled meanwhile.
            Dim authorization As String = Me.GetAuthorizationHeader(mode)
            If Me.token.IsCancellationRequested Then
                Return Nothing
            End If

            Try

                ' Create a WebClient. 
                Using wc As New WebClient()

                    Using ctr As CancellationTokenRegistration = Me.token.Register(Sub() wc.CancelAsync())

                        ' Read the image.
                        Dim values As New NameValueCollection() From
                            {
                                {"image", Convert.ToBase64String(File.ReadAllBytes(img))},
                                {"type", "base64"},
                                {"title", Path.GetFileNameWithoutExtension(img)},
                                {"description", Path.GetFileNameWithoutExtension(img)}
                            }

                        ' Set the Headers.
                        Dim headers As New NameValueCollection() From
                            {
                                {"Authorization", authorization}
                            }

                        ' Add the headers.
                        wc.Headers.Add(headers)

                        ' Upload the image, and get the response.
                        Try
                            Me.responseAsync = Nothing
                            AddHandler wc.UploadValuesCompleted, AddressOf Me.WC_UploadValuesCompleted
                            wc.UploadValuesAsync(New Uri("https://api.imgur.com/3/upload.xml"), values)

                            Do Until Me.responseAsync IsNot Nothing OrElse Me.asyncError
#If Not DEBUG Then
                                Thread.Sleep(100)
                                Application.DoEvents()
#End If
                                ' MsgBox(16)
                                If Me.cts.IsCancellationRequested OrElse Me.asyncError Then
                                    Me.asyncError = False
                                    Return Nothing
                                End If
                            Loop

                        Catch ex As Exception
                            RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                            Return Nothing

                        End Try

                        ' Read the response (Converting Byte-Array to Stream).
                        Using sr As New StreamReader(New MemoryStream(Me.responseAsync))

                            Dim serverResponse As String = sr.ReadToEnd
                            Dim xdoc As New XDocument(XDocument.Parse(serverResponse))
                            Dim status As ImgurStatus = Me.GetResultFromStatus(Convert.ToInt32(xdoc.Root.Attribute("status").Value))

                            If status <> ImgurStatus.Success Then
                                Me.RaiseStatusEvent(status)
                                Return Nothing
                            End If

                            Dim uploadedImage As New ImgurImage(New Uri(xdoc.Descendants("link").Value))
                            RaiseEvent OnAsyncSuccess(Me, uploadedImage)
                            Return uploadedImage

                            '    Select Case status

                            '        Case ImgurStatus.Success ' 200
                            '            Return New ImgurImage(New Uri(xdoc.Descendants("link").Value))

                            '        Case ImgurStatus.AccessForbidden ' 403
                            '            RaiseEvent OnAccessForbidden(Me, ImgurStatus.AccessForbidden)

                            '        Case ImgurStatus.AuthorizationFailed ' 401
                            '            RaiseEvent OnAuthorizationFailed(Me, ImgurStatus.AuthorizationFailed)

                            '        Case ImgurStatus.BadImageFormat ' 400
                            '            RaiseEvent OnBadImageFormat(Me, ImgurStatus.BadImageFormat)

                            '        Case ImgurStatus.InternalServerError ' 500
                            '            RaiseEvent OnInternalServerError(Me, ImgurStatus.InternalServerError)

                            '        Case ImgurStatus.PageIsNotFound ' 404
                            '            RaiseEvent OnPageIsNotFound(Me, ImgurStatus.PageIsNotFound)

                            '        Case ImgurStatus.UploadRateLimitError ' 429
                            '            RaiseEvent OnUploadRateLimitError(Me, ImgurStatus.UploadRateLimitError)

                            '        Case ImgurStatus.UnknownError ' -0
                            '            RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                            '            Return Nothing

                            ' End Select

                        End Using '/ sr As New StreamReader

                    End Using ' ctr As CancellationTokenRegistration

                End Using '/  wc As New WebClient()

            Catch ex As Exception
                RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                Return Nothing

            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Cancels the current asynchronous uploading.
        ''' </summary>
        Public Sub UploadImageAsyncCancel()
            Me.cts?.Cancel(True)
        End Sub

        ''' <summary>
        ''' Authorizes the application to upload images to the account of a user, using the PIN that
        ''' Imgur shows to the user after he allows the application (see <see cref="ImgurAPI.GetAuthorizationUrl"/>).
        ''' <para></para>
        ''' This is a blocking network request. If it succeeds, <see cref="ImgurAPI.RefreshToken"/> and
        ''' <see cref="ImgurAPI.AccountUsername"/> are set and <see cref="ImgurAPI.OnAccountTokensChanged"/> is raised.
        ''' </summary>
        ''' <param name="pin">The PIN that Imgur shows to the user. A PIN can be used only once.</param>
        ''' <exception cref="ArgumentException">
        ''' The PIN is empty.
        ''' </exception>
        ''' <exception cref="InvalidOperationException">
        ''' The 'ClientId' or 'ClientSecret' property is empty, Imgur could not be reached or it could not process the request.
        ''' </exception>
        ''' <exception cref="ImgurAuthorizationException">
        ''' Imgur rejected the PIN (it is wrong, it has expired or it was already used).
        ''' </exception>
        Public Sub AuthorizeWithPin(pin As String)

            If String.IsNullOrWhiteSpace(pin) Then
                Throw New ArgumentException("The PIN is empty.", NameOf(pin))
            End If

            Me.ValidateCredentials()

            Dim values As New NameValueCollection() From
                {
                    {"client_id", Me.ClientId},
                    {"client_secret", Me.ClientSecret},
                    {"grant_type", "pin"},
                    {"pin", pin.Trim()}
                }

            Me.ApplyTokens(Me.RequestTokens(values))

        End Sub

        ''' <summary>
        ''' Renews the access token using the refresh token.
        ''' <para></para>
        ''' This is a blocking network request. It is done automatically before the uploads that need it.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">
        ''' The application is not authorized, Imgur could not be reached or it could not process the request.
        ''' </exception>
        ''' <exception cref="ImgurAuthorizationException">
        ''' Imgur rejected the refresh token (it is invalid or the user revoked the authorization).
        ''' </exception>
        Public Sub RefreshAccessToken()

            If Not Me.IsAuthorized Then
                Throw New InvalidOperationException("The application is not authorized to use an Imgur account yet.")
            End If

            Me.ValidateCredentials()

            Dim values As New NameValueCollection() From
                {
                    {"client_id", Me.ClientId},
                    {"client_secret", Me.ClientSecret},
                    {"grant_type", "refresh_token"},
                    {"refresh_token", Me.RefreshToken}
                }

            Me.ApplyTokens(Me.RequestTokens(values))

        End Sub

        ''' <summary>
        ''' Restores an account that was authorized in a previous session.
        ''' The <see cref="ImgurAPI.OnAccountTokensChanged"/> event is not raised.
        ''' </summary>
        ''' <param name="refreshToken">The stored refresh token.</param>
        ''' <param name="username">The stored Imgur user name (can be empty).</param>
        Public Sub RestoreAccount(refreshToken As String, username As String)

            Me.RefreshToken = If(refreshToken, String.Empty).Trim()
            Me.accountUsernameValue = If(username, String.Empty).Trim()
            Me.accessToken = String.Empty
            Me.accessTokenExpiresUtc = Date.MinValue

        End Sub

        ''' <summary>
        ''' Forgets the authorized account. This does not revoke the authorization on the imgur website.
        ''' The <see cref="ImgurAPI.OnAccountTokensChanged"/> event is not raised.
        ''' </summary>
        Public Sub SignOut()

            Me.RestoreAccount(String.Empty, String.Empty)

        End Sub

#End Region

#Region " Public Shared Methods "

        ''' <summary>
        ''' Returns the url of the imgur page where a user can authorize the application.
        ''' After allowing it, Imgur shows a PIN to the user, which must be passed to
        ''' <see cref="ImgurAPI.AuthorizeWithPin"/>.
        ''' </summary>
        ''' <param name="clientId">
        ''' Indicates the unique client identifier that is provided with the imgur API application registration.
        ''' </param>
        ''' <returns>The authorization url.</returns>
        ''' <exception cref="ArgumentException">
        ''' <paramref name="clientId"/> is empty.
        ''' </exception>
        Public Shared Function GetAuthorizationUrl(clientId As String) As String

            If String.IsNullOrWhiteSpace(clientId) Then
                Throw New ArgumentException("The client id is empty.", NameOf(clientId))
            End If

            Return $"https://api.imgur.com/oauth2/authorize?client_id={Uri.EscapeDataString(clientId.Trim())}&response_type=pin"

        End Function

        ''' <summary>
        ''' Returns the value of the HTTP 'Authorization' header that the imgur API requires.
        ''' </summary>
        ''' <param name="mode">
        ''' Indicates whether the request is anonymous (uses the client id) or made on behalf of a user (uses an access token).
        ''' </param>
        ''' <param name="clientId">The client identifier of the application.</param>
        ''' <param name="accessToken">The access token of the user (it is not used by the anonymous mode).</param>
        ''' <returns>The header value.</returns>
        ''' <exception cref="ArgumentException">
        ''' The credential that the mode requires is empty.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' <paramref name="mode"/> is not a valid <see cref="ImgurUploadMode"/> value.
        ''' </exception>
        Public Shared Function GetAuthorizationHeaderValue(mode As ImgurUploadMode,
                                                          clientId As String,
                                                          accessToken As String) As String

            Select Case mode

                Case ImgurUploadMode.Anonymous
                    If String.IsNullOrWhiteSpace(clientId) Then
                        Throw New ArgumentException("The client id is empty.", NameOf(clientId))
                    End If
                    Return $"Client-ID {clientId.Trim()}"

                Case ImgurUploadMode.Account
                    If String.IsNullOrWhiteSpace(accessToken) Then
                        Throw New ArgumentException("The access token is empty.", NameOf(accessToken))
                    End If
                    Return $"Bearer {accessToken.Trim()}"

                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(mode), mode, "Invalid upload mode.")

            End Select

        End Function

        ''' <summary>
        ''' Returns the thumbnail image url of an imgur image url.
        ''' </summary>
        ''' <param name="ImageUrl">Indicates the imgur image url.</param>
        ''' <param name="Thumbnail">Indicates the kind of thumbnail url to get.</param>
        ''' <returns>The imgur thumbnail image url.</returns>
        Public Shared Function GetThumbnail(imageUrl As Uri,
thumbnail As ThumbnailType) As String

            Return String.Format("https://i.imgur.com/{0}{1}{2}",
                                 Path.GetFileNameWithoutExtension(imageUrl.AbsoluteUri),
                                 ImgurAPI.thumbnailKeys(thumbnail.ToString),
                                 Path.GetExtension(imageUrl.AbsoluteUri))

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Checks that the application credentials (client id and client secret) are set.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">
        ''' The 'ClientId' or the 'ClientSecret' property is empty.
        ''' </exception>
        Private Sub ValidateCredentials()

            If String.IsNullOrWhiteSpace(Me.ClientId) Then
                Throw New InvalidOperationException("'ClientId' property is empty.")
            End If

            If String.IsNullOrWhiteSpace(Me.ClientSecret) Then
                Throw New InvalidOperationException("'ClientSecret' property is empty.")
            End If

        End Sub

        ''' <summary>
        ''' Returns the value of the HTTP 'Authorization' header for the specified upload mode.
        ''' For <see cref="ImgurUploadMode.Account"/>, the access token is renewed first if it is missing or about to expire.
        ''' </summary>
        ''' <param name="mode">The upload mode.</param>
        ''' <returns>The header value.</returns>
        Private Function GetAuthorizationHeader(mode As ImgurUploadMode) As String

            If mode = ImgurUploadMode.Account Then

                If Not Me.IsAuthorized Then
                    Throw New InvalidOperationException("The application is not authorized to use an Imgur account yet.")
                End If

                If String.IsNullOrEmpty(Me.accessToken) OrElse (Date.UtcNow >= Me.accessTokenExpiresUtc) Then
                    Me.RefreshAccessToken()
                End If

            End If

            Return ImgurAPI.GetAuthorizationHeaderValue(mode, Me.ClientId, Me.accessToken)

        End Function

        ''' <summary>
        ''' Requests tokens to the imgur token endpoint.
        ''' </summary>
        ''' <param name="values">The form values of the request.</param>
        ''' <returns>The tokens that Imgur returned.</returns>
        ''' <exception cref="ImgurAuthorizationException">
        ''' Imgur rejected the request (HTTP 400 or 401): a wrong PIN, or an invalid or revoked refresh token.
        ''' </exception>
        ''' <exception cref="InvalidOperationException">
        ''' Imgur could not be reached, it failed to process the request, or its response is not valid.
        ''' </exception>
        Private Function RequestTokens(values As NameValueCollection) As ImgurTokens

            Dim responseText As String = String.Empty

            Using wc As New WebClient()

                Try
                    responseText = Encoding.UTF8.GetString(wc.UploadValues("https://api.imgur.com/oauth2/token", values))

                Catch ex As WebException

                    Dim httpResponse As HttpWebResponse = TryCast(ex.Response, HttpWebResponse)

                    If httpResponse Is Nothing Then
                        Throw New InvalidOperationException("Imgur could not be reached. Check your Internet connection and try again.", ex)
                    End If

                    Dim statusCode As Integer = CInt(httpResponse.StatusCode)
                    Dim reason As String = ImgurTokens.GetErrorMessage(ImgurAPI.ReadResponseText(httpResponse))
                    If reason.Length = 0 Then
                        reason = ex.Message
                    End If

                    ' Only a 400 (invalid pin, invalid or revoked refresh token) or 401 means that the authorization is wrong.
                    ' Other errors (rate limit, server errors...) are temporary and must not make the user to authorize again.
                    If (statusCode = 400) OrElse (statusCode = 401) Then
                        Throw New ImgurAuthorizationException($"Imgur rejected the authorization: {reason}", ex)
                    End If

                    Throw New InvalidOperationException($"Imgur could not process the authorization request (HTTP {statusCode}): {reason}", ex)

                End Try

            End Using

            Try
                Return ImgurTokens.Parse(responseText)

            Catch ex As FormatException
                Throw New InvalidOperationException("Imgur returned an unexpected authorization response.", ex)

            End Try

        End Function

        ''' <summary>
        ''' Stores the tokens that Imgur returned, and raises <see cref="ImgurAPI.OnAccountTokensChanged"/>
        ''' if the refresh token or the user name have changed.
        ''' </summary>
        ''' <param name="tokens">The tokens.</param>
        Private Sub ApplyTokens(tokens As ImgurTokens)

            Me.accessToken = tokens.AccessToken
            Me.accessTokenExpiresUtc = Date.UtcNow.AddSeconds(Math.Max(tokens.ExpiresInSeconds - AccessTokenSafetyMarginSeconds, 0))

            Dim changed As Boolean = False

            If (tokens.RefreshToken.Length > 0) AndAlso (tokens.RefreshToken <> Me.RefreshToken) Then
                Me.RefreshToken = tokens.RefreshToken
                changed = True
            End If

            If (tokens.Username.Length > 0) AndAlso (tokens.Username <> Me.accountUsernameValue) Then
                Me.accountUsernameValue = tokens.Username
                changed = True
            End If

            If changed Then
                RaiseEvent OnAccountTokensChanged(Me, EventArgs.Empty)
            End If

        End Sub

        ''' <summary>
        ''' Reads the body of an HTTP response as text.
        ''' </summary>
        ''' <param name="response">The HTTP response.</param>
        ''' <returns>The body, or an empty string if it cannot be read.</returns>
        Private Shared Function ReadResponseText(response As WebResponse) As String

            Try
                Using bodyStream As Stream = response.GetResponseStream()
                    If bodyStream Is Nothing Then
                        Return String.Empty
                    End If

                    Using reader As New StreamReader(bodyStream, Encoding.UTF8)
                        Return reader.ReadToEnd()
                    End Using
                End Using

            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is WebException OrElse TypeOf ex Is ObjectDisposedException
                Return String.Empty

            End Try

        End Function

        ''' <summary>
        ''' Raises the event that corresponds to the specified imgur status.
        ''' </summary>
        ''' <param name="status">The imgur status.</param>
        Private Sub RaiseStatusEvent(status As ImgurStatus)

            Select Case status

                Case ImgurStatus.AccessForbidden
                    RaiseEvent OnAccessForbidden(Me, status)

                Case ImgurStatus.AuthorizationFailed
                    RaiseEvent OnAuthorizationFailed(Me, status)

                Case ImgurStatus.BadImageFormat
                    RaiseEvent OnBadImageFormat(Me, status)

                Case ImgurStatus.InternalServerError
                    RaiseEvent OnInternalServerError(Me, status)

                Case ImgurStatus.PageIsNotFound
                    RaiseEvent OnPageIsNotFound(Me, status)

                Case ImgurStatus.UploadRateLimitError
                    RaiseEvent OnUploadRateLimitError(Me, status)

                Case Else
                    RaiseEvent OnUnknownError(Me, status)

            End Select

        End Sub

        Private Function GetResultFromStatus(status As Integer) As ImgurStatus

            Dim statusCode As ImgurStatus = ImgurStatus.UnknownError
            [Enum].TryParse(Of ImgurStatus)(value:=status.ToString, result:=statusCode)
            Return statusCode

        End Function

#End Region

    End Class

    ''' <summary>
    ''' Contains the urls that points to an uploaded image and it's thmbnails.
    ''' </summary>
    Public NotInheritable Class ImgurImage

#Region " Vars "

        ''' <summary>
        ''' Indicates the url of the uploaded image.
        ''' </summary>
        Private ReadOnly imageUrl As Uri = Nothing

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="imgurImage" /> class.
        ''' </summary>
        ''' <param name="ImageUrl">Indicates the url of the uploaded image.</param>
        Public Sub New(imageUrl As Uri)

            If imageUrl Is Nothing Then
                Throw New ArgumentNullException(NameOf(imageUrl))
            End If

            ' Imgur still returns 'http' links for some responses; always use HTTPS.
            Dim builder As New UriBuilder(imageUrl) With {.Scheme = Uri.UriSchemeHttps, .Port = -1}
            Me.imageUrl = builder.Uri
        End Sub

        ''' <summary>
        ''' Prevents a default instance of the <see cref="imgurImage"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Properties "

        ''' <summary>
        ''' The image at normal size.
        ''' </summary>
        Public ReadOnly Property Normal As String
            Get
                Return Me.imageUrl.AbsoluteUri
            End Get
        End Property

        ''' <summary>
        ''' The Imgur identifier of the image (the file name of the direct url, without extension).
        ''' </summary>
        Public ReadOnly Property Id As String
            Get
                Return Path.GetFileNameWithoutExtension(Me.imageUrl.AbsolutePath)
            End Get
        End Property

        ''' <summary>
        ''' The url of the Imgur page of the image (the 'Share Link').
        ''' </summary>
        Public ReadOnly Property ShareLink As String
            Get
                Return $"https://imgur.com/{Me.Id}"
            End Get
        End Property

        ''' <summary>
        ''' The image resized to 90x90 (without preserving image proportions).
        ''' </summary>
        Public ReadOnly Property SmallSquare As String
            Get
                Return ImgurAPI.GetThumbnail(Me.imageUrl, ImgurAPI.ThumbnailType.SmallSquare)
            End Get
        End Property

        ''' <summary>
        ''' The image resized to 160x160 (without preserving image proportions).
        ''' </summary>
        Public ReadOnly Property BigSquare As String
            Get
                Return ImgurAPI.GetThumbnail(Me.imageUrl, ImgurAPI.ThumbnailType.BigSquare)
            End Get
        End Property

        ''' <summary>
        ''' The image resized to 160x160 (preserving image proportions).
        ''' </summary>
        Public ReadOnly Property SmallThumbnail As String
            Get
                Return ImgurAPI.GetThumbnail(Me.imageUrl, ImgurAPI.ThumbnailType.SmallThumbnail)
            End Get
        End Property

        ''' <summary>
        ''' The image resized to 320x320 (without preserving image proportions).
        ''' </summary>
        Public ReadOnly Property MediumThumbnail As String
            Get
                Return ImgurAPI.GetThumbnail(Me.imageUrl, ImgurAPI.ThumbnailType.MediumThumbnail)
            End Get
        End Property

        ''' <summary>
        ''' The image resized to 640x640 (without preserving image proportions).
        ''' </summary>
        Public ReadOnly Property LargeThumbnail As String
            Get
                Return ImgurAPI.GetThumbnail(Me.imageUrl, ImgurAPI.ThumbnailType.LargeThumbnail)
            End Get
        End Property

        ''' <summary>
        ''' The image resized to 1024x1024 (without preserving image proportions).
        ''' </summary>
        Public ReadOnly Property HugeThumbnail As String
            Get
                Return ImgurAPI.GetThumbnail(Me.imageUrl, ImgurAPI.ThumbnailType.HugeThumbnail)
            End Get
        End Property

#End Region

    End Class

End Namespace

