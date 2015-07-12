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
Imports DotNetNuke

Namespace Data

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' The DataProvider Class Is an abstract class that provides the DataLayer
  ''' for the News Module.
  ''' </summary>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public MustInherit Class DataProvider

#Region " Shared/Static Methods "

    ' singleton reference to the instantiated object 
    Private Shared objProvider As DataProvider = Nothing

    ' constructor
    Shared Sub New()
      CreateProvider()
    End Sub

    ' dynamically create provider
    Private Shared Sub CreateProvider()
      objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.News.Data", ""), DataProvider)
    End Sub

    ' return the provider
    Public Shared Shadows Function Instance() As DataProvider
      Return objProvider
    End Function

#End Region

#Region " Abstract methods "
    Public MustOverride Function GetFeed(ByVal FeedID As Integer, ByVal ModuleId As Integer) As IDataReader
    Public MustOverride Function GetExpiredFeeds(ByVal ModuleId As Integer, ByVal RetryTimes As Integer, ByVal RetryTimeOut As Integer) As IDataReader
    Public MustOverride Function GetFeedsByModule(ByVal ModuleId As Integer) As IDataReader
    Public MustOverride Function AddFeed(ByVal Cache As String, ByVal CacheTime As Integer, ByVal FailedRetrieveTimes As Integer, ByVal FeedUrl As String, ByVal LastRetrieve As Date, ByVal LastRetrieveTry As Date, ByVal ModuleID As Integer, ByVal OverrideTransform As String, ByVal ParsedFeedtype As String, ByVal Password As String, ByVal User As String) As Integer
    Public MustOverride Sub UpdateFeed(ByVal FeedID As Integer, ByVal Cache As String, ByVal CacheTime As Integer, ByVal FailedRetrieveTimes As Integer, ByVal FeedUrl As String, ByVal LastRetrieve As Date, ByVal LastRetrieveTry As Date, ByVal ModuleID As Integer, ByVal OverrideTransform As String, ByVal ParsedFeedtype As String, ByVal Password As String, ByVal User As String)
    Public MustOverride Sub DeleteFeed(ByVal FeedID As Integer)
    Public MustOverride Sub ClearFeedsCache(ByVal ModuleId As Integer)
    Public MustOverride Sub SetFeedCacheTime(ByVal FeedID As Integer, CacheTime As Integer)
    Public MustOverride Sub FeedRetrieveFail(ByVal FeedID As Integer, ByVal ModuleId As Integer, ByVal LastRetrieveTry As Date)
    Public MustOverride Sub FeedRetrieveSuccess(ByVal FeedID As Integer, ByVal ModuleId As Integer, ByVal LastRetrieve As Date, ByVal Cache As String, ByVal ParsedFeedtype As String)
    Public MustOverride Function GetUpdatableModules(ByVal PortalId As Integer) As IDataReader
#End Region

  End Class

End Namespace
