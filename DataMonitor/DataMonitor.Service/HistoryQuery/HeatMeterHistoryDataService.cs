﻿using DataMonitor.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitor.Service.HistoryQuery
{
    public class HeatMeterHistoryDataService
    {
        public static DataTable GetHeatMeterHistoryDataTable(string startTime, string endTime) 
        {
            string connectionString = ConnectionStringFactory.JCJTConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            DataTable result = new DataTable();
            string mySql = "";
            string Hsql = @"select Field_name from [dbo].[GaugeContrast] where Gauge_number like 'H%'
                        select top 1 vDate from [History_H_Heat] where vDate>'{0}' order by vDate 
                        select top 1 vDate from [History_H_Heat] where vDate<'{1}' order by vDate desc
                        ";
            Hsql = string.Format(Hsql, startTime, endTime);
            DataSet dataSet = GetDataSetAdapter.GetdataSet(connectionString, Hsql);

            DataTable table_H = dataSet.Tables[0];
            string mstartTime = "";
            string mendTime = "";
            if (dataSet.Tables[1].Rows.Count>0 && dataSet.Tables[2].Rows.Count > 0) {
                mstartTime = dataSet.Tables[1].Rows[0]["vDate"].ToString().Trim();
                mendTime = dataSet.Tables[2].Rows[0]["vDate"].ToString().Trim();
                if (Convert.ToDateTime(mstartTime) < Convert.ToDateTime(mendTime))
                {
                    string colStr = "";
                    string Hnull = "";
                    foreach (DataRow dr in table_H.Rows)
                    {
                        string _name = dr["Field_name"].ToString().Trim();
                        colStr = colStr + _name + ",";
                        Hnull = Hnull + "isnull(" + _name + ",0)" + _name + ",";
                    }
                    colStr = colStr.Remove(colStr.Length - 1, 1);
                    Hnull = Hnull.Remove(Hnull.Length - 1, 1);

                    mySql = @"select B.Floor_name as FloorName,B.Gauge_number as GaugeNumber,B.Gauge_description as MeterName,B.Gauge_address as mAddress,B.Floor,A.s_Value as StartValue,C.s_Value as EndValue,(C.s_Value-A.s_Value) as Consume
	                   from [dbo].[GaugeContrast] B,
			                (select [s_Name],[s_Value] from " + "(select " + Hnull + " from [dbo].[History_H_Heat] where vDate=@Startime)WSH unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) A,
			                (select [s_Name],[s_Value] from" + "(select " + Hnull + " from [dbo].[History_H_Heat] where vDate=@Endtime)WEH unpivot ([s_Value] for [s_Name] in(" + colStr + @"))c) C";
                    StringBuilder sqlBuilder = new StringBuilder();
                    sqlBuilder.Append(mySql);
                    mySql = @" where B.Gauge_number like 'H%' 
                            and B.Field_name=A.s_Name 
                            and B.Field_name=C.s_Name 
	                            order by B.Floor";
                    sqlBuilder.Append(mySql);
                    mySql = sqlBuilder.ToString();
                    SqlParameter[] para = new SqlParameter[] {new SqlParameter("@Startime",mstartTime),
                                                      new SqlParameter("@Endtime",mendTime)};
                    result = dataFactory.Query(mySql, para);
                    result = GetContrast(result);
                }
            }          
            return result;

        }
        public static DataTable GetContrast(DataTable table)
        {
            int rowsCount = table.Rows.Count;
            //整楼汇总
            // double mreal = 0;
            //double mdaySum = 0;
            //double mmonthSum = 0;
            double myearSum = 0;
            //每层汇总
            string floorName = table.Rows[0]["FloorName"].ToString();
            double Consume = Convert.ToDouble(table.Rows[0]["Consume"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["Consume"]);
            //double ConsumeNew = Convert.ToDouble(table.Rows[0]["ConsumeNew"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["ConsumeNew"]);
            for (int i = 1; i < rowsCount; i++)
            {
                if (table.Rows[i]["Floor"].ToString().Equals(table.Rows[i - 1]["Floor"].ToString()))
                {
                    Consume = Consume + (Convert.ToDouble(table.Rows[i]["Consume"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Consume"]));
                    //ConsumeNew = ConsumeNew + (Convert.ToDouble(table.Rows[i]["ConsumeNew"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["ConsumeNew"]));
                    if (rowsCount - 1 == i)
                    {
                        table.Rows.Add(table.Rows[i]["FloorName"], null, "总计", null, table.Rows[i]["Floor"], null, null, Consume);
                        myearSum = myearSum + Consume;
                        //mmonthSum = mmonthSum + ConsumeNew;
                        table.Rows.Add("所有楼层", null, "总计", null, table.Rows[i]["Floor"], null, null, myearSum);
                    }
                }
                else
                {
                    table.Rows.Add(null, null, "总计", null, table.Rows[i - 1]["Floor"], null, null, Consume);
                    myearSum = myearSum + Consume;
                    //mmonthSum = mmonthSum + ConsumeNew;
                    Consume = Convert.ToDouble(table.Rows[i]["Consume"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Consume"]);
                    //ConsumeNew = Convert.ToDouble(table.Rows[i]["ConsumeNew"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["ConsumeNew"]);
                }
            }
            table.DefaultView.Sort = "Floor asc";
            table = table.DefaultView.ToTable();
            return table;
        }
    }
}
