using System;
using OnyxAPI_NET;

namespace LogFeature
{
    class Program
    {
        static void Main(string[] args)
        {
            // Enable log feature and set log file output path
            Api.EnableLog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Log files\Log.txt");
            var devices = DeviceInformation.ListDevices();

            // Disable log feature and call an API method
            //Api.DisableLog();
            //var devices = DeviceInformation.ListDevices();

            foreach (var logEvent in Api.LogEvents)
            {
                Console.WriteLine(logEvent);
            }
        }
    }
}
