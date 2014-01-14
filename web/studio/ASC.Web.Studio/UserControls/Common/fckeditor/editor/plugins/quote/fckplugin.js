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


QuoteCommand = new FCKDialogCommand( 'Quote', FCKLang.QuoteDlgTitle, FCKPlugins.Items['quote'].Path + 'fck_quote.html', 500, 358 );
 FCKCommands.RegisterCommand( 'Quote',  QuoteCommand) ;

// Create the "Plaholder" toolbar button.
var oQuoteItem = new FCKToolbarButton( 'Quote', FCKLang.QuoteBtn ) ;
oQuoteItem.IconPath = FCKPlugins.Items['quote'].Path + 'quote.png' ;

FCKToolbarItems.RegisterItem( 'Quote', oQuoteItem ) ;

QuoteCommand.GetState = function()
		{
			return FCK_TRISTATE_OFF;
		}
		



