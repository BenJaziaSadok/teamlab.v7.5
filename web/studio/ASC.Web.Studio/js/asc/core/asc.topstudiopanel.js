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

;
jq(document).ready(function () {

    jq.dropdownToggle({
        switcherSelector: ".studio-top-panel .product-menu",
        dropdownID: "studio_productListPopupPanel",
        addTop: 2,
        addLeft: 16,
        rightPos: true
    });

    jq.dropdownToggle({
        switcherSelector: ".studio-top-panel .staff-profile-box",
        dropdownID: "studio_myStaffPopupPanel",
        addTop: 0,
        addLeft: 16,
        rightPos: true
    });

    jq.dropdownToggle({
        switcherSelector: ".studio-top-panel .searchActiveBox",
        dropdownID: "studio_searchPopupPanel",
        addTop: 8,
        addLeft: 0,
        afterShowFunction: function () {
            jq("#studio_search").focus();

            var w = jq(window),
                scrWidth = w.width(),
                leftPadding = w.scrollLeft(),
                elem = jq(".studio-top-panel .searchActiveBox"),
                dropElem = jq("#studio_searchPopupPanel"),
                tooth = dropElem.children(".corner-top");

            if ((elem.offset().left + dropElem.outerWidth()) > scrWidth + leftPadding) {
                dropElem.css("left", Math.max(0, elem.offset().left - dropElem.outerWidth() + elem.outerWidth()) + "px");
                tooth.removeClass("left").addClass("right");
            } else {
                tooth.removeClass("right").addClass("left");
            }
        }
    });

    jq("#studio_search").keydown(function (event) {
        var code;

        if (!e) {
            var e = event;
        }
        if (e.keyCode) {
            code = e.keyCode;
        } else if (e.which) {
            code = e.which;
        }

        if (code == 13) {
            Searcher.Search();
            return false;
        }

        if (code == 27) {
            jq("#studio_search").val("");
        }
    });

    jq("#studio_searchPopupPanel .search-btn").on("click", function () {
        Searcher.Search();
        return false;
    })

    resizeTrialTitle();

    jq(window).resize(function () {
        resizeTrialTitle();
    });

    VideoSaver.Init();
});

var Searcher = new function () {
    this.Search = function () {
        var text = encodeURIComponent(jq("#studio_search").val());

        if (text == "") {
            return false;
        }

        var url = jq("#studio_search").attr("data-url");

        var selectedProducts = [];
        jq("#studio_searchPopupPanel .search-options-box :checkbox:checked").each(function (i, val) {
            selectedProducts.push(jq(val).attr("data-product-id"));
        });
        var productIds = selectedProducts.join(",");

        var productsUriPart = (productIds ? "&products=" + productIds : "");
        url += "?search=" + text + productsUriPart;

        window.open(url, "_self");
    };
};

var VideoSaver = new function () {
    this.Init = function () {
        if (jq("#dropVideoList li a").length != 0) {
            jq(".top-item-box.video").addClass("has-led");
            jq(".top-item-box.video").find(".inner-label").text(jq("#dropVideoList li a").length);
            jq("a.videoActiveBox").removeAttr("href");

            jq.dropdownToggle({
                switcherSelector: ".studio-top-panel .has-led .videoActiveBox",
                dropdownID: "studio_videoPopupPanel",
                addTop: 5,
                addLeft: -300
            });

            jq("#dropVideoList li a").on("click", function () {
                AjaxPro.timeoutPeriod = 1800000;
                UserVideoGuideUsage.SaveWatchVideo([jq(this).attr("id")]);
            });

            jq("#markVideoRead").on("click", function () {
                var allVideoIds = new Array();
                jq("#dropVideoList li a").each(function () {
                    allVideoIds.push(jq(this).attr("id"));
                });
                AjaxPro.timeoutPeriod = 1800000;
                UserVideoGuideUsage.SaveWatchVideo(allVideoIds);

                jq("#studio_videoPopupPanel").hide();
                jq(".top-item-box.video").removeClass("has-led");
                var boxVideo = jq(".videoActiveBox");
                boxVideo.attr("href", boxVideo.attr("data-videourl"));
            });
        }
    };
};

var resizeTrialTitle = function () {
    var panel = jq(".studio-top-panel");
    var elem = jq(panel).find(".studio-top-trial-period");
    var list = jq(panel).find(".top-item-box:visible");
    var listWidth = jq(panel).find(".staff-profile-box").outerWidth(true) +
        jq(panel).find(".studio-top-logo").outerWidth(true) +
        jq(panel).find(".product-menu").outerWidth(true);
    for (var i = 0, n = list.length; i < n; i++) {
        listWidth = listWidth + jq(list[i]).outerWidth(true);
    }
    elem.css("max-width", panel.width() - listWidth);
};