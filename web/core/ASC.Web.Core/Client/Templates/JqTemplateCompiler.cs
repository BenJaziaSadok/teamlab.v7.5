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

using System.IO;
using Jurassic;

namespace ASC.Web.Core.Client.Templates
{
    public class JqTemplateCompiler
    {
        private readonly ScriptEngine _engine;

        public JqTemplateCompiler()
        {
            
            _engine = new ScriptEngine();
            using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("ASC.Web.Core.Client.Templates.jqTemplateCompiler.js")))
            {
                _engine.Execute(reader.ReadToEnd());
            }
        }

        public string GetCompiledCode(string source)
        {
            return _engine.CallGlobalFunction<string>("buildTmplFn", source);
        }
    }
}