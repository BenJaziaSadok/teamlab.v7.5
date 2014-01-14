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

using ASC.Api.Exceptions;
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Routing;

namespace ASC.Api.Impl
{
    public class ApiHttpHandler : ApiHttpHandlerBase
    {
        #region IApiHttpHandler Members


        public ApiHttpHandler(RouteData routeData)
            : base(routeData)
        {

        }

        protected override void DoProcess(HttpContextBase context)
        {
            Log.Debug("strating request. context: '{0}'", ApiContext);

            //Neeeded to rollback errors
            context.Response.Buffer = true;
            context.Response.BufferOutput = true;

            try
            {
                Log.Debug("method invoke");
                ApiResponce.Count = ApiContext.Count;
                ApiResponce.StartIndex = ApiContext.StartIndex;

                if (Method != null)
                {
                    var responce = ApiManager.InvokeMethod(Method, ApiContext);
                    if (responce is Exception)
                    {
                        SetError(context, (Exception)responce, HttpStatusCode.InternalServerError);
                    }
                    else
                    {
                        // success
                        PostProcessResponse(context, responce);
                    }
                }
                else
                {
                    SetError(context, new MissingMethodException("Method not found"), HttpStatusCode.NotFound);
                }
            }
            catch (TargetInvocationException targetInvocationException)
            {
                if (targetInvocationException.InnerException is ItemNotFoundException)
                {
                    SetError(context, targetInvocationException.InnerException, HttpStatusCode.NotFound, "The record could not be found");
                }
                else if (targetInvocationException.InnerException is ArgumentException)
                {
                    SetError(context, targetInvocationException.InnerException, HttpStatusCode.BadRequest, "Invalid arguments");
                }
                else
                {
                    SetError(context, targetInvocationException.InnerException, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                SetError(context, e, HttpStatusCode.InternalServerError);
            }

            Exception responseError;
            try
            {
                RespondTo(Method, context);
                return;
            }
            catch (ThreadAbortException e)
            {
                //Do nothing. someone killing response
                Log.Error(e, "thread aborted. response not sent");
                return;
            }
            catch (HttpException exception)
            {
                responseError = exception;
                SetError(context, exception, (HttpStatusCode)exception.GetHttpCode());//Set the code of throwed exception
            }
            catch (Exception exception)
            {
                responseError = exception;
                SetError(context, exception, HttpStatusCode.InternalServerError);
            }
            Log.Error(responseError, "error happened while sending response. can't be here");
            RespondTo(Method, context);//If we got there then something went wrong
        }

        #endregion
    }
}