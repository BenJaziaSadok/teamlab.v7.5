<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<%--Cases List--%>

<script id="caseListTmpl" type="text/x-jquery-tmpl">
    <tbody>
        {{tmpl(cases) "caseTmpl"}}
    </tbody>
</script>

<script id="caseTmpl" type="text/x-jquery-tmpl">
    <tr id="caseItem_${id}" class="with-entity-menu">
        <td class="borderBase" style="padding: 0 0 0 6px;">
            <input type="checkbox" id="checkCase_${id}" onclick="ASC.CRM.ListCasesView.selectItem(this);"
                 {{if isChecked == true}}checked="checked"{{/if}} />
            <img id="loaderImg_${id}" style="display:none;" alt=""
                src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID)%>" />
        </td>

        <td class="borderBase">
            {{if isPrivate == true}}
                <label class="crm-private-lock"></label>
            {{/if}}
            <a class="linkHeaderMedium{{if isClosed == true}} gray-text{{/if}}" href="?id=${id}">
                ${title}
            </a>
        </td>
        <td class="borderBase" style="padding:5px;">
            <div id="caseMenu_${id}" class="entity-menu" title="<%= CRMCommonResource.Actions %>"></div>
        </td>
    </tr>
</script>