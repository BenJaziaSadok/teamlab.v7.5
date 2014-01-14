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

#if DEBUG
using ASC.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ASC.Common.Tests.Security.Cryptography
{
    [TestClass]
    public class Hasher_Test
    {
        [TestMethod]
        public void DoHash()
        {
            string str = "Hello, Jhon!";
            
            Assert.AreEqual(
                Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
                Hasher.Base64Hash(str,HashAlg.MD5)
                );

            Assert.AreEqual(
               Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
               Hasher.Base64Hash(str, HashAlg.SHA1)
               );

            Assert.AreEqual(
               Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
               Hasher.Base64Hash(str, HashAlg.SHA256)
               );

            Assert.AreEqual(
               Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
               Hasher.Base64Hash(str, HashAlg.SHA512)
               );

            Assert.AreEqual(
              Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
              Hasher.Base64Hash(str) //DEFAULT
              );
        }
    }
}
#endif