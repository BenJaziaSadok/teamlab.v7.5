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

#region Usings

using System.Linq;
using System.Runtime.Serialization;
using ASC.Projects.Engine;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "search", Namespace = "")]
    public class SearchWrapper
    {
        [DataMember(Order = 10)]
        public SearchItemWrapper[] Items  { get; set; }

        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }


        private SearchWrapper()
        {
        }

        public SearchWrapper(SearchGroup searchGroup)
        {
            if (searchGroup.Items != null)
            {
                Items = searchGroup.Items.Select(x => new SearchItemWrapper(x)).ToArray();
            }
            ProjectOwner = new SimpleProjectWrapper(searchGroup.ProjectID, searchGroup.ProjectTitle);
        }


        public static SearchWrapper GetSample()
        {
            return new SearchWrapper
                       {
                           
                           Items =
                               new[]
                                   {
                                       SearchItemWrapper.GetSample(),
                                       SearchItemWrapper.GetSample(),
                                       SearchItemWrapper.GetSample()
                                   },
                           ProjectOwner = SimpleProjectWrapper.GetSample()
                       };
        }
    }
}
