using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Socket2
{
    internal class Server
    {
        const int port = 8005; // порт для приема входящих запросов
        public Server()
        {
            String Host = Dns.GetHostName();
            Console.WriteLine("Comp name = " + Host);
            IPAddress[] IPs;
            IPs = Dns.GetHostAddresses(Host);
            foreach (IPAddress ip1 in IPs)
                Console.WriteLine(ip1);


            //получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет сервера
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // связываем сокет с локальной точкой, по которой будем принимать данные
            listenSocket.Bind(ipPoint);
            
            // начинаем прослушивание
            listenSocket.Listen(10);
            
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            // сокет для связи с клиентом

            

            while (true)
            {
                
                Socket handler = listenSocket.Accept();
                Thread thread = new Thread(() => Process(handler));
                thread.Start();
                //Process(handler);
            }
        }
        void Process(Socket handler)
        {
            try
            {
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов за 1 раз
                    int kol_bytes = 0;//количество полученных байтов
                    byte[] data = new byte[255]; // буфер для получаемых данных
                    do
                    {
                        bytes = handler.Receive(data);  // получаем сообщение
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        kol_bytes += bytes;
                    }
                    while (handler.Available > 0);
            
                    string input = builder.ToString();
                    string response = input;
            
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + response);
                    Console.WriteLine(kol_bytes + "bytes\n");
                    // отправляем ответ клиенту, то, что получили от него
                    handler.Send(Encoding.Unicode.GetBytes(response));
            
                    // закрываем сокет
                    if (response == "exit")
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
