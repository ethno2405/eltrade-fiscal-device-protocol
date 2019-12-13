namespace EltradeProtocol.Requests
{
    public class AddFreeTextToFiscalReceipt : EltradeFiscalDeviceRequestPackage
    {
        public AddFreeTextToFiscalReceipt(string text) : base(0x36)
        {
            AppendData(text);
        }
    }
}
