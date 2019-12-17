using System;
using System.Text;
using EltradeProtocol.Requests;

namespace EltradeProtocol
{
    public static class Program
    {
        static Encoding windows1251 = Encoding.GetEncoding("windows-1251");

        static void Main(string[] args)
        {
            Console.OutputEncoding = windows1251;
            using (var driver = new EltradeFiscalDeviceDriver())
            {
                driver.Send(new SetDateTime());
            }

            using (var driver = new EltradeFiscalDeviceDriver())
            {
                driver.Send(new OpenFiscalReceipt("qwe", "ED325011-0050-0000012"));
                driver.Send(new AddFreeTextToFiscalReceipt("Коментар"));
                driver.Send(new RegisterPlu(1, 1));
                driver.Send(new RegisterPlu(4444, 1));
                driver.Send(new RegisterPlu(5555, 1));
                driver.Send(new RegisterGoods("Салам", "", 'Б', 10, 2));
                driver.Send(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
                driver.Send(new AddFreeTextToFiscalReceipt("Втори коментар"));
                driver.Send(new CalculateTotal("", "", CalculateTotal.PaymentType.Cash, 500.60m));
                driver.Send(new CloseFiscalReceipt());
            }

            using (var driver = new EltradeFiscalDeviceDriver())
            {
                driver.Send(new OpenOperatorErrorReceipt("asdf", "ED325011-0050-0000012", "44325011", 1419));
                driver.Send(new RegisterPlu(4444));
                driver.Send(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
                driver.Send(new CalculateTotal());
                driver.Send(new CloseFiscalReceipt());
            }

            using (var driver = new EltradeFiscalDeviceDriver())
            {
                driver.Send(new GetLastReceiptNumber());
            }

            using (var driver = new EltradeFiscalDeviceDriver())
            {
                driver.Send(new PrintDailyReportByDepartmentsAndArticles());
            }

            Console.ReadLine();
        }
    }
}
