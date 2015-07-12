'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports System.Collections.Specialized
Imports System.ServiceModel.Syndication
Imports System.Text
Imports System.Collections.Generic

''' <summary>
''' Class for common methods
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[pdonker]	03/01/2008  Created
''' </history>
Public Module Common

 Public Const LocalTransformationPath As String = "\Newsfeeds\Transformations"
 Public Const GlobalTransformationPath As String = "~/DesktopModules/News/Resources/Transformations"
 Public Const glbNewsNs As String = "http://www.dotnetnuke.com/modules/news"
 Public Const glbNewsNsAbbr As String = "news"


 ''' <summary>
 ''' Get the en/decryption key from the web.config
 ''' </summary>
 ''' <returns>Encryption key (as string)</returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Function GetEncryptionKey() As String
  Try
   Dim xmlConfig As XmlDocument = Config.Load
   Dim xmlMachineKey As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web/machineKey")
   Return xmlMachineKey.Attributes("decryptionKey").InnerText
  Catch ex As Exception
   Return ""
  End Try
 End Function

 ''' <summary>
 ''' Save a block of text to a file
 ''' </summary>
 ''' <param name="FileName">Full path to file. File will be created or overwritten.</param>
 ''' <param name="Text">Text to write to the file.</param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Sub SaveTextToFile(ByVal FileName As String, ByVal Text As String)
  Dim sw As New IO.StreamWriter(FileName, False)
  sw.WriteLine(Text)
  sw.Flush()
  sw.Close()
  sw = Nothing
 End Sub

 ''' <summary>
 ''' Write feed as text to a file.
 ''' </summary>
 ''' <param name="feed">Rss XML Document to save.</param>
 ''' <param name="ModuleId">ModuleId</param>
 ''' <param name="CachePath">Absolute path to the caching directory.</param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Sub SaveResultFeed(ByVal feed As XmlDocument, ByVal ModuleId As Integer, ByVal CachePath As String)
  Using xmlw As New XmlTextWriter(CachePath & CacheFeedname(ModuleId), System.Text.Encoding.UTF8)
   xmlw.Formatting = Formatting.Indented
   feed.WriteTo(xmlw)
  End Using
 End Sub

 ''' <summary>
 ''' Get the name for the cache file for this module ID
 ''' </summary>
 ''' <param name="ModuleId">Module ID</param>
 ''' <returns>Filename to be used to store the feed.</returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Function CacheFeedname(ByVal ModuleId As Integer) As String
  Return "Feed_" & ModuleId.ToString & ".resources"
 End Function

 ''' <summary>
 ''' Gets a string regardless of the input.
 ''' </summary>
 ''' <param name="Value">An object that we try to convert to string.</param>
 ''' <returns>String value of object</returns>
 ''' <remarks>This method will prevent conversion errors when dealing with NULLs.</remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Function GetAString(ByVal Value As Object) As String
  If Value Is Nothing Then
   Return ""
  Else
   If Value Is DBNull.Value Then
    Return ""
   Else
    Return CType(Value, String)
   End If
  End If
 End Function

#Region " Extension Methods "
 <System.Runtime.CompilerServices.Extension()>
 Public Function ToSyndicationString(feed As SyndicationFeed) As String
  Return feed.ToString("RSS20")
 End Function

 <System.Runtime.CompilerServices.Extension()>
 Public Function ToString(feed As SyndicationFeed, format As String) As String
  Dim res As New StringBuilder
  Using writer As XmlWriter = XmlWriter.Create(res)
   Select Case format.ToUpper
    Case "ATOM", "ATOM10"
     feed.SaveAsAtom10(writer)
    Case Else
     feed.SaveAsRss20(writer)
   End Select
  End Using
  Return res.ToString
 End Function

 <System.Runtime.CompilerServices.Extension()>
 Public Function ToXml(feed As SyndicationFeed) As XmlDocument
  Return feed.ToXml("RSS20")
 End Function

 <System.Runtime.CompilerServices.Extension()>
 Public Function ToXml(feed As SyndicationFeed, format As String) As XmlDocument
  Dim res As New XmlDocument
  Using writer As XmlWriter = res.CreateNavigator.AppendChild
   Select Case format.ToUpper
    Case "ATOM10"
     feed.SaveAsAtom10(writer)
    Case Else
     feed.SaveAsRss20(writer)
   End Select
  End Using
  Return res
 End Function

#End Region

#Region " Reading Values "
 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As Hashtable, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Integer)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Long)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Long)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As String)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Boolean)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
    Select Case ValueTable.Item(ValueName).ToLower
     Case "on", "yes"
      Variable = True
     Case Else
      Variable = False
    End Select
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As Date)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByRef ValueTable As NameValueCollection, ByVal ValueName As String, ByRef Variable As TimeSpan)
  If Not ValueTable.Item(ValueName) Is Nothing Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As Integer)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Integer)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As String)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), String)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As Boolean)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Boolean)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As Date)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = CType(ValueTable.Item(ValueName), Date)
   Catch ex As Exception
   End Try
  End If
 End Sub

 <System.Runtime.CompilerServices.Extension()>
 Public Sub ReadValue(ByVal ValueTable As Dictionary(Of String, String), ByVal ValueName As String, ByRef Variable As TimeSpan)
  If ValueTable.ContainsKey(ValueName) Then
   Try
    Variable = TimeSpan.Parse(CType(ValueTable.Item(ValueName), String))
   Catch ex As Exception
   End Try
  End If
 End Sub
#End Region

End Module

