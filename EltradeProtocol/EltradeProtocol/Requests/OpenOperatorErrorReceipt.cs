namespace EltradeProtocol.Requests
{
    public class OpenOperatorErrorReceipt : EltradeFiscalDeviceRequestPackage
    {
        public OpenOperatorErrorReceipt(string operatorName, string usn, string fiscalMemoryNumber, int fiscalReceiptNumber) : base(0x90, $"{operatorName},{usn},S,{fiscalMemoryNumber},O,{fiscalReceiptNumber}") { }
    }
}
