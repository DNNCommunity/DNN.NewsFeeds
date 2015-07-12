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

Imports System.Web.UI.WebControls

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Modules.News.Entities.Feeds

''' <summary>
''' Control to edit the definition of an aggregated feed.
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[pdonker]	03/01/2008  Created
''' </history>
Partial Public Class EditRss
 Inherits Modulebase

#Region " Private Members "
 Private _opml As String = ""
#End Region

#Region " Public Members "
#End Region

#Region " Controls "
 Protected WithEvents ctlXsl As UI.UserControls.UrlControl
#End Region

#Region " Event Handlers "
 ''' <summary>
 ''' Called upon Page Load
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' 	[pdonker]	24/03/2010  Changed custom transformation to full mapped path
 ''' </history>
 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
  Try

   If Not Page.IsPostBack Then
    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, DotNetNuke.Services.Localization.Localization.GetString("Delete.Confirm", Me.LocalResourceFile))
    With ddOverrideTransform
     .Items.Add(New ListItem(GetString("UseDefault.Option", Me.LocalResourceFile), ""))
     .Items.Add(New ListItem(GetString("Atom.Option", Me.LocalResourceFile), "atom"))
     .Items.Add(New ListItem(GetString("Rdf.Option", Me.LocalResourceFile), "rdf"))
     .Items.Add(New ListItem(GetString("Rss091.Option", Me.LocalResourceFile), "rss091"))
     .Items.Add(New ListItem(GetString("Twitter.Option", Me.LocalResourceFile), "DotNetNuke.Modules.News.Services.PreProcessing.TwitterPreProcessor, DotNetNuke.Modules.News"))
    End With
    If Not IO.Directory.Exists(PortalSettings.HomeDirectoryMapPath & Common.LocalTransformationPath) Then
     IO.Directory.CreateDirectory(PortalSettings.HomeDirectoryMapPath & Common.LocalTransformationPath)
    End If
    For Each f As String In IO.Directory.GetFiles(PortalSettings.HomeDirectoryMapPath & Common.LocalTransformationPath, "*.xsl?")
     Dim fName As String = Mid(f, f.LastIndexOf("\") + 2)
     ddOverrideTransform.Items.Add(New ListItem(fName & " [Portal]", PortalSettings.HomeDirectoryMapPath & Common.LocalTransformationPath & "\" & fName))
    Next

    pnlEdit.Visible = False
    BindList()
   End If

  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' <summary>
 ''' Update all settings and go back to page.
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
  Try
   If Page.IsValid Then
    Dim feedId As Integer = CInt(hidFeedId.Value)
    Dim f As FeedInfo = Nothing
    If feedId = -1 Then
     f = New FeedInfo
     With f
      .ModuleID = ModuleId
     End With
    Else
     f = FeedController.GetFeed(feedId, ModuleId)
    End If
    With f
     .CacheTime = Integer.Parse(txtCacheTime.Text)
     .FeedUrl = txtFeedUrl.Text
     Dim Password As String = txtPassword.Text
     If Not String.IsNullOrEmpty(Password) Then
      Dim encKey As String = Common.GetEncryptionKey
      If Not String.IsNullOrEmpty(encKey) Then
       Dim ps As New DotNetNuke.Security.PortalSecurity
       Password = ps.Encrypt(encKey, Password)
      End If
      .Password = Password
     Else
      .Password = ""
     End If
     .User = txtUser.Text
     If optStandard.Checked Then
      .OverrideTransform = ddOverrideTransform.Text
     ElseIf optType.Checked Then
      .OverrideTransform = txtOverrideTransform.Text
     Else
      .OverrideTransform = ctlXsl.Url
     End If
    End With
    If feedId = -1 Then
     FeedController.AddFeed(f)
    Else
     FeedController.UpdateFeed(f)
    End If

    CloseEdit()
   End If
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' <summary>
 ''' Cancel edits and go back to page.
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
  Try
   CloseEdit()
  Catch exc As Exception    'Module failed to load
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' <summary>
 ''' Show add new RSS feed panel
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
  showEdit(-1)
 End Sub

 ''' <summary>
 ''' Delete the selected news feed
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
  Dim feedId As Integer = CInt(hidFeedId.Value)
  FeedController.DeleteFeed(feedId)
  CloseEdit()
 End Sub

 ''' <summary>
 ''' Return to RSS feed view
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 '''   [tstensitzki] 07/25/2010 Force a cache refresh to reflect recent config changes
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReturn.Click
  Response.Redirect(NavigateURL(Me.TabId, "", "ClearCache=1"), False)
 End Sub

 ''' <summary>
 ''' Edit feed in list
 ''' </summary>
 ''' <param name="source"></param>
 ''' <param name="e"></param>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Private Sub dlFeeds_EditCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlFeeds.EditCommand
  dlFeeds.EditItemIndex = e.Item.ItemIndex
  Dim feedId As Integer = CInt(dlFeeds.DataKeys(e.Item.ItemIndex))
  showEdit(feedId)
 End Sub

 ''' <summary>
 ''' User decided to change type of preprocessing
 ''' </summary>
 ''' <param name="sender"></param>
 ''' <param name="e"></param>
 ''' <remarks></remarks>
 Private Sub optOverrideTransform_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles optStandard.CheckedChanged, optType.CheckedChanged, optUrl.CheckedChanged
  If optUrl.Checked Then
   ddOverrideTransform.Enabled = False
   ctlXsl.Visible = True
   txtOverrideTransform.Enabled = False
  ElseIf optType.Checked Then
   ddOverrideTransform.Enabled = False
   ctlXsl.Visible = False
   txtOverrideTransform.Enabled = True
  Else 'standard
   ddOverrideTransform.Enabled = True
   ctlXsl.Visible = False
   txtOverrideTransform.Enabled = False
  End If
 End Sub
#End Region

#Region " Private Methods "
 Private Sub showEdit(ByVal FeedId As Integer)
  If FeedId = -1 Then
   txtCacheTime.Text = "-1"
   txtFeedUrl.Text = ""
   txtPassword.Text = ""
   txtUser.Text = ""
   ddOverrideTransform.ClearSelection()
   txtOverrideTransform.Text = ""
   txtOverrideTransform.Enabled = False
   ctlXsl.Visible = False
   optStandard.Checked = True
   txtCache.Text = ""
   txtCache.Enabled = False
  Else
   Dim f As FeedInfo = FeedController.GetFeed(FeedId, ModuleId)
   txtCacheTime.Text = "-1"
   txtFeedUrl.Text = f.FeedUrl
   If f.User <> "" Then
    txtUser.Text = f.User
    txtPassword.Text = GetString("PasswordEncoded", Me.LocalResourceFile)
    txtPassword.Attributes.Add("onclick", "Javascript:this.value='';")
   Else
    txtPassword.Text = ""
    txtUser.Text = ""
   End If
   ddOverrideTransform.ClearSelection()
   If f.OverrideTransform = "" Then 'no transformation
    ddOverrideTransform.Enabled = True
    ctlXsl.Visible = False
    txtOverrideTransform.Enabled = False
    optStandard.Checked = True
   ElseIf ddOverrideTransform.Items.FindByValue(f.OverrideTransform) IsNot Nothing Then ' standard
    ddOverrideTransform.Items.FindByValue(f.OverrideTransform).Selected = True
    ddOverrideTransform.Enabled = True
    ctlXsl.Visible = False
    txtOverrideTransform.Enabled = False
    txtOverrideTransform.Text = ""
    optStandard.Checked = True
   ElseIf Text.RegularExpressions.Regex.Match(f.OverrideTransform.Replace(" ", ""), "[a-zA-Z0-9\.]*,[a-zA-Z0-9\.]*").Success Then
    txtOverrideTransform.Text = f.OverrideTransform
    ddOverrideTransform.Enabled = False
    ctlXsl.Visible = False
    txtOverrideTransform.Enabled = True
    optType.Checked = True
   Else ' url
    ctlXsl.Url = f.OverrideTransform
    ddOverrideTransform.Enabled = False
    ctlXsl.Visible = True
    txtOverrideTransform.Enabled = False
    txtOverrideTransform.Text = ""
    optUrl.Checked = True
   End If
   txtCache.Text = f.Cache
   txtCache.Enabled = True
  End If
  hidFeedId.Value = FeedId.ToString
  pnlEdit.Visible = True
  pnlList.Visible = False
 End Sub

 Private Sub CloseEdit()
  pnlEdit.Visible = False
  pnlList.Visible = True
  BindList()
 End Sub

 ''' <summary>
 ''' Make the datalist with feeds.
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' 	[pdonker]	24/03/2010  DL invisible on no items
 ''' </history>
 Private Sub BindList()
  dlFeeds.DataSource = DotNetNuke.Common.ConvertDataReaderToDataTable(Data.DataProvider.Instance().GetFeedsByModule(ModuleId))
  dlFeeds.DataBind()
  If dlFeeds.Items.Count = 0 Then
   dlFeeds.Visible = False
  Else
   dlFeeds.Visible = True
  End If
 End Sub
#End Region

End Class
