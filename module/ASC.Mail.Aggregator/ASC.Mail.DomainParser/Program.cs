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
using System.Configuration;
using System.Linq;
using System.IO;
using ASC.Mail.Aggregator;

namespace ASC.Mail.DomainParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Begin");

            var parse_path = @"../../XmlData";

            try
            {
                var manger = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 25);

                var list_of_config = new List<clientConfig>();

                if (args != null && args.Any())
                {
                    parse_path = args[0];
                }

                Console.WriteLine("\r\nParser path: '{0}'", parse_path);

                if (File.GetAttributes(parse_path) == FileAttributes.Directory)
                {
                    var parse_path_info = new DirectoryInfo(parse_path);

                    var files = parse_path_info.GetFiles();

                    Console.WriteLine("\r\n{0} file(s) found!", files.Count());
                    Console.WriteLine("");

                    var index = 0;
                    var count = files.Count();

                    files
                        .ToList()
                        .ForEach(f =>
                        {
                            if (f.Attributes == FileAttributes.Directory) return;
                            clientConfig obj;
                            if (!ParseXml(f.FullName, out obj)) return;
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write("                                 ");
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write("{0} from {1}", ++index, count);
                            list_of_config.Add(obj);
                        });
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("\r\n1 file found!");

                    clientConfig obj;
                    if (ParseXml(parse_path, out obj))
                    {
                        list_of_config.Add(obj);
                    }
                }

                Console.WriteLine("\r\n{0} config(s) parsed!", list_of_config.Count);

                if (list_of_config.Count > 0)
                {
                    do
                    {
                        Console.Write("\r\nDo you want add configs to DB? [y, n]: ");
                        var info = Console.ReadKey();
                        if (info.Key == ConsoleKey.Y)
                        {
                            var index = 0;
                            var count = list_of_config.Count;
                            
                            Console.WriteLine("\r\n");

                            list_of_config.ForEach(c =>
                            {
                                Console.Write("{0} from {1}", ++index, count);

                                if (!manger.SetMailBoxSettings(c)) return;
                                if (index >= count) return;
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write("                                 ");
                                Console.SetCursorPosition(0, Console.CursorTop);
                            });

                            Console.WriteLine("\r\n");
                            Console.WriteLine("{0} config(s) added to DB!", list_of_config.Count);
                            Console.WriteLine("");
                            break;
                        }
                        if (info.Key != ConsoleKey.N) continue;
                        Console.WriteLine("\r\n");
                        break;
                    } while (true);
                }

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Such path not exists: '{0}'", parse_path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(".End");
            Console.ReadKey();
        }

        static bool ParseXml(string filepath, out clientConfig obj)
        {
            obj = null;

            try
            {
                obj = clientConfig.LoadFromFile(filepath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
