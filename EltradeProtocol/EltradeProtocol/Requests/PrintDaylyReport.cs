namespace EltradeProtocol.Requests
{
    public class PrintDaylyReport : EltradeFiscalDeviceRequestPackage
    {
        public PrintDaylyReport() : this(false) { }
        public PrintDaylyReport(bool zero) : base(0x45, zero ? "0" : "2") { }
    }
}
