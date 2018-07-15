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
Imports DotNetNuke.Common.Utilities.XmlUtils
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Web.Api
Imports System.Text
Imports System.Collections.Generic
Imports System.IO
Imports System.Net.Http
Imports System.Web.Http
Imports System.Net
Imports System.ServiceModel.Syndication
Imports DotNetNuke.Modules.News.Entities.Feeds
Imports DotNetNuke.Modules.News.Services.Retrieval

''' <summary>
''' Module Controller class
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[pdonker]	03/01/2008  Created
''' 	[pdonker]	24/11/2012  Added Web API
''' </history>
Public Class NewsController
 Inherits DnnApiController
 Implements IServiceRouteMapper
 Implements IPortable
 Implements IUpgradeable

#Region " IPortable Implementation "
 Public Function ExportModule(ByVal ModuleID As Integer) As String Implements IPortable.ExportModule
  Dim sb As New StringBuilder()
  Dim settings As New XmlWriterSettings()
  settings.ConformanceLevel = ConformanceLevel.Fragment
  settings.OmitXmlDeclaration = True

  sb.Append("<News><Settings>")
  Dim mi As ModuleInfo = (New ModuleController).GetModule(ModuleID)
  For Each name As String In mi.ModuleSettings.Keys
   sb.Append("<Setting Name=""" + name + """>")
   sb.Append("<Value>" + XMLEncode(mi.ModuleSettings(name).ToString) + "</Value>")
   sb.Append("</Setting>")
  Next
  sb.Append("</Settings>")

  Dim arrFeeds As List(Of FeedInfo) = FeedController.GetFeedsByModule(ModuleID)
  If arrFeeds.Count <> 0 Then
   Dim writer As XmlWriter = XmlWriter.Create(sb, settings)

   'Write start of Feeds Node
   writer.WriteStartElement("Feeds")

   For Each Feed As FeedInfo In arrFeeds
    Feed.WriteXml(writer)
   Next

   'Write end of Feeds Node
   writer.WriteEndElement()

   writer.Close()
  End If

  sb.Append("</News>")
  Return sb.ToString()

 End Function

 Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements IPortable.ImportModule

  If Version.StartsWith("03") Then

   ' old version import
   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   Dim xmlDoc As New XmlDocument
   xmlDoc.LoadXml(Content)
   Dim Feed As New FeedInfo
   For Each node As XmlNode In xmlDoc.SelectNodes("news/settings/setting")
    Dim xmlname As XmlAttribute = node.Attributes.ItemOf("name")
    Dim xmlvalue As XmlNode = node.SelectSingleNode("value")
    Select Case xmlname.Value
     Case "xmlsrc"
      Feed.FeedUrl = DotNetNuke.Common.ImportUrl(ModuleID, xmlvalue.InnerText)
     Case "xslsrc"
      objModules.UpdateModuleSetting(ModuleID, "XslUrl", DotNetNuke.Common.ImportUrl(ModuleID, xmlvalue.InnerText))
     Case "account"
      Feed.User = xmlvalue.InnerText
     Case "password"
      Feed.Password = ""
      If xmlvalue.InnerText.Trim <> "" Then
       Dim encKey As String = Common.GetEncryptionKey
       If Not String.IsNullOrEmpty(encKey) Then
        Dim ps As New DotNetNuke.Security.PortalSecurity
        Feed.Password = ps.Encrypt(encKey, xmlvalue.InnerText.Trim)
       End If
      End If
    End Select
   Next
   If Feed.FeedUrl.Trim <> "" Then
    FeedController.AddFeed(Feed)
   End If

  Else
   ' this is the current import function
   Dim xmlDoc As New XmlDocument
   xmlDoc.LoadXml(Content)

   Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
   For Each xSetting As XmlNode In xmlDoc.SelectNodes("News/Settings/Setting")
    Dim settingName As String = xSetting.Attributes("Name").InnerText
    Dim settingValue As String = xSetting.SelectSingleNode("Value").InnerText
    objModules.UpdateModuleSetting(ModuleID, settingName, settingValue)
   Next

   Using reader As XmlReader = XmlReader.Create(New StringReader(Content))

    'reader.ReadToNextSibling("Feeds")
    If reader.Read() Then
     reader.ReadStartElement("News")
     'reader.ReadStartElement("Settings")
     reader.ReadToFollowing("Feeds")
     reader.Read()
     If reader.ReadState <> ReadState.EndOfFile And reader.NodeType <> XmlNodeType.None And reader.LocalName <> "" Then
      Do
       reader.ReadStartElement("Feed")
       Dim Feed As New FeedInfo

       'Deserialize Feed
       Feed.ReadXml(reader)

       'initialize values of the new Feed to this module and this user
       Feed.FeedID = Null.NullInteger
       Feed.ModuleID = ModuleID

       'Save Feed
       FeedController.AddFeed(Feed)
      Loop While reader.ReadToNextSibling("Feed")
     End If
    End If

    reader.Close()
   End Using
  End If

 End Sub
#End Region

#Region " IUpgradeable Implementation "
 Public Function UpgradeModule(ByVal Version As String) As String Implements IUpgradeable.UpgradeModule
  Dim strResults As String = ""
  Try
   Select Case Version
    Case "04.00.00"
     strResults += "Upgrading for v 04.00.00" & vbCrLf
     Dim encKey As String = Common.GetEncryptionKey
     strResults += "Encryption Key: " & encKey & vbCrLf
     If Not String.IsNullOrEmpty(encKey) Then
      Dim ps As New DotNetNuke.Security.PortalSecurity
      Dim feeds As List(Of FeedInfo) = FeedController.GetFeedsByModule(-10)
      strResults += "Upgrading " & feeds.Count.ToString & " feeds" & vbCrLf
      For Each feed As FeedInfo In feeds
       If Not String.IsNullOrEmpty(feed.Password) Then
        feed.Password = ps.Encrypt(encKey, feed.Password)
        FeedController.UpdateFeed(feed)
       End If
      Next
     End If
   End Select
  Catch ex As Exception
   strResults += "Error: " & ex.Message & vbCrLf
   Try
    DotNetNuke.Services.Exceptions.LogException(ex)
   Catch
    ' ignore
   End Try
  End Try
  Return strResults
 End Function
#End Region

#Region " Web API "
 Public Sub RegisterRoutes(mapRouteManager As Web.Api.IMapRoute) Implements Web.Api.IServiceRouteMapper.RegisterRoutes
  mapRouteManager.MapHttpRoute("News", "Default", "News", New With {.controller = "News", .action = "GetModuleFeed"}, Nothing, New String() {"DotNetNuke.Modules.News"})
  mapRouteManager.MapHttpRoute("News", "Specific", "News.{method}", New With {.controller = "News", .action = "GetModuleFeed"}, New With {.method = "(rss|atom)"}, New String() {"DotNetNuke.Modules.News"})
 End Sub

 <HttpGet()>
 <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
 Public Function GetModuleFeed() As HttpResponseMessage
  Return GetModuleFeed("")
 End Function

 <HttpGet()>
 <DnnModuleAuthorize(AccessLevel:=DotNetNuke.Security.SecurityAccessLevel.View)>
 Public Function GetModuleFeed(method As String) As HttpResponseMessage
  Dim res As String = ""
  If Not IO.File.Exists(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(ActiveModule.ModuleID)) Then
   res = ""
  Else
   Using rdr As New IO.StreamReader(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(ActiveModule.ModuleID))
    res = rdr.ReadToEnd
   End Using
  End If
  Select Case method.ToLower
   Case "atom"
    Dim feed As SyndicationFeed = FeedAggregator.Create(res)
    res = feed.ToString("ATOM")
    Return Request.CreateResponse(HttpStatusCode.OK, res)
   Case Else
    Return Request.CreateResponse(HttpStatusCode.OK, res)
  End Select
 End Function
#End Region

End Class


