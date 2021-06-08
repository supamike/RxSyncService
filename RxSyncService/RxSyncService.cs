using RxSyncService.Config;
using RxSyncService.SyncTasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService
{
    public partial class RxSyncService : ServiceBase
    {
        //Adding event log
        public RxSyncService()
        {
            InitializeComponent();

            DatabaseConnection.readConfigFile();
            // Set up a timer to trigger every set of time; note 1 min=60s=60000.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = DatabaseConnection.SYNC_INTERVAL; // 60000=60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            eventLog1.Log = "Application";
            //Name that will appear on the source column
            eventLog1.Source = "RxSyncService";
        }

        public RxSyncService(int code)
        {
            DatabaseConnection.readConfigFile();
        }

        //Write on the log when service starts
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("RxSyncService has Started", EventLogEntryType.Information);
            try
            {
                this.WriteToFile("Service started");
            }
            catch (Exception e)
            {
                this.WriteToFile("Error on service start");
            }
        }
        //Write on the log when service stops
        protected override void OnStop()
        {
            eventLog1.WriteEntry("RxSyncService has Stopped", EventLogEntryType.Information);
            try
            {
                this.WriteToFile("Service stopped");
            }
            catch (Exception e)
            {
                this.WriteToFile("Error on service stop");
            }
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            this.TasksAll();
        }

        public void TasksAll()
        {
            DateTime startDate = DateTime.Now;
            //Check for Remote server connection
            if (DatabaseConnection.CheckInternetConnection())
            {
                //Check for Local database connection
                if (DatabaseConnection.IsConnectionLocal())
                {
                    WriteToFile("START-SYNC : Started");
                    Sync task = new Sync();
                    if (task.NewIssueRecordsCount() == 0)
                    {
                        this.WriteToFile("INFO: NO ISSUES DATA TO SYNC:");
                    }
                    if (task.NewIssueRecordsCount() > 0)
                    {
                        SyncIssuesData syncIssues = new SyncIssuesData();
                        syncIssues.syncIssueData();
                        this.WriteToFile("INFO: SUCCESSFULLY UPLOADED ISSUES DATA:");
                    }
                    else if (task.NewIssueRecordsCount() <= 0)
                    {
                        syncRequisitions req = new syncRequisitions();
                        req.getRequisitions();
                        this.WriteToFile("INFO: SUCCESSFULLY DOWNLOADED REQUISITION DATA:");
                    }
                }
                else
                {
                    this.WriteToFile("INFO: THERE ARE NO RECORDS TO SYNC");
                }
            }
            else
            {
                this.WriteToFile("INFO: REMOTE-SERVER IS NOT REACHABLE");
            }
            //Complete the Synchronization 
            this.WriteToFile("END-SYNC : Completed");
        }

        public void WriteToFile(string text)
        {
            string path = DatabaseConnection.LOG_PATH;
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(DateTime.Now.ToString("dd/MM/yy hh:mm:ss tt") + ": " + text);
                writer.Close();
            }
        }

        //Adding an installer on Designer

    }
}
