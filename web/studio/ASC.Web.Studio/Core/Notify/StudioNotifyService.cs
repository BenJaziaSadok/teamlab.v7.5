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

using ASC.Common.Security.Authentication;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Feed;
using ASC.Feed.Data;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using log4net;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace ASC.Web.Studio.Core.Notify
{
    public class StudioNotifyService
    {
        private readonly INotifyClient client;
        internal readonly StudioNotifySource source;

        private static string EMailSenderName { get { return ASC.Core.Configuration.Constants.NotifyEMailSenderSysName; } }
        private static string[] AllSenderNames { get { return WorkContext.AvailableNotifySenders; } }

        public static StudioNotifyService Instance
        {
            get;
            private set;
        }


        static StudioNotifyService()
        {
            Instance = new StudioNotifyService();
        }


        private StudioNotifyService()
        {
            source = new StudioNotifySource();
            client = WorkContext.NotifyContext.NotifyService.RegisterClient(source);
        }

        public void RegisterSendMethod()
        {
            client.RegisterSendMethod(SendMsgWhatsNew, "0 0 * ? * *"); // every hour
            client.RegisterSendMethod(SendTariffWarnings, "0 0 5 ? * *"); // 5am every day
        }


        public void SendMsgWhatsNew(DateTime scheduleDate)
        {
            if (WebItemManager.Instance.GetItemsAll<IProduct>().Count == 0)
            {
                return;
            }

            var log = LogManager.GetLogger("ASC.Notify.WhatsNew");
            log.Info("Start send whats new.");

            var products = WebItemManager.Instance.GetItemsAll().ToDictionary(p => p.GetSysName());

            foreach (var tenantid in GetChangedTenants(scheduleDate))
            {
                try
                {
                    var tenant = CoreContext.TenantManager.GetTenant(tenantid);
                    if (tenant == null ||
                        tenant.Status != TenantStatus.Active ||
                        !TimeToSendWhatsNew(TenantUtil.DateTimeFromUtc(tenant, scheduleDate)) ||
                        TariffState.NotPaid <= CoreContext.PaymentManager.GetTariff(tenantid).State)
                    {
                        continue;
                    }

                    CoreContext.TenantManager.SetCurrentTenant(tenant);

                    log.InfoFormat("Start send whats new in {0} ({1}).", tenant.TenantDomain, tenantid);
                    foreach (var user in CoreContext.UserManager.GetUsers())
                    {
                        if (!IsSubscribeToWhatsNew(user))
                        {
                            continue;
                        }

                        SecurityContext.AuthenticateMe(CoreContext.Authentication.GetAccountByID(user.ID));
                        Thread.CurrentThread.CurrentCulture = user.GetCulture();
                        Thread.CurrentThread.CurrentUICulture = user.GetCulture();

                        var feeds = FeedAggregateDataProvider.GetFeeds(new FeedApiFilter
                        {
                            From = scheduleDate.Date.AddDays(-1),
                            To = scheduleDate.Date.AddSeconds(-1),
                            Max = 100,
                        });

                        var activities = feeds
                            .Select(f => f.ToFeedMin())
                            .SelectMany(f =>
                            {
                                if (f.Comments == null || !f.Comments.Any())
                                {
                                    return new[] { f };
                                }
                                var comment = f.Comments.Last().ToFeedMin();
                                comment.Id = f.Id;
                                comment.Product = f.Product;
                                comment.ItemUrl = f.ItemUrl;
                                if (f.Date < scheduleDate.Date.AddDays(-1)) 
                                {
                                    return new[] { comment };
                                }
                                return new[] { f, comment };
                            })
                            .Where(f => products.ContainsKey(f.Product) && !f.Id.StartsWith("participant"))
                            .GroupBy(f => products[f.Product])
                            .ToDictionary(g => g.Key.Name, g => g.Select(f => new WhatsNewUserActivity
                            {
                                Date = TenantUtil.DateTimeFromUtc(tenant, f.Date),
                                UserName = f.Author != null && f.Author.UserInfo != null ? f.Author.UserInfo.DisplayUserName() : string.Empty,
                                UserAbsoluteURL = f.Author != null && f.Author.UserInfo != null ? CommonLinkUtility.GetFullAbsolutePath(f.Author.UserInfo.GetUserProfilePageURL()) : string.Empty,
                                Title = f.Title,
                                URL = CommonLinkUtility.GetFullAbsolutePath(f.ItemUrl),
                                BreadCrumbs = GetBreadCrumbs(products, f),
                            }).ToList());

                        if (0 < activities.Count)
                        {
                            log.InfoFormat("Send whats new to {0}", user.Email);
                            client.SendNoticeAsync(
                                Constants.ActionSendWhatsNew, null, user, null,
                                new TagValue(Constants.TagActivities, activities),
                                new TagValue(Constants.TagDate, DateToString(scheduleDate.AddDays(-1), user.GetCulture())),
                                new TagValue(CommonTags.Priority, 1)
                            );
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error(error);
                }
            }
        }

        private IList<string> GetBreadCrumbs(Dictionary<string, IWebItem> products, FeedMin f)
        {
            var result = new List<string>();
            if (f.Product == "projects")
            {
                if (f.Id.StartsWith("taskComment"))
                {
                    result.Add(f.AdditionalInfo2);
                }
                else if (f.Module == "projects")
                {
                    result.Add(f.Title);
                }
                else
                {
                    result.Add(f.AdditionalInfo);
                }
            }
            else if (f.Product == "community")
            {
                if (f.Module == "blogs" && products.ContainsKey("community-blogs"))
                {
                    result.Add(products["community-blogs"].Name);
                }
                else if (f.Module == "forums" && products.ContainsKey("community-forum"))
                {
                    result.Add(products["community-forum"].Name);
                }
                else if (f.Module == "bookmarks" && products.ContainsKey("community-bookmarking"))
                {
                    result.Add(products["community-bookmarking"].Name);
                }
                else if (f.Module == "events" && products.ContainsKey("community-news"))
                {
                    result.Add(products["community-news"].Name);
                }
            }
            if (result.Count == 0)
            {
                result.Add(string.Empty);
            }
            return result;
        }

        private IEnumerable<int> GetChangedTenants(DateTime date)
        {
            return new FeedAggregateDataProvider().GetTenants(new TimeInterval(date.Date.AddDays(-1), date.Date.AddSeconds(-1)));
        }

        private bool TimeToSendWhatsNew(DateTime currentTime)
        {
            var hourToSend = 7;
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["web.whatsnew-time"]))
            {
                var hour = 0;
                if (int.TryParse(WebConfigurationManager.AppSettings["web.whatsnew-time"], out hour))
                {
                    hourToSend = hour;
                }
            }
            return currentTime.Hour == hourToSend;
        }

        private string DateToString(DateTime d, CultureInfo c)
        {
            return d.ToString(c.TwoLetterISOLanguageName == "ru" ? "d MMMM" : "M", c);
        }

        public bool IsSubscribeToWhatsNew(Guid userID)
        {
            return IsSubscribeToWhatsNew(ToRecipient(userID));
        }

        private bool IsSubscribeToWhatsNew(IRecipient recipient)
        {
            if (recipient == null) return false;
            return source.GetSubscriptionProvider().IsSubscribed(Constants.ActionSendWhatsNew, recipient, null);
        }

        public void SubscribeToWhatsNew(Guid userID, bool subscribe)
        {
            var recipient = ToRecipient(userID);
            if (recipient != null)
            {
                if (subscribe)
                {
                    source.GetSubscriptionProvider().Subscribe(Constants.ActionSendWhatsNew, null, recipient);
                }
                else
                {
                    source.GetSubscriptionProvider().UnSubscribe(Constants.ActionSendWhatsNew, null, recipient);
                }
            }
        }

        public bool IsSubscribeToAdminNotify(Guid userID)
        {
            return source.GetSubscriptionProvider().IsSubscribed(Constants.ActionAdminNotify, ToRecipient(userID), null);
        }

        public void SubscribeToAdminNotify(Guid userID, bool subscribe)
        {
            var recipient = source.GetRecipientsProvider().GetRecipient(userID.ToString());
            if (recipient != null)
            {
                if (subscribe)
                {
                    source.GetSubscriptionProvider().Subscribe(Constants.ActionAdminNotify, null, recipient);
                }
                else
                {
                    source.GetSubscriptionProvider().UnSubscribe(Constants.ActionAdminNotify, null, recipient);
                }
            }
        }


        public void SendMsgToAdminAboutProfileUpdated()
        {
            client.SendNoticeAsync(Constants.ActionSelfProfileUpdated, null, null);
        }

        public void SendMsgToAdminFromNotAuthUser(string email, string message)
        {
            client.SendNoticeAsync(Constants.ActionUserMessageToAdmin, null, null, new TagValue(Constants.TagBody, message), new TagValue(Constants.TagUserEmail, email));
        }


        public void UserPasswordChange(UserInfo userInfo)
        {
            var email = userInfo.Email;
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email + ConfirmType.PasswordChange.ToString());
            var inviteUrl = CommonLinkUtility.GetFullAbsolutePath(string.Format("~/confirm.aspx?type={2}&email={0}&key={1}",
                HttpUtility.UrlEncode(email),
                validationKey,
                ConfirmType.PasswordChange.ToString()));

            client.SendNoticeToAsync(
                        Constants.ActionPasswordChange,
                        null,
                        RecipientFromEmail(new[] { email }, false),
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagUserName, SecurityContext.IsAuthenticated ? DisplayUserSettings.GetFullUserName(SecurityContext.CurrentAccount.ID) : ((HttpContext.Current != null) ? HttpContext.Current.Request.UserHostAddress : null)),
                        new TagValue(Constants.TagInviteLink, inviteUrl),
                        new TagValue(Constants.TagBody, string.Empty),
                        new TagValue("UserDisplayName", userInfo.DisplayUserName()));
        }

        public void UserPasswordChanged(Guid userID, string password)
        {
            var author = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            var user = CoreContext.UserManager.GetUsers(userID);

            ISendInterceptor initInterceptor = null;
            if (!ASC.Core.Users.Constants.LostUser.Equals(author))
            {
                initInterceptor = new InitiatorInterceptor(new[] { ToRecipient(author.ID) });
                client.AddInterceptor(initInterceptor);
            }

            client.SendNoticeToAsync(
                           Constants.ActionPasswordChanged,
                           null,
                           new[] { ToRecipient(user.ID) },
                           new[] { EMailSenderName },
                           null,
                           new TagValue(Constants.TagUserName, user.DisplayUserName()),
                           new TagValue(Constants.TagUserEmail, user.Email),
                           new TagValue(Constants.TagMyStaffLink, GetMyStaffLink()),
                           new TagValue(Constants.TagPassword, password));

            if (initInterceptor != null)
            {
                client.RemoveInterceptor(initInterceptor.Name);
            }
        }

        public void SendUserPassword(UserInfo ui, string password)
        {
            client.SendNoticeToAsync(
                        Constants.ActionSendPassword,
                        null,
                        new[] { ToRecipient(ui.ID) },
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagPassword, password),
                        new TagValue(Constants.TagUserName, ui.DisplayUserName()),
                        new TagValue(Constants.TagUserEmail, ui.Email),
                        new TagValue(Constants.TagMyStaffLink, GetMyStaffLink()),
                        new TagValue(Constants.TagAuthor, (HttpContext.Current != null) ? HttpContext.Current.Request.UserHostAddress : null));
        }

        public void UserInfoUpdated(UserInfo prevUser, UserInfo newUser, string password)
        {
            var author = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            ISendInterceptor interceptor = null;
            if (!ASC.Core.Users.Constants.LostUser.Equals(author))
            {
                interceptor = new InitiatorInterceptor(new[] { ToRecipient(author.ID) });
                client.AddInterceptor(interceptor);
            }

            client.SendNoticeToAsync(
                           Constants.ActionYourProfileUpdated,
                           null,
                           new[] { ToRecipient(newUser.ID) },
                           AllSenderNames,
                           null,
                           new TagValue(Constants.TagProfileChanges, GetUserInfoChanges(prevUser, newUser)),
                           new TagValue(Constants.TagUserName, newUser.DisplayUserName()),
                           new TagValue(Constants.TagMyStaffLink, GetMyStaffLink()),
                           new TagValue(Constants.TagPassword, password),
                           new TagValue(Constants.TagIsPasswordChange, password != null));

            if (interceptor != null)
            {
                client.RemoveInterceptor(interceptor.Name);
            }
        }

        public void SendEmailChangeInstructions(UserInfo user, string email)
        {
            client.SendNoticeToAsync(
                        Constants.ActionEmailChange,
                        null,
                        RecipientFromEmail(new[] { email }, false),
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagUserName, SecurityContext.IsAuthenticated ? DisplayUserSettings.GetFullUserName(SecurityContext.CurrentAccount.ID) : ((HttpContext.Current != null) ? HttpContext.Current.Request.UserHostAddress : null)),
                        new TagValue(Constants.TagInviteLink, GenerateConfirmUrl(email, ConfirmType.EmailChange)),
                        new TagValue(Constants.TagBody, string.Empty),
                        new TagValue("UserDisplayName", string.Empty));
        }

        public void SendEmailActivationInstructions(UserInfo user, string email)
        {
            client.SendNoticeToAsync(
                        Constants.ActionActivateEmail,
                        null,
                        RecipientFromEmail(new[] { email }, false),
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagUserName, SecurityContext.IsAuthenticated ? DisplayUserSettings.GetFullUserName(SecurityContext.CurrentAccount.ID) : ((HttpContext.Current != null) ? HttpContext.Current.Request.UserHostAddress : null)),
                        new TagValue(Constants.TagInviteLink, GenerateConfirmUrl(email, ConfirmType.EmailActivation)),
                        new TagValue(Constants.TagBody, string.Empty),
                        new TagValue("UserDisplayName", (user.DisplayUserName() ?? string.Empty).Trim()));
        }

        public void SendMsgMobilePhoneChange(UserInfo userInfo)
        {
            client.SendNoticeToAsync(
                Constants.ActionPhoneChange,
                null,
                RecipientFromEmail(new[] { userInfo.Email.ToLower() }, false),
                new[] { EMailSenderName },
                null,
                new TagValue(Constants.TagInviteLink, GenerateConfirmUrl(userInfo.Email.ToLower(), ConfirmType.PhoneActivation)),
                new TagValue("UserDisplayName", userInfo.DisplayUserName()));
        }


        public void UserHasJoin()
        {
            client.SendNoticeAsync(Constants.ActionUserHasJoin, null, null);
        }

        public void InviteUsers(string emailList, string inviteMessage, bool join, EmployeeType emplType)
        {
            if (string.IsNullOrWhiteSpace(emailList))
            {
                return;
            }

            foreach (var email in emailList.Split(new[] { " ", ",", ";", Environment.NewLine, "\n", "\n\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                SendInvite(new UserInfo() { Email = email }, inviteMessage, join, emplType);
            }
        }

        private void SendInvite(UserInfo user, string inviteMessage, bool join, EmployeeType emplType)
        {
            var email = user.Email;
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email + ConfirmType.EmpInvite.ToString() + (int)emplType);
            var inviteUrl = String.Format("~/confirm.aspx?type={2}&email={0}&firstname={4}&lastname={5}&key={1}&emplType={3}&uid={6}",
                                            HttpUtility.UrlEncode(email),
                                            validationKey,
                                            ConfirmType.EmpInvite.ToString(),
                                            (int)emplType,
                                            HttpUtility.UrlEncode(user.FirstName),
                                            HttpUtility.UrlEncode(user.LastName),
                                            SecurityContext.CurrentAccount.ID);

            client.SendNoticeToAsync(
                        join ? Constants.ActionJoinUsers : Constants.ActionInviteUsers,
                        null,
                        RecipientFromEmail(new string[] { email }, join),/*if it's invite - don't check activation status*/
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagUserName, SecurityContext.IsAuthenticated ? DisplayUserSettings.GetFullUserName(SecurityContext.CurrentAccount.ID) : ((HttpContext.Current != null) ? HttpContext.Current.Request.UserHostAddress : null)),
                        new TagValue(Constants.TagInviteLink, CommonLinkUtility.GetFullAbsolutePath(inviteUrl)),
                        new TagValue(Constants.TagBody, inviteMessage ?? string.Empty),
                        Constants.TagTableTop(),
                        Constants.TagTableItem(1),
                        Constants.TagTableItem(2),
                        Constants.TagTableItem(3),
                        Constants.TagTableBottom(),
                        new TagValue("WithPhoto", "photo"),
                        new TagValue("UserDisplayName", (user.DisplayUserName() ?? "").Trim()),
                        CreateSendFromTag());
        }

        public void UserInfoAddedAfterInvite(UserInfo newUserInfo, string password)
        {
            if (CoreContext.UserManager.UserExists(newUserInfo.ID))
            {
                client.SendNoticeToAsync(
                            Constants.ActionYouAddedAfterInvite,
                            null,
                            RecipientFromEmail(new[] { newUserInfo.Email }, false),
                            new[] { EMailSenderName },
                            null,
                            new TagValue(Constants.TagUserName, newUserInfo.DisplayUserName()),
                            new TagValue(Constants.TagUserEmail, newUserInfo.Email),
                            new TagValue(Constants.TagMyStaffLink, GetMyStaffLink()),
                            new TagValue(Constants.TagPassword, password));
            }
        }

        public void GuestInfoAddedAfterInvite(UserInfo newUserInfo, string password)
        {
            if (CoreContext.UserManager.UserExists(newUserInfo.ID))
            {
                client.SendNoticeToAsync(
                            Constants.ActionYouAddedLikeGuest,
                            null,
                            RecipientFromEmail(new[] { newUserInfo.Email }, false),
                            new[] { EMailSenderName },
                            null,
                            new TagValue(Constants.TagUserName, newUserInfo.DisplayUserName()),
                            new TagValue(Constants.TagUserEmail, newUserInfo.Email),
                            new TagValue(Constants.TagMyStaffLink, GetMyStaffLink()),
                            new TagValue(Constants.TagPassword, password));
            }
        }

        public void UserInfoActivation(UserInfo newUserInfo)
        {
            if (newUserInfo.IsActive)
            {
                throw new ArgumentException("User is already activated!");
            }

            client.SendNoticeToAsync(
                CoreContext.Configuration.YourDocsDemo ? Constants.ActionActivateUsersYourDocs : Constants.ActionActivateUsers,
                null,
                RecipientFromEmail(new[] { newUserInfo.Email.ToLower() }, false),
                new[] { EMailSenderName },
                null,
                new TagValue(Constants.TagInviteLink, GenerateActivationConfirmUrl(newUserInfo)),
                new TagValue(Constants.TagUserName, newUserInfo.DisplayUserName()),
                Constants.TagTableTop(),
                Constants.TagTableItem(1),
                Constants.TagTableItem(2),
                Constants.TagTableItem(3),
                Constants.TagTableBottom(),
                new TagValue("WithPhoto", "photo"),
                CreateSendFromTag());
        }

        public void GuestInfoActivation(UserInfo newUserInfo)
        {
            if (newUserInfo.IsActive)
            {
                throw new ArgumentException("User is already activated!");
            }
            client.SendNoticeToAsync(
                CoreContext.Configuration.YourDocsDemo ? Constants.ActionActivateUsersYourDocs : Constants.ActionActivateGuests,
                null,
                RecipientFromEmail(new[] { newUserInfo.Email.ToLower() }, false),
                new[] { EMailSenderName },
                null,
                new TagValue(Constants.TagInviteLink, GenerateActivationConfirmUrl(newUserInfo)),
                new TagValue(Constants.TagUserName, newUserInfo.DisplayUserName()),
                CreateSendFromTag());
        }

        public void SendMsgProfileDeletion(string email)
        {
            client.SendNoticeToAsync(
                        Constants.ActionProfileDelete,
                        null,
                        RecipientFromEmail(new[] { email }, false),
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagInviteLink, GenerateConfirmUrl(email, ConfirmType.ProfileRemove)));
        }


        public void SendMsgBackupCompleted(Guid userId, string url, DateTime availibleDue)
        {
            var user = CoreContext.UserManager.GetUsers(userId);
            client.SendNoticeToAsync(
                        Constants.ActionBackupCreated,
                        null,
                        new[] { user },
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagBackupUrl, url),
                        new TagValue(Constants.TagBackupHours, (int)(availibleDue - DateTime.UtcNow).TotalHours),
                        new TagValue(Constants.TagOwnerName, user.DisplayUserName()));
        }

        public void SendMsgPortalDeactivation(Tenant t, string d_url, string a_url)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            client.SendNoticeToAsync(
                        Constants.ActionPortalDeactivate,
                        null,
                        new[] { u },
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagActivateUrl, a_url),
                        new TagValue(Constants.TagDeactivateUrl, d_url),
                        new TagValue(Constants.TagOwnerName, u.DisplayUserName()));
        }

        public void SendMsgPortalDeletion(Tenant t, string url)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            client.SendNoticeToAsync(
                        Constants.ActionPortalDelete,
                        null,
                        new[] { u },
                        new[] { EMailSenderName },
                        null,
                        new TagValue(Constants.TagDeleteUrl, url),
                        new TagValue(Constants.TagOwnerName, u.DisplayUserName()));
        }

        public void SendMsgDnsChange(Tenant t, string confirmDnsUpdateUrl, string portalAddress, string portalDns)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            client.SendNoticeToAsync(
                        Constants.ActionDnsChange,
                        null,
                        new[] { u },
                        new[] { EMailSenderName },
                        null,
                        new TagValue("ConfirmDnsUpdate", confirmDnsUpdateUrl),
                        new TagValue("PortalAddress", AddHttpToUrl(portalAddress)),
                        new TagValue("PortalDns", AddHttpToUrl(portalDns ?? string.Empty)),
                        new TagValue(Constants.TagOwnerName, u.DisplayUserName()));
        }


        public void SendMsgConfirmChangeOwner(Tenant t, string newOwnerName, string confirmOwnerUpdateUrl)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            client.SendNoticeToAsync(
                        Constants.ActionConfirmOwnerChange,
                        null,
                        new[] { u },
                        new[] { EMailSenderName },
                        null,
                        new TagValue("ConfirmPortalOwnerUpdate", confirmOwnerUpdateUrl),
                        new TagValue(Constants.TagUserName, newOwnerName),
                        new TagValue(Constants.TagOwnerName, u.DisplayUserName()));
        }


        public void SendCongratulations(UserInfo u)
        {
            client.SendNoticeToAsync(
                Constants.ActionCongratulations,
                null,
                RecipientFromEmail(new[] { u.Email.ToLower() }, false),
                new[] { EMailSenderName },
                null,
                new TagValue(Constants.TagUserName, u.DisplayUserName()),
                new TagValue(Constants.TagUserEmail, u.Email),
                new TagValue(Constants.TagMyStaffLink, GetMyStaffLink()),
                new TagValue(Constants.TagSettingsLink, CommonLinkUtility.GetAdministration(ManagementType.General)),
                new TagValue(Constants.TagInviteLink, GenerateConfirmUrl(u.Email, ConfirmType.EmailActivation)),
                Constants.TagNoteStart,
                Constants.TagNoteEnd,
                new TagValue("WithPhoto", "photo"));
        }

        public void SendTariffWarnings(DateTime scheduleDate)
        {
            var log = LogManager.GetLogger("ASC.Notify");
            var now = scheduleDate.Date;

            log.Info("Start SendTariffWarnings.");

            foreach (var tenant in CoreContext.TenantManager.GetTenants().Where(t => t.Status == TenantStatus.Active))
            {
                try
                {
                    var tariff = CoreContext.PaymentManager.GetTariff(tenant.TenantId);
                    var quota = CoreContext.TenantManager.GetTenantQuota(tenant.TenantId);
                    var duedate = tariff.DueDate.Date;

                    INotifyAction action = null;
                    var onlyadmins = true;
                    var footer = "links";

                    var greenButtonText = string.Empty;
                    var greenButtonUrl = string.Empty;

                    var tableItemText1 = string.Empty;
                    var tableItemText2 = string.Empty;
                    var tableItemText3 = string.Empty;
                    var tableItemText4 = string.Empty;
                    var tableItemText5 = string.Empty;

                    var tableItemUrl1 = string.Empty;
                    var tableItemUrl2 = string.Empty;
                    var tableItemUrl3 = string.Empty;
                    var tableItemUrl4 = string.Empty;
                    var tableItemUrl5 = string.Empty;

                    var tableItemImg1 = string.Empty;
                    var tableItemImg2 = string.Empty;
                    var tableItemImg3 = string.Empty;
                    var tableItemImg4 = string.Empty;
                    var tableItemImg5 = string.Empty;

                    var tableItemComment1 = string.Empty;
                    var tableItemComment2 = string.Empty;
                    var tableItemComment3 = string.Empty;
                    var tableItemComment4 = string.Empty;
                    var tableItemComment5 = string.Empty;

                    if (tenant.CreatedDateTime.Date.AddDays(3) == now)
                    {
                        action = Constants.ActionAfterCreation1;

                        tableItemImg1 = "http://cdn.teamlab.com/media/newsletters/images/integrate_documents.jpg";
                        tableItemText1 = "ItemCreateWorkspaceDocs";
                        tableItemUrl1 = "http://helpcenter.teamlab.com/tipstricks/add-resource.aspx?utm_medium=newsletter&utm_source=after_signup_1&utm_campaign=email";

                        tableItemImg2 = "http://cdn.teamlab.com/media/newsletters/images/import_projects.jpg";
                        tableItemText2 = "ItemImportProjectsBasecamp";
                        tableItemUrl2 = "http://helpcenter.teamlab.com/tipstricks/basecamp-import.aspx?utm_medium=newsletter&utm_source=after_signup_1&utm_campaign=email";

                        tableItemImg3 = "http://cdn.teamlab.com/media/newsletters/images/csv.jpg";
                        tableItemText3 = "ItemUploadCrmContactsCsv";
                        tableItemUrl3 = "http://helpcenter.teamlab.com/guides/import-contacts.aspx?utm_medium=newsletter&utm_source=after_signup_1&utm_campaign=email";

                        tableItemImg4 = "http://cdn.teamlab.com/media/newsletters/images/mail.jpg";
                        tableItemText4 = "ItemAddTeamlabMail";
                        tableItemUrl4 = "http://helpcenter.teamlab.com/gettingstarted/mail.aspx?utm_medium=newsletter&utm_source=after_signup_1&utm_campaign=email";
                    }
                    if (tenant.CreatedDateTime.Date.AddDays(7) == now)
                    {
                        action = Constants.ActionAfterCreation4;

                        tableItemImg1 = "http://cdn.teamlab.com/media/newsletters/images/integrate_documents.jpg";
                        tableItemText1 = "ItemAddFilesCreatWorkspace";
                        tableItemUrl1 = "http://helpcenter.teamlab.com/tipstricks/add-resource.aspx?utm_medium=newsletter&utm_source=after_signup_7&utm_campaign=email";

                        tableItemImg2 = "http://cdn.teamlab.com/media/newsletters/images/coedit.jpg";
                        tableItemText2 = "ItemTryOnlineDocEditor";
                        tableItemUrl2 = "http://helpcenter.teamlab.com/guides/collaborative-editing.aspx?utm_medium=newsletter&utm_source=after_signup_7&utm_campaign=email";

                        tableItemImg3 = "http://cdn.teamlab.com/media/newsletters/images/crm_customization.jpg";
                        tableItemText3 = "ItemUploadCrmContacts";
                        tableItemUrl3 = "http://helpcenter.teamlab.com/guides/import-contacts.aspx?utm_medium=newsletter&utm_source=after_signup_7&utm_campaign=email";

                        tableItemImg4 = "http://cdn.teamlab.com/media/newsletters/images/mail.jpg";
                        tableItemText4 = "ItemAddTeamlabMail";
                        tableItemUrl4 = "http://helpcenter.teamlab.com/gettingstarted/mail.aspx?utm_medium=newsletter&utm_source=after_signup_7&utm_campaign=email";

                        tableItemImg5 = "http://cdn.teamlab.com/media/newsletters/images/jabber.jpg";
                        tableItemText5 = "ItemIntegrateIM";
                        tableItemUrl5 = "http://helpcenter.teamlab.com/tipstricks/integrating-talk.aspx?utm_medium=newsletter&utm_source=after_signup_7&utm_campaign=email";
                    }
                    if (tenant.CreatedDateTime.Date.AddDays(14) == now)
                    {
                        onlyadmins = false;
                        action = Constants.ActionAfterCreation2;

                        tableItemImg1 = "http://cdn.teamlab.com/media/newsletters/images/arm.jpg";
                        tableItemText1 = "ItemFeatureARM";
                        tableItemUrl1 = "http://helpcenter.teamlab.com/video/manage-access-rights.aspx?utm_medium=newsletter&utm_source=after_signup_14&utm_campaign=email";
                        tableItemComment1 = "ItemFeatureARMText";

                        tableItemImg2 = "http://cdn.teamlab.com/media/newsletters/images/coedit.jpg";
                        tableItemText2 = "ItemFeatureCoediting";
                        tableItemUrl2 = "http://www.youtube.com/watch?v=yzZLn3RBBE8?utm_medium=newsletter&utm_source=after_signup_14&utm_campaign=email";
                        tableItemComment2 = "ItemFeatureCoeditingText";

                        tableItemImg3 = "http://cdn.teamlab.com/media/newsletters/images/mail.jpg";
                        tableItemText3 = "ItemFeatureMail";
                        tableItemUrl3 = "http://helpcenter.teamlab.com/gettingstarted/mail.aspx?utm_medium=newsletter&utm_source=after_signup_14&utm_campaign=email";
                        tableItemComment3 = "ItemFeatureMailText";

                        tableItemImg4 = "http://cdn.teamlab.com/media/newsletters/images/calendars.jpg";
                        tableItemText4 = "ItemFeatureCalendar";
                        tableItemUrl4 = "http://helpcenter.teamlab.com/video/share-calendar.aspx?utm_medium=newsletter&utm_source=after_signup_14&utm_campaign=email";
                        tableItemComment4 = "ItemFeatureCalendarText";

                        tableItemImg5 = "http://cdn.teamlab.com/media/newsletters/images/crm_customization.jpg";
                        tableItemText5 = "ItemFeatureCRM";
                        tableItemUrl5 = "http://helpcenter.teamlab.com/tipstricks/integrating-talk.aspx?utm_medium=newsletter&utm_source=after_signup_14&utm_campaign=email";
                        tableItemComment5 = "ItemFeatureCRMText";

                        greenButtonText = "ButtonLogInTeamlab";
                        greenButtonUrl = CommonLinkUtility.GetFullAbsolutePath("~/");
                    }
                    if (tenant.CreatedDateTime.Date.AddDays(21) == now)
                    {
                        onlyadmins = false;
                        action = Constants.ActionAfterCreation3;

                        tableItemImg1 = "http://cdn.teamlab.com/media/newsletters/images/online_editor.jpg";
                        tableItemText1 = "ItemUseOnlineEditor";
                        tableItemUrl1 = "http://helpcenter.teamlab.com/TeamLab-Editors/index.aspx?utm_medium=newsletter&utm_source=after_signup_21&utm_campaign=email";

                        tableItemImg2 = "http://cdn.teamlab.com/media/newsletters/images/coedit.jpg";
                        tableItemText2 = "ItemCreatCoeditDocs";
                        tableItemUrl2 = "http://www.youtube.com/watch?v=yzZLn3RBBE8?utm_medium=newsletter&utm_source=after_signup_21&utm_campaign=email";

                        tableItemImg3 = "http://cdn.teamlab.com/media/newsletters/images/document_sharing.jpg";
                        tableItemText3 = "ItemShareDocsPeople";
                        tableItemUrl3 = "http://helpcenter.teamlab.com/guides/collaborative-editing.aspx?utm_medium=newsletter&utm_source=after_signup_21&utm_campaign=email";

                        tableItemImg4 = "http://cdn.teamlab.com/media/newsletters/images/manage_documentation.jpg";
                        tableItemText4 = "ItemOrganazeTeamDocs";
                        tableItemUrl4 = "http://helpcenter.teamlab.com/administratorguides/organize-company-documentation.aspx?utm_medium=newsletter&utm_source=after_signup_21&utm_campaign=email";

                        tableItemImg5 = "http://cdn.teamlab.com/media/newsletters/images/crm_attach.jpg";
                        tableItemText5 = "ItemAttachFilesCrm";
                        tableItemUrl5 = "http://helpcenter.teamlab.com/gettingstarted/crm.aspx?utm_medium=newsletter&utm_source=after_signup_21&utm_campaign=email";
                    }
                    if (quota.Trial && duedate.AddDays(-5) == now)
                    {
                        action = Constants.ActionTariffWarningTrial;
                        footer = "photo";
                        greenButtonText = "ButtonSelectPricingPlans";
                        greenButtonUrl = CommonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
                    }
                    if (quota.Trial && duedate == now)
                    {
                        action = Constants.ActionTariffWarningTrial2;
                        footer = "photo";
                    }
                    if (quota.Trial && duedate.AddDays(5) == now && tenant.VersionChanged <= tenant.CreatedDateTime)
                    {
                        action = Constants.ActionTariffWarningTrial3;
                        footer = "photo";
                        greenButtonText = "ButtonExtendTrialButton";
                        greenButtonUrl = "mailto:support@teamlab.com";
                    }

                    if (action != null)
                    {
                        CoreContext.TenantManager.SetCurrentTenant(tenant.TenantId);
                        var users = onlyadmins ? CoreContext.UserManager.GetUsersByGroup(ASC.Core.Users.Constants.GroupAdmin.ID) : CoreContext.UserManager.GetUsers();
                        foreach (var u in users)
                        {
                            var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = culture;

                            client.SendNoticeToAsync(
                                action,
                                null,
                                new[] { ToRecipient(u.ID) },
                                new[] { EMailSenderName },
                                null,
                                new TagValue(Constants.TagUserName, u.DisplayUserName()),
                                new TagValue("PricingPage", CommonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx")),
                                new TagValue("FAQ", CommonLinkUtility.GetHelpLink(true) + "faq/pricing.aspx"),
                                new TagValue("ActiveUsers", CoreContext.UserManager.GetUsers().Count()),
                                new TagValue("Price", TenantExtra.GetRightQuota().Price),//TODO: use price partner
                                Constants.TagBlueButton("ButtonRequestCallButton", "http://www.teamlab.com/call-back-form.aspx"),
                                Constants.TagGreenButton(greenButtonText, greenButtonUrl),
                                Constants.TagTableTop(),
                                Constants.TagTableItem(1, tableItemText1, tableItemUrl1, tableItemImg1, tableItemComment1),
                                Constants.TagTableItem(2, tableItemText2, tableItemUrl2, tableItemImg2, tableItemComment2),
                                Constants.TagTableItem(3, tableItemText3, tableItemUrl3, tableItemImg3, tableItemComment3),
                                Constants.TagTableItem(4, tableItemText4, tableItemUrl4, tableItemImg4, tableItemComment4),
                                Constants.TagTableItem(5, tableItemText5, tableItemUrl5, tableItemImg5, tableItemComment5),
                                Constants.TagTableBottom(),
                                new TagValue("WithPhoto", string.IsNullOrEmpty(tenant.PartnerId) ? footer : string.Empty ));
                        }
                    }
                }
                catch (Exception err)
                {
                    log.Error(err);
                }
            }
            log.Info("End SendTariffWarnings.");
        }

        public void MigrationPortalStart(string region, bool notify)
        {
            MigrationNotify(Constants.ActionMigrationPortalStart, region, string.Empty, notify);
        }

        public void MigrationPortalSuccess(string region, string url, bool notify)
        {
            MigrationNotify(Constants.ActionMigrationPortalSuccess, region, url, notify);
        }

        public void MigrationPortalError(string region, string url, bool notify)
        {
            MigrationNotify(!string.IsNullOrEmpty(region) ? Constants.ActionMigrationPortalError : Constants.ActionMigrationPortalServerFailure, region, url, notify);
        }

        private void MigrationNotify(INotifyAction action, string region, string url, bool notify)
        {
            var users = CoreContext.UserManager.GetUsers()
                .Where(u => notify ? u.ActivationStatus == EmployeeActivationStatus.Activated : u.IsOwner())
                .Select(u => ToRecipient(u.ID));

            if (users.Any())
            {
                client.SendNoticeToAsync(
                    action,
                    null,
                    users.ToArray(),
                    new[] { EMailSenderName },
                    null,
                    new TagValue(Constants.TagRegionName, TransferResourceHelper.GetRegionDescription(region)),
                    new TagValue("PortalUrl", url));
            }
        }


        #region Helpers

        public static string GenerateConfirmUrl(string email, ConfirmType confirmType)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email + confirmType.ToString());
            return CommonLinkUtility.GetFullAbsolutePath(String.Format("~/confirm.aspx?type={2}&email={0}&key={1}", HttpUtility.UrlEncode(email), validationKey, confirmType.ToString()));
        }

        private IRecipient ToRecipient(Guid userID)
        {
            return source.GetRecipientsProvider().GetRecipient(userID.ToString());
        }

        private IDirectRecipient[] RecipientFromEmail(string[] emails, bool checkActivation)
        {
            return (emails ?? new string[0])
                .Select(e => new DirectRecipient(e, null, new[] { e }, checkActivation))
                .ToArray();
        }

        private static TagValue CreateSendFromTag()
        {
            return new TagValue(CommonTags.SendFrom,
                SecurityContext.IsAuthenticated && SecurityContext.CurrentAccount is IUserAccount ?
                    DisplayUserSettings.GetFullUserName(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID), false).Replace(">", "&#62").Replace("<", "&#60") :
                    CoreContext.TenantManager.GetCurrentTenant().Name);
        }

        private string GetMyStaffLink()
        {
            return CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetMyStaff());
        }

        private string AddHttpToUrl(string url)
        {
            var httpPrefix = Uri.UriSchemeHttp + Uri.SchemeDelimiter;
            return !string.IsNullOrEmpty(url) && !url.StartsWith(httpPrefix) ? httpPrefix + url : url;
        }

        private List<ProfileChangeItem> GetUserInfoChanges(UserInfo o, UserInfo n)
        {
            var result = new List<ProfileChangeItem>();
            if (o.FirstName != n.FirstName)
            {
                result.Add(new ProfileChangeItem(Resource.FirstName, o.FirstName, n.FirstName));
            }
            if (o.LastName != n.LastName)
            {
                result.Add(new ProfileChangeItem(Resource.LastName, o.LastName, n.LastName));
            }
            if (o.BirthDate != n.BirthDate)
            {
                result.Add(new ProfileChangeItem(Resource.Birthdate, o.BirthDate.HasValue ? o.BirthDate.Value.ToShortDateString() : "", n.BirthDate.HasValue ? n.BirthDate.Value.ToShortDateString() : ""));
            }
            if (o.Department != n.Department)
            {
                result.Add(new ProfileChangeItem(CustomNamingPeople.Substitute<Resource>("Department"), o.Department, n.Department));
            }
            if (o.Title != n.Title)
            {
                result.Add(new ProfileChangeItem(CustomNamingPeople.Substitute<Resource>("UserPost"), o.Title, n.Title));
            }
            if (o.WorkFromDate != n.WorkFromDate)
            {
                result.Add(new ProfileChangeItem(
                    CustomNamingPeople.Substitute<Resource>("WorkFromDate"),
                    o.WorkFromDate.HasValue ? o.WorkFromDate.Value.ToShortDateString() : "",
                    n.WorkFromDate.HasValue ? n.WorkFromDate.Value.ToShortDateString() : ""));
            }
            if (o.Email != n.Email)
            {
                result.Add(new ProfileChangeItem(Resource.Email, o.Email, n.Email));
            }
            return result;
        }

        private string GenerateActivationConfirmUrl(UserInfo user)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(user.Email + ConfirmType.Activation.ToString());
            return CommonLinkUtility.GetFullAbsolutePath(String.Format("~/confirm.aspx?type={2}&email={0}&firstname={4}&lastname={5}&key={1}&uid={3}",
                HttpUtility.UrlEncode(user.Email),
                validationKey,
                ConfirmType.Activation.ToString(),
                SecurityContext.CurrentAccount.ID,
                HttpUtility.UrlEncode(user.FirstName),
                HttpUtility.UrlEncode(user.LastName)));
        }

        #endregion


        public class ProfileChangeItem
        {
            public ProfileChangeItem() { }
            public ProfileChangeItem(string field, string old, string @new)
            {
                Field = field;
                Old = old ?? string.Empty;
                New = @new ?? string.Empty;
            }
            public string Field { get; set; }
            public string Old { get; set; }
            public string New { get; set; }
        }

        class WhatsNewUserActivity
        {
            public IList<string> BreadCrumbs { get; set; }
            public string Title { get; set; }
            public string URL { get; set; }
            public string UserName { get; set; }
            public string UserAbsoluteURL { get; set; }
            public DateTime Date { get; set; }
        }
    }
}