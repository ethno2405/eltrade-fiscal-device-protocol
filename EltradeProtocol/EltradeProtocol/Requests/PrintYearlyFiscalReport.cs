using System;

namespace EltradeProtocol.Requests
{
    public class PrintYearlyFiscalReport : EltradeFiscalDeviceRequestPackage
    {
        public PrintYearlyFiscalReport(DateTime start) : base(0x4f, $"{start.ToString("yy")}") { }
    }
}
