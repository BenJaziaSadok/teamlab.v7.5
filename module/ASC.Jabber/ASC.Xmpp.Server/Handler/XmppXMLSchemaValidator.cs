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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Handler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using log4net;
    using Storage;
    using Streams;


    public class XmppXMLSchemaValidator
    {
        private static readonly Dictionary<string, XmlSchema> schemaSet = new Dictionary<string, XmlSchema>();
        private static readonly ILog log = LogManager.GetLogger(typeof(XmppXMLSchemaValidator));

        public XmppXMLSchemaValidator()
        {
#if (SCHEME)
            log.Debug("loading xsd");
            if (schemaSet.Count == 0)
            {
                LoadSchemes();
            }
#endif

        }

        private void LoadSchemes()
        {
            string schemaDir = Path.GetFullPath(".\\schemas\\");
            if (Directory.Exists(schemaDir))
            {
                string[] schemas = Directory.GetFiles(schemaDir, "*.xsd", SearchOption.TopDirectoryOnly);
                foreach (var schemaPath in schemas)
                {
                    //Load
                    try
                    {
                        using (TextReader reader = File.OpenText(schemaPath))
                        {
                            XmlSchema schema = XmlSchema.Read(reader, XmppSchemeValidationEventHandler);
                            if (!schemaSet.ContainsKey(schema.TargetNamespace))
                            {
#pragma warning disable 618,612
                                schema.Compile(XmppSchemeValidationEventHandler);
#pragma warning restore 618,612
                                schemaSet.Add(schema.TargetNamespace, schema);

                            }
                        }
                    }
                    catch (Exception)
                    {
                        log.ErrorFormat("error loading scheme {0}", schemaPath);
                    }
                }
            }
        }

       

        private void XmppSchemeValidationEventHandler(object sender, ValidationEventArgs e)
        {
            //log.DebugFormat("{1} validation:{0}", e.Message, e.Severity);
        }


        public bool ValidateNode(Node node, XmppStream stream, XmppHandlerContext handlercontext)
        {
#if (SCHEME)
            var stopwatch = new Stopwatch();
            if (log.IsDebugEnabled)
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
            int  result = ValidateNodeInternal(node);
            if (log.IsDebugEnabled)
            {
                stopwatch.Stop();
                log.DebugFormat("Node validated. Error count: {1}. time: {0}ms", stopwatch.ElapsedMilliseconds, result);
            }
            return result==0;
#else
            return true;
#endif
            
        }


        private int ValidateNodeInternal(Node node)
        {
            int[] errorcount = {0};
            try
            {

                if (node is Element)
                {
                    List<XmlSchema> schemasUsed = new List<XmlSchema>();
                    AddSchemas(node as Element, schemasUsed);
                    if (schemasUsed.Count > 0)
                    {
                        using (StringReader reader = new StringReader(node.ToString()))
                        {
                            StringBuilder validationErrors = new StringBuilder();
                            XmlReaderSettings xmppSettings = new XmlReaderSettings
                                                                 {ValidationType = ValidationType.Schema};
                            xmppSettings.ValidationEventHandler += (x, y) =>
                                                                       {
                                                                           validationErrors.AppendLine(
                                                                               GetErrorString(x, y));
                                                                           errorcount[0]++;
                                                                       };
                        
                        
                            foreach (XmlSchema schema in schemasUsed)
                            {
                                xmppSettings.Schemas.Add(schema);
                            }


                            XmlReader validator = XmlReader.Create(reader, xmppSettings);
                            bool bContinue = true;
                            while (bContinue)
                            {
                                try
                                {

                                    bContinue = validator.Read();
                                }
                                catch (Exception ex)
                                {
                                    errorcount[0]++;
                                    validationErrors.AppendLine(ex.Message);
                                }
                            }
                            if (validationErrors.Length > 0)
                            {
                                log.DebugFormat("validation summary:{1} {0}", validationErrors.ToString(), Environment.NewLine);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return errorcount[0];
        }

        private string GetErrorString(object sender, ValidationEventArgs args)
        {
            return string.Format("{0} at line {1} symbol {2}. Namespace {3}",
                                 args.Message,
                                 args.Exception.LineNumber,
                                 args.Exception.LinePosition, ((XmlReader)sender).NamespaceURI);
        }


        private void AddSchemas(Element element, List<XmlSchema> schemasUsed)
        {
            if (schemaSet.ContainsKey(element.Namespace))
            {
                if (!schemasUsed.Contains(schemaSet[element.Namespace]))
                {
                    schemasUsed.Add(schemaSet[element.Namespace]);
                }
            }
            if (element.HasChildElements)
            {
                foreach (Node childNode in element.ChildNodes)
                {
                    if (childNode is Element)
                    {
                        AddSchemas(childNode as Element, schemasUsed);
                    }
                }
            }
        }
    }
}