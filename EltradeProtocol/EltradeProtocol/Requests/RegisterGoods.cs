using System;
using System.Globalization;

namespace EltradeProtocol.Requests
{
    public class RegisterGoods : EltradeFiscalDeviceRequestPackage
    {
        public RegisterGoods(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity = 1) : this(articleName, articleDescription, taxType, unitPrice, quantity, 0, DiscountType.Relative) { }

        public RegisterGoods(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity, decimal discount, DiscountType discountType) : base(0x31)
        {
            AppendData(Truncate(articleName, 30));
            AppendData(LineFeed);
            AppendData(Truncate(articleDescription, 30));
            AppendData(Tab);
            AppendData(taxType.ToString());
            AppendData(Math.Round(unitPrice, 2).ToString(CultureInfo.InvariantCulture));

            if (quantity != 1)
                AppendData($"*{Math.Round(quantity, 2).ToString(CultureInfo.InvariantCulture)}");

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
