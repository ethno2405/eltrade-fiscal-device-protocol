namespace EltradeProtocol.Requests
{
    public class GetFiscalTransactionStatus : EltradeFiscalDeviceRequestPackage
    {
        public GetFiscalTransactionStatus() : base(0x4c, "T") { }
    }
}
