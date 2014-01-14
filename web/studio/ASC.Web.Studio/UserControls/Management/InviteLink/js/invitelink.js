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

jq(function() {
    if (jq("#shareLinkCopy").length != 0) {
        var clip = new window.ZeroClipboard.Client();

        clip.addEventListener("mouseDown",
            function() {
                var url = jq("#shareLink").val();
                clip.setText(url);
        });

        clip.addEventListener("onComplete",
            function() {
                jq("#shareLink, #shareLinkCopy").yellowFade();
        });

        clip.glue("shareLinkCopy", "shareLinkPanel");
    }
    if (jq("#hiddenUserLink").length != 0 && jq("#hiddenVisitorLink").length != 0) {
        jq("#chkVisitor").on("click", function () {
            changeEmployeeType(this);
        });
    }
});

function changeEmployeeType (obj) {
    if (jq(obj).is(":checked"))
        jq("#shareLink").text(jq("#hiddenVisitorLink").val());
    else
        jq("#shareLink").text(jq("#hiddenUserLink").val());
}