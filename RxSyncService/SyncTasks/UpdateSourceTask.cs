using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.SyncTasks
{
    class UpdateSourceTask
    {
        public async Task updateRequisition(int requisition_id)
        {
            string uri = Properties.ConfigFile.Default.API_URL_REQ;

            try
            {
                var pocoObject = new
                {
                    requisition_id = requisition_id,
                    //requisition_id = 7,
                    status = "elmis",
                    status_description = "Sent to elmis for issue"
                };
                string json = JsonConvert.SerializeObject(pocoObject);

                //var client = new RestClient("http://197.157.32.194/elmisAPI/cddp/requisitions");
                var client = new RestClient(uri);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer mME1wB9mnhnDHhWdKOlntgi5QcDad69J");
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                //request.AddParameter("application/json", "{\"requisition_id\":7,\"status\":\"elmis\",\"status_description\":\"Sent to elmis for issue\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                string[] alert = new string[] { DateTime.Now.ToString() + "INFO: Requisition ID " + requisition_id + ". Status Update!" };
                File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                string[] alert = new string[] { DateTime.Now.ToString() + ": Error : " + ex.Message };
                File.AppendAllLines(@"C:\CRDDP\RxAPI_Log.txt", alert);
            }
            finally
            {
                //Do nothing
            }
        }
    }
}
