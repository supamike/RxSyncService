using RxSyncService.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.SyncTasks
{
    class Sync
    {
        public long NewIssueRecordsCount()
        {
            //string connString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
            long n = 0;
            String sql_from = "SELECT COUNT(*) as row_count "
                                    + " FROM TBLRequisition INNER JOIN TblRequisitionItems ON TBLRequisition.Requisition_ID = TblRequisitionItems.Requisition_ID "
                                    + " WHERE TBLRequisition.Requisition_str IS NOT NULL AND Requisition_dat IS NOT NULL "
                                    + " AND IssuedBy_str IS NOT NULL AND Issued_Dat IS NOT NULL "
                                    + " AND Received_dat IS NOT NULL AND ReceivedBy_str IS NOT NULL "
                                    + " AND reference = 'pharmacy' "
                                    + " AND TblRequisitionItems.QtyIssued_int IS NOT NULL "
                                    + " AND sync_status = 0";
            try
            {
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand cmd = new SqlCommand(sql_from, conn);
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr.Read())
                {
                    try
                    {
                        n = Convert.ToInt64(dr["row_count"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        n = 0;
                        Console.WriteLine(ice);
                    }
                }
                dr.Close();
            }
            catch (SqlException me)
            {
                n = 0;
                Console.WriteLine(me);
            }
            return n;
        }

        //Check whether internet is available

    }
}
