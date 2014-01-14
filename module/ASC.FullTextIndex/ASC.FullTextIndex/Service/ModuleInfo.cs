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

namespace ASC.FullTextIndex.Service
{
	class ModuleInfo
	{
		public string Name
		{
			get;
			private set;
		}

		public string Select
		{
			get;
			private set;
		}

		public string ConnectionStringName
		{
			get;
			private set;
		}

		public ModuleInfo(string name, string select, string connectionStringName)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			if (string.IsNullOrEmpty(select)) throw new ArgumentNullException("select");
			if (connectionStringName == null) throw new ArgumentNullException("connectionString");

			Name = name;
			Select = select.Trim();
			ConnectionStringName = connectionStringName;
		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var mi = obj as ModuleInfo;
			return mi != null && Name == mi.Name;
		}
	}
}
