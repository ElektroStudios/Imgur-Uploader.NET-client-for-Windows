
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Web.Script.Serialization ' Reference: System.Web.Extensions

#End Region

#Region " ImgurTokens "

Namespace Tools

    ''' <summary>
    ''' Contains the OAuth tokens that Imgur returns when a user authorizes the application
    ''' (or when an access token is renewed with a refresh token).
    ''' </summary>
    Public NotInheritable Class ImgurTokens

#Region " Constants "

        ''' <summary>
        ''' The lifetime, in seconds, that is assumed when Imgur does not specify one.
        ''' </summary>
        Private Const DefaultExpiresInSeconds As Integer = 3600

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the access token, which authorizes the requests made on behalf of the user. It is short-lived.
        ''' </summary>
        Public ReadOnly Property AccessToken As String

        ''' <summary>
        ''' Gets the refresh token, which is used to get new access tokens. It does not expire.
        ''' It is empty if the response did not include one.
        ''' </summary>
        Public ReadOnly Property RefreshToken As String

        ''' <summary>
        ''' Gets the lifetime of the access token, in seconds.
        ''' </summary>
        Public ReadOnly Property ExpiresInSeconds As Integer

        ''' <summary>
        ''' Gets the Imgur user name of the account. It is empty if the response did not include it.
        ''' </summary>
        Public ReadOnly Property Username As String

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImgurTokens"/> class.
        ''' </summary>
        ''' <param name="accessToken">The access token.</param>
        ''' <param name="refreshToken">The refresh token (can be empty).</param>
        ''' <param name="expiresInSeconds">The lifetime of the access token, in seconds.</param>
        ''' <param name="username">The Imgur user name (can be empty).</param>
        ''' <exception cref="ArgumentException">
        ''' <paramref name="accessToken"/> is empty.
        ''' </exception>
        Public Sub New(accessToken As String,
                       refreshToken As String,
                       expiresInSeconds As Integer,
                       username As String)

            If String.IsNullOrWhiteSpace(accessToken) Then
                Throw New ArgumentException("The access token is empty.", NameOf(accessToken))
            End If

            Me.AccessToken = accessToken
            Me.RefreshToken = If(refreshToken, String.Empty)
            Me.ExpiresInSeconds = expiresInSeconds
            Me.Username = If(username, String.Empty)

        End Sub

#End Region

#Region " Public Shared Methods "

        ''' <summary>
        ''' Parses the JSON response of the Imgur token endpoint (https://api.imgur.com/oauth2/token).
        ''' </summary>
        ''' <param name="json">The JSON response.</param>
        ''' <returns>The tokens that the response contains.</returns>
        ''' <exception cref="FormatException">
        ''' The response is empty, it is not a valid JSON object, or it does not contain an access token.
        ''' </exception>
        Public Shared Function Parse(json As String) As ImgurTokens

            Dim data As Dictionary(Of String, Object) = ImgurTokens.ParseObject(json)

            If data Is Nothing Then
                Throw New FormatException("The Imgur token response is empty or it is not a valid JSON object.")
            End If

            Dim accessToken As String = ImgurTokens.GetString(data, "access_token")
            If accessToken.Length = 0 Then
                Throw New FormatException("The Imgur token response does not contain an access token.")
            End If

            Dim expiresIn As Integer = DefaultExpiresInSeconds
            Dim expiresValue As Object = ImgurTokens.GetValue(data, "expires_in")
            If expiresValue IsNot Nothing Then
                Dim parsedValue As Integer
                If Integer.TryParse(Convert.ToString(expiresValue, Globalization.CultureInfo.InvariantCulture),
                                    Globalization.NumberStyles.Integer,
                                    Globalization.CultureInfo.InvariantCulture,
                                    parsedValue) AndAlso (parsedValue > 0) Then
                    expiresIn = parsedValue
                End If
            End If

            Return New ImgurTokens(accessToken,
                                   ImgurTokens.GetString(data, "refresh_token"),
                                   expiresIn,
                                   ImgurTokens.GetString(data, "account_username"))

        End Function

        ''' <summary>
        ''' Extracts the human-readable error message from a JSON error response of Imgur.
        ''' </summary>
        ''' <param name="json">The JSON response.</param>
        ''' <returns>
        ''' The error message, or an empty string if the response is empty, invalid or does not contain a message.
        ''' </returns>
        Public Shared Function GetErrorMessage(json As String) As String

            Dim data As Dictionary(Of String, Object) = ImgurTokens.ParseObject(json)

            If data Is Nothing Then
                Return String.Empty
            End If

            ' Format: {"data":{"error":"..."},"success":false,"status":400}
            ' The error can also be an object: {"data":{"error":{"message":"..."}}}
            Dim nested As Dictionary(Of String, Object) = TryCast(ImgurTokens.GetValue(data, "data"), Dictionary(Of String, Object))
            If nested IsNot Nothing Then

                Dim nestedError As Object = ImgurTokens.GetValue(nested, "error")
                Dim errorText As String = TryCast(nestedError, String)

                If Not String.IsNullOrWhiteSpace(errorText) Then
                    Return errorText.Trim()
                End If

                Dim errorObject As Dictionary(Of String, Object) = TryCast(nestedError, Dictionary(Of String, Object))
                If errorObject IsNot Nothing Then
                    Dim message As String = ImgurTokens.GetString(errorObject, "message")
                    If message.Length > 0 Then
                        Return message
                    End If
                End If

            End If

            ' Format: {"error":"invalid_grant","error_description":"..."}
            Dim description As String = ImgurTokens.GetString(data, "error_description")
            If description.Length > 0 Then
                Return description
            End If

            Return ImgurTokens.GetString(data, "error")

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Deserializes a JSON object.
        ''' </summary>
        ''' <param name="json">The JSON text.</param>
        ''' <returns>
        ''' The deserialized object, or <see langword="Nothing"/> if the text is empty or it is not a JSON object.
        ''' </returns>
        Private Shared Function ParseObject(json As String) As Dictionary(Of String, Object)

            If String.IsNullOrWhiteSpace(json) Then
                Return Nothing
            End If

            Try
                Return TryCast(New JavaScriptSerializer().DeserializeObject(json), Dictionary(Of String, Object))

            Catch ex As ArgumentException
                Return Nothing

            Catch ex As InvalidOperationException
                Return Nothing

            End Try

        End Function

        ''' <summary>
        ''' Gets a value of a deserialized JSON object, ignoring the case of the key.
        ''' </summary>
        ''' <param name="data">The deserialized JSON object.</param>
        ''' <param name="key">The key.</param>
        ''' <returns>The value, or <see langword="Nothing"/> if the key does not exist.</returns>
        Private Shared Function GetValue(data As Dictionary(Of String, Object), key As String) As Object

            For Each pair As KeyValuePair(Of String, Object) In data
                If String.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase) Then
                    Return pair.Value
                End If
            Next

            Return Nothing

        End Function

        ''' <summary>
        ''' Gets a string value of a deserialized JSON object.
        ''' </summary>
        ''' <param name="data">The deserialized JSON object.</param>
        ''' <param name="key">The key.</param>
        ''' <returns>The trimmed value, or an empty string if the key does not exist or it is not a string.</returns>
        Private Shared Function GetString(data As Dictionary(Of String, Object), key As String) As String

            Dim value As String = TryCast(ImgurTokens.GetValue(data, key), String)
            Return If(value Is Nothing, String.Empty, value.Trim())

        End Function

#End Region

    End Class

    ''' <summary>
    ''' The exception that is thrown when Imgur rejects an authorization request:
    ''' a wrong or already used PIN, or a refresh token that is invalid or has been revoked by the user.
    ''' </summary>
    Public NotInheritable Class ImgurAuthorizationException
        Inherits Exception

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImgurAuthorizationException"/> class.
        ''' </summary>
        ''' <param name="message">The message that describes the error.</param>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImgurAuthorizationException"/> class.
        ''' </summary>
        ''' <param name="message">The message that describes the error.</param>
        ''' <param name="innerException">The exception that is the cause of the current exception.</param>
        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

End Namespace

#End Region
