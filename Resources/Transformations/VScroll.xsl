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
  <marquee height="300" direction="up" scrollamount="1" scrolldelay="1" OnMouseOver="this.stop();" OnMouseOut="this.start();">
   <xsl:for-each select="channel/item[position()&lt;=$ItemsToShow or $ItemsToShow&lt;1]">
    <xsl:sort data-type="text" select="news:pubDateParsed" order="descending" />
    <div class="DNN_News_Item">
     <div class="DNN_News_ItemDate">
      <xsl:if test="$ShowItemDate='true'">
       <xsl:value-of select="newsext:FormatDateTimeFromString(news:pubDateParsed, 'D')" />
      </xsl:if>
      <xsl:if test="$ShowItemDate='true'">
       <xsl:text> - </xsl:text>
      </xsl:if>
     </div>
     <div class="DNN_News_ItemLink">
      <a href="{link}" target="_blank">
       <xsl:value-of select="title"/>
      </a>
     </div>
     <xsl:if test="$ShowItemDetails='true'">
      <div class="DNN_News_ItemDetails">
       <a href="{link}" target="_blank">
        <xsl:value-of select="description" disable-output-escaping="yes"/>
       </a>
      </div>
     </xsl:if>
    </div>
   </xsl:for-each>
  </marquee>
 </xsl:template>
</xsl:stylesheet>
