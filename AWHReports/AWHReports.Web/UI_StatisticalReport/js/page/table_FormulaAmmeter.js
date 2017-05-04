var mValue = "month";
var myear = "";
var smonth = "";
var emonth = "";
var sday = "";
var eda = "";
$(function () {
    InitialType();
    InitialDate("month")
    loadtzReportDatagrid("first");
    loadtformulaDatagrid("first", []);
    //  initPageAuthority();
});
初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate();
    $('#startDate').datetimebox('setValue', beforeString);
    $('#endDate').datetimebox('setValue', nowString);
}
function InitialType() {
    $("#reportType").combobox({
        valueField: 'value',
        textField: 'type',
        data: [{
            type: '日报',
            value: 'day'
        }, {
            type: '月报',
            value: 'month',
            selected: true
        }, {
            type: '年报',
            value: 'year'
        }],
        panelHeight: 'auto',
        onSelect: function (node) {
            mValue = node.value;
            InitialDate(mValue)
        }
    });
}
function loadtzReportDatagrid(type, myData) {
    if ("first" == type) {
        $("#grid_tzReportDatagrid").datagrid({
            striped: true,
            rownumbers: true,
            singleSelect: true,
            fit: true,
            // toolbar: '#toolbar_tzReportDatagrid',
            columns: [
                //[{ "title": "报表列表", "colspan": 2 }],
                       // { field: 'Name', title: '报表名称', width: 120 },
		               [{ field: 'Date', title: '报表日期', align: 'left', width: 100, hight: 60 }, ]
                        //{
                        //    field: 'MeterType', title: '表类型', width: 75, formatter: function (value, row, index) {
                        //        if (value == "Ammeter") { return value = "电表"; } else if (value == "WaterMeter") { return value = "水表"; } else if (value == "HeatMeter") { return value = "热表"; }
                        //    }
                        //},
                        //{ field: 'ReportType', title: '报表类型', width: 80 ,formatter: function (value, row, index) {
                        //    if (value == "year") { return value = "年报"; } else if (value == "month") { return value = "月报"; } else if (value == "day") { return value = "日报"; }
                        //}},
                        //{ field: 'CreationDate', title: '创建日期', width: 80 }
            ],
            onDblClickRow: function (index, row) {
                LoadformulaDatagridData(row);
                $('#reportName').textbox('setText', row.Name);
                $('#reportTime').textbox('setText', row.Date);
            }
        });
    }
    else {
        $("#grid_tzReportDatagrid").datagrid('loadData', myData);
    }
}
function loadtformulaDatagrid(type,frozenColumnsData, columnsData, rowData) {
    if ("first" == type) {
        $("#grid_formulaDatagrid").datagrid({
            striped: true,
            rownumbers: true,
            singleSelect: true,
            fit: true,
            fitColumns:false,
            toolbar: '#toolbar_formulaDatagrid',
            frozenColumns:[frozenColumnsData],
            columns: [columnsData]
        });
    }
    else {
        $("#grid_formulaDatagrid").datagrid({
            frozenColumns: [frozenColumnsData],
            columns: [columnsData]
        });
        $("#grid_formulaDatagrid").datagrid('loadData', rowData);
    }
}
function StringToDate(Str) {
    var dateCell = Str.split("-");
    var leng = dateCell.length;
    var myDate = dateCell[0];
    for (var i = 1; i < leng; i++) {
        if (Number(dateCell[i]) < 10) {
            dateCell[i] = "0" + dateCell[i];
        }
        myDate = myDate + "-" + dateCell[i];
    }
    return myDate;
}
function researchFun() {
    var urlString = "";
    var paraData = "";
    if ("year" == mValue) {
        myear = $("#date_year").val();
        urlString = "table_FormulaAmmeter.aspx/GettzAmmeterJsonbyYear";
        paraData = "{dateType:'" + mValue + "',yearDate:'" + myear + "'}";
    }
    else if ("month" == mValue) {
        smonth = StringToDate($("#date_smonth").val());
        emonth = StringToDate($("#date_emonth").val());
        urlString = "table_FormulaAmmeter.aspx/GettzAmmeterJsonbyStartEndDate";
        paraData = "{dateType:'" + mValue + "',startTime:'" + smonth + "',endTime:'" + emonth + "'}";
        if (smonth >= emonth) {
            $.messager.alert('提示', '请输入正确的日期！');
        }
    }
    else if ("day" == mValue) {
        sday = StringToDate($("#date_sday").val());
        eday = StringToDate($("#date_eday").val());
        urlString = "table_FormulaAmmeter.aspx/GettzAmmeterJsonbyStartEndDate";
        paraData = "{dateType:'" + mValue + "',startTime:'" + sday + "',endTime:'" + eday + "'}";

        if (sday >= eday) {
            $.messager.alert('提示', '请选择正确的日期！');
        }
    }
    $.ajax({
        type: "POST",
        url: urlString,
        data: paraData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                loadtzReportDatagrid("last", []);
                $.messager.alert('提示', "没有查询的数据");
                return;
            } else {
                loadtzReportDatagrid("last", myData);
                LoadformulaDatagridData(myData.rows[0])
            }
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            mger = $.messager.alert('提示', "加载中...");
        },
        error: function () {
            $("#grid_tzReportDatagrid").datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function LoadformulaDatagridData(rowInfo) {
    $('#reportTime').textbox('setText', rowInfo.Date);
    var tableName = rowInfo.StoreName;
    var mKeyID = rowInfo.KeyID;
    var meterType = rowInfo.MeterType;
    $.ajax({
        type: "POST",
        url: "table_FormulaAmmeter.aspx/GetformulaDatagridDataJson",
        data: "{tableName:'" + tableName + "',mKeyID:'" + mKeyID + "',meterType:'" + meterType + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                loadtformulaDatagrid("last", myData.columns);
                $.messager.alert('提示', "没有查询的数据");
                return;
            } else {
                loadtformulaDatagrid("last",myData.frozenColumnsData, myData.columns, myData.rows);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            mger = $.messager.alert('提示', "加载中...");
        },
        error: function () {
            $("#grid_formulaDatagrid").datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function InitialDate(type) {
    var nowDate = new Date();
    var beforeDate = new Date();
    if ("year" == type) {
        $(".myear").show();
        $(".mmonth").hide();
        $(".mday").hide();
        var lastYear = nowDate.getFullYear() - 1;
        myear = $("#date_year").val(lastYear);
    }
    else if ("month" == type) {
        $(".myear").hide();
        $(".mmonth").show();
        $(".mday").hide();
        beforeDate.setDate(nowDate.getDate() - 90);
        var startDate = beforeDate.getFullYear() + '-' + '1';
        var endDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1);
        smonth = $("#date_smonth").val(startDate);
        emonth = $("#date_emonth").val(endDate);
    }
    else if ("day" == type) {
        $(".myear").hide();
        $(".mmonth").hide();
        $(".mday").show();
        beforeDate.setDate(nowDate.getDate() - 10);
        var startDate = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + '1';
        var endDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
        sday = $("#date_sday").val(startDate);
        eday = $("#date_eday").val(endDate);
    }
}
function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = "Parameter1";
    var m_Parameter2 = "Parameter2";

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "table_FormulaAmmeter.aspx");
    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);
    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
    /*
    var m_Parmaters = { "myFunctionName": m_FunctionName, "myParameter1": m_Parameter1, "myParameter2": m_Parameter2 };
    $.ajax({
        type: "POST",
        url: "Report_Example.aspx",
        data: m_Parmater,                       //'myFunctionName=' + m_FunctionName + '&myParameter1=' + m_Parameter1 + '&myParameter2=' + m_Parameter2,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "1") {
                alert("导出成功!");
            }
            else{
                alert(msg.d);
            }
        }
    });
    */
}
function PrintFileFun() {
    $.ajax({
        type: "POST",
        url: "table_FormulaAmmeter.aspx/PrintFile",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            PrintHtml(msg.d);
        }
    });
}