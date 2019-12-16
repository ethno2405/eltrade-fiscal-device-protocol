namespace EltradeProtocol.Requests
{
    public class PrintDailyReportByDepartments : EltradeFiscalDeviceRequestPackage
    {
        public PrintDailyReportByDepartments() : this(false) { }
        public PrintDailyReportByDepartments(bool zero) : base(0x75, zero ? "0" : "2") { }
    }
}
