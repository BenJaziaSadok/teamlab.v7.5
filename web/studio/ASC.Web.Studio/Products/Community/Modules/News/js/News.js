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

function toggleNewsControllist(element) 
{
    var divid = "textNewsDiv"+element.id.substr(2);
    
    if(document.getElementById(divid).className != "newsText") 
    {
        document.getElementById(divid).className = "newsText"; 
    } 
    else 
    {
        document.getElementById(divid).className = "newsFullText"; 
    }
}

function callbackRemove(result)
{
    if (result.value.rs1!="0")
    {
        var itemDiv = "viewItem";
        if (itemDiv!=null)
        {
            var divDel = document.getElementById(itemDiv);
            divDel.className = 'errorBox errorText';
            divDel.innerHTML = result.value.rs2;
        }
    }
}

function callbackRemoveFromTable(result)
{
    if (result.value.rs1!="0")
    {
        var itemDiv = "item_"+result.value.rs1;
        if (itemDiv!=null)
        {
            var trDel = document.getElementById(itemDiv);
            trDel.className = 'errorBox errorText';
            var firstTD = 0;
            for(var i = 0; i < trDel.childNodes.length; i++)
            {
                if(trDel.childNodes[i].tagName == 'TD')
                {
                    if (firstTD == 0)
                    {
                        trDel.childNodes[i].innerHTML = "<td>"+result.value.rs2+"</td>"
                        firstTD = 1;
                    }
                    else
                    {
                        trDel.childNodes[i].innerHTML = "<td></td>"
                    }
                        
                }
            }
            
        }
    }
}

function ShowMore()
{
    jq('#divMore').show();
    //document.getElementById('divMore').style.display = 'block';
}

function HideMore()
{
    jq('#divMore').hide();
    //document.getElementById('divMore').style = 'none';
}

function NewsBlockButtons()
{
    jq('#panel_buttons').hide();
    jq('#action_loader').show();
}
function NewsUnBlockButtons() {
    jq('#panel_buttons').show();
    jq('#action_loader').hide();
}
function CheckData() {
    var question = jq("input[name$='poll_question']");
    if (jq(question).val()=="") {
        ShowRequiredError(question);
        NewsUnBlockButtons();
        return;
    }
    else {
        __doPostBack('', '');
    }
}

function CheckDataNews() {
    var newsName = jq("input[id$='_feedName']");
    if (jq(newsName).val() == "") {
        ShowRequiredError(newsName);
        NewsUnBlockButtons();
        return;
    }
    else {
        __doPostBack('', '');
    }
}

function getURLParam(strParamName) {

        strParamName = strParamName.toLowerCase();

        var strReturn = "";
        var strHref = window.location.href.toLowerCase();
        var bFound = false;

        var cmpstring = strParamName + "=";
        var cmplen = cmpstring.length;

        if (strHref.indexOf("?") > -1) {
            var strQueryString = strHref.substr(strHref.indexOf("?") + 1);
            var aQueryString = strQueryString.split("&");
            for (var iParam = 0; iParam < aQueryString.length; iParam++) {
                if (aQueryString[iParam].substr(0, cmplen) == cmpstring) {
                    var aParam = aQueryString[iParam].split("=");
                    strReturn = aParam[1];
                    bFound = true;
                    break;
                }

            }
        }
        if (bFound == false) return null;

        if (strReturn.indexOf("#") > -1)
            return strReturn.split("#")[0];

        return strReturn;
}

function changeCountOfRows (val) {
    var type = getURLParam("type");
    var search = getURLParam("search");
    var href = window.location.href.split("?")[0]+"?";
    if(type!=null)
        href += "&type=" + type;
    if(search!=null)
        href += "&search=" + search;
    window.location.href = href + "&size=" + val;
}

function showCommentBox() {
    if(typeof(FCKeditorAPI)!="undefined" && FCKeditorAPI!=null) {
        CommentsManagerObj.AddNewComment();
    } else {
        setTimeout("showCommentBox();", 500);
    }
}

jq(document).ready(function() {
    jq.dropdownToggle({
        dropdownID: "eventsActionsMenuPanel",
        switcherSelector: ".eventsHeaderBlock .menu-small",
        addTop: -4,
        addLeft: -11,
        showFunction: function(switcherObj, dropdownItem) {
        jq('.eventsHeaderBlock .menu-small.active').removeClass('active');
            if (dropdownItem.is(":hidden")) {
                switcherObj.addClass('active');
            }
        },
        hideFunction: function() {
        jq('.eventsHeaderBlock .menu-small.active').removeClass('active');
        }
    });
    if (jq('#eventsActionsMenuPanel .dropdown-content a').length == 0) {
        jq('span.menu-small').hide();
    }
    var $firstInput = jq("input[id$='_feedName']");
    if ($firstInput.length) {
        $firstInput.focus();
    }
    var anchor = ASC.Controls.AnchorController.getAnchor();
    if (anchor == "addcomment" && CommentsManagerObj) {
        showCommentBox();
    }

});
