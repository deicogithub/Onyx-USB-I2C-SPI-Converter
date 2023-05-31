using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OnyxAPI_NET;

namespace ContinousSlaveOperation
{
    class Program
    {
        private static SlaveBackroundWorker slaveBackroundWorker;
        static void Main(string[] args)
        { 
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
                // Find a specific device from the device list. If no device is found return.
                var myDevice = deviceList.FirstOrDefault(x => x.SerialNumber == "DC000007A");
                if (myDevice is null) return;

                // Open device
                myDevice.Connect();

                // Set operation mode to I2C/SPI mode.
                myDevice.SetOperationMode(OperationMode.I2C_SPI);

                // Enable SPI
                myDevice.SPI.SlaveEnable();

                // Set slave response
                byte[] slaveResponse = new byte[] { 0xAA, 0xBB, 0xCC };
                var setSlaveResponseLength = myDevice.SPI.SlaveSetResponse(slaveResponse);

                // Create the background worker for continuous operation. Attach DataReceived event to listen received messages.
                slaveBackroundWorker = new SlaveBackroundWorker(myDevice);
                slaveBackroundWorker.DataReceivedEventHandler += (sender, arguments) =>
                {
                    var receivedByteCount = arguments.DataLength;
                    byte[] slaveRxBytes = new byte[receivedByteCount];
                    int actualReadByteCount = 0;

                    // Use lock object to prevent race condition
                    lock (SlaveBackroundWorker.lockObject)
                    {
                        actualReadByteCount = myDevice.SPI.SlaveRead(receivedByteCount, out slaveRxBytes);
                    }
                    Console.WriteLine($"Slave received {actualReadByteCount} bytes of data");

                    Console.WriteLine($"Received bytes: ");
                    foreach (var rxByte in slaveRxBytes)
                    {
                        Console.Write(Convert.ToString(rxByte, 16) + " ");
                    }
                    Console.WriteLine();

                    Console.WriteLine("Transmitted bytes: ");
                    for (int i = 0; i < actualReadByteCount; i++)
                    {
                        Console.Write(Convert.ToString(slaveResponse[i % setSlaveResponseLength], 16) + " ");
                    }
                    Console.WriteLine();
                };
                slaveBackroundWorker.StartWorker();

                Console.WriteLine("Slave operation started");
                Console.WriteLine("-----------------------");
                Console.WriteLine("Press enter to stop the operation");
                Console.Read();

                Console.WriteLine("Disabling slave...");
                slaveBackroundWorker.StopWorker();
                // Disable I2C slave
                myDevice.SPI.SlaveDisable();

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
