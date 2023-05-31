using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using OnyxAPI_NET;
using Microsoft.Win32;

namespace SPIFromFile
{
    class Program
    {
        private static readonly int ChunkSize = 2048;
        private static readonly string FileName = (Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\45kbdata.txt");
        private static readonly SPI_Speed SpiSpeed = SPI_Speed.SPI_Speed_3MHz;
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

                // Set SPI bit rate
                myDevice.SPI.SetSpeed(SpiSpeed);
                Console.WriteLine($"SPI Speed is set to: {SpiSpeed.GetDisplayName()}");


                FileStream file;
                int transNum = 0;
                int totalBytesWritten = 0;
                Stopwatch stopWatch = new Stopwatch();

                // Open the file
                try
                {
                    Console.WriteLine("Opening file...");
                    file = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to open file '{0}'", FileName);
                    return;
                }

                Console.WriteLine("Starting transaction...");
                Console.WriteLine("-----------------------");
                stopWatch.Start();
                while (file.Position != file.Length)
                {
                    byte[] txBytes = new byte[ChunkSize];
                    byte[] rxBytes = new byte[ChunkSize];

                    int numWrite, count;
                    int i;

                    numWrite = file.Read(txBytes, 0, ChunkSize);
                    if(numWrite == 0) break;

                    if (numWrite < ChunkSize)
                    {
                        byte[] tempBytes = new byte[numWrite];
                        Array.Copy(txBytes, tempBytes, numWrite);
                        txBytes = tempBytes;
                    }

                    var numberOfBytesWritten = myDevice.SPI.MasterExchange(0, txBytes, out rxBytes, numWrite);

                    if (numberOfBytesWritten < 0)
                    {
                        Console.WriteLine($"Error during transaction. Status code: {(Status)numberOfBytesWritten}");
                    }
                    else if (numberOfBytesWritten == 0)
                    {
                        Console.WriteLine("No bytes are written");
                    }
                    else if (numberOfBytesWritten != numWrite)
                    {
                        Console.WriteLine($"Error during transaction. Written byte count: {numberOfBytesWritten}");
                    }


                    // Uncomment to print received data
                    //Console.Write($"Received data: ");
                    //foreach (var rxByte in rxBytes)
                    //{
                    //    Console.Write(Convert.ToString(rxByte, 16) + " ");
                    //}

                    Console.WriteLine();
                    Console.WriteLine($"Transaction #{transNum}");
                    ++transNum;
                    totalBytesWritten += numberOfBytesWritten;
                    Thread.Sleep(10);
                }
                stopWatch.Stop();
                Console.WriteLine($"Total bytes written: {totalBytesWritten}");
                Console.WriteLine($"Duration: {stopWatch.Elapsed}");
               
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
