<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:custFuncs="urn:customFunctions"
    xmlns:xhtml="http://www.w3.org/1999/xhtml">

  <xsl:output method="xml" indent="no" omit-xml-declaration="yes" />

  <xsl:template match="/">
    <listings>
      <xsl:variable name="rows" select="//xhtml:div[@id='MarketGrid']/xhtml:table/xhtml:tr/xhtml:td/xhtml:table/xhtml:tr[@id]"/>
      <xsl:for-each select="$rows">
        <listing>
          <xsl:variable name="link">
            <xsl:text>http://cars.avto.ru</xsl:text>
            <xsl:value-of select="xhtml:td[3]/xhtml:span/xhtml:a/@href"/>
          </xsl:variable>
          <detailsLink>
            <xsl:value-of select="$link"/>
          </detailsLink>
          <price>
            <xsl:value-of select="xhtml:td[1]/xhtml:span[@class='text_in_tabl']"/>
          </price>
          <xsl:value-of disable-output-escaping="yes" select="custFuncs:ApplyTransformationForWebRequestResult($link, 'transform.xsl', 'nativetransform.xml','transtransform.xml')"/>
        </listing>
      </xsl:for-each>
    </listings>
  </xsl:template>

</xsl:stylesheet>
