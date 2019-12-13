using System;

namespace EltradeProtocol.Requests
{
    public class RegisterArticle : EltradeFiscalDeviceRequestPackage
    {
        public RegisterArticle(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity = 1) : this(articleName, articleDescription, taxType, unitPrice, quantity, 0, DiscountType.Relative) { }

        public RegisterArticle(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity, decimal discount, DiscountType discountType) : base(0x31)
        {
            Append(Truncate(articleName, 30));
            Append(LineFeed);
            Append(Truncate(articleDescription, 30));
            Append(Tab);
            Append(taxType.ToString());
            Append(unitPrice.ToString());

            if (quantity != 1)
                Append($"*{quantity}");

            if (discount != 0)
            {
                if (discountType == DiscountType.Absolute)
                    Append($";{discount}");
                else if (discountType == DiscountType.Relative)
                    Append($",{discount}");
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
