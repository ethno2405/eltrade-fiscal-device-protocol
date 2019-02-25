using System;
using System.Linq;

namespace EltradeProtocol
{
    public static class Program
    {
        static EltradeFiscalDeviceDriver driver;

        static void Main(string[] args)
        {
            Console.WriteLine("Type 'q' to exit");

            while (true)
            {
                var message = Console.ReadLine();

                if ("q".Equals(message, StringComparison.OrdinalIgnoreCase))
                    break;

                var parts = message.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                var cmd = int.Parse(parts[0]);
                var data = string.Join(string.Empty, parts.Skip(1).ToArray());
                var package = new EltradeFiscalDeviceRequestPackage((byte)cmd, data);

                Console.WriteLine("Sending package...");
                driver = new EltradeFiscalDeviceDriver();

                var response = driver.Send(package);
                var responsePackage = string.Join(" ", response.Package.Select(x => x.ToString("x2")).ToArray());
                Console.WriteLine("Response package: " + responsePackage);
            }

            driver?.Dispose();
        }
    }
}
