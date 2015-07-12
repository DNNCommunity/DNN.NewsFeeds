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

#Region " Private Members "
    Private _moduleId As Integer = -1
    Private _settings As Hashtable = Nothing
#End Region

#Region " Constructor "
    ''' <summary>
    ''' Gets settings from ModuleSettings table.
    ''' </summary>
    ''' <param name="ModuleId"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pdonker]	03/01/2008  Created
    ''' </history>
    Public Sub New(ByVal ModuleId As Integer)

        _moduleId = ModuleId
        _settings = (New DotNetNuke.Entities.Modules.ModuleController).GetModuleSettings(ModuleId)

        _settings.ReadValue("DefaultCacheTime", DefaultCacheTime)
        _settings.ReadValue("XslUrl", XslUrl)
        _settings.ReadValue("ItemsToShow", ItemsToShow)
        _settings.ReadValue("ShowItemDetails", ShowItemDetails)
        _settings.ReadValue("ShowItemDate", ShowItemDate)
        _settings.ReadValue("RetryTimes", RetryTimes)
        _settings.ReadValue("RetryTimeOut", RetryTimeOut)
        _settings.ReadValue("UseAjax", UseAjax)
        _settings.ReadValue("BackgroundDownload", BackgroundDownload)
        _settings.ReadValue("Target", Target)

    End Sub
#End Region

#Region " Public Methods "
    ''' <summary>
    ''' Save settings to ModuleSettings table
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''   [tstensitzki] 07/24/2010 Clear feeds cache after changing settings added
    ''' 	[pdonker]	03/01/2008  Created
    ''' </history>
    Public Sub Save()
        Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
        objModules.UpdateModuleSetting(_moduleId, "DefaultCacheTime", Me.DefaultCacheTime.ToString)
        objModules.UpdateModuleSetting(_moduleId, "XslUrl", Me.XslUrl.ToString)
        objModules.UpdateModuleSetting(_moduleId, "ItemsToShow", Me.ItemsToShow.ToString)
        objModules.UpdateModuleSetting(_moduleId, "ShowItemDetails", Me.ShowItemDetails.ToString)
        objModules.UpdateModuleSetting(_moduleId, "ShowItemDate", Me.ShowItemDate.ToString)
        objModules.UpdateModuleSetting(_moduleId, "RetryTimes", Me.RetryTimes.ToString)
        objModules.UpdateModuleSetting(_moduleId, "RetryTimeOut", Me.RetryTimeOut.ToString)
        objModules.UpdateModuleSetting(_moduleId, "UseAjax", Me.UseAjax.ToString)
        objModules.UpdateModuleSetting(_moduleId, "BackgroundDownload", Me.BackgroundDownload.ToString)
        objModules.UpdateModuleSetting(_moduleId, "Target", Me.Target)
        DotNetNuke.Common.Utilities.DataCache.RemoveCache(CacheKey(_moduleId))
        Entities.Feeds.FeedController.ClearFeedsCache(_moduleId)
    End Sub

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

#Region " Properties "
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
    Public Property RetryTimes As Integer = 3

    ''' <summary>
    ''' Timeout observed for retries. After RetryTimes nr of retries this timeout will be observed before retrying again.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RetryTimeOut As Integer = 120

    ''' <summary>
    ''' Use Ajax (if present)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseAjax As Boolean = False

    ''' <summary>
    ''' Whether to use the background loading scheduled task to refresh the feeds
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackgroundDownload As Boolean = True

    ''' <summary>
    ''' The target to use for the link. Default is "_main".
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Target As String = "_main"
#End Region

#Region " Static Methods "
    ''' <summary>
    ''' Gets the module's settings and caches them
    ''' </summary>
    ''' <param name="ModuleId">Module ID</param>
    ''' <returns>These settings</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pdonker]	03/01/2008  Created
    ''' </history>
    Public Shared Function GetModuleSettings(ByVal ModuleId As Integer) As ModuleSettings
        Dim modSettings As ModuleSettings = Nothing
        Try
            modSettings = CType(DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey(ModuleId)), ModuleSettings)
        Catch
        End Try
        If modSettings Is Nothing Then
            modSettings = New ModuleSettings(ModuleId)
            DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey(ModuleId), modSettings)
        End If
        Return modSettings
    End Function

    ''' <summary>
    ''' Key to use for caching these settings.
    ''' </summary>
    ''' <param name="ModuleId"></param>
    ''' <returns>Key as string</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pdonker]	03/01/2008  Created
    ''' </history>
    Private Shared Function CacheKey(ByVal ModuleId As Integer) As String
        Return "NewsSettingsModule" & ModuleId.ToString
    End Function
#End Region

End Class
