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

if (typeof(ASC) == 'undefined')
    ASC ={};

if (typeof(ASC.Studio) == 'undefined')
    ASC.Studio = {};

function nothing(e)
{
    return false;
};

ASC.Studio.ThumbnailEditor = new function() {

	this.ThumbnailItem = function(height, width, imgUrl){		
		this.Height = height;
		this.Width = width;
		this.ImgUrl = imgUrl;
	};

	this.ThumbnailEditorPrototype = function(id, objName) {
		this.ID = id;
		this.ObjName = objName;
		
		this.JcropElement = null;
		this.JcropMinSize = [0,0];
		this.JcropMaxSize = [0,0];
		this.JcropAspectRatio = 0;
		
		this.ThumbnailItems = new Array();
		this.SaveThumbnailsFunction = null;

		this.OnCancelButtonClick = null;
		this.OnOkButtonClick = null;

		this.ShowDialog = function(zIdex) {
			var zi = 6666;
			if (zIdex != undefined && zIdex != null)
				zi = zIdex;

			try {

				jq.blockUI({ message: jq("#usrdialog_" + this.ID),
					css: {
						opacity: '1',
						border: 'none',
						padding: '0px',
						width: '450px',
						height: '450px',
						cursor: 'default',
						textAlign: 'left',
						'background-color': 'Transparent',
						'margin-left': '-225px',
						'top': '25%'
					},

					overlayCSS: {
						backgroundColor: '#aaaaaa',
						cursor: 'default',
						opacity: '0.3'
					},
					focusInput: false,
					baseZ: zi,

					fadeIn: 0,
					fadeOut: 0
				});
			}
			catch (e) { };
			
			this.InitJcrop(this.JcropMinSize, this.JcropMaxSize, this.JcropAspectRatio);
			
			this.InitThumbnailDefault(this.ThumbnailItems);

			PopupKeyUpActionProvider.CloseDialogAction = this.ObjName + '.CloseAction();';
			PopupKeyUpActionProvider.EnterAction = this.ObjName + '.ApplyAndCloseDialog();';

		};

		this.CloseAction = function() {
			if (this.OnCancelButtonClick != null)
				this.OnCancelButtonClick();
		};

		this.ApplyAndCloseDialog = function() {
			PopupKeyUpActionProvider.ClearActions();
			jq.unblockUI();  
				
			var result = this.JcropElement.tellScaled();
			
			if(result.w != 0 && result.h !=0)
			{
				AjaxPro.onLoading = function(b) {}					
				ThumbnailEditor.SaveThumbnails(result.x, result.y, result.w, result.h, this.SaveThumbnailsFunction, jq('#UserIDHiddenInput').val(), function(result){
						if(result.error != null)
						{
							alert(result.error.Message);
							return;
						}							
						location.reload();
					});	
			}
					 			
			if (this.OnOkButtonClick != null)
				this.OnOkButtonClick();
		}		
		
		this.InitThumbnailDefault = function(ThumbnailItems){
			jq('[id^="preview_"]').each(function(i)
				{
					var thumbnailItem = ThumbnailItems[i];
					jq(this).attr("src", thumbnailItem.ImgUrl);
					jq(this).css("width", thumbnailItem.Height);
					jq(this).css("height", thumbnailItem.Width);
					jq(this).css("margin-left", 0);
					jq(this).css("margin-top", 0);
				});
		}
		
		this.InitJcrop = function(JcropMinSize, JcropMaxSize, JcropAspectRatio) {
		
			if(this.JcropElement != null)
				this.JcropElement.destroy();
			
			this.JcropElement = jQuery.Jcrop('#mainimg_' + this.ID,
			{
				onChange:		this.showPreview,
				onSelect:		this.showPreview,
				minSize:		JcropMinSize,
				maxSize:		JcropMaxSize,				
				aspectRatio:	JcropAspectRatio
			});	
	
		};

		this.showPreview = function(coords)
		{
			var picSrc = jq('[id^="mainimg_"]').attr("src");
			var picHeight = jq('[id^="mainimg_"]').height();
			var picWidth = jq('[id^="mainimg_"]').width();

			jq('[id^="preview_"]').each(function()
			{
				jq(this).attr("src",picSrc);
				var rx = jq(this).parent().width() / coords.w;
				var ry = jq(this).parent().height() / coords.h;

				if(rx == Infinity || ry == Infinity)
					return;
			
				jq(this).css({
					width: Math.round(rx * picWidth) + 'px',
					height: Math.round(ry * picHeight) + 'px',
					marginLeft: '-' + Math.round(rx * coords.x) + 'px',
					marginTop: '-' + Math.round(ry * coords.y) + 'px'
				});
			});
		};

	}
};   