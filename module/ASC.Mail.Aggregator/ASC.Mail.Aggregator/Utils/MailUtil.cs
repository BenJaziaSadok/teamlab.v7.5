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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ASC.Mail.Aggregator
{
    public static class MailUtil
    {
        public static List<int> GetLabelsFromString(string stringLabel)
        {
            var list = new List<int>();
            if (!string.IsNullOrEmpty(stringLabel))
            {
                var labels = stringLabel.Split(',');
                foreach (var label in labels)
                {
                    int labelIn;
                    if (int.TryParse(label, out labelIn))
                    {
                        list.Add(labelIn);
                    }
                }
            }
            return list;
        }

        public static string GetStringFromLabels(List<int> labels)
        {
            if (labels != null)
            {
                return string.Join(",", labels.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
            }
            return string.Empty;
        }

        public static IEnumerable<string> GetEmailsFromString(string stringEmail)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(stringEmail))
            {
                var labels = stringEmail.Split(',');
                list.AddRange(labels);
            }
            return list;
        }

        public static string GetStringFromEmails(IEnumerable<string> emails)
        {
            if (emails != null)
            {
                return string.Join(",", emails.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
            }
            return string.Empty;
        }
    }
}