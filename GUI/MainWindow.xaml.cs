using System.Windows;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;
using LaptopMonitorLibrary;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using System.ServiceProcess;
using System.Windows.Media;
using System;
using System.IO;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JsonFile<MonitoringRecord> Records;

        readonly CartesianChart ChargeChart;
        readonly CartesianChart TempChart;
        readonly CartesianChart ClockChart;

        public MainWindow()
        {
            try
            {
                Records = new JsonFile<MonitoringRecord>(ConfigurationManager.AppSettings["DatabasePath"]);
            }
            catch (Exception)
            {
                var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var filepath = Path.Combine(systemPath, "database.json");
                Records = new JsonFile<MonitoringRecord>(filepath);
            }

            InitializeComponent();

            // Create charge chart
            ChargeChart = new CartesianChart();
            WindowGrid.Children.Add(ChargeChart);
            Grid.SetRow(ChargeChart, 0);
            Grid.SetColumn(ChargeChart, 1);
            Grid.SetRowSpan(ChargeChart, 1);
            Grid.SetColumnSpan(ChargeChart, 3);
            DrawChargeChart(Records.Data);

            // Create temperatuers chart
            TempChart = new CartesianChart();
            WindowGrid.Children.Add(TempChart);
            Grid.SetRow(TempChart, 1);
            Grid.SetColumn(TempChart, 1);
            Grid.SetRowSpan(TempChart, 1);
            Grid.SetColumnSpan(TempChart, 3);
            DrawTempChart(Records.Data);

            // Create clocks chart
            ClockChart = new CartesianChart();
            WindowGrid.Children.Add(ClockChart);
            Grid.SetRow(ClockChart, 2);
            Grid.SetColumn(ClockChart, 1);
            Grid.SetRowSpan(ClockChart, 1);
            Grid.SetColumnSpan(ClockChart, 3);
            DrawClockChart(Records.Data);

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

        private void DrawClockChart(List<MonitoringRecord> data)
        {
            ClockChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Average Core Clock",
                    Values = new ChartValues<float>(data.Select(x => x.AverageCPUClock).ToArray()),
                    LineSmoothness = 0.2,
                    PointGeometry=null,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255)),
                    Fill = new SolidColorBrush(Color.FromArgb(60, 0, 153, 255))
                }
            };

            ClockChart.AxisY.Clear();
            ClockChart.AxisX.Clear();

            ClockChart.AxisY.Add(new Axis
            {
                Title = "Frequency (Hz)",
                LabelFormatter = value => value.ToString(),
            });

            ClockChart.AxisX.Add(new Axis
            {
                Title = "Time",
                Labels = data.Select(x => x.Date.ToString("HH:mm")).ToArray()
            });

        }

        private void DrawTempChart(List<MonitoringRecord> data)
        {
            TempChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Average Core Temperature",
                    Values = new ChartValues<float>(data.Select(x => x.AverageCPUTemp).ToArray()),
                    LineSmoothness = 0.2,
                    PointGeometry=null,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 204, 0, 0)),
                    Fill = new SolidColorBrush(Color.FromArgb(60, 204, 0, 0))
                }
            };

            TempChart.AxisY.Clear();
            TempChart.AxisX.Clear();

            TempChart.AxisY.Add(new Axis
            {
                Title = "Temperature (C)",
                LabelFormatter = value => value.ToString(),
            });

            TempChart.AxisX.Add(new Axis
            {
                Title = "Time",
                Labels = data.Select(x => x.Date.ToString("HH:mm")).ToArray()
            });

        }

        private void DrawChargeChart(List<MonitoringRecord> data)
        {
            ChargeChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Battery charge",
                    Values = new ChartValues<float>(data.Select(x => x.Charge).ToArray()),
                    LineSmoothness = 0.2,
                    PointGeometry=null,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 51, 204, 51)),
                    Fill = new SolidColorBrush(Color.FromArgb(60, 51, 204, 51))
                }
            };

            ChargeChart.AxisY.Clear();
            ChargeChart.AxisX.Clear();

            ChargeChart.AxisY.Add(new Axis
            {
                Title = "Charge percentage",
                LabelFormatter = value => value.ToString("P"),
            });

            ChargeChart.AxisX.Add(new Axis
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
            DrawChargeChart(Records.Data);
            DrawTempChart(Records.Data);
            DrawClockChart(Records.Data);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Records.Clear();
            DrawChargeChart(Records.Data);
            DrawTempChart(Records.Data);
            DrawClockChart(Records.Data);
        }
    }
}
