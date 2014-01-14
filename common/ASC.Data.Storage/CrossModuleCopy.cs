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

namespace ASC.Data.Storage
{
    ///<summary>
    /// Helper for copying from one module to another
    ///</summary>
    public static class CrossModuleCopy
    {
        public static Uri CrossCopy(IDataStore srcStore, string srcFilename, IDataStore dstStore, string dstFilename)
        {
            return CrossCopy(srcStore, string.Empty, srcFilename, dstStore, string.Empty, dstFilename);
        }

        ///<summary>
        /// Copy from one module to another. Can copy from s3 to disk and vice versa
        ///</summary>
        ///<param name="srcStore"></param>
        ///<param name="srcDomain"></param>
        ///<param name="srcFilename"></param>
        ///<param name="dstStore"></param>
        ///<param name="dstDomain"></param>
        ///<param name="dstFilename"></param>
        ///<returns></returns>
        ///<exception cref="ArgumentNullException"></exception>
        public static Uri CrossCopy(IDataStore srcStore, string srcDomain, string srcFilename, IDataStore dstStore,
                                    string dstDomain, string dstFilename)
        {
            if (srcStore == null) throw new ArgumentNullException("srcStore");
            if (srcDomain == null) throw new ArgumentNullException("srcDomain");
            if (srcFilename == null) throw new ArgumentNullException("srcFilename");
            if (dstStore == null) throw new ArgumentNullException("dstStore");
            if (dstDomain == null) throw new ArgumentNullException("dstDomain");
            if (dstFilename == null) throw new ArgumentNullException("dstFilename");
            //Read contents
            using (Stream srcStream = srcStore.GetReadStream(srcDomain, srcFilename))
            {
                using (var memoryStream = TempStream.Create())
                {
                    //Copy
                    var buffer = new byte[4096];
                    int readed;
                    while ((readed = srcStream.Read(buffer, 0, 4096)) != 0)
                    {
                        memoryStream.Write(buffer, 0, readed);
                    }

                    memoryStream.Position = 0;
                    return dstStore.Save(dstDomain, dstFilename, memoryStream);
                }
            }
        }
    }
}