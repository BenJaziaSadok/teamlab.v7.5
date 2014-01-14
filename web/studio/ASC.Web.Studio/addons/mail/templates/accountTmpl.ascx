<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="accountTmpl" type="text/x-jquery-tmpl">
    <div  id="account_container" class="popup popupMailBox advanced">
      <div class="email">
          <div id="mail_EMailContainer" class="requiredField">
              <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
              <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.EMailLabel %></div>
              <div>
                  <input type="email" id="email" value="${email}" class="textEdit"/>
              </div>
              <div style="clear: both;" />
          </div>
      </div>
      <div class="name">
        <div id="mail_NameContainer" class="unrequiredField">
              <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.AccountLabel %></div>
              <div>
                  <input type="text" id="name" value="${name}" class="textEdit"/>
              </div>
        </div>
      </div>
      <div class="receive">
        <b><%: MailResource.ReceiveMailLabel %>:</b>
        <div id="mail_POPServerContainer" class="requiredField">
              <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
              <div class="headerPanelSmall" id="receive-server-header" style="font-weight: bold;">{{if imap}}<%: MailResource.ImapServerLabel %>{{else}}<%: MailResource.PopServerLabel %>{{/if}}</div>
              <div>
                  <select id="server-type" class="server-type">
                      <option value="pop" {{if typeof(imap)==='undefined' || !imap}}selected="selected"{{/if}}>POP</option>
                      <option value="imap" {{if typeof(imap)!=='undefined' && imap}}selected="selected"{{/if}}>IMAP</option>
                  </select>
                  <input type="text" id="server" value="${server}" class="textEdit receive-server"/>
              </div>
              <div style="clear: both;" />
        </div>
        <div id="mail_POPPortContainer" class="requiredField">
              <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
              <div class="headerPanelSmall" style="font-weight: bold; display: inline"><%: MailResource.Port %></div>
              <div class="headerPanelSmall" style="font-weight: bold; display: inline; margin-left: 34px;"><%: MailResource.AuthenticationType %></div>
              <div>
                 <input type="text" maxlength="5" class="portBox" id="port" value="{{if typeof(port)!=='undefined' && port !=''}}${port}{{else}}110{{/if}}" />
                 <select id="auth_type_in_sel" class="authenticationTypeBox">
                      <option value="1" {{if typeof(auth_type_in)==='undefined' || auth_type_in == 1}}selected="selected"{{/if}}><%: MailResource.SimplePasswordAuthentication %></option>
                      <option value="4" {{if typeof(auth_type_in)!=='undefined' && auth_type_in == 4}}selected="selected"{{/if}}><%: MailResource.EncryptedPaswordAuthentication %></option>
                  </select>
              </div>
              <div style="clear: both;" />
        </div>
        <div id="mail_POPLoginContainer" class="requiredField">
              <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
              <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.Login %></div>
              <div>
                  <input type="text" id="account" value="${account}" class="textEdit"/>
              </div>
              <div style="clear: both;" />
        </div>
        <div id="mail_POPPasswordContainer" class="requiredField">
              <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
              <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.Password %></div>
              <div>
                  <input type="password" id="password" value="${password}" class="textEdit"/>
              </div>
              <div style="clear: both;" />
        </div>
        <div class="ssl">
            <div id="use_incomming_ssl_label" class="headerPanelSmall">{{if typeof(imap)==='undefined' || !imap}}<%: MailResource.UsePOP3SSL %>{{else}}<%: MailResource.UseImapSSL %>{{/if}}</div>
            <select id="incoming_encryption_type" class="encryptionTypeSelect">
                <option value="0" {{if typeof(incoming_encryption_type)==='undefined' || incoming_encryption_type == 0}}selected="selected"{{/if}}><%: MailResource.NoEcncryptionNeeded %></option>
                <option value="1" {{if typeof(incoming_encryption_type)!=='undefined' && incoming_encryption_type == 1}}selected="selected"{{/if}}>SSL</option>
                <option value="2" {{if typeof(incoming_encryption_type)!=='undefined' && incoming_encryption_type == 2}}selected="selected"{{/if}}>STARTTLS</option>
            </select>
        </div>
      </div>
      <div class="send">
        <b><%: MailResource.SendMail %></b>
          <div id="mail_SMTPServerContainer" class="requiredField">
              <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
              <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.SmtpServerLabel %></div>
              <div>
                  <input type="text" id="smtp_server" value="${smtp_server}" class="textEdit"/>
              </div>
              <div style="clear: both;" />
          </div>
          <div style="height:60px;">
          <div class="port_auth">
            <div id="mail_SMTPPortContainer" class="requiredField">
                  <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
                  <div class="headerPanelSmall" style="font-weight: bold; display: inline"><%: MailResource.Port %></div>
                  <div class="headerPanelSmall" style="font-weight: bold; display: inline; margin-left: 34px;"><%: MailResource.AuthenticationType %></div>
                  <div>
                      <input type="text" maxlength="5" class="portBox" id="smtp_port" value="{{if typeof(smtp_port)!=='undefined' && smtp_port !=''}}${smtp_port}{{else}}25{{/if}}" />
                      <div class="smtp_auth">
                          <select id="auth_type_smtp_sel" class="authenticationTypeBox">
                                <option value="0" {{if typeof(auth_type_smtp)==='undefined' || auth_type_smtp == 0}}selected="selected"{{/if}}><%: MailResource.NonePasswordAuthentication %></option>
                                <option value="1" {{if typeof(auth_type_smtp)!=='undefined' && auth_type_smtp == 1}}selected="selected"{{/if}}><%: MailResource.SimplePasswordAuthentication %></option>
                                <option value="4" {{if typeof(auth_type_smtp)!=='undefined' && auth_type_smtp == 4}}selected="selected"{{/if}}><%: MailResource.EncryptedPaswordAuthentication %></option>
                          </select>
                      </div>
                  </div>
            </div>
          </div>
          </div>
          <div style="clear: both;" />
          <div id="mail_SMTPLoginContainer" class="{{if true!=smtp_auth}} unrequiredField {{else}} requiredField {{/if}}">
                <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
                <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.Login %></div>
                <div>
                    <input type="text" id="smtp_account" value="${smtp_account}" class="textEdit"{{if true!=smtp_auth}} disabled="true"{{/if}}/>
                </div>
                <div style="clear: both;" />
          </div>
          <div id="mail_SMTPPasswordContainer" class="{{if true!=smtp_auth}} unrequiredField {{else}} requiredField {{/if}}">
                <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
                <div class="headerPanelSmall" style="font-weight: bold;"><%: MailResource.Password %></div>
                <div>
                    <input type="password" id="smtp_password" value="${smtp_password}" class="textEdit"{{if true!=smtp_auth}} disabled="true"{{/if}}/>
                </div>
                <div style="clear: both;" />
          </div>
          <div class="ssl">
            <div class="headerPanelSmall"><%: MailResource.UseSmtpSSL %></div>
            <select id="outcoming_encryption_type" class="encryptionTypeSelect">
              <option value="0" {{if typeof(outcoming_encryption_type)==='undefined' || outcoming_encryption_type == 0}}selected="selected"{{/if}}><%: MailResource.NoEcncryptionNeeded %></option>
              <option value="1" {{if typeof(outcoming_encryption_type)!=='undefined' && outcoming_encryption_type == 1}}selected="selected"{{/if}}>SSL</option>
              <option value="2" {{if typeof(outcoming_encryption_type)!=='undefined' && outcoming_encryption_type == 2}}selected="selected"{{/if}}>STARTTLS</option>
            </select>
          </div>
        </div>
      <div class="mail-limit">
          <label class="checkbox">
              <input type="checkbox" id="mail-limit"{{if typeof(restrict)!=='undefined' && restrict}}value="1" checked="true"{{else}}value="0"{{/if}}/>
              <span><%: MailResource.MailLimitLabel %></span>
          </label>
      </div>
      <div class="progressContainer" style="padding-top: 7px;">
        <div class="loader" style="display: none;"></div>
      </div>
      <div class="errorsContainer"></div>
      <div class="buttons new-account">
        <button class="button blue" id="save" type="button"><%: MailResource.SaveBtnLabel %></button>
        <button class="button gray cancel" id="cancel" type="button"><%: MailScriptResource.CancelBtnLabel %></button>
        <a class="anchorLinkButton" id="getDefaultSettings"><%: MailResource.GetDefaultSettingsLinkLabel %></a>
      </div>
    </div>
</script>