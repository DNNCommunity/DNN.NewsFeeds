<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 <xsl:output method="html" indent="yes"/>
 <!-- Headlines News Ticker by Phil 'iwonder' Guerra -->
 <!-- Works with new DNN News v4.0 module -->
 <xsl:param name="ItemsToShow"/>
 <xsl:template match="rss">
  <html>
   <body>
    <marquee direction="left" OnMouseOver="this.stop();" OnMouseOut="this.start();">
     <xsl:apply-templates select="channel/item"/>
    </marquee>
   </body>
  </html>
 </xsl:template>
 <xsl:template match="channel/item">
  <xsl:variable name="spacer" select="' *** '"/>
  <xsl:variable name="headline" select="title"/>
  <xsl:if test="position() &lt; $ItemsToShow + 1 or $ItemsToShow &lt; 0 ">
   <strong>
    <a href="{link}">
     <xsl:value-of select="title"/>
    </a>
    <xsl:value-of select="$spacer"/>
   </strong>
  </xsl:if>
 </xsl:template>
</xsl:stylesheet>
