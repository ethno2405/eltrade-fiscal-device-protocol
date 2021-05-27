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
            using (var transaction = EltradeFiscalDeviceDriver.GetInstance().BeginTransaction())
            {
                transaction.Enqueue(new SetDateTime());

                transaction.Enqueue(new OpenFiscalReceipt("op1", "ED325011-0050-0000012"));
                transaction.Enqueue(new AddFreeTextToFiscalReceipt("Коментар"));
                transaction.Enqueue(new RegisterPlu(1, 1));
                transaction.Enqueue(new RegisterPlu(4444, 1));
                transaction.Enqueue(new RegisterPlu(5555, 1));
                transaction.Enqueue(new RegisterGoods("Салам", "", 'Б', 10, 2));
                transaction.Enqueue(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
                transaction.Enqueue(new AddFreeTextToFiscalReceipt("Втори коментар"));
                transaction.Enqueue(new CalculateTotal("", "", CalculateTotal.PaymentType.Cash, 500.60m));
                transaction.Enqueue(new CloseFiscalReceipt());

                transaction.Enqueue(new OpenRefundReceipt("op2", "ED123456-0001-0000001", "44123456", 1419, DateTime.Now.AddDays(-1)));
                transaction.Enqueue(new RegisterPlu(4444));
                transaction.Enqueue(new RegisterGoods("Кучешка радост", "", 'Б', 20.0m, 2, -10.5m, DiscountType.Relative));
                transaction.Enqueue(new CalculateTotal());
                transaction.Enqueue(new CloseFiscalReceipt());

                transaction.Enqueue(new GetLastReceiptNumber(), x =>
                {
                    Console.WriteLine(x.GetHumanReadableData());
                    return true;
                });

                transaction.Commit();
            }
        }
    }
}
