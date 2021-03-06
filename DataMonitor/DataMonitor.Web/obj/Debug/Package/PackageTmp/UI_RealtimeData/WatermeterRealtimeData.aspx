﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WatermeterRealtimeData.aspx.cs" Inherits="DataMonitor.Web.UI_RealtimeData.RealtimeDataMonitorWatermeter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>水表实时监控</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="js/common/MergeCell.js" charset="utf-8"></script>
     <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="js/page/WatermeterRealtimeData.js" charset="utf-8"></script>
</head>
<body>
     <div class="easyui-layout" data-options="fit:true,border:false">
        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
            <table id="realTime_WaterMeter" class="easyui-datagrid"></table>
        </div>   
    </div>
</body>
</html>
