' ***********************************************************************
' Author           : Elektro
' Last Modified On : 19-January-2015
' ***********************************************************************
' <copyright file="ImgurAPI.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

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
Imports System.Threading

#End Region

Namespace Tools

    ''' <summary>
    ''' Uploads anonymously an image using imgur API.
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

#End Region

#Region " Events "

        ''' <summary>
        ''' Occurs when imgur service throws an 'access forbidden' response.
        ''' </summary>
        Public Event OnAccessForbidden(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'Authorization Failed' response.
        ''' </summary>
        Public Event OnAuthorizationFailed(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws a 'Bad Image Format' response.
        ''' </summary>
        Public Event OnBadImageFormat(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'Internal Server Error' response.
        ''' </summary>
        Public Event OnInternalServerError(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'Page Is Not Found' response.
        ''' </summary>
        Public Event OnPageIsNotFound(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an 'access forbidden' response.
        ''' </summary>
        Public Event OnUploadRateLimitError(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an unknown (unhandled) response.
        ''' </summary>
        Public Event OnUnknownError(ByVal sender As Object, ByVal e As ImgurStatus)

        ''' <summary>
        ''' Occurs when imgur service throws an unknown (unhandled) response on an Asynchronous upload.
        ''' </summary>
        Public Event OnAsyncError(ByVal sender As Object, ByVal ex As Exception)

        ''' <summary>
        ''' Occurs when imgur service throws sucessful response on an Asynchronous upload.
        ''' </summary>
        Public Event OnAsyncSuccess(ByVal sender As Object, ByVal e As ImgurImage)

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
        Public Sub New(ByVal clientId As String, ByVal clientSecret As String)

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
        Private Sub WC_UploadValuesCompleted(ByVal sender As Object, ByVal e As UploadValuesCompletedEventArgs)

            If e.Error IsNot Nothing Then

                If TypeOf (e.Error.InnerException) Is ObjectDisposedException Then
                    ' Do Nothing

                Else 'If TypeOf (e.Error.InnerException) Is WebException Then
                    asyncError = True
                    RaiseEvent OnAsyncError(Me, e.Error)
                    Exit Sub

                End If

            End If

            If Not e.Cancelled Then
                responseAsync = e.Result
            End If

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Uploads an image anonymously to imgur.
        ''' </summary>
        ''' <param name="img">
        ''' Indicates a filename that points to an existing image.
        ''' </param>
        ''' <returns>An instance of the imgurImage Class that contains the url and it's thumbnail urls.</returns>
        Public Function UploadImage(ByVal img As String) As ImgurImage

            If String.IsNullOrEmpty(Me.ClientId) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientId) Then
                Throw New Exception("'ClientId' property is empty.")
            End If

            If String.IsNullOrEmpty(Me.ClientSecret) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientSecret) Then
                Throw New Exception("'ClientSecret' property is empty.")
            End If

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
                            {"Authorization", String.Format("Client-ID {0}", Me.ClientId)}
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
        ''' Uploads asynchronous an image  anonymously to imgur.
        ''' </summary>
        ''' </summary>
        ''' <param name="img">
        ''' Indicates a filename that points to an existing image.
        ''' </param>
        ''' <returns>An instance of the imgurImage Class that contains the url and it's thumbnail urls.</returns>
        Public Function UploadImageAsync(ByVal img As String) As ImgurImage

            cts = New CancellationTokenSource
            token = cts.Token

            If String.IsNullOrEmpty(Me.ClientId) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientId) Then
                Throw New Exception("'ClientId' property is empty.")
            End If

            If String.IsNullOrEmpty(Me.ClientSecret) _
            OrElse String.IsNullOrWhiteSpace(Me.ClientSecret) Then
                Throw New Exception("'ClientSecret' property is empty.")
            End If

            Try

                ' Create a WebClient. 
                Using wc As New WebClient()

                    Using ctr As CancellationTokenRegistration = token.Register(Sub() wc.CancelAsync())

                        ' Read the image.
                        Dim values As New NameValueCollection() From
                            {
                                {"image", Convert.ToBase64String(File.ReadAllBytes(img))}
                            }

                        ' Set the Headers.
                        Dim headers As New NameValueCollection() From
                            {
                                {"Authorization", String.Format("Client-ID {0}", Me.ClientId)}
                            }

                        ' Add the headers.
                        wc.Headers.Add(headers)

                        ' Upload the image, and get the response.
                        Try
                            responseAsync = Nothing
                            AddHandler wc.UploadValuesCompleted, AddressOf WC_UploadValuesCompleted
                            wc.UploadValuesAsync(New Uri("https://api.imgur.com/3/upload.xml"), values)

                            Do Until responseAsync IsNot Nothing OrElse asyncError
                                If cts.IsCancellationRequested OrElse asyncError Then
                                    asyncError = False
                                    Return Nothing
                                End If
                            Loop

                        Catch ex As Exception
                            RaiseEvent OnUnknownError(Me, ImgurStatus.UnknownError)
                            Return Nothing

                        End Try

                        ' Read the response (Converting Byte-Array to Stream).
                        Using sr As New StreamReader(New MemoryStream(responseAsync))

                            Dim serverResponse As String = sr.ReadToEnd
                            Dim xdoc As New XDocument(XDocument.Parse(serverResponse))
                            ' Dim status As ImgurStatus = Nothing

                            ' status = Me.GetResultFromStatus(Convert.ToInt32(xdoc.Root.LastAttribute.Value.ToString))

                            RaiseEvent OnAsyncSuccess(Me, New ImgurImage(New Uri(xdoc.Descendants("link").Value)))
                            Return New ImgurImage(New Uri(xdoc.Descendants("link").Value))

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
            cts.Cancel(True)
        End Sub

#End Region

#Region " Public Shared Methods "

        ''' <summary>
        ''' Returns the thumbnail image url of an imgur image url.
        ''' </summary>
        ''' <param name="ImageUrl">Indicates the imgur image url.</param>
        ''' <param name="Thumbnail">Indicates the kind of thumbnail url to get.</param>
        ''' <returns>The imgur thumbnail image url.</returns>
        Public Shared Function GetThumbnail(ByVal imageUrl As Uri,
                                            ByVal thumbnail As ThumbnailType) As String

            Return String.Format("http://i.imgur.com/{0}{1}{2}",
                                 Path.GetFileNameWithoutExtension(imageUrl.AbsoluteUri),
                                 ImgurAPI.thumbnailKeys(thumbnail.ToString),
                                 Path.GetExtension(imageUrl.AbsoluteUri))

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Gets the result from status.
        ''' </summary>
        ''' <param name="Status">The status.</param>
        ''' <returns>imgurStatus.</returns>
        Private Function GetResultFromStatus(ByVal status As Integer) As ImgurStatus

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
        Public Sub New(ByVal imageUrl As Uri)
            Me.imageUrl = imageUrl
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