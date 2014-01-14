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

if (typeof (ASC) == 'undefined')
    ASC = {};
if (typeof (ASC.Controls) == 'undefined')
    ASC.Controls = {};

window.ASC.Controls.StartSteps = (function () {

    var projVideo,
        peopleVideo,
        docsVideo,
        crmVideo,
        arVideo;

    var init = function () {
        var culture = jq("#curCulture").text(),        
        protocol = location.protocol;

        switch (culture) {
            case "de-DE":
                projVideo = protocol + "//www.youtube.com/embed/FtGwZPaRY6k?autoplay=1";
                peopleVideo = protocol + "//www.youtube.com/embed/JuiojlK2e58?autoplay=1";
                docsVideo = protocol + "//www.youtube.com/embed/ZWUpl9DBCco?autoplay=1";
                crmVideo = protocol + "//www.youtube.com/embed/OPf8wV2m6lg?autoplay=1";
                arVideo = protocol + "//www.youtube.com/embed/cgmLl9ZxSL4?autoplay=1";
                break;
            case "es-ES":
                projVideo = protocol + "//www.youtube.com/embed/UPVb9LrctA0?autoplay=1";
                peopleVideo = protocol + "//www.youtube.com/embed/8_J9y9EEc5s?autoplay=1";
                docsVideo = protocol + "//www.youtube.com/embed/c4VZUxe1dcA?autoplay=1";
                crmVideo = protocol + "//www.youtube.com/embed/OrcRr-Wmfes?autoplay=1";
                arVideo = protocol + "//www.youtube.com/embed/NjSu1APX6Io?autoplay=1";
                break;
            case "fr-FR":
                projVideo = protocol + "//www.youtube.com/embed/Qn64XoOoZxM?autoplay=1";
                peopleVideo = protocol + "//www.youtube.com/embed/xbvpN0Jv8uM?autoplay=1";
                docsVideo = protocol + "//www.youtube.com/embed/WUOAcJPVF-g?autoplay=1";
                crmVideo = protocol + "//www.youtube.com/embed/1EF7BSibv2o?autoplay=1";
                arVideo = protocol + "//www.youtube.com/embed/cpWpIHFipdM?autoplay=1";
                break;
            case "it-IT":
                projVideo = protocol + "//www.youtube.com/embed/brDilp1I02k?autoplay=1";
                peopleVideo = protocol + "//www.youtube.com/embed/YptsfQoTqu8?autoplay=1";
                docsVideo = protocol + "//www.youtube.com/embed/A3nWJAk1o6s?autoplay=1";
                crmVideo = protocol + "//www.youtube.com/embed/Yyt8MOrslHU?autoplay=1";
                arVideo = protocol + "//www.youtube.com/embed/SwJ9PnjPi1Y?autoplay=1";
                break;
            case "ru-RU":
                projVideo = protocol + "//www.youtube.com/embed/utTZDwXyyoE?autoplay=1";
                peopleVideo = protocol + "//www.youtube.com/embed/lI3dsjZd5e0?autoplay=1";
                docsVideo = protocol + "//www.youtube.com/embed/qJY3BaIHgyQ?autoplay=1";
                crmVideo = protocol + "//www.youtube.com/embed/Pme06OhkvGk?autoplay=1";
                arVideo = protocol + "//www.youtube.com/embed/CeutfrndjrE?autoplay=1";
                break;
            default:
                projVideo = protocol + "//www.youtube.com/embed/X9_8z-Y0uZM?autoplay=1";
                peopleVideo = protocol + "//www.youtube.com/embed/2fPVa1A93Pg?autoplay=1";
                docsVideo = protocol + "//www.youtube.com/embed/a2w-KmqbAsE?autoplay=1";
                crmVideo = protocol + "//www.youtube.com/embed/5vqW_1WWfzE?autoplay=1";
                arVideo = protocol + "//www.youtube.com/embed/YemscFLqgIo?autoplay=1";
                break;
        }

        var moduleValue = getParameterByName("module");

        if (moduleValue) {
            var choice = jq('.item-module.' + moduleValue).children('.default-module');
            jq(choice).addClass("choosed");

            setTimeout(UnlockButton, 60000);

            var module = jq(choice).attr('data-name');
            var link, relate;


            switch (module) {
                case "documents":
                    link = docsVideo;
                    relate = peopleVideo;
                    break;
                case "projects":
                    link = projVideo;
                    relate = arVideo;
                    break;
                case "crm":
                    link = crmVideo;
                    relate = arVideo;
                    break;
                default:
                    return undefined;
            }
            jq('#relatedVideo').on('click', function () {
                MatchVideo();
                if (jq('.choose-module-video iframe').attr('src') == arVideo) {
                    relate = peopleVideo;
                }
                jq('.choose-module-video iframe').attr('src', relate);
            });

            OnloadYoutube();

            jq('.choose-module-video iframe').attr('src', link);
            jq('#chooseDefaultModule').addClass('display-none');
            jq('#chooseVideoModule').removeClass('display-none');
            MatchVideo();
        }

        jq('.item-module').on('click', function () {
            var module = jq(this).children('.default-module').attr('data-name');
            window.location.replace('welcome.aspx?module=' + module);
        });
    }

    var UnlockButton = function() {
        var moduleUrl = jq(".default-module.choosed").attr('data-url');
        if (moduleUrl == null) {
            moduleUrl = jq('.item-module.documents').parent().attr('data-url');
        }
        jq("#continueVideoModule").attr('href', moduleUrl);

        jq("#continueVideoModule").removeClass("disable");
        jq("#continueVideoModule").text(jq("#continueVideoModule").attr("data-value"));
    };

    var IframeLoad = function() {
        setTimeout(UnlockButton, 60000);
        var frame = "<iframe id=\"docsFrame\" width=1 height=1 style=\"position: absolute; visibility: hidden;\" ></iframe>";
        jq("#listScriptStyle").append(frame);
        jq("#docsFrame").attr("src", jq("#listScriptStyle").attr("data-docs"));

        document.getElementsByTagName('iframe')[2].onload = function () {
            UnlockButton();
        };
    };

    var IframeCommonLoad = function () {
        var frame = "<iframe id=\"commonFrame\" width=1 height=1 style=\"position: absolute; visibility: hidden;\" ></iframe>";
        var module = getParameterByName("module");
        jq("#listScriptStyle").append(frame);
        jq("#commonFrame").attr("src", jq("#listScriptStyle").attr("data-common") + "?module=" + module);

        document.getElementsByTagName('iframe')[1].onload = function () {
            UnlockButton();
        };
    }

    var OnloadYoutube = function() {
        document.getElementsByTagName('iframe')[0].onload = function () {
            setTimeout(IframeCommonLoad, 5000);
            setTimeout(IframeLoad, 6000);
        };
    };

    var getParameterByName = function(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.search);
        if (results == null) {
            return "";
        } else {
            return decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var MatchVideo = function() {
        var id;
        switch (jq('.choose-module-video iframe').attr('src')) {
            case projVideo:
                id = "video-guides-3";
                break;
            case peopleVideo:
                id = "video-guides-5";
                break;
            case docsVideo:
                id = "video-guides-2";
                break;
            case crmVideo:
                id = "video-guides-4";
                break;
            case arVideo:
                id = "video-guides-1";
                break;
            default:
                return undefined;
        }
        AjaxPro.timeoutPeriod = 1800000;
        UserVideoGuideUsage.SaveWatchVideo([id]);
    };

    return {
        init: init
    };
})();

jq(document).ready(function() {
    ASC.Controls.StartSteps.init();
});