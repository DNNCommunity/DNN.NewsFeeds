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

Imports System.Text.RegularExpressions

Namespace Services.PreProcessing
 ''' <summary>
 ''' This class can be used in preprocessing to change the description verbose part) of a Twitter feed to expand all urls
 ''' To use specify the transformation of the feed to be 'DotNetNuke.Modules.News.PreProcessing.TwitterPreProcessor, DotNetNuke.Modules.News'
 ''' </summary>
 ''' <remarks></remarks>
 Public Class TwitterPreProcessor
  Implements IPreProcessor

  Public Function Process(ByVal xml As String) As String Implements IPreProcessor.Process
   Dim res As String = Regex.Replace(xml, "<description>([^<]*)</description>", AddressOf ProcessDescription)
   res = Regex.Replace(res, "<title>([^<]*)</title>", AddressOf ProcessTitle)
   Return res
  End Function

  Private Function ProcessTitle(ByVal m As Match) As String
   Dim res As String = "<title>"
   Dim m2 As Match = Regex.Match(m.Groups(1).Value, "(^[^:]*): (.*)$")
   Dim line As String = m2.Groups(2).Value
   res &= line
   res &= "</title>"
   Return res
  End Function

  Private Function ProcessDescription(ByVal m As Match) As String
   Dim res As String = "<description>"
   Dim m2 As Match = Regex.Match(m.Groups(1).Value, "(^[^:]*): (.*)$")
   Dim author As String = m2.Groups(1).Value
   Dim line As String = m2.Groups(2).Value
   res &= Regex.Replace(line, "(?i)(https?://[^\s]*)|(#[a-zA-Z0-9_-]*)|(@[a-zA-Z0-9_-]*)(?-i)", AddressOf ReplaceLink)
   res &= "</description>"
   res &= "<author>" & author & "</author>"
   Return res
  End Function

  Private Function ReplaceLink(ByVal m As Match) As String
   Dim res As String = ""
   If m.Groups(1).Success Then ' http link
    res = "&lt;a href=""" & m.Groups(1).Value & """&gt;" & m.Groups(1).Value & "&lt;/a&gt;"
   ElseIf m.Groups(2).Success Then ' #value
    res = "&lt;a href=""http://twitter.com/search?q=%23" & Mid(m.Groups(2).Value, 2) & """&gt;" & m.Groups(2).Value & "&lt;/a&gt;"
   ElseIf m.Groups(3).Success Then ' @person
    res = "&lt;a href=""http://twitter.com/" & Mid(m.Groups(3).Value, 2) & """&gt;" & m.Groups(3).Value & "&lt;/a&gt;"
   End If
   Return res
  End Function

 End Class
End Namespace
