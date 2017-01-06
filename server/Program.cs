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
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));
            socket.Listen(0);

            new TaskFactory().StartNew(() =>
            {
                while (true)
                {
                    var link = socket.Accept();
                    new TaskFactory().StartNew(() =>
                    {
                        Console.OutputEncoding = Encoding.UTF8;
                        Console.WriteLine("客户端连接");
                        while (true)
                        {
                            try
                            {
                                var messageBuffer = new byte[1024];
                                var count = link.Receive(messageBuffer);
                                if (count <= 0)
                                {
                                    Console.WriteLine("客户端断开连接");
                                    break;
                                }
                                var message = Encoding.UTF8.GetString(messageBuffer, 0, count);
                                Console.WriteLine(message);

                                link.Send(Encoding.UTF8.GetBytes("收到" + message));
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e);
                                Console.WriteLine("出现异常，断开连接");
                                break;
                            }
                        }
                    });
                }
            });
            while (true)
                Thread.Sleep(Int32.MaxValue);
        }
    }
}
