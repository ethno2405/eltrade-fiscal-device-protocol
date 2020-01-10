using System;

namespace EltradeProtocol.Requests
{
    public class PrintMonthlyFiscalReport : EltradeFiscalDeviceRequestPackage
    {
        public PrintMonthlyFiscalReport(DateTime start) : base(0x4f, $"{start.ToString("MMy")}") { }
    }
}
