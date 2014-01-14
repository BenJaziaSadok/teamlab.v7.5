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

/*sharing data sample
    {
        actions : [{name: 'actionName',
                    id  : 'id',
                    defaultAction : 'true/false'
                  }],
                  
        items : [{
            id : 'itemId',
            data : 'customData',
            isGroup : true,
            name : 'name',
            canEdit : true/false,
            hideRemove : true/false,
            selectedAction : {
                                name: 'actionName',
                                id  : 'id'
                             }
            
        }]
    }
*/

var SharingSettingsManager = function (elementId, sharingData) {

    var clone = function (o) {
        if (!o || 'object' !== typeof o) {
            return o;
        }

        var c = 'function' === typeof o.pop ? [] : {};
        var p, v;
        for (p in o) {
            if (o.hasOwnProperty(p)) {
                v = o[p];
                if (v && 'object' === typeof v) {
                    c[p] = clone(v);
                } else {
                    c[p] = v;
                }
            }
        }
        return c;
    };

    this.OnSave = null;

    var _data = sharingData;
    var _workData = clone(sharingData);

    var _manager = this;
    jq(function () {
        if (elementId != undefined) {
            jq('#' + elementId).click(function () {
                _manager.ShowDialog();
            });
        }

        jq('#sharingSettingsSaveButton').click(_manager.SaveAndCloseDialog);
        jq("#studio_sharingSettingsDialog").on("click", ".sharing-cancel-button", _manager.CloseDialog);

        jq('#studio_sharingSettingsDialog').on('click', ".removeItem", function () {
            RemoveItem(jq(this));
        });
        jq('#sharingSettingsItems').on('change', ".action select", function () {
            ChangeItemAction(jq(this));
        });

        jq("#sharingSettingsItems").on("click", ".combobox-title", function () {
            jq("#sharingSettingsItems").scrollTo(jq(".combobox-container:visible"));
        });

        jq("#shareAddMessage").on("click", function () {
            showShareMessage();
            return false;
        });

        jq("#shareRemoveMessage").on("click", function () {
            hideShareMessage();
            jq("#shareMessage").val("");
            return false;
        });

        jq("#shareMessageSend").on("change", function () {
            if (!jq("#shareMessageSend").is(":checked")) {
                hideShareMessage();
            }
        });

        hideShareMessage();
    });

    var hideShareMessage = function () {
        jq("#shareRemoveMessage").hide();
        jq("#sharingSettingsDialogBody").removeClass("with-message");
        jq("#shareAddMessage").show();
    };

    var showShareMessage = function () {
        jq("#shareAddMessage").hide();
        jq("#shareRemoveMessage").show();
        jq("#sharingSettingsDialogBody").addClass("with-message");
        jq("#shareMessageSend").prop("checked", true);
    };

    var ChangeItemAction = function (el) {
        var itemId = jq(el).attr('data'),
            actId = jq(el).val(),

            act = null;
        for (var i = 0; i < _workData.actions.length; i++) {
            if (_workData.actions[i].id == actId) {
                act = _workData.actions[i];
                break;
            }
        }

        for (var i = 0; i < _workData.items.length; i++) {
            if (_workData.items[i].id == itemId) {

                _workData.items[i].selectedAction = act;
                break;
            }
        }
    };

    var RemoveItem = function (el) {
        var itemId = jq(el).attr('data');

        for (var i = 0; i < _workData.items.length; i++) {
            if (_workData.items[i].id == itemId) {
                _workData.items.splice(i, 1);
                break;
            }
        }

        jq("#sharing_item_" + itemId).remove();
        jq("#sharingSettingsItems div.sharingItem.tintMedium").removeClass("tintMedium");
        jq("#sharingSettingsItems div.sharingItem:even").addClass("tintMedium");

        shareUserSelector.HideUser(itemId, false);
        window.shareGroupSelector.HideGroup(itemId, false);
    };

    var AddUserItem = function (userId, userName) {

        var defAct = null;
        for (var i = 0; i < _workData.actions.length; i++) {
            if (_workData.actions[i].defaultAction) {
                defAct = _workData.actions[i];
                break;
            }
        }
        var newItem = { id: userId, name: userName, selectedAction: defAct, isGroup: false, canEdit: true };
        _workData.items.push(newItem);

        jq('#sharingSettingsItems').append(jq.tmpl("sharingListTemplate", { items: [newItem], actions: _workData.actions }));
        jq('#studio_sharingSettingsDialog .action select:last').tlcombobox();

        jq("#sharingSettingsItems div.sharingItem.tintMedium").removeClass("tintMedium");
        jq("#sharingSettingsItems div.sharingItem:even").addClass("tintMedium");

        shareUserSelector.HideUser(userId, true);
    };

    var AddGroupItem = function (group) {
        var defAct = null;
        for (var i = 0; i < _workData.actions.length; i++) {
            if (_workData.actions[i].defaultAction) {
                defAct = _workData.actions[i];
                break;
            }
        }
        var newItem = { id: group.Id, name: group.Name, selectedAction: defAct, isGroup: true, canEdit: true };
        _workData.items.push(newItem);

        jq('#sharingSettingsItems').append(jq.tmpl("sharingListTemplate", { items: [newItem], actions: _workData.actions }));
        jq('#studio_sharingSettingsDialog .action select:last').tlcombobox();

        jq("#sharingSettingsItems div.sharingItem.tintMedium").removeClass("tintMedium");
        jq("#sharingSettingsItems div.sharingItem:even").addClass("tintMedium");

        window.shareGroupSelector.HideGroup(group.Id, true);
    };

    var ReDrawItems = function () {

        jq('#sharingSettingsItems').html(jq.tmpl("sharingListTemplate", _workData));

        if (jq.browser.mobile) {
            jq("#sharingSettingsItems").addClass("isMobileAgent");
        }

        jq('#studio_sharingSettingsDialog .action select').each(function () {
            jq(this).tlcombobox();
        });

        shareUserSelector.AdditionalFunction = AddUserItem;
        shareUserSelector.DisplayAll();
        shareUserSelector.ChangeDepartment("00000000-0000-0000-0000-000000000000");

        window.shareGroupSelector.AdditionalFunction = AddGroupItem;
        window.shareGroupSelector.DisplayAll();

        for (var i = 0; i < _workData.items.length; i++) {
            var item = _workData.items[i];
            if (item.isGroup) {
                window.shareGroupSelector.HideGroup(item.id, true);
            } else {
                shareUserSelector.HideUser(item.id, true);
            }
        }
    };

    this.UpdateSharingData = function (data) {
        _data = data;
        _workData = clone(data);
    };

    this.GetSharingData = function () {
        return _data;
    };

    this.ShowDialog = function (width, asFlat) {
        ReDrawItems();
        hideShareMessage();
        jq("#shareMessage").val("");
        jq("#shareMessageSend").prop("checked", true);

        width = width || 600;

        if (!asFlat) {
            jq.blockUI({
                message: jq("#studio_sharingSettingsDialog"),
                css: {
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: (width || 600) + 'px',
                    height: 'auto',
                    "min-height": '600px',
                    cursor: 'default',
                    textAlign: 'left',
                    backgroundColor: 'transparent',
                    marginLeft: -width / 2 + 'px',
                    left: '50%',
                    top: '50%',
                    position: "absolute",
                    overflow: "visible",
                    "overflow-x": (jq.browser.msie && jq.browser.version < 8) ? "auto" : "visible",
                    "overflow-y": (jq.browser.msie && jq.browser.version < 8) ? "auto" : "visible",
                    "margin-top": (jq(window).scrollTop() - 300) + "px"
                },
                overlayCSS: {
                    backgroundColor: '#AAA',
                    cursor: 'default',
                    opacity: '0.3'
                },
                focusInput: true,
                fadeIn: 0,
                fadeOut: 0
            });
        } else {
            jq("#studio_sharingSettingsDialog").show()
                .css({ "display": "block" });
        }

        PopupKeyUpActionProvider.EnterAction = "jq('#sharingSettingsSaveButton').click();";
    };

    this.SaveAndCloseDialog = function () {
        _data = _workData;

        if (_manager.OnSave != null) {
            _manager.OnSave(_data);
        }

        _manager.CloseDialog();
        return false;
    };

    this.CloseDialog = function () {
        PopupKeyUpActionProvider.CloseDialog();
        return false;
    };
};