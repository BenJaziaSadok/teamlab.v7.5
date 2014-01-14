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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ASC.Security.Cryptography;

#if (DEBUG)
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace ASC.Common.Utils
{
    public static class Signature
    {
        public static string Create<T>(T obj)
        {
            return Create(obj, Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant()));
        }

        public static string Create<T>(T obj, string secret)
        {
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(obj);
            var payload = GetHashBase64MD5(str + secret) + "?" + str;
            return HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(payload));
        }

        public static T Read<T>(string signature)
        {
            return Read<T>(signature, Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant()));
        }

        public static T Read<T>(string signature, string secret)
        {
            return Read<T>(signature, secret, true);
        }

        public static T Read<T>(string signature, string secret, bool useSecret)
        {
            try
            {
                var payloadParts = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(signature)).Split('?');
                if (!useSecret || GetHashBase64(payloadParts[1] + secret) == payloadParts[0]
                    || GetHashBase64MD5(payloadParts[1] + secret) == payloadParts[0] //todo: delete
                    )
                {
                    //Sig correct
                    return new JavaScriptSerializer().Deserialize<T>(payloadParts[1]);
                }
            }
            catch (Exception)
            {
            }
            return default(T);
        }

        private static string GetHashBase64(string str)
        {
            return Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        private static string GetHashBase64MD5(string str)
        {
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)));
        }
    }

#if (DEBUG)
    [TestClass]
    public class SignatureTest
    {
        [TestMethod]
        public void TestSignature()
        {
            var validObject = new { expire = DateTime.UtcNow.AddMinutes(15), key = "345jhndfg", ip = "192.168.1.1" };
            var encoded = Signature.Create(validObject, "ThE SeCret Key!");
            Assert.IsNotNull(Signature.Read<object>(encoded, "ThE SeCret Key!"));
            Assert.IsNull(Signature.Read<object>(encoded, "ThE SeCret Key"));
        }
    }
#endif
}