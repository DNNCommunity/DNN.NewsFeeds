<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="DotNetNuke.Modules.News.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>

<table cellspacing="0" cellpadding="2" border="0">
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plDefaultCacheTime" runat="server" controlname="txtDefaultCacheTime" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtDefaultCacheTime" runat="server" width="60" maxlength="10" cssclass="NormalTextBox" />&nbsp;
   <asp:comparevalidator ID="Comparevalidator1" runat="server" errormessage="Not a valid whole number!" operator="DataTypeCheck" type="Integer" controltovalidate="txtDefaultCacheTime" display="Dynamic" />&nbsp;
   <asp:requiredfieldvalidator ID="Requiredfieldvalidator1" runat="server" errormessage="Required!" controltovalidate="txtDefaultCacheTime" display="Dynamic" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plRetryTimes" runat="server" controlname="txtRetryTimes" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtRetryTimes" runat="server" width="60" maxlength="10" cssclass="NormalTextBox" />&nbsp;
   <asp:comparevalidator runat="server" errormessage="Not a valid whole number!" operator="DataTypeCheck" type="Integer" controltovalidate="txtRetryTimes" display="Dynamic" />&nbsp;
   <asp:requiredfieldvalidator runat="server" errormessage="Required!" controltovalidate="txtRetryTimes" display="Dynamic" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plRetryTimeOut" runat="server" controlname="txtRetryTimeOut" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtRetryTimeOut" runat="server" width="60" maxlength="10" cssclass="NormalTextBox" />&nbsp;
   <asp:comparevalidator runat="server" errormessage="Not a valid whole number!" operator="DataTypeCheck" type="Integer" controltovalidate="txtRetryTimeOut" display="Dynamic" />&nbsp;
   <asp:requiredfieldvalidator runat="server" errormessage="Required!" controltovalidate="txtRetryTimeOut" display="Dynamic" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plXslUrl" runat="server" controlname="txtXslUrl" suffix=":" />
  </td>
  <td>
   <asp:DropDownList runat="server" ID="ddXsl" AutoPostBack="true" /><br />
   <portal:url id="ctlRSSxsl" runat="server" width="300" showtabs="False" showurls="True" showfiles="True"
				urltype="F" showlog="False" shownewwindow="False" showtrack="False" required="True" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plItemsToShow" runat="server" controlname="txtItemsToShow" suffix=":" />
  </td>
  <td>
   <asp:textbox id="txtItemsToShow" runat="server" width="60" maxlength="10" cssclass="NormalTextBox" />&nbsp;
   <asp:comparevalidator ID="Comparevalidator2" runat="server" errormessage="Not a valid whole number!" operator="DataTypeCheck" type="Integer" controltovalidate="txtItemsToShow" display="Dynamic" />&nbsp;
   <asp:requiredfieldvalidator ID="Requiredfieldvalidator2" runat="server" errormessage="Required!" controltovalidate="txtItemsToShow" display="Dynamic" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plShowItemDetails" runat="server" controlname="chkShowItemDetails" suffix=":" />
  </td>
  <td>
   <asp:checkbox runat="server" id="chkShowItemDetails" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plShowItemDate" runat="server" controlname="chkShowItemDate" suffix=":" />
  </td>
  <td>
   <asp:checkbox runat="server" id="chkShowItemDate" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plTarget" runat="server" controlname="ddTarget" suffix=":" />
  </td>
  <td>
   <asp:DropDownList runat="server" ID="ddTarget">
    <asp:ListItem Value="_main" resourcekey="targetMain" />
    <asp:ListItem Value="_blank" resourcekey="targetBlank" />
    <asp:ListItem Value="_new" resourcekey="targetNew" />
   </asp:DropDownList>
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plUseAjax" runat="server" controlname="chkUseAjax" suffix=":" />
  </td>
  <td>
   <asp:checkbox runat="server" id="chkUseAjax" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="165">
   <dnn:label id="plBackgroundDownload" runat="server" controlname="chkBackgroundDownload" suffix=":" />
  </td>
  <td>
   <asp:checkbox runat="server" id="chkBackgroundDownload" />
  </td>
 </tr>
</table>
