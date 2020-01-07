using System;
using System.Globalization;

namespace EltradeProtocol.Requests
{
    public class CalculateSubtotal : EltradeFiscalDeviceRequestPackage
    {
        public CalculateSubtotal() : base(0x33, "00")
        {

        }

        public CalculateSubtotal(decimal discount, DiscountType discountType) : base(0x33, "00")
        {
            if (discount != 0)
            {
                if (discountType == DiscountType.Absolute)
                    AppendData($";{Math.Round(discount, 2).ToString(CultureInfo.InvariantCulture)}");
                else if (discountType == DiscountType.Relative)
                    AppendData($",{Math.Round(discount, 2).ToString(CultureInfo.InvariantCulture)}");
                else
                    throw new NotSupportedException($"Not supported discount type '{discountType}'");
            }
        }
    }
}
