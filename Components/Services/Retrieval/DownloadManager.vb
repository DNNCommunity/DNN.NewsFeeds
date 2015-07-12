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

Imports System.IO
Imports System.Collections.Generic
Imports System.Xml
Imports System.Globalization
Imports System.Net

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Modules.News.Entities.Feeds

Namespace Services.Retrieval
 ''' <summary>
 ''' Class to do the downloading and caching of feeds. 
 ''' </summary>
 ''' <remarks>
 ''' This class is an edited version of the Download manager from the RssToolkit.
 ''' Caching to SQL has been added in favor of caching on disk.
 ''' Autentication for a feed added.
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Class DownloadManager
  Private Shared _downloadManager As New DownloadManager()
  Private ReadOnly _cache As Dictionary(Of String, CacheInfo)

#Region " Constructors "
  Private Sub New()
   _cache = New Dictionary(Of String, CacheInfo)()
  End Sub
#End Region

#Region " Public Shared Methods "
  Public Shared Function GetFeed(ByVal Feed As FeedInfo) As Stream
   Return _downloadManager.GetFeedDom(Feed).Data
  End Function
#End Region

#Region " Private Methods "
  Private Shared Function GetTtlFromString(ByVal ttlString As String, ByVal ModuleId As Integer) As Integer
   If Not String.IsNullOrEmpty(ttlString) Then
    Dim ttlMinutes As Integer
    If Integer.TryParse(ttlString, ttlMinutes) Then
     If ttlMinutes >= 0 Then
      Return ttlMinutes
     End If
    End If
   End If
   Dim settings As New ModuleSettings(ModuleId)
   Return settings.DefaultCacheTime
  End Function

  Private Function GetFeedDom(ByVal Feed As FeedInfo) As CacheInfo
   Dim dom As CacheInfo = Nothing

   SyncLock _cache
    If _cache.TryGetValue(Feed.FeedUrl, dom) Then
     If dom IsNot Nothing AndAlso DateTime.UtcNow > dom.Expiry Then
      _cache.Remove(Feed.FeedUrl)
      dom = Nothing
     End If
    End If
   End SyncLock

   If Not CacheReadable(dom) Then
    dom = DownLoadFeedDom(Feed)

    SyncLock _cache
     _cache(Feed.FeedUrl) = dom
    End SyncLock
   End If

   Return dom
  End Function

  Private Shared Function CacheReadable(ByVal dom As CacheInfo) As Boolean
   Return (dom IsNot Nothing AndAlso dom.Data IsNot Nothing AndAlso dom.Data.CanRead)
  End Function

  Private Function DownLoadFeedDom(ByVal Feed As FeedInfo) As CacheInfo

   Dim dom As CacheInfo = TryLoadFromDataCache(Feed)

   If CacheReadable(dom) Then
    Return dom
   End If

   Dim doc As New XmlDocument()

   Dim wr As HttpWebRequest = CType(WebRequest.Create(Feed.FeedUrl), HttpWebRequest)
   Dim Password As String = Feed.Password
   If Not String.IsNullOrEmpty(Password) Then
    Dim encKey As String = Common.GetEncryptionKey
    If Not String.IsNullOrEmpty(encKey) Then
     Dim ps As New DotNetNuke.Security.PortalSecurity
     Password = ps.Decrypt(encKey, Feed.Password)
    End If
    ' Set correct authentication method
    Dim auth As String = "basic"
    Select Case auth
     Case "ntml"
      Dim UserAccount As String = Mid(Feed.User, InStr(Feed.User, "\") + 1)
      Dim DomainName As String = ""
      If InStr(Feed.User, "\") <> 0 Then
       DomainName = Left(Feed.User, InStr(Feed.User, "\") - 1)
      End If
      If UserAccount <> "" Then
       wr.Credentials = New NetworkCredential(UserAccount, Password, DomainName)
      End If
     Case "digest"
      Dim cc As New CredentialCache
      cc.Add(New Uri(Feed.FeedUrl), "Digest", New NetworkCredential(Feed.User, Password))
      wr.Credentials = cc
     Case Else
      Dim cc As New CredentialCache
      cc.Add(New Uri(Feed.FeedUrl), "Basic", New NetworkCredential(Feed.User, Password))
      wr.Credentials = cc
    End Select
   End If
   ' set proxy server
   If Not String.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.ProxyServer) Then
    wr.Proxy = New WebProxy(DotNetNuke.Entities.Host.Host.ProxyServer, DotNetNuke.Entities.Host.Host.ProxyPort)
    ' set the credentials for an authenticated proxy
    If Not String.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.ProxyUsername) Then
     wr.Proxy.Credentials = New NetworkCredential(DotNetNuke.Entities.Host.Host.ProxyUsername, DotNetNuke.Entities.Host.Host.ProxyPassword)
    End If
    'Else ' uncomment this to debug using Fiddler
    ' wr.Proxy = New WebProxy("127.0.0.1", 8888)
   End If
   wr.Accept = "*/*"
   wr.UserAgent = "Mozilla/4.0"
   wr.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate")
   wr.AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate

   ' set the HTTP properties
   wr.Timeout = 10000       ' 10 seconds

   ' read the response
   Dim resp As WebResponse = wr.GetResponse()
   Dim stream As Stream = resp.GetResponseStream()

   ' load XML document
   Using reader As New XmlTextReader(stream)
    reader.XmlResolver = Nothing
    doc.Load(reader)
   End Using

   ' preprocessing?
   If Feed.OverrideTransform <> "" Then '
    doc.LoadXml(RssXmlHelper.DoXslTransform(doc.OuterXml, Feed.OverrideTransform))
   End If

   Dim ttlString As String = Nothing

   If doc.SelectSingleNode("/rss/channel/ttl") IsNot Nothing Then
    ttlString = doc.SelectSingleNode("/rss/channel/ttl").Value
   End If

   If Feed.CacheTime < 0 Then
    Dim ttlMinutes As Integer = GetTtlFromString(ttlString, Feed.ModuleID)
    Feed.CacheTime = ttlMinutes
    FeedController.SetFeedCacheTime(Feed.FeedID, ttlMinutes)
   End If

   Return TrySaveToDataCache(doc, Feed)

  End Function

  ''' <summary>
  ''' Check to see if it should be retrieved from SQL and if so get it.
  ''' </summary>
  ''' <param name="Feed">FeedInfo of the feed to retrieve</param>
  ''' <returns>The cached feed or nothing</returns>
  ''' <remarks></remarks>
  Private Function TryLoadFromDataCache(ByVal Feed As FeedInfo) As CacheInfo

   If String.IsNullOrEmpty(Feed.Cache) Then
    Return Nothing
   End If

   Dim found As CacheInfo = Nothing
   Dim localExpiryFromFeedInfo As DateTime = Feed.LastRetrieve.AddMinutes(Feed.CacheTime)

   If localExpiryFromFeedInfo > DateTime.Now Then
    Dim feedDoc As New XmlDocument
    feedDoc.LoadXml(Feed.Cache)
    found = New CacheInfo(feedDoc, localExpiryFromFeedInfo, Feed)
   End If
   Return found
  End Function

  ''' <summary>
  ''' Save feed to the SQL cache
  ''' </summary>
  ''' <param name="doc"></param>
  ''' <param name="Feed"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Private Function TrySaveToDataCache(ByVal doc As XmlDocument, ByVal Feed As FeedInfo) As CacheInfo

   FeedController.FeedRetrieveSuccess(Feed.FeedID, Feed.ModuleID, DateTime.Now, doc.OuterXml, Feed.ParsedFeedtype)

   Dim utcExpiry As DateTime = DateTime.UtcNow.AddMinutes(Feed.CacheTime)
   Return New CacheInfo(doc, utcExpiry, Feed)

  End Function
#End Region

#Region " CacheInfo "
  Private Class CacheInfo
   Implements IDisposable
   Private ReadOnly m_data As Stream
   Private ReadOnly m_expiry As DateTime
   Private ReadOnly m_feed As FeedInfo

   Public Sub New(ByVal doc As XmlDocument, ByVal utcExpiry As DateTime, ByVal Feed As FeedInfo)
    Dim documentStream As New MemoryStream()
    doc.Save(documentStream)
    documentStream.Flush()
    documentStream.Position = 0
    Me.m_data = documentStream
    Me.m_expiry = utcExpiry
    Me.m_feed = Feed
   End Sub

   ''' <summary>
   ''' Gets the expiration time in UTC.
   ''' </summary>
   ''' <value>The expiry.</value>
   Public ReadOnly Property Expiry() As DateTime
    Get
     Return m_expiry
    End Get
   End Property

   ''' <summary>
   ''' Gets the data stream
   ''' </summary>
   ''' <value>The data.</value>
   Public ReadOnly Property Data() As Stream
    Get
     Return m_data
    End Get
   End Property

   ''' <summary>
   ''' Gets the filename 
   ''' </summary>
   Public ReadOnly Property Feed() As FeedInfo
    Get
     Return m_feed
    End Get
   End Property

   ''' <summary>
   ''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
   ''' </summary>
   Public Sub Dispose() Implements IDisposable.Dispose
    m_data.Dispose()
   End Sub
  End Class
#End Region

 End Class
End Namespace
