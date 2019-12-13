namespace EltradeProtocol.Requests
{
    public class OpenFiscalReceipt : EltradeFiscalDeviceRequestPackage
    {
        public OpenFiscalReceipt(string operatorName, string usn) : base(0x90, $"{operatorName},{usn}") { }
    }
}
