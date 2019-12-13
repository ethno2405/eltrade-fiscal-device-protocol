using System;

namespace EltradeProtocol.Requests
{
    public class RegisterArticle : EltradeFiscalDeviceRequestPackage
    {
        public RegisterArticle(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity = 1) : this(articleName, articleDescription, taxType, unitPrice, quantity, 0, DiscountType.Relative) { }

        public RegisterArticle(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity, decimal discount, DiscountType discountType) : base(0x31)
        {
            AppendData(Truncate(articleName, 30));
            AppendData(LineFeed);
            AppendData(Truncate(articleDescription, 30));
            AppendData(Tab);
            AppendData(taxType.ToString());
            AppendData(unitPrice.ToString());

            if (quantity != 1)
                AppendData($"*{quantity}");

            if (discount != 0)
            {
                if (discountType == DiscountType.Absolute)
                    AppendData($";{discount}");
                else if (discountType == DiscountType.Relative)
                    AppendData($",{discount}");
                else
                    throw new NotSupportedException($"Not supported discount type '{discountType}'");
            }
        }

        public enum DiscountType
        {
            Absolute,
            Relative
        }
    }
}
