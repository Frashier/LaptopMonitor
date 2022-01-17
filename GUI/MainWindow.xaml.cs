using System.Windows;
using GUI.ServiceReference1;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WCFServiceClient Client;

        public MainWindow()
        {
            InitializeComponent();
            Client = new WCFServiceClient();
            ConsoleTextBlock.Text = Client.Echo("Test");
        }
    }
}
