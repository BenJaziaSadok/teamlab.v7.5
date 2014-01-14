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

function actionButtonClick(actionButtonID, redirectFlag) {

	AjaxPro.onLoading = function(b) {
		if (b) {
			showHideActionButtonPanel(actionButtonID, true);
		}
		else {
			setTimeout(function() {
				if (!redirectFlag) {
					showHideActionButtonPanel(actionButtonID, false);
				}		
			}, 200);			
		}
	}
}

function showHideActionButtonPanel(actionButtonID, showFlag) {
	if (showFlag) {
		jq('#' + actionButtonID).parent().children().each(function() {
			var child = jq(this);
			child.hide();
		});
		jq('#' + actionButtonID + 'AjaxRequestPanel').show();
	} else {
		jq('#' + actionButtonID).parent().children().each(function() {
			var child = jq(this);
			child.show();
		});
		hideAjaxRequestPanels();
	}
}

function hideAjaxRequestPanels() {
	jq('div[id$="AjaxRequestPanel"]').each(
		function() {
			jq(this).hide();
		}
	);
}

function showPopupWindowWithActionButton(popupWindowID) {
	hideAjaxRequestPanels();
	jq.blockUI({ message: jq('#' + popupWindowID),
		css: {
			opacity: '1',
			border: 'none',
			padding: '0px',
			width: '400px',
			height: '300px',
			cursor: 'default',
			textAlign: 'left',
			'background-color': 'Transparent',
			'margin-left': '-200px',
			'top': '25%'
		},

		overlayCSS: {
			backgroundColor: '#aaaaaa',
			cursor: 'default',
			opacity: '0.3'
		},
		focusInput: false,
		fadeIn: 0,
		fadeOut: 0
	});
}