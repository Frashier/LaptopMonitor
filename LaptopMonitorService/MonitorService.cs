using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System;
using System.Timers;
using System.Windows.Forms;
using LaptopMonitorLibrary;

namespace LaptopMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        private readonly EventLog LaptopMonitorLog;
        private readonly System.Timers.Timer ReadValuesTimer;
        private readonly JsonFile<MonitoringRecord> jsonFile;
        private readonly OpenHardwareMonitor.Hardware.Computer _Computer;

        public MonitorService()
        {
            LaptopMonitorLog = SetupEventLog(ConfigurationManager.AppSettings["EventLogName"], ConfigurationManager.AppSettings["SourceName"]);
            _Computer = new OpenHardwareMonitor.Hardware.Computer { CPUEnabled = true };
            jsonFile = new JsonFile<MonitoringRecord>(ConfigurationManager.AppSettings["DatabasePath"]);
            ReadValuesTimer = SetupTimer();
            InitializeComponent();
        }

        private System.Timers.Timer SetupTimer()
        {
            int interval;
            try
            {
                interval = Int32.Parse(ConfigurationManager.AppSettings["ReadInterval"]);
            }
            catch(Exception)
            {
                return null;
            }

            System.Timers.Timer timer = new System.Timers.Timer(interval);
            timer.Elapsed += ReadValues;
            timer.AutoReset = true;

            return timer;
        }

        private void ReadValues(Object source, ElapsedEventArgs e)
        {
            var status = SystemInformation.PowerStatus;

            MonitoringRecord data = new MonitoringRecord
            {
                Name = "Charge",
                Value = status.BatteryLifePercent
            };

            jsonFile.Append(data);
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
            ReadValuesTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            LaptopMonitorLog.WriteEntry("MonitorService stopped");
            ReadValuesTimer.Enabled = false;
        }
    }
}
