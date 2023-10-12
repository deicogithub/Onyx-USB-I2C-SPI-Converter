using System;
using System.Linq;
using onyx_dotnet_api;

namespace I2CMaster
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
                if(myDevice is null) return;

                // Open device
                myDevice.Connect();

                // Set operation mode to I2C/SPI mode.
                myDevice.SetOperationMode(OperationMode.I2C_SPI);

                // Set I2C bit rate to 400 kHz
                myDevice.I2C.SetBitRate(new BitRate() { Value= 400000 }, out var actualBitRateSet);

                // I2C master write operation
                byte[] txBytes = new byte[] { 0xFF, 0xFF, 0xFF };
                var status = myDevice.I2C.MasterWrite(0x15, txBytes, out var writtenByteCount);
                Console.WriteLine($"Number of bytes written is: {writtenByteCount}");

                // I2C master read operation
                byte[] rxBytes = Array.Empty<byte>();
                status = myDevice.I2C.MasterRead(0x15, 3, out rxBytes, out var readByteCount);
                Console.WriteLine($"Number of bytes read is: {readByteCount}");

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
