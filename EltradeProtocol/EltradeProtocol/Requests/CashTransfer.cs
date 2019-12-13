namespace EltradeProtocol.Requests
{
    public class CashTransfer : EltradeFiscalDeviceRequestPackage
    {
        public CashTransfer(decimal amount) : base(0x46, amount.ToString()) { }
    }
}
