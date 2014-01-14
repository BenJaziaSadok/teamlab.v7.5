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

namespace ASC.Web.Core.Client.Bundling
{
    public enum BundleResourceType
    {
        EmbeddedScript,
        ScriptFile,
        ClientScript,
        StyleFile,
        LessFile,
        EmbeddedStyle,
        None
    }

    public class Bundle
    {
        public Bundle()
        {
            BundleStyles = new HashSet<BundleResource>();
            BundleScripts = new HashSet<BundleResource>();
        }

        public IEnumerable<string> StyleReferences { get; set; }
        public IEnumerable<string> ScriptReferences { get; set; }


        public ICollection<BundleResource> BundleStyles { get; private set; }
        public ICollection<BundleResource> BundleScripts { get; private set; }

        public string Hash { get; set; }
    }

    public struct BundleResource : IEquatable<BundleResource>
    {
        public static readonly BundleResource Empty = new BundleResource(BundleResourceType.None, null);
        public string Content;
        public string ServerPath;
        public readonly BundleResourceType Type;
        public bool ObfuscateJs;

        public BundleResource(BundleResourceType type, string content) : this(type, content, null)
        {
        }

        public BundleResource(BundleResourceType type, string content, string serverPath)
        {
            Type = type;
            Content = content;
            ServerPath = serverPath;
            ObfuscateJs = true;
        }

        #region IEquatable<BundleResource> Members

        public bool Equals(BundleResource other)
        {
            return Equals(other.Type, Type) && Equals(other.Content, Content);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (BundleResource)) return false;
            return Equals((BundleResource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ (Content != null ? Content.GetHashCode() : 0);
            }
        }

        public static bool operator ==(BundleResource left, BundleResource right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BundleResource left, BundleResource right)
        {
            return !left.Equals(right);
        }
    }
}