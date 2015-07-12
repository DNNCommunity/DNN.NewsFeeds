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

Imports System.Web.UI
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Net
Imports System.IO

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports System.Web
Imports DotNetNuke.Entities.Portals

Namespace Controls
 ''' <summary>
 ''' Server control to render feed.
 ''' </summary>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Class Feed
  Inherits System.Web.UI.Control

#Region " Private Members "
  Private _isEditable As Boolean = False
  Private _xmlDoc As XmlDocument
  Private _xslDoc As String = ""
  Private _portalId As Integer = -1
  Private _newsSettings As ModuleSettings
#End Region

#Region " Properties "
  ''' <summary>
  ''' Whether we are in edit mode. If so then debug messages will be shown. 
  ''' If not then the module will render the feeds it can and ignore others.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property IsEditable() As Boolean
   Get
    Return _isEditable
   End Get
   Set(ByVal value As Boolean)
    _isEditable = value
   End Set
  End Property

  ''' <summary>
  ''' Property to pass on all settings from the module to this control.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property NewsSettings() As ModuleSettings
   Get
    Return _newsSettings
   End Get
   Set(ByVal value As ModuleSettings)
    _newsSettings = value
   End Set
  End Property

  ''' <summary>
  ''' Portal ID
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property PortalId() As Integer
   Get
    Return _portalId
   End Get
   Set(ByVal value As Integer)
    _portalId = value
   End Set
  End Property

  ''' <summary>
  ''' The feed as XML document
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property XmlDoc() As XmlDocument
   Get
    Return _xmlDoc
   End Get
   Set(ByVal value As XmlDocument)
    _xmlDoc = value
   End Set
  End Property

  ''' <summary>
  ''' Path to the transformation
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Property XslDoc() As String
   Get
    Return _xslDoc
   End Get
   Set(ByVal value As String)
    _xslDoc = value
   End Set
  End Property
#End Region

#Region " Overrides "
  ''' <summary>
  ''' Renders the control to the client
  ''' </summary>
  ''' <param name="output"></param>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Protected Overrides Sub Render(ByVal output As System.Web.UI.HtmlTextWriter)
   If XmlDoc Is Nothing Then Exit Sub

   output.WriteBeginTag("div")
   output.WriteAttribute("class", "normal")
   output.Write(HtmlTextWriter.TagRightChar)
   Try
    Dim Xslt As XslCompiledTransform = GetXslTransform()
    Dim Xml As XmlReader = New XmlNodeReader(XmlDoc)
    If Not Xslt Is Nothing And Not Xml Is Nothing Then
     Xslt.Transform(Xml, NewsSettings.GetParameterList, output)
    End If
   Catch exc As Exception    'Module failed to load
    ProcessModuleLoadException(Me, exc)
    'UI.Skins.Skin.AddModuleMessage don't work during render time
    If Me.IsEditable Then output.Write("<table class=""normal"" ><tr><td><img src=""{2}""></td><td><span style=""COLOR: red""><strong>{1}</strong></span><br/>{0}</td></tr></table>", exc.Message, exc.GetType.FullName, ResolveUrl("~/images/red-error.gif"))
   Finally
    output.WriteEndTag("div")
   End Try
  End Sub
#End Region

#Region " Private Methods "
  ''' <summary>
  ''' Method to get the transformation from the path specification.
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Protected Function GetXslTransform() As XslCompiledTransform
   If XslDoc <> "" Then
    If GetURLType(XslDoc) = DotNetNuke.Entities.Tabs.TabType.Url Then
     If XslDoc.ToLower.StartsWith("http") Then
      Return GetXSLContent(XslDoc)
     ElseIf XslDoc.StartsWith("~") Or XslDoc.StartsWith("/") Then
      Dim trans As New XslCompiledTransform
      trans.Load(Me.Context.Server.MapPath(XslDoc))
      Return trans
     ElseIf XslDoc.Contains(":\") Then
      Dim trans As New XslCompiledTransform
      trans.Load(XslDoc)
      Return trans
     End If
    Else
     Dim trans As New XslCompiledTransform
     trans.Load(GetMappedPath(XslDoc))
     Return trans
    End If
   End If
   Return Nothing
  End Function

  ''' <summary>
  ''' Get XSL if not on the own server.
  ''' </summary>
  ''' <param name="ContentURL"></param>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Public Function GetXSLContent(ByVal ContentURL As String) As Xsl.XslCompiledTransform

   GetXSLContent = New Xsl.XslCompiledTransform
   Dim req As WebRequest = GetExternalRequest(ContentURL)
   Dim result As WebResponse = req.GetResponse()
   Dim ReceiveStream As Stream = result.GetResponseStream()
   Dim objXSLTransform As XmlReader = New XmlTextReader(result.GetResponseStream)
   GetXSLContent.Load(objXSLTransform, Nothing, Nothing)

  End Function

  ''' <summary>
  ''' Map path function
  ''' </summary>
  ''' <param name="localPath"></param>
  ''' <returns></returns>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[pdonker]	03/01/2008  Created
  ''' </history>
  Protected Function GetMappedPath(ByVal localPath As String) As String
   If Not (HttpContext.Current Is Nothing) Then
    Return PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath + localPath
   Else
    Return CStr(System.Threading.Thread.GetDomain.GetData(".appPath")) & New PortalController().GetPortal(PortalId).HomeDirectory & "\" & localPath
   End If
  End Function
#End Region

 End Class
End Namespace
