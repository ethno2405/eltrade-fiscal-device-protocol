using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EltradeProtocol
{
    public class EltradeFiscalDeviceResponsePackage
    {
        public const byte SYN = 0x16;
        public const byte NAK = 0x15;
        public const byte Preamble = 0x1;
        public const byte Delimiter = 0x4;
        public const byte Postamble = 0x5;
        public const byte Terminator = 0x3;

        public static readonly EltradeFiscalDeviceResponsePackage Empty = new EltradeFiscalDeviceResponsePackage(new byte[] { });

        public EltradeFiscalDeviceResponsePackage(byte[] package)
        {
            if (ReferenceEquals(null, package)) throw new ArgumentNullException(nameof(package));

            Data = new byte[0];
            Status = new byte[0];
            Package = package;
            ParsePackage();
        }

        public bool InvalidRequest { get; private set; }
        public bool MalformedResponse { get; private set; }
        public bool Acknowledged { get; private set; }
        public bool Printing { get; private set; }

        public byte[] Package { get; }
        public byte Seq { get; private set; }
        public byte Command { get; private set; }
        public byte[] Data { get; private set; }
        public byte[] Status { get; private set; }
        public byte Length { get; private set; }

        public string GetHumanReadableData()
        {
            return GetHumanReadableData(Encoding.GetEncoding("windows-1251"));
        }

        public string GetHumanReadableData(Encoding encoding)
        {
            return encoding.GetString(Data);
        }

        private void ParsePackage() // <01><LEN><SEQ><CMD><DATA><04><STATUS><05><BCC><03>
        {
            if (ReferenceEquals(null, Package) || Package.Any() == false)
            {
                InvalidRequest = true;
                return;
            }

            if (Package.All(x => x == SYN))
            {
                Printing = true;
                return;
            }


            if (Package.All(x => x == NAK))
            {
                InvalidRequest = true;
                return;
            }

            if (Package.All(x => x == 1))
            {
                Acknowledged = true;
                return;
            }

            var payload = Package.SkipWhile(x => x == SYN || x == NAK);
            if (payload.First() != Preamble || payload.Last() != Terminator || payload.Contains(Delimiter) == false || payload.Contains(Postamble) == false)
            {
                MalformedResponse = true;
                return;
            }

            var queue = new Queue<byte>(payload);
            queue.Dequeue(); // preamble
            Length = queue.Dequeue();
            Seq = queue.Dequeue();
            Command = queue.Dequeue();
            Data = ReadTo(Delimiter, queue);
            Status = ReadTo(Postamble, queue);
        }

        private byte[] ReadTo(byte to, Queue<byte> queue)
        {
            var next = queue.Dequeue();
            var result = new List<byte>();

            while (next != to && queue.Count != 0)
            {
                result.Add(next);
                next = queue.Dequeue();
            }

            return result.ToArray();
        }
    }
}
