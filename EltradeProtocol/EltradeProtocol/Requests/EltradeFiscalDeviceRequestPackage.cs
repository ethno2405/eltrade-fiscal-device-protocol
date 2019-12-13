using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EltradeProtocol.Requests
{
    public class EltradeFiscalDeviceRequestPackage
    {
        private Encoding windows1251 = Encoding.GetEncoding("windows-1251");
        private const byte Preamble = 0x1;
        private const byte Postamble = 0x5;
        private const byte Terminator = 0x3;
        private const byte StartSeq = 0x20;
        private const byte EndSeq = 0x7f;
        private const byte LengthOffset = 0x24;
        private const byte Escape = 0x10;
        private const byte EscapeOffset = 0x40;

        protected const byte LineFeed = 0x0a;
        protected const byte Tab = 0x09;

        static EltradeFiscalDeviceRequestPackage()
        {
            Seq = StartSeq;
        }

        public EltradeFiscalDeviceRequestPackage(byte command)
        {
            Command = command;
            Data = new byte[] { };
        }

        public EltradeFiscalDeviceRequestPackage(byte command, string data) : this(command)
        {
            if (string.IsNullOrEmpty(data) == false)
                AppendData(data);
        }

        public byte Command { get; }
        public byte[] Data { get; private set; }
        public static byte Seq { get; private set; }
        public bool HasData { get { return Data.Length > 0; } }
        public byte Length { get { return (byte)(Data.Length + LengthOffset); } }

        public byte[] Build()
        {
            return Build(false);
        }

        public byte[] Build(bool nextSeq)
        {
            if (nextSeq)
                NextSeq();

            var package = ComposePackage();
            return package;
        }

        protected void AppendData(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            Data = Data.Concat(windows1251.GetBytes(data)).ToArray();
        }

        protected void AppendData(byte value)
        {
            Data = Data.Concat(new byte[] { value }).ToArray();
        }

        protected string Truncate(string value, int length)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.Length <= length ? value : value.Substring(0, length);
        }

        protected byte[] EscapeData(byte[] data)
        {
            var escapedData = new List<byte>();
            foreach (var item in data)
            {
                if (item < 0x20)
                {
                    escapedData.Add(Escape);
                    escapedData.Add((byte)(item + EscapeOffset));
                }
                else
                    escapedData.Add(item);
            }

            return escapedData.ToArray();
        }

        private void NextSeq()
        {
            Seq++;

            if (Seq > EndSeq)
                Seq = StartSeq;
        }

        private byte[] ComposePackage() // <01><LEN><SEQ><CMD><DATA><05><BCC><03>
        {
            var package = new List<byte> { Preamble, Length, Seq, Command };

            if (HasData)
                package.AddRange(Data);

            package.Add(Postamble);

            var bcc = CalculateBlockCheckCharacter();
            package.AddRange(bcc);

            package.Add(Terminator);

            return package.ToArray();
        }

        private byte[] CalculateBlockCheckCharacter()
        {
            var sum = Length + Seq + Command + Data.Sum(x => x) + Postamble;

            var bcc = new byte[4];
            bcc[3] = (byte)(((sum & 0xf) >> 0) + 0x30);
            bcc[2] = (byte)(((sum & 0xf0) >> 4) + 0x30);
            bcc[1] = (byte)(((sum & 0xf00) >> 8) + 0x30);
            bcc[0] = (byte)(((sum & 0xf000) >> 12) + 0x30);

            return bcc;
        }
    }
}
