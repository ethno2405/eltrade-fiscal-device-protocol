using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using EltradeProtocol.Requests;

namespace EltradeProtocol
{
    public class EltradeFiscalDeviceDriver : IDisposable
    {
        private SerialPort serialPort;
        private int attempts = 0;
        private System.Timers.Timer readTimer = new System.Timers.Timer();
        private bool reading;
        private Thread readThread;
        private EltradeFiscalDeviceResponsePackage response;

        public EltradeFiscalDeviceDriver()
        {
            FindFiscalDevicePort();

            readTimer.Interval = serialPort.ReadTimeout;
            readTimer.Elapsed += Timer_Elapsed;
            readTimer.AutoReset = true;
        }

        public static bool Ping()
        {
            EltradeFiscalDeviceDriver driver = null;
            try
            {
                driver = new EltradeFiscalDeviceDriver();
                var response = driver.Send(new GetStatuses("W"));
                return response.Package.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                driver?.Dispose();
            }
        }

        public EltradeFiscalDeviceResponsePackage Send(EltradeFiscalDeviceRequestPackage package)
        {
            if (ReferenceEquals(null, package)) throw new ArgumentNullException(nameof(package));

            readThread = new Thread(Read);
            OpenPort();
            response = EltradeFiscalDeviceResponsePackage.Empty;
            var bytes = package.Build(true);
            readThread.Start();
            reading = true;

            serialPort.Write(bytes, 0, bytes.Length);

            readTimer.Start();
            readThread.Join();

            return response;
        }

        private void FindFiscalDevicePort()
        {
            var bytes = new GetStatuses("W").Build();

            foreach (var portName in SerialPort.GetPortNames())
            {
                serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);

                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;
                serialPort.Encoding = Encoding.ASCII;
                serialPort.ErrorReceived += SerialPort_ErrorReceived;

                OpenPort();
                try
                {
                    serialPort.Write(bytes, 0, bytes.Length);
                    Thread.Sleep(100);
                    var response = serialPort.ReadExisting();

                    if (string.IsNullOrEmpty(response) == false)
                        return;
                }
                catch (IOException) { }
                finally
                {
                    serialPort?.Dispose();
                }
            }

            throw new InvalidOperationException("Unable to connect to fiscal device.");
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
                        if (attempts >= 10)
                            throw;

                        attempts++;
                        Thread.Sleep(1000 * attempts);
                    }
                }
            }
        }

        private void ClosePort()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                }
                catch (IOException) { }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            readTimer.Stop();
            ClosePort();
        }

        private void Read()
        {
            var buffer = new byte[serialPort.ReadBufferSize];
            while (reading)
            {
                try
                {
                    buffer = new byte[serialPort.ReadBufferSize];
                    var readBytes = serialPort.Read(buffer, 0, serialPort.ReadBufferSize);
                    response = new EltradeFiscalDeviceResponsePackage(buffer.Take(readBytes).ToArray());
                    if (response.Printing)
                    {
                        if (readTimer.Interval <= 10000)
                            readTimer.Interval += 60;

                        continue;
                    }
                }
                catch (TimeoutException) { }
                catch (IOException)
                {
                    reading = false;
                }
                finally
                {
                    if (response.Printing == false)
                    {
                        ClosePort();
                        reading = false;
                    }
                }
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ClosePort();
        }

        public void Dispose()
        {
            ClosePort();
            serialPort?.Dispose();
            serialPort = null;
            readTimer?.Dispose();
            readTimer = null;
            readThread?.Abort();
            readThread = null;
        }
    }
}
