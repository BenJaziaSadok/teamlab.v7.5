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

using System;

#endregion

namespace ASC.Xmpp.Core.utils
{

    #region usings

    #endregion

    /// <summary>
    ///   Class handles the XMPP time format
    /// </summary>
    public class Time
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="date"> </param>
        /// <returns> </returns>
        public static DateTime Date(string date)
        {
            // better put here a try catch in case a client sends a wrong formatted date
            try
            {
                var dt = new DateTime(int.Parse(date.Substring(0, 4)),
                                      int.Parse(date.Substring(4, 2)),
                                      int.Parse(date.Substring(6, 2)),
                                      int.Parse(date.Substring(9, 2)),
                                      int.Parse(date.Substring(12, 2)),
                                      int.Parse(date.Substring(15, 2)), DateTimeKind.Utc);

                return dt.ToLocalTime();
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        ///   Get a XMPP string representation of a Date
        /// </summary>
        /// <param name="date"> DateTime </param>
        /// <returns> XMPP string representation of a DateTime value </returns>
        public static string Date(DateTime date)
        {
            return date.ToString("yyyyMMddTHH:mm:ss");
        }

        /// <summary>
        ///   The new standard used by XMPP in JEP-82 (ISO-8601)
        ///   <example>
        ///     1970-01-01T00:00Z
        ///   </example>
        /// </summary>
        /// <param name="date"> </param>
        /// <returns> </returns>
        public static DateTime ISO_8601Date(string date)
        {
            // .NET does a great Job parsing this Date profile
            try
            {
                return DateTime.Parse(date);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        ///   The new standard used by XMPP in JEP-82 (ISO-8601) converts a local DateTime to a ISO-8601 formatted date in UTC format.
        ///   <example>
        ///     1970-01-01T00:00Z
        ///   </example>
        /// </summary>
        /// <param name="date"> local Datetime </param>
        /// <returns> </returns>
        public static string ISO_8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");	
            // return date.ToString("yyyy-MM-ddTHH:mm:ssZ");	

            // ("yyyy'-'MM'-'dd HH':'mm':'ss'Z'") 			
        }

        #endregion

        /*
            <x xmlns="jabber:x:delay" from="cachet@conference.cachet.myjabber.net/dan" stamp="20060303T15:43:08" />         
        */
    }
}