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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Utilities
Imports System.Xml.Serialization
Imports DotNetNuke.Services.Tokens
Imports System.Xml.Schema
Imports System.Xml
Imports DotNetNuke.Entities.Portals
Imports System.Web

Namespace Entities.Feeds
 ''' <summary>
 ''' Core feed object class
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Class FeedInfo
  Implements IHydratable
  Implements IXmlSerializable

  ' local property declarations
  Dim _FeedID As Integer
  Dim _Cache As String
  Dim _CacheTime As Integer
  Dim _FailedRetrieveTimes As Integer
  Dim _FeedUrl As String
  Dim _LastRetrieve As Date
  Dim _LastRetrieveTry As Date
  Dim _ModuleID As Integer
  Dim _ParsedFeedtype As String
  Dim _Password As String
  Dim _User As String
  Dim _overrideTransform As String

  Private _index As Integer = -1

#Region " Constructors "
  Public Sub New()
  End Sub

  Public Sub New(ByVal FeedID As Integer, ByVal Cache As String, ByVal CacheTime As Integer, ByVal FailedRetrieveTimes As Integer, ByVal FeedUrl As String, ByVal LastRetrieve As Date, ByVal LastRetrieveTry As Date, ByVal ModuleID As Integer, ByVal ParsedFeedtype As String, ByVal Password As String, ByVal User As String)
   Me.Cache = Cache
   Me.CacheTime = CacheTime
   Me.FailedRetrieveTimes = FailedRetrieveTimes
   Me.FeedID = FeedID
   Me.FeedUrl = FeedUrl
   Me.LastRetrieve = LastRetrieve
   Me.LastRetrieveTry = LastRetrieveTry
   Me.ModuleID = ModuleID
   Me.ParsedFeedtype = ParsedFeedtype
   Me.Password = Password
   Me.User = User
  End Sub
#End Region

#Region " Public Properties "
  ''' <summary>
  ''' Feed ID
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property FeedID() As Integer
   Get
    Return _FeedID
   End Get
   Set(ByVal Value As Integer)
    _FeedID = Value
   End Set
  End Property

  ''' <summary>
  ''' Cached feed
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' Note this is normally the transformed feed (to Rss 2.0, the internal standard) that is cached as a string.
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property Cache() As String
   Get
    Return _Cache
   End Get
   Set(ByVal Value As String)
    _Cache = Value
   End Set
  End Property

  ''' <summary>
  ''' The time when the feed was written to the cache
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property CacheTime() As Integer
   Get
    Return _CacheTime
   End Get
   Set(ByVal Value As Integer)
    _CacheTime = Value
   End Set
  End Property

  ''' <summary>
  ''' Nr of times the retrieval failed
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property FailedRetrieveTimes() As Integer
   Get
    Return _FailedRetrieveTimes
   End Get
   Set(ByVal Value As Integer)
    _FailedRetrieveTimes = Value
   End Set
  End Property

  ''' <summary>
  ''' Url of the feed
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property FeedUrl() As String
   Get
    Return _FeedUrl
   End Get
   Set(ByVal Value As String)
    _FeedUrl = Value
   End Set
  End Property

  ''' <summary>
  ''' Last time a successful retrieve was made
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Property LastRetrieve() As Date
   Get
    Return _LastRetrieve
   End Get
   Set(ByVal Value As Date)
    _LastRetrieve = Value
   End Set
  End Property

  ''' <summary>
  ''' Last time a retrieval was attempted
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property LastRetrieveTry() As Date
   Get
    Return _LastRetrieveTry
   End Get
   Set(ByVal Value As Date)
    _LastRetrieveTry = Value
   End Set
  End Property

  ''' <summary>
  ''' Module ID
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property ModuleID() As Integer
   Get
    Return _ModuleID
   End Get
   Set(ByVal Value As Integer)
    _ModuleID = Value
   End Set
  End Property

  ''' <summary>
  ''' Original type of the feed (e.g. Rss 2.0, ATOM, etc). Used for debugging.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property ParsedFeedtype() As String
   Get
    Return _ParsedFeedtype
   End Get
   Set(ByVal Value As String)
    _ParsedFeedtype = Value
   End Set
  End Property

  ''' <summary>
  ''' Password if authentication is needed.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property Password() As String
   Get
    Return _Password
   End Get
   Set(ByVal Value As String)
    _Password = Value
   End Set
  End Property

  ''' <summary>
  ''' User account if authentication is needed.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property User() As String
   Get
    Return _User
   End Get
   Set(ByVal Value As String)
    _User = Value
   End Set
  End Property

  ''' <summary>
  ''' Index in aggregator. For internal use only.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' This property is used to let the downloader keep track of the feeds it is retrieving.
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property Index() As Integer
   Get
    Return _index
   End Get
   Set(ByVal value As Integer)
    _index = value
   End Set
  End Property

  ''' <summary>
  ''' If specified then the incoming feed will be transformed using this transformation
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' This property is used to allow fine tuning of (1) defective feeds, (2) feeds with additional info that one wants to preserve
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	12/15/2008  Created
  ''' </history>
  Public Property OverrideTransform() As String
   Get
    Return _overrideTransform
   End Get
   Set(ByVal value As String)
    _overrideTransform = value
   End Set
  End Property
#End Region

#Region " Public Methods "

  ''' <summary>
  ''' This function returns the relative path to a file if a file was selected in the url control. Otherwise just the OverrideTransform value.
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
  ''' <history>
  ''' 	[pdonker]	24/03/2010  Created
  ''' </history>
  Public Function GetOverrideTransformPath() As String

   Dim strLink As String = OverrideTransform.ToLowerInvariant
   If strLink.StartsWith("fileid=") Then
    Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
    Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo = objModules.GetModule(ModuleID)
    If objModule IsNot Nothing Then
     Dim objFile As DotNetNuke.Services.FileSystem.IFileInfo = DotNetNuke.Services.FileSystem.FileManager.Instance.GetFile(Integer.Parse(UrlUtils.GetParameterValue(strLink)))
     If Not objFile Is Nothing Then
      strLink = objFile.PhysicalPath
     End If
    End If
   End If
   Return strLink

  End Function
#End Region

#Region " IHydratable Implementation "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Fill hydrates the object from a Datareader
  ''' </summary>
  ''' <remarks>The Fill method is used by the CBO method to hydrtae the object
  ''' rather than using the more expensive Refection  methods.</remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub Fill(ByVal dr As IDataReader) Implements IHydratable.Fill

   FeedID = Convert.ToInt32(Null.SetNull(dr.Item("FeedID"), FeedID))
   ModuleID = Convert.ToInt32(Null.SetNull(dr.Item("ModuleID"), ModuleID))
   FeedUrl = Convert.ToString(Null.SetNull(dr.Item("FeedUrl"), FeedUrl))
   User = Convert.ToString(Null.SetNull(dr.Item("User"), User))
   Password = Convert.ToString(Null.SetNull(dr.Item("Password"), Password))
   Cache = Convert.ToString(Null.SetNull(dr.Item("Cache"), Cache))
   CacheTime = Convert.ToInt32(Null.SetNull(dr.Item("CacheTime"), CacheTime))
   LastRetrieve = Convert.ToDateTime(Null.SetNull(dr.Item("LastRetrieve"), LastRetrieve))
   LastRetrieveTry = Convert.ToDateTime(Null.SetNull(dr.Item("LastRetrieveTry"), LastRetrieveTry))
   FailedRetrieveTimes = Convert.ToInt32(Null.SetNull(dr.Item("FailedRetrieveTimes"), FailedRetrieveTimes))
   ParsedFeedtype = Convert.ToString(Null.SetNull(dr.Item("ParsedFeedtype"), ParsedFeedtype))
   OverrideTransform = Convert.ToString(Null.SetNull(dr.Item("OverrideTransform"), OverrideTransform))

  End Sub
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' Gets and sets the Key ID
  ''' </summary>
  ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
  ''' as the key property when creating a Dictionary</remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Property KeyID() As Integer Implements IHydratable.KeyID
   Get
    Return FeedID
   End Get
   Set(ByVal value As Integer)
    FeedID = value
   End Set
  End Property
#End Region

#Region " IXmlSerializable Implementation "

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' GetSchema returns the XmlSchema for this class
  ''' </summary>
  ''' <remarks>GetSchema is implemented as a stub method as it is not required</remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
   Return Nothing
  End Function

  Private Function readElement(ByVal reader As XmlReader, ByVal ElementName As String) As String
   If (Not reader.NodeType = XmlNodeType.Element) OrElse reader.Name <> ElementName Then
    reader.ReadToFollowing(ElementName)
   End If
   If reader.NodeType = XmlNodeType.Element Then
    Return reader.ReadElementContentAsString
   Else
    Return ""
   End If
  End Function

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' ReadXml fills the object (de-serializes it) from the XmlReader passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="reader">The XmlReader that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
   Try

    ' when importing, ItemId will always be null.nullinteger (or -1)
    Int32.TryParse(readElement(reader, "FeedID"), FeedID)
    Int32.TryParse(readElement(reader, "ModuleID"), ModuleID)
    FeedUrl = readElement(reader, "FeedUrl")
    User = readElement(reader, "User")
    Password = readElement(reader, "Password")
    Int32.TryParse(readElement(reader, "CacheTime"), CacheTime)
    OverrideTransform = readElement(reader, "OverrideTransform")

   Catch ex As Exception
    ' log exception as DNN import routine does not do that
    DotNetNuke.Services.Exceptions.LogException(ex)
    ' re-raise exception to make sure import routine displays a visible error to the user
    Throw New Exception("An error occured during import of a NewsFeed Definition", ex)
   End Try

  End Sub

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
  ''' </summary>
  ''' <remarks></remarks>
  ''' <param name="writer">The XmlWriter that contains the xml for the object</param>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
   writer.WriteStartElement("Feed")
   writer.WriteElementString("FeedID", FeedID.ToString())
   writer.WriteElementString("ModuleID", ModuleID.ToString())
   writer.WriteElementString("FeedUrl", FeedUrl)
   writer.WriteElementString("User", User)
   writer.WriteElementString("Password", Password)
   writer.WriteElementString("CacheTime", CacheTime.ToString)
   writer.WriteElementString("OverrideTransform", OverrideTransform)
   writer.WriteEndElement()
  End Sub

#End Region

 End Class

End Namespace
