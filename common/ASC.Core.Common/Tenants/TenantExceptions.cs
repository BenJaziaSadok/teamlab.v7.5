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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantTooShortException : Exception
    {
        public TenantTooShortException(string message)
            : base(message)
        {
        }

        protected TenantTooShortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantIncorrectCharsException : Exception
    {
        public TenantIncorrectCharsException(string message)
            : base(message)
        {
        }

        protected TenantIncorrectCharsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantAlreadyExistsException : Exception
    {
        public IEnumerable<string> ExistsTenants
        {
            get;
            private set;
        }

        public TenantAlreadyExistsException(string message, IEnumerable<string> existsTenants)
            : base(message)
        {
            ExistsTenants = existsTenants ?? Enumerable.Empty<string>();
        }

        protected TenantAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}