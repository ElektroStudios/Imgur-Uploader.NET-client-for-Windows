
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Security.Cryptography ' Reference: System.Security
Imports System.Text

#End Region

#Region " UserAccountStore "

Namespace Tools

    ''' <summary>
    ''' Stores, for the current Windows user, the imgur account that authorized the application
    ''' and the preferred upload mode.
    ''' <para></para>
    ''' The data is saved in the profile of the user (not next to the executable, which is usually in a folder
    ''' that a standard user cannot write to), and the refresh token is encrypted with the Windows Data Protection API,
    ''' so it can only be decrypted by the same user on the same computer.
    ''' </summary>
    Public NotInheritable Class UserAccountStore

#Region " Constants "

        ''' <summary>
        ''' The key of the preferred upload mode.
        ''' </summary>
        Private Const UploadModeKey As String = "UploadMode"

        ''' <summary>
        ''' The key of the imgur user name.
        ''' </summary>
        Private Const UsernameKey As String = "Username"

        ''' <summary>
        ''' The key of the (encrypted) refresh token.
        ''' </summary>
        Private Const RefreshTokenKey As String = "RefreshToken"

#End Region

#Region " Fields "

        ''' <summary>
        ''' Serializes the access to the file.
        ''' </summary>
        Private Shared ReadOnly syncRoot As New Object()

        ''' <summary>
        ''' The additional entropy of the encryption, so other applications of the same user
        ''' cannot decrypt the token by accident.
        ''' </summary>
        Private Shared ReadOnly entropy As Byte() = Encoding.UTF8.GetBytes("ImgurUploader.NET/RefreshToken")

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the path of the file where the data is stored.
        ''' </summary>
        Public Shared Property FilePath As String =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "ElektroStudios", "Imgur Uploader.NET", "Account.dat")

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="UserAccountStore"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Shared Methods "

        ''' <summary>
        ''' Gets the preferred upload mode.
        ''' </summary>
        ''' <returns>
        ''' The stored mode, or <see cref="ImgurUploadMode.Account"/> if there is none (the default).
        ''' </returns>
        Public Shared Function LoadUploadMode() As ImgurUploadMode

            SyncLock syncRoot

                Dim values As Dictionary(Of String, String) = UserAccountStore.ReadValues()
                Dim text As String = Nothing

                If values.TryGetValue(UploadModeKey, text) Then
                    Dim mode As ImgurUploadMode
                    If [Enum].TryParse(Of ImgurUploadMode)(text, True, mode) AndAlso [Enum].IsDefined(GetType(ImgurUploadMode), mode) Then
                        Return mode
                    End If
                End If

                Return ImgurUploadMode.Account

            End SyncLock

        End Function

        ''' <summary>
        ''' Stores the preferred upload mode.
        ''' </summary>
        ''' <param name="mode">The upload mode.</param>
        ''' <returns><see langword="True"/> if it was saved; otherwise, <see langword="False"/>.</returns>
        Public Shared Function SaveUploadMode(mode As ImgurUploadMode) As Boolean

            SyncLock syncRoot

                Dim values As Dictionary(Of String, String) = UserAccountStore.ReadValues()
                values(UploadModeKey) = mode.ToString()
                Return UserAccountStore.WriteValues(values)

            End SyncLock

        End Function

        ''' <summary>
        ''' Gets the stored account.
        ''' </summary>
        ''' <param name="refreshToken">The decrypted refresh token, or an empty string if there is no valid account.</param>
        ''' <param name="username">The imgur user name, or an empty string if it is unknown.</param>
        ''' <returns>
        ''' <see langword="True"/> if there is a stored account whose refresh token could be decrypted;
        ''' otherwise, <see langword="False"/> (for example, when the file was copied from another computer or user).
        ''' </returns>
        Public Shared Function TryLoadAccount(ByRef refreshToken As String, ByRef username As String) As Boolean

            refreshToken = String.Empty
            username = String.Empty

            SyncLock syncRoot

                Dim values As Dictionary(Of String, String) = UserAccountStore.ReadValues()
                Dim encrypted As String = Nothing

                If Not values.TryGetValue(RefreshTokenKey, encrypted) OrElse String.IsNullOrWhiteSpace(encrypted) Then
                    Return False
                End If

                Try
                    Dim decrypted As Byte() = ProtectedData.Unprotect(Convert.FromBase64String(encrypted),
                                                                      entropy,
                                                                      DataProtectionScope.CurrentUser)
                    refreshToken = Encoding.UTF8.GetString(decrypted)

                Catch ex As Exception When TypeOf ex Is FormatException OrElse TypeOf ex Is CryptographicException
                    Return False

                End Try

                Dim storedName As String = Nothing
                If values.TryGetValue(UsernameKey, storedName) Then
                    username = storedName
                End If

                Return (refreshToken.Length > 0)

            End SyncLock

        End Function

        ''' <summary>
        ''' Stores the account that authorized the application.
        ''' </summary>
        ''' <param name="refreshToken">The refresh token. It is stored encrypted.</param>
        ''' <param name="username">The imgur user name.</param>
        ''' <returns>
        ''' <see langword="True"/> if it was saved; otherwise, <see langword="False"/>
        ''' (for example, when the profile folder cannot be written).
        ''' </returns>
        ''' <exception cref="ArgumentException">
        ''' <paramref name="refreshToken"/> is empty.
        ''' </exception>
        Public Shared Function SaveAccount(refreshToken As String, username As String) As Boolean

            If String.IsNullOrWhiteSpace(refreshToken) Then
                Throw New ArgumentException("The refresh token is empty.", NameOf(refreshToken))
            End If

            SyncLock syncRoot

                Dim values As Dictionary(Of String, String) = UserAccountStore.ReadValues()

                Try
                    Dim encrypted As Byte() = ProtectedData.Protect(Encoding.UTF8.GetBytes(refreshToken.Trim()),
                                                                    entropy,
                                                                    DataProtectionScope.CurrentUser)
                    values(RefreshTokenKey) = Convert.ToBase64String(encrypted)

                Catch ex As CryptographicException
                    Return False

                End Try

                values(UsernameKey) = If(username, String.Empty).Trim()
                Return UserAccountStore.WriteValues(values)

            End SyncLock

        End Function

        ''' <summary>
        ''' Removes the stored account (the preferred upload mode is kept).
        ''' </summary>
        ''' <returns><see langword="True"/> if the account was removed; otherwise, <see langword="False"/>.</returns>
        Public Shared Function ClearAccount() As Boolean

            SyncLock syncRoot

                Dim values As Dictionary(Of String, String) = UserAccountStore.ReadValues()

                If Not (values.Remove(RefreshTokenKey) Or values.Remove(UsernameKey)) Then
                    Return True
                End If

                Return UserAccountStore.WriteValues(values)

            End SyncLock

        End Function

#End Region

#Region " Private Shared Methods "

        ''' <summary>
        ''' Reads the stored values.
        ''' </summary>
        ''' <returns>
        ''' The values (the keys ignore the case), or an empty collection if the file does not exist or cannot be read.
        ''' </returns>
        Private Shared Function ReadValues() As Dictionary(Of String, String)

            Dim values As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            Try
                If File.Exists(FilePath) Then

                    For Each line As String In File.ReadAllLines(FilePath, Encoding.UTF8)

                        Dim separator As Integer = line.IndexOf("="c)
                        If separator > 0 Then
                            values(line.Substring(0, separator).Trim()) = line.Substring(separator + 1).Trim()
                        End If

                    Next

                End If

            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
                values.Clear()

            End Try

            Return values

        End Function

        ''' <summary>
        ''' Writes the values to the file, creating its folder if needed.
        ''' </summary>
        ''' <param name="values">The values.</param>
        ''' <returns><see langword="True"/> if the file was written; otherwise, <see langword="False"/>.</returns>
        Private Shared Function WriteValues(values As Dictionary(Of String, String)) As Boolean

            Try
                Dim folder As String = Path.GetDirectoryName(FilePath)
                If Not String.IsNullOrEmpty(folder) Then
                    Directory.CreateDirectory(folder)
                End If

                Dim lines As New List(Of String)
                For Each pair As KeyValuePair(Of String, String) In values
                    lines.Add($"{pair.Key}={pair.Value}")
                Next

                File.WriteAllLines(FilePath, lines, Encoding.UTF8)
                Return True

            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
                Return False

            End Try

        End Function

#End Region

    End Class

End Namespace

#End Region
