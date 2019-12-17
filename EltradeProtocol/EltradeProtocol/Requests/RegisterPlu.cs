using System;

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
            AppendData($"*{quantity}");
        }

        public RegisterPlu(int plu, decimal quantity, decimal discount, DiscountType discountType) : base(0x3a)
        {
            AppendData(plu.ToString());
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

        public RegisterPlu(int plu, int department, decimal quantity, decimal discount, DiscountType discountType) : base(0x3a)
        {
            AppendData(plu.ToString());
            AppendData(Tab);
            AppendData(department.ToString());
            AppendData(Tab);
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
    }
}
