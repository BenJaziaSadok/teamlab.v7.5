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

namespace ASC.Xmpp.Core.protocol.iq.register
{
    public delegate void RegisterEventHandler(object sender, RegisterEventArgs args);

    public class RegisterEventArgs
    {
        private bool m_Auto = true;

        public RegisterEventArgs()
        {
        }

        public RegisterEventArgs(Register reg)
        {
            Register = reg;
        }

        // by default we register automatically

        /// <summary>
        ///   Set Auto to true if the library should register automatically Set it to false if you want to fill out the registration fields manual
        /// </summary>
        public bool Auto
        {
            get { return m_Auto; }
            set { m_Auto = value; }
        }

        public Register Register { get; set; }
    }
}