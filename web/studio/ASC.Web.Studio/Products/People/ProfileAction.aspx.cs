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
using System.IO;
using System.Web;

using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.People.Resources;


namespace ASC.Web.People
{
    internal class UserPhotoUploader : IFileUploadHandler
    {
        #region IFileUploadHandler Members

        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var result = new FileUploadResult();
            try
            {
                if (context.Request.Files.Count != 0)
                {
                    var userPhoto = context.Request.Files[0];
                    var data = new byte[userPhoto.InputStream.Length];

                    var br = new BinaryReader(userPhoto.InputStream);
                    br.Read(data, 0, (int)userPhoto.InputStream.Length);
                    br.Close();
                    result.Data = UserPhotoManager.SaveTempPhoto(data, SetupInfo.MaxImageUploadSize, UserPhotoManager.MaxFotoSize.Width, UserPhotoManager.MaxFotoSize.Height);
                    result.Success = true;

                }
                else
                {
                    result.Success = false;
                    result.Message = PeopleResource.ErrorEmptyUploadFileSelected;
                }

            }
            catch (UnknownImageFormatException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorUnknownFileImageType;
            }
            catch (ImageWeightLimitException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorImageWeightLimit;
            }
            catch (ImageSizeLimitException)
            {
                result.Success = false;
                result.Message = PeopleResource.ErrorImageSizetLimit;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message.HtmlEncode();
            }

            return result;
        }

        #endregion
    }

    public partial class ProfileAction : MainPage
    {
        #region Properies
        public ProfileHelper ProfileHelper;
  
        protected bool CanEdit
        {
            get
            {
                return ProfileHelper.CanEdit;
            }
        }              
     
        protected bool IsPageEditProfile()
        {
            return (Request["action"] == "edit");
        }
        protected bool IsAdmin()
        {
            return CoreContext.UserManager.GetUsers(ASC.Core.SecurityContext.CurrentAccount.ID).IsAdmin();
        }


        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            ProfileHelper = new ProfileHelper(Request["user"]);

            if (ProfileHelper.isMe && ProfileHelper.isVisitor && IsPageEditProfile())
            {
                Response.Redirect("/my.aspx?action=edit");
            }

            if ((IsPageEditProfile() && !CanEdit) || (!IsPageEditProfile() && !IsAdmin()))
            {
                Response.Redirect("~/products/people/", true);
            }

            var userProfileEditControl = LoadControl(UserProfileEditControl.Location) as UserProfileEditControl;


            _contentHolderForEditForm.Controls.Add(userProfileEditControl);
        } 
      
        #endregion

        
    }
}
