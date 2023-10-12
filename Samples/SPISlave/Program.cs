using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using onyx_dotnet_api;

namespace SPISlave
{
    class Program
    {
        private static readonly byte[] SlaveResponse = new byte[] { 0xDC, 0xEE, 0xAA };
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

                // Set operation mode to I2C/SPI mode.
                myDevice.SetOperationMode(OperationMode.I2C_SPI);

                // Enable SPI slave
                myDevice.SPI.SlaveEnable();


                // Set slave response
                var status = myDevice.SPI.SlaveSetResponse(SlaveResponse, out var setSlaveResponseLength);
                Console.WriteLine($"Slave response length: {setSlaveResponseLength}");

                // Create AvailableAsyncData struct
                AvailableAsyncData availableAsyncData = new AvailableAsyncData();



                // Call AsyncPoll periodically to check if any data is arrived or written to the master
                // User can set the period as desired but too small period may cause connection loss
                var task = Task.Run(() =>
                {
                    Console.WriteLine("Waiting for data...");
                    // AnyData() returns true if there is available data
                    while (!availableAsyncData.AnyData())
                    {
                        // Poll data
                        status = myDevice.AsyncPoll(out availableAsyncData);
                        Thread.Sleep(100);
                    }
                });

                // Wait 5 seconds for slave operation. 
                // If operation times out, it means no data arrived to the SPI slave.
                if (task.Wait(5000))
                {
                    // Slave received or transmitted data
                    var receivedByteCount = availableAsyncData.AvailableSPIsReadBytes;

                    // Check received data count
                    if (receivedByteCount > 0)
                    {
                        byte[] slaveRxBytes = new byte[receivedByteCount];
                        status = myDevice.SPI.SlaveRead(receivedByteCount, out slaveRxBytes, out var actualReadByteCount);
                        Console.WriteLine($"Slave received {actualReadByteCount} bytes of data");
                        foreach (var rxByte in slaveRxBytes)
                        {
                            Console.Write(Convert.ToString(rxByte, 16) + " ");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Timeout! ");
                }

                // Disable SPI slave
                myDevice.SPI.SlaveDisable();

                // Close device
                myDevice.Disconnect();

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
