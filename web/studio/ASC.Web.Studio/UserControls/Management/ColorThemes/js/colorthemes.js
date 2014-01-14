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

ASC.Controls.ColorThemesSettings = function () {
    var isInit = false;

    var init = function() {
        if (isInit) {
            return;
        }
        isInit = true;

        jq("input[name='colorTheme']").on("click", function() {
            var theme = jq(this).val(),
                $preview = jq(".preview-theme-image");
            
            jq("input[name='colorTheme']").each(function() {
                var className = jq(this).val();
                if ($preview.hasClass(className)) {
                    $preview.removeClass(className);
                }
            });
            $preview.addClass(theme);
        });

        jq("#colorThemeBlock .button.blue").on("click", function() {
            saveColorThemeSettings();
        });

        var saveColorThemeSettings = function() {
            var color = jq("input[name='colorTheme']:checked").val();
            ColorThemeController.SaveColorTheme(color, function(result) {
                jq("body").addClass(color);
                window.location.reload(true);
                console.log(result);
            });
        };
    };
    return {
        init: init
    };
}();

jq(function () {
    ASC.Controls.ColorThemesSettings.init();
})