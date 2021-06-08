using Newtonsoft.Json;
using RestSharp;
using RxSyncService.Config;
using RxSyncService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.SyncTasks
{
    [Serializable]
    class SyncIssuesData
    {
        public void syncIssueData()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords = new Sync().NewIssueRecordsCount();
            double RecordsBatchFactor = TotalRecords / Properties.ConfigFile.Default.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;

            while (loop <= Loops)
            {
                string[] alert = new string[] { DateTime.Now.ToString() + ": SYNC TASK - START: STOCK ISSUE" };
                string uri = Properties.ConfigFile.Default.API_URL_REQ;
                string bearer_token = "Bearer " + Properties.ConfigFile.Default.AUTHORIZATION;
                try
                {
                    Issues issues = new Issues();
                    DataTable dt = GetData("SELECT Requisition_ID, REPLACE(REPLACE(IssuedBy_str , '(' , ''),')','') as IssuedBy_str, Requisition_dat, Requisition_str, Issued_Dat, "
                                                    + " ReceivedBy_str, LastUpdate_dat FROM TblRequisition "
                                                    + " WHERE TBLRequisition.Requisition_ID IS NOT NULL AND TBLRequisition.Requisition_dat IS NOT NULL AND "
                                                    + " TBLRequisition.Requisition_str IS NOT NULL AND TBLRequisition.Issued_Dat IS NOT NULL AND TBLRequisition.IssuedBy_str IS NOT NULL "
                                                    + " AND sync_status = 0");

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        int record_id = int.Parse(dt.Rows[i]["Requisition_id"].ToString());
                        var PocoObject = new
                        {
                            issue_id = 0,
                            requisition_id = Convert.ToInt32(dt.Rows[i]["Requisition_id"]),
                            issued_by_str = Convert.ToString(dt.Rows[i]["IssuedBy_str"]),
                            issue_date = Convert.ToDateTime(dt.Rows[i]["Issued_Dat"]).AddDays(0).ToString("yyyy-MM-dd hh:mm:ss"),
                            received_date = Convert.ToDateTime(dt.Rows[i]["Issued_Dat"]).AddDays(0).ToString("yyyy-MM-dd hh:mm:ss"),
                            receivedBy_str = Convert.ToString(dt.Rows[i]["ReceivedBy_str"]),
                            lastUpdate_dat = Convert.ToDateTime(dt.Rows[i]["LastUpdate_dat"]).AddDays(0).ToString("yyyy-MM-dd hh:mm:ss"),
                            issueItems = GetItems(Convert.ToInt32(dt.Rows[i]["Requisition_id"]))
                        };
                        ;
                        var json = JsonConvert.SerializeObject(PocoObject, Formatting.Indented);
                        var client = new RestClient(uri);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", bearer_token);
                        request.AddHeader("Content-Type", "application/json; charset=utf-8");
                        request.AddParameter("application/json", json, ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);

                        Console.WriteLine(response.Content);
                        string[] alert1 = new string[] { DateTime.Now.ToString() + ": INFO - Issue data saved: " + PocoObject.requisition_id };
                        File.AppendAllLines(@"C:\CRDDP\CrddpAPI_Log.txt", alert1);

                        //Update Record Status
                        updateIssuesTable(record_id);
                        updateIssuesItemTable(record_id);
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    string[] issue_start = new string[] { DateTime.Now.ToString() + ": SYNC TASK - END: STOCK ISSUE. " + SyncPass + "/" + TotalRecords + " Synced" + " Loops:" + Loops };
                    File.AppendAllLines(@"C:\CRDDP\CrddpAPI_Log.txt", issue_start);
                }

            }

        }


        public List<Dictionary<string, object>> GetItems(int Requisition_id)
        {
            int req_id = Requisition_id;
            List<IssueItems> Items = new List<IssueItems>();
            DataTable dt = GetData(String.Format($"SELECT Requisition_ID as requisition_id, ProductReportCode as product_code , QtyIssued_int as qtyIssued_int, BatchNumber_str as batchNumber_str, " +
                $"convert(varchar, Expiry_dat, 23) as expiry_Dat, convert(varchar, TblRequisitionItems.LastUpdate_dat, 20) as created_at, convert(varchar, TblRequisitionItems.LastUpdate_dat, 20) as modified_at, " +
                $"0 as issue_ref_id FROM TblRequisitionItems INNER JOIN " +
                $"TBLProductPackSize ON TblRequisitionItems.ProductCode_ID = TBLProductPackSize.ProductCode_ID WHERE Requisition_ID IS NOT NULL AND TblRequisitionItems.ProductCode_ID IS NOT NULL " +
                $"AND QtyOrdered_int IS NOT NULL AND BatchNumber_str IS NOT NULL AND Expiry_dat IS NOT NULL AND sync_status = 0 AND Requisition_id = {req_id}"));

            List<Dictionary<string, object>> dictionaries = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> dictionary = Enumerable.Range(0, dt.Columns.Count)
                .ToDictionary(i => dt.Columns[i].ColumnName, i => row.ItemArray[i]);
                dictionaries.Add(dictionary);
            }
            return dictionaries;
        }

        private DataTable GetData(string query)
        {
            //string connString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
            SqlCommand cmd = new SqlCommand(query);
            using (SqlConnection con = new SqlConnection(DatabaseConnection.getLocalConnectionString()))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public void updateIssuesTable(int Requisition_id)
        {
            try
            {
                String sql_str = "UPDATE TBLRequisition SET sync_status = 1 WHERE Requisition_id = @id";
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand command = new SqlCommand(sql_str, conn);

                conn.Open();

                int id = Requisition_id;
                command.Parameters.AddWithValue("@id", Requisition_id);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1);
            }
        }

        public void updateIssuesItemTable(int Requisition_id)
        {
            try
            {
                String sql_str = "UPDATE TblRequisitionItems SET sync_status = 1 WHERE Requisition_id = @id";
                //string connString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand command = new SqlCommand(sql_str, conn);

                conn.Open();

                int id = Requisition_id;
                command.Parameters.AddWithValue("@id", Requisition_id);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1);
            }
            finally
            {

            }
        }

    }
}
