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
Imports System.ServiceModel.Syndication

Namespace Services.Retrieval

 ''' <summary>
 ''' Creates formatters for RSS 2.0 and Atom 1.0 according to the input content.
 ''' </summary>
 ''' <remarks>
 ''' Created by Daniel Cazzulino.
 ''' </remarks>
 Public NotInheritable Class SyndicationFormatterFactory
  Private Sub New()
  End Sub
  Shared settings As XmlReaderSettings

  Shared Sub New()
   ' Makes the processing faster for the readers we create.
   settings = New XmlReaderSettings()
   settings.IgnoreComments = True
   settings.IgnoreProcessingInstructions = True
   settings.IgnoreWhitespace = True
   settings.CheckCharacters = True
   settings.CloseInput = True
  End Sub

  ''' <summary>
  ''' Creates a <see cref="SyndicationFeedFormatter"/> according to the
  ''' input format.
  ''' </summary>
  ''' <param name="uriString">Feed location</param>
  ''' <exception cref="NotSupportedException">The input does not contain a valid RSS 2.0 or Atom 1.0 feed.</exception>
  Public Shared Function CreateFeedFormatter(uriString As String) As SyndicationFeedFormatter
   Using reader As XmlReader = XmlReader.Create(uriString, settings)
    Return CreateFeedFormatter(reader)
   End Using
  End Function

  ''' <summary>
  ''' Creates a <see cref="SyndicationFeedFormatter"/> according to the
  ''' input format.
  ''' </summary>
  ''' <param name="uri">Feed location</param>
  ''' <exception cref="NotSupportedException">The input does not contain a valid RSS 2.0 or Atom 1.0 feed.</exception>
  Public Shared Function CreateFeedFormatter(uri As Uri) As SyndicationFeedFormatter
   Using reader As XmlReader = XmlReader.Create(uri.ToString(), settings)
    Return CreateFeedFormatter(reader)
   End Using
  End Function

  ''' <summary>
  ''' Creates a <see cref="SyndicationFeedFormatter"/> according to the
  ''' input format.
  ''' </summary>
  ''' <param name="reader">Feed source</param>
  ''' <exception cref="NotSupportedException">The input does not contain a valid RSS 2.0 or Atom 1.0 feed.</exception>
  Public Shared Function CreateFeedFormatter(reader As XmlReader) As SyndicationFeedFormatter
   If reader.ReadState = ReadState.Initial Then
    reader.MoveToContent()
   End If

   Dim rss As New Rss20FeedFormatter()
   If rss.CanRead(reader) Then
    Return rss
   End If

   Dim atom As New Atom10FeedFormatter()
   If atom.CanRead(reader) Then
    Return atom
   End If

   Throw New NotSupportedException("Invalid feed root element: " & Convert.ToString(reader.Name))
  End Function

  ''' <summary>
  ''' Creates a <see cref="SyndicationItemFormatter"/> according to the
  ''' input format.
  ''' </summary>
  ''' <param name="uriString">Item location</param>
  ''' <exception cref="NotSupportedException">The input does not contain a valid RSS 2.0 or Atom 1.0 item.</exception>
  Public Shared Function CreateItemFormatter(uriString As String) As SyndicationItemFormatter
   Using reader As XmlReader = XmlReader.Create(uriString, settings)
    Return CreateItemFormatter(reader)
   End Using
  End Function

  ''' <summary>
  ''' Creates a <see cref="SyndicationItemFormatter"/> according to the
  ''' input format.
  ''' </summary>
  ''' <param name="uri">Item location</param>
  ''' <exception cref="NotSupportedException">The input does not contain a valid RSS 2.0 or Atom 1.0 item.</exception>
  Public Shared Function CreateItemFormatter(uri As Uri) As SyndicationItemFormatter
   Using reader As XmlReader = XmlReader.Create(uri.ToString(), settings)
    Return CreateItemFormatter(reader)
   End Using
  End Function

  ''' <summary>
  ''' Creates a <see cref="SyndicationItemFormatter"/> according to the
  ''' input format.
  ''' </summary>
  ''' <param name="reader">Item source</param>
  ''' <exception cref="NotSupportedException">The input does not contain a valid RSS 2.0 or Atom 1.0 item.</exception>
  Public Shared Function CreateItemFormatter(reader As XmlReader) As SyndicationItemFormatter
   If reader.ReadState = ReadState.Initial Then
    reader.MoveToContent()
   End If

   Dim rss As New Rss20ItemFormatter()
   If rss.CanRead(reader) Then
    Return rss
   End If

   Dim atom As New Atom10ItemFormatter()
   If atom.CanRead(reader) Then
    Return atom
   End If

   Throw New NotSupportedException("Invalid item element: " & Convert.ToString(reader.Name))
  End Function
 End Class
End Namespace
