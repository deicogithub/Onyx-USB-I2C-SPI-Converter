using System;
using System.Linq;
using OnyxAPI_NET;

namespace SPIMaster
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

                // Set operation mode to I2C/SPI mode.
                myDevice.SetOperationMode(OperationMode.I2C_SPI);

                // Configure SPI mode
                myDevice.SPI.SetConfig(SPI_Polarity.POLARITY_RISING_FALLING, SPI_Phase.PHASE_SAMPLE_SETUP);

                // Set SPI bit rate to 400 kHz
                myDevice.SPI.SetSpeed(SPI_Speed.SPI_Speed_1600kHz);

                // SPI master exchange operation
                byte[] txBytes = new byte[] { 0xFF, 0xFF, 0xFF };
                byte[] rxBytes = Array.Empty<byte>();

                // Send exchange command
                // First parameter of the MasterExchange() method is the slave select pin number.
                // It is currently not supported, only 1 slave select pin is available.
                var exchangedByteCount = myDevice.SPI.MasterExchange(0, txBytes, out rxBytes, txBytes.Length);
                
                // Print exchanged byte count
                Console.WriteLine($"Number of bytes exchanged is: {exchangedByteCount}");
                
                // Print received data
                Console.Write($"Received data: ");
                foreach (var rxByte in rxBytes)
                {
                    Console.Write(Convert.ToString(rxByte, 16) + " ");
                }

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
