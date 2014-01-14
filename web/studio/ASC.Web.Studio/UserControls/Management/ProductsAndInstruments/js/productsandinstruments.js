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

if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.Settings === "undefined")
    ASC.Settings = (function() { return {}; })();

ASC.Settings.ProductsAndInstruments = new function() {

    var _timeoutHandler = null;

    return {

        disableElements: function (disable) {
            if (disable) {
                LoadingBanner.displayLoading();
                jq(".web-item-list input[type=checkbox]").attr("disabled", true);
                jq("#btnSaveSettings").addClass("disable");
            } else {
                LoadingBanner.hideLoading();
                jq(".web-item-list input[type=checkbox]").removeAttr("disabled");
                jq("#btnSaveSettings").removeClass("disable");
            }
        },

        showInfoPanel: function (success, message) {
            var infoPanel = jq("#studio_productSettings .okBox");
            if (!success) {
                infoPanel = jq("#studio_productSettings .errorBox");
            }
            if (message) {
                infoPanel.text(message);
            }
            infoPanel.removeClass("display-none");

            _timeoutHandler = setTimeout(function() {
                 infoPanel.addClass("display-none");
            }, 4000);
        },

        changeSubItems: function(cbx) {
            var webItem = jq(cbx).closest(".web-item");
            var subItemList = jq(webItem).find(".web-item-subitem-list");
            if(subItemList.length > 0) {
                var checked = jq(cbx).is(":checked");
                if(checked) {
                    jq(subItemList).find("input[type=checkbox]").each(function() {
                        jq(this).prop("checked", true);
                    });
                    jq(subItemList).show();
                } else {
                    jq(subItemList).find("input[type=checkbox]").each(function() {
                        jq(this).prop("checked", false);
                    });
                    jq(subItemList).hide();
                }
            }
        },

        saveSettings: function () {

            if (_timeoutHandler)
                clearInterval(_timeoutHandler);

            var data = {};
            data.items = new Array();
            var products = new Array();

            jq(".web-item").each(function() {
                var cbx = jq(this).find(".web-item-header input[type=checkbox]");
                var itemId = jq(cbx).attr("data-id");
                var itemName = jq(cbx).attr("id").split("_")[1];
                var itemEnabled = jq(cbx).is(":checked");
                data.items.push({
                    Key: itemId,
                    Value: itemEnabled
                });
                products.push({
                    Name: itemName,
                    Value: itemEnabled
                });
                var subItemList = jq(this).find(".web-item-subitem-list");
                if(subItemList.length > 0 && itemEnabled) {
                    jq(subItemList).find("input[type=checkbox]").each(function() {
                        var subItemId = jq(this).attr("data-id");
                        var subItemEnabled = jq(this).is(":checked");
                        data.items.push({
                            Key: subItemId,
                            Value: subItemEnabled
                        });
                    });
                }
            });

            Teamlab.setAccessToWebItems({}, data, {
                before: function() {
                    ASC.Settings.ProductsAndInstruments.disableElements(true);
                },
                error: function (params, error) {
                    ASC.Settings.ProductsAndInstruments.disableElements(false);
                    ASC.Settings.ProductsAndInstruments.showInfoPanel(false, error[0]);
                },
                success: function() {
                    ASC.Settings.ProductsAndInstruments.showInfoPanel(true);
                    window.location.reload();
                }
            });

        }

    };
};

(function ($) {
    $(function () {

        jq(".web-item-header input[type=checkbox]").change(function () {
            ASC.Settings.ProductsAndInstruments.changeSubItems(this);
        });

        jq("#btnSaveSettings").click(function () {
            ASC.Settings.ProductsAndInstruments.saveSettings();
        });

    });
})(jQuery);

