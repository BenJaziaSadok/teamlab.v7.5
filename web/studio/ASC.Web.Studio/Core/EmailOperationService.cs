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
using System.Collections.Specialized;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core.Notify;
using ASC.Security.Cryptography;
using System.Collections.Generic;
using ASC.Core.Tenants;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Helpers;

namespace ASC.Web.Studio.Core
{
    [AjaxNamespace("EmailOperationService")]
    public class EmailOperationService
    {
        public class InvalidEmailException : Exception
        {
            public InvalidEmailException() : base() { }
            public InvalidEmailException(string message) : base(message) { }
        }

        public class AccessDeniedException : Exception
        {
            public AccessDeniedException() : base() { }
            public AccessDeniedException(string message) : base(message) { }
        }

        public class UserNotFoundException : Exception
        {
            public UserNotFoundException() : base() { }
            public UserNotFoundException(string message) : base(message) { }
        }

        public class InputException : Exception
        {
            public InputException() : base() { }
            public InputException(string message) : base(message) { }
        }

        /// <summary>
        /// Sends the email activation instructions to the specified email
        /// </summary>
        /// <param name="userID">The ID of the user who should activate the email</param>
        /// <param name="email">Email</param>
        /// <param name="changeCurrentEmail">Defines whether must change the current user email to specified. 
        ///  The using of this option is available for admin only</param>
        [AjaxMethod]
        public string SendEmailActivationInstructions(Guid userID, string email)
        {
            if (userID == null)
                throw new ArgumentNullException("userID");

            if (String.IsNullOrEmpty(email))
                throw new ArgumentNullException(Resources.Resource.ErrorEmailEmpty);

            if (!email.TestEmailRegex())
                throw new InvalidEmailException(Resources.Resource.ErrorNotCorrectEmail);

            try
            {
                UserInfo viewer = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                UserInfo user = CoreContext.UserManager.GetUsers(userID);

                if (user == null)
                    throw new UserNotFoundException(Resources.Resource.ErrorUserNotFound);

                if (viewer == null)
                    throw new AccessDeniedException(Resources.Resource.ErrorAccessDenied);

                if (viewer.IsAdmin() || viewer.ID == user.ID)
                {
                    var existentUser = CoreContext.UserManager.GetUserByEmail(email);
                    if (existentUser.ID != ASC.Core.Users.Constants.LostUser.ID && existentUser.ID != userID)
                        throw new InputException(CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists"));

                    user.Email = email;
                    if (user.ActivationStatus == EmployeeActivationStatus.Activated)
                    {
                        user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                    }
                    CoreContext.UserManager.SaveUserInfo(user);

                    if (viewer.ID == user.ID)
                    {
                        string newCookie = SecurityContext.AuthenticateMe(user.ID);
                        CookiesManager.SetCookies(CookiesType.AuthKey, newCookie);
                    }
                }
                else
                {
                    email = user.Email;
                }

                if (user.ActivationStatus == EmployeeActivationStatus.Pending)
                {
                    if (user.IsVisitor())
                    {
                        StudioNotifyService.Instance.GuestInfoActivation(user);
                    }
                    else
                    {
                        StudioNotifyService.Instance.UserInfoActivation(user);
                    }
                }
                else
                {
                    StudioNotifyService.Instance.SendEmailActivationInstructions(user, email);
                }

                return String.Format(Resources.Resource.MessageEmailActivationInstuctionsSentOnEmail, CreateEmailLink(email));
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (AccessDeniedException)
            {
                throw;
            }
            catch (InputException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception(Resources.Resource.UnknownError);
            }
        }

        /// <summary>
        /// Sends the email change instructions to the specified email
        /// </summary>
        /// <param name="userID">The ID of the user who is changing the email</param>
        /// <param name="email">Email</param>
        [AjaxMethod]
        public string SendEmailChangeInstructions(Guid userID, string email)
        {
            if (userID == null)
                throw new ArgumentNullException("userID");

            if (String.IsNullOrEmpty(email))
                throw new Exception(Resources.Resource.ErrorEmailEmpty);

            if (!email.TestEmailRegex())
                throw new Exception(Resources.Resource.ErrorNotCorrectEmail);

            try
            {
                UserInfo viewer = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                UserInfo user = CoreContext.UserManager.GetUsers(userID);

                if (user == null)
                    throw new UserNotFoundException(Resources.Resource.ErrorUserNotFound);

                if (viewer == null)
                    throw new AccessDeniedException(Resources.Resource.ErrorAccessDenied);

                if (!viewer.IsAdmin())
                {
                    email = user.Email;
                    StudioNotifyService.Instance.SendEmailChangeInstructions(user, email);
                }

                if (viewer.IsAdmin())
                {
                    if (email == user.Email)
                        throw new InputException(Resources.Resource.ErrorEmailsAreTheSame);

                    UserInfo existentUser = CoreContext.UserManager.GetUserByEmail(email);
                    if (existentUser.ID != ASC.Core.Users.Constants.LostUser.ID)
                        throw new InputException(CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists"));

                    user.Email = email;
                    user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                    CoreContext.UserManager.SaveUserInfo(user);
                    StudioNotifyService.Instance.SendEmailActivationInstructions(user, email);

                    if (viewer.ID == user.ID)
                    {
                        string newCookie = SecurityContext.AuthenticateMe(user.ID);
                        CookiesManager.SetCookies(CookiesType.AuthKey, newCookie);
                    }

                }

                return String.Format(Resources.Resource.MessageEmailChangeInstuctionsSentOnEmail, CreateEmailLink(email));
            }
            catch (AccessDeniedException ex)
            {
                throw ex;
            }
            catch (UserNotFoundException ex)
            {
                throw ex;
            }
            catch (InputException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new Exception(Resources.Resource.UnknownError);
            }
        }

        [AjaxMethod]
        public AjaxResponse SendEmailActivationInstructionsOnChange(string oldEmail, string newEmail, string newEmailConfirm, string queryString)
        {
            const string StatusSuccess = "success";
            const string StatusError = "error";
            const string StatusFatalError = "fatalerror";

            AjaxResponse response = new AjaxResponse();
            response.status = StatusSuccess;

            if (String.IsNullOrEmpty(oldEmail))
            {
                response.status = StatusFatalError;
                response.message = Resources.Resource.ErrorConfirmURLError;
                return response;
            }

            if (String.IsNullOrEmpty(queryString))
            {
                response.status = StatusFatalError;
                response.message = Resources.Resource.ErrorConfirmURLError;
                return response;
            }

            if (String.IsNullOrEmpty(newEmail))
            {
                response.status = StatusError;
                response.message = Resources.Resource.ErrorEmailEmpty;
                return response;
            }

            if (String.IsNullOrEmpty(newEmailConfirm))
            {
                response.status = StatusError;
                response.message = Resources.Resource.ErrorEmailConfirmEmpty;
                return response;
            }

            try
            {
                if (!newEmail.TestEmailRegex())
                {
                    response.status = StatusError;
                    response.message = Resources.Resource.ErrorNotCorrectEmail;
                    return response;
                }

                var result = CheckValidationKey(queryString.Substring(1));
                if (result != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    response.status = StatusFatalError;
                    switch (result)
                    {
                        case EmailValidationKeyProvider.ValidationResult.Invalid:
                            response.message = Resources.Resource.ErrorInvalidActivationLink;
                            break;
                        case EmailValidationKeyProvider.ValidationResult.Expired:
                            response.message = Resources.Resource.ErrorExpiredActivationLink;
                            break;
                        default:
                            response.message = Resources.Resource.ErrorConfirmURLError;
                            break;
                    }
                    return response;
                }

                if (!String.Equals(newEmail, newEmailConfirm, StringComparison.InvariantCultureIgnoreCase))
                {
                    response.status = StatusError;
                    response.message = Resources.Resource.ErrorMissMatchEmail;
                    return response;
                }

                UserInfo user = CoreContext.UserManager.GetUserByEmail(oldEmail);
                if (user == null)
                {
                    response.status = StatusFatalError;
                    response.message = String.Format(Resources.Resource.ErrorUserNotFoundByEmail, oldEmail);
                    return response;
                }

                UserInfo existentUser = CoreContext.UserManager.GetUserByEmail(newEmail);
                if (existentUser != null && existentUser.ID == user.ID)
                {
                    response.status = StatusError;
                    response.message = Resources.Resource.ErrorEmailsAreTheSame;
                    return response;
                }

                if (existentUser.ID != ASC.Core.Users.Constants.LostUser.ID)
                {
                    response.status = StatusError;
                    response.message = CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists");
                    return response;
                }

                user.Email = newEmail;
                user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                CoreContext.UserManager.SaveUserInfo(user);

                StudioNotifyService.Instance.SendEmailActivationInstructions(user, newEmail);
                response.message = String.Format(Resources.Resource.MessageEmailActivationInstuctionsSentOnEmail, CreateEmailLink(newEmail));
                return response;
            }
            catch (Exception)
            {
                response.status = StatusFatalError;
                response.message = Resources.Resource.UnknownError;
                return response;
            }
        }

        private EmailValidationKeyProvider.ValidationResult CheckValidationKey(string queryString)
        {
            var request = BuildRequestFromQueryString(queryString);

            var type = request["type"];
            if (String.IsNullOrEmpty(type) || type.ToLowerInvariant() != ConfirmType.EmailChange.ToString().ToLowerInvariant())
            {
                return EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            var email = request["email"];
            if (String.IsNullOrEmpty(email) || !email.TestEmailRegex())
            {
                return EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            var key = request["key"];
            if (String.IsNullOrEmpty(key))
            {
                return EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            return EmailValidationKeyProvider.ValidateEmailKey(email + type, key, SetupInfo.ValidEamilKeyInterval);
        }

        private NameValueCollection BuildRequestFromQueryString(string queryString)
        {
            return HttpUtility.ParseQueryString(queryString);
        }

        private string CreateEmailLink(string email)
        {
            return string.Format("<a href='mailto:{0}' title='{0}'>{0}</a>", email);
        }
    }
}
