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
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework.Providers

Namespace Data

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' The SqlDataProvider Class is an SQL Server implementation of the DataProvider Abstract
  ''' class that provides the DataLayer for the HTML Module.
  ''' </summary>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Class SqlDataProvider
    Inherits DataProvider


#Region " Private Members "

    Private Const ProviderType As String = "data"
    Private Const ModuleQualifier As String = "News_"

    Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
    Private _connectionString As String
    Private _providerPath As String
    Private _objectQualifier As String
    Private _databaseOwner As String

#End Region

#Region " Constructors "

    Public Sub New()

      ' Read the configuration specific information for this provider
   Dim objProvider As Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)

   'Get Connection string from web.config
      _connectionString = Config.GetConnectionString()

   If _connectionString = "" Then
    ' Use connection string specified in provider
    _connectionString = objProvider.Attributes("connectionString")
   End If

      _providerPath = objProvider.Attributes("providerPath")

      _objectQualifier = objProvider.Attributes("objectQualifier")
      If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
        _objectQualifier += "_"
      End If

      _databaseOwner = objProvider.Attributes("databaseOwner")
      If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
        _databaseOwner += "."
      End If

    End Sub

#End Region

#Region " Properties "

    Public ReadOnly Property ConnectionString() As String
      Get
        Return _connectionString
      End Get
    End Property

    Public ReadOnly Property ProviderPath() As String
      Get
        Return _providerPath
      End Get
    End Property

    Public ReadOnly Property ObjectQualifier() As String
      Get
        Return _objectQualifier
      End Get
    End Property

    Public ReadOnly Property DatabaseOwner() As String
      Get
        Return _databaseOwner
      End Get
    End Property

#End Region

#Region " Public Methods "

    Private Function GetNull(ByVal Field As Object) As Object
      Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
    End Function

#End Region

#Region " Feeds Methods "
    Public Overrides Function GetFeed(ByVal FeedID As Integer, ByVal ModuleId As Integer) As IDataReader
      Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetFeed", FeedID, ModuleId), IDataReader)
    End Function

    Public Overrides Function GetExpiredFeeds(ByVal ModuleId As Integer, ByVal RetryTimes As Integer, ByVal RetryTimeOut As Integer) As System.Data.IDataReader
      Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetExpiredFeeds", ModuleId, RetryTimes, RetryTimeOut), IDataReader)
    End Function

    Public Overrides Function GetFeedsByModule(ByVal ModuleId As Integer) As IDataReader
      Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetFeedsByModule", ModuleId), IDataReader)
    End Function

    Public Overrides Function AddFeed(ByVal Cache As String, ByVal CacheTime As Integer, ByVal FailedRetrieveTimes As Integer, ByVal FeedUrl As String, ByVal LastRetrieve As Date, ByVal LastRetrieveTry As Date, ByVal ModuleID As Integer, ByVal OverrideTransform As String, ByVal ParsedFeedtype As String, ByVal Password As String, ByVal User As String) As Integer
      Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "AddFeed", GetNull(Cache), CacheTime, GetNull(FailedRetrieveTimes), FeedUrl, GetNull(LastRetrieve), GetNull(LastRetrieveTry), ModuleID, GetNull(OverrideTransform), GetNull(ParsedFeedtype), GetNull(Password), GetNull(User)), Integer)
    End Function

    Public Overrides Sub UpdateFeed(ByVal FeedID As Integer, ByVal Cache As String, ByVal CacheTime As Integer, ByVal FailedRetrieveTimes As Integer, ByVal FeedUrl As String, ByVal LastRetrieve As Date, ByVal LastRetrieveTry As Date, ByVal ModuleID As Integer, ByVal OverrideTransform As String, ByVal ParsedFeedtype As String, ByVal Password As String, ByVal User As String)
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "UpdateFeed", FeedID, GetNull(Cache), CacheTime, GetNull(FailedRetrieveTimes), FeedUrl, GetNull(LastRetrieve), GetNull(LastRetrieveTry), ModuleID, GetNull(OverrideTransform), GetNull(ParsedFeedtype), GetNull(Password), GetNull(User))
    End Sub

    Public Overrides Sub DeleteFeed(ByVal FeedID As Integer)
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "DeleteFeed", FeedID)
    End Sub

    Public Overrides Sub ClearFeedsCache(ByVal ModuleId As Integer)
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "ClearFeedsCache", ModuleId)
    End Sub

    Public Overrides Sub SetFeedCacheTime(ByVal FeedID As Integer, CacheTime As Integer)
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "SetFeedCacheTime", FeedID, CacheTime)
    End Sub

    Public Overrides Sub FeedRetrieveFail(ByVal FeedID As Integer, ByVal ModuleId As Integer, ByVal LastRetrieveTry As Date)
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "FeedRetrieveFail", FeedID, ModuleId, LastRetrieveTry)
    End Sub

    Public Overrides Sub FeedRetrieveSuccess(ByVal FeedID As Integer, ByVal ModuleId As Integer, ByVal LastRetrieve As Date, ByVal Cache As String, ByVal ParsedFeedtype As String)
      SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "FeedRetrieveSuccess", FeedID, ModuleId, LastRetrieve, Cache, ParsedFeedtype)
    End Sub
#End Region

#Region " Other Methods "
    Public Overrides Function GetUpdatableModules(ByVal PortalId As Integer) As System.Data.IDataReader
      Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ModuleQualifier & "GetUpdatableModules", PortalId), IDataReader)
    End Function
#End Region

  End Class

End Namespace
