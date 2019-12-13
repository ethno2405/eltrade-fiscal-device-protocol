using System;
using System.Linq;
using System.Text;
using EltradeProtocol.Requests;

namespace EltradeProtocol
{
    public static class Program
    {
        static EltradeFiscalDeviceDriver driver;
        static Encoding windows1251 = Encoding.GetEncoding("windows-1251");

        static void Main(string[] args)
        {
            Console.OutputEncoding = windows1251;
            driver = new EltradeFiscalDeviceDriver();

            Console.WriteLine("Sending package...");

            Send(new SetDateTime());
            Send(new OpenFiscalReceipt("qwe", "ED325011-0050-0000002"));
            Send(new RegisterArticle("bahur", "mazen", 'Б', 5.68m, 3));
            Send(new RegisterArticle("salam", "", 'Б', 10, 2));
            Send(new RegisterArticle("bahur1", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
            Send(new CloseFiscalReceipt());

            driver?.Dispose();

            Console.ReadLine();
        }

        static void Send(EltradeFiscalDeviceRequestPackage pkg)
        {
            var payload = pkg.Build(false);
            Console.WriteLine($"Payload: {windows1251.GetString(payload)}");
            Print($"Package {pkg.GetType().Name}", pkg.Build(false));
            var response = driver.Send(pkg);
            PrintResponse(response);
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
            var package = string.Join("", bytes.Select(x => x.ToString(format)).ToArray()).ToUpper();
            Console.WriteLine($"{type}: " + package);
        }

        static void PrintFlags(string type, byte[] bytes)
        {
            var package = string.Join(" ", bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray());
            Console.WriteLine($"{type}: " + package);
        }
    }
}
