<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:cf="urn:customFunctions"
    xmlns:xhtml="http://www.w3.org/1999/xhtml">


  <xsl:output method="xml" indent="no" omit-xml-declaration="yes" />

  <xsl:template match="/">

    <old.id>
      <xsl:value-of select="cf:GetID()"/>
    </old.id>

    <xsl:value-of select="cf:ParseAvtoData()" disable-output-escaping="yes"/>
    
    <price>
      <xsl:value-of select="//xhtml:span[@class='price_foCar']"/>
    </price>

    <basic.information>
      <xsl:variable name="basic" select="//xhtml:div[@id='itemPhotoD']/xhtml:table/xhtml:tr/xhtml:td/xhtml:table/xhtml:tr"/>
      <xsl:for-each select="$basic">
        <xsl:variable name="name" select="xhtml:td[@class='txt_ofCard']"/>
        <xsl:variable name="value" select="xhtml:td[@class='txt_ofCardB']"/>
        <xsl:choose>
          <xsl:when test="cf:IsColor($name)">
            <xsl:value-of select="cf:ParseBaseInformation($name, xhtml:td[@class='txt_ofCardB']/xhtml:div/@title)" disable-output-escaping="yes"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="cf:ParseBaseInformation($name, $value)" disable-output-escaping="yes"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
    </basic.information>

    <additional.info>
      <xsl:value-of select="//xhtml:table[@id='AvtoMarketItem_trAddInfo']//xhtml:div[@class='txt_ofCard2']"/>
    </additional.info>

    <completion>
      <xsl:variable name="data" select="//xhtml:div['itemPhotoD']/following::xhtml:table/xhtml:tr[2]/xhtml:td/xhtml:div[@class='txt_ofCardPad']"/>
      <xsl:for-each select="$data">
        <xsl:value-of select="cf:GetCompletionData(., ./following::xhtml:div[@class='txt_ofPrintCard'][1])" disable-output-escaping="yes"/>
      </xsl:for-each>
    </completion>

    <photos>
      <xsl:variable name="photos" select="//xhtml:div[@id='AvtoMarketItem_DivBigImg']/..//xhtml:img"/>
      <xsl:for-each select="$photos">
        <xsl:value-of select="cf:DownloadPhotos(@src)" disable-output-escaping="yes"/>
      </xsl:for-each>
    </photos>

    <owner>
      <xsl:variable name="city" select="//xhtml:tr[@id='AvtoMarketItem_trPhone']/../xhtml:tr"/>
      <xsl:for-each select="$city">
        <xsl:variable name="capture" select=".//xhtml:span[@class='txt_ofCard']"/>
        <xsl:variable name="value" select="./xhtml:td[2]"/>
        <xsl:value-of select="cf:ParsePersonalInfo($capture, $value)" disable-output-escaping="yes"/>
      </xsl:for-each>
    </owner>

  </xsl:template>
</xsl:stylesheet>