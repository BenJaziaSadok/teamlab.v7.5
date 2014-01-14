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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Newtonsoft.Json;
using TMResourceData.Model;

namespace TMResourceData
{
    public static class JsonManager
    {
        public static void UploadJson(string fileName, Stream fileStream, string projectName, string moduleName)
        {
            var culture = GetCultureFromFileName(fileName);

            string jsonString;
            using (var reader = new StreamReader(fileStream))
            {
                jsonString = reader.ReadToEnd();
            }
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

            var fileID = ResourceData.AddFile(fileName, projectName, moduleName);
            const string resourceType = "text";
            foreach (var key in jsonObj.Keys)
            {
                var word = new ResWord
                    {
                        Title = key,
                        ValueFrom = jsonObj[key],
                        ResFile = new ResFile {FileID = fileID}
                    };
                ResourceData.AddResource(culture, resourceType, DateTime.UtcNow, word, true, "Console");
            }
        }

        public static string ExportJson(string project, string module, List<string> languages, string exportPath)
        {
            using (var fastZip = new ZipFile())
            {
                var filter = new ResCurrent
                    {
                        Project = new ResProject {Name = project},
                        Module = new ResModule {Name = module}
                    };

                var zipDirectory = Directory.CreateDirectory(exportPath + module);
                foreach (var language in languages)
                {
                    filter.Language = new ResCulture {Title = language};

                    var words = GetResource.GetListResWords(filter, string.Empty).GroupBy(x => x.ResFile.FileID).ToList();
                    if (!words.Any())
                    {
                        Console.WriteLine("Error!!! Can't find appropriate project and module. Possibly wrong names!");
                        return null;
                    }

                    foreach (var fileWords in words)
                    {
                        var wordsDictionary = new Dictionary<string, string>();
                        foreach (var word in fileWords.OrderBy(x=>x.Title).Where(word => !wordsDictionary.ContainsKey(word.Title)))
                        {
                            wordsDictionary[word.Title] = word.ValueTo ?? word.ValueFrom;
                        }

                        var firstWord = fileWords.FirstOrDefault();
                        var fileName = firstWord == null ? module : Path.GetFileNameWithoutExtension(firstWord.ResFile.FileName);

                        var zipFileName = zipDirectory.FullName + "\\" + fileName
                                          + (language == "Neutral" ? string.Empty : "." + language) + ".json";
                        using (TextWriter writer = new StreamWriter(zipFileName))
                        {
                            var obj = JsonConvert.SerializeObject(wordsDictionary, Formatting.Indented);
                            writer.Write(obj);
                        }
                    }
                }

                var zipPath = exportPath + "\\" + module + ".zip";
                fastZip.AddDirectory(zipDirectory.FullName);
                fastZip.Save(zipPath);

                zipDirectory.Delete(true);
                return zipPath;
            }
        }

        private static string GetCultureFromFileName(string fileName)
        {
            var culture = "Neutral";
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            if (nameWithoutExtension != null && nameWithoutExtension.Split('.').Length > 1)
            {
                culture = nameWithoutExtension.Split('.')[1];
            }

            return culture;
        }
    }
}