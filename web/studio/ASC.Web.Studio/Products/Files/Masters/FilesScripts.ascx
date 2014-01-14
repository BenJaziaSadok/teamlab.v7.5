<%@ Assembly Name="ASC.Web.Files" %>
<%@Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>

<%--Third party--%>

<script type="text/javascript" language="javascript" src="<%= ResolveUrl("~/js/third-party/jquery/jquery.mousewheel.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= ResolveUrl("~/js/third-party/jquery/jquery.uri.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= ResolveUrl("~/js/third-party/jquery/toastr.js") %>"></script>

<script type="text/javascript" language="javascript" src="<%= ResolveUrl("~/js/third-party/sorttable.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= ResolveUrl("~/js/third-party/zeroclipboard.js") %>"></script>

<%--Common--%>

<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("auth.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("common.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("filter.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("xsltmanager.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("templatemanager.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("ui.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("mousemanager.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("markernew.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("actionmanager.js",true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("eventhandler.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("anchormanager.js", true) %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("foldermanager.js", true) %>"></script>

<%--Controls--%>

<script type="text/javascript" language="javascript" src="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/createmenu/createmenu.js" %>"></script>
<script type="text/javascript" language="javascript" src="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/emptyfolder/emptyfolder.js" %>"></script>
<script type="text/javascript" language="javascript" src="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/fileviewer/fileviewer.js" %>"></script>
<script type="text/javascript" language="javascript" src="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/tree/tree.js" %>"></script>
<script type="text/javascript" language="javascript" src="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/convertfile/convertfile.js" %>"></script>
<script type="text/javascript" language="javascript" src="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/chunkuploaddialog/chunkuploadmanager.js" %>"></script>
