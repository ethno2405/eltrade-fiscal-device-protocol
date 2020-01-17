using System;
using System.Text;
using EltradeProtocol.Requests;

namespace EltradeProtocol
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding("windows-1251");
            Print();

            Console.ReadLine();
        }

        static void Print()
        {
            using (var driver = EltradeFiscalDeviceDriver.GetInstance())
            {
                driver.Send(new SetDateTime());

                driver.Send(new OpenFiscalReceipt("op1", "ED325011-0050-0000012"));
                driver.Send(new AddFreeTextToFiscalReceipt("Коментар"));
                driver.Send(new RegisterPlu(1, 1));
                driver.Send(new RegisterPlu(4444, 1));
                driver.Send(new RegisterPlu(5555, 1));
                driver.Send(new RegisterGoods("Салам", "", 'Б', 10, 2));
                driver.Send(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
                driver.Send(new AddFreeTextToFiscalReceipt("Втори коментар"));
                driver.Send(new CalculateTotal("", "", CalculateTotal.PaymentType.Cash, 500.60m));
                driver.Send(new CloseFiscalReceipt());

                driver.Send(new OpenRefundReceipt("op2", "ED123456-0001-0000001", "44123456", 1419, DateTime.Now.AddDays(-1)));
                driver.Send(new RegisterPlu(4444));
                driver.Send(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
                driver.Send(new CalculateTotal());
                driver.Send(new CloseFiscalReceipt());

                var number = driver.Send(new GetLastReceiptNumber()).GetHumanReadableData();
                Console.WriteLine(number);
            }
        }
    }
}
