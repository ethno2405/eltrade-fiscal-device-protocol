using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace EltradeProtocol
{
    public class EltradeFiscalDeviceDriver
    {
        private const byte SYN = 0x16;
        private SerialPort serialPort;
        private int attempts = 0;
        private System.Timers.Timer readTimer = new System.Timers.Timer();

        public EltradeFiscalDeviceDriver(string portName)
        {
            if (string.IsNullOrEmpty(portName)) throw new ArgumentNullException(nameof(portName));

            serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);

            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
            serialPort.Encoding = Encoding.ASCII;
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.ErrorReceived += SerialPort_ErrorReceived;

            readTimer.Interval = serialPort.ReadTimeout;
            readTimer.Elapsed += Timer_Elapsed;
            readTimer.AutoReset = true;
        }

        public event EventHandler<EltradeOnReadEventArgs> OnRead;

        public void Send(EltradeFiscalDeviceRequestPackage package)
        {
            if (ReferenceEquals(null, package)) throw new ArgumentNullException(nameof(package));

            OpenPort();
            var bytes = package.Build();
            serialPort.Write(bytes, 0, bytes.Length);
            readTimer.Start();
        }

        private void OpenPort()
        {
            if (serialPort.IsOpen == false)
            {
                while (attempts < 5)
                {
                    try
                    {
                        serialPort.Open();
                        attempts = 0;
                        break;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (attempts >= 5)
                            throw;

                        attempts++;
                        Thread.Sleep(100 * attempts);
                    }
                }
            }
        }

        private void ClosePort()
        {
            if (serialPort.IsOpen)
            {
                Console.WriteLine("Closing port!");
                serialPort.Close();
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            readTimer.Stop();
            ClosePort();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var buffer = new byte[serialPort.ReadBufferSize];

            try
            {
                var readBytes = serialPort.Read(buffer, 0, serialPort.ReadBufferSize);
                if (buffer[0] == SYN)
                {
                    readTimer.Interval += 60;
                    return;
                }

                var response = new EltradeFiscalDeviceResponsePackage(buffer.Take(readBytes).ToArray());
                OnRead(this, new EltradeOnReadEventArgs(response));
            }
            catch (TimeoutException) { }
            finally
            {
                if (buffer[0] != SYN)
                {
                    ClosePort();
                }
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ClosePort();
        }

        public class EltradeOnReadEventArgs : EventArgs
        {
            public EltradeOnReadEventArgs(EltradeFiscalDeviceResponsePackage response) : base()
            {
                if (ReferenceEquals(null, response)) throw new ArgumentNullException(nameof(response));

                Response = response;
            }

            public EltradeFiscalDeviceResponsePackage Response { get; }
        }
    }
}
