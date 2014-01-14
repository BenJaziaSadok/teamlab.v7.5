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

using ASC.Api.Impl;
using ASC.Api.Interfaces;

namespace ASC.Api.Community
{
    ///<summary>
    ///Provides access to community data api
    ///</summary>
    public partial class CommunityApi : IApiEntryPoint
    {
        private readonly ApiContext _context;


        ///<summary>
        /// Starting entry point name
        ///</summary>
        public string Name
        {
            get { return "community"; }
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="context"></param>
        public CommunityApi(ApiContext context)
        {
            _context = context;
        }
    }
}
