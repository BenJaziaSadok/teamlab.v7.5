<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductsAndInstruments.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ProductsAndInstruments" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>


<div id="studio_productSettings">
    <div class="clearFix">
        <div class="float-left" style="margin-right: 5px;">
            <div class="errorBox display-none"></div>
            <div class="okBox display-none">
                <%=Resources.Resource.SuccessfullySaveSettingsMessage%>
            </div>

            <div class="web-item-list">
                <div class="header-base">
                    <%=Resources.Resource.ProductsAndInstruments_Products%>
                </div>
                <% foreach (var product in Products) %>
                <% { %>
                <div class="web-item">
                    <div class="web-item-header header-base-small">
                        <% if (product.Disabled) %>
                        <% { %>
                        <input id="cbx_<%= product.ItemName %>" type="checkbox" data-id="<%= product.ID %>"/>
                        <% } else { %>
                        <input id="cbx_<%= product.ItemName %>" type="checkbox" data-id="<%= product.ID %>" checked="checked"/>
                        <% } %>
                        <img src="<%= product.IconUrl %>" align="absmiddle"/>
                        <label for="cbx_<%= product.ItemName %>">
                            <%= product.Name%>
                        </label>
                    </div>
                    <% if (product.SubItems.Count > 0) %>
                    <% { %>
                    <div class="web-item-subitem-list" style="<%= product.Disabled ? "display:none" : "" %>">
                        <% foreach (var subItem in product.SubItems) %>
                        <% { %>
                        <div class="web-item-subitem">
                            <% if (subItem.Disabled) %>
                            <% { %>
                            <input id="cbx_<%= subItem.ItemName %>" type="checkbox" data-id="<%= subItem.ID %>"/>
                            <% } else { %>
                            <input id="cbx_<%= subItem.ItemName %>" type="checkbox" data-id="<%= subItem.ID %>" checked="checked"/>
                            <% } %>
                            <label for="cbx_<%= subItem.ItemName %>">
                                <%= subItem.Name%>
                            </label>
                        </div>
                        <% } %>
                    </div>
                    <% } %>
                </div>
                <% } %>
            </div>

            <div class="web-item-list">
                <div class="header-base">
                    <%=Resources.Resource.ProductsAndInstruments_Instruments%>
                </div>
                <% foreach (var module in Modules) %>
                <% { %>
                <div class="web-item">
                    <div class="web-item-header header-base-small">
                        <% if (module.Disabled) %>
                        <% { %>
                        <input id="cbx_<%= module.ItemName %>" type="checkbox" data-id="<%= module.ID %>"/>
                        <% } else { %>
                        <input id="cbx_<%= module.ItemName %>" type="checkbox" data-id="<%= module.ID %>" checked="checked"/>
                        <% } %>
                        <img src="<%= module.IconUrl %>" align="absmiddle"/>
                        <label for="cbx_<%= module.ItemName %>">
                            <%= module.Name%>
                        </label>
                    </div>
                </div>
                <% } %>
            </div>
        </div>
        <div class="settings-help-block">
            <p>
                <%=Resources.Resource.ProductsAndInstruments_Info%>
            </p>
        </div>
    </div>

    <div class="action-block top-splitter">
        <a id="btnSaveSettings" class="button blue">
            <%=Resources.Resource.SaveButton%>
        </a>
    </div>
    <div class="info-block top-splitter display-none">
        <span class="text-medium-describe">
            <%=Resources.Resource.LoadingProcessing%>
        </span>
        <br />
        <img src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
    </div>
</div>