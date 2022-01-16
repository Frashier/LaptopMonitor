using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using WCFMonitorLibrary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System;

namespace LaptopMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        private readonly EventLog LaptopMonitorLog;
        private readonly ServiceHost Host;

        public MonitorService()
        {
            LaptopMonitorLog = SetupEventLog(ConfigurationManager.AppSettings["EventLogName"], ConfigurationManager.AppSettings["SourceName"]);
            Host = SetupHost(new Uri(ConfigurationManager.AppSettings["BaseAddress"]));

            InitializeComponent();
        }

        private ServiceHost SetupHost(Uri baseAddress)
        {
            ServiceHost host = new ServiceHost(typeof(WCFService), baseAddress);
            host.AddServiceEndpoint(typeof(IWCFService), new WSHttpBinding(), "WCFService");

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            return host;
        }

        /// <summary>
        /// Method used for setting up event log for the service
        /// </summary>
        /// <returns></returns>
        private EventLog SetupEventLog(string eventLogName, string sourceName)
        {
            EventLog newEventLog = new EventLog(eventLogName);

            // Create source if it doesn't exist
            if (!EventLog.SourceExists(sourceName, "."))
            {
                EventLog.CreateEventSource(sourceName, eventLogName);
                Thread.Sleep(1000);
            }

            newEventLog.Source = sourceName;
            return newEventLog;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Host.Open();
            } 
            catch (Exception)
            {
                Host.Abort();
            }
            
        }

        protected override void OnStop()
        {
            try
            {
                Host.Close();
            }
            catch (Exception)
            {
                Host.Abort();
            }
        }
    }
}
