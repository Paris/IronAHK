<?xml version="1.0" encoding="utf-8"?>
<!-- DWXMLSource="control/index.xml" -->
<!DOCTYPE xsl:stylesheet  [
	<!ENTITY nbsp   "&#160;">
	<!ENTITY copy   "&#169;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>
  <xsl:template match="/doc">
    <html xmlns="http://www.w3.org/1999/xhtml">
    <xsl:variable name="cmd" select="substring(substring-after(substring-before(concat(members/member/@name,'('),'('),assembly/name),7)"/>
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><xsl:value-of select="$cmd"/></title>
    <link href="../../primary.css" rel="stylesheet" type="text/css" />
    <link href="../../api.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
    <h1><xsl:value-of select="$cmd"/></h1>
    <div id="page">
      <xsl:if test="members/member/summary">
        <p><xsl:value-of select="members/member/summary"/></p>
      </xsl:if>
      <h3 class="command"><xsl:value-of select="$cmd"/><xsl:text>(</xsl:text>
        <xsl:for-each select="members/member/param">
          <xsl:value-of select="@name"/>
          <xsl:if test="generate-id(.)!=generate-id(../param[last()])">
            <xsl:text>, </xsl:text>
          </xsl:if>
        </xsl:for-each>
        <xsl:text>)</xsl:text></h3>
      <xsl:if test="members/member/param">
        <h4>Parameters</h4>
        <table class="params">
          <xsl:for-each select="members/member/param">
            <tr>
              <td><xsl:value-of select="@name"/></td>
              <td><xsl:value-of select="."/></td>
            </tr>
          </xsl:for-each>
        </table>
      </xsl:if>
      <xsl:if test="members/member/returns">
        <h4>Returns</h4>
        <p><xsl:value-of select="members/member/returns"/></p>
      </xsl:if>
      <xsl:if test="members/member/remarks">
        <h4>Remarks</h4>
        <p><xsl:value-of select="members/member/remarks"/></p>
      </xsl:if>
    </div>
    </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
