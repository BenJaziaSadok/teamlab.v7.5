/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

using ASC.Notify.Model;
using ASC.Notify.Patterns;
using System;

namespace ASC.Web.Studio.Core.Notify
{
    static class Constants
    {
        public static string TagUserName = "UserName";
        public static string TagUserEmail = "UserEmail";
        public static string TagSubject = "Subject";
        public static string TagBody = "Body";
        public static string TagMyStaffLink = "MyStaffLink";
        public static string TagSettingsLink = "SettingsLink";
        public static string TagInviteLink = "InviteLink";
        public static string TagDate = "Date";
        public static string TagIP = "IP";
        public static string TagPassword = "Password";
        public static string TagWebStudioLink = "WebStudioLink";
        public static string TagAuthor = "Author";
        public static string TagAuthorLink = "AuthorLink";
        public static string TagProfileChanges = "ProfileChanges";
        public static string TagIsPasswordChange = "IsPasswordChange";
        public static string TagActivities = "Activities";
        public static string TagBackupUrl = "BackupUrl";
        public static string TagBackupHours = "BackupHours";

        public static string TagDeactivateUrl = "DeactivateUrl";
        public static string TagActivateUrl = "ActivateUrl";
        public static string TagDeleteUrl = "DeleteUrl";
        public static string TagOwnerName = "OwnerName";
        public static string TagRegionName = "RegionName";


        public static INotifyAction ActionAdminNotify = new NotifyAction("admin_notify", "admin notifications");
        public static INotifyAction ActionSelfProfileUpdated = new NotifyAction("self_profile_updated", "self profile updated");
        public static INotifyAction ActionUserHasJoin = new NotifyAction("user_has_join", "user has join");
        public static INotifyAction ActionUserMessageToAdmin = new NotifyAction("for_admin_notify", "for_admin_notify");

        public static INotifyAction ActionYouAddedLikeGuest = new NotifyAction("you_added_like_guest", "You added like guest");
        public static INotifyAction ActionYouAddedAfterInvite = new NotifyAction("you_added_after_invite", "You added after invite");
        public static INotifyAction ActionYourProfileUpdated = new NotifyAction("profile_updated", "profile updated");
        public static INotifyAction ActionSendPassword = new NotifyAction("send_pwd", "send password");
        public static INotifyAction ActionInviteUsers = new NotifyAction("invite", "invite users");
        public static INotifyAction ActionJoinUsers = new NotifyAction("join", "join users");
        public static INotifyAction ActionSendWhatsNew = new NotifyAction("send_whats_new", "send whats new");
        public static INotifyAction ActionBackupCreated = new NotifyAction("backup_created", "backup created");
        public static INotifyAction ActionPortalDeactivate = new NotifyAction("portal_deactivate", "portal deactivate");
        public static INotifyAction ActionPortalDelete = new NotifyAction("portal_delete", "portal delete");
        public static INotifyAction ActionProfileDelete = new NotifyAction("profile_delete", "profile_delete");
        public static INotifyAction ActionDnsChange = new NotifyAction("dns_change", "dns_change");

        public static INotifyAction ActionConfirmOwnerChange = new NotifyAction("owner_confirm_change", "owner_confirm_change");
        public static INotifyAction ActionActivateUsers = new NotifyAction("activate", "activate");
        public static INotifyAction ActionActivateGuests = new NotifyAction("activate_guest", "activate_guest");
        public static INotifyAction ActionActivateUsersYourDocs = new NotifyAction("activate_your_docs", "activate_your_docs");
        public static INotifyAction ActionActivateEmail = new NotifyAction("activate_email", "activate_email");
        public static INotifyAction ActionEmailChange = new NotifyAction("change_email", "change_email");
        public static INotifyAction ActionPasswordChange = new NotifyAction("change_password", "change_password");
        public static INotifyAction ActionPasswordChanged = new NotifyAction("change_pwd", "password changed");
        public static INotifyAction ActionPhoneChange = new NotifyAction("change_phone", "change_phone");
        public static INotifyAction ActionCongratulations = new NotifyAction("congratulations");
        public static INotifyAction ActionMigrationPortalStart = new NotifyAction("migration_start", "migration start");
        public static INotifyAction ActionMigrationPortalSuccess = new NotifyAction("migration_success", "migration success");
        public static INotifyAction ActionMigrationPortalError = new NotifyAction("migration_error", "migration error");
        public static INotifyAction ActionMigrationPortalServerFailure = new NotifyAction("migration_server_failure", "migration_server_failure");

        public static INotifyAction ActionAfterCreation1 = new NotifyAction("after_creation1");
        public static INotifyAction ActionAfterCreation2 = new NotifyAction("after_creation2");
        public static INotifyAction ActionAfterCreation3 = new NotifyAction("after_creation3");
        public static INotifyAction ActionAfterCreation4 = new NotifyAction("after_creation4");
        public static INotifyAction ActionTariffWarningTrial = new NotifyAction("tariff_warning_trial");
        public static INotifyAction ActionTariffWarningTrial2 = new NotifyAction("tariff_warning_trial2");
        public static INotifyAction ActionTariffWarningTrial3 = new NotifyAction("tariff_warning_trial3");

        public static ITagValue TagNoteStart
        {
            get { return new TagValue("NoteStart", "<table cellpadding=\"0\" cellspacing=\"0\" style=\"margin: 20px 0 0; border-spacing: 0; empty-cells: show; width: 540px; font-size: 14px;\"><tr><td style=\"width: 21px; background-color:#9bd1f0;\"></td><td style=\"width: 5px;\"></td><td style=\"width: 5px;background-color:#38a4e2;\"></td><td style=\"background-color:#e6eff4; padding:10px;\">"); }
        }

        public static ITagValue TagNoteEnd
        {
            get { return new TagValue("NoteEnd", "</td></tr></table>"); }
        }

        public static ITagValue TagBlueButton(string resourceName, string url)
        {
            Func<string> action = () =>
            {
                return "<table style=\"background :url(' http://cdn.teamlab.com/media/newsletters/images/button-blue.jpg') 0 0 no-repeat #00576a; width:279px; height:55px; text-align: center; margin: 0 auto; padding: 0 ;\"><tbody><tr style=\"padding:0;\"><td style=\"padding: 6px;margin: 0;padding:0;\"><a style=\"display: block; text-shadow: 1px 1px 5px #00576a; font-size: 20px; color:#fff; padding: 13px 0 0; width: 274px; height: 38px; text-decoration: none;\" href=\"$ButtonUrl\">$ButtonText</a></td></tr></tbody></table>"
                    .Replace("$ButtonText", WebstudioPatternResource.ResourceManager.GetString(resourceName))
                    .Replace("$ButtonUrl", url);
            };
            return new TagActionValue("BlueButton", action);
        }

        public static ITagValue TagGreenButton(string resourceName, string url)
        {
            Func<string> action = () =>
            {
                return "<table style=\"background :url(' http://cdn.teamlab.com/media/newsletters/images/button-green.jpg') 0 0 no-repeat #436400; width: 484px; height: 60px; text-align: center; text-shadow: 1px 1px 5px #436400; font-size: 20px; color:#fff; margin: 25px auto ;\"><tbody><tr style=\"padding:0;\"><td style=\"padding: 6px;margin: 0;padding:0;\"><a style=\"display: block; text-shadow: 1px 1px 5px #436400; font-size: 20px; color:#fff; padding: 13px 0 0; width: 477px; height: 40px; text-decoration: none;\" href=\"$ButtonUrl\">$ButtonText</a></td></tr></tbody></table>"
                    .Replace("$ButtonText", WebstudioPatternResource.ResourceManager.GetString(resourceName))
                    .Replace("$ButtonUrl", url);
            };
            return new TagActionValue("GreenButton", action);
        }

        public static ITagValue TagTableTop()
        {
            return new TagValue("TableItemsTop", "<table cellpadding=\"0\" cellspacing=\"0\" style=\"margin: 20px 0 0; border-spacing: 0; empty-cells: show; width: 540px; font-size: 14px;\">");
        }

        public static ITagValue TagTableBottom()
        {
            return new TagValue("TableItemsBtm", "</table>");
        }

        public static ITagValue TagTableItem(int number, string resourceName, string url, string img, string comment)
        {
            Func<string> action = () =>
            {
                return "<tr><td style=\"padding: 5px 0; width: 175px;\"><img style=\"border: 0; border-left: 5px solid #38A4E2; padding: 0 5px; width: 190px; height: 80px;\" alt=\"$TableItemText\" src=\"$TableItemImg\"/></td><td style=\"vertical-align: middle; font-size: 14px; width: 420px;\"><a target=\"_blank\" style=\"color: #0078bd; font-weight: bold; font-family: Arial; font-size: 14px;\" href=\"$TableItemUrl\">$TableItemText</a><br >$TableItemComment</td></tr>"
                    .Replace("$TableItemText", WebstudioPatternResource.ResourceManager.GetString(resourceName))
                    .Replace("$TableItemImg", img ?? string.Empty)
                    .Replace("$TableItemUrl", url ?? string.Empty)
                    .Replace("$TableItemComment", string.IsNullOrWhiteSpace(comment) ? string.Empty : WebstudioPatternResource.ResourceManager.GetString(comment));
            };
            return new TagActionValue("TableItem" + number, action);
        }

        public static ITagValue TagTableItem(int number)
        {
            switch (number)
            {
                case 1: return TagTableItem(number, "ItemIntegrateDocs", "http://helpcenter.teamlab.com/tipstricks/add-resource.aspx?utm_medium=newsletter&utm_source=invite_user&utm_campaign=email", "http://cdn.teamlab.com/media/newsletters/images/integrate_documents.jpg", null);
                case 2: return TagTableItem(number, "ItemImportProjects", "http://helpcenter.teamlab.com/tipstricks/basecamp-import.aspx?utm_medium=newsletter&utm_source=invite_user&utm_campaign=email", "http://cdn.teamlab.com/media/newsletters/images/import_projects.jpg", null);
                case 3: return TagTableItem(number, "ItemUploadCrm", "http://helpcenter.teamlab.com/guides/import-contacts.aspx?utm_medium=newsletter&utm_source=invite_user&utm_campaign=email", "http://cdn.teamlab.com/media/newsletters/images/csv.jpg", null);
                default: throw new NotSupportedException();
            }
        }


        private class TagActionValue : ITagValue
        {
            private readonly Func<string> action;

            public string Tag
            {
                get;
                private set;
            }

            public object Value
            {
                get { return action(); }
            }

            public TagActionValue(string name, Func<string> action)
            {
                Tag = name;
                this.action = action;
            }
        }
    }

    public sealed class CommonTags
    {
        public static readonly string VirtualRootPath = "__VirtualRootPath";

        public static readonly string ProductID = "__ProductID";

        public static readonly string ModuleID = "__ModuleID";

        public static readonly string ProductUrl = "__ProductUrl";

        public static readonly string DateTime = "__DateTime";

        public static readonly string AuthorID = "__AuthorID";

        public static readonly string AuthorName = "__AuthorName";

        public static readonly string SendFrom = "MessageFrom";

        public static readonly string AuthorUrl = "__AuthorUrl";

        public static readonly string Helper = "__Helper";

        public static readonly string RecipientID = "__RecipientID";

        public static readonly string RecipientSubscriptionConfigURL = "RecipientSubscriptionConfigURL";

        public static readonly string Priority = "Priority";
    }
}
