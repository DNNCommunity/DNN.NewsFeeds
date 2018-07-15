<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RssModule.ascx.vb" Inherits="DotNetNuke.Modules.News.RssModule" %>
<%@ Register assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI" tagprefix="asp" %>
<%@ Register TagPrefix="dnnNews" Namespace="DotNetNuke.Modules.News.Controls" Assembly="DotNetNuke.Modules.News" %>
<p class="rssLine" runat="server" id="rssLine">
 <a href="<%= ResolveUrl("~/DesktopModules/News/API/News.rss") & "?TabId=" & TabId.ToString & "&ModuleId=" & ModuleId.ToString %>" title="<%= DotNetNuke.Services.Localization.Localization.GetString("SyndicateModule.Action", LocalResourceFile) %>"><img src="<%= ResolveUrl("~/images/rss.gif") %>" alt="<%= DotNetNuke.Services.Localization.Localization.GetString("SyndicateModule.Action", LocalResourceFile) %>" width="16" height="16" border="0" /></a>
</p>
<asp:Panel runat="server" ID="pnlRss">
 <dnnNews:Feed runat="server" id="feedOutput" />
 <asp:PlaceHolder runat="server" ID="plhErrors" />
 <div style="display:none;">
  <asp:Button Id="btnUpdate" Text="Hit Me" runat="server" />
 </div>
</asp:Panel>
