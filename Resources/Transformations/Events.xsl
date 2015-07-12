<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:news="http://www.dotnetnuke.com/modules/news" xmlns:e="http://www.dotnetnuke.com/dnnevents" xmlns:newsext="urn:NewsExtensions">
 <xsl:output method="html" indent="yes"/>
 <xsl:param name="ItemsToShow"/>
 <xsl:param name="ShowItemDetails"/>
 <xsl:param name="ShowItemDate"/>
 <xsl:param name="Locale"/>
 <xsl:param name="Target"/>
 <xsl:template match="rss">
  <xsl:for-each select="channel/item[position()&lt;=$ItemsToShow or $ItemsToShow&lt;1]">
   <xsl:sort data-type="text" select="e:EventTimeBegin" order="ascending"/>
   <h6 style="margin-bottom:0px;padding-bottom:0px;">
    <xsl:value-of select="newsext:FormatDateTimeFromString(e:EventTimeBegin, 'D')" />
    <xsl:if test="e:AllDayEvent='False'">&#160;<xsl:value-of select="newsext:FormatDateTimeFromString(e:EventTimeBegin, 't')" /></xsl:if>
    &#160;<xsl:value-of select="e:LocationName"/>
   </h6>
   <h3 style="margin-top:0px;padding-top:0px;">
    <a>
     <xsl:choose>
      <xsl:when test="normalize-space(e:DetailUrl)=''">
       <xsl:attribute name="href"><xsl:value-of select="link"/></xsl:attribute>
      </xsl:when>
      <xsl:otherwise>
       <xsl:attribute name="href"><xsl:value-of select="e:DetailUrl"/></xsl:attribute>
      </xsl:otherwise>
     </xsl:choose>
     <xsl:attribute name="target"><xsl:value-of select="$Target"/></xsl:attribute>
     <xsl:value-of select="title"/>
    </a>
   </h3>
  </xsl:for-each>
 </xsl:template>
</xsl:stylesheet>
