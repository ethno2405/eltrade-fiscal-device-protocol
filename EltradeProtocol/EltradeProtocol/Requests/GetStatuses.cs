namespace EltradeProtocol.Requests
{
    public class GetStatuses : EltradeFiscalDeviceRequestPackage
    {
        public GetStatuses(string option) : base(0x4a, option) { }
    }
}
