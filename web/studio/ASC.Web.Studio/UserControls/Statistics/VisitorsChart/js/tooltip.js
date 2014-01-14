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

(function () {
  if (typeof window.ASC === 'undefined') {
    window.ASC = {};
  }
  if (typeof window.ASC.Common === 'undefined') {
    window.ASC.Common = {};
  }

  window.ASC.Common.toolTip = (function () {
    var
      wrapperId = '',
      wrapperClassName = 'tooltip-wrapper',
      wrapperHandler = null;

    var uniqueId = function (prefix) {
      return (typeof prefix != 'undefined' ? prefix + '-' : '') + Math.floor(Math.random() * 1000000);
    };

    var create = function () {
      if (!wrapperHandler) {
        wrapperId = uniqueId('tooltipWrapper');
        wrapperHandler = document.createElement('div');
        wrapperHandler.id = wrapperId;
        wrapperHandler.className = wrapperClassName;
        wrapperHandler.style.display = 'none';
        wrapperHandler.style.left = '0';
        wrapperHandler.style.top = '0';
        wrapperHandler.style.position = 'absolute';
        document.body.appendChild(wrapperHandler);
      }
      return wrapperHandler;
    };

    var show = function (content, handler) {
      create();
      wrapperHandler.innerHTML = content;
      wrapperHandler.style.display = 'block';

      if (typeof handler === 'function') {
        handler.call(wrapperHandler);
      }
    };

    var hide = function () {
      if (wrapperHandler && typeof wrapperHandler === 'object') {
        wrapperHandler.style.display = 'none';
      }
    };

    return {
      show  : show,
      hide  : hide
    }
  })();
})();
