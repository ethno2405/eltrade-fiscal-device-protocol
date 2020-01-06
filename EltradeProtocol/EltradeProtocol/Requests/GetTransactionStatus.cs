namespace EltradeProtocol.Requests
{
    public class GetTransactionStatus : EltradeFiscalDeviceRequestPackage
    {
        public GetTransactionStatus() : base(0x4c, "T") { }
    }
}
