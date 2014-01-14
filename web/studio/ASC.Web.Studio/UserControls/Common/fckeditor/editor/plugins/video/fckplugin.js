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



FCKCommands.RegisterCommand( 'Video', new FCKDialogCommand( 'Video', FCKLang.VideoDlgTitle, FCKPlugins.Items['video'].Path + 'fck_flv.html', 541, 524 ) ) ;

// Create the "Plaholder" toolbar button.
var oVideoItem = new FCKToolbarButton( 'Video', FCKLang.VideoBtn ) ;
oVideoItem.IconPath = FCKPlugins.Items['video'].Path + 'video.png' ;

FCKToolbarItems.RegisterItem( 'Video', oVideoItem ) ;

FCK.ContextMenu.RegisterListener( {
        AddItems : function( menu, tag, tagName )
        {
                // under what circumstances do we display this option
                if ( tagName == 'IMG' && /*tag._fckflv*/tag.className == 'FCK__Flv'  )
                {
                        // when the option is displayed, show a separator  the command
                        menu.AddSeparator() ;
                        // the command needs the registered command name, the title for the context menu, and the icon path
                        menu.AddItem( 'Video', FCKLang.VideoDlgTitle, oVideoItem.IconPath ) ;
                }
        }}
);




