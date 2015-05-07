using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;

namespace SystemStatusData
{
    class Program
    {
        static SystemStatusData systemStatusData = new SystemStatusData(System.Environment.MachineName);
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
                catch (Exception)
                {}

                switch (menu)
                {
                    case 1:
                        StoreData();
                        break;
                    case 2:
                        PrintCpuLoad();
                        break;
                    case 3:
                        GetNetworkLoad();
                        break;
                    case 4:
                        StoreData();
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Sorry, invalid selection");
                        break;
                }
            }
        }

        private static async void StoreData()
        {
            Console.WriteLine("Gathering system information and writing to disk, please wait...");
            GetTotalRamValues();
            systemStatusData.ProcessorCount = Environment.ProcessorCount;
            systemStatusData.Time = DateTime.Now;

            bool addToDb = true;
            try
            {
                GetCpuLoad();
            }
            catch (Exception e)
            {
                addToDb = false;
                Console.WriteLine("Failed to fetch CPU information");
                Console.WriteLine(e);
            }
            try
            {
                GetNetworkLoad();
            }
            catch (Exception e)
            {
                addToDb = false;
                Console.WriteLine("Failed to fetch network information");
                Console.WriteLine(e);
            }
            try
            {
                GetMemoryLoad();
            }
            catch (Exception e)
            {
                addToDb = false;
                Console.WriteLine("Failed to fetch memory information");
                Console.WriteLine(e);
            }

            if (addToDb)
            {
                Console.WriteLine("System information successfully stored");
                var collection = Database.GetCollection<SystemStatusData>("SystemStatusData");
                await collection.InsertOneAsync(systemStatusData);
            }
        }

        private static void GetTotalRamValues()
        {
            PerformanceCounter usedRamCounter = new PerformanceCounter();
            usedRamCounter.CounterName = "Committed Bytes";
            usedRamCounter.CategoryName = "Memory";
            long usedRam = (long) usedRamCounter.NextValue();
            systemStatusData.UsedRam = usedRam;

            PerformanceCounter freeRamCounter = new PerformanceCounter();
            freeRamCounter.CounterName = "Available Bytes";
            freeRamCounter.CategoryName = "Memory";
            long freeRam = (long) freeRamCounter.NextValue();
            systemStatusData.FreeRam = freeRam;

            systemStatusData.TotalRam = freeRam + usedRam;
        }

        private static void GetMemoryLoad()
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                MemoryStatus memStat = new MemoryStatus();
                memStat.PhysicalMemoryUsage = process.WorkingSet64;
                memStat.BasePriority = process.BasePriority;
                try
                {
                    memStat.PriorityClass = process.PriorityClass.ToString();
                }
                catch (Exception)
                {
                    memStat.PriorityClass = "Unavailable";
                }
                
                memStat.PagedSystemMemorySize = process.PagedSystemMemorySize64;
                memStat.PagedMemorySize = process.PagedMemorySize64;
                memStat.VirtualMemorySize = process.VirtualMemorySize64;
                memStat.NonpagedSystemMemorySize = process.NonpagedSystemMemorySize64;
                memStat.PeakPagedMemorySize = process.PeakPagedMemorySize64;
                memStat.PeakVirtualMemorySize = process.PeakVirtualMemorySize64;
                memStat.PeakWorkingSetMemorySize = process.PeakWorkingSet64;
                
                systemStatusData.MemoryStatus.Add(memStat);
            }
        }

        private static void GetNetworkLoad()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                NetworkStatusData data = new NetworkStatusData();
                data.InterfaceType = adapter.NetworkInterfaceType.ToString();
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

        public static void GetCpuLoad()
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                process.Refresh();
                SystemStatusProcess systemData = new SystemStatusProcess();
                systemData.Processname = process.ProcessName.ToString();
                systemData.ProcessId = process.Id;
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

                systemStatusData.SystemStatusProcess.Add(systemData);
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
        }

        public static void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Menu");
            Console.WriteLine("===========");
            Console.WriteLine("1. Gather and store data");
            Console.WriteLine("2. Print CPU load");
            Console.WriteLine("3. Network usage");
            Console.WriteLine("4. Store CPU data");
            Console.WriteLine("5. Exit");
            Console.Write("Please enter your choice: ");
        }
    }
}
