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

using System.Text;
using System.Web.Optimization;
using Yahoo.Yui.Compressor;

namespace ASC.Web.Core.Client.Bundling
{
    class JsTransform : IItemTransform, IBundleTransform
    {
        private readonly bool obfuscate = false;


        public JsTransform()
            : this(true)
        {
        }

        public JsTransform(bool obfuscate)
        {
            this.obfuscate = obfuscate;
        }


        public string Process(string includedVirtualPath, string input)
        {
            if (BundleTable.EnableOptimizations)
            {
                var compressor = new JavaScriptCompressor
                {
                    CompressionType = CompressionType.Standard,
                    Encoding = Encoding.UTF8,
                    ObfuscateJavascript = obfuscate,
                };
                input = compressor.Compress(input);
            }
            return input;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            response.ContentType = "application/javascript";
        }
    }
}