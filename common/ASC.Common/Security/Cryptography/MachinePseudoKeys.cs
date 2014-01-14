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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace ASC.Security.Cryptography
{
    public static class MachinePseudoKeys
    {
        public static byte[] GetMachineConstant()
        {
            string confkey = ConfigurationManager.AppSettings["core.machinekey"];
            if (string.IsNullOrEmpty(confkey)) confkey = ConfigurationManager.AppSettings["asc.common.machinekey"];
            if (!String.IsNullOrEmpty(confkey))
            {
                return Encoding.UTF8.GetBytes(confkey);
            }
            else
            {
                string path = typeof(MachinePseudoKeys).Assembly.Location;
                var fi = new FileInfo(path);
                return BitConverter.GetBytes(fi.CreationTime.ToOADate());
            }
        }

        public static byte[] GetMachineConstant(int bytesCount)
        {
            byte[] cnst = Enumerable.Repeat<byte>(0, sizeof(int)).Concat(GetMachineConstant()).ToArray();
            int icnst = BitConverter.ToInt32(cnst, cnst.Length - sizeof(int));
            var rnd = new Random(icnst);
            var buff = new byte[bytesCount];
            rnd.NextBytes(buff);
            return buff;
        }
    }
}