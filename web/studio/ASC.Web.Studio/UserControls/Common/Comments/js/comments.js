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

var CommentsManagerObj = new function() {
    this.obj = null;
    this.iFCKEditor = null;

    this.javaScriptAddCommentFunctionName = "";
    this.javaScriptLoadBBcodeCommentFunctionName = "";
    this.javaScriptUpdateCommentFunctionName = "";
    this.javaScriptCallBackAddComment = "";
    this.javaScriptPreviewCommentFunctionName = "";

    this.OnEditedCommentJS = "";
    this.OnRemovedCommentJS = "";
    this.OnCanceledCommentJS = "";
    this.FckDomainName = "";
    this.currentCommentID = "";

    this.FckUploadHandlerPath = "";

    this.isSimple = false;
    this.inactiveMessage = "";
    this.RemoveAttachButton = "";
    this.CurrentAttachID = 0;

    this.mainLoader = "";

    this.maxLevel = 8;

    this._jsObjName = "";
    this.PID = "";

    this.isDisableCtrlEnter = false;
    this.EnableAttachmets = false;

    this.HandlerTypeName = "";

    this.InitEditor = function(FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss) {
        __FCKeditorNS = null;
        FCKeditorAPI = null;

        this.oFCKeditor = new FCKeditor('CommentsFckEditor_' + this._jsObjName);
        this.oFCKeditor.BasePath = FCKBasePath;
        this.oFCKeditor.ToolbarSet = FCKToolbar;
        this.oFCKeditor.Height = FCKHeight;
        this.oFCKeditor.Width = FCKWidth;

        if (FCKEditorAreaCss != "") {
            this.oFCKeditor.Config['EditorAreaCSS'] = FCKEditorAreaCss;
        }

        fckDiv = document.getElementById('CommentsFckEditorPlaceHolder_' + this._jsObjName);

        if (fckDiv)
            fckDiv.innerHTML = this.oFCKeditor.CreateHtml();
    };

    this.Redraw = function() {
        oddcnt = jq('#mainContainer div[id^=comment_]:even')
                    .removeClass('')
                    .addClass('tintMedium')
                    .css('border-top', '1px solid #DDD')
                    .css('border-bottom', '1px solid #DDD')
                    .length;
        evencnt = jq('#mainContainer div[id^=comment_]:odd')
                    .removeClass('tintMedium')
                    .css('border-top', '')
                    .css('border-bottom', '')
                    .addClass('').length;

        if (oddcnt == evencnt)
            jq('#mainContainer').css('border-bottom', '1px solid #DDD');
        else
            jq('#mainContainer').css('border-bottom', '');
    };

    this.ResponseToComment = function(obj, id) {
        this.obj = obj.id.replace("response_", "");
        this.SetParentComment(obj.id.replace("response_", ""));
        this.SetAction("add", null);
        this.SetMaxImageWidth(id, true);
        this.ShowCommentBox("", obj);
    };

    this.AddNewComment = function() {
        this.obj = null;
        this.SetParentComment("");
        this.SetAction("add", null);
        this.SetMaxImageWidth();
        this.ShowCommentBox("", null);

        jq("#comment_attachments").html("");
        this.CurrentAttachID = 0;
        this.currentCommentID = "";
        jq('#hdnCommentID').val('');
    };

    this.EditComment = function(obj, id) {
        this.obj = obj.id.replace("edit_", "");
        this.SetParentComment("");
        this.SetAction("update", obj.id.replace("edit_", ""));
        this.SetMaxImageWidth(id);
        this.ShowCommentBox(obj.id.replace("edit_", ""));

        jq("#comment_attachments").html("");
        this.CurrentAttachID = 0;
        this.FillAttachments(id);
        this.currentCommentID = id;
    };

    this.ShowCommentBox = function(id) {
        jq('#add_comment_btn').hide();

        if (!this.isSimple) {
            var ContentDiv = document.getElementById('content_' + id);
            var iFCKEditor = FCKeditorAPI.GetInstance('CommentsFckEditor_' + this._jsObjName);

            if (ContentDiv != null) {
                iFCKEditor.SetHTML(ContentDiv.innerHTML);
                iFCKEditor.Config.RedirectUrlToUpload(this.FckUploadHandlerPath + "?esid=" + this.FckDomainName + "&iid=" + id);
            }
            else {
                iFCKEditor.SetHTML('');
                iFCKEditor.Config.RedirectUrlToUpload(this.FckUploadHandlerPath + "?esid=" + this.FckDomainName);
            }

            jq('#commentBox').show();
            iFCKEditor.Focus();
            iFCKEditor.Focus();

            jq(window).scrollTop(jq('#commentBox').position().top, { speed: 500 });
            jq('#previewBox').hide("slow");
        }
        else {
            //if (this.javaScriptLoadBBcodeCommentFunctionName == "") {
            var ContentDiv = document.getElementById('content_' + id);
            if (ContentDiv != null) {
                var text = TextHelper.Html2FormattedText(ContentDiv);
                jq('#simpleTextArea').val(text);
            }
            else {
                jq('#simpleTextArea').val('');
            }

            AjaxPro.onLoading = function(b) {
                if (b) {
                    CommentsManagerObj.BlockCommentsBox();
                }
                else {
                    CommentsManagerObj.UnblockCommentsBox();
                };
            }

            jq('#commentBox').show();
            jq('#simpleTextArea').focus();
            jq(window).scrollTop(jq('#commentBox').position().top, { speed: 500 });
            jq('#previewBox').hide("slow");

            //            }
            //            else {
            //                AjaxPro.onLoading = function(b) { };

            //                if (this.PID != "") {
            //                    eval(this.javaScriptLoadBBcodeCommentFunctionName + "(id, CommentsManagerObj.PID, CommentsManagerObj.callBackLoadComment)");
            //                }
            //                else {
            //                    eval(this.javaScriptLoadBBcodeCommentFunctionName + "(id, CommentsManagerObj.callBackLoadComment)");
            //                }
            //            }
        }
    };

    this.SetMaxImageWidth = function(id, responseFlag) {
        if (this.isSimple)
            return;

        var fckConfig = FCKeditorAPI.GetInstance('CommentsFckEditor_' + this._jsObjName).Config;
        if (id) {
            var width = jq('#content_' + id).width() - 30;
            fckConfig.MaxImageWidth = width;

            if (responseFlag) {
                var level = jq('#container_' + id).parents('div[id^="container_"]').length;
                if (level < this.maxLevel - 1) {
                    fckConfig.MaxImageWidth = width - 35;
                }
            }
        } else {
            fckConfig.MaxImageWidth = 0;
        }
    };

    this.callBackLoadComment = function(result) {
        if (result != null && result.value != '') {
            jq('#simpleTextArea').val(result.value);
        }
        else {
            jq('#simpleTextArea').val('');
        }

        jq('#simpleTextArea').focus();
        jq('#simpleTextArea').scrollTo();
        jq('#commentBox').show();
        jq('#previewBox').hide();
    };

    this.SetParentComment = function(value) {
        jq('#hdnParentComment').val(value);
    };

    this.SetAction = function(action, comment_id) {
        jq('#hdnAction').val(action);

        if (comment_id != null)
            jq('#hdnCommentID').val(comment_id);
    };

    this.callbackRemove = function(result) {
        if (result.value != null) {

            var html = "<div style='padding:10px;'>" + CommentsManagerObj.inactiveMessage + "</div>";
            jq('#comment_' + result.value).html(html);
            CommentsManagerObj.currentCommentID = result.value;
            CommentsManagerObj.CallActionHandlerJS('remove', 'CommentsManagerObj.UnblockCommentsBox');

        }
        CommentsManagerObj.Redraw();
        CommentsManagerObj.Cancel();

    };

    this.Cancel = function() {
        CommentsManagerObj.CallActionHandlerJS('cancel', 'CommentsManagerObj.UnblockCommentsBox');

        jq('#commentBox').hide();
        jq('#add_comment_btn').show();
    };

    this.CallActionHandlerJS = function(action, callBack) {
        switch (action) {
            case "add":
                if (CommentsManagerObj.OnEditedCommentJS != "" && !CommentsManagerObj.isSimple) {
                    var fckEd = FCKeditorAPI.GetInstance('CommentsFckEditor_' + CommentsManagerObj._jsObjName);
                    eval(CommentsManagerObj.OnEditedCommentJS + "('" + CommentsManagerObj.currentCommentID + "', fckEd.GetXHTML() , '" + CommentsManagerObj.FckDomainName + "', false, '" + callBack + "')");
                }
                return;

            case "edit":
                if (CommentsManagerObj.OnEditedCommentJS != "") {
                    var text = '';

                    if (CommentsManagerObj.isSimple)
                        text = jq('#simpleTextArea').val();
                    else {
                        var fckEd = FCKeditorAPI.GetInstance('CommentsFckEditor_' + CommentsManagerObj._jsObjName);
                        text = fckEd.GetXHTML();
                    }

                    eval(CommentsManagerObj.OnEditedCommentJS + "('" + CommentsManagerObj.currentCommentID + "', text, '" + CommentsManagerObj.FckDomainName + "', true, '" + callBack + "')");
                }
                return;

            case "remove":
                if (CommentsManagerObj.OnRemovedCommentJS != "")
                    eval(CommentsManagerObj.OnRemovedCommentJS + "('" + CommentsManagerObj.currentCommentID + "', '" + CommentsManagerObj.FckDomainName + "', '" + callBack + "')");
                return;

            case "cancel":
                if (CommentsManagerObj.OnCanceledCommentJS != "" && !CommentsManagerObj.isSimple)
                    eval(CommentsManagerObj.OnCanceledCommentJS + "('" + CommentsManagerObj.currentCommentID + "', '" + CommentsManagerObj.FckDomainName + "', (CommentsManagerObj.currentCommentID != ''), '" + callBack + "')");
                return;
        }

        eval(callBack + "()");
    };


    this.CallFCKComplete = function() {
        if (jq('#hdnAction').val() == "update") {
            CommentsManagerObj.CallActionHandlerJS('edit', 'CommentsManagerObj.UnblockCommentsBox');
        }
        else {
            CommentsManagerObj.CallActionHandlerJS('add', 'CommentsManagerObj.UnblockCommentsBox');
        }
    };

    this.AddComment_Click = function() {
        if (this.isSimple) {

            if (jq('#simpleTextArea').val() != "") {
                AjaxPro.onLoading = function(b) {
                    if (b) {
                        CommentsManagerObj.BlockCommentsBox();
                    }
                    else {
                        CommentsManagerObj.UnblockCommentsBox();
                    };
                }

                var postText = TextHelper.Text2EncodedHtml(jq('#simpleTextArea').val());

                if (jq('#hdnAction').val() == "add") {

                    var strMethod = this.javaScriptAddCommentFunctionName + "(jq('#hdnParentComment').val(), jq('#hdnObjectID').val(), postText, ";

                    if (this.PID != "") {
                        strMethod += "CommentsManagerObj.PID, ";
                    }

                    if (this.EnableAttachmets) {
                        strMethod += "CommentsManagerObj.GetAttachments(), ";
                    }

                    strMethod += "CommentsManagerObj.callBackAddComment)";

                    eval(strMethod);
                }
                else if (jq('#hdnAction').val() == "update") {
                    var strMethod = this.javaScriptUpdateCommentFunctionName + "(jq('#hdnCommentID').val(), postText, ";

                    if (this.PID != "") {
                        strMethod += "CommentsManagerObj.PID, ";
                    }

                    if (this.EnableAttachmets) {
                        strMethod += "CommentsManagerObj.GetAttachments(), ";
                    }

                    strMethod += "CommentsManagerObj.callBackUpdateComment)";

                    eval(strMethod);
                }
            }
            else {
                alert(jq('#EmptyCommentErrorMessage').val());
                return false;
            }
        }
        else {
            if (CancelButtonController.IsEmptyFckEditorTextField('CommentsFckEditor_' + this._jsObjName)) {
                alert(jq('#EmptyCommentErrorMessage').val());
                return false;
            }

            CommentsManagerObj.iFCKEditor = FCKeditorAPI.GetInstance('CommentsFckEditor_' + this._jsObjName);
            var text = CommentsManagerObj.iFCKEditor.GetXHTML();
            text = CommentsManagerObj.iFCKEditor.GetXHTML();

            if (text != "") {
                AjaxPro.onLoading = function(b) {
                    if (b) {
                        CommentsManagerObj.BlockCommentsBox();
                    }
                }

                if (jq('#hdnAction').val() == "add") {
                    var strMethod = this.javaScriptAddCommentFunctionName + "(jq('#hdnParentComment').val(), jq('#hdnObjectID').val(), CommentsManagerObj.iFCKEditor.GetXHTML(), "

                    if (this.PID != "") {
                        strMethod += "CommentsManagerObj.PID, ";
                    }

                    if (this.EnableAttachmets) {
                        strMethod += "CommentsManagerObj.GetAttachments(), ";
                    }

                    strMethod += "CommentsManagerObj.callBackAddComment)";

                    eval(strMethod);
                }
                else if (jq('#hdnAction').val() == "update") {
                    var strMethod = this.javaScriptUpdateCommentFunctionName + "(jq('#hdnCommentID').val(), CommentsManagerObj.iFCKEditor.GetXHTML(), "

                    if (this.PID != "") {
                        strMethod += "CommentsManagerObj.PID, ";
                    }

                    if (this.EnableAttachmets) {
                        strMethod += "CommentsManagerObj.GetAttachments(), ";
                    }

                    strMethod += "CommentsManagerObj.callBackUpdateComment)";

                    eval(strMethod);
                }
            }
        }
          jq('#noComments').hide();
    };

    this.BlockCommentsBox = function() {
        this.mainLoader = jq.blockUI.defaults.message;
        jq.blockUI.defaults.message = '';
        jq('#comments_btns').hide();
        jq('#comments_loader').show();
        jq.blockUI();
    };

    this.UnblockCommentsBox = function() {
        jq('#comments_btns').show();
        jq('#comments_loader').hide();
        jq.blockUI.defaults.message = CommentsManagerObj.mainLoader;
        jq.unblockUI();
    };

    this.callBackAddComment = function(result) {
        var res = result.value;
        if (res == null) return;

        if (res.rs10 == "postDeleted") {
            window.location = res.rs11;
            return;
        }

        if (res.rs1 == "") {
            jq('#mainContainer').append(res.rs2);
        }
        else {
            var level = false;

            var obj = jq('#container_' + res.rs1).parent();

            for (var i = CommentsManagerObj.maxLevel; i > 1; i--) {
                if (obj.attr("id") == "mainContainer") {
                    level = true;
                    break;
                }
                obj = obj.parent();
            }
            var html = res.rs2;
            if (level == false)
                html = html.replace("margin-left: 35px;", "");

            jq('#container_' + res.rs1).append(html);
        }

        jq('#mainContainer').show();
        jq('#commentsTitle').show();
        jq('#add_comment_btn').show();

        if (this.javaScriptCallBackAddComment != "")
            eval(this.javaScriptCallBackAddComment);

        jq('#commentBox').hide("slow");
        CommentsManagerObj.Redraw();

        var obj;

        if (res.rs1 == "") {
            obj = jq('#mainContainer > div:last-child').children();
        }
        else {
            obj = jq('#container_' + res.rs1 + ' > div:last-child').children();
        }

        CommentsManagerObj.currentCommentID = obj.attr("id").replace("comment_", "");


        jq(window).scrollTop(obj.position().top, { speed: 500 });

        var endbackgroundColor;
        if (obj.hasClass('tintMedium')) {
            endbackgroundColor = '#fff';
            obj.removeClass('tintMedium');
        }
        else
            endbackgroundColor = '#ffffff';


        obj.css({ "background-color": "#ffffcc" });
        obj.animate({ backgroundColor: endbackgroundColor }, 1000);
        //   obj.hover(obj.css({ "background-color": "#f2f2f2" }));

        CommentsManagerObj.CallFCKComplete();
    };

    this.callBackUpdateComment = function(result) {
        var res = result.value;
        if (res == null) return;

        jq('#content_' + res.rs1).html(res.rs2);

        jq('#commentBox').hide("slow");
        jq('#add_comment_btn').show();

        var obj = jq('#comment_' + res.rs1);

        CommentsManagerObj.currentCommentID = obj.attr("id").replace("comment_", "");

        jq(window).scrollTop(obj.position().top, { speed: 500 });
        
        if (obj.hasClass('tintMedium')) {
            endbackgroundColor = '#fff';
            obj.removeClass('tintMedium');
        }
        else
            endbackgroundColor = '#ffffff';


        obj.css({ "background-color": "#ffffcc" });
        obj.animate({ backgroundColor: endbackgroundColor }, 1000);

        CommentsManagerObj.CallFCKComplete();
    };

    this.Preview_Click = function() {
        if (this.isSimple) {
            var html = TextHelper.Text2EncodedHtml(jq('#simpleTextArea').val());
            jq('#previewBoxBody').html(html);
            jq('#previewBox').show();
            jq(window).scrollTop(jq('#previewBox').position().top, { speed: 500 });
        }
        else {
            AjaxPro.onLoading = function(b) {
                if (b) {
                    CommentsManagerObj.BlockCommentsBox();
                }
                else {
                    CommentsManagerObj.UnblockCommentsBox();
                };
            };

            this.iFCKEditor = FCKeditorAPI.GetInstance('CommentsFckEditor_' + this._jsObjName);
            eval(this.javaScriptPreviewCommentFunctionName + "(CommentsManagerObj.iFCKEditor.GetXHTML(true), jq('#hdnCommentID').val(), CommentsManagerObj.callBackPreview)");
        }
    };

    this.callBackPreview = function(result) {
        jq('#previewBoxBody').html(result.value);
        jq('#previewBox').show();
        jq(window).scrollTop(jq('#previewBox').position().top, { speed: 500 });
    };

    this.HidePreview = function() {
        jq(window).scrollTop(jq('#commentBox').position().top, { speed: 500 });
        jq('#previewBox').hide("slow");
    };

    this.UploadCallBack = function(file, response) {
        var result = eval("(" + response + ")");

        if (result.Success) {
            var html = jq("#comment_attachments").html();

            html += "<span id='attach_" + CommentsManagerObj.CurrentAttachID + "'>";

            if (CommentsManagerObj.CurrentAttachID > 0)
                html += ", ";

            html += file + " <a class='linkDescribe' href='javascript:CommentsManagerObj.RemoveAttach(" + CommentsManagerObj.CurrentAttachID + ");'>" + CommentsManagerObj.RemoveAttachButton + "<input name='attach_comments' type='hidden' value='" + result.Data + "' /></a></span>";

            CommentsManagerObj.CurrentAttachID++;
            jq("#comment_attachments").html(html);
        }
    };

    this.FillAttachments = function(commentID) {
        var listNames = jq("input[name^='attacment_name_" + commentID + "']");
        var listPaths = jq("input[name^='attacment_path_" + commentID + "']");
        var html = "";

        for (var i = 0; i < listNames.length; i++) {
            html += "<span id='attach_" + CommentsManagerObj.CurrentAttachID + "'>";

            if (CommentsManagerObj.CurrentAttachID > 0)
                html += ", ";

            html += listNames[i].value + " <a class='linkDescribe' href='javascript:CommentsManagerObj.RemoveAttach(" + CommentsManagerObj.CurrentAttachID + ");'>" + CommentsManagerObj.RemoveAttachButton + "<input name='attach_comments' type='hidden' value='" + listPaths[i].value + "' /></a></span>";
            CommentsManagerObj.CurrentAttachID++;

        }
        jq("#comment_attachments").html(html);

    };

    this.GetAttachments = function() {
        var list = jq("input[name^='attach_comments']");
        var result = "";

        for (var i = 0; i < list.length; i++) {
            if (i != 0)
                result += ";"

            result += list[i].value;
        }

        return result;
    };

    this.RemoveAttach = function(id) {
        jq('#attach_' + id).remove();
    };
}
