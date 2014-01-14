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

using System.Collections.Generic;
using ASC.Files.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Web.Projects.Classes
{
    internal class FileEngine2
    {
        private static FileEngine Engine
        {
            get { return Global.EngineFactory.GetFileEngine(); }
        }

        public static object GetRoot(int projectId)
        {
            return Engine.GetRoot(projectId);
        }

        public static File GetFile(int id, int version)
        {
            return Engine.GetFile(id, version);
        }

        public static File SaveFile(File file, System.IO.Stream stream)
        {
            return Engine.SaveFile(file, stream);
        }

        public static void RemoveFile(object id)
        {
            Engine.RemoveFile(id);
        }

        public static Folder SaveFolder(Folder folder)
        {
            return Engine.SaveFolder(folder);
        }

        public static void AttachFileToMessage(Message message, object fileId)
        {
            Global.EngineFactory.GetMessageEngine().AttachFile(message, fileId, false);
        }

        public static void AttachFileToTask(Task task, object fileId)
        {
            Global.EngineFactory.GetTaskEngine().AttachFile(task, fileId, false);
        }

        public static IEnumerable<File> GetTaskFiles(Task task)
        {
            return Global.EngineFactory.GetTaskEngine().GetFiles(task);
        }

        public static IEnumerable<File> GetMessageFiles(Message message)
        {
            return Global.EngineFactory.GetMessageEngine().GetFiles(message);
        }
    }
}