using GUI.ServiceReference1;
using System.ServiceProcess;
using System.Configuration;
using System;

namespace GUI
{
    internal class WCFClient
    {
        private readonly WCFServiceClient Client;

        public WCFClient()
        {
            try
            {
                StartService();
            }
            catch (Exception e)
            {
                throw e;
            }

            Client = new WCFServiceClient();
        }

        public void StartService()
        {
            ServiceController sc = new ServiceController(ConfigurationManager.AppSettings["ServiceName"]);

            // Check if service is running
            // if is not running, then try starting
            // and wait for it to change status
            if (sc.Status != ServiceControllerStatus.Running)
            {
                try
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}
