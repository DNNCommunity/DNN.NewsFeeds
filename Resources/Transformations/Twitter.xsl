<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:news="http://www.dotnetnuke.com/modules/news" xmlns:newsext="urn:NewsExtensions">
 <xsl:output method="html" indent="yes"/>
 <xsl:param name="ItemsToShow"/>
 <xsl:param name="ShowItemDetails"/>
 <xsl:param name="ShowItemDate"/>
 <xsl:param name="Locale"/>
 <xsl:template match="rss">
  <xsl:for-each select="channel/item[position()&lt;=$ItemsToShow or $ItemsToShow&lt;1]">
   <xsl:sort data-type="text" select="news:pubDateParsed" order="descending" />
   <p class="DNN_News_ItemLink">
    <a href="{link}" target="_main">
     <xsl:value-of select="newsext:FormatDateTimeFromString(news:pubDateParsed, 'D')" />
    </a>
   </p>
   <xsl:if test="$ShowItemDetails='true'">
    <p class="DNN_News_ItemDetails">
     <xsl:value-of select="description" disable-output-escaping="yes"/>
    </p>
   </xsl:if>
  </xsl:for-each>
 </xsl:template>
</xsl:stylesheet>
