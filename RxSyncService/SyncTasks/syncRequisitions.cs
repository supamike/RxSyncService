using Newtonsoft.Json;
using RxSyncService.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static RxSyncService.Models.RequisitionModel;

namespace RxSyncService.SyncTasks
{
    class syncRequisitions
    {
        //string connString = @"server=UG1-MODAL\SQLEXPRESS;database=KABALE_RRH;user id=admin;password=MOUlin123;";
        string facility_code = Properties.ConfigFile.Default.FACILITY_CODE;
        string uri = Properties.ConfigFile.Default.API_URL_REQ;

        public void getRequisitions()
        {
            string[] sync_start = new string[] { DateTime.Now.ToString() + ": SYNC TASK - START: STOCK REQUISITION" };
            File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", sync_start);

            using (var client = new WebClient()) //WebClient  
            {
                try
                {
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");
                    client.Headers.Add("Authorization", "Bearer mME1wB9mnhnDHhWdKOlntgi5QcDad69J");
                    client.QueryString.Add("facility_code", facility_code);
                    var result = client.DownloadString(uri); //URI  
                    var root = JsonConvert.DeserializeObject<IEnumerable<Root>>(result);
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());

                    try
                    {
                        conn.Open();
                        foreach (var p in root)
                        {
                            DateTime orderDate = DateTime.Parse(p.ordered_dat);
                            string order_date = orderDate.ToString("yyyy-MM-ddTHH:mm:ss");
                            DateTime requisitionDate = DateTime.Parse(p.requisition_dat);
                            string reqDate = requisitionDate.ToString("yyyy-MM-ddTHH:mm:ss");
                            SqlCommand cmd = new SqlCommand("EXEC proc_AddRequisitions '"
                                         + p.demander_str.Replace(" ", string.Empty) + "', '"
                                         + p.requisition_str.Replace(" ", string.Empty) + "' , '"
                                         + reqDate + "' , '"
                                         + order_date + "' , '"
                                         + p.ordered_by_str.Replace(" ", string.Empty) + "' ", conn);
                            cmd.ExecuteNonQuery();
                            Console.WriteLine(Environment.NewLine + "Requisition inserted");
                            string[] alert1 = new string[] { DateTime.Now.ToString() + ":Requisition number: " + p.requisition_str.Replace(" ", string.Empty) + " Added" };
                            File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert1);
                            try
                            {
                                for (int i = 0; i < p.requisitionItems.Count(); i++)
                                {
                                    try
                                    {
                                        SqlCommand cmd1 = new SqlCommand("EXEC proc_AddRequisitionItems '"
                                        + p.requisitionItems[i].product_code.Replace(" ", string.Empty) + "', '"
                                        + p.requisitionItems[i].product_name.Replace(" ", string.Empty) + "' , '"
                                        + p.requisitionItems[i].order_qty + "'", conn);
                                        cmd1.ExecuteNonQuery();
                                        Console.WriteLine(Environment.NewLine + "Requisition inserted");
                                    }
                                    catch (SqlException e)
                                    {
                                        string[] alert2 = new string[] { DateTime.Now.ToString() + ":Requisition Item: " + p.requisitionItems[i].product_name.Replace(" ", string.Empty) + " Added" };
                                        File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert2);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(Environment.NewLine + ex.StackTrace);
                            }
                            finally
                            {
                                string[] alert = new string[] { DateTime.Now.ToString() + ": Requisitions have been Downloaded " };
                                File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert);
                                //Update CRDDP on Requisition Status
                                UpdateSourceTask update = new UpdateSourceTask();
                                update.updateRequisition(p.requisition_id);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(Environment.NewLine + e);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                catch (ArgumentNullException ex)
                {
                    //code specifically for a ArgumentNullException
                    string[] alert = new string[] { DateTime.Now.ToString() + ": REMOTE-API-CONNECTION : FAIL. " + ex.Message };
                    File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert);
                }
                catch (WebException ex)
                {
                    string[] alert = new string[] { DateTime.Now.ToString() + ": REMOTE-API-CONNECTION : FAIL. " + ex.Message };
                    File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert);
                }
                catch (Exception ex)
                {
                    //code for any other type of exception
                    string[] alert = new string[] { DateTime.Now.ToString() + ": REMOTE-API-CONNECTION : FAIL. " + ex.Message };
                    File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert);
                }
                finally
                {

                }
            }
            string[] sync_end = new string[] { DateTime.Now.ToString() + ": SYNC TASK - START: STOCK REQUISITION" };
            File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", sync_end);
        }
    }
}
