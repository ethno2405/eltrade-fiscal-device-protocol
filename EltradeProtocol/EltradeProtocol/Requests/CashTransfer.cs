using System;
using System.Globalization;

namespace EltradeProtocol.Requests
{
    public class CashTransfer : EltradeFiscalDeviceRequestPackage
    {
        public CashTransfer(decimal amount) : base(0x46, Math.Round(amount, 2).ToString(CultureInfo.InvariantCulture)) { }
    }
}
