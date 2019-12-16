using System;

namespace EltradeProtocol.Requests
{
    public class CalculateTotal : EltradeFiscalDeviceRequestPackage
    {
        public CalculateTotal(string line1, string line2, PaymentType paymentType, decimal amount) : base(0x35)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), $"Payment amount '{amount}' must be non-negative");

            AppendData(Truncate(line1, 36));
            AppendData(LineFeed);
            AppendData(Truncate(line2, 36));
            AppendData(Tab);

            switch (paymentType)
            {
                case PaymentType.Cash:
                    AppendData("P");
                    break;
                case PaymentType.BankTransfer:
                    AppendData("M");
                    break;
                case PaymentType.CreditCard:
                    AppendData("L");
                    break;
                case PaymentType.Cheque:
                    AppendData("N");
                    break;
                default:
                    throw new NotSupportedException($"Not supported payment type '{paymentType}'");
            }

            AppendData(amount.ToString());
        }

        public enum PaymentType
        {
            Cash,
            BankTransfer,
            CreditCard,
            Cheque
        }
    }
}
