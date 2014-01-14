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

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.vcard_update
{

    #region usings

    #endregion

    /*
        <presence>
          <x xmlns='vcard-temp:x:update'>
            <photo/>
          </x>
        </presence>
    */

    /// <summary>
    /// </summary>
    public class VcardUpdate : Element
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="VcardUpdate" /> class.
        /// </summary>
        public VcardUpdate()
        {
            TagName = "x";
            Namespace = Uri.VCARD_UPDATE;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="VcardUpdate" /> class.
        /// </summary>
        /// <param name="photo"> The photo. </param>
        public VcardUpdate(string photo) : this()
        {
            Photo = photo;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   SHA1 hash of the avatar image data <para>if no image/avatar should be advertised, or other clients should be forced
        ///                                        to remove the image set it to a empty string value ("")</para> <para>if this protocol is supported but you ae not ready o advertise a imaeg yet
        ///                                                                                                         set teh value to null.</para> <para>Otherwise teh value must the SHA1 hash of the image data.</para>
        /// </summary>
        public string Photo
        {
            get { return GetTag("photo"); }

            set
            {
                if (value == null)
                {
                    RemoveTag("photo");
                }
                else
                {
                    SetTag("photo", value);
                }
            }
        }

        #endregion
    }
}