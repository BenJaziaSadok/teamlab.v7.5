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
using System;

namespace ASC.Common.Tests.Security.Cryptography
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ASC.Security.Cryptography;
    using System.Security.Cryptography;

    [TestClass]
    public class EmailValidationKeyPairProvider_Test
    {
        public void PasswordDerivedBytes_Test()
        {

            byte[] randBytes = new byte[5];
            new Random(10032010).NextBytes(randBytes);


            var tdes = new TripleDESCryptoServiceProvider();
            var pwddb = new PasswordDeriveBytes("1", new byte[] { 1 });
            tdes.Key = pwddb.CryptDeriveKey("TripleDES", "SHA1", 192, tdes.IV);
            //string s = Convert.ToBase64String(tdes.Key);

        }

        [TestMethod]
        public void GetEmailKey_MillisecondDistanceDifference()
        {
            var k1 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");
            System.Threading.Thread.Sleep(15);
            var k2 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");

            Assert.AreNotEqual(k1, k2);
        }

        [TestMethod]
        public void ValidateKeyImmediate()
        {
            var k1 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");
            Assert.AreEqual(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru", k1), EmailValidationKeyProvider.ValidationResult.Ok);
            Assert.AreEqual(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru2", k1), EmailValidationKeyProvider.ValidationResult.Invalid);
        }

        [TestMethod]
        public void ValidateKey_Delayed()
        {
            var k1 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");
            System.Threading.Thread.Sleep(100);
            Assert.AreEqual(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru", k1, TimeSpan.FromMilliseconds(150)), EmailValidationKeyProvider.ValidationResult.Ok);
            System.Threading.Thread.Sleep(100);
            Assert.AreEqual(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru", k1, TimeSpan.FromMilliseconds(150)), EmailValidationKeyProvider.ValidationResult.Expired);
        }

        [TestMethod]
        public void DebugValidation()
        {
            EmailValidationKeyProvider.ValidateEmailKey("ignis_minded@bk.ruPasswordChange", "123075394398.ZVJMVHKNEF67EHMKX7EHZTYEP8CSG4SSBX8E28OPNS8");
        }
    }
}

#endif