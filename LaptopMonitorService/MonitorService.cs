using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System;
using System.Timers;
using LaptopMonitorLibrary;
using System.IO;

namespace LaptopMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        private readonly EventLog LaptopMonitorLog;
        private readonly System.Timers.Timer ReadValuesTimer;
        private readonly JsonFile<MonitoringRecord> jsonFile;

        public MonitorService()
        {
            LaptopMonitorLog = SetupEventLog(ConfigurationManager.AppSettings["EventLogName"], ConfigurationManager.AppSettings["SourceName"]);

            try
            {
                jsonFile = new JsonFile<MonitoringRecord>(ConfigurationManager.AppSettings["DatabasePath"]);

            }
            catch (Exception)
            {
                var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var filepath = Path.Combine(systemPath, "database.json");
                jsonFile = new JsonFile<MonitoringRecord>(filepath);
            }

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
            SystemInfo SysInfo = new SystemInfo();
            jsonFile.Append(SysInfo.GetSystemInfo());
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
