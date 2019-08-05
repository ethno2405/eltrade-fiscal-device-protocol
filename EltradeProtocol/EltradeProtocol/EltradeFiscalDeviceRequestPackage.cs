using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EltradeProtocol
{
    public class EltradeFiscalDeviceRequestPackage
    {
        private static byte previousCommand;
        public const byte Preamble = 0x1;
        public const byte Postamble = 0x5;
        public const byte Terminator = 0x3;
        public static readonly EltradeFiscalDeviceRequestPackage Status = new EltradeFiscalDeviceRequestPackage(DeviceCommand.GetStatuses, "W");
        public static readonly EltradeFiscalDeviceRequestPackage SetCurrentDateTime = new EltradeFiscalDeviceRequestPackage(DeviceCommand.SetDateTime, DateTime.Now.ToString("dd-MM-y HH:mm:ss"));

        static EltradeFiscalDeviceRequestPackage()
        {
            Seq = 0x20;
        }

        public EltradeFiscalDeviceRequestPackage(byte command)
        {
            Command = command;
            Data = new byte[] { };
        }

        public EltradeFiscalDeviceRequestPackage(byte command, params byte[] data) : this(command)
        {
            if (data.Length == 0) throw new ArgumentNullException(nameof(data));

            Data = EscapeData(data);
        }

        public EltradeFiscalDeviceRequestPackage(byte command, string data) : this(command)
        {
            if (string.IsNullOrEmpty(data) == false)
            {
                var bytes = Encoding.ASCII.GetBytes(data);
                Data = EscapeData(bytes);
            }
        }

        public byte Command { get; }
        public byte[] Data { get; }
        public static byte Seq { get; private set; }
        public bool HasData { get { return Data.Length > 0; } }
        public byte Length { get { return (byte)(Data.Length + 0x24); } }

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

        private void NextSeq()
        {
            if (Command == previousCommand)
                return;

            previousCommand = Command;
            Seq++;

            if (Seq > 0x7f)
                Seq = 0x20;
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

        private byte[] EscapeData(byte[] data)
        {
            var escapedData = new List<byte>();
            foreach (var item in data)
            {
                if (item < 0x20)
                {
                    escapedData.Add(0x10);
                    escapedData.Add((byte)(item + 0x40));
                }
                else
                    escapedData.Add(item);
            }

            return escapedData.ToArray();
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
