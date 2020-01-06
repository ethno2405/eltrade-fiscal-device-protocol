namespace EltradeProtocol.Requests
{
    public class GetReceiptStatus : EltradeFiscalDeviceRequestPackage
    {
        public GetReceiptStatus() : base(0x67) { }
    }
}
