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
using System.Runtime.Serialization;
using System.Diagnostics;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    [DataContract(Name = "operation_result")]
    [DebuggerDisplay("Id = {Id}, Op = {OperationType}, Progress = {Progress}, Result = {Result}, Error = {Error}")]
    public class FileOperationResult
    {
        public FileOperationResult()
        {
            Id = String.Empty;
            Processed = String.Empty;
            Progress = 0;
            Error = String.Empty;
        }

        public FileOperationResult(FileOperationResult fileOperationResult)
        {
            Id = fileOperationResult.Id;
            OperationType = fileOperationResult.OperationType;
            Progress = fileOperationResult.Progress;
            Source = fileOperationResult.Source;
            Result = fileOperationResult.Result;
            Error = fileOperationResult.Error;
            Processed = fileOperationResult.Processed;
        }

        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "operation", IsRequired = false)]
        public FileOperationType OperationType { get; set; }

        [DataMember(Name = "progress", IsRequired = false)]
        public int Progress { get; set; }

        [DataMember(Name = "source", IsRequired = false)]
        public string Source { get; set; }

        [DataMember(Name = "result", IsRequired = false)]
        public string Result { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        public string Error { get; set; }

        [DataMember(Name = "processed", IsRequired = false)]
        public string Processed { get; set; }
    }
}