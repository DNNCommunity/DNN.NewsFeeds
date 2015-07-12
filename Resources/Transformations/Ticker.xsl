<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:news="http://www.dotnetnuke.com/modules/news" xmlns:newsext="urn:NewsExtensions">
 <xsl:output method="html" indent="yes"/>
 <xsl:param name="ItemsToShow"/>
 <xsl:param name="ShowItemDetails"/>
 <xsl:param name="ShowItemDate"/>
 <xsl:param name="Locale"/>
 <xsl:template match="rss">
  <style>
			.DNN_News_Item { padding-bottom: 10px; }
		</style>
  <marquee direction="left" scrollamount="8" scrolldelay="1" OnMouseOver="this.stop();" OnMouseOut="this.start();">
   <div class="DNN_News_Item">
    <xsl:for-each select="channel/item[position()&lt;=$ItemsToShow or $ItemsToShow&lt;1]">
     <xsl:sort data-type="text" select="news:pubDateParsed" order="descending" />
     <span class="DNN_News_ItemDate">
      <xsl:if test="$ShowItemDate='true'">
       <xsl:value-of select="newsext:FormatDateTimeFromString(news:pubDateParsed, 'D')" />
       <xsl:text> - </xsl:text>
      </xsl:if>
      <xsl:value-of select="source"/>
      <xsl:text> - </xsl:text>
     </span>
     <span class="DNN_News_ItemLink">
      <a href="{link}" target="_blank">
       <xsl:value-of select="title"/>
      </a>
      <xsl:text> - </xsl:text>
     </span>
     <xsl:if test="$ShowItemDetails='true'">
      <span class="DNN_News_ItemDetails">
       <a href="{link}" target="_blank">
        <xsl:value-of select="description" disable-output-escaping="yes"/>
       </a>
      </span>
     </xsl:if>
     <xsl:text> +++ </xsl:text>
    </xsl:for-each>
   </div>
  </marquee>
 </xsl:template>
</xsl:stylesheet>
