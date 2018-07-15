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

Imports System.Xml.Xsl
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Settings

''' <summary>
''' Settings class for module
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[pdonker]	03/01/2008  Created
''' </history>
<Serializable()>
Public Class ModuleSettings

 ''' <summary>
 ''' Defautl Cache time. All feeds are assumed to have this cache time unless specified otherwise.
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 <ModuleSetting>
 Public Property DefaultCacheTime As Integer = 30

 ''' <summary>
 ''' Url of transformation
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 <ModuleSetting>
 Public Property XslUrl As String = ""

 ''' <summary>
 ''' Number of items to show of the aggregated feed. This is passed as parameter to the transformation.
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 <ModuleSetting>
 Public Property ItemsToShow As Integer = -1

 ''' <summary>
 ''' Boolean whether or not to show the description of the item. This is passed as parameter to the transformation.
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 <ModuleSetting>
 Public Property ShowItemDetails As Boolean = True

 ''' <summary>
 ''' Boolean whether or not to show the pub date of the item. This is passed as parameter to the transformation.
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 <ModuleSetting>
 Public Property ShowItemDate As Boolean = True

 ''' <summary>
 ''' Number of times to retry getting the feed.
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 <ModuleSetting>
 Public Property RetryTimes As Integer = 3

 ''' <summary>
 ''' Timeout observed for retries. After RetryTimes nr of retries this timeout will be observed before retrying again.
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
 <ModuleSetting>
 Public Property RetryTimeOut As Integer = 120

 ''' <summary>
 ''' Use Ajax (if present)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
 <ModuleSetting>
 Public Property UseAjax As Boolean = False

 ''' <summary>
 ''' Whether to use the background loading scheduled task to refresh the feeds
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
 <ModuleSetting>
 Public Property BackgroundDownload As Boolean = True

 ''' <summary>
 ''' The target to use for the link. Default is "_main".
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
 <ModuleSetting>
 Public Property Target As String = "_main"

#Region " Public Methods "
 ''' <summary>
 ''' Get list of parameters for the XSL transformation
 ''' </summary>
 ''' <returns></returns>
 ''' <remarks>
 ''' </remarks>
 ''' <history>
 ''' 	[pdonker]	03/01/2008  Created
 ''' </history>
 Public Function GetParameterList() As XsltArgumentList
  Dim res As New XsltArgumentList
  res.AddParam("ItemsToShow", "", Me.ItemsToShow)
  res.AddParam("ShowItemDetails", "", Me.ShowItemDetails)
  res.AddParam("ShowItemDate", "", Me.ShowItemDate)
  res.AddParam("Locale", "", Threading.Thread.CurrentThread.CurrentUICulture.Name)
  res.AddParam("Target", "", Me.Target)
  res.AddExtensionObject("urn:NewsExtensions", New XslExtensions)
  Return res
 End Function
#End Region

 Public Shared Function GetSettings(ByVal [module] As ModuleInfo) As ModuleSettings
  Dim repo As New ModuleSettingsRepository()
  Return repo.GetSettings([module])
 End Function

 Public Shared Function GetSettings(ByVal ModuleId As Integer) As ModuleSettings
  Dim m As ModuleInfo = (New ModuleController).GetModule(ModuleId)
  Dim repo As New ModuleSettingsRepository()
  Return repo.GetSettings(m)
 End Function

 Public Sub SaveSettings(ByVal [module] As ModuleInfo)
  Dim repo As New ModuleSettingsRepository()
  repo.SaveSettings([module], Me)
 End Sub
End Class

Public Class ModuleSettingsRepository
 Inherits SettingsRepository(Of ModuleSettings)
End Class
