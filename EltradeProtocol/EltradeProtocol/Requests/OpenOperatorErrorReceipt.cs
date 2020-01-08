using System;

namespace EltradeProtocol.Requests
{
    public class OpenOperatorErrorReceipt : EltradeFiscalDeviceRequestPackage
    {
        public OpenOperatorErrorReceipt(string operatorName, string usn, string fiscalMemoryNumber, int fiscalReceiptNumber, DateTime issuedOn) : base(0x90, $"{operatorName},{usn},S,{fiscalMemoryNumber},O,{fiscalReceiptNumber},{issuedOn.ToString("yyyy-MM-ddTHH:mm:ss")}") { }
    }
}
