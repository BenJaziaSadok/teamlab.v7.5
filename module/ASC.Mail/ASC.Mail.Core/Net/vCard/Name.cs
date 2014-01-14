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

namespace ASC.Mail.Net.Mime.vCard
{
    /// <summary>
    /// vCard name implementation.
    /// </summary>
    public class Name
    {
        #region Members

        private string m_AdditionalNames = "";
        private string m_FirstName = "";
        private string m_HonorificPrefix = "";
        private string m_HonorificSuffix = "";
        private string m_LastName = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="additionalNames">Comma separated additional names.</param>
        /// <param name="honorificPrefix">Honorific prefix.</param>
        /// <param name="honorificSuffix">Honorific suffix.</param>
        public Name(string lastName,
                    string firstName,
                    string additionalNames,
                    string honorificPrefix,
                    string honorificSuffix)
        {
            m_LastName = lastName;
            m_FirstName = firstName;
            m_AdditionalNames = additionalNames;
            m_HonorificPrefix = honorificPrefix;
            m_HonorificSuffix = honorificSuffix;
        }

        /// <summary>
        /// Internal parse constructor.
        /// </summary>
        internal Name() {}

        #endregion

        #region Properties

        /// <summary>
        /// Gets comma separated additional names.
        /// </summary>
        public string AdditionalNames
        {
            get { return m_AdditionalNames; }
        }

        /// <summary>
        /// Gets first name.
        /// </summary>
        public string FirstName
        {
            get { return m_FirstName; }
        }

        /// <summary>
        /// Gets honorific prefix.
        /// </summary>
        public string HonorificPerfix
        {
            get { return m_HonorificPrefix; }
        }

        /// <summary>
        /// Gets honorific suffix.
        /// </summary>
        public string HonorificSuffix
        {
            get { return m_HonorificSuffix; }
        }

        /// <summary>
        /// Gets last name.
        /// </summary>
        public string LastName
        {
            get { return m_LastName; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts item to vCard N structure string.
        /// </summary>
        /// <returns></returns>
        public string ToValueString()
        {
            return m_LastName + ";" + m_FirstName + ";" + m_AdditionalNames + ";" + m_HonorificPrefix + ";" +
                   m_HonorificSuffix;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Parses name info from vCard N item.
        /// </summary>
        /// <param name="item">vCard N item.</param>
        internal static Name Parse(Item item)
        {
            string[] items = item.DecodedValue.Split(';');
            Name name = new Name();
            if (items.Length >= 1)
            {
                name.m_LastName = items[0];
            }
            if (items.Length >= 2)
            {
                name.m_FirstName = items[1];
            }
            if (items.Length >= 3)
            {
                name.m_AdditionalNames = items[2];
            }
            if (items.Length >= 4)
            {
                name.m_HonorificPrefix = items[3];
            }
            if (items.Length >= 5)
            {
                name.m_HonorificSuffix = items[4];
            }
            return name;
        }

        #endregion
    }
}