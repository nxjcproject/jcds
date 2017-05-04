using AWHReports.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWHReports.Service.StatisticalReport
{
    public class tableFormulaService
    {
        public static DataTable GettzMeterTable(string meterType, string dateType, string yearDate)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.Name,A.MeterType,A.ReportType,A.StoreName,B.[Date],B.[CreationDate],B.[KeyID] from [dbo].[system_ReportDescription] A,[dbo].[tz_" + meterType + @"] B
                                where A.Isformula=1 
                                and A.Enabled=1
                                and B.ReportID=A.ID
                                and A.MeterType=@meterType
                                and A.ReportType=@dateType
                                and B.[Date]=@yearDate";
            SqlParameter[] para ={
                                    new SqlParameter("@meterType",meterType),
                                    new SqlParameter("@dateType",dateType),
                                    new SqlParameter("@yearDate",yearDate)
                                  };
            DataTable table = dataFactory.Query(mySql, para);
            return table;
        }
        public static DataTable GettzMeterTable(string meterType, string dateType, string startTime, string endTime)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            //string sTime = Convert.ToDateTime(startTime.Trim()).ToString().Replace('/','-');
            //string eTime = Convert.ToDateTime(endTime.Trim()).ToString().Replace('/', '-');
            //DateTime sTime = DateTime.ParseExact(startTime.Trim(), "yyyy-MM-dd HH:mm:ss", null);
            //DateTime eTime = DateTime.ParseExact(endTime.Trim(), "yyyy-MM-dd HH:mm:ss", null);

            string mySql = @"select A.Name,A.MeterType,A.ReportType,A.StoreName,B.[Date],B.[CreationDate],B.[KeyID] from [dbo].[system_ReportDescription] A,[dbo].[tz_" + meterType + @"] B
                                where A.Isformula=1 
                                and A.Enabled=1
                                and B.ReportID=A.ID
                                and A.MeterType=@meterType
                                and A.ReportType=@dateType
                                and B.[Date]>=@startTime
                                and B.[Date]<=@endTime
                                order by B.[Date] desc";
            //            string mySql = @"select A.Name,A.MeterType,A.ReportType,A.StoreName,B.[Date],B.[CreationDate],B.[KeyID] from [dbo].[system_ReportDescription] A,[dbo].[tz_" + meterType + @"] B
            //                                            where  A.Enabled=1
            //                                            and B.ReportID=A.ID
            //                                            and A.MeterType=@meterType
            //                                            and A.ReportType=@dateType
            //                                            and B.[Date]>=@startTime
            //                                            and B.[Date]<=@endTime
            //                                            order by A.[Isformula],B.[Date] desc";
            SqlParameter[] para ={
                                    new SqlParameter("@meterType",meterType),
                                    new SqlParameter("@dateType",dateType),
                                    new SqlParameter("@startTime",startTime),
                                    new SqlParameter("@endTime",endTime)
                                  };
            DataTable table = dataFactory.Query(mySql, para);
            return table;
        }
        public static DataTable GetformulaDataTable(string tableName, string mKeyID, string meterType)
        {
            DataTable table = new DataTable();
            if (meterType == "Ammeter")
            {
                table = getAmmeterformulaDataTable(tableName, mKeyID);
            }
            else if (meterType == "WaterMeter")
            {

                table = getWaterMeterformulaDataTable(tableName, mKeyID);
            }
            else if (meterType == "HeatMeter")
            {
                table = getHeatMeterformulaDataTable(tableName, mKeyID);
            }
            return table;
        }
        public static DataTable GetformulaDataTableNew(string tableName, string mKeyID, string meterType, List<string> floorName)
        {
            DataTable table = new DataTable();
            if (meterType == "Ammeter")
            {
                table = getAmmeterformulaDataTableNew(tableName, mKeyID,floorName);
            }
            else if (meterType == "WaterMeter")
            {

                table = getWaterMeterformulaDataTableNew(tableName, mKeyID,floorName);
            }
            else if (meterType == "HeatMeter")
            {
                table = getHeatMeterformulaDataTableNew(tableName, mKeyID,floorName);
            }
            return table;
        }
        private static DataTable getAmmeterformulaDataTable(string tableName, string mKeyID)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.FormulaCode,A.ProcessName,A.Peak,A.Valley,A.Flat,B.PowerPeakPrice,B.PowerValleyPrice,B.PowerFlatPrice,A.Amountto,''from  [dbo].[{0}] A,[dbo].[system_AWHUnitPrice] B
                            where A.KeyID='{1}'
                            and B.Enabled=1
                            order by A.FormulaCode asc";
            mySql = string.Format(mySql, tableName, mKeyID);
            DataTable table = dataFactory.Query(mySql);
            table.Columns.Add("PeakMoney");
            table.Columns.Add("ValleyMoney");
            table.Columns.Add("FlatMoney");
            table.Columns.Add("AmounttoMoney");
            foreach (DataRow dr in table.Rows)
            {
                dr["PeakMoney"] = (Convert.ToDecimal(dr["Peak"]) * Convert.ToDecimal(dr["PowerPeakPrice"])).ToString("0.00");
                dr["ValleyMoney"] = (Convert.ToDecimal(dr["Valley"]) * Convert.ToDecimal(dr["PowerValleyPrice"])).ToString("0.00");
                dr["FlatMoney"] = (Convert.ToDecimal(dr["Flat"]) * Convert.ToDecimal(dr["PowerFlatPrice"])).ToString("0.00");
                dr["AmounttoMoney"] = (Convert.ToDecimal(dr["Peak"]) * Convert.ToDecimal(dr["PowerPeakPrice"]) + Convert.ToDecimal(dr["Valley"]) * Convert.ToDecimal(dr["PowerValleyPrice"]) + Convert.ToDecimal(dr["Flat"]) * Convert.ToDecimal(dr["PowerFlatPrice"])).ToString("0.00");
            }
            return table;
        }
        private static DataTable getAmmeterformulaDataTableNew(string tableName, string mKeyID, List<string> floorName)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.ProcessName,A.Peak, ''as Peakvalue,A.Valley, '' as Valleyvalue,A.Flat,'' as Flatvalue,A.Amountto , '' as Amounttovalue from [dbo].[{0}] A inner join  [dbo].[formula_FormulaDetail] as B on A.Formulacode=B.FormulaCode
                            where A.KeyID='{1}'and Name in (
                            ";
            StringBuilder m_mySql = new StringBuilder();
            m_mySql.Append(mySql);
            //m_mySql.Append("'" + floorName[0] + "',");
            for (int i = 0; i < floorName.Count; i++)
            {
                m_mySql.Append("'" + floorName[i] + "',");
            }
            m_mySql.Remove(m_mySql.Length - 1, 1);
            m_mySql.Append(")");
            m_mySql.Append("order by A.FormulaCode asc");
            mySql = m_mySql.ToString();
            mySql = string.Format(mySql, tableName, mKeyID);
            DataTable table = dataFactory.Query(mySql);
            foreach( DataRow dr in table.Rows) 
            {
                if (dr[0].ToString() == "宁夏建材(十九层)")
                {
                    dr[2] = (Convert.ToDouble(dr[1])/1).ToString("0.00");
                    dr[4] = (Convert.ToDouble(dr[3]) / 1).ToString("0.00");
                    dr[6] = (Convert.ToDouble(dr[5]) / 1).ToString("0.00");
                    dr[8] = (Convert.ToDouble(dr[7]) / 1).ToString("0.00");
                }
                else if (dr[0].ToString() == "负一层")
                {
                    dr[2] = null;
                    dr[4] = null;
                    dr[6] = null;
                    dr[8] = null;
                }
               else 
                {
                    dr[2] = (Convert.ToDouble(dr[1]) / 6).ToString("0.00");
                    dr[4] = (Convert.ToDouble(dr[3]) / 6).ToString("0.00");
                    dr[6] = (Convert.ToDouble(dr[5]) / 6).ToString("0.00");
                    dr[8] = (Convert.ToDouble(dr[7]) / 6).ToString("0.00");
            }
            }
            return table;
       }
        private static DataTable getWaterMeterformulaDataTable(string tableName, string mKeyID)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.[ID]
                          ,A.[KeyID]
                          ,A.[FormulaCode]
                          ,A.[ProcessName]
                          ,A.[Amountto] as WAmountto,B.WaterPrice,'' as TotalCost from [dbo].[{0}] A,[dbo].[system_AWHUnitPrice] B
                            where A.KeyID='{1}'
                            and B.Enabled=1
                            order by A.FormulaCode asc";
            mySql = string.Format(mySql, tableName, mKeyID);
            DataTable table = dataFactory.Query(mySql);
            foreach (DataRow dr in table.Rows)
            {
                dr["TotalCost"] = (Convert.ToDecimal(dr["WAmountto"]) * Convert.ToDecimal(dr["WaterPrice"])).ToString("0.00");
            }
            return table;
        }
        private static DataTable getWaterMeterformulaDataTableNew(string tableName, string mKeyID, List<string> floorName)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.[ID]
                          ,A.[KeyID]
                          ,A.[FormulaCode]
                          ,A.[ProcessName]
                          ,A.[Amountto] as WAmountto,B.WaterPrice,'' as TotalCost from [dbo].[{0}]  as A inner join [dbo].[formula_FormulaDetail] as C on A.Formulacode=C.FormulaCode
                          ,[dbo].[system_AWHUnitPrice] B
                            where A.KeyID='{1}'
                            and B.Enabled=1
                            and C.Name in (
                            ";
            StringBuilder m_mysql = new StringBuilder();
            m_mysql.Append(mySql);
            for (int i = 0; i < floorName.Count; i++)
            {
                m_mysql.Append("'" + floorName[i] + "',");
            }
            m_mysql.Remove(m_mysql.Length - 1, 1);
            m_mysql.Append(")");
            m_mysql.Append("order by A.FormulaCode asc");
            mySql = m_mysql.ToString();
            mySql = string.Format(mySql, tableName, mKeyID);
            DataTable table = dataFactory.Query(mySql);
            foreach (DataRow dr in table.Rows)
            {
                dr["TotalCost"] = (Convert.ToDecimal(dr["WAmountto"]) * Convert.ToDecimal(dr["WaterPrice"])).ToString("0.00");
            }
            return table;
        }
        private static DataTable getHeatMeterformulaDataTable(string tableName, string mKeyID)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.[ID]
                          ,A.[KeyID]
                          ,A.[FormulaCode]
                          ,A.ProcessName         
                          ,A.[Amountto] as HAmountto,B.HeatPrice,'' as TotalCost from [dbo].[{0}] A,[dbo].[system_AWHUnitPrice] B
                            where A.KeyID='{1}'
                            and B.Enabled=1
                            order by A.FormulaCode asc";
            mySql = string.Format(mySql, tableName, mKeyID);
            DataTable table = dataFactory.Query(mySql);
            foreach (DataRow dr in table.Rows)
            {
                dr["TotalCost"] = (Convert.ToDecimal(dr["HAmountto"]) * Convert.ToDecimal(dr["HeatPrice"])).ToString("0.00");
            }
            return table;
        }
        private static DataTable getHeatMeterformulaDataTableNew(string tableName, string mKeyID, List<string> floorName)
        {
            string connectionString = ConnectionStringFactory.JCDSConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @" select A.[ID]
                          ,A.[KeyID]
                          ,A.[FormulaCode]
                          ,A.[ProcessName]
                          ,A.[Amountto] as HAmountto,B.HeatPrice,'' as TotalCost from [dbo].[{0}]  as A inner join [dbo].[formula_FormulaDetail] as C on A.Formulacode=C.FormulaCode
                          ,[dbo].[system_AWHUnitPrice] B
                            where A.KeyID='{1}'
                            and B.Enabled=1
                            and C.Name in (
                            ";
            StringBuilder m_mysql = new StringBuilder();
            m_mysql.Append(mySql);
            for (int i = 0; i < floorName.Count; i++)
            {
                m_mysql.Append("'" + floorName[i] + "',");
            }
            m_mysql.Remove(m_mysql.Length - 1, 1);
            m_mysql.Append(")");
            m_mysql.Append("order by A.FormulaCode asc");
            mySql = m_mysql.ToString();
            mySql = string.Format(mySql, tableName, mKeyID);
            DataTable table = dataFactory.Query(mySql);
            foreach (DataRow dr in table.Rows)
            {
                dr["TotalCost"] = (Convert.ToDecimal(dr["HAmountto"]) * Convert.ToDecimal(dr["HeatPrice"])).ToString("0.00");
            }
            return table;
        }
        public static void ExportExcelFile(string myFileType, string myFileName, string myData)
        {
            if (myFileType == "xls")
            {
                UpDownLoadFiles.DownloadFile.ExportExcelFile(myFileName, myData);
            }
        }
    }
}
