<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />

  <xsl:template match="folderList">
    <xsl:for-each select="entry">
      <li>
        <xsl:attribute name="class">
          tree-node jstree-closed
          <xsl:if test="access = 'Read'">
            access-read
          </xsl:if>
          <xsl:if test="total_sub_folder = 0 and not(provider_id)">
            jstree-empty
          </xsl:if>
          <xsl:if test="provider_key">
            third-party-entry
          </xsl:if>
        </xsl:attribute>
        <xsl:attribute name="data-id"><xsl:value-of select="id" /></xsl:attribute>
        <xsl:if test="folder_url">
          <xsl:attribute name="data-href">
            <xsl:value-of select="folder_url" />
          </xsl:attribute>
        </xsl:if>
        <span class="jstree-icon jstree-expander" > </span>
        <a>
          <xsl:attribute name="title">
            <xsl:value-of select="title" />
          </xsl:attribute>
          <xsl:attribute name="data-id"><xsl:value-of select="id" /></xsl:attribute>
          <span>
            <xsl:attribute name="class">
              jstree-icon
              <xsl:if test="provider_key">
                <xsl:value-of select="provider_key" />
              </xsl:if>
            </xsl:attribute>
          </span>
          <xsl:value-of select="title" />
        </a>
      </li>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>