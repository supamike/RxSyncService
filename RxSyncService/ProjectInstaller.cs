using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace RxSyncService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            //Set properties of the objects
            //1. Run service as local service
            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

            //2. Set properties of service installer    
            RxSyncService.ServiceName = "RxSyncService";
            RxSyncService.Description = "Rx Synchronization Service for Community Health Drug Distribution";
            RxSyncService.DisplayName = "RxSync Service";
            RxSyncService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

        }



    }
}
