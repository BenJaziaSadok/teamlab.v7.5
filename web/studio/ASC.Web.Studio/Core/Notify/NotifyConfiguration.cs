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

using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Patterns;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Studio.Core.Notify
{
    public static class NotifyConfiguration
    {
        private static bool configured = false;
        private static object locker = new object();
        private static readonly Regex urlReplacer = new Regex(@"(<a [^>]*href=(('(?<url>[^>']*)')|(""(?<url>[^>""]*)""))[^>]*>)|(<img [^>]*src=(('(?<url>(?!data:)[^>']*)')|(""(?<url>(?!data:)[^>""]*)""))[^/>]*/?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex textileLinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);

        public static void Configure()
        {
            lock (locker)
            {
                if (!configured)
                {
                    configured = true;

                    WorkContext.NotifyContext.NotifyClientRegistration += NotifyClientRegisterCallback;
                    WorkContext.NotifyContext.NotifyEngine.BeforeTransferRequest += BeforeTransferRequest;
                }
            }
        }

        public static void RegisterSendMethods()
        {
            StudioNotifyService.Instance.RegisterSendMethod();
        }


        private static void NotifyClientRegisterCallback(Context context, INotifyClient client)
        {
            var absoluteUrl = new SendInterceptorSkeleton(
                "Web.UrlAbsoluter",
                InterceptorPlace.MessageSend,
                InterceptorLifetime.Global,
                (r, p) =>
                {
                    if (r != null && r.CurrentMessage != null && r.CurrentMessage.ContentType == Pattern.HTMLContentType)
                    {
                        var body = r.CurrentMessage.Body;

                        body = urlReplacer.Replace(body, m =>
                        {
                            var url = m.Groups["url"].Value;
                            var ind = m.Groups["url"].Index - m.Index;
                            return string.IsNullOrEmpty(url) && ind > 0 ?
                                m.Value.Insert(ind, CommonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                                m.Value.Replace(url, CommonLinkUtility.GetFullAbsolutePath(url));
                        });

                        body = textileLinkReplacer.Replace(body, m =>
                        {
                            var url = m.Groups["link"].Value;
                            var ind = m.Groups["link"].Index - m.Index;
                            return string.IsNullOrEmpty(url) && ind > 0 ?
                                m.Value.Insert(ind, CommonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                                m.Value.Replace(url, CommonLinkUtility.GetFullAbsolutePath(url));
                        });

                        r.CurrentMessage.Body = body;
                    }
                    return false;
                });
            client.AddInterceptor(absoluteUrl);

            var securityAndCulture = new SendInterceptorSkeleton(
                "ProductSecurityInterceptor",
                 InterceptorPlace.DirectSend,
                 InterceptorLifetime.Global,
                 (r, p) =>
                 {
                     try
                     {
                         // culture
                         var u = ASC.Core.Users.Constants.LostUser;

                         if (32 <= r.Recipient.ID.Length)
                         {
                             var guid = default(Guid);
                             try
                             {
                                 guid = new Guid(r.Recipient.ID);
                             }
                             catch (FormatException) { }
                             catch (OverflowException) { }

                             if (guid != default(Guid))
                             {
                                 u = CoreContext.UserManager.GetUsers(guid);
                             }
                         }

                         if (ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             u = CoreContext.UserManager.GetUserByEmail(r.Recipient.ID);
                         }

                         if (ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             u = CoreContext.UserManager.GetUserByUserName(r.Recipient.ID);
                         }

                         if (!ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             var culture = !string.IsNullOrEmpty(u.CultureName) ? u.GetCulture() : CoreContext.TenantManager.GetCurrentTenant().GetCulture();
                             Thread.CurrentThread.CurrentCulture = culture;
                             Thread.CurrentThread.CurrentUICulture = culture;

                             // security
                             var tag = r.Arguments.Find(a => a.Tag == CommonTags.ModuleID);
                             var productId = tag != null ? (Guid)tag.Value : Guid.Empty;
                             if (productId == Guid.Empty)
                             {
                                 tag = r.Arguments.Find(a => a.Tag == CommonTags.ProductID);
                                 productId = tag != null ? (Guid)tag.Value : Guid.Empty;
                             }
                             if (productId == Guid.Empty)
                             {
                                 productId = (Guid)(CallContext.GetData("asc.web.product_id") ?? Guid.Empty);
                             }
                             if (productId != Guid.Empty && productId != new Guid("f4d98afdd336433287783c6945c81ea0") /* ignore people product */)
                             {
                                 return !WebItemSecurity.IsAvailableForUser(productId.ToString(), u.ID);
                             }
                         }
                     }
                     catch (Exception error)
                     {
                         LogManager.GetLogger(typeof(NotifyConfiguration)).Error(error);
                     }
                     return false;
                 });
            client.AddInterceptor(securityAndCulture);
        }


        private static void BeforeTransferRequest(NotifyEngine sender, NotifyRequest request)
        {
            var aid = Guid.Empty;
            var aname = string.Empty;
            if (SecurityContext.IsAuthenticated)
            {
                aid = SecurityContext.CurrentAccount.ID;
                if (CoreContext.UserManager.UserExists(aid))
                {
                    aname = CoreContext.UserManager.GetUsers(aid).DisplayUserName(false)
                        .Replace(">", "&#62")
                        .Replace("<", "&#60");
                }
            }

            IProduct product;
            IModule module;
            CommonLinkUtility.GetLocationByRequest(out product, out module);
            if (product == null && CallContext.GetData("asc.web.product_id") != null)
            {
                product = WebItemManager.Instance[(Guid)CallContext.GetData("asc.web.product_id")] as IProduct;
            }

            request.Arguments.Add(new TagValue(CommonTags.AuthorID, aid));
            request.Arguments.Add(new TagValue(CommonTags.AuthorName, aname));
            request.Arguments.Add(new TagValue(CommonTags.AuthorUrl, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(aid))));
            request.Arguments.Add(new TagValue(CommonTags.VirtualRootPath, CommonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')));
            request.Arguments.Add(new TagValue(CommonTags.ProductID, product != null ? product.ID : Guid.Empty));
            request.Arguments.Add(new TagValue(CommonTags.ModuleID, module != null ? module.ID : Guid.Empty));
            request.Arguments.Add(new TagValue(CommonTags.ProductUrl, CommonLinkUtility.GetFullAbsolutePath(product != null ? product.StartURL : "~")));
            request.Arguments.Add(new TagValue(CommonTags.DateTime, TenantUtil.DateTimeNow()));
            request.Arguments.Add(new TagValue(CommonTags.Helper, new PatternHelper()));
            request.Arguments.Add(new TagValue(CommonTags.RecipientID, Context.SYS_RECIPIENT_ID));
            request.Arguments.Add(new TagValue(CommonTags.RecipientSubscriptionConfigURL, CommonLinkUtility.GetMyStaff()));
            if (!request.Arguments.Any(x => CommonTags.SendFrom.Equals(x.Tag)))
            {
                request.Arguments.Add(new TagValue(CommonTags.SendFrom, CoreContext.TenantManager.GetCurrentTenant().Name));
            }
        }
    }
}
