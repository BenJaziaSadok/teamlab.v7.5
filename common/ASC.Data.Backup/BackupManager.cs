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
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ASC.Data.Backup
{
    public class BackupManager
    {
        private const string ROOT = "backup";
        private const string XML_NAME = "backupinfo.xml";

        private IDictionary<string, IBackupProvider> providers;
        private readonly string backup;
        private readonly string[] configs;
        
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;


        public BackupManager(string backup)
            : this(backup, null)
        {
        }

        public BackupManager(string backup, params string[] configs)
        {
            this.backup = backup;
            this.configs = configs ?? new string[0];

            providers = new Dictionary<string, IBackupProvider>();
            AddBackupProvider(new DbBackupProvider());
            AddBackupProvider(new FileBackupProvider());
        }

        public void AddBackupProvider(IBackupProvider provider)
        {
            providers.Add(provider.Name, provider);
            provider.ProgressChanged += OnProgressChanged;
        }

        public void RemoveBackupProvider(string name)
        {
            if (providers.ContainsKey(name))
            {
                providers[name].ProgressChanged -= OnProgressChanged;
                providers.Remove(name);
            }
        }


        public void Save(int tenant)
        {
            using (var backupWriter = new ZipWriteOperator(backup))
            {
                var doc = new XDocument(new XElement(ROOT, new XAttribute("tenant", tenant)));
                foreach (var provider in providers.Values)
                {
                    var elements = provider.GetElements(tenant, configs, backupWriter);
                    if (elements != null)
                    {
                        doc.Root.Add(new XElement(provider.Name, elements));
                    }
                }

                var data = Encoding.UTF8.GetBytes(doc.ToString(SaveOptions.None));
                var stream = backupWriter.BeginWriteEntry(XML_NAME);
                stream.Write(data, 0, data.Length);
                backupWriter.EndWriteEntry();
            }
            ProgressChanged(this, new ProgressChangedEventArgs(string.Empty, 100, true));
        }

        public void Load()
        {
            using (var reader = new ZipReadOperator(backup))
            using (var xml = XmlReader.Create(reader.GetEntry(XML_NAME)))
            {
                var root = XDocument.Load(xml).Element(ROOT);
                if (root == null) return;

                var tenant = int.Parse(root.Attribute("tenant").Value);
                foreach (var provider in providers.Values)
                {
                    var element = root.Element(provider.Name);
                    provider.LoadFrom(element != null ? element.Elements() : new XElement[0], tenant, configs, reader);
                }
            }
            ProgressChanged(this, new ProgressChangedEventArgs(string.Empty, 100, true));
        }


        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null) ProgressChanged(this, e);
        }
    }
}