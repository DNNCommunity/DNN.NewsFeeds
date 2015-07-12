<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditRss.ascx.vb" Inherits="DotNetNuke.Modules.News.EditRss" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>

<asp:Panel runat="server" ID="pnlList">
<asp:DataList runat="server" ID="dlFeeds" DataKeyField="FeedId" ItemStyle-CssClass="Normal">
 <HeaderTemplate>
<table class="dnnGrid">
 <tr class="dnnGridHeader">
  <th />
  <th><asp:Label runat="server" ID="lblFeedUrl" resourcekey="lblFeedUrl" /></th>
  <th><asp:Label runat="server" ID="lblSecure" resourcekey="lblSecure" /></th>
  <th><asp:Label runat="server" ID="lblCustomTransform" resourcekey="lblCustomTransform" /></th>
 </tr>
 </HeaderTemplate>
 <ItemTemplate>
  <tr class="dnnGridItem">
   <td><asp:imagebutton runat="server" causesvalidation="false" commandname="Edit" imageurl="~/images/edit.gif" alternatetext="Edit" resourcekey="Edit" id="cmdEditFeed" /></td>
   <td><%#DataBinder.Eval(Container.DataItem, "FeedUrl")%></td>
   <td><img src="<%#ResolveUrl("~/images/" & CStr(IIF(DotNetNuke.Modules.News.Common.GetAString(DataBinder.Eval(Container.DataItem, "User"))="", "unchecked.gif", "checked.gif")))%>" alt="" border="0"/></td>
   <td><img src="<%#ResolveUrl("~/images/" & CStr(IIF(DotNetNuke.Modules.News.Common.GetAString(DataBinder.Eval(Container.DataItem, "OverrideTransform"))="", "unchecked.gif", "checked.gif")))%>" alt="" border="0"/></td>
  </tr>
 </ItemTemplate>
 <FooterTemplate>
</table>
 </FooterTemplate>
</asp:DataList>
 <p style="margin: 20px 0px 0px 0px;">
  <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CausesValidation="false" CssClass="dnnSecondaryAction" />
  <asp:LinkButton runat="server" ID="cmdReturn" resourcekey="cmdReturn" CausesValidation="false" CssClass="dnnPrimaryAction" />
 </p>
</asp:Panel>
<asp:Panel runat="server" ID="pnlEdit">
<asp:HiddenField runat="server" ID="hidFeedId" />
 <table cellspacing="0" cellpadding="2" border="0">
  <tr>
   <td class="SubHead">
    <dnn:label id="plFeedUrl" runat="server" controlname="txtFeedUrl" suffix=":" />
   </td>
   <td />
   <td>
    <asp:TextBox id="txtFeedUrl" runat="server" Width="500" />
   </td>
  </tr>
  <tr>
   <td class="SubHead">
    <dnn:label id="plCacheTime" runat="server" controlname="txtCacheTime" suffix=":" />
   </td>
   <td />
   <td>
    <asp:TextBox id="txtCacheTime" runat="server" />
    <asp:comparevalidator id="valcompCacheTime" runat="server" controltovalidate="txtCacheTime" display="Dynamic" errormessage="Not a valid (whole) number!" type="Integer" operator="DataTypeCheck" resourcekey="valWholeNumber.Error" />
    <asp:requiredfieldvalidator ID="reqCacheTime" runat="server" errormessage="Required!" ResourceKey="Required.Error" controltovalidate="txtCacheTime" display="Dynamic" />
   </td>
  </tr>
  <tr>
   <td class="SubHead">
    <dnn:label id="plUser" runat="server" controlname="txtUser" suffix=":" />
   </td>
   <td />
   <td>
    <asp:TextBox id="txtUser" runat="server" />
   </td>
  </tr>
  <tr>
   <td class="SubHead">
    <dnn:label id="plPassword" runat="server" controlname="txtPassword" suffix=":" />
   </td>
   <td />
   <td>
    <asp:TextBox id="txtPassword" runat="server" />
   </td>
  </tr>
  <tr>
   <td class="SubHead" style="vertical-align:top">
    <dnn:label id="plOverrideTransform" runat="server" controlname="txtOverrideTransform" suffix=":" />
   </td>
   <td style="vertical-align:top">
    <asp:radiobutton id="optStandard" runat="server" autopostback="True" groupname="optOverrideTransform" />
   </td>
   <td style="vertical-align:top">
    <p><%=String.Format(DotNetNuke.Services.Localization.Localization.GetString("optStandard", LocalResourceFile), PortalId)%><br /><br />
    <asp:DropDownList runat="server" ID="ddOverrideTransform" /></p>
   </td>
  </tr>
  <tr>
   <td>&nbsp;</td>
   <td style="vertical-align:top">
    <asp:radiobutton id="optUrl" runat="server" autopostback="True" groupname="optOverrideTransform" /></p>
   </td>
   <td style="vertical-align:top">
    <p><%=String.Format(DotNetNuke.Services.Localization.Localization.GetString("optUrl", LocalResourceFile), PortalId)%></p>
    <dnn:url id="ctlXsl" runat="server" width="300" showtabs="False" showurls="True" showfiles="True"
	 			urltype="F" showlog="False" shownewwindow="False" showtrack="False" required="True" />
	   </td>
  </tr>
  <tr>
   <td>&nbsp;</td>
   <td style="vertical-align:top">
    <asp:radiobutton id="optType" runat="server" autopostback="True" groupname="optOverrideTransform" />
   </td>
   <td style="vertical-align:top">
    <p><%=DotNetNuke.Services.Localization.Localization.GetString("optType", LocalResourceFile)%><br /><br />
    <asp:TextBox id="txtOverrideTransform" runat="server" Width="500" /></p>
   </td>
  </tr>
  <tr>
   <td class="SubHead">
    <dnn:label id="plCache" runat="server" controlname="txtCache" suffix=":" />
   </td>
   <td />
   <td>
    <asp:TextBox id="txtCache" runat="server" TextMode="MultiLine" Width="500" Height="300" />
   </td>
  </tr>
 </table>
 <p style="margin: 20px 0px 0px 0px;">
  <asp:LinkButton runat="server" ID="cmdCancel" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" CausesValidation="false" />
  <asp:LinkButton runat="server" ID="cmdDelete" resourcekey="cmdDelete" CssClass="dnnSecondaryAction" CausesValidation="false" />
  <asp:LinkButton runat="server" ID="cmdUpdate" resourcekey="cmdUpdate" CssClass="dnnPrimaryAction" />
 </p>
</asp:Panel>