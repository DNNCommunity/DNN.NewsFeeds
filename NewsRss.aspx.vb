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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Portals

Public Class NewsRss
 Inherits DotNetNuke.Framework.PageBase

#Region " Private Members "
 Private _authorized As Boolean = False
 Private _modSettings As ModuleSettings
 Private _module As ModuleInfo
 Private _moduleId As Integer = -1
 Private _portalId As Integer = -1
 Private _tabId As Integer = -1
#End Region

#Region " Properties "
 Public Property Settings() As ModuleSettings
  Get
   If _modSettings Is Nothing Then
    _modSettings = ModuleSettings.GetSettings(_module)
   End If
   Return _modSettings
  End Get
  Set(ByVal value As ModuleSettings)
   _modSettings = value
  End Set
 End Property
#End Region

#Region " Page Events "
 Private Sub NewsRss_Init(sender As Object, e As System.EventArgs) Handles Me.Init

  Common.ReadValue(Me.Request.Params, "PortalId", _portalId)
  Common.ReadValue(Me.Request.Params, "TabId", _tabId)
  Common.ReadValue(Me.Request.Params, "ModuleId", _moduleId)

 End Sub

 Private Sub NewsRss_Load(sender As Object, e As System.EventArgs) Handles Me.Load

  Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
  _module = objModules.GetModule(_moduleId)
  If DotNetNuke.Security.Permissions.ModulePermissionController.CanViewModule(_module) Then
   _authorized = True
  End If

 End Sub
#End Region

#Region " Render "
 Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

  If Not _authorized Then
   Me.Response.StatusCode = 401
   Exit Sub
  End If

  If Not IO.File.Exists(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(_moduleId)) Then Exit Sub

  Me.Response.Clear()
  Me.Response.ContentEncoding = System.Text.Encoding.UTF8
  Me.Response.ContentType = "text/xml"

  Using rdr As New IO.StreamReader(Me.PortalSettings.HomeDirectoryMapPath & "\Cache\" & Common.CacheFeedname(_moduleId))
   Dim contents As String = rdr.ReadToEnd
   writer.Write(contents)
  End Using

  writer.Flush()

 End Sub
#End Region

End Class