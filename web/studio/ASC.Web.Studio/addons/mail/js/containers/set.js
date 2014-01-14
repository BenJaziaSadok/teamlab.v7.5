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

window.TMContainers = (function($) {

    function IdMap() {

        var _map = {};

        this.AddId = function(messageId, what) {
            _map[messageId] = what;
        }

        this.RemoveId = function(messageId) {
            delete _map[messageId];
        }

        this.RemoveIds = function(messageIds) {
            for (var i = 0; i < messageIds.length; i++) {
                var id = messageIds[i];
                delete _map[id];
            }
        }

        this.GetIds = function() {
            var result = [];
            for (var prop in _map) {
                result.push(prop);
            }
            return result;
        }

        this.GetValues = function() {
            var result = [];
            for (var id in _map) {
                result.push(_map[id]);
            }
            return result;
        }

        this.Count = function() {
            var num_props = 0;
            for (var prop in _map) {
                num_props++;
            }
            return num_props;
        };

        this.Clear = function() {
            _map = {};
        };

        this.Each = function(callback) {
            if (callback === undefined) return;

            $.each(_map, callback);
        };

        this.HasId = function(id) {
            return _map.hasOwnProperty(id);
        };

    }

    function StringSet() {

        var values = [];

        this.Add = function(value) {
            if (!this.Has(value))
                values.push(value);
        }

        this.GetValues = function() {
            return values;
        }

        this.Count = function() {
            return values.length;
        };

        this.Clear = function() {
            values = [];
        };

        this.Each = function(callback) {
            if (callback === undefined) return;

            var count = this.Count();
            for (var i = 0; i < count; i++) {
                var ret = callback(values[i]);
                if (ret === false) break;
            }
        };

        this.Has = function(value) {
            var count = this.Count();
            for (var i = 0; i < count; i++) {
                if (values[i] === value) return true;
            }
            return false;
        };

    }


    return {
        IdMap: IdMap,
        StringSet: StringSet
    };

})(jQuery);
