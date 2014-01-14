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

#region using

using System.Security.Cryptography;
using System.Text;

#endregion

#if !CF

#endif

namespace ASC.Xmpp.Core.utils
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for Hash.
    /// </summary>
    public class Hash
    {
        #region << SHA1 Hash Desktop Framework and Mono >>		

#if !CF

        /// <summary>
        /// </summary>
        /// <param name="pass"> </param>
        /// <returns> </returns>
        public static string Sha1Hash(string pass)
        {
            SHA1 sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(pass));
            return HexToString(hash);
        }

        /// <summary>
        /// </summary>
        /// <param name="pass"> </param>
        /// <returns> </returns>
        public static byte[] Sha1HashBytes(string pass)
        {
            SHA1 sha = SHA1.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(pass));
        }

#endif

        /// <summary>
        ///   Converts all bytes in the Array to a string representation.
        /// </summary>
        /// <param name="buf"> </param>
        /// <returns> string representation </returns>
        public static string HexToString(byte[] buf)
        {
            var sb = new StringBuilder();
            foreach (byte b in buf)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        #endregion

        #region << SHA1 Hash Compact Framework >>

#if CF
		

    // <summary>
    /// return a SHA1 Hash on PPC and Smartphone
    /// </summary>
    /// <param name="pass"></param>
    /// <returns></returns>
		public static byte[] Sha1Hash(byte[] pass)
		{
			IntPtr hProv;
			bool retVal = WinCeApi.CryptAcquireContext( out hProv, null, null, (int) WinCeApi.SecurityProviderType.RSA_FULL, 0 );
			IntPtr hHash;
			retVal = WinCeApi.CryptCreateHash( hProv, (int) WinCeApi.SecurityProviderType.CALG_SHA1, IntPtr.Zero, 0, out hHash );
			
			byte [] publicKey = pass;
			int publicKeyLen = publicKey.Length;
			retVal = WinCeApi.CryptHashData( hHash, publicKey, publicKeyLen, 0 );
			int bufferLen = 20; // SHA1 size
			byte [] buffer = new byte[bufferLen];
			retVal = WinCeApi.CryptGetHashParam( hHash, (int) WinCeApi.SecurityProviderType.HP_HASHVAL, buffer, ref bufferLen, 0 );
			retVal = WinCeApi.CryptDestroyHash( hHash );
			retVal = WinCeApi.CryptReleaseContext( hProv, 0 );
			
			return buffer;
		}

		/// <summary>
		/// return a SHA1 Hash on PPC and Smartphone
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		public static string Sha1Hash(string pass)
		{
			return HexToString(Sha1Hash(System.Text.Encoding.ASCII.GetBytes(pass)));		
		}

		/// <summary>
		/// return a SHA1 Hash on PPC and Smartphone
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
		public static byte[] Sha1HashBytes(string pass)
		{
			return Sha1Hash(System.Text.Encoding.UTF8.GetBytes(pass));
		}

		/// <summary>
		/// omputes the MD5 hash value for the specified byte array.		
		/// </summary>
		/// <param name="pass">The input for which to compute the hash code.</param>
		/// <returns>The computed hash code.</returns>
		public static byte[] MD5Hash(byte[] pass)
		{
			IntPtr hProv;
			bool retVal = WinCeApi.CryptAcquireContext( out hProv, null, null, (int) WinCeApi.SecurityProviderType.RSA_FULL, 0 );
			IntPtr hHash;
			retVal = WinCeApi.CryptCreateHash( hProv, (int) WinCeApi.SecurityProviderType.CALG_MD5, IntPtr.Zero, 0, out hHash );
			
			byte [] publicKey = pass;
			int publicKeyLen = publicKey.Length;
			retVal = WinCeApi.CryptHashData( hHash, publicKey, publicKeyLen, 0 );
			int bufferLen = 16; // SHA1 size
			byte [] buffer = new byte[bufferLen];
			retVal = WinCeApi.CryptGetHashParam( hHash, (int) WinCeApi.SecurityProviderType.HP_HASHVAL, buffer, ref bufferLen, 0 );
			retVal = WinCeApi.CryptDestroyHash( hHash );
			retVal = WinCeApi.CryptReleaseContext( hProv, 0 );

			return buffer;
		}

#endif

        #endregion
    }
}