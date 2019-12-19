namespace EltradeProtocol.Requests
{
    public class VoidReceipt : EltradeFiscalDeviceRequestPackage
    {
        public VoidReceipt() : base(0x3c) { }
    }
}
