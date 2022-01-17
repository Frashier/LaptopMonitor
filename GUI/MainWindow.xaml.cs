using System;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WCFClient CommunicationClient;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                CommunicationClient = new WCFClient();
                WriteToConsole("Connection with service estabilished");
            }
            catch (Exception e)
            {
                WriteToConsole("Error during starting service");
            }
        }

        private void WriteToConsole(string message)
        {
            ConsoleTextBlock.Text += "[" + DateTime.Now.ToString() + "]: " + message + "\n";
        }
    }
}
