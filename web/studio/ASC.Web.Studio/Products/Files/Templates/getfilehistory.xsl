<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />

  <register type="ASC.Web.Files.Resources.FilesCommonResource,ASC.Web.Files" alias="fres" />

  <xsl:template match="fileList">
    <xsl:for-each select="entry">
      <tr class="version-row" >
        <xsl:attribute name="data-version">
          <xsl:value-of select="version" />
        </xsl:attribute>
        <td class="fade-td"></td>
        <td class="version-num">
          <xsl:value-of select="version" />.
        </td>
        <td class="version-datetime">
          <b>
            <xsl:value-of select="substring-before(modified_on, ' ')" />
          </b>
          <xsl:value-of select="substring-after(modified_on, ' ')" />
        </td>
        <td class="version-author" >
          <span class="userLink">
            <xsl:attribute name="title">
              <xsl:value-of select="modified_by" />
            </xsl:attribute>
            <xsl:value-of select="modified_by" />
          </span>
        </td>
        <td class="version-operation">
          <div class="version-download">
            <xsl:attribute name="title">
              <resource name="fres.ButtonDownload" />
            </xsl:attribute>
          </div>
          <div class="version-preview">
            <xsl:attribute name="title">
              <resource name="fres.OpenFile" />
            </xsl:attribute>
          </div>
        </td>
        <td class="version-size">
          <xsl:value-of select="content_length" />
        </td>
        <td class="version-operation version-restore">
          <a class="baseLinkAction">
            <xsl:attribute name="title">
              <resource name="fres.MakeCurrent" />
            </xsl:attribute>
            <resource name="fres.MakeCurrent" />
          </a>
        </td>
      </tr>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>