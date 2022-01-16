using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace LaptopMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        private const string EventLogName = "LaptopMonitorLog";
        private const string SourceName = "LaptopMonitorSource";
        private readonly EventLog LaptopMonitorLog;

        public MonitorService()
        {
            LaptopMonitorLog = SetupEventLog();
            InitializeComponent();
        }

        /// <summary>
        /// Method used for setting up event log for the service
        /// </summary>
        /// <returns></returns>
        private EventLog SetupEventLog()
        {
            EventLog newEventLog = new EventLog(EventLogName);

            // Create source if it doesn't exist
            if (!EventLog.SourceExists(SourceName, "."))
            {
                EventLog.CreateEventSource(SourceName, EventLogName);
                Thread.Sleep(1000);
            }

            newEventLog.Source = SourceName;
            return newEventLog;
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
