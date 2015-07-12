<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:news="http://www.dotnetnuke.com/modules/news" xmlns:newsext="urn:NewsExtensions">
 <xsl:output method="html" indent="yes"/>
 <xsl:param name="ItemsToShow"/>
 <xsl:param name="ShowItemDetails"/>
 <xsl:param name="ShowItemDate"/>
 <xsl:param name="Locale"/>
 <xsl:param name="Target"/>
 <xsl:template match="rss">
  <xsl:for-each select="channel/item[position()&lt;=$ItemsToShow or $ItemsToShow&lt;1]">
   <xsl:sort data-type="text" select="news:pubDateParsed" order="descending" />
   <h4>
    <a href="{link}">
     <xsl:attribute name="target"><xsl:value-of select="$Target"/></xsl:attribute>
     <xsl:value-of select="title"/>
    </a>
   </h4>
   <xsl:if test="$ShowItemDate='true'">
    <h6>
     <xsl:value-of select="newsext:FormatDateTimeFromString(news:pubDateParsed, 'D')" />
    </h6>
   </xsl:if>
   <xsl:if test="$ShowItemDetails='true'">
    <p class="Normal">
     <xsl:value-of select="description" disable-output-escaping="yes"/>
    </p>
   </xsl:if>
  </xsl:for-each>
 </xsl:template>
</xsl:stylesheet>
