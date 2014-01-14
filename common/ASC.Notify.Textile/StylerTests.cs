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

#if (DEBUG)
namespace ASC.Notify.Textile
{
    using ASC.Notify.Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class StylerTests
    {
        private readonly string pattern = "h1.New Post in Forum Topic: \"==Project(%: \"Sample Title\"==\":\"==http://sssp.teamlab.com==\"" + System.Environment.NewLine +
            "25/1/2022 \"Jim\":\"http://sssp.teamlab.com/myp.aspx\"" + System.Environment.NewLine +
            "has created a new post in topic:" + System.Environment.NewLine +
            "==<b>- The text!&nbsp;</b>==" + System.Environment.NewLine +
            "\"Read More\":\"http://sssp.teamlab.com/forum/post?id=4345\"" + System.Environment.NewLine +
            "Your portal address: \"http://sssp.teamlab.com\":\"http://teamlab.com\" " + System.Environment.NewLine +
            "\"Edit subscription settings\":\"http://sssp.teamlab.com/subscribe.aspx\"";

        [TestMethod]
        public void TestJabberStyler()
        {
            var message = new NoticeMessage() { Body = pattern };
            new JabberStyler().ApplyFormating(message);
        }

        [TestMethod]
        public void TestTextileStyler()
        {
            var message = new NoticeMessage() { Body = pattern };
            new TextileStyler().ApplyFormating(message);
        }
    }
}
#endif