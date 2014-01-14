<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />

  <register type="ASC.Web.Files.Resources.FilesCommonResource,ASC.Web.Files" alias="fres" />

  <xsl:template match="folder">
    <li name="addRow">
      <xsl:attribute name="class">
        clearFix file-row folder-row new-folder
        <xsl:if test="provider_key">
          third-party-entry
        </xsl:if>
        <xsl:if test="error">
          error-entry
        </xsl:if>
        <xsl:if test="shared = 'true'">
          share-open
        </xsl:if>
      </xsl:attribute>
      <xsl:attribute name="data-id">folder_<xsl:value-of select="id" /></xsl:attribute>
      <xsl:if test="spare_data">
        <xsl:attribute name="spare_data"><xsl:value-of select="spare_data" /></xsl:attribute>
      </xsl:if>
      <div class="checkbox">
        <input type="checkbox" >
          <xsl:attribute name="title">
            <resource name="fres.TitleSelectFile" />
          </xsl:attribute>
        </input>
      </div>
      <div class="thumb-folder">
        <xsl:attribute name="title">
          <xsl:value-of select="title" />
        </xsl:attribute>
        <xsl:if test="provider_key">
          <div>
            <xsl:attribute name="class">
              provider-key
              <xsl:value-of select="provider_key" />
            </xsl:attribute>
          </div>
        </xsl:if>
      </div>
      <div class="entry-info">
        <div class="entry-title">
          <div class="name">
            <a>
              <xsl:attribute name="title">
                <xsl:value-of select="title" />
              </xsl:attribute>
              <xsl:value-of select="title" />
            </a>
          </div>
          <xsl:if test="isnew > 0">
            <div class="is-new">
              <xsl:attribute name="title">
                <resource name="fres.RemoveIsNew" />
              </xsl:attribute>
              <xsl:value-of select="isnew" />
            </div>
          </xsl:if>
        </div>
        <div class="entry-descr">

          <xsl:if test="create_by">
            <xsl:choose>
              <xsl:when test="error">
                <span>
                  <xsl:attribute name="title">
                    <xsl:value-of select="error" />
                  </xsl:attribute>
                  <xsl:value-of select="error" />
                </span>
              </xsl:when>
              <xsl:otherwise>
                <span class="create-by">
                  <xsl:attribute name="title">
                    <xsl:value-of select="create_by" />
                  </xsl:attribute>
                  <xsl:value-of select="create_by" />
                </span>
                <span> | </span>
                <resource name="fres.TitleCreated" />&#160;<span>
                  <xsl:value-of select="create_on" />
                </span>
                <xsl:if test="not(provider_key)">
                  <span> | </span>
                  <resource name="fres.TitleFiles" />&#160;<span class="countFiles">
                    <xsl:value-of select="total_files" />
                  </span>
                  <span> | </span>
                  <resource name="fres.TitleSubfolders" />&#160;<span class="countFolders">
                    <xsl:value-of select="total_sub_folder" />
                  </span>
                </xsl:if>
                <input type="hidden" name="create_by">
                  <xsl:attribute name="value">
                    <xsl:value-of select="create_by" />
                  </xsl:attribute>
                </input>
                <input type="hidden" name="modified_by">
                  <xsl:attribute name="value">
                    <xsl:value-of select="modified_by" />
                  </xsl:attribute>
                </input>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:if>

          <input type="hidden" name="entry_data">
            <xsl:attribute name="value">
              {
              "entryType": "folder",
              "access": <xsl:value-of select="access" />,
              "create_by_id": "<xsl:value-of select="create_by_id" />",
              "create_on": "<xsl:value-of select="create_on" />",
              "id": "<xsl:value-of select="id" />",
              "modified_on": "<xsl:value-of select="modified_on" />",
              "shared": <xsl:value-of select="shared" />,
              "title": "<xsl:value-of select="title" />",
              "isnew": <xsl:value-of select="isnew" />,
              "shareable": <xsl:value-of select="shareable" />,
              "provider_key": "<xsl:value-of select="provider_key" />",
              "provider_id": "<xsl:value-of select="provider_id" />",
              "error": "<xsl:value-of select="error" />"
              <!--create_by: "<xsl:value-of select="create_by" />",    encode-->
              <!--modified_by: "<xsl:value-of select="modified_by" />",    encode-->
              <!--total_files: <xsl:value-of select="total_files" />,    dynamic-->
              <!--total_sub_folder: <xsl:value-of select="total_sub_folder" />,    dynamic-->
              }
            </xsl:attribute>
          </input>
        </div>
      </div>
      <div class="menu-small">
        <xsl:attribute name="title">
          <resource name="fres.TitleShowFolderActions" />
        </xsl:attribute>
      </div>
      <div class="share-action share-button">
        <xsl:attribute name="title">
          <resource name="fres.TitleShareFile" />
        </xsl:attribute>
        <span></span>
        <resource name="fres.Share" />
      </div>
      <div class="share-action access-button">
        <xsl:attribute name="title">
          <resource name="fres.TitleShareFile" />
        </xsl:attribute>
        <span></span>
        <resource name="fres.Access" />
      </div>
      <div class="entry-descr-compact">
        <xsl:choose>
          <xsl:when test="error">
            <span>
              <xsl:attribute name="title">
                <xsl:value-of select="error" />
              </xsl:attribute>
              <xsl:value-of select="error" />
            </span>
          </xsl:when>
          <xsl:otherwise>
            <span>
              <xsl:attribute name="title">
                <xsl:value-of select="create_by" />
              </xsl:attribute>
              <xsl:value-of select="create_by" />
            </span>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </li>
  </xsl:template>

  <xsl:template match="file">
    <li class="display-none" name="addRow">
      <xsl:attribute name="class">
        clearFix file-row new-file
        <xsl:if test="contains(file_status, 'IsEditing')">
          on-edit
        </xsl:if>
        <xsl:if test="provider_key">
          third-party-entry
        </xsl:if>
        <xsl:if test="error">
          error-entry
        </xsl:if>
        <xsl:if test="shared = 'true'">
          share-open
        </xsl:if>
      </xsl:attribute>
      <xsl:attribute name="data-id">file_<xsl:value-of select="id" /></xsl:attribute>
      <xsl:if test="spare_data">
        <xsl:attribute name="spare_data"><xsl:value-of select="spare_data" /></xsl:attribute>
      </xsl:if>
      <div class="checkbox">
        <input type="checkbox" >
          <xsl:attribute name="title">
            <resource name="fres.TitleSelectFile" />
          </xsl:attribute>
        </input>
      </div>
      <div class="thumb-file">
        <xsl:attribute name="title">
          <xsl:value-of select="title" />
        </xsl:attribute>
      </div>
      <div class="entry-info">
        <div class="entry-title">
          <div class="name">
            <a>
              <xsl:attribute name="title">
                <xsl:value-of select="title" />
              </xsl:attribute>
              <xsl:value-of select="title" />
            </a>
          </div>
          <a class="file-edit pencil">
            <xsl:attribute name="title">
              <resource name="fres.ButtonEdit" />
            </xsl:attribute>
          </a>
          <div class="file-editing pencil"></div>
          <div class="convert-action pencil display-none">
            <xsl:attribute name="title">
              <resource name="fres.ButtonConvertOpen" />
            </xsl:attribute>
          </div>
          <xsl:if test="version > 1">
            <div class="version">
              <xsl:attribute name="title">
                <resource name="fres.ShowVersions" />(<xsl:value-of select="version" />)
              </xsl:attribute>
              <resource name="fres.Version" />
              <xsl:value-of select="version" />
            </div>
          </xsl:if>
          <xsl:if test="contains(file_status, 'IsNew')">
            <div class="is-new">
              <xsl:attribute name="title">
                <resource name="fres.RemoveIsNew" />
              </xsl:attribute>
              <resource name="fres.IsNew" />
            </div>
          </xsl:if>
          <xsl:if test="contains(file_status, 'IsOriginal')">
            <div class="is-original">
              <resource name="fres.IsOriginal" />
            </div>
          </xsl:if>
        </div>
        <div class="entry-descr">

          <xsl:if test="create_by">
            <xsl:choose>
              <xsl:when test="error">
                <span>
                  <xsl:attribute name="title">
                    <xsl:value-of select="error" />
                  </xsl:attribute>
                  <xsl:value-of select="error" />
                </span>
              </xsl:when>
              <xsl:otherwise>
                <span class="create-by">
                  <xsl:attribute name="title">
                    <xsl:value-of select="create_by" />
                  </xsl:attribute>
                  <xsl:value-of select="create_by" />
                </span>
                <span> | </span>
                <span class="title-created">
                  <xsl:choose>
                    <xsl:when test="version > 1">
                      <resource name="fres.TitleModified" />
                    </xsl:when>
                    <xsl:otherwise>
                      <resource name="fres.TitleUploaded" />
                    </xsl:otherwise>
                  </xsl:choose>
                </span>&#160;<span class="modified-date">
                  <xsl:choose>
                    <xsl:when test="version > 1">
                      <xsl:value-of select="modified_on" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="create_on" />
                    </xsl:otherwise>
                  </xsl:choose>
                </span>
                <span> | </span>
                <span>
                  <xsl:value-of select="content_length" />
                </span>
                <input type="hidden" name="create_by">
                  <xsl:attribute name="value">
                    <xsl:value-of select="create_by" />
                  </xsl:attribute>
                </input>
                <input type="hidden" name="modified_by">
                  <xsl:attribute name="value">
                    <xsl:value-of select="modified_by" />
                  </xsl:attribute>
                </input>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:if>

          <input type="hidden" name="entry_data">
            <xsl:attribute name="value">
              {
              "entryType": "file",
              "access": <xsl:value-of select="access" />,
              "create_by_id": "<xsl:value-of select="create_by_id" />",
              "create_on": "<xsl:value-of select="create_on" />",
              "id": "<xsl:value-of select="id" />",
              "modified_on": "<xsl:value-of select="modified_on" />",
              "shared": <xsl:value-of select="shared" />,
              "title": "<xsl:value-of select="title" />",
              "content_length": "<xsl:value-of select="content_length" />",
              "file_status": "<xsl:value-of select="file_status" />",
              "version": <xsl:value-of select="version" />,
              "provider_key": "<xsl:value-of select="provider_key" />",
              "error": "<xsl:value-of select="error" />"
              <!--create_by: "<xsl:value-of select="create_by" />",    encode-->
              <!--modified_by: "<xsl:value-of select="modified_by" />",    encode-->
              }
            </xsl:attribute>
          </input>
        </div>
      </div>
      <div class="menu-small">
        <xsl:attribute name="title">
          <resource name="fres.TitleShowActions" />
        </xsl:attribute>
      </div>
      <div class="share-action share-button">
        <xsl:attribute name="title">
          <resource name="fres.TitleShareFile" />
        </xsl:attribute>
        <span></span>
        <resource name="fres.Share" />
      </div>
      <div class="share-action access-button">
        <xsl:attribute name="title">
          <resource name="fres.TitleShareFile" />
        </xsl:attribute>
        <span></span>
        <resource name="fres.Access" />
      </div>
      <div class="entry-descr-compact">
        <xsl:choose>
          <xsl:when test="error">
            <span>
              <xsl:attribute name="title">
                <xsl:value-of select="error" />
              </xsl:attribute>
              <xsl:value-of select="error" />
            </span>
          </xsl:when>
          <xsl:otherwise>
            <span>
              <xsl:attribute name="title">
                <xsl:value-of select="create_by" />
              </xsl:attribute>
              <xsl:value-of select="create_by" />
            </span>
          </xsl:otherwise>
        </xsl:choose>
      </div>
    </li>
  </xsl:template>

</xsl:stylesheet>