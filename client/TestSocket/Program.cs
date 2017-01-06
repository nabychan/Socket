using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));
            }
            catch
            {
                Console.WriteLine("无法链接到服务器");
                return;
            }

            new TaskFactory().StartNew(() =>
            {
                var bufferd = Encoding.UTF8.GetBytes("连接");
                socket.Send(bufferd);
                while (true)
                {
                    try
                    {
                        var message = Console.ReadLine();
                        if (!String.IsNullOrWhiteSpace(message))
                        {
                            var buffer = Encoding.UTF8.GetBytes(message);
                            socket.Send(buffer);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("出现异常，断开连接");
                        break;
                    }
                }
            });

            new TaskFactory().StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var messageBuffer = new byte[1024];
                        var count = socket.Receive(messageBuffer);
                        if (count <= 0)
                        {
                            Console.WriteLine("服务器断开连接");
                            break;
                        }
                        var message = Encoding.UTF8.GetString(messageBuffer, 0, count);
                        Console.WriteLine(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("出现异常，断开连接");
                        break;
                    }
                }
            });
            while (true)
                Thread.Sleep(Int32.MaxValue);
        }
    }
}
