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

window.blankModal = (function ($) {
    var close = function () {
        $('[blank-page]').remove();
    };
    var addAccount = function () {
        ASC.Controls.AnchorController.move('#accounts');
        accountsModal.addBox(undefined);
        close();
    };
    return {
        close: close,
        addAccount: addAccount
    };
})(jQuery);