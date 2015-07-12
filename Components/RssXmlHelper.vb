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

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Xml.XPath
Imports System.Xml.Serialization
Imports DotNetNuke.Modules.News.Services.PreProcessing

''' <summary>
''' Helper class
''' </summary>
''' <history>
''' 	[pdonker]	01/03/2008  Caching directory no longer retrieved from web.config but passed as a parameter
''' 	[pdonker]	01/03/2008  Distinction between Rss 0.91 and 2.0
''' </history>
Public NotInheritable Class RssXmlHelper
 Private Sub New()
 End Sub
 Private Const TimeZoneCacheKey As String = "DateTimeParser"

 ''' <summary>
 ''' Parse is able to parse RFC2822/RFC822 formatted dates.
 ''' It has a fallback mechanism: if the string does not match,
 ''' the normal DateTime.Parse() function is called.
 ''' 
 ''' Copyright of RssBandit.org
 ''' Author - t_rendelmann
 ''' </summary>
 ''' <param name="dateTime">Date Time to parse</param>
 ''' <returns>DateTime instance</returns>
 ''' <exception cref="FormatException">On format errors parsing the DateTime</exception>
 Public Shared Function Parse(ByVal dateTime As String) As DateTime
  Dim rfc2822 As New Regex("\s*(?:(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun)\s*,\s*)?(\d{1,2})\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(\d{2,})\s+(\d{2})\s*:\s*(\d{2})\s*(?::\s*(\d{2}))?\s+([+\-]\d{4}|UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|PDT|[A-IK-Z])", RegexOptions.Compiled)
  Dim months As New ArrayList(New String() {"ZeroIndex", "Jan", "Feb", "Mar", "Apr", "May", _
   "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", _
   "Dec"})

  If dateTime Is Nothing Then
   Return System.DateTime.Now.ToUniversalTime()
  End If

  If dateTime.Trim().Length = 0 Then
   Return System.DateTime.Now.ToUniversalTime()
  End If

  Dim m As Match = rfc2822.Match(dateTime)
  If m.Success Then
   Try
    Dim dd As Integer = Int32.Parse(m.Groups(1).Value, CultureInfo.InvariantCulture)
    Dim mth As Integer = months.IndexOf(m.Groups(2).Value)
    Dim yy As Integer = Int32.Parse(m.Groups(3).Value, CultureInfo.InvariantCulture)
    '  following year completion is compliant with RFC 2822.
    yy = CInt((IIf(yy < 50, 2000 + yy, (IIf(yy < 1000, 1900 + yy, yy)))))
    Dim hh As Integer = Int32.Parse(m.Groups(4).Value, CultureInfo.InvariantCulture)
    Dim mm As Integer = Int32.Parse(m.Groups(5).Value, CultureInfo.InvariantCulture)
    Dim ss As Integer = Int32.Parse(m.Groups(6).Value, CultureInfo.InvariantCulture)
    Dim zone As String = m.Groups(7).Value

    Dim xd As New DateTime(yy, mth, dd, hh, mm, ss)
    Return xd.AddHours(RFCTimeZoneToGMTBias(zone) * -1)
   Catch e As FormatException
    Throw New FormatException("Error parsing date", e)
   End Try
  Else
   ' fall-back, if regex does not match:
   Return System.DateTime.Parse(dateTime, CultureInfo.InvariantCulture)
  End If
 End Function

 ''' <summary>
 ''' Converts a DateTime to a valid RFC 2822/822 string
 ''' </summary>
 ''' <param name="dt">The DateTime to convert, recognizes Zulu/GMT</param>
 ''' <returns>Returns the local time in RFC format with the time offset properly appended</returns>
 Public Shared Function ToRfc822(ByVal dt As DateTime) As String
  Dim timeZone As String

  If dt.Kind = DateTimeKind.Utc Then
   timeZone = "Z"
  Else
   Dim offset As TimeSpan = System.TimeZone.CurrentTimeZone.GetUtcOffset(dt)

   If offset.Ticks < 0 Then
    offset = -offset
    timeZone = "-"
   Else
    timeZone = "+"
   End If

   timeZone += offset.Hours.ToString(CultureInfo.InvariantCulture).PadLeft(2, "0"c)
   timeZone += offset.Minutes.ToString(CultureInfo.InvariantCulture).PadLeft(2, "0"c)
  End If

  Return dt.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone, CultureInfo.InvariantCulture)
 End Function

 ''' <summary>
 ''' Does the XSL transform.
 ''' </summary>
 ''' <param name="inputXml">The input XML.</param>
 ''' <param name="xslResource">The XSL resource.</param>
 ''' <returns></returns>
 Public Shared Function DoXslTransform(ByVal inputXml As String, ByVal xslResource As Stream) As String
  ' TODO Make the culture an argument
  Using outputWriter As New StringWriter(System.Threading.Thread.CurrentThread.CurrentUICulture)
   Using stringReader As New StringReader(inputXml)
    Dim xpathDoc As New XPathDocument(stringReader)
    Dim params As New XsltArgumentList
    params.AddParam("Now", "", DateTime.Now.ToString("u"))
    Dim transform As New XslCompiledTransform()
    Using styleSheetReader As XmlReader = XmlTextReader.Create(xslResource)
     transform.Load(styleSheetReader)
     transform.Transform(xpathDoc, params, outputWriter)
    End Using
   End Using

   Return outputWriter.ToString()
  End Using
 End Function

 ''' <summary>
 ''' Does the XSL transform.
 ''' </summary>
 ''' <param name="inputXml">The input XML.</param>
 ''' <param name="xslResource">The url to the XSL resource. This is either (1) a type, (2) a mapped path or (3) a url.</param>
 ''' <returns></returns>
 Public Shared Function DoXslTransform(ByVal inputXml As String, ByVal xslResource As String) As String

  Dim res As String = ""
  If Regex.Match(xslResource, "^[a-zA-Z0-9\.]*,\s*[a-zA-Z0-9\.]*$").Success Then
   Dim proc As IPreProcessor = CType(DotNetNuke.Framework.Reflection.CreateObject(xslResource, xslResource), IPreProcessor)
   res = proc.Process(inputXml)
  Else
   ' TODO Make the culture an argument
   Using outputWriter As New StringWriter(System.Threading.Thread.CurrentThread.CurrentUICulture)
    Using stringReader As New StringReader(inputXml)
     Dim xpathDoc As New XPathDocument(stringReader)
     Dim params As New XsltArgumentList
     params.AddParam("Now", "", DateTime.Now.ToString("u"))
     Dim transform As New XslCompiledTransform()
     Using styleSheetReader As XmlReader = XmlTextReader.Create(xslResource)
      transform.Load(styleSheetReader)
      transform.Transform(xpathDoc, params, outputWriter)
     End Using
    End Using
    res = outputWriter.ToString()
   End Using
  End If
  Return res

 End Function

 ''' <summary>
 ''' Changes Time zone based on GMT
 ''' 
 ''' Copyright of RssBandit.org
 ''' Author - t_rendelmann
 ''' </summary>
 ''' <param name="zone">The zone.</param>
 ''' <returns>RFCTimeZoneToGMTBias</returns>
 Private Shared Function RFCTimeZoneToGMTBias(ByVal zone As String) As Double
  Dim timeZones As Dictionary(Of String, Integer) = Nothing

  If HttpContext.Current IsNot Nothing Then
   timeZones = TryCast(HttpContext.Current.Cache(TimeZoneCacheKey), Dictionary(Of String, Integer))
  End If

  If timeZones Is Nothing Then
   timeZones = New Dictionary(Of String, Integer)()
   timeZones.Add("GMT", 0)
   timeZones.Add("UT", 0)
   timeZones.Add("EST", -5 * 60)
   timeZones.Add("EDT", -4 * 60)
   timeZones.Add("CST", -6 * 60)
   timeZones.Add("CDT", -5 * 60)
   timeZones.Add("MST", -7 * 60)
   timeZones.Add("MDT", -6 * 60)
   timeZones.Add("PST", -8 * 60)
   timeZones.Add("PDT", -7 * 60)
   timeZones.Add("Z", 0)
   timeZones.Add("A", -1 * 60)
   timeZones.Add("B", -2 * 60)
   timeZones.Add("C", -3 * 60)
   timeZones.Add("D", -4 * 60)
   timeZones.Add("E", -5 * 60)
   timeZones.Add("F", -6 * 60)
   timeZones.Add("G", -7 * 60)
   timeZones.Add("H", -8 * 60)
   timeZones.Add("I", -9 * 60)
   timeZones.Add("K", -10 * 60)
   timeZones.Add("L", -11 * 60)
   timeZones.Add("M", -12 * 60)
   timeZones.Add("N", 1 * 60)
   timeZones.Add("O", 2 * 60)
   timeZones.Add("P", 3 * 60)
   timeZones.Add("Q", 4 * 60)
   timeZones.Add("R", 3 * 60)
   timeZones.Add("S", 6 * 60)
   timeZones.Add("T", 3 * 60)
   timeZones.Add("U", 8 * 60)
   timeZones.Add("V", 3 * 60)
   timeZones.Add("W", 10 * 60)
   timeZones.Add("X", 3 * 60)
   timeZones.Add("Y", 12 * 60)

   If HttpContext.Current IsNot Nothing Then
    HttpContext.Current.Cache.Insert(TimeZoneCacheKey, timeZones)
   End If
  End If

  If zone.IndexOfAny(New Char() {"+"c, "-"c}) = 0 Then
   ' +hhmm format
   Dim sign As Integer = CInt((IIf(zone(0) = "-"c, -1, 1)))
   Dim s As String = zone.Substring(1).TrimEnd()
   Dim hh As Integer = Math.Min(23, Int32.Parse(s.Substring(0, 2), CultureInfo.InvariantCulture))
   Dim mm As Integer = Math.Min(59, Int32.Parse(s.Substring(2, 2), CultureInfo.InvariantCulture))
   Return sign * (hh + (mm / 60))
  Else
   ' named format
   Dim s As String = zone.ToUpper(CultureInfo.InvariantCulture).Trim()
   For Each key As String In timeZones.Keys
    If key.Equals(s) Then
     Return timeZones(key) / 60
    End If
   Next
  End If

  Return 0
 End Function

 Private Shared Function GetStreamFromResource(ByVal resourceFileName As String) As Stream
  Dim assembly As Assembly = assembly.GetExecutingAssembly()
  If assembly IsNot Nothing Then
   Return assembly.GetManifestResourceStream(resourceFileName)
  End If
  Return Nothing
 End Function
End Class
