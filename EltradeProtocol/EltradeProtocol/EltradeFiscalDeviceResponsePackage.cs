using System;

namespace EltradeProtocol
{
    public class EltradeFiscalDeviceResponsePackage
    {
        public static readonly EltradeFiscalDeviceResponsePackage Empty = new EltradeFiscalDeviceResponsePackage(new byte[] { });

        public EltradeFiscalDeviceResponsePackage(byte[] package)
        {
            if (ReferenceEquals(null, package)) throw new ArgumentNullException(nameof(package));

            Package = package;
        }

        public byte[] Package { get; }
    }
}
