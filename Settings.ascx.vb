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

Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Modules.News.Common

Partial Public Class Settings
 Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region " Private Members "
 Private _portalTransformationPath As String = ""
#End Region

#Region " Properties "
 Public ReadOnly Property PortalTransformationPath() As String
  Get
   If _portalTransformationPath = "" Then
    Dim pi As DotNetNuke.Entities.Portals.PortalInfo = (New DotNetNuke.Entities.Portals.PortalController).GetPortal(PortalId)
    _portalTransformationPath = "~/" & pi.HomeDirectory & LocalTransformationPath.Replace("\", "/")
   End If
   Return _portalTransformationPath
  End Get
 End Property
#End Region

#Region " Controls "
 Protected WithEvents ctlRSSxsl As UI.UserControls.UrlControl
#End Region

#Region " Base Method Implementations "
 ''' <summary>
 ''' Load the module's settings into the controls.
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Overrides Sub LoadSettings()
  Try
   If Not Page.IsPostBack Then

    ' Load Xsl dropdown
    If Not IO.Directory.Exists(PortalSettings.HomeDirectoryMapPath & LocalTransformationPath) Then
     IO.Directory.CreateDirectory(PortalSettings.HomeDirectoryMapPath & LocalTransformationPath)
    End If
    For Each f As String In IO.Directory.GetFiles(Server.MapPath(GlobalTransformationPath), "*.xsl?")
     Dim fName As String = Mid(f, f.LastIndexOf("\") + 2)
     ddXsl.Items.Add(New ListItem(fName & " [System]", GlobalTransformationPath & "/" & fName))
    Next
    For Each f As String In IO.Directory.GetFiles(PortalSettings.HomeDirectoryMapPath & LocalTransformationPath, "*.xsl?")
     Dim fName As String = Mid(f, f.LastIndexOf("\") + 2)
     ddXsl.Items.Add(New ListItem(fName & " [Portal]", PortalTransformationPath & "/" & fName))
    Next
    ddXsl.Items.Add(New ListItem(GetString("Other.Option", Me.LocalResourceFile), ""))

    Dim Settings As ModuleSettings = News.ModuleSettings.GetSettings(ModuleConfiguration)
    Dim xslsrc As String = Settings.XslUrl

    Try
     If xslsrc.StartsWith(GlobalTransformationPath) Or xslsrc.StartsWith(PortalTransformationPath) Then
      ddXsl.Items.FindByValue(xslsrc).Selected = True
     ElseIf xslsrc = "" Then
      ddXsl.Items.FindByValue(GlobalTransformationPath & "/Default.xsl").Selected = True
     Else
      ddXsl.Items.FindByValue("").Selected = True
     End If
    Catch ex As Exception
    End Try

    If Not DotNetNuke.Framework.AJAX.IsInstalled Then
     Settings.UseAjax = False
     chkUseAjax.Enabled = False
    End If

    With Settings
     txtDefaultCacheTime.Text = .DefaultCacheTime.ToString
     txtItemsToShow.Text = .ItemsToShow.ToString
     chkShowItemDetails.Checked = .ShowItemDetails
     chkShowItemDate.Checked = .ShowItemDate
     chkUseAjax.Checked = .UseAjax
     chkBackgroundDownload.Checked = .BackgroundDownload
     txtRetryTimes.Text = .RetryTimes.ToString
     txtRetryTimeOut.Text = .RetryTimeOut.ToString
     Try
      ddTarget.Items.FindByValue(.Target).Selected = True
     Catch ex As Exception
     End Try
    End With
    If ddXsl.SelectedValue = "" Then
     ctlRSSxsl.Visible = True
     If xslsrc <> "" Then
      ctlRSSxsl.Url = xslsrc
     Else
      ctlRSSxsl.UrlType = "U"
     End If
    Else
     ctlRSSxsl.Visible = False
    End If
   End If
  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub

 ''' <summary>
 ''' Write settings back to the Module settings class.
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Overrides Sub UpdateSettings()
  Try

   Dim Settings As ModuleSettings = News.ModuleSettings.GetSettings(ModuleConfiguration)
   With Settings
    .DefaultCacheTime = Integer.Parse(txtDefaultCacheTime.Text)
    If ddXsl.SelectedValue = "" Then
     .XslUrl = ctlRSSxsl.Url
    Else
     .XslUrl = ddXsl.SelectedValue
    End If
    .ItemsToShow = Integer.Parse(txtItemsToShow.Text)
    .ShowItemDetails = chkShowItemDetails.Checked
    .ShowItemDate = chkShowItemDate.Checked
    .UseAjax = chkUseAjax.Checked
    .BackgroundDownload = chkBackgroundDownload.Checked
    .RetryTimes = Integer.Parse(txtRetryTimes.Text)
    .RetryTimeOut = Integer.Parse(txtRetryTimeOut.Text)
    .Target = ddTarget.SelectedValue
    .SaveSettings(ModuleConfiguration)
   End With

  Catch exc As Exception
   ProcessModuleLoadException(Me, exc)
  End Try
 End Sub
#End Region

#Region " Event Handlers "
 Private Sub ddXsl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddXsl.SelectedIndexChanged
  ctlRSSxsl.Visible = (ddXsl.SelectedValue = "")
 End Sub
#End Region

End Class
