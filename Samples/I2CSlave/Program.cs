using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using onyx_dotnet_api;

namespace I2CSlave
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

                // Enable I2C slave with the address 45
                myDevice.I2C.SlaveEnable(45);


                // Set slave response
                byte[] slaveResponse = new byte[] {0xAA, 0xBB, 0xCC};
                var status = myDevice.I2C.SlaveSetResponse(slaveResponse, out var setSlaveResponseLength);

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
                // If operation times out, it means no data arrived to the I2C slave.
                if (task.Wait(5000))
                {
                    // Slave received or transmitted data
                    var receivedByteCount = availableAsyncData.AvailableI2CsReadBytes;
                    var transmittedByteCount = availableAsyncData.AvailableI2CsWriteBytes;

                    // Check received data count
                    if (receivedByteCount > 0)
                    {
                        byte[] slaveRxBytes = new byte[receivedByteCount];
                        status = myDevice.I2C.SlaveRead(45, receivedByteCount, out slaveRxBytes, out var actualReadByteCount);
                        Console.WriteLine($"Slave received {actualReadByteCount} bytes of data");
                        foreach (var rxByte in slaveRxBytes)
                        {
                            Console.Write(Convert.ToString(rxByte, 16) + " ");
                        }
                    }

                    // Check transimtted data count
                    if (transmittedByteCount > 0)
                    {
                        status = myDevice.I2C.SlaveWriteStats(out var writtenByteCount);
                        Console.WriteLine($"Write stats: {writtenByteCount}");
                    }
                }
                else
                {
                    Console.WriteLine($"Timeout! ");
                }

                // Disable I2C slave
                myDevice.I2C.SlaveDisable();

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
