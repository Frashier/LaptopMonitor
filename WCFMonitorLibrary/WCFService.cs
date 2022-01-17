namespace WCFMonitorLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class WCFService : IWCFService
    {
       public string Echo(string message)
        {
            return message;
        }
        
    }
}
