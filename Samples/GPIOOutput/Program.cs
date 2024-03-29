﻿using System;
using System.Linq;
using onyx_dotnet_api;

namespace GPIOOutput
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
                var myDevice = deviceList.FirstOrDefault(x => x.SerialNumber == "DC000026A");
                if (myDevice is null) return;

                // Open device
                myDevice.Connect();

                // Set operation mode
                myDevice.SetOperationMode(OperationMode.GPIO);

                // Set Pin #5 and Pin #7 as output.
                myDevice.GPIO.SetDirection((byte)(GPIO_Masks.GPIO_5_SPI_MISO | GPIO_Masks.GPIO_7_SPI_SCK));

                // Set output value of Pin #7 to high
                myDevice.GPIO.SetOutput((byte)(GPIO_Masks.GPIO_7_SPI_SCK));

                //Get values
                var status = myDevice.GPIO.Read(out var valueMask);

                Console.WriteLine($"Output values are set to: {Convert.ToString(valueMask, 2).PadLeft(8,'0')}");

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
