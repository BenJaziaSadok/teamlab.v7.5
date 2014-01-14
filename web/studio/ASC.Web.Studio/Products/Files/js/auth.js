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

/*******************************************************************************
    Auth for Import
*******************************************************************************/

var OAuthCallback = function (token, source) {
};

var OAuthError = function (error, source) {
    ASC.Files.UI.displayInfoPanel(error, true);
};

var OAuthPopup = function (url, width, height) {
    var newwindow;
    try {
        var params = "height=" + (height || 600) + ",width=" + (width || 1020) + ",resizable=0,status=0,toolbar=0,menubar=0,location=1";
        newwindow = window.open(url, "Authorization", params);
    } catch (err) {
        newwindow = window.open(url, "Authorization");
    }
    return newwindow;
};