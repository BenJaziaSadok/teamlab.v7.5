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
using System.Web.UI;
using AjaxPro;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core.Search;
using ASC.Web.Studio.UserControls.Common.Search;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio
{
    [AjaxNamespace("SearchController")]
    public partial class Search : MainPage
    {
        protected string SearchText;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Master.DisabledSidePanel = true;

            Title = HeaderStringHelper.GetPageTitle(Resource.Search);

            var productID = !String.IsNullOrEmpty(Request["productID"]) ? new Guid(Request["productID"]) : Guid.Empty;
            var moduleID = !String.IsNullOrEmpty(Request["moduleID"]) ? new Guid(Request["moduleID"]) : Guid.Empty;

            SearchText = Request["search"] ?? "";

            var searchResultsData = new List<SearchResult>();
            if (!string.IsNullOrEmpty(SearchText))
            {
                List<ISearchHandlerEx> handlers = null;

                var products = !String.IsNullOrEmpty(Request["products"]) ? Request["products"] : string.Empty;
                if (!string.IsNullOrEmpty(products))
                {
                    try
                    {
                        var productsStr = products.Split(new[] { ',' });
                        var productsGuid = productsStr.Select(p => new Guid(p)).ToArray();

                        handlers = SearchHandlerManager.GetHandlersExForProductModule(productsGuid);
                    }
                    catch
                    {
                    }
                }

                if (handlers == null)
                {
                    handlers = SearchHandlerManager.GetHandlersExForProductModule(productID, moduleID);
                }

                searchResultsData = GetSearchresultByHandlers(handlers, SearchText);
            }

            if (searchResultsData.Count <= 0)
            {
                var emptyScreenControl = new EmptyScreenControl
                    {
                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_search.png"),
                        Header = Resource.SearchNotFoundMessage,
                        Describe = Resource.SearchNotFoundDescript
                    };
                SearchContent.Controls.Add(emptyScreenControl);
            }
            else
            {
                searchResultsData = GroupSearchresult(productID, searchResultsData);
                var oSearchView = (SearchResults)LoadControl(SearchResults.Location);
                oSearchView.SearchResultsData = searchResultsData;
                SearchContent.Controls.Add(oSearchView);
            }
        }

        private List<SearchResult> GroupSearchresult(Guid productID, List<SearchResult> searchResult)
        {
            if (!productID.Equals(Guid.Empty) && productID != WebItemManager.CommunityProductID) return searchResult;

            var groupedData = GroupDataModules(searchResult);

            foreach (var result in groupedData)
            {
                result.Items.Sort(new DateSearchComparer());
            }
            groupedData.Sort(new SearchComparer());

            return groupedData;
        }

        private List<SearchResult> GroupDataModules(List<SearchResult> data)
        {
            var guids = data.Select(searchResult => searchResult.ProductID).ToList();

            if (!guids.Any())
                return data;

            guids = guids.Distinct().ToList();

            var groupedData = new List<SearchResult>();
            foreach (var productID in guids)
            {
                foreach (var searchResult in data)
                {
                    if (searchResult.ProductID != productID) continue;

                    var item = GetContainer(groupedData, productID);
                    foreach (var searchResultItem in searchResult.Items)
                    {
                        if (searchResultItem.Additional == null)
                            searchResultItem.Additional = new Dictionary<string, object>();

                        if (!searchResultItem.Additional.ContainsKey("imageRef"))
                            searchResultItem.Additional.Add("imageRef", searchResult.LogoURL);
                        if (!searchResultItem.Additional.ContainsKey("Hint"))
                            searchResultItem.Additional.Add("Hint", searchResult.Name);
                    }
                    item.Items.AddRange(searchResult.Items);

                    if (item.PresentationControl == null) item.PresentationControl = searchResult.PresentationControl;
                    if (String.IsNullOrEmpty(item.LogoURL)) item.LogoURL = searchResult.LogoURL;
                    if (String.IsNullOrEmpty(item.Name)) item.Name = searchResult.Name;
                }
            }

            return groupedData;
        }

        private SearchResult GetContainer(ICollection<SearchResult> newData, Guid productID)
        {
            foreach (var searchResult in newData.Where(searchResult => searchResult.ProductID == productID))
                return searchResult;

            var newResult = CreateCertainContainer(productID);
            newData.Add(newResult);
            return newResult;
        }

        private SearchResult CreateCertainContainer(Guid productID)
        {
            var certainProduct = WebItemManager.Instance[productID];
            var container = new SearchResult
                {
                    ProductID = productID,
                    Name = (certainProduct != null) ? certainProduct.Name : String.Empty,
                    LogoURL = (certainProduct != null) ? certainProduct.GetIconAbsoluteURL() : String.Empty
                };

            if (productID == WebItemManager.CommunityProductID || productID == Guid.Empty)
                container.PresentationControl = new CommonResultsView { MaxCount = 7, Text = SearchText };

            return container;
        }

        private static List<SearchResult> GetSearchresultByHandlers(IEnumerable<ISearchHandlerEx> handlers, string searchText)
        {
            var searchResults = new List<SearchResult>();
            foreach (var sh in handlers)
            {
                var module = WebItemManager.Instance[sh.ModuleID];
                if (module != null && module.IsDisabled())
                    continue;

                var items = sh.Search(searchText);

                if (items.Length == 0)
                    continue;

                var searchResult = new SearchResult
                    {
                        ProductID = sh.ProductID,
                        PresentationControl = (ItemSearchControl)sh.Control,
                        Name = module != null ? module.Name : sh.SearchName,
                        LogoURL = module != null
                                      ? module.GetIconAbsoluteURL()
                                      : WebImageSupplier.GetAbsoluteWebPath(sh.Logo.ImageFileName, sh.Logo.PartID)
                    };

                searchResult.PresentationControl.Text = searchText;
                searchResult.PresentationControl.MaxCount = 7;
                searchResult.Items.AddRange(items);

                searchResults.Add(searchResult);
            }
            return searchResults;
        }

        [AjaxMethod]
        public string GetAllData(string product, string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var productID = new Guid(product);

            var handlers = SearchHandlerManager.GetHandlersExForProductModule(productID, Guid.Empty);
            var searchResultsData = GetSearchresultByHandlers(handlers, text);

            searchResultsData = GroupSearchresult(productID, searchResultsData);
            if (searchResultsData.Count <= 0) return string.Empty;

            var control = searchResultsData[0].PresentationControl ?? new CommonResultsView();

            control.Items = new List<SearchResultItem>();
            foreach (var searchResult in searchResultsData)
            {
                control.Items.AddRange(searchResult.Items);
            }
            control.MaxCount = int.MaxValue;
            control.Text = control.Text ?? text;
            var stringWriter = new StringWriter();
            var htmlWriter = new HtmlTextWriter(stringWriter);
            control.RenderControl(htmlWriter);

            return stringWriter.ToString();
        }
    }
}