using System;
using System.Collections.Generic;

namespace EltradeProtocol.Requests
{
    public class RegisterArticle : EltradeFiscalDeviceRequestPackage
    {
        public RegisterArticle(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity = 1) : this(articleName, articleDescription, taxType, unitPrice, quantity, 0, DiscountType.Relative) { }

        public RegisterArticle(string articleName, string articleDescription, char taxType, decimal unitPrice, decimal quantity, decimal discount, DiscountType discountType) : base(0x31)
        {
            var package = new List<byte>();
            package.AddRange(EscapeData(Windows1251.GetBytes(Truncate(articleName, 30))));
            package.Add(0x0a);
            package.AddRange(EscapeData(Windows1251.GetBytes(Truncate(articleDescription, 30))));
            package.Add(0x09);
            package.AddRange(Windows1251.GetBytes(taxType.ToString()));
            package.AddRange(Windows1251.GetBytes(unitPrice.ToString()));
            if (quantity != 1)
                package.AddRange(Windows1251.GetBytes($"*{quantity}"));

            if (discount != 0)
            {
                if (discountType == DiscountType.Absolute)
                    package.AddRange(Windows1251.GetBytes($";{discount}"));
                else if (discountType == DiscountType.Relative)
                    package.AddRange(Windows1251.GetBytes($",{discount}"));
                else
                    throw new NotSupportedException($"Not supported discount type '{discountType}'");
            }

            Data = package.ToArray();
        }
    }
}
