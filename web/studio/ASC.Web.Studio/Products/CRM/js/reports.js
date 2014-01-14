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

if (typeof ASC.CRM === "undefined")
    ASC.CRM = function() { return {} };

ASC.CRM.Reports = (function() {

    var callbackMethods = {
        getContacts: function() {alert("1"); }
    };

    var bred = function() {
        var allData = [
          { label: "Данные 1", color: 0, data: [["2010/10/01", 0], ["2010/11/01", 1], ["2010/12/01", 7]]},
          { label: "Данные 2", color: 1, data: [["2010/10/01", 13], ["2010/11/01", 23], ["2010/12/01", 32]]}
        ];
        
        for(var j = 0; j < allData.length; ++j) {
            for (var i = 0; i < allData[j].data.length; ++i)
                allData[j].data[i][0] = Date.parse(allData[j].data[i][0]);
        }
        
        var plotConf = {
            series: {
                lines: {
                    show: true,
                    lineWidth: 2
                }
            },
            xaxis: {
                mode: "time",
                timeformat: "%y/%m/%d"
            }
        };

        
        jq.plot(jq("#placeholder"), allData, plotConf);
    };

    function bred1() {
        bred2();
    }

    function bred2() {
        alert("3");
    }

    return {
            callbackMethods : callbackMethods,
            bred : bred,
            bred1 : bred1
    };
})();