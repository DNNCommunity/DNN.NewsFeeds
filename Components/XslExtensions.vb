Imports System.Globalization

Public Class XslExtensions

 Public Function FormatDateTimeFromString(d As String, format As String) As String
  Try
   Return Date.Parse(d).ToString(format)
  Catch ex As Exception
   Return ""
  End Try
 End Function

 Public Function FormatDateTimeFromString(d As String, format As String, locale As String) As String
  Dim culture As New CultureInfo(locale)
  Try
   Return Date.Parse(d).ToString(format, culture)
  Catch ex As Exception
   Return ""
  End Try
 End Function

End Class
