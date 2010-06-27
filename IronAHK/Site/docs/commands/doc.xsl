<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">
  <xsl:template match="c">
    <code>
    <xsl:apply-templates />
    
    </code>
  </xsl:template>
  <xsl:template match="para">
    <p>
      <xsl:apply-templates />
    </p>
  </xsl:template>
  <xsl:template match="see">
    <a>
    <xsl:variable name="cref" select="substring(substring-after(@cref,/doc/assembly/name),7)" />
    <xsl:attribute name="href"><xsl:text>../</xsl:text><xsl:value-of select="$cref" /><xsl:text>/</xsl:text></xsl:attribute>
    <xsl:if test=".=''">
      <xsl:value-of select="$cref" />
    </xsl:if>
    <xsl:apply-templates />
    </a>
  </xsl:template>
  <xsl:template match="code">
    <pre>
      <xsl:apply-templates />
    </pre>
  </xsl:template>
  <xsl:template match="example">
    <div class="example">
      <xsl:apply-templates />
    </div>
  </xsl:template>
  <xsl:template match="paramref">
    <a>
    <xsl:attribute name="href"><xsl:text>#</xsl:text><xsl:value-of select="@name" /></xsl:attribute>
    <xsl:apply-templates />
    </a>
  </xsl:template>
  <xsl:template match="list">
    <xsl:choose>
      <xsl:when test="@type='bullet'">
        <ul>
          <xsl:for-each select="item">
            <li>
              <xsl:apply-templates />
            </li>
          </xsl:for-each>
        </ul>
      </xsl:when>
      <xsl:when test="@type='number'">
        <ol>
          <xsl:for-each select="item">
            <li>
              <xsl:apply-templates />
            </li>
          </xsl:for-each>
        </ol>
      </xsl:when>
      <xsl:when test="@type='table'">
        <xsl:if test="listheader">
          <tr>
            <xsl:for-each select="listheader/term">
              <th><xsl:apply-templates />
              </th>
            </xsl:for-each>
          </tr>
        </xsl:if>
        <xsl:for-each select="item">
          <tr>
            <xsl:for-each select="term">
              <td><xsl:apply-templates />
              </td>
            </xsl:for-each>
          </tr>
        </xsl:for-each>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="item">
    <dfn>
    <xsl:apply-templates />
    </dfn>
  </xsl:template>
  <xsl:template match="description">
    <span class="description">
    <xsl:apply-templates />
    </span>
  </xsl:template>
</xsl:stylesheet>
