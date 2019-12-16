namespace EltradeProtocol.Requests
{
    public class PrintDailyReport : EltradeFiscalDeviceRequestPackage
    {
        public PrintDailyReport() : this(false) { }
        public PrintDailyReport(bool zero) : base(0x45, zero ? "0" : "2") { }
    }
}
