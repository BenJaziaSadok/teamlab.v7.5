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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ASC.Mail.Aggregator;
using ASC.Mail.Aggregator.Extension;

namespace ASC.Api.Mail.DAO
{
    internal class SmileToAttachmentConvertor
    {
        private Dictionary<string, MailAttachment> loaded_smiles;

        public static string SmileBaseUrl
        {
            get { return "usercontrols/common/fckeditor/editor/images/smiley"; }
        }

        public SmileToAttachmentConvertor()
        {
            loaded_smiles = new Dictionary<string, MailAttachment>();
        }

        public MailAttachment ToMailAttachment(string link)
        {
            var file_name = Path.GetFileName(link);
            if (!loaded_smiles.ContainsKey(link))
            {
                var attach = new MailAttachment
                                {
                                    fileName = file_name,
                                    storedName = file_name,
                                    contentId = link.GetMD5(),
                                    data = LoadSmileData(link)
                                };
                loaded_smiles[link] = attach;
            }
            return loaded_smiles[link];
        }

        private byte[] LoadSmileData(string link)
        {
            byte[] data;
            using (var web_client = new WebClient())
            {
                data = web_client.DownloadData(link);
            }
            return data;
        }
    }
}
