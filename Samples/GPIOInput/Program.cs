using System;
using System.Linq;
using OnyxAPI_NET;

namespace GPIOInput
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get number of connected devices
            var numberOfConnectedDevices = DeviceInformation.GetNumberOfConnectedDevices();

            // Get device list
            var deviceList = DeviceInformation.ListDevices();

            // Print device information
            foreach (var device in deviceList)
            {
                Console.WriteLine($"Device description   : {device.Description}");
                Console.WriteLine($"Device id            : {device.Id}");
                Console.WriteLine($"Device serial number : {device.SerialNumber}");
                Console.WriteLine($"Device status        : {device.DeviceConnectionStatus}");
                Console.WriteLine($"------------------------------------------------------");
            }

            try
            {
                // Get device information at the specified index
                //var dev = DeviceInformation.GetDeviceAtIndex(0);

                // Find a specific device from the device list. If no device is found return.
                var myDevice = deviceList.FirstOrDefault(x => x.SerialNumber == "DC000007A");
                if (myDevice is null) return;

                // Open device
                myDevice.Connect();

                // Set operation mode
                myDevice.SetOperationMode(OperationMode.GPIO);

                // Set all pins as input.
                myDevice.GPIO.SetDirection(0);

                // Set pull mode of the Pin#5 and Pin#7 to pull-up
                myDevice.GPIO.SetPull((byte)(GPIO_Masks.GPIO_5_SPI_MISO | GPIO_Masks.GPIO_7_SPI_SCK));

                //Get values
                var valueMask = myDevice.GPIO.Read();

                Console.WriteLine($"Output values are set to: {Convert.ToString(valueMask, 2).PadLeft(8, '0')}");

                // Close device
                myDevice.Disconnect();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
