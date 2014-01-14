<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Backup.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.Backup" %>
<%@ Import Namespace="ASC.Core.Tenants" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="Resources" %>

<asp:PlaceHolder ID="backupContainer" runat="server" />
<div id="backupSettings" class="clearFix <%=EnableBackup ? "" : "disable" %>">
    <div class="settings-block">
        <div class="header-base clearFix">
            <%= Resource.DataBackup %>
        </div>
        <div>
            <div>
                <%= string.Format(Resource.BackupDesc, "<br/>") %>
            </div>
            <div>
                <div>
                    <a id="create_btn" class="button blue <%= EnableBackup ? "" : " disable" %>" href="javascript:void(0);"
                        onclick="<%= EnableBackup ? "BackupManager.CreateBackup();" : "" %>">
                        <%= Resource.PerformBackupButton %>
                    </a>
                </div>

                <div style="padding:20px 0 0 0;">
                    <div id="progressbar_container" style="display: none; margin-bottom: 10px;">
                        <div class="asc-progress-wrapper">
                            <div class="asc-progress-value"></div>
                        </div>
                        <div style="padding-top: 2px;" class="text-medium-describe">
                            <%= Resource.CreatingBackup %>
                            <span id="backup_percent"></span>
                        </div>
                    </div>
                    <div id="backup_error" class="errorText" style="display:none;"></div>
                    <div id="backup_ready" class="display-none">
                        <div id="backup_link" class="longWordsBreak"></div>
                        <%= string.Format(Resource.BackupReadyText, "<p>", "</p><p>", "</p>") %>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="settings-help-block">
        <% if (!EnableBackup) { %>
        <p>
            <%= Resources.Resource.ErrorNotAllowedOption %></p>
        <a href="<%= TenantExtra.GetTariffPageLink() %>" target="_blank">
            <%= Resources.Resource.ViewTariffPlans %></a>
        <% } else { %>
        <p><%=String.Format(Resource.HelpAnswerDataBackup, "<br />", "<b>", "</b>")%></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#CreatingBackup_block" target="_blank"><%= Resource.LearnMore %></a>
        <% } %>
    </div>
</div>
<div class="clearFix">
    <div class="settings-block">
        <div class="header-base clearFix">
            <%=Resource.AccountDeactivation%>
        </div>
        <div>
            <div>
                <%=Resource.DeactivationDesc%>
            </div>
            <div>
                <a class="button blue" href="javascript:void(0);" onclick="BackupManager.Deactivate()" style="width: 100px;"><%=Resource.DeactivateButton%></a>
            </div>
            <p id="deativate_sent" class="display-none"></p>
        </div>
    </div>
    <div class="settings-help-block">
        <p><%=String.Format(Resource.HelpAnswerAccountDeactivation, "<br />", "<b>", "</b>")%></p>
    </div>
</div>
<div class="clearFix">
    <div class="settings-block">
        <div class="header-base">
            <%=Resource.AccountDeletion%>
        </div>
        <div>
            <div><%=Resource.DeletionDesc%></div>
            <div>
                <a class="button blue" href="javascript:void(0);" onclick="BackupManager.Delete()" style="width: 100px;"><%=Resource.DeleteButton%></a>
            </div>
            <p id="delete_sent" class="display-none"></p>
        </div>
    </div>
</div>
