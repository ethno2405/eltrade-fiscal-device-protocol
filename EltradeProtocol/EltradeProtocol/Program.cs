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
            Send(new OpenFiscalReceipt("qwe", "ED325011-0050-0000012"));
            Send(new AddFreeTextToFiscalReceipt("Коментар"));
            Send(new RegisterPlu(1, 1));
            Send(new RegisterPlu(4444, 1));
            Send(new RegisterPlu(5555, 1));
            Send(new RegisterGoods("Салам", "", 'Б', 10, 2));
            Send(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
            Send(new AddFreeTextToFiscalReceipt("Втори коментар"));
            Send(new CalculateTotal("", "", CalculateTotal.PaymentType.Cash, 500.60m));
            Send(new CloseFiscalReceipt());

            //Send(new OpenOperatorErrorReceipt("asdf", "ED325011-0050-0000012", "44325011", 1419));
            //Send(new RegisterPlu(4444));
            //Send(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
            //Send(new CalculateTotal());
            //Send(new CloseFiscalReceipt());

            Send(new PrintDailyReportByDepartmentsAndArticles());
            driver?.Dispose();

            Console.ReadLine();
        }

        static void Send(EltradeFiscalDeviceRequestPackage pkg)
        {
            Print($"Request", pkg.Build(false));
            Console.WriteLine($"Package {pkg.GetType().Name}: {pkg.Command.ToString("x2")} {pkg.DataString}");
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
