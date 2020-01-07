using System;
using System.Globalization;

namespace EltradeProtocol.Requests
{
    public class RegisterPlu : EltradeFiscalDeviceRequestPackage
    {
        public RegisterPlu(int plu) : base(0x3a)
        {
            AppendData(plu.ToString());
        }

        public RegisterPlu(int plu, decimal quantity) : base(0x3a)
        {
            AppendData(plu.ToString());
            AppendData($"*{Math.Round(quantity, 2).ToString(CultureInfo.InvariantCulture)}");
        }

        public RegisterPlu(int plu, decimal quantity, decimal discount, DiscountType discountType) : base(0x3a)
        {
            AppendData(plu.ToString());
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

        public RegisterPlu(int plu, int department, decimal quantity, decimal discount, DiscountType discountType) : base(0x3a)
        {
            AppendData(plu.ToString());
            AppendData(Tab);
            AppendData(department.ToString());
            AppendData(Tab);
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
