using DataMonitor.Service.RealtimeData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DataMonitor.Web.UI_RealtimeData
{
    public partial class HeatMeterRealtimeData : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "负一层", "一层", "二层" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            }
        }
        [WebMethod]
        public static string GetHeatMeterRealtimeData() 
        {
            List<string> floorName = GetDataValidIdGroup("ProductionOrganization");
            DataTable table = HeatMeterRealtimeDataService.GetHeatMeterRealtimeDataTable(floorName);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table).Replace("-1.00", "Null");
            return json;
        }
    }
}