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
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ASC.Api.Web.Help.BingSearch;
using ASC.Api.Web.Help.Helpers;
using ASC.Api.Web.Help.Models;

namespace ASC.Api.Web.Help.Controllers
{
    public class DocumentationController : AsyncController
    {
        private BreadCrumbsBuilder _breadCrumbsBuilder;
        //
        // GET: /Documentation/
        public DocumentationController()
        {
            _breadCrumbsBuilder = new BreadCrumbsBuilder(this);
        }


        public ActionResult Index(string section, string type, string url)
        {
            if (string.IsNullOrEmpty(section))
            {
                //Render all
                return View("Toc", Documentation.GetAll());
            }
            var docsSection = Documentation.GetDocs(section);
            if (docsSection==null)
            {
                return View("SectionNotFound");
            }
            _breadCrumbsBuilder.Add(docsSection);
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(url))
            {
                //Lookup method
                var function = docsSection.Methods.Where(x => x.Path.Equals(url, StringComparison.OrdinalIgnoreCase) && x.HttpMethod.Equals(type, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (function!=null)
                {
                    _breadCrumbsBuilder.Add(docsSection,function);
                    return View("Function", new SectionMethodViewModel(docsSection,function));
                }
                return View("FunctionNotFound");
            }
            return View("Section",docsSection);
        }

        public ActionResult Toc(string type)
        {
            if (string.Equals("top",type))
            {
                return PartialView("TopTocControl", Documentation.GetAll());
            }
            return PartialView("TocContorl",Documentation.GetAll());
        }

/*        public void SearchAsync(string query)
        {
            
            AsyncManager.OutstandingOperations.Increment();
            AsyncManager.Parameters["query"] = query;
            if (string.IsNullOrEmpty(query))
                AsyncManager.OutstandingOperations.Decrement();

            SearchRequest request;
            var client = PrepareBingRequest(query, out request);

            client.BeginSearch(request, (result) =>
                                            {
                                                var serchResult = client.EndSearch(result);
                                                AsyncManager.Parameters["searchResponse"] = serchResult;
                                                AsyncManager.OutstandingOperations.Decrement();
                                            }, null);
        }

        private BingPortTypeClient PrepareBingRequest(string query, out SearchRequest request)
        {
            var client = new BingPortTypeClient();
            var keys = WebConfigurationManager.AppSettings["bing_search"].Split(',');

            request = new SearchRequest();
            request.AppId = keys[new Random().Next(keys.Length)];
            request.Options = new[] {SearchOption.EnableHighlighting, SearchOption.DisableLocationDetection};
            request.Adult = AdultOption.Off;
            request.Query = query+" site:"+Request.Url.Host;
            request.Sources = new[] {SourceType.Web, SourceType.Spell, SourceType.RelatedSearch};
            request.Web = new WebRequest();
            return client;
        }*/

        public ActionResult Search(string query, SearchResponse searchResponse)
        {
            ViewData["query"] = query ?? string.Empty;

/*            if (searchResponse!=null && searchResponse.Web!=null)
            {
                if (searchResponse.Web.Results!=null && searchResponse.Web.Results.Any())
                {
                    //Format results
                    try
                    {
                        var results = BingSearchHelper.CreateResults(searchResponse.Web.Results);
                        return View(results);
                    }
                    catch (Exception)
                    {
                        //Failed. fallback to our search
                    }
                }
            }*/
            return View(Documentation.Search(query ?? string.Empty));
        }
    }
}
