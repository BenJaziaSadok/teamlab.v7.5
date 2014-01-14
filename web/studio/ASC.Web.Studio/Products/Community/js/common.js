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

if (typeof ASC.Community === "undefined")
    ASC.Community = (function() { return {} })();
    
ASC.Community.Common = (function() {
return {
    renderSideActionButton: function (linkData) {
        if(linkData.length > 0)
        {
        	jq("#studio_sidePanel #otherActions .dropdown-content").html("");
        	
        	for (var i = 0; i < linkData.length; i++) {
        		if(linkData[i].text) {
        			var container = jq("<li></li>");
        			var link = jq("<a></a>").css("cursor","pointer").addClass("dropdown-item").text(linkData[i].text);
        			if (linkData[i].id) {
        				link.attr("id", linkData[i].id);
        			}
        			if (linkData[i].href) {
        				link.attr("href", linkData[i].href);
        			}
        			container.append(link);
        			jq("#studio_sidePanel #otherActions .dropdown-content").append(container);
        			if (linkData[i].onclick) {
        				var func = linkData[i].onclick;
        				link.bind("click", func);
        			}
        		}
        	}
            
            jq("#studio_sidePanel #menuCreateNewButton").removeClass("big").addClass("middle");
            jq("#studio_sidePanel #menuOtherActionsButton").removeClass("display-none");
        }
    }
};
})(jQuery);

jq(function() {
    calculateWidthTitleBlock();
    jq(window).resize(function() {
        calculateWidthTitleBlock();
    });
    
    jq('#addUsersDashboard').bind('click', function() {
        jq('.dashboard-center-box, .backdrop').css('z-index', '500');
        ImportUsersManager.ShowImportControl();       
        return false;
    });

});

var calculateWidthTitleBlock = function() {
    var commonWidth = jq(window).width() - jq(".mainPageTableSidePanel").width();
    var titleWidth = commonWidth - 100;
    jq(".BlogsHeaderBlock").width(titleWidth);
    jq(".BlogsHeaderBlock").css("overflow", "hidden");
    jq(".BlogsHeaderBlock").css("text-overflow", "ellipsis");
};
