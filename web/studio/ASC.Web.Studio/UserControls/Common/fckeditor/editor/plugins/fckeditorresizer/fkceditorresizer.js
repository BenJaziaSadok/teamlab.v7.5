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

var posy = 0;
var isMouseButtonDown = false;
var curHeight = 0;
var defaultHeight = 0;

function saveMousePosY(e){
	posy = getMousePosY(e);
	curHeight = window.frameElement.offsetHeight;
	if(defaultHeight == 0){
		defaultHeight = curHeight;
	}
	setMouseButtonDown();
}

function setFckEditorHeight(height){
	window.frameElement.style.height = height + "px" ;
	window.frameElement.height = height + "px" ;
}
function restoreFckEditorDefaultHeight(){
	if(defaultHeight != 0){
		setFckEditorHeight(defaultHeight);
	}
}

function getMousePosY(e){
	var y = 0;
	if (!e) var e = window.event;
	if (e.pageY) {
		y = e.pageY;
	}
	else if (e.clientY) {
		y = e.clientY;	// + document.body.scrollTop+ document.documentElement.scrollTop;
	}
	return y;
}

function setMouseButtonDown(){
	isMouseButtonDown = true;
}

function resetMouseButtonDown(){
	isMouseButtonDown = false;	
}

//parent.window.document.onmouseup = resetMouseButtonDown;
parent.window.onmouseup = resetMouseButtonDown;
//parent.onmouseup = resetMouseButtonDown;

function resizeFckEditor(e){	
	if(!isMouseButtonDown){
		return;
	}	
	var curMousePos = getMousePosY(e);
	var delta = curMousePos - posy;
	
	if(Math.abs(delta) < 2){
		return;
	}
	
	var newHeight = curHeight + delta;
	if(newHeight < defaultHeight){
		resetMouseButtonDown();
		newHeight = defaultHeight;
	}	
	setFckEditorHeight(newHeight);
}