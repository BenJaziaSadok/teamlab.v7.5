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

;
(function($) {
    // binds
    function onButtonClick(evt) {
        var $this = jq(this);

        peopleActions.callMethodByClassname($this.attr('class'), this, [evt, $this]);
        jq(document.body).trigger('click');
    }

    function onChangeGroup(evt, groupId) {
        var $container = $('#peopleSidepanel');
        groupId = groupId || '@persons';
        jq(".profile-title .menu-small").hide();
        jq(".profile-title .header").html(PeopleManager.DefaultHeader);
        document.title = PeopleManager.DefaultHeader + " - " + PeopleManager.DefaultHeader;
        jq(".profile-title .header").attr("title", PeopleManager.DefaultHeader);
        switch (groupId) {
            case '@persons':
                $container.find('li.menu-sub-item.active').removeClass('active');
                $container.find('li.menu-item').removeClass('active').filter('li[data-id="' + groupId + '"]:first').addClass('active open');
                break;
            case '@clients':
                break;
            case '@freelancers':
                break;
            default:
                $container.find('li.menu-item.active').removeClass('active');
                $container.find('li.menu-sub-item').removeClass('active').filter('li[data-id="' + groupId + '"]:first').addClass('active');
                var nameGroup = jq(".menu-sub-list li.active a").html();
                jq(".profile-title .header").html(nameGroup).attr("title", nameGroup);
                document.title = htmlDecode(nameGroup) + " - " + PeopleManager.DefaultHeader;
                jq(".profile-title .menu-small").show();
                jq.dropdownToggle({
                    dropdownID: "actionGroupMenu",
                    switcherSelector: ".profile-title .menu-small",
                    addTop: 4,
                    addLeft: -11,
                    showFunction: function(switcherObj, dropdownItem) {
                        if (dropdownItem.is(":hidden")) {
                            switcherObj.addClass('active');
                        } else {
                            switcherObj.removeClass('active');
                        }
                    },
                    hideFunction: function() {
                        jq(".profile-title .menu-small.active").removeClass("active");
                    }
                });
                break;
        }
    }
    function htmlDecode(value) {
        return $('<div/>').html(value).text();
    }

    function initToolbar() {
        var $buttons = $("#peopleSidepanel").find("a.dropdown-item");
        $buttons.bind("click", onButtonClick);

        //var $b = actionGroupMenu
        jq("#actionGroupMenu").find("a.dropdown-item").bind("click", onButtonClick);
    }

    function initMenuList() {
        $(window).bind("change-group", onChangeGroup);
    }

    $(function() {
        initToolbar();
        initMenuList();
    });
})(jQuery);