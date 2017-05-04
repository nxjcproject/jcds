using AWHReports.Service.StatisticalReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AWHReports.Web.UI_StatisticalReport
{
    public partial class table_FormulaAmmeterNew : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "负一层","四层", "二层", "十一层" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            }
                ///以下是接收js脚本中post过来的参数
                string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
                string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
                                 //方法的参数名称2
                if (m_FunctionName == "ExcelStream")
                {
                    //ExportFile("xls", "导出报表1.xls");
                    string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                    m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                    //m_ExportTable = m_ExportTable.Replace("&nbsp", "  ");
                    tableFormulaService.ExportExcelFile("xls", "电能消耗报表.xls", m_ExportTable);
                }

        }
        [WebMethod]
        public static string GettzAmmeterJsonbyYear(string dateType, string yearDate)
        {
            DataTable table = tableFormulaService.GettzMeterTable("Ammeter", dateType, yearDate);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GettzAmmeterJsonbyStartEndDate(string dateType,string startTime,string endTime) 
        {
            DataTable table = tableFormulaService.GettzMeterTable("Ammeter", dateType, startTime, endTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json; 
        }
        [WebMethod]
        public static string GetformulaDatagridDataJson(string tableName, string mKeyID, string meterType) 
        {
            List<string> floorName = GetDataValidIdGroup("ProductionOrganization");
            DataTable table = tableFormulaService.GetformulaDataTableNew(tableName, mKeyID, meterType, floorName);
           // string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            string json = DataGridJsonParser.GetRowsAndColumnsJson(table);
            return json; 
        }
    }
}
