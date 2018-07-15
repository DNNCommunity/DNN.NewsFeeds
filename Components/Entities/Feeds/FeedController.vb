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
Imports System.Data
Imports DotNetNuke.Modules.News.Data
Imports System.Collections.Generic

Namespace Entities.Feeds
 ''' <summary>
 ''' Main Controller class
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Class FeedController

  ''' <summary>
  ''' Get a feed object
  ''' </summary>
  ''' <param name="FeedID">Feed ID</param>
  ''' <param name="ModuleId">Module ID</param>
  ''' <returns>Feed</returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Function GetFeed(ByVal FeedID As Integer, ByVal ModuleId As Integer) As FeedInfo

   Return DotNetNuke.Common.Utilities.CBO.FillObject(Of FeedInfo)(DataProvider.Instance().GetFeed(FeedID, ModuleId))

  End Function

  ''' <summary>
  ''' Get expired feeds for this module
  ''' </summary>
  ''' <param name="ModuleId">Module ID</param>
  ''' <param name="RetryTimes">Nr of retries allowed to get a feed before giving up</param>
  ''' <param name="RetryTimeOut">Retry timeout</param>
  ''' <returns>List of feeds that should be retrieved</returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Function GetExpiredFeeds(ByVal ModuleId As Integer, ByVal RetryTimes As Integer, ByVal RetryTimeOut As Integer) As List(Of FeedInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of FeedInfo)(DataProvider.Instance().GetExpiredFeeds(ModuleId, RetryTimes, RetryTimeOut))

  End Function

  ''' <summary>
  ''' Determines whether the module needs to retrieve feeds
  ''' </summary>
  ''' <param name="ModuleId">Module ID</param>
  ''' <param name="RetryTimes">Nr of retries allowed to get a feed before giving up</param>
  ''' <param name="RetryTimeOut">Retry timeout</param>
  ''' <returns>Boolean. If true the module needs to retrieve at least one feed.</returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' 	[pdonker]	07/29/2008  Repaired open datareader
  ''' </history>
  Public Shared Function HasExpiredFeeds(ByVal ModuleId As Integer, ByVal RetryTimes As Integer, ByVal RetryTimeOut As Integer) As Boolean

   Dim res As Boolean = False
   Using ir As IDataReader = DataProvider.Instance().GetExpiredFeeds(ModuleId, RetryTimes, RetryTimeOut)
    If ir.Read Then
     res = True
    End If
   End Using
   Return res

  End Function

  ''' <summary>
  ''' Get all feeds for this module
  ''' </summary>
  ''' <param name="ModuleId">Module ID</param>
  ''' <returns>List of feeds</returns>
  ''' <remarks>
  ''' Use ModuleId -10 to retrieve all feeds (used in Upgrade script)
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Function GetFeedsByModule(ByVal ModuleId As Integer) As List(Of FeedInfo)

   Return DotNetNuke.Common.Utilities.CBO.FillCollection(Of FeedInfo)(DataProvider.Instance().GetFeedsByModule(ModuleId))

  End Function

  ''' <summary>
  ''' Add a feed to this module
  ''' </summary>
  ''' <param name="objFeed">Feed</param>
  ''' <returns>Feed ID</returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Function AddFeed(ByVal objFeed As FeedInfo) As Integer

   Return CType(DataProvider.Instance().AddFeed(objFeed.Cache, objFeed.CacheTime, objFeed.FailedRetrieveTimes, objFeed.FeedUrl, objFeed.LastRetrieve, objFeed.LastRetrieveTry, objFeed.ModuleID, objFeed.OverrideTransform, objFeed.ParsedFeedtype, objFeed.Password, objFeed.User), Integer)

  End Function

  ''' <summary>
  ''' Update a feed
  ''' </summary>
  ''' <param name="objFeed">Feed</param>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Sub UpdateFeed(ByVal objFeed As FeedInfo)

   DataProvider.Instance().UpdateFeed(objFeed.FeedID, objFeed.Cache, objFeed.CacheTime, objFeed.FailedRetrieveTimes, objFeed.FeedUrl, objFeed.LastRetrieve, objFeed.LastRetrieveTry, objFeed.ModuleID, objFeed.OverrideTransform, objFeed.ParsedFeedtype, objFeed.Password, objFeed.User)

  End Sub

  ''' <summary>
  ''' Delete a feed
  ''' </summary>
  ''' <param name="FeedID">Feed ID</param>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Sub DeleteFeed(ByVal FeedID As Integer)

   DataProvider.Instance().DeleteFeed(FeedID)

  End Sub

  ''' <summary>
  ''' Clear the cache of all module's feeds. This is used to force the module to update all feeds.
  ''' </summary>
  ''' <param name="ModuleId">Module ID</param>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Sub ClearFeedsCache(ByVal ModuleId As Integer)

   DataProvider.Instance().ClearFeedsCache(ModuleId)

  End Sub

  ''' <summary>
  ''' Sets the cache time for a feed.
  ''' </summary>
  ''' <param name="FeedID">Id of the feed</param>
  ''' <param name="CacheTime">Cache time in minutes</param>
  ''' <remarks></remarks>
  ''' <history>
  ''' 	[pdonker]	25/03/2010  Created
  ''' </history>
  Public Shared Sub SetFeedCacheTime(ByVal FeedID As Integer, CacheTime As Integer)

   DataProvider.Instance().SetFeedCacheTime(FeedID, CacheTime)

  End Sub

  ''' <summary>
  ''' Flag a retrieval failure. Used to make sure we don't continue trying a dead feed indefinitely.
  ''' </summary>
  ''' <param name="FeedID">Feed ID</param>
  ''' <param name="ModuleId">Module ID</param>
  ''' <param name="LastRetrieveTry">Time of last try</param>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Shared Sub FeedRetrieveFail(ByVal FeedID As Integer, ByVal ModuleId As Integer, ByVal LastRetrieveTry As Date)

   DataProvider.Instance().FeedRetrieveFail(FeedID, ModuleId, LastRetrieveTry)

  End Sub

  ''' <summary>
  ''' Flag a succesful retrieve.
  ''' </summary>
  ''' <param name="FeedID">Feed ID</param>
  ''' <param name="ModuleId">Module ID</param>
  ''' <param name="LastRetrieve">Time of retrieval</param>
  ''' <param name="Cache">Feed as XML to cache</param>
  ''' <param name="ParsedFeedtype">Original type of feed (e.g. Rss 2.0, ATOM, etc). Used for debugging feed translation issues.</param>
  ''' <remarks></remarks>
  Public Shared Sub FeedRetrieveSuccess(ByVal FeedID As Integer, ByVal ModuleId As Integer, ByVal LastRetrieve As Date, ByVal Cache As String, ByVal ParsedFeedtype As String)

   DataProvider.Instance().FeedRetrieveSuccess(FeedID, ModuleId, LastRetrieve, Cache, ParsedFeedtype)

  End Sub
 End Class
End Namespace
