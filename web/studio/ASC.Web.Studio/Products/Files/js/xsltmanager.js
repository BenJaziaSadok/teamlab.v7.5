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

window.ASC.Controls.XSLTManager = (function () {
    var supportActiveXObject = false;
    try {
        var test1 = new ActiveXObject('Microsoft.XMLDOM');
        var test2 = new ActiveXObject('Microsoft.XMLHTTP');
        supportActiveXObject = test1 && test2;
    } catch (e) {
    }

    var loadXML = (function () {
        if (supportActiveXObject) {
            return function (file) {
                if (typeof file !== 'string' || file.length === 0) {
                    return undefined;
                }
                var xhttp = new ActiveXObject('Microsoft.XMLHTTP');
                xhttp.open('GET', file, false);
                xhttp.send('');
                return xhttp.responseXML;
            };
        }

        return function (file) {
            if (typeof file !== 'string' || file.length === 0) {
                return undefined;
            }
            var xhttp = new XMLHttpRequest();
            xhttp.open('GET', file, false);
            xhttp.send('');
            return xhttp.responseXML;
        };
    })();

    var createXML = (function () {
        if (supportActiveXObject) {
            return function (data) {
                if (typeof data !== 'string' || data.length === 0) {
                    return undefined;
                }
                var xmlDoc = new ActiveXObject('Microsoft.XMLDOM');
                xmlDoc.async = 'false';
                xmlDoc.loadXML(data);
                if (xmlDoc.parseError.errorCode != 0) {
                    throw 'Can\'t create xml document';
                }
                return xmlDoc;
            };
        }

        return function (data) {
            if (typeof data !== 'string' || data.length === 0) {
                return undefined;
            }
            var xmlDoc = new DOMParser();
            try {
                xmlDoc = xmlDoc.parseFromString(data, 'text/xml');
            } catch (err) {
                throw 'Can\'t create xml document : ' + err;
            }
            return xmlDoc;
        };
    })();

    var translateFromFile = function (xml, xsl) {
        if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
            return '';
        }
        if (typeof xml === 'string') {
            xml = loadXML(xml);
        }
        if (typeof xsl === 'string') {
            xsl = loadXML(xsl);
        }
        return xmlTranslate(xml, xsl);
    };

    var translateFromString = function (xml, xsl) {
        if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
            return '';
        }
        if (typeof xml === 'string') {
            xml = createXML(xml);
        }
        if (typeof xsl === 'string') {
            xsl = createXML(xsl);
        }

        return xmlTranslate(xml, xsl);
    };

    var xmlTranslate = (function () {
        if (supportActiveXObject) {
            return function (xml, xsl) {
                var xmlstr = '';
                if (typeof xml === 'undefined' || xml == null || typeof xsl === 'undefined' || xsl == null) {
                    return xmlstr;
                }
                try {
                    xsl.resolveExternals = true;
                } catch (err) {
                }
                try {
                    xmlstr = xml.transformNode(xsl);
                } catch (err) {
                    throw 'Can\'t translate xml : ' + err;
                }
                return xmlstr;
            };
        }

        return function (xml, xsl) {
            var xmlstr = '';
            if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
                return xmlstr;
            }
            var xmlDocument;
            var xsltProcessor;
            var xmlSerializer;
            try {
                xsltProcessor = new XSLTProcessor();
                xsltProcessor.importStylesheet(xsl);
                xmlDocument = xsltProcessor.transformToFragment(xml, document);
            } catch (err) {
                throw 'Can\'t translate xml : ' + err;
            }
            try {
                xmlSerializer = new XMLSerializer();
                xmlstr = xmlSerializer.serializeToString(xmlDocument);
            } catch (err) {
                throw 'Can\'t serialized xml : ' + err;
            }
            return xmlstr;
        };
    })();

    return {
        createXML: createXML,
        loadXML: loadXML,

        translate: xmlTranslate,
        translateFromFile: translateFromFile,
        translateFromString: translateFromString
    };
})();