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
Imports System.Text
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.News.Entities.Feeds

Namespace Services.Retrieval
 Public Class BackgroundLoader
  Inherits DotNetNuke.Services.Scheduling.SchedulerClient

  Private Log As New StringBuilder

  Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
   MyBase.new()
   Me.ScheduleHistoryItem = objScheduleHistoryItem
  End Sub

  Public Overrides Sub DoWork()

   Try

    Me.Progressing() ' Start

    For Each portal As PortalInfo In (New PortalController).GetPortals
     Log.AppendLine(String.Format("Checking Portal '{0}' (ID: {1})", portal.PortalName, portal.PortalID))
     Using ir As IDataReader = Data.DataProvider.Instance.GetUpdatableModules(portal.PortalID)
      Do While ir.Read
       Dim ModuleId As Integer = CInt(ir.Item("ModuleId"))
       Dim TabId As Integer = CInt(ir.Item("TabId"))
       Dim m As ModuleInfo = (New ModuleController).GetModule(ModuleId, TabId, False)
       Dim ModuleTitle As String = m.ModuleTitle
       Dim RetryTimes As Integer = CInt(ir.Item("RetryTimes"))
       Dim RetryTimeOut As Integer = CInt(ir.Item("RetryTimeOut"))
       If FeedController.HasExpiredFeeds(ModuleId, RetryTimes, RetryTimeOut) Then
        Log.Append(String.Format("Refreshing Module '{0}' (ID: {1})", ModuleTitle, ModuleId))
        If Not IO.Directory.Exists(portal.HomeDirectoryMapPath & "\Cache\") Then
         IO.Directory.CreateDirectory(portal.HomeDirectoryMapPath & "\Cache\")
        End If
        Dim settings As ModuleSettings = ModuleSettings.GetModuleSettings(ModuleId)
        Dim aggregator As New FeedAggregator(portal.HomeDirectoryMapPath & "\Cache\", settings, ModuleTitle)
        aggregator.Load(FeedController.GetFeedsByModule(ModuleId))
        Common.SaveResultFeed(aggregator.ToXml, ModuleId, portal.HomeDirectoryMapPath & "\Cache\")
        Log.AppendLine(" - Success")
       End If
      Loop
     End Using
    Next

    Me.ScheduleHistoryItem.AddLogNote(Log.ToString.Replace(vbCrLf, "<br />"))
    Me.ScheduleHistoryItem.Succeeded = True

   Catch ex As Exception
    Me.ScheduleHistoryItem.AddLogNote(Log.ToString & vbCrLf & "Failure: " & ex.Message & "(" & ex.StackTrace & ")" & vbCrLf & Log.ToString)
    Me.ScheduleHistoryItem.Succeeded = False
    Me.Errored(ex)
    LogException(ex)
   End Try


  End Sub
 End Class
End Namespace

