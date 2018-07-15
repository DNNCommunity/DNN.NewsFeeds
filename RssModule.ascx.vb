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
Imports System.Net
Imports System.IO
Imports System.Text

Imports DotNetNuke
Imports DotNetNuke.UI
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports System.Web.UI
Imports DotNetNuke.Modules.News.Entities.Feeds
Imports DotNetNuke.Modules.News.Services.Retrieval
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions

''' <summary>
''' This control shows the Aggregated feed.
''' </summary>
''' <remarks>
''' The module needs a refresh if one of the feeds has not been updated recently. 
''' If the module needs a refresh it will see if it can do an Ajax rendering meaning 
''' it will display itself on screen and load the contents afterwards.
''' </remarks>
''' <history>
''' 	[pdonker]	03/01/2008  Created
''' </history>
Partial Public Class RssModule
 Inherits Modulebase
 Implements IActionable

#Region " Controls "
 Protected WithEvents feedOutput As Controls.Feed
#End Region

#Region " Private Members "
 Private _script As String = ""
 Private _loadAfterPage As Boolean = False
 Private _needsRefresh As Boolean = False
#End Region

#Region " Event Handlers "
 ''' <summary>
 ''' Initialize page and determine the course of action for this control. Add Ajax JS code if necessary to do post loading.
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' 	[pdonker]	09/23/2008  Include a manual setting to override use of Ajax
 ''' 	                      Changed Javascript that is emitted with the module
 ''' 	[pdonker]	24/03/2010  Added security to prevent non-edit users to clear cache (20323)
 ''' </history>
 Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

  If Not Me.IsPostBack And Me.Request.Params("ClearCache") IsNot Nothing And DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(Me.ModuleConfiguration) Then
   FeedController.ClearFeedsCache(ModuleId)
   If IO.File.Exists(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(ModuleId)) Then
    IO.File.Delete(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(ModuleId))
   End If
  End If

  _needsRefresh = ((Not Settings.BackgroundDownload) Or (Not Me.IsPostBack And Me.Request.Params("ClearCache") IsNot Nothing)) AndAlso FeedController.HasExpiredFeeds(ModuleId, Settings.RetryTimes, Settings.RetryTimeOut)
  _loadAfterPage = (DotNetNuke.Framework.AJAX.IsInstalled And Me.ModuleConfiguration.CacheTime = 0 And _needsRefresh)
  _loadAfterPage = _loadAfterPage And Settings.UseAjax
  rssLine.Visible = ModuleConfiguration.DisplaySyndicate

  If _loadAfterPage Then
   DotNetNuke.Framework.AJAX.RegisterScriptManager()
   DotNetNuke.Framework.AJAX.WrapUpdatePanelControl(pnlRss, True)
  End If

  If Not Me.IsPostBack Then
   If _loadAfterPage Then
    Dim script As New StringBuilder
    script.AppendLine("<script language=""javascript"">")
    script.AppendLine("newsMods[newsMods.length]='" & btnUpdate.ClientID & "';")
    script.AppendLine("</script>")
    _script = script.ToString
    EmitGeneralScriptBlock()
   End If
  End If

 End Sub

 ''' <summary>
 ''' If multiple News Modules exist on the page this will only appear once. Javascript has been moved into its own file.
 ''' </summary>
 ''' <remarks>Added to solve loading issues with multiple news modules on a single page all using Ajax</remarks>
 ''' <history>
 ''' 	[pdonker]	09/23/2008  Created
 ''' </history>
 Private Sub EmitGeneralScriptBlock()
  Dim script As New StringBuilder
  script.AppendLine("<script language=""javascript"" src=""" & ResolveUrl("~/DesktopModules/News/loadlater.js") & """>")
  script.AppendLine("</script>")
  Utilities.ClientAPI.RegisterClientScriptBlock(Page, "DNNNewsLoad", script.ToString)
  Utilities.ClientAPI.RegisterStartUpScript(Page, "DNNNewsLoadInit", "<script language=""javascript"">updateNewsModsLoad();</script>")
 End Sub

 ''' -----------------------------------------------------------------------------
 ''' <summary>
 ''' Page_Load runs when the control is loaded
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' 	[pdonker]	24/03/2010  Automatically revert back to normal url if clearing cache
 ''' </history>
 ''' -----------------------------------------------------------------------------
 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Try

   If Not Me.IsPostBack Then
    If Not _loadAfterPage Then
     DataBind()
    End If
   End If

   If Me.Request.Params("ClearCache") IsNot Nothing Then
    Response.Redirect(DotNetNuke.Common.NavigateURL(Me.TabId), False)
   End If

  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try


 End Sub

 ''' <summary>
 ''' This is not a button the user sees, but a hidden button used to call back to.
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks></remarks>
 Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click

  DataBind()

 End Sub
#End Region

#Region " Optional Interfaces "

 Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements IActionable.ModuleActions
  Get
   Dim Actions As New ModuleActionCollection
   Actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl(), False, Security.SecurityAccessLevel.Edit, True, False)
   Actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.ClearCache, LocalResourceFile), ModuleActionType.ClearCache, "", "action_refresh.gif", DotNetNuke.Common.NavigateURL(TabId, "", "ClearCache=1"), False, Security.SecurityAccessLevel.Edit, True, False)
   Actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.SyndicateModule, LocalResourceFile), ModuleActionType.SyndicateModule, "", "rss.gif", ResolveUrl("~/DesktopModules/News/API/News.rss") & "?TabId=" & TabId.ToString & "&ModuleId=" & ModuleId.ToString, False, DotNetNuke.Security.SecurityAccessLevel.View, True, False)
   Return Actions
  End Get
 End Property

#End Region

#Region " Overrides "

 ''' <summary>
 ''' Get the aggregated feed and perform transformation.
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Overrides Sub DataBind()

  Dim xslsrc As String = Settings.XslUrl
  If xslsrc <> "" Then
   If xslsrc.ToLower.IndexOf("://") <> -1 Or xslsrc.StartsWith("/") Or xslsrc.StartsWith("~") Then
    ' it's already in the correct format
   Else
    xslsrc = PortalSettings.HomeDirectory & xslsrc
   End If
  Else    ' default
   xslsrc = Common.GlobalTransformationPath & "/Default.xsl"
  End If

  If Not _needsRefresh Then

   If Not IO.File.Exists(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(ModuleId)) Then Exit Sub
   Try
    Dim cachedDoc As New XmlDocument
    cachedDoc.Load(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(ModuleId))
    With feedOutput
     .PortalId = PortalId
     .NewsSettings = Settings
     .IsEditable = IsEditable
     .XmlDoc = cachedDoc
     .XslDoc = xslsrc
    End With
    Exit Sub
   Catch ex As Exception
    ' continue below and reconstruct the feeds
   End Try

  End If

  If Not IO.Directory.Exists(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\") Then
   IO.Directory.CreateDirectory(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\")
  End If

  Dim aggregator As New FeedAggregator(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\", Me.Settings, Me.ModuleConfiguration.ModuleTitle)
  aggregator.Load(FeedController.GetFeedsByModule(ModuleId))
  Common.SaveResultFeed(aggregator.ToXml, ModuleId, Me.PortalSettings.HomeDirectoryMapPath & "\Cache\")

  If aggregator.FeedErrors.Count > 0 Then
   Dim sbError As New StringBuilder
   For Each fdError As FeedAggregator.FeedError In aggregator.FeedErrors
    sbError.Append(String.Format(Localization.GetString("FeedError", Me.LocalResourceFile), fdError.Feed.FeedUrl, fdError.ErrorMessage))
    sbError.Append("<br />")
   Next
   DotNetNuke.Services.Exceptions.ProcessModuleLoadException("Feed errors:<br />" & sbError.ToString, Me, New Exception("Feed Errors"), Me.IsEditable)
  End If

  With feedOutput
   .PortalId = PortalId
   .NewsSettings = Settings
   .IsEditable = IsEditable
   .XmlDoc = aggregator.ToXml
   .XslDoc = xslsrc
  End With

  For Each fdError As FeedAggregator.FeedError In aggregator.FeedErrors
   If fdError.ErrorType = FeedAggregator.FeedErrorType.Retrieval Then
    FeedController.FeedRetrieveFail(fdError.Feed.FeedID, ModuleId, Now)
   End If
  Next

  If aggregator.FeedErrors.Count > 0 And Me.IsEditable Then
   Dim sbError As New StringBuilder
   sbError.Append("<div class=""NormalRed"">")
   For Each fdError As FeedAggregator.FeedError In aggregator.FeedErrors
    sbError.Append("<p>")
    sbError.Append(String.Format(Localization.GetString("FeedError", Me.LocalResourceFile), fdError.Feed.FeedUrl, fdError.ErrorMessage))
    sbError.Append("</p>")
   Next
   sbError.Append("</div>")
   plhErrors.Controls.Add(New LiteralControl(sbError.ToString))
  End If

 End Sub

 ''' <summary>
 ''' Overridden Render to add script to stream.
 ''' </summary>
 ''' <param name="output"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Protected Overrides Sub Render(ByVal output As System.Web.UI.HtmlTextWriter)
  MyBase.Render(output)
  output.Write(_script)
 End Sub
#End Region

End Class
