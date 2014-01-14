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


jq(function () {
    jq("#joinAffilliateBanner").on("click", function () {
        //track event

        jq("#joinAffilliateBanner").trackEvent("affilliate_system", "action-click", "affilliate-banner");

        AjaxPro.timeoutPeriod = 1800000;
        AjaxPro.onLoading = function (b) {
            if (b) {
                LoadingBanner.displayLoading();
            } else {
                LoadingBanner.hideLoading();
            }
        };
        AjaxPro.BannerController.JoinToAffiliateProgram(function (result) {
            var res = result.value;
            if (res.rs1 == "1" && res.rs2) {
                location.href = res.rs2;
            } else if (res.rs1 == "0") {
                jq("#errorAffilliateBanner").text(res.rs2);
            }
        });
    });
});