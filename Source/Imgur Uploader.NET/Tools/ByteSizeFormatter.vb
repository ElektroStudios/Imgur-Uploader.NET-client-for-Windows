
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices
Imports System.Text

#End Region

#Region " ByteSizeFormatter "

Namespace Tools

    ''' <summary>
    ''' Formats byte counts as Windows Explorer does (for example "1.25 MB"), using the Windows shell API.
    ''' </summary>
    Public NotInheritable Class ByteSizeFormatter

#Region " P/Invoke "

        ''' <summary>
        ''' Converts a numeric value into a string that represents the number expressed as a size value in bytes, KB, MB, GB, TB, PB, or EB.
        ''' </summary>
        <DllImport("shlwapi.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True, PreserveSig:=True)>
        Private Shared Function StrFormatByteSizeEx(size As ULong,
                                                    flags As Integer,
                                                    buffer As StringBuilder,
                                                    bufferSize As Integer) As Integer
        End Function

        ''' <summary>
        ''' SFBS_FLAGS_TRUNCATE_UNDISPLAYED_DECIMAL_DIGITS (0x0002): truncates the undisplayed decimal digits,
        ''' like the Windows Explorer properties dialog does.
        ''' </summary>
        Private Const TruncateUndisplayedDecimalDigits As Integer = 2

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
        ''' Formats a byte count in the same way as Windows Explorer.
        ''' </summary>
        ''' <param name="byteCount">The number of bytes.</param>
        ''' <returns>The formatted size, in the user's culture.</returns>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="byteCount"/> is negative.</exception>
        Public Shared Function Format(byteCount As Long) As String

            If byteCount < 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(byteCount), byteCount, "The byte count cannot be negative.")
            End If

            Dim buffer As New StringBuilder(32)
            Dim result As Integer = StrFormatByteSizeEx(CULng(byteCount), TruncateUndisplayedDecimalDigits, buffer, buffer.Capacity)

            If result <> 0 Then
                Throw New InvalidOperationException($"StrFormatByteSizeEx failed with HRESULT 0x{result:X8}.", Marshal.GetExceptionForHR(result))
            End If

            Return buffer.ToString()

        End Function

#End Region

    End Class

End Namespace

#End Region
