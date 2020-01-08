using System;

namespace EltradeProtocol.Requests
{
    public class OpenRefundReceipt : EltradeFiscalDeviceRequestPackage
    {
        public OpenRefundReceipt(string operatorName, string usn, string fiscalMemoryNumber, int fiscalReceiptNumber, DateTime issuedOn) : base(0x90, $"{operatorName},{usn},S,{fiscalMemoryNumber},R,{fiscalReceiptNumber},{issuedOn.ToString("yyyy-MM-ddTHH:mm:ss")}") { }
    }
}
