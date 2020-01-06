namespace EltradeProtocol.Requests
{
    public class GetPrinterDiagnosticInfo : EltradeFiscalDeviceRequestPackage
    {
        public GetPrinterDiagnosticInfo() : base(0x5a, "1") { }
    }
}
