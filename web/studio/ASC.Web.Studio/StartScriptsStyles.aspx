<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartScriptsStyles.aspx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.StartScriptsStyles" %>
<!DOCTYPE html>
<html>
    <head>
        <title>TeamLab</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <meta name="description" content="" />
        <meta name="keywords" content="" />
        <style type="text/css"></style> 
    </head>
    <body>
        <% if (ListUri != null)
       { %>  
        <% foreach (var item in ListUri)
           { %>
            <%if (item.EndsWith(".css")) { %>
            <link type="text/css" href="<%= item %>" rel="stylesheet"/>
            <%}%>

            <% if (item.EndsWith(".js")){ %>
            <script type="text/javascript" src="<%= item %>"></script>
            <%}%> 
         <% } %>        
    <% } %>
    </body>
</html>
   