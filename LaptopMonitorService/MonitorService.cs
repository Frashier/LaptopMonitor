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
            LaptopMonitorLog.WriteEntry("MonitorService started");
            try
            {
                Host.Open();
                LaptopMonitorLog.WriteEntry("WCF host opened");
            } 
            catch (Exception)
            {
                Host.Abort();
                LaptopMonitorLog.WriteEntry("Error encountered when opening a host");
            }
            
        }

        protected override void OnStop()
        {
            LaptopMonitorLog.WriteEntry("MonitorService stopped");

            try
            {
                Host.Close();
            }
            catch (Exception)
            {
                Host.Abort();
            }

            LaptopMonitorLog.WriteEntry("WCF host closed");
        }
    }
}
