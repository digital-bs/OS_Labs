using System.Net;
using System.Net.Sockets;

Console.Write("Enter your name :");
string? userName = Console.ReadLine();

IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 12345);
using TcpClient client = new TcpClient();
StreamReader? Reader = null;
StreamWriter? Writer = null;
try
{
    client.Connect(localEndPoint);
    Console.WriteLine($"Now you can type your message and press Enter.");
    Reader = new StreamReader(client.GetStream());
    Writer = new StreamWriter(client.GetStream());
    if (Writer is null || Reader is null) return;
    Task.Run(() => ReceiveMessageAsync(Reader));
    await SendMessageAsync(Writer);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
Writer?.Close();
Reader?.Close();


async Task SendMessageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(userName);
    await writer.FlushAsync();

    while (true)
    {
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}

async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            string? message = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(message)) continue;
            Console.WriteLine(message);
        }
        catch
        {
            break;
        }
    }
}