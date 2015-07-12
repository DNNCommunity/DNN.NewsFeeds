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

Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Xml
Imports System.ServiceModel.Syndication
Imports System.Text
Imports System.Linq
Imports DotNetNuke.Modules.News.Entities.Feeds

Namespace Services.Retrieval
 ''' <summary>
 ''' Main aggregator class.
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' 	[pdonker]	11/23/2012  Removed RssToolkit
 ''' </history>
 Public Class FeedAggregator
  Inherits SyndicationFeed

#Region " Private Members "
  Private _settings As ModuleSettings
  Private _moduleTitle As String = "New Feed"
#End Region

#Region " Properties "
  Public Property FeedErrors As New List(Of FeedError)
  Public Property Feeds As New Dictionary(Of Integer, SyndicationFeed)
#End Region

#Region " Constructors "
  Public Sub New(ByVal tempDir As String, ByVal NewsSettings As ModuleSettings, ByVal ModuleTitle As String)
   MyBase.New()
   MyBase.Title = New TextSyndicationContent(ModuleTitle)
   _settings = NewsSettings
   _moduleTitle = ModuleTitle
  End Sub
#End Region

#Region " Public Methods "
  Public Overloads Sub Load(ByVal Feeds As List(Of FeedInfo))

   If Feeds.Count < 1 Then Exit Sub
   LoadRssFeeds(Feeds)
   MergeRss()

  End Sub

  Public Shared Function Create(feed As FeedInfo) As SyndicationFeed

   Using stream As IO.Stream = DownloadManager.GetFeed(feed)
    Using reader As XmlReader = XmlReader.Create(stream)
     Dim formatter As SyndicationFeedFormatter = SyndicationFormatterFactory.CreateFeedFormatter(reader)
     If formatter IsNot Nothing Then
      formatter.ReadFrom(reader)
      Return formatter.Feed
     End If
    End Using
   End Using
   Return Nothing

  End Function

  Public Shared Function Create(feed As String) As SyndicationFeed
   Using sr As New IO.StringReader(feed)
    Using x As XmlReader = XmlReader.Create(sr)
     Dim formatter As SyndicationFeedFormatter = SyndicationFormatterFactory.CreateFeedFormatter(x)
     If formatter IsNot Nothing Then
      formatter.ReadFrom(x)
      Return formatter.Feed
     End If
    End Using
   End Using
   Return Nothing
  End Function
#End Region

#Region " Private Methods "
  Private Sub LoadRssFeeds(ByVal Feeds As List(Of FeedInfo))
   For Each feed As FeedInfo In Feeds
    GetRssFeedsByFeedInfo(feed)
   Next
  End Sub

  Private Sub GetRssFeedsByFeedInfo(feed As FeedInfo)

   Dim xmlString As String = String.Empty

   ' check to see if we should skip this one
   If feed.FailedRetrieveTimes >= _settings.RetryTimes Then
    If feed.LastRetrieveTry.AddMinutes(_settings.RetryTimeOut) > Now Then
     Exit Sub
    End If
   End If

   Try

    Feeds.Add(feed.FeedID, FeedAggregator.Create(feed))

   Catch ex As Exception
    _FeedErrors.Add(New FeedError(feed, String.Format("Download error ({0})", ex.Message), FeedErrorType.Retrieval))
   End Try

  End Sub

  Private Sub MergeRss()

   Dim items As New List(Of SyndicationItem)
   For Each feed As SyndicationFeed In Feeds.Values
    For Each item As SyndicationItem In feed.Items
     items.Add(item)
     item.ElementExtensions.Add(New SyndicationElementExtension("pubDateParsed", glbNewsNs, item.PublishDate.ToString("yyyy-MM-dd HH:mm K")))
     item.ElementExtensions.Add(New SyndicationElementExtension("pubDateParsedLocalTime", glbNewsNs, item.PublishDate.ToLocalTime.ToString("yyyy-MM-dd HH:mm K")))
    Next
   Next

   Me.Items = items.OrderByDescending(Function(u) u.PublishDate)

  End Sub
#End Region

#Region " FeedError "
  Public Structure FeedError
   Public Feed As FeedInfo
   Public ErrorMessage As String
   Public ErrorType As FeedErrorType
   Public Sub New(ByVal feed As FeedInfo, ByVal errorMessage As String, ByVal errorType As FeedErrorType)
    Me.Feed = feed
    Me.ErrorMessage = errorMessage
    Me.ErrorType = errorType
   End Sub
  End Structure

  Public Enum FeedErrorType
   Retrieval
   Aggregation
   Parsing
  End Enum
#End Region

 End Class
End Namespace
