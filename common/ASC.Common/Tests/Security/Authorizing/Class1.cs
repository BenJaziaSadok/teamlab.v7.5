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
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using System.Collections.Generic;

namespace ASC.Common.Tests.Security.Authorizing {

	public class Class1 {

		public int Id {
			get;
			set;
		}

		public Class1() { }

		public Class1(int id) {
			Id = id;
		}

		public override string ToString() {
			return Id.ToString();
		}

		public override bool Equals(object obj) {
			var class1 = obj as Class1;
			return class1 != null && Equals(class1.Id, Id);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}
	}

	public class Class1SecurityProvider : ISecurityObjectProvider {

		private readonly Type type1 = typeof(Class1);

		#region ISecurityObjectProvider Members

		public bool InheritSupported {
			get { return true; }
		}

		public ISecurityObjectId InheritFrom(ISecurityObjectId objectId) {
			if (objectId.ObjectType == type1) {
				if (objectId.SecurityId.Equals(2)) return new SecurityObjectId(1, type1);
			}

			return null;
		}

		public bool ObjectRolesSupported {
			get { return true; }
		}

		public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext) {
			var roles = new List<IRole>();

			if (objectId.ObjectType == type1) {
				if (objectId.SecurityId.Equals(1) && account.Equals(Domain.accountNik)) {
					roles.Add(Constants.Owner);
					roles.Add(Constants.Self);
				}
				if (objectId.SecurityId.Equals(3) && account.Equals(Domain.accountAnton)) {
					roles.Add(Constants.Owner);
				}
			}

			return roles;
		}

		#endregion
	}
}
#endif