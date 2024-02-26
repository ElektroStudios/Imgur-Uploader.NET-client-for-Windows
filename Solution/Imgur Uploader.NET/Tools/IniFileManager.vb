
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Usage Examples "

'' Set the initialization file path.
'INIFileManager.FilePath = IO.Path.Combine(Application.StartupPath, "Config.ini")

'' Create the initialization file.
'INIFileManager.File.Create()

'' Check that the initialization file exist.
'MsgBox(INIFileManager.File.Exist)

'' Writes a new entire initialization file with the specified text content.
'INIFileManager.File.Write(New List(Of String) From {"[Section Name 1]"})

'' Set an existing value or append it at the enf of the initialization file.
'INIFileManager.Key.Set("KeyName1", "Value1")

'' Set an existing value on a specific section or append them at the enf of the initialization file.
'INIFileManager.Key.Set("KeyName2", "Value2", "[Section Name 2]")

'' Gets the value of the specified Key name,
'MsgBox(INIFileManager.Key.Get("KeyName1"))

'' Gets the value of the specified Key name on the specified Section.
'MsgBox(INIFileManager.Key.Get("KeyName2", , "[Section Name 2]"))

'' Gets the value of the specified Key name and returns a default value if the key name is not found.
'MsgBox(INIFileManager.Key.Get("KeyName0", "I'm a default value"))

'' Gets the value of the specified Key name, and assign it to a control property.
'CheckBox1.Checked = CType(INIFileManager.Key.Get("KeyName1"), Boolean)

'' Checks whether a Key exists.
'MsgBox(INIFileManager.Key.Exist("KeyName1"))

'' Checks whether a Key exists on a specific section.
'MsgBox(INIFileManager.Key.Exist("KeyName2", "[First Section]"))

'' Remove a key name.
'INIFileManager.Key.Remove("KeyName1")

'' Remove a key name on the specified Section.
'INIFileManager.Key.Remove("KeyName2", "[Section Name 2]")

'' Add a new section.
'INIFileManager.Section.Add("[Section Name 3]")

'' Get the contents of a specific section.
'MsgBox(String.Join(Environment.NewLine, INIFileManager.Section.Get("[Section Name 1]")))

'' Remove an existing section.
'INIFileManager.Section.Remove("[Section Name 2]")

'' Checks that the initialization file contains at least one section.
'MsgBox(INIFileManager.Section.Has())

'' Sort the initialization file (And remove empty lines).
'INIFileManager.File.Sort(True)

'' Gets the initialization file section names.
'MsgBox(String.Join(", ", INIFileManager.Section.GetNames()))

'' Gets the initialization file content.
'MsgBox(String.Join(Environment.NewLine, INIFileManager.File.Get()))

'' Delete the initialization file from disk.
'INIFileManager.File.Delete()

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Text

#End Region

#Region " INI File Manager "

Namespace Tools

    ''' <summary>
    ''' Manages an INI file and it's sections to load/save values.
    ''' </summary>
    Public Class IniFileManager

#Region " Properties "

        ''' <summary>
        ''' Indicates the initialization file location.
        ''' </summary>
        Public Shared Property FilePath As String =
            IO.Path.Combine(Application.StartupPath, Process.GetCurrentProcess().ProcessName & ".ini")

        ''' <summary>
        ''' Indicates the initialization file encoding to read/write.
        ''' </summary>
        Public Shared Property TextEncoding As Encoding = Encoding.Default

#End Region

#Region " Variables "

        ''' <summary>
        ''' Stores the initialization file content.
        ''' </summary>
        Private Shared content As New List(Of String)

        ''' <summary>
        ''' Stores the INI section names.
        ''' </summary>
        Private Shared sectionNames As String() = {String.Empty}

        ''' <summary>
        ''' Indicates the start element index of a section name.
        ''' </summary>
        Private Shared sectionStartIndex As Integer = -1

        ''' <summary>
        ''' Indicates the end element index of a section name.
        ''' </summary>
        Private Shared sectionEndIndex As Integer = -1

        ''' <summary>
        ''' Stores a single sorted section block with their keys and values.
        ''' </summary>
        Private Shared sortedSection As New List(Of String)

        ''' <summary>
        ''' Stores all the sorted section blocks with their keys and values.
        ''' </summary>
        Private Shared ReadOnly sortedSections As List(Of String) = New List(Of String)

        ''' <summary>
        ''' Indicates the INI element index that contains the Key and value.
        ''' </summary>
        Private Shared keyIndex As Integer = -1

        ''' <summary>
        ''' Indicates the culture to compare the strings.
        ''' </summary>
        Private Shared ReadOnly compareMode As StringComparison = StringComparison.InvariantCultureIgnoreCase

#End Region

#Region " Exceptions "

        ''' <summary>
        ''' Exception is thrown when a section name parameter has invalid format.
        ''' </summary>
        <Serializable()>
        Private Class SectionNameInvalidFormatException : Inherits Exception

            ''' <summary>
            ''' Initializes a new instance of the <see cref="SectionNameInvalidFormatException"/> class.
            ''' </summary>
            Public Sub New()

                MyBase.New("Section name parameter has invalid format." _
                           & Environment.NewLine _
                           & "The rigth syntax is: [SectionName]")

            End Sub

            ''' <summary>
            ''' Initializes a new instance of the <see cref="SectionNameInvalidFormatException" /> class with a specified error message.
            ''' </summary>
            ''' <param name="Message">Indicates the message that describes the error.</param>
            Public Sub New(message As String)

                MyBase.New(message)

            End Sub

            ''' <summary>
            ''' Initializes a new instance of the <see cref="SectionNameInvalidFormatException"/> class.
            ''' </summary>
            ''' <param name="Message">Indicates the message that describes the error.</param>
            ''' <param name="InnerException">Indicates the InnerException message.</param>
            Public Sub New(message As String,
innerException As Exception)

                MyBase.New(message, innerException)

            End Sub

        End Class '/ SectionNameInvalidFormatException

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="INIFileManager"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="INIFileManager" /> class.
        ''' </summary>
        ''' <param name="IniFile">
        ''' Indicates the initialization file location.
        ''' ( If this parametter is not set, the INIFileManager instance assumes that
        ''' the ini file is in the startup folder and has the same name as the running process. )
        ''' </param>
        ''' <param name="IniEncoding">Indicates a textencoding to read/write the ini file.</param>
        Public Sub New(Optional iniFile As String = Nothing,
                       Optional iniEncoding As Encoding = Nothing)

            If Not String.IsNullOrEmpty(iniFile) Then
                FilePath = iniFile
            End If

            If Not iniEncoding Is Nothing Then
                TextEncoding = iniEncoding
            End If

        End Sub

#End Region
#Region " Methods "

        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Shadows Sub ReferenceEquals()
        End Sub

        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Shadows Sub Equals()
        End Sub

        ''' <summary>
        ''' Contains a set of procedures to manage the INI file in a general way.
        ''' </summary>
        Public Class [File]

            <EditorBrowsable(EditorBrowsableState.Never)>
            Private Shadows Sub ReferenceEquals()
            End Sub

            <EditorBrowsable(EditorBrowsableState.Never)>
            Private Shadows Sub Equals()
            End Sub

            ''' <summary>
            ''' Checks whether the initialization file exist.
            ''' </summary>
            ''' <returns>True if initialization file exist, otherwise False.</returns>
            Public Shared Function Exist() As Boolean
                Return IO.File.Exists(FilePath)
            End Function

            ''' <summary>
            ''' Creates the initialization file.
            ''' If the file already exist it would be replaced.
            ''' </summary>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Create() As Boolean

                Try
                    IO.File.WriteAllText(FilePath, String.Empty, TextEncoding)
                    Return True

                Catch ex As Exception
                    Throw New Exception(ex.Message)
                    Return False

                End Try

            End Function

            ''' <summary>
            ''' Deletes the initialization file.
            ''' </summary>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Delete() As Boolean

                Try
                    IO.File.Delete(FilePath)
                    content = Nothing
                    Return True

                Catch ex As Exception
                    Throw New Exception(ex.Message)
                    Return False

                End Try

            End Function

            ''' <summary>
            ''' Returns the initialization file content.
            ''' </summary>
            Public Shared Function [Get]() As List(Of String)

                content = IO.File.ReadAllLines(FilePath, TextEncoding).ToList()
                Return content

            End Function

            ''' <summary>
            ''' Sort the initialization file content by the Key names.
            ''' If the initialization file contains sections then the sections are sorted by their names also.
            ''' </summary>
            ''' <param name="RemoveEmptyLines">Remove empty lines.</param>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Sort(Optional removeEmptyLines As Boolean = False) As Boolean

                If Not [File].Exist() Then Return False

                [File].[Get]()

                Select Case Section.Has()

                    Case True ' initialization file contains at least one Section.

                        sortedSection.Clear()
                        sortedSections.Clear()

                        Section.GetNames() ' Get the (sorted) section names

                        For Each name As String In sectionNames

                            sortedSection = Section.[Get](name) ' Get the single section lines.

                            If removeEmptyLines Then ' Remove empty lines.
                                sortedSection = sortedSection.Where(Function(line) _
                                                                    Not String.IsNullOrEmpty(line) AndAlso
                                                                    Not String.IsNullOrWhiteSpace(line)).ToList
                            End If

                            sortedSection.Sort() ' Sort the single section keys.

                            sortedSections.Add(name) ' Add the section name to the sorted sections list.
                            sortedSections.AddRange(sortedSection) ' Add the single section to the sorted sections list.

                        Next name

                        content = sortedSections

                    Case False ' initialization file doesn't contains any Section.
                        content.Sort()

                        If removeEmptyLines Then
                            content = content.Where(Function(line) _
                                                            Not String.IsNullOrEmpty(line) AndAlso
                                                            Not String.IsNullOrWhiteSpace(line)).ToList
                        End If

                End Select ' Section.Has()

                ' Save changes.
                Return [File].Write(content)

            End Function

            ''' <summary>
            ''' Writes a new initialization file with the specified text content..
            ''' </summary>
            ''' <param name="Content">Indicates the text content to write in the initialization file.</param>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Write(content As List(Of String)) As Boolean

                Try
                    IO.File.WriteAllLines(FilePath, content, TextEncoding)
                    Return True

                Catch ex As Exception
                    Throw New Exception(ex.Message)
                    Return False

                End Try

            End Function

        End Class

        ''' <summary>
        ''' Contains a set of procedures to manage the INI keys/values.
        ''' </summary>
        Public Class [Key]

            <EditorBrowsable(EditorBrowsableState.Never)>
            Private Shadows Sub ReferenceEquals()
            End Sub

            <EditorBrowsable(EditorBrowsableState.Never)>
            Private Shadows Sub Equals()
            End Sub

            ''' <summary>
            ''' Return a value indicating whether a key name exist or not.
            ''' </summary>
            ''' <param name="KeyName">Indicates the key name that contains the value to modify.</param>
            ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
            ''' <returns>True if the key name exist, otherwise False.</returns>
            Public Shared Function Exist(keyName As String,
                                         Optional sectionName As String = Nothing) As Boolean

                If Not [File].Exist() Then Return False

                [File].[Get]()

                [Key].GetIndex(keyName, sectionName)

                Select Case sectionName Is Nothing

                    Case True
                        Return Convert.ToBoolean(Not keyIndex)

                    Case Else
                        Return Convert.ToBoolean(Not (keyIndex + sectionStartIndex))

                End Select

            End Function

            ''' <summary>
            ''' Set the value of an existing key name.
            ''' 
            ''' If the initialization file doesn't exists, or else the Key doesn't exist,
            ''' or else the Section parameter is not specified and the key name doesn't exist;
            ''' then the 'key=value' is appended to the end of the initialization file.
            ''' 
            ''' if the specified Section name exist but the Key name doesn't exist,
            ''' then the 'key=value' is appended to the end of the Section.
            ''' 
            ''' </summary>
            ''' <param name="KeyName">Indicates the key name that contains the value to modify.</param>
            ''' <param name="Value">Indicates the new value.</param>
            ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function [Set](keyName As String,
value As String,
                                         Optional sectionName As String = Nothing) As Boolean

                If Not [File].Exist() Then [File].Create()

                [File].[Get]()

                [Key].GetIndex(keyName, sectionName)

                ' If KeyName is not found and indicated Section is found, then...
                If keyIndex = -1 AndAlso sectionEndIndex <> -1 Then

                    ' If section EndIndex is the last line of file, then...
                    If sectionEndIndex = content.Count Then

                        content(content.Count - 1) = content(content.Count - 1) &
                                                             Environment.NewLine &
                                                             String.Format("{0}={1}", keyName, value)

                    Else ' If not section EndIndex is the last line of file, then...

                        content(sectionEndIndex) = String.Format("{0}={1}", keyName, value) &
                                                        Environment.NewLine &
                                                        content(sectionEndIndex)
                    End If

                    ' If KeyName is found then...
                ElseIf keyIndex <> -1 Then
                    content(keyIndex) = String.Format("{0}={1}", keyName, value)

                    ' If KeyName is not found and Section parameter is passed. then...
                ElseIf keyIndex = -1 AndAlso sectionName IsNot Nothing Then
                    content.Add(sectionName)
                    content.Add(String.Format("{0}={1}", keyName, value))

                    ' If KeyName is not found, then...
                ElseIf keyIndex = -1 Then
                    content.Add(String.Format("{0}={1}", keyName, value))

                End If

                ' Save changes.
                Return [File].Write(content)

            End Function

            ''' <summary>
            ''' Get the value of an existing key name.
            ''' If the initialization file or else the Key doesn't exist then a 'Nothing' object is returned. 
            ''' </summary>
            ''' <param name="KeyName">Indicates the key name to retrieve their value.</param>
            ''' <param name="DefaultValue">Indicates a default value to return if the key name is not found.</param>
            ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
            Public Shared Function [Get](keyName As String,
                                         Optional defaultValue As Object = Nothing,
                                         Optional sectionName As String = Nothing) As Object

                If Not [File].Exist() Then Return defaultValue

                [File].[Get]()

                [Key].GetIndex(keyName, sectionName)

                Select Case keyIndex

                    Case Is <> -1 ' KeyName found.
                        Return content(keyIndex).Substring(content(keyIndex).IndexOf("=") + 1)

                    Case Else ' KeyName not found.
                        Return defaultValue

                End Select

            End Function

            ''' <summary>
            ''' Returns the initialization file line index of the key name.
            ''' </summary>
            ''' <param name="KeyName">Indicates the Key name to retrieve their value.</param>
            ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
            Private Shared Sub GetIndex(keyName As String,
                                        Optional sectionName As String = Nothing)

                If content Is Nothing Then [File].Get()

                ' Reset the INI index elements to negative values.
                keyIndex = -1
                sectionStartIndex = -1
                sectionEndIndex = -1

                If sectionName IsNot Nothing AndAlso Not sectionName Like "[[]?*[]]" Then
                    Throw New SectionNameInvalidFormatException
                    Exit Sub
                End If

                ' Locate the KeyName and set their element index.
                ' If the KeyName is not found then the value is set to "-1" to return an specified default value.
                Select Case String.IsNullOrEmpty(sectionName)

                    Case True ' Any SectionName parameter is specified.

                        keyIndex = content.FindIndex(Function(line) line.StartsWith(String.Format("{0}=", keyName),
                                                                                  StringComparison.InvariantCultureIgnoreCase))

                    Case False ' SectionName parameter is specified.

                        Select Case Section.Has()

                            Case True ' INI contains at least one Section.

                                sectionStartIndex = content.FindIndex(Function(line) line.Trim.Equals(sectionName.Trim, compareMode))
                                If sectionStartIndex = -1 Then ' Section doesn't exist.
                                    Exit Sub
                                End If

                                sectionEndIndex = content.FindIndex(sectionStartIndex + 1, Function(line) line.Trim Like "[[]?*[]]")
                                If sectionEndIndex = -1 Then
                                    ' This fixes the value if the section is at the end of file.
                                    sectionEndIndex = content.Count
                                End If

                                keyIndex = content.FindIndex(sectionStartIndex, sectionEndIndex - sectionStartIndex,
                                                                      Function(line) line.StartsWith(String.Format("{0}=", keyName),
                                                                                          StringComparison.InvariantCultureIgnoreCase))

                            Case False ' INI doesn't contains Sections.
                                GetIndex(keyName, )

                        End Select ' Section.Has()

                End Select ' String.IsNullOrEmpty(SectionName)

            End Sub

            ''' <summary>
            ''' Remove an existing key name.
            ''' </summary>
            ''' <param name="KeyName">Indicates the key name to retrieve their value.</param>
            ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Remove(keyName As String,
                                          Optional sectionName As String = Nothing) As Boolean

                If Not [File].Exist() Then Return False

                [File].[Get]()

                [Key].GetIndex(keyName, sectionName)

                Select Case keyIndex

                    Case Is <> -1 ' Key found.

                        ' Remove the element containing the key name.
                        content.RemoveAt(keyIndex)

                        ' Save changes.
                        Return [File].Write(content)

                    Case Else ' KeyName not found.
                        Return False

                End Select

            End Function

        End Class

        ''' <summary>
        ''' Contains a set of procedures to manage the INI sections.
        ''' </summary>
        Public Class Section

            <EditorBrowsable(EditorBrowsableState.Never)>
            Private Shadows Sub ReferenceEquals()
            End Sub

            <EditorBrowsable(EditorBrowsableState.Never)>
            Private Shadows Sub Equals()
            End Sub

            ''' <summary>
            ''' Adds a new section at bottom of the initialization file.
            ''' </summary>
            ''' <param name="SectionName">Indicates the Section name to add.</param>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Add(Optional sectionName As String = Nothing) As Boolean

                If Not [File].Exist() Then [File].Create()

                If Not sectionName Like "[[]?*[]]" Then
                    Throw New SectionNameInvalidFormatException
                    Exit Function
                End If

                [File].[Get]()

                Select Case Section.GetNames().Where(Function(line) line.Trim.Equals(sectionName.Trim, compareMode)).Any

                    Case False ' Any of the existing Section names is equal to given section name.

                        ' Add the new section name.
                        content.Add(sectionName)

                        ' Save changes.
                        Return [File].Write(content)

                    Case Else ' An existing Section name is equal to given section name.
                        Return False

                End Select

            End Function

            ''' <summary>
            ''' Returns all the keys and values of an existing Section Name.
            ''' </summary>
            ''' <param name="SectionName">Indicates the section name where to retrieve their keynames and values.</param>
            Public Shared Function [Get](sectionName As String) As List(Of String)

                If content Is Nothing Then [File].Get()

                sectionStartIndex = content.FindIndex(Function(line) line.Trim.Equals(sectionName.Trim, compareMode))

                sectionEndIndex = content.FindIndex(sectionStartIndex + 1, Function(line) line.Trim Like "[[]?*[]]")

                If sectionEndIndex = -1 Then
                    sectionEndIndex = content.Count ' This fixes the value if the section is at the end of file.
                End If

                Return content.GetRange(sectionStartIndex, sectionEndIndex - sectionStartIndex).Skip(1).ToList

            End Function

            ''' <summary>
            ''' Returns all the section names of the initialization file.
            ''' </summary>
            Public Shared Function GetNames() As String()

                If content Is Nothing Then [File].Get()

                ' Get the Section names.
                sectionNames = (From line In content Where line.Trim Like "[[]?*[]]").ToArray

                ' Sort the Section names.
                If sectionNames.Count <> 0 Then Array.Sort(sectionNames)

                ' Return the Section names.
                Return sectionNames

            End Function

            ''' <summary>
            ''' Gets a value indicating whether the initialization file contains at least one Section.
            ''' </summary>
            ''' <returns>True if the INI contains at least one section, otherwise False.</returns>
            Public Shared Function Has() As Boolean

                If content Is Nothing Then [File].Get()

                Return (From line In content Where line.Trim Like "[[]?*[]]").Any()

            End Function

            ''' <summary>
            ''' Removes an existing section with all of it's keys and values.
            ''' </summary>
            ''' <param name="SectionName">Indicates the Section name to remove with all of it's key/values.</param>
            ''' <returns>True if the operation success, otherwise False.</returns>
            Public Shared Function Remove(Optional sectionName As String = Nothing) As Boolean

                If Not [File].Exist() Then Return False

                If Not sectionName Like "[[]?*[]]" Then
                    Throw New SectionNameInvalidFormatException
                    Exit Function
                End If

                [File].[Get]()

                Select Case [Section].GetNames().Where(Function(line) line.Trim.Equals(sectionName.Trim, compareMode)).Any

                    Case True ' An existing Section name is equal to given section name.

                        ' Get the section StartIndex and EndIndex.
                        [Get](sectionName)

                        ' Remove the section range index.
                        content.RemoveRange(sectionStartIndex, sectionEndIndex - sectionStartIndex)

                        ' Save changes.
                        Return [File].Write(content)

                    Case Else ' Any of the existing Section names is equal to given section name.
                        Return False

                End Select

            End Function

        End Class

#End Region

    End Class

End Namespace

#End Region