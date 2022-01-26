using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;

namespace LaptopMonitorLibrary
{
    public class SystemInfo
    {
        UpdateVisitor Visitor;
        Computer computer;

        public SystemInfo()
        {
            Visitor = new UpdateVisitor();
            computer = new Computer();

            computer.Open();
            computer.CPUEnabled = true;
            computer.Accept(Visitor);
        }

        ~SystemInfo()
        {
            computer.Close();
        }

        private float CalculateAverage(List<float> values)
        {
            float sum = 0;
            
            foreach (float value in values)
            {
                sum += value;
            }

            return (float) sum / values.Count;
        }

        public MonitoringRecord GetSystemInfo()
        {
            MonitoringRecord monitoringRecord = new MonitoringRecord();
            monitoringRecord.Date = DateTime.Now;

            // Get CPU frequencies and temperatures
            List<float> clocks = new List<float>();
            List<float> temps = new List<float>();
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Clock && computer.Hardware[i].Sensors[j].Name.StartsWith("CPU"))
                        {
                            clocks.Add((float) computer.Hardware[i].Sensors[j].Value);
                        }

                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            temps.Add((float)computer.Hardware[i].Sensors[j].Value);
                        }
                    }
                }
            }
            monitoringRecord.AverageCPUClock = CalculateAverage(clocks);
            monitoringRecord.AverageCPUTemp = CalculateAverage(temps);

            // Get battery charge
            var status = SystemInformation.PowerStatus;
            monitoringRecord.Charge = status.BatteryLifePercent;

            return monitoringRecord;
        }
    }
}
