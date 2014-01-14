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

window.wysiwygEditor = (function ($) {
    var editor_instance,
        editor_textarea,
        editor_timer = null,
        message_body_separator = '----------------\r\n',
        events = $({});

    function init() {
        if (TMMail.isIe11()) {
            // ToDo: migrate on ckeditor
            var editor = $('#WYSIWYGEditor');
            editor_textarea = editor.find('textarea');
            editor_textarea.attr('rows', 16);
            editor.css('min-height', 'auto');
        } else {
            // ToDo: refactore editor retrieve
            try {
                var instance_name = $('#WYSIWYGEditor').find('input[type="hidden"]:first').attr('id');
                editor_instance = window.FCKeditorAPI.GetInstance(instance_name);
                editor_instance.Events.AttachEvent('OnAfterSetHTML', onAfterSetHtml);
            } catch (ex) {
                window.addFckEditor_OnComplete(function (editor) {
                    editor_instance = editor;
                    editor_instance.Events.AttachEvent('OnAfterSetHTML', onAfterSetHtml);
                });
            }
        }
    }

    function onAfterSetHtml() {
        editor_instance.ResetIsDirty();
        editor_timer = window.setInterval(valueTimerCallback, 1000); //1 second
    }

    function valueTimerCallback() {
        if (editor_instance){
            if (editor_instance.IsDirty()) {
                events.trigger('onchange');
                editor_instance.ResetIsDirty();
            }
        }
    }

    function onTextChange() {
        events.trigger('onchange');
    }

    function getValue() {
        if (editor_instance) {
            return editor_instance.GetHTML();
        }
        if (editor_textarea)
            return editor_textarea.val();
        return '';
    }

    function setFocus() {
        if (editor_instance) {
            editor_instance.EditingArea.IFrame.contentWindow.focus();
        } else if (editor_textarea)
            editor_textarea.focus();
    }

    function getBodyAsText(body) {
        return $('<div></div>').html(body).text().replace(/^\s+/gm, "").replace(/^\s*[\r\n]/gm, "");
    }

    function showUnavailableEditorWarning() {
        $('#unavailable_wysiwyg_editor_warning').show();
    }

    function setReply(message) {
        close();
        if (editor_instance) {
            editor_instance.SetHTML($.tmpl('replyMessageHtmlBodyTmpl', message).html());
        } else if (editor_textarea) {
            showUnavailableEditorWarning();
            editor_textarea.val('\r\n\r\n' + message.date + ', ' + message.originalFrom + ':\r\n' + message_body_separator + getBodyAsText(message.htmlBody));
            editor_textarea.bind('textchange', onTextChange);
        }
    }

    function setForward(message) {
        close();
        if (editor_instance) {
            editor_instance.SetHTML($.tmpl('forwardMessageHtmlBodyTmpl', message).html());

        } else if (editor_textarea) {
            showUnavailableEditorWarning();
            var text = '\r\n\r\n-------- ' + MailScriptResource.ForwardTitle + ' --------';
            text += MailScriptResource.FromLabel + ': ' + message.to + '\r\n';
            text += MailScriptResource.ToLabel + ': ' + message.from + '\r\n';
            text += MailScriptResource.DateLabel + ': ' + message.date + '\r\n';
            text += MailScriptResource.SubjectLabel + ': ' + message.subject + '\r\n';
            text += message_body_separator;
            text += getBodyAsText(message.htmlBody);
            editor_textarea.val(text);
            editor_textarea.bind('textchange', onTextChange);
        }
    }

    function setDraft(message) {
        close();
        if (editor_instance) {
            editor_instance.SetHTML(message.htmlBody);
        } else if (editor_textarea) {
            showUnavailableEditorWarning();
            editor_textarea.val(getBodyAsText(message.htmlBody));
            editor_textarea.bind('textchange', onTextChange);
        }
    }

    function close() {
        if (editor_instance) {
            clearInterval(editor_timer);
            editor_timer = null;
        } else if (editor_textarea) {
            editor_textarea.unbind('textchange');
        }
    }

    return {
        init: init,
        getValue: getValue,
        setFocus: setFocus,
        setReply: setReply,
        setForward: setForward,
        setDraft: setDraft,
        close: close,
        events: events
    };

})(jQuery);