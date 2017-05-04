﻿$(function () {
    InitDate();
    LoadAmmeterData("first");
});
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getMonth() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-01 00:00:00';
    $('#startDate').datetimebox('setValue', beforeString);
    $('#endDate').datetimebox('setValue', nowString);
    //$('#startDate').datetimebox('setValue', '2016-01-25 16:07:48.000');
    //$('#endDate').datetimebox('setValue', '2016-01-25 17:17:54.000');
}
function LoadAmmeterData(type,myData)
{
    if (type == "first")
    {
        $('#grid_AmmeterDatagrid').datagrid({
            columns: [[
                {
                    field: 'FloorName', title: '楼层', width: 100, align: 'center'
                      , styler: function (value, row, index) {
                          if (row["FloorName"] != "")
                          {
                              return 'border-top:1px solid #1F1F1F;background-color:#B0E0E6;border-bottom:1px solid #1F1F1F;';
                          }
                          else {
                              return 'background-color:#B0E0E6;';
                          }
                      }
                },
                {
                    field: 'GaugeNumber', title: '电表号', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
                {
                    field: 'AmmeterName', title: '电表名', width: 100, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
              //  {field: 'mIP', title: 'IP地址', width: 120, align: 'center'},
                {
                    field: 'mAddress', title: '表地址', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
               {
                   field: 'StartValue', title: '期初值', width: 80, align: 'center', styler: function (value, row, index) {
                       if (row["GaugeNumber"] == "总计")
                       { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                   }
               },
                {
                    field: 'StartValueNew', title: '期初表码值', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
                {
                    field: 'EndValue', title: '期末值', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
                {
                    field: 'EndValueNew', title: '期末表码值', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
                {
                    field: 'Consume', title: '耗电量', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
                {
                    field: 'ConsumeNew', title: '耗电表码值', width: 80, align: 'center', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计")
                        { return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;border-bottom:1px solid #1F1F1F;'; }
                    }
                },
               //{ field: 'Asum', title: '小计', width: 100, rowspan: 2, align: 'right' }
                
             ]],
            striped: true,
            singleSelect: true,
            fit: true
        });
    }
    else if (type == "last") {
        $('#grid_AmmeterDatagrid').datagrid('loadData',myData);
    }
}
function queryHistoryData() {
    var mger = "";
    var m_starTime = $('#startDate').datetimebox('getValue');
    var m_endTime = $('#endDate').datetimebox('getValue');
    $.ajax({
        type: "POST",
        url: "AmmeterHistoryData.aspx/GetAmmeterHistoryData",
        data: "{startTime: '" + m_starTime + "',endTime:'" + m_endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData == []||myData.total==0) {
                $.messager.alert('提示', "没有查询的数据");
                return;
            }
            else {                  
                LoadAmmeterData("last", myData);
                myMergeCell('grid_AmmeterDatagrid', "FloorName");
            }          
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            mger = $.messager.alert('提示', "加载中...");
        },
        error: handleError
    });
}
function handleError() {
    $('#grid_AmmeterDatagrid').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}