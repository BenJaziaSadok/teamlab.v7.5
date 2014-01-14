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
using System.Text;
using System.Linq;
using Ionic.Zip;

namespace ASC.Data.Backup
{
    public class ZipWriteOperator : IDataWriteOperator
    {
        private readonly ZipOutputStream zip;

        public ZipWriteOperator(string targetFile)
        {
            zip = new ZipOutputStream(new FileStream(targetFile, FileMode.Create))
            {
                AlternateEncodingUsage = ZipOption.AsNecessary,
                AlternateEncoding = Encoding.Unicode
            };
        }

        public Stream BeginWriteEntry(string key)
        {
            zip.PutNextEntry(key);
            return zip;
        }

        public void EndWriteEntry()
        {
        }

        public void Dispose()
        {
            zip.Flush();
            zip.Close();
        }
    }

    public class ZipReadOperator : IDataReadOperator
    {
        private readonly ZipFile zip;

        public ZipReadOperator(string targetFile)
        {
            zip = new ZipFile(targetFile);
        }

        public Stream GetEntry(string key)
        {
            var entry = zip.SingleOrDefault(x => key.Replace('\\', '/').Equals(x.FileName, StringComparison.OrdinalIgnoreCase));
            if (entry == null && key.Contains('/'))
            {
                entry = zip.SingleOrDefault(x => key.Replace('/', '\\').Equals(x.FileName, StringComparison.OrdinalIgnoreCase));
            }
            return entry != null ? entry.OpenReader() : null;
        }

        public void Dispose()
        {
            zip.Dispose();
        }
    }
}