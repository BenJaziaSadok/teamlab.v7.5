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
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio.Core
{
	[Serializable]
    [DataContract]
	public class WizardSettings : ISettings
	{
        [DataMember(Name = "Completed")]
        public bool Completed { get; set; }

		public Guid ID
		{
			get { return new Guid("{9A925891-1F92-4ed7-B277-D6F649739F06}"); }
		}


        public ISettings GetDefault()
		{
			return new WizardSettings()
			{
                Completed = true
			};
		}
	}
}
