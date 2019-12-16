namespace EltradeProtocol.Requests
{
    public class PrintDailyReportByDepartmentsAndArticles : EltradeFiscalDeviceRequestPackage
    {
        public PrintDailyReportByDepartmentsAndArticles() : this(false) { }
        public PrintDailyReportByDepartmentsAndArticles(bool zero) : base(0x76, zero ? "0" : "2") { }
    }
}
