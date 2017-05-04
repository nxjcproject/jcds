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
    public partial class table_FormulaHeatmeter : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "四层", "二层", "十一层" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            }
        }
        [WebMethod]
        public static string GettzHeatmeterJsonbyYear(string dateType, string yearDate)
        {
            DataTable table = tableFormulaService.GettzMeterTable("Heatmeter", dateType, yearDate);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GettzHeatmeterJsonbyStartEndDate(string dateType, string startTime, string endTime)
        {
            DataTable table = tableFormulaService.GettzMeterTable("Heatmeter", dateType, startTime, endTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetformulaDatagridDataJson(string tableName, string mKeyID, string meterType)
        {
            List<string> floorName = GetDataValidIdGroup("ProductionOrganization");
            DataTable table = tableFormulaService.GetformulaDataTableNew(tableName, mKeyID, meterType, floorName);
            string json = DataGridJsonParser.GetRowsAndColumnsJson(table);
            return json;
        }
    }
}