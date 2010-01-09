<?xml version="1.0" encoding="utf-8"?>
<!-- DWXMLSource="IronAHK.Rusty.xml" -->
<!DOCTYPE xsl:stylesheet  [
	<!ENTITY nbsp   "&#160;">
	<!ENTITY copy   "&#169;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
    <xsl:variable name="title" select="substring-after(doc/assembly/name,'.')"/>
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><xsl:value-of select="$title"/> Documentation</title>
    <link href="../../style/primary.css" rel="stylesheet" type="text/css" />
    <link href="../../style/base.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
    <h1><a href="#"><xsl:value-of select="$title"/></a></h1>
    <div id="contents">
      <div>
        <ul>
          <xsl:for-each select="doc/members/member/@name">
            <xsl:if test="substring(.,1,1)='M' and contains(.,'.Core.')">
              <xsl:variable name="cmd" select="substring(substring-after(substring-before(concat(.,'('),'('),../../../assembly/name),7)"/>
              <li><a>
                <xsl:attribute name="href"><xsl:text>#</xsl:text><xsl:value-of select="$cmd"/></xsl:attribute>
                <xsl:value-of select="$cmd"/></a></li>
            </xsl:if>
          </xsl:for-each>
        </ul>
      </div>
    </div>
    <p class="descr"><br />
      <span class="sub">Browse through the topics in the contents list to the right.</span></p>
    <div id="page">
      <xsl:for-each select="doc/members/member">
        <xsl:if test="substring(@name,1,1)='M' and contains(@name,'.Core.')">
          <xsl:variable name="cmd" select="substring(substring-after(substring-before(concat(@name,'('),'('),../../assembly/name),7)"/>
          <div class="method">
            <xsl:attribute name="id"><xsl:value-of select="$cmd"/></xsl:attribute>
            <h2><xsl:value-of select="$cmd"/></h2>
            <xsl:if test="summary">
              <p><xsl:value-of select="summary"/></p>
            </xsl:if>
            <h3 class="command"><xsl:value-of select="$cmd"/><xsl:text>(</xsl:text>
              <xsl:for-each select="param">
                <xsl:value-of select="@name"/>
                <xsl:if test="generate-id(.)!=generate-id(../param[last()])">
                  <xsl:text>, </xsl:text>
                </xsl:if>
              </xsl:for-each>
              <xsl:text>)</xsl:text></h3>
            <xsl:if test="param">
              <h4>Parameters</h4>
              <table class="params">
                <xsl:for-each select="param">
                  <tr>
                    <td><xsl:value-of select="@name"/></td>
                    <td><xsl:value-of select="."/></td>
                  </tr>
                </xsl:for-each>
              </table>
            </xsl:if>
            <xsl:if test="returns">
              <h4>Returns</h4>
              <p><xsl:value-of select="returns"/></p>
            </xsl:if>
            <xsl:if test="remarks">
              <h4>Remarks</h4>
              <p><xsl:value-of select="remarks"/></p>
            </xsl:if>
          </div>
        </xsl:if>
      </xsl:for-each>
    </div>
    <div style="clear: both;"></div>
    </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
