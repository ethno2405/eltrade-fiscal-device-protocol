namespace EltradeProtocol.Requests
{
    public class OpenRefundReceipt : EltradeFiscalDeviceRequestPackage
    {
        public OpenRefundReceipt(string operatorName, string usn, string fiscalMemoryNumber, int fiscalReceiptNumber) : base(0x90, $"{operatorName},{usn},S,{fiscalMemoryNumber},R,{fiscalReceiptNumber}") { }
    }
}
