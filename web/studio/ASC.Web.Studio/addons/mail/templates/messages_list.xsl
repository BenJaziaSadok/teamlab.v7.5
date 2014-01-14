<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:t="http://tempuri.org/"
                xmlns:i="http://www.w3.org/2001/XMLSchema-instance"
                >
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/html" />

  <xsl:template match="/CommonHash">
    <xsl:apply-templates select="Messages"/>
  </xsl:template>

  <xsl:variable name="inbox_id">1</xsl:variable>
  <xsl:variable name="sent_id">2</xsl:variable>
  <xsl:variable name="drafts_id">3</xsl:variable>
  <xsl:variable name="trash_id">4</xsl:variable>
  <xsl:variable name="spam_id">5</xsl:variable>

  <!-- Messages list -->
  <xsl:template match="Messages">

    <div class="maintableWrapper" anchor="new" page="{/CommonHash/Page}" total_messages="{/CommonHash/TotalMessagesFiltered}">
      <div class="maintable">
        <xsl:apply-templates select="entry" mode="messages_list"/>
      </div>
    </div>
    
  </xsl:template>

  <xsl:variable name="More"><resource name="More" /></xsl:variable>
 
  <xsl:template match="entry" mode="messages_list">
    <div messageid="{Id}" date="{Date}" fromCRM="{IsFromCRM}" fromTL="{IsFromTL}">
      <xsl:if test="../../FolderId = $trash_id">
          <xsl:attribute name="PrevFolderId">
              <xsl:value-of select="RestoreFolderId" />
          </xsl:attribute>
      </xsl:if>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="IsNew = 'true'">message new</xsl:when>
          <xsl:otherwise>message</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <div class="td_checkbox">
        <div class="cb">
          <input type="checkbox" messageid="{Id}">
            <xsl:attribute name="title"><resource name="Select" /></xsl:attribute>
          </input>
        </div>
      </div>
      <div class="importance">
        <div messageid="{Id}">
          <xsl:attribute name="class">
            <xsl:choose>
              <xsl:when test="Important = 'true'">flag-message-true</xsl:when>
              <xsl:otherwise>flag-message</xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </div>
      </div>
      <div class="td_attach">
        <div>
          <xsl:attribute name="class">
            <xsl:choose>
              <xsl:when test="HasAttachments = 'true'">attachment-message has-attachment</xsl:when>
              <xsl:otherwise>attachment-message</xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </div>
      </div>
      <div class="td_datetime"><div><xsl:value-of select="DateDisplay" /></div></div>
      <div class="subject">
        <a href="#" class="message">
          <xsl:attribute name="_tags">
            <xsl:for-each select="TagIds/entry">
              <xsl:if test="position() &gt; 1">,</xsl:if>
              <xsl:value-of select="." />
            </xsl:for-each>
          </xsl:attribute>
          <div class="fromblock">

            <xsl:variable name="fromTo">
              <xsl:choose>
                <!-- Sent and Draft folder show To field -->
                <xsl:when test="../../FolderId = $sent_id or ../../FolderId = $drafts_id">
                  <xsl:choose>
                    <xsl:when test="To = ''">
                      <resource name="NoAddress" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="To" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="From" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:variable>

            <xsl:variable name="author">
              <xsl:value-of select="normalize-space(translate(substring-before($fromTo, ' &lt;'), '&quot;', ' '))"/>
            </xsl:variable>

            <span class="author">
                <xsl:choose>
                  <xsl:when test="string-length($author) > 0">
                    <xsl:value-of select="$author" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="$fromTo" />
                  </xsl:otherwise>
                </xsl:choose>
            </span>

            <span class="fromsender" onclick="return mailBox.setFromFilter(this);" mail="{substring-before(substring-after($fromTo, '&lt;'), '&gt;')}">
              <xsl:attribute name="alt">
                <resource name="AllLettersFrom" />
                <resource name="Space" />
                <xsl:value-of select="$fromTo" />
              </xsl:attribute>
            </span>
          </div>
          <xsl:for-each select="TagIds/entry[position() &lt; 4]">
            <xsl:variable name="labelid">
              <xsl:value-of select="." />
            </xsl:variable>
            <span>
              <xsl:attribute name="labelid">
                <xsl:value-of select="$labelid" />
              </xsl:attribute>
            </span>
          </xsl:for-each>
          <xsl:if test="count(TagIds/entry) &gt; 3">
              <span class="more" onclick="return mailBox.showMessageTags(this);">
                <xsl:value-of select="substring-before($More, '%1')"/>
                <xsl:value-of select="count(TagIds/entry)-3"/>
                <xsl:value-of select="substring-after($More, '%1')"/>
              </span>
          </xsl:if>
          <div class="subject_labels">
            <xsl:choose>
              <xsl:when test="Subject = ''">(<resource name="NoSubject" />)</xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="Subject" />
              </xsl:otherwise>
            </xsl:choose>
          </div>
        </a>
      </div>
    </div>
    
  </xsl:template>

</xsl:stylesheet>
