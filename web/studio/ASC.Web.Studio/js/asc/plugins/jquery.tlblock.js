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

(function ($, win, doc, body) {
    var supportedClient = true,
        blockClassname = "tl-block",
        blocksCollection = [],
        $window = $(window),
        $body = $(body),
        setResizeCallback = false;

    // check client
    supportedClient = $.browser.msie && $.browser.version < 8 ? false : supportedClient;

    // methods
    function getFullOffsetTop (o) {
        var offsetTop = 0,
            fullOffsetTop = o.offsetTop;
        while (o = o.offsetParent) {
            offsetTop = o.offsetTop;
            offsetTop ? fullOffsetTop += offsetTop : null;
        }
        return fullOffsetTop;
    }

    function resizeBlock ($block) {
        var maxContentHeight,
            blockHeight,
            column = null,
            columnsInd;

        blockHeight = $block.css("overflow-y", "auto").height("auto").height();
        maxContentHeight = $window.height() - getFullOffsetTop($block[0]) - 1; // 1 - border-fix
        $block.height(maxContentHeight + 'px');
        if (blockHeight > maxContentHeight) {
            $block.css("overflow-y", "scroll");
        }

        //maxColumnHeight = 0;
        //columnsInd = $columns.length;
        //while (columnsInd--) {
        //    column = $columns[columnsInd];
        //    column.style.height = "auto";
        //    columnHeight = column.offsetHeight;
        //    maxColumnHeight = columnHeight > maxColumnHeight ? columnHeight : maxColumnHeight;
        //}

        //$columns.height(maxColumnHeight > windowHeight ? windowHeight : )
    }

    // callbacks
    function onWindowResize (evt) {
        var blocks = blocksCollection,
            blocksInd = 0;

        blocksInd = blocks.length;
        while (blocksInd--) {
            resizeBlock(blocks[blocksInd]);
        }
    }

    function initBlock ($block, opts) {
        $body.addClass(blockClassname);
        $block.addClass(blockClassname).css("overflow-x", "visible");

        resizeBlock($block);
        blocksCollection.push($block);
    }

    $.fn.tlBlock = function (opt) {
        opt = $.extend({
            title: "",
            classname: ""
        }, opt);

        if (!setResizeCallback) {
            setResizeCallback = true;
            $window.unbind("resize", onWindowResize).bind("resize", onWindowResize);
        }

        return this.each(function () {
            var $this = $(this);
            if (supportedClient) {
                initBlock($this, opt);
            }
        });

        return this;
    };
})(jQuery, window, document, document.body);
