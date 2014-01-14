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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Web.Files.Controls
{
    public partial class MainContent : UserControl
    {
        private bool _enableThirdParty;

        protected bool EnableThirdpartySettings
        {
            get { return FilesSettings.EnableThirdParty; }
        }

        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("MainContent/MainContent.ascx"); }
        }

        public object FolderIDCurrentRoot { get; set; }

        public String TitlePage { get; set; }

        public bool EnableHelp;

        public bool EnableThirdParty
        {
            get
            {
                return _enableThirdParty
                       && ImportConfiguration.SupportInclusion
                       && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor()
                       && (Global.IsAdministrator
                           || CoreContext.Configuration.YourDocs
                           || EnableThirdpartySettings);
            }
            set { _enableThirdParty = value; }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();

            InitScripts();
        }

        #endregion

        #region Methods

        private void InitControls()
        {
            confirmRemoveDialog.Options.IsPopup = true;
            confirmOverwriteDialog.Options.IsPopup = true;

            if (EnableThirdParty)
                SettingPanelHolder.Controls.Add(LoadControl(ThirdParty.Location));

            var emptyFolder = (EmptyFolder)LoadControl(EmptyFolder.Location);
            emptyFolder.AllContainers = FolderIDCurrentRoot == null;
            EmptyScreenFolder.Controls.Add(emptyFolder);

            ControlPlaceHolder.Controls.Add(LoadControl(ConvertFile.Location));

            ControlPlaceHolder.Controls.Add(LoadControl(TariffLimitExceed.Location));

            if (FileUtility.ExtsImagePreviewed.Count != 0)
                ControlPlaceHolder.Controls.Add(LoadControl(FileViewer.Location));

            UploaderPlaceHolder.Controls.Add(LoadControl(ChunkUploadDialog.Location));
        }

        private void InitScripts()
        {
            string tasksStatuses;
            
            if (!GetTasksStatuses(out tasksStatuses))
                return;

            var inlineScript = new StringBuilder();

            inlineScript.AppendFormat("ASC.Files.EventHandler.onGetTasksStatuses({0}, {{doNow: true}});", tasksStatuses);

            Page.RegisterInlineScript(inlineScript.ToString(), onReady: false);
        }

        private static bool GetTasksStatuses(out string taskStatuses)
        {
            taskStatuses = null;

            List<FileOperationResult> tasks;
            try
            {
                var docService = ServiceLocator.Current.GetInstance<IFileStorageService>();
                tasks = docService.GetTasksStatuses();
            }
            catch
            {
                return false;
            }
            if (tasks.Count == 0) return false;

            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(ItemList<FileOperationResult>));
                serializer.WriteObject(ms, tasks);
                ms.Seek(0, SeekOrigin.Begin);

                taskStatuses= Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                return true;
            }
        }

        #endregion
    }
}