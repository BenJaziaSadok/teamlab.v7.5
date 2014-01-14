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

window.helpPage = (function($) {
    var 
        isInit = false,
        _page;

    var init = function() {
        if (isInit === false) {
            isInit = true;
            _page = $('#helpPanel');
        }
    };

    var show = function(helpId) {
        var params = { helpId: helpId };

        if (!_page.text().trim().length) {
            params.update = true;
            serviceManager.getHelpCenterHtml(params, {success: onGetHelpCenterHtml }, ASC.Resources.Master.Resource.LoadingProcessing);
        } else {
            onGetHelpCenterHtml(params, null);
        }
    };

    var onGetHelpCenterHtml = function(params, html)
    {
        if (params.update) {
            _page.html(html);
            messagePage.initImageZoom();
        }

        _page.show();

        showHelpPage(params.helpId);
        mailBox.hideLoadingMask();
    };

    var hide = function() {
        _page.hide();
    };

    return {
        init: init,
        show: show,
        hide: hide
    };

})(jQuery);