using System.Windows;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;
using LaptopMonitorLibrary;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using System.ServiceProcess;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JsonFile<MonitoringRecord> Records;
        CartesianChart Chart;
        public MainWindow()
        {
            Records = new JsonFile<MonitoringRecord>(ConfigurationManager.AppSettings["DatabasePath"]);
            InitializeComponent();

            // Create chart
            Chart = new CartesianChart();
            WindowGrid.Children.Add(Chart);
            Grid.SetRow(Chart, 0);
            Grid.SetColumn(Chart, 1);
            Grid.SetRowSpan(Chart, 3);
            Grid.SetColumnSpan(Chart, 3);
            DrawChart(Records.Data);

            // Update checkbox
            ServiceController sc = new ServiceController("MonitorService");
            if (sc.Status == ServiceControllerStatus.Running)
            {
                RunCheckBox.IsChecked = true;
            }
            else
            {     
                RunCheckBox.IsChecked = false;
            }
        }

        private void DrawChart(List<MonitoringRecord> data)
        {
            Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Battery charge",
                    Values = new ChartValues<float>(data.Select(x => x.Charge).ToArray()),
                    LineSmoothness = 0,
                    PointGeometry=null
                }
            };

            Chart.AxisY.Clear();
            Chart.AxisX.Clear();

            Chart.AxisY.Add(new Axis
            {
                Title = "Charge percentage",
                LabelFormatter = value => value.ToString("P"),
            });

            Chart.AxisX.Add(new Axis
            {
                Title = "Time",
                Labels = data.Select(x => x.Date.ToString("HH:mm")).ToArray()
            });

        }

        private void RunCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ServiceController sc = new ServiceController("MonitorService");
            sc.Start();
            sc.WaitForStatus(ServiceControllerStatus.Running);
        }
        private void RunCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ServiceController sc = new ServiceController("MonitorService");
            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DrawChart(Records.Data);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Records.Clear();
            DrawChart(Records.Data);
        }
    }
}
