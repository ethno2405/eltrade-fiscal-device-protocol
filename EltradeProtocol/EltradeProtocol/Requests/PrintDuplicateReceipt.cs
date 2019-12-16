namespace EltradeProtocol.Requests
{
    public class PrintDuplicateReceipt : EltradeFiscalDeviceRequestPackage
    {
        public PrintDuplicateReceipt() : base(0x6d) { }
    }
}
