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

using ASC.Common.Module;
using ASC.Mail.Autoreply.AddressParsers;
using log4net.Config;

[assembly: XmlConfigurator]

namespace ASC.Mail.Autoreply
{

    public class AutoreplyServiceController : IServiceController
    {
        private AutoreplyService _autoreplyService;

        public void Start()
        {
            if (_autoreplyService == null)
            {
                _autoreplyService = new AutoreplyService();
                _autoreplyService.RegisterAddressParser(new CommentAddressParser());
                _autoreplyService.RegisterAddressParser(new CommunityAddressParser());
                _autoreplyService.RegisterAddressParser(new FileAddressParser());
                _autoreplyService.RegisterAddressParser(new ProjectAddressParser());
                _autoreplyService.Start();
            }
        }

        public void Stop()
        {
            if (_autoreplyService != null)
            {
                _autoreplyService.Stop();
                _autoreplyService.Dispose();
            }
        }
    }
}
