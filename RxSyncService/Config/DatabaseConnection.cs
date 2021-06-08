using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace RxSyncService.Config
{
    class DatabaseConnection
    {
        private static String LOCAL_HOST = "";
        private static String LOCAL_DATABASE = "";
        private static String LOCAL_PORT = "";
        private static String LOCAL_USER = "";
        private static String LOCAL_PASSWORD = "";
        //private static DateTime LAST_SYNC_DATE;
        private static String FACILITY_CODE = Properties.ConfigFile.Default.FACILITY_CODE;
        private static String API_URL_REQ = "";
        private static String API_URL_ISSUE = "";
        public static long SYNC_INTERVAL = 3600000;//60000=1 min
        public static String LOG_PATH = "C:\\CRDDP\\RxSyncService_Log.txt";

        public static void readConfigFile()
        {
            if (Properties.ConfigFile.Default.SYNC_INTERVAL > 0)
            {
                DatabaseConnection.SYNC_INTERVAL = Properties.ConfigFile.Default.SYNC_INTERVAL;
            }
            if (Properties.ConfigFile.Default.LOG_PATH.Length > 0)
            {
                DatabaseConnection.LOG_PATH = Properties.ConfigFile.Default.LOG_PATH;
            }
            DatabaseConnection.LOCAL_HOST = Properties.ConfigFile.Default.LOCAL_HOST;
            DatabaseConnection.LOCAL_DATABASE = Properties.ConfigFile.Default.LOCAL_DATABASE;
            DatabaseConnection.LOCAL_PORT = Properties.ConfigFile.Default.LOCAL_PORT;
            DatabaseConnection.LOCAL_USER = Properties.ConfigFile.Default.LOCAL_USER;
            DatabaseConnection.LOCAL_PASSWORD = Properties.ConfigFile.Default.LOCAL_PASSWORD;
            DatabaseConnection.FACILITY_CODE = Properties.ConfigFile.Default.FACILITY_CODE;
            DatabaseConnection.API_URL_REQ = Properties.ConfigFile.Default.API_URL_REQ;
            DatabaseConnection.API_URL_ISSUE = Properties.ConfigFile.Default.API_URL_ISSUE;
        }

        public static Boolean IsConnection(SqlConnection aConn)
        {
            try
            {
                aConn.Open();
                if (aConn.State == ConnectionState.Open)
                {
                    aConn.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException se)
            {
                return false;
            }
        }

        public static Boolean IsConnectionLocal()
        {
            SqlConnection aConn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
            try
            {
                aConn.Open();
                if (aConn.State == ConnectionState.Open)
                {
                    aConn.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException se)
            {
                return false;
            }
        }

        public static Boolean IsConnectionRemote()
        {
            string api_url = Properties.ConfigFile.Default.API_URL_REQ;
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(api_url);
            Console.WriteLine("Status of Host: {0}", api_url);
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
                return false;
        }

        public static String getLocalConnectionString()
        {
            String LocalConnectionString = "";
            try
            {
                if (DatabaseConnection.LOCAL_PORT.Length > 0)
                {
                    LocalConnectionString = @"Data Source=" + DatabaseConnection.LOCAL_HOST + "," + DatabaseConnection.LOCAL_PORT + ";Initial Catalog=" + DatabaseConnection.LOCAL_DATABASE + ";User ID=" + DatabaseConnection.LOCAL_USER + ";Password=" + DatabaseConnection.LOCAL_PASSWORD + "";
                }
                else
                {
                    LocalConnectionString = @"Data Source=" + DatabaseConnection.LOCAL_HOST + ";Initial Catalog=" + DatabaseConnection.LOCAL_DATABASE + ";User ID=" + DatabaseConnection.LOCAL_USER + ";Password=" + DatabaseConnection.LOCAL_PASSWORD + "";
                }

            }
            catch (Exception e)
            {
                string[] alert = new string[] { DateTime.Now.ToString() + "INFO: Configuration File Error. Missing Port Number." };
                //File.AppendAllLines(@"C:\Users\modal\Documents\TETE\CrddpAPI_Log.txt", alert);
                File.AppendAllLines(LOG_PATH, alert);
            }
            finally
            {

            }
            return LocalConnectionString;
        }

        //Check for Connectivity to the PIP
        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("https://pip.health.go.ug"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
