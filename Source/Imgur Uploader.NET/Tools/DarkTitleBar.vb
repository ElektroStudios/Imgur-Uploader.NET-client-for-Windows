
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.Win32
Imports System.Runtime.InteropServices

#End Region

#Region " DarkTitleBar "

Namespace Tools

    ''' <summary>
    ''' Provides methods to switch the title bar of a window to the dark theme (Windows 10 build 18985+ and Windows 11).
    ''' </summary>
    Public NotInheritable Class DarkTitleBar

#Region " Constants "

        ''' <summary>
        ''' DWMWA_USE_IMMERSIVE_DARK_MODE for Windows 10 builds before 20H1.
        ''' </summary>
        Private Const DwmwaUseImmersiveDarkModeBefore20H1 As Integer = 19

        ''' <summary>
        ''' DWMWA_USE_IMMERSIVE_DARK_MODE.
        ''' </summary>
        Private Const DwmwaUseImmersiveDarkMode As Integer = 20

        ''' <summary>
        ''' The registry key that stores the Windows theme preferences.
        ''' </summary>
        Private Const PersonalizeKeyPath As String = "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"

        ''' <summary>
        ''' The registry value that is 0 when the apps use the dark theme.
        ''' </summary>
        Private Const AppsUseLightThemeValueName As String = "AppsUseLightTheme"

#End Region

#Region " P/Invoke "

        <DllImport("dwmapi.dll", PreserveSig:=True)>
        Private Shared Function DwmSetWindowAttribute(hwnd As IntPtr,
                                                      attribute As Integer,
                                                      ByRef attributeValue As Integer,
                                                      attributeSize As Integer) As Integer
        End Function

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents instantiation of the class.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Determines whether the user has selected the dark theme for apps in the Windows settings.
        ''' </summary>
        ''' <returns>
        ''' <see langword="True"/> if the dark theme is enabled; otherwise, <see langword="False"/>
        ''' (also when the setting does not exist, for example on old Windows versions).
        ''' </returns>
        Public Shared Function IsAppsDarkThemeEnabled() As Boolean

            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(PersonalizeKeyPath, writable:=False)

                If key Is Nothing Then
                    Return False
                End If

                Dim value As Object = key.GetValue(AppsUseLightThemeValueName)
                Return (TypeOf value Is Integer) AndAlso (CInt(value) = 0)

            End Using

        End Function

        ''' <summary>
        ''' Enables or disables the dark title bar of a window.
        ''' This method must be called when the window handle is created (<see cref="Control.OnHandleCreated"/>).
        ''' </summary>
        ''' <param name="windowHandle">The window handle.</param>
        ''' <param name="enabled"><see langword="True"/> to use the dark title bar; <see langword="False"/> to restore the default one.</param>
        ''' <returns>
        ''' <see langword="True"/> if the attribute was applied; <see langword="False"/> if the operating system does not support it.
        ''' </returns>
        ''' <exception cref="ArgumentException">
        ''' <paramref name="windowHandle"/> is <see cref="IntPtr.Zero"/>.
        ''' </exception>
        Public Shared Function SetDarkTitleBar(windowHandle As IntPtr, enabled As Boolean) As Boolean

            If windowHandle = IntPtr.Zero Then
                Throw New ArgumentException("The window handle is not valid.", NameOf(windowHandle))
            End If

            Dim value As Integer = If(enabled, 1, 0)

            Try
                If DwmSetWindowAttribute(windowHandle, DwmwaUseImmersiveDarkMode, value, Marshal.SizeOf(Of Integer)()) = 0 Then
                    Return True
                End If

                Return (DwmSetWindowAttribute(windowHandle, DwmwaUseImmersiveDarkModeBefore20H1, value, Marshal.SizeOf(Of Integer)()) = 0)

            Catch ex As DllNotFoundException
                Return False

            Catch ex As EntryPointNotFoundException
                Return False

            End Try

        End Function

        ''' <summary>
        ''' Applies the dark title bar only when the Windows apps theme is dark.
        ''' </summary>
        ''' <param name="windowHandle">The window handle.</param>
        ''' <returns><see langword="True"/> if the attribute was applied; otherwise, <see langword="False"/>.</returns>
        Public Shared Function ApplyCurrentTheme(windowHandle As IntPtr) As Boolean

            Return DarkTitleBar.SetDarkTitleBar(windowHandle, DarkTitleBar.IsAppsDarkThemeEnabled())

        End Function

#End Region

    End Class

End Namespace

#End Region
