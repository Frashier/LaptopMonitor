using System;

namespace LaptopMonitorLibrary
{
    public class MonitoringRecord
    {
        public DateTime Date;
        public float Charge { get; set; }
        public float AverageCPUTemp { get; set; }
        public float AverageCPUClock { get; set; }
    }
}
