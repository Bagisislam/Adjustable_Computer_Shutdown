
using System.Diagnostics;
using System.Net.NetworkInformation;


string networkAdapterName = GetFirstNetworkAdapterName();

if (networkAdapterName == null)
{
    Console.WriteLine("No network adapter instance found for PerformanceCounter.");
    return;
}

Console.WriteLine($"Monitoring network usage for adapter: {networkAdapterName}");
await MonitorNetworkUsageAndShutdownAsync(networkAdapterName);

void GetAllNetwos()
{
    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
    {
        // Check if the adapter is up and has received data
        if (ni.OperationalStatus == OperationalStatus.Up && ni.Speed > 0)
        {
            Console.WriteLine($"Adapter Name: {ni.Name}");
            Console.WriteLine($"Description: {ni.Description}");
            Console.WriteLine($"Speed: {ni.Speed / 1_000_000} Mbps"); // Convert to Mbps
            Console.WriteLine($"ID: {ni.Id}");
            Console.WriteLine($"Type: {ni.NetworkInterfaceType}");
            Console.WriteLine();
        }
    }

}

static string GetFirstNetworkAdapterName()
{
    var category = new PerformanceCounterCategory("Network Interface");
    var instanceNames = category.GetInstanceNames();

    return instanceNames.Length > 0 ? instanceNames[0] : null;
}

static async Task MonitorNetworkUsageAndShutdownAsync(string networkAdapterName)
{
    PerformanceCounter bytesReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", networkAdapterName);
    PerformanceCounter bytesSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkAdapterName);

    while (true)
    {
        float bytesReceived = bytesReceivedCounter.NextValue();
        float bytesSent = bytesSentCounter.NextValue();

        float totalBytesPerSecond = bytesReceived + bytesSent;
        float speedKbps = (totalBytesPerSecond * 8) / 1024; // Convert to kbps

        Console.WriteLine($"Current Network Speed: {speedKbps} kbps");

        //if (speedKbps < 10)
        //{
        //    Console.WriteLine("Network usage is low. Shutting down...");
        //    ShutdownComputer();
        //    break;
        //}

        Console.WriteLine($"Network speedkbps = {speedKbps},TotoalBytsPerSecnd = {totalBytesPerSecond}");

        await Task.Delay(5000);
    }
}

static void ShutdownComputer()
{
    Process.Start("shutdown", "/s /f /t 0");
}
