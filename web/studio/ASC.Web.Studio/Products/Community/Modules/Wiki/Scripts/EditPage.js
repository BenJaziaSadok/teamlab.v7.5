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

var MarkupControl = "xEditingArea";


function WrapSelectedMarkup(preTag, postTag) {
    var objTextArea = document.getElementById(MarkupControl).firstChild;
    if (objTextArea) {
        if (document.selection && document.selection.createRange) {

            objTextArea.focus();
            var objSelectedTextRange = document.selection.createRange();
            var strSelectedText = objSelectedTextRange.text;
            if (strSelectedText.substring(0, preTag.length) == preTag && strSelectedText.substring(strSelectedText.length - postTag.length, strSelectedText.length) == postTag) {
                objSelectedTextRange.text = strSelectedText.substring(preTag.length, strSelectedText.length - postTag.length);
            }
            else {
                objSelectedTextRange.text = preTag + strSelectedText + postTag;
            }
        }
        else {
            objTextArea.focus();
            var scrollPos = objTextArea.scrollTop;
            var selStart = objTextArea.selectionStart;
            var strFirst = objTextArea.value.substring(0, objTextArea.selectionStart);
            var strSelected = objTextArea.value.substring(objTextArea.selectionStart, objTextArea.selectionEnd);
            var strSecond = objTextArea.value.substring(objTextArea.selectionEnd);
            if (strSelected.substring(0, preTag.length) == preTag && strSelected.substring(strSelected.length - postTag.length, strSelected.length) == postTag) {
                // Remove tags
                strSelected = strSelected.substring(preTag.length, strSelected.length - postTag.length);
                objTextArea.value = strFirst + strSelected + strSecond;
                objTextArea.selectionStart = selStart;
                objTextArea.selectionEnd = selStart + strSelected.length;
            }
            else {
                objTextArea.value = strFirst + preTag + strSelected + postTag + strSecond;
                objTextArea.selectionStart = selStart;
                objTextArea.selectionEnd = selStart + preTag.length + strSelected.length + postTag.length;
            }
            objTextArea.scrollTop = scrollPos;
        }
    }
    return false;
}


/*function WrapSelectedMarkupWYSIWYG(preTag, postTag) {
    insertHTML(preTag + getSelectedText() + postTag);
    return false;
}*/


function InsertSelection(type) {

}


function InsertMarkup(tag) {


    var objTextArea = document.getElementById(MarkupControl).firstChild;
    if (objTextArea) {
        if (document.selection && document.selection.createRange) {
            objTextArea.focus();
            var objSelectedTextRange = document.selection.createRange();
            var strSelectedText = objSelectedTextRange.text;
            objSelectedTextRange.text = tag + strSelectedText;
        }
        else {
            objTextArea.focus();
            var scrollPos = objTextArea.scrollTop;
            var selStart = objTextArea.selectionStart;
            var strFirst = objTextArea.value.substring(0, objTextArea.selectionStart);
            var strSecond = objTextArea.value.substring(objTextArea.selectionStart);
            objTextArea.value = strFirst + tag + strSecond;
            objTextArea.selectionStart = selStart + tag.length;
            objTextArea.selectionEnd = selStart + tag.length;
            objTextArea.scrollTop = scrollPos;
        }
    }
    return false;
}