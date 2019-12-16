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
            Send(new OpenFiscalReceipt("qwe", "ED325011-0050-0000011"));
            Send(new AddFreeTextToFiscalReceipt("Коментар"));
            Send(new RegisterArticle("Бахур", "със сланина", 'Б', 5.68m, 3));
            Send(new RegisterArticle("Салам", "", 'Б', 10, 2));
            Send(new RegisterArticle("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, RegisterArticle.DiscountType.Relative));
            Send(new AddFreeTextToFiscalReceipt("Втори коментар"));
            Send(new CalculateTotal("", "", CalculateTotal.PaymentType.Cash, 500.60m));
            Send(new CloseFiscalReceipt());

            Send(new PrintDailyReportByDepartmentsAndArticles());
            //Send(new OpenOperatorErrorReceipt("asdf", "ED325011-0050-0000010", "44325011", 1372));
            ////Send(new RegisterArticle("Бахур", "със сланина", 'Б', 5.68m, 1));
            //Send(new CalculateTotal());
            //Send(new CloseFiscalReceipt());

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
