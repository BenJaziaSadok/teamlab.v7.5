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

(function($) {

    var _initAutocomplete = function(input) {
        if (input.length == 0) return;

        $(input).autocomplete({
            minLength: 1,
            delay: 500,
            autoFocus: true,
            appendTo: $(input).parent(),
            select: function(event, ui) {
                var result = _fullSearchString + ui.item.value;
                if ($(this).hasClass('multipleAutocomplete')) result = result + ', ';
                if ($(this).hasClass('emailOnly')) result = TMMail.parseEmailFromFullAddress(result);
                $(this).val(result);
                $(this).trigger('input');
                return false;
            },
            create: function(event, ui) {
                $(window).resize(function () {
                    if ($(input).data("uiAutocomplete") != undefined) $(input).data("uiAutocomplete").close();
                });
            },
            focus: function(event, ui) {
                return false;
            },
            search: function(event, ui) {
                return true;
            },
            source: function(request, response) {
                var term = request.term;

                if (input.hasClass('multipleAutocomplete')) {
                    var stringList = term.split(',');
                    term = stringList[stringList.length - 1].trim();
                    _fullSearchString = request.term;
                    _fullSearchString = _fullSearchString.slice(0, _fullSearchString.lastIndexOf(term));
                }

                if (term in _emailAutocompleteCache) {
                    var resp = '';
                    if (input.hasClass('emailOnly')) {
                        var result;
                        for (var i = 0; i < _emailAutocompleteCache[term].length; i++) {
                            result = (_emailAutocompleteCache[term])[i];
                            result = TMMail.parseEmailFromFullAddress(result);
                            if (term != result && input[0].id == document.activeElement.id) {
                                resp = _emailAutocompleteCache[term];
                                break;
                            }
                        }
                    }
                    else if (document.activeElement && input[0].id == document.activeElement.id) resp = _emailAutocompleteCache[term];
                    response(resp);
                    return;
                }
                serviceManager.getMailContacts({ searchText: term, responseFunction: response, input: input }, { term: term }, { success: _onGetContacts });
            }
        });

        $(input).data("uiAutocomplete")._resizeMenu = function() {
            var ul = this.menu.element;
            ul.outerWidth(Math.max($(input).width(), this.element.outerWidth()));
        };
    };

    var _onGetContacts = function(params, contacts) {
        var current_value = params.input[0].value;
        _emailAutocompleteCache[params.searchText] = contacts;
        var resp = '';
        if ($(params.input).hasClass('emailOnly')) {
            var result;
            for (var i = 0; i < contacts.length; i++) {
                result = contacts[i];
                result = TMMail.parseEmailFromFullAddress(result);
                if (params.searchText != result && document.activeElement && (params.input)[0].id == document.activeElement.id) {
                    resp = contacts;
                    break;
                }
            }
        }
        else if (document.activeElement && (params.input)[0].id == document.activeElement.id) resp = contacts;
        params.responseFunction(resp);
        params.input[0].value = current_value;
    };

    var _fullSearchString = '';
    var _emailAutocompleteCache = {};


    $.fn.emailAutocomplete = function(params) {
        var input = $(this);
        if (params.multiple) input.addClass('multipleAutocomplete');
        if (params.emailOnly) input.addClass('emailOnly');
        _initAutocomplete(input);
    };
})(jQuery);
