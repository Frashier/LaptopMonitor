using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using LaptopMonitorLibrary;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public SeriesCollection Series { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public Chart()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  Create series to display on chart
        /// </summary>
        /// <param name="data"></param>
        private SeriesCollection CreateSeries(List<MonitoringRecord> data)
        {
            // Sort by date
            data.Sort((a, b) => a.Date.CompareTo(b.Date));

            // Add labels
            Labels = data.Select(x => x.Date.ToString()).ToArray();

            // Create series
            var lineSeries = new LineSeries
            {
                Values = new ChartValues<float>(data.Select(x => x.Charge).ToArray()),
                LineSmoothness = 0
            };

            return new SeriesCollection(lineSeries);
        }
    }
}
