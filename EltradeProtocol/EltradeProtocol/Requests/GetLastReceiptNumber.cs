namespace EltradeProtocol.Requests
{
    public class GetLastReceiptNumber : EltradeFiscalDeviceRequestPackage
    {
        public GetLastReceiptNumber() : base(0x71) { }
    }
}
