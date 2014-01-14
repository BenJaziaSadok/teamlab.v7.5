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
(function() {
    var dropdownToggleHash = {};
    jQuery.extend({
        dropdownToggle: function(options) {
            // default options
            options = jQuery.extend({
                //switcherSelector: "#id" or ".class",          - button
                //dropdownID: "id",                             - drop panel
                //anchorSelector: "#id" or ".class",            - near field
                //noActiveSwitcherSelector: "#id" or ".class",  - dont hide
                addTop: 0,
                addLeft: 0,
                position: "absolute",
                fixWinSize: true,
                enableAutoHide: true,
                beforeShowFunction: null,
                showFunction: null,
                afterShowFunction: null,
                hideFunction: null,
                alwaysUp: false,
                simpleToggle: false,
                rightPos: false
            }, options);

            var _toggle = function(switcherObj, dropdownID, addTop, addLeft, fixWinSize, position, anchorSelector, showFunction, alwaysUp, simpleToggle, beforeShowFunction, afterShowFunction) {
                var dropdownItem = jq("#" + dropdownID),
                    targetPos = null,
                    elemPosLeft = 0,
                    elemPosTop = 0,
                    w = null,
                    topPadding = 0,
                    leftPadding = 0,
                    scrWidth = 0,
                    scrHeight = 0;

                if (typeof beforeShowFunction === "function") {
                    beforeShowFunction(switcherObj, dropdownItem);
                }


                if (typeof(simpleToggle) == "undefined" || simpleToggle === false) {
                    fixWinSize = fixWinSize === true;
                    addTop = addTop || 0;
                    addLeft = addLeft || 0;
                    position = position || "absolute";

                    targetPos = jq(anchorSelector || switcherObj).offset();

                    if (!targetPos) {
                        return;
                    }

                    elemPosLeft = targetPos.left;
                    elemPosTop = targetPos.top + jq(anchorSelector || switcherObj).outerHeight();
                    if (options.rightPos) {
                        elemPosLeft = Math.max(0, targetPos.left - dropdownItem.outerWidth() + jq(anchorSelector || switcherObj).outerWidth());
                    }

                    w = jq(window);
                    topPadding = w.scrollTop();
                    leftPadding = w.scrollLeft();

                    if (position == "fixed") {
                        addTop -= topPadding;
                        addLeft -= leftPadding;
                    }

                    scrWidth = w.width();
                    scrHeight = w.height();

                    if (fixWinSize && (!options.rightPos)
                        && (targetPos.left + addLeft + dropdownItem.outerWidth()) > (leftPadding + scrWidth)) {
                        elemPosLeft = Math.max(0, leftPadding + scrWidth - dropdownItem.outerWidth()) - addLeft;
                    }

                    if (fixWinSize
                        && (elemPosTop + dropdownItem.outerHeight()) > (topPadding + scrHeight)
                        && (targetPos.top - dropdownItem.outerHeight()) > topPadding
                        || alwaysUp) {
                        elemPosTop = targetPos.top - dropdownItem.outerHeight();
                    }

                    dropdownItem.css(
                        {
                            "position": position,
                            "top": elemPosTop + addTop,
                            "left": elemPosLeft + addLeft
                        });
                }
                if (typeof showFunction === "function") {
                    showFunction(switcherObj, dropdownItem);
                }

                dropdownItem.toggle();

                if (typeof afterShowFunction === "function") {
                    afterShowFunction(switcherObj, dropdownItem);
                }
            };

            var _registerAutoHide = function(event, switcherSelector, dropdownSelector, hideFunction) {
                if (jq(dropdownSelector).is(":visible")) {
                    var $targetElement = jq((event.target) ? event.target : event.srcElement);
                    if (!$targetElement.parents().addBack().is(switcherSelector + ", " + dropdownSelector)) {
                        if (typeof hideFunction === "function") {
                            hideFunction($targetElement);
                        }
                        jq(dropdownSelector).hide();
                    }
                }
            };

            if (options.switcherSelector && options.dropdownID) {
                var toggleFunc = function() {
                    _toggle(jq(this), options.dropdownID, options.addTop, options.addLeft, options.fixWinSize, options.position, options.anchorSelector, options.showFunction, options.alwaysUp, options.simpleToggle, options.beforeShowFunction, options.afterShowFunction);
                };
                if (!dropdownToggleHash.hasOwnProperty(options.switcherSelector + options.dropdownID)) {
                    jq(document).on("click", options.switcherSelector, toggleFunc);
                    dropdownToggleHash[options.switcherSelector + options.dropdownID] = true;
                }
            }

            if (options.enableAutoHide && options.dropdownID) {
                var hideFunc = function(e) {
                    var allSwitcherSelectors = options.noActiveSwitcherSelector ?
                        options.switcherSelector + ", " + options.noActiveSwitcherSelector : options.switcherSelector;
                    _registerAutoHide(e, allSwitcherSelectors, "#" + options.dropdownID, options.hideFunction);

                };
                jq(document).unbind("click", hideFunc);
                jq(document).bind("click", hideFunc);
            }

            return {
                toggle: _toggle,
                registerAutoHide: _registerAutoHide
            };
        }
    });
})();