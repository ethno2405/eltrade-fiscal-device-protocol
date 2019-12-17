using System;
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
        private bool reading;
        private Thread readThread;
        private EltradeFiscalDeviceResponsePackage response;
        private static string lastWorkingPort = string.Empty;

        public EltradeFiscalDeviceDriver()
        {
            FindFiscalDevicePort();
        }

        public EltradeFiscalDeviceResponsePackage Send(EltradeFiscalDeviceRequestPackage package)
        {
            if (ReferenceEquals(null, package)) throw new ArgumentNullException(nameof(package));

            response = EltradeFiscalDeviceResponsePackage.Empty;
            try
            {
                readThread = new Thread(Read);
                OpenPort();

                var bytes = package.Build(true);
                reading = true;

                serialPort.Write(bytes, 0, bytes.Length);

                readThread.Start();
                readThread.Join();
            }
            finally
            {
                ClosePort();
            }
            return response;
        }

        private void FindFiscalDevicePort()
        {
            byte[] bytes = new GetStatuses().Build();

            if (CheckPortConnectivity(lastWorkingPort, bytes))
            {
                return; // We have a response and we are happy
            }
            else
            {
                foreach (var portName in SerialPort.GetPortNames())
                {
                    if (CheckPortConnectivity(portName, bytes))
                        return;  // We have a response and we are happy
                }
            }

            throw new InvalidOperationException("Unable to connect to fiscal device.");
        }

        private bool CheckPortConnectivity(string portName, byte[] bytes)
        {
            if (string.IsNullOrEmpty(portName))
                return false;

            try
            {
                serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);

                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;
                serialPort.Encoding = Encoding.ASCII;
                serialPort.ErrorReceived += SerialPort_ErrorReceived;

                while (attempts < 10)
                {
                    try
                    {
                        OpenPort();

                        serialPort.Write(bytes, 0, bytes.Length);
                        Thread.Sleep(100);
                        var response = serialPort.ReadExisting();

                        if (string.IsNullOrEmpty(response) == false)
                        {
                            lastWorkingPort = portName;
                            return true;
                        }

                        attempts++;
                    }
                    catch (Exception ex)
                    {
                        if (attempts >= 10)
                            throw;

                        attempts++;
                        Thread.Sleep(1000 * attempts);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                attempts = 0;
                ClosePort();
            }

            return false;
        }

        private void OpenPort()
        {
            if (serialPort.IsOpen == false)
            {
                while (attempts < 10)
                {
                    try
                    {
                        serialPort.Open();
                        attempts = 0;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (attempts >= 10)
                            throw;

                        attempts++;
                        Thread.Sleep(1000 * attempts);
                    }
                }
            }

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        private void ClosePort()
        {
            if (serialPort is null)
                return;

            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                serialPort.Close();
            }
        }

        private void Read()
        {
            var buffer = new byte[serialPort.ReadBufferSize];
            while (reading)
            {
                // Stupidity
                try
                {
                    buffer = new byte[serialPort.ReadBufferSize];
                    var readBytes = serialPort.Read(buffer, 0, serialPort.ReadBufferSize);
                    response = new EltradeFiscalDeviceResponsePackage(buffer.Take(readBytes).ToArray());
                    if (response.Printing)
                    {
                        continue;
                    }
                    else
                    {
                        reading = false;
                    }
                }
                catch (TimeoutException) { }
                catch (Exception)
                {
                    reading = false;
                }
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ClosePort();
        }

        public void Dispose()
        {
            readThread?.Abort();
            readThread = null;

            ClosePort();
            if (serialPort is null == false)
            {
                serialPort.ErrorReceived -= SerialPort_ErrorReceived;

                serialPort.Dispose();
                serialPort = null;
            }
        }

        public static PingResult Ping()
        {
            using (EltradeFiscalDeviceDriver driver = new EltradeFiscalDeviceDriver())
            {
                try
                {
                    var response = driver.Send(new GetStatuses());
                    if (response.Package.Length == 0)
                        return new PingResult("Status: Response package length is 0");

                    var dateTimeResponse = driver.Send(new SetDateTime());
                    return dateTimeResponse.Package.Length > 0 ? new PingResult() : new PingResult($"SetCurrentDateTime: Response package length is {dateTimeResponse.Package.Length}");
                }
                catch (Exception ex)
                {
                    return new PingResult(ex);
                }
            }
        }
    }

    public class PingResult
    {
        public PingResult()
        {
        }

        public PingResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public PingResult(Exception exception)
        {
            Exception = exception;
        }

        public bool Success { get { return ReferenceEquals(null, Exception) && string.IsNullOrEmpty(ErrorMessage); } }
        public Exception Exception { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}
