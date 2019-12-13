using System;

namespace EltradeProtocol.Requests
{
    public class CalculateTotal : EltradeFiscalDeviceRequestPackage
    {
        public CalculateTotal(string line1, string line2, PaymentType paymentType, decimal amount) : base(0x35)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), $"Payment amount '{amount}' must be positive");

            Append(Truncate(line1, 36));
            Append(LineFeed);
            Append(Truncate(line2, 36));
            Append(Tab);

            switch (paymentType)
            {
                case PaymentType.Cash:
                    Append("P");
                    break;
                case PaymentType.BankTransfer:
                    Append("M");
                    break;
                case PaymentType.CreditCard:
                    Append("L");
                    break;
                case PaymentType.Cheque:
                    Append("N");
                    break;
                default:
                    throw new NotSupportedException($"Not supported payment type '{paymentType}'");
            }

            Append(amount.ToString());
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
