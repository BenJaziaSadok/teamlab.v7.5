<%@Assembly Name="ASC.Web.Core" %>
<%@Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>

<link rel="stylesheet" href="<%= VirtualPathUtility.ToAbsolute("~/skins/default/toastr.css") %>" />

<link rel="stylesheet" href="<%= PathProvider.GetFileStaticRelativePath("common.css", true) %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/maincontent/maincontent.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/emptyfolder/emptyfolder.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/tree/tree.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/accessrights/accessrights.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/fileviewer/fileviewer.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/importcontrol/importcontrol.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/thirdparty/thirdparty.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/convertfile/convertfile.css" %>" />
<link rel="stylesheet" href="<%= CommonLinkUtility.FilesBaseAbsolutePath + "controls/chunkuploaddialog/chunkuploaddialog.css" %>" />
