using System;
using System.Linq;

namespace EltradeProtocol
{
    public static class Program
    {
        static EltradeFiscalDeviceDriver driver;

        static void Main(string[] args)
        {
            driver = new EltradeFiscalDeviceDriver();

            while (true)
            {
                var message = Console.ReadLine();

                if ("q".Equals(message, StringComparison.OrdinalIgnoreCase))
                    break;

                if ("p".Equals(message, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Sending package...");
                    var response = driver.Send(EltradeFiscalDeviceRequestPackage.SetCurrentDateTime);
                    PrintResponse(response);
                }
            }

            driver?.Dispose();
        }

        static void PrintResponse(EltradeFiscalDeviceResponsePackage response)
        {
            Print("Response package", response.Package);
            Print("Data package", response.Data);
            Print("Status package", response.Status);
            PrintFlags("Data flags", response.Data);
            PrintFlags("Status flags", response.Status);
            Console.WriteLine();
        }

        static void Print(string type, byte[] bytes, string format = "x2")
        {
            var package = string.Join(" ", bytes.Select(x => x.ToString(format)).ToArray());
            Console.WriteLine($"{type}: " + package);
        }

        static void PrintFlags(string type, byte[] bytes)
        {
            var package = string.Join(" ", bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray());
            Console.WriteLine($"{type}: " + package);
        }
    }
}
