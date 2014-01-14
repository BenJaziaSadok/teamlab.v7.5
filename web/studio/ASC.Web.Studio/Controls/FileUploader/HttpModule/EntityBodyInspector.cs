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
using System.Web;

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    internal class EntityBodyInspector
    {
        private EntityBodyChunkStateWaiter _current;
        private string _lastCdName;
        private readonly EntityBodyChunkStateWaiter _boundaryWaiter;
        private readonly EntityBodyChunkStateWaiter _boundaryInfoWaiter;
        private readonly EntityBodyChunkStateWaiter _formValueWaiter;
        private readonly UploadProgressStatistic _statistic;

        internal EntityBodyInspector(HttpUploadWorkerRequest request)
        {
            _statistic = new UploadProgressStatistic
                {
                    TotalBytes = request.GetTotalEntityBodyLength()
                };

            var contentType = request.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);

            var boundary = string.Format("--{0}\r\n", UploadProgressUtils.GetBoundary(contentType));

            _boundaryWaiter = new EntityBodyChunkStateWaiter(boundary, false);
            _boundaryWaiter.MeetGuard += BoundaryWaiterMeetGuard;
            _current = _boundaryWaiter;

            _boundaryInfoWaiter = new EntityBodyChunkStateWaiter("\r\n\r\n", true);
            _boundaryInfoWaiter.MeetGuard += BoundaryInfoWaiterMeetGuard;

            _formValueWaiter = new EntityBodyChunkStateWaiter("\r\n", true);
            _formValueWaiter.MeetGuard += FormValueWaiterMeetGuard;

            _lastCdName = string.Empty;
        }

        internal void EndRequest()
        {
            _statistic.EndUpload();
        }

        internal void Inspect(byte[] buffer, int offset, int size)
        {
            if (buffer == null)
                return;

            _statistic.AddUploadedBytes(buffer.Length);
            Inspect(buffer, offset);
        }

        private void Inspect(byte[] buffer, int offset)
        {
            if (buffer == null)
                return;

            _current.Wait(buffer, offset);
        }

        private void BoundaryWaiterMeetGuard(object sender, EventArgs e)
        {
            var sw = sender as EntityBodyChunkStateWaiter;
            sw.Reset();
            _current = _boundaryInfoWaiter;
            _current.Wait(sw);
        }

        private void BoundaryInfoWaiterMeetGuard(object sender, EventArgs e)
        {
            var sw = sender as EntityBodyChunkStateWaiter;
            var cdi = UploadProgressUtils.GetContentDisposition(sw.Value);
            sw.Reset();
            if (!cdi.IsFile)
            {
                _lastCdName = cdi.name;
                _current = _formValueWaiter;
                _current.Wait(sw);
            }
            else
            {
                _statistic.BeginFileUpload(cdi.filename);
                _current = _boundaryWaiter;
                _current.Wait(sw);
            }
        }

        private void FormValueWaiterMeetGuard(object sender, EventArgs e)
        {
            var sw = sender as EntityBodyChunkStateWaiter;

            var fieldValue = sw.Value;
            _statistic.AddFormField(_lastCdName, fieldValue);

            sw.Reset();

            _current = _boundaryWaiter;
            _current.Wait();
        }
    }
}