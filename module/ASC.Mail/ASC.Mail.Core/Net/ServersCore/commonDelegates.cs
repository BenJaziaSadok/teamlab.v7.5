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

namespace ASC.Mail.Net
{
    /// <summary>
    /// Represent the method what will handle Error event.
    /// </summary>
    /// <param name="sender">Delegate caller.</param>
    /// <param name="e">Event data.</param>
    public delegate void ErrorEventHandler(object sender, Error_EventArgs e);

    /// <summary>
    /// To be supplied.
    /// </summary>
    public delegate void LogEventHandler(object sender, Log_EventArgs e);

    /// <summary>
    /// Represents the method that will handle the <see href="LumiSoftMailServerSMTPSMTP_ServerValidateIPAddressFieldOrEvent.html">SMTP_Server.ValidateIPAddress</see> and <see href="LumiSoftMailServerPOP3POP3_ServerValidateIPAddressFieldOrEvent.html">POP3_Server.ValidateIPAddress</see>event.
    /// </summary>
    /// <param name="sender">The source of the event. </param>
    /// <param name="e">A <see href="LumiSoftMailServerValidateIP_EventArgs.html">ValidateIP_EventArgs</see> that contains the event data.</param>
    public delegate void ValidateIPHandler(object sender, ValidateIP_EventArgs e);
}