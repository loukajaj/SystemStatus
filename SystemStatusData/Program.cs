using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SystemStatusData
{
    class Program
    {
        static SystemStatusData systemStatusData = new SystemStatusData();
        static NetworkInterface[] _nics = NetworkInterface.GetAllNetworkInterfaces();
        static Process[] _processes = Process.GetProcesses();
        static private List<SystemStatusProcess> processes = new List<SystemStatusProcess>();
        protected static IMongoClient Client = new MongoClient();
        protected static IMongoDatabase Database = Client.GetDatabase("test");

        public static void Main(string[] args)
        {
            int menu = 0;

            while (menu != 5)
            {
                PrintMenu();
                try
                {
                    menu = int.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {}

                switch (menu)
                {
                    case 1:
                        GetCpuLoad();
                        break;
                    case 2:
                        PrintCpuLoad();
                        break;
                    case 3:
                        NetworkUsage();
                        break;
                    case 4:
                        StoreCpuData();
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Sorry, invalid selection");
                        break;
                }
            }
        }

        private static async void StoreCpuData()
        {
            var collection = Database.GetCollection<SystemStatusData>("cpudata");

            foreach (var process in processes)
            {
                SystemStatusProcess data = new SystemStatusProcess();
                
                data.ProcessId = process.ProcessId;
                data.ProcessName = process.ProcessName;
                data.ProcessorTime = process.ProcessorTime;
                data.ProcessorUserTime = process.ProcessorUserTime;
                data.PrivilegedProcessorTime = process.PrivilegedProcessorTime;
                data.WorkingSetMemory = process.WorkingSetMemory;
                data.WorkingSetPeakMemory = process.WorkingSetPeakMemory;
                data.WorkingSetPrivateMemory = process.WorkingSetPrivateMemory;
                data.Time = process.Time;

                systemStatusData.SystemStatusProcess.Add(data);
            }

        }

        private static void NetworkUsage()
        {
            NetworkStatusData data = new NetworkStatusData();
            foreach (NetworkInterface adapter in _nics)
            {
                data.InterfaceType = adapter.NetworkInterfaceType;
                data.PhysicalAddress = adapter.GetPhysicalAddress().ToString();
                data.OperationalStatus = adapter.OperationalStatus.ToString();
                data.BytesSent = adapter.GetIPv4Statistics().BytesSent;
                data.BytesReceived = adapter.GetIPv4Statistics().BytesReceived;

                systemStatusData.NetworkStatusData.Add(data);

                /*IPInterfaceProperties props = adapter.GetIPProperties(); ;
                Console.WriteLine();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}", adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Operational status ...................... : {0}", adapter.OperationalStatus);
                Console.WriteLine("  Bytes sent .............................. : {0}", adapter.GetIPv4Statistics().BytesSent);
                Console.WriteLine("  Bytes received .......................... : {0}", adapter.GetIPv4Statistics().BytesReceived);
                Console.WriteLine();*/
            }
        }

        private static async void PrintCpuLoad()
        {
            /*foreach (var process in processes)
            {
                Console.WriteLine();
                Console.WriteLine("{0}", process.ProcessName);
                Console.WriteLine(String.Empty.PadLeft(process.ProcessName.Length, '='));
                Console.WriteLine("  Process id .............................. : {0}", process.ProcessId);
                Console.WriteLine("  Processor time .......................... : {0}", process.ProcessorTime);
                Console.WriteLine("  Processor user time ..................... : {0}", process.ProcessorUserTime);
                Console.WriteLine("  Privileged processor time ............... : {0}", process.PrivilegedProcessorTime);
                Console.WriteLine("  Working set memory ...................... : {0}", process.WorkingSetMemory);
                Console.WriteLine("  Working set peak memory ................. : {0}", process.WorkingSetPeakMemory);
                Console.WriteLine("  Working set private memory .............. : {0}", process.WorkingSetPrivateMemory);
                Console.WriteLine("  Thread count ............................ : {0}", process.ThreadCount);
                Console.WriteLine("  Handle count ............................ : {0}", process.HandleCount);
                Console.WriteLine();
            }*/

            var collection = Database.GetCollection<BsonDocument>("cpudata");
            var count = await collection.CountAsync(new BsonDocument());
            var query = new BsonDocument();
            using (var cursor = await collection.FindAsync<BsonDocument>(query))
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (var doc in cursor.Current)
                    {
                        Console.WriteLine("{0}", doc.GetElement("Name").Value);
                    }
                }
            }
        }

        public static void GetCpuLoad()
        {
            foreach (var process in _processes)
            {
                process.Refresh();
                
                SystemStatusProcess systemData = new SystemStatusProcess();
                systemData.MachineName = System.Environment.MachineName;
                systemData.ProcessId = process.Id;
                systemData.ProcessName = process.ToString();
                PerformanceCounter totalProcessorTimeCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                systemData.ProcessorTime = totalProcessorTimeCounter.NextValue();

                PerformanceCounter userProcessorCounter = new PerformanceCounter("Process", "% User Time", process.ProcessName);
                systemData.ProcessorUserTime = userProcessorCounter.NextValue();

                PerformanceCounter privilegedProcessorCounter = new PerformanceCounter("Process", "% Privileged Time", process.ProcessName);
                systemData.PrivilegedProcessorTime = privilegedProcessorCounter.NextValue();

                PerformanceCounter workingSetMemoryCounter = new PerformanceCounter("Process", "Working Set", process.ProcessName);
                systemData.WorkingSetMemory = (long)workingSetMemoryCounter.NextValue();

                PerformanceCounter workingSetPeakMemoryCounter = new PerformanceCounter("Process", "Working Set Peak", process.ProcessName);
                systemData.WorkingSetPeakMemory = (long) workingSetPeakMemoryCounter.NextValue();

                PerformanceCounter workingSetPrivatememoryCounter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
                systemData.WorkingSetPrivateMemory = (long) workingSetPrivatememoryCounter.NextValue();

                PerformanceCounter threadCounter = new PerformanceCounter("Process", "Thread Count", process.ProcessName);
                systemData.ThreadCount = threadCounter.NextValue();

                PerformanceCounter handleCounter = new PerformanceCounter("Process", "Handle Count", process.ProcessName);
                systemData.HandleCount = handleCounter.NextValue();

                systemData.Time = DateTime.Now;

                processes.Add(systemData);
            }

            Console.WriteLine("System analysis done");
        }

        public static void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Menu");
            Console.WriteLine("===========");
            Console.WriteLine("1. CPU load");
            Console.WriteLine("2. Print CPU load");
            Console.WriteLine("3. Network usage");
            Console.WriteLine("4. Store CPU data");
            Console.WriteLine("5. Exit");
            Console.Write("Please enter your choice: ");
        }
    }
}
