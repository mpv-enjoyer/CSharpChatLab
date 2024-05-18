using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Socket2
{
    internal class Server
    {
        const int port = 8005; // порт для приема входящих запросов
        int? CurrentClient;
        static int GenerateClientCode()
        {
            Random rnd = new Random();
            return rnd.Next();
        }
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
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();  // сокет для связи с     клиентом
                    // готовимся  получать  сообщение
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
                    string[] splitted = input.Split('|');
                    int code;

                    string response = "";


                    if (splitted.Length > 0 && int.TryParse(splitted[0], out code) && code == CurrentClient)
                    {
                        if (splitted.Length > 1 && splitted[1].ToLower() == "exit")
                        {
                            response = "(exit)|" + input.Substring(splitted[0].Length + 1);
                            CurrentClient = null;
                        }
                        else
                        {
                            response = "(good)|" + input.Substring(splitted[0].Length + 1);
                        }
                    }
                    else
                    {
                        if (CurrentClient.HasValue)
                        {
                            response = "(busy)";
                        }
                        else
                        {
                            CurrentClient = GenerateClientCode();
                            response = CurrentClient.ToString();
                        }
                    }

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                    Console.WriteLine(kol_bytes + "bytes\n");
                    // отправляем ответ клиенту, то, что получили от него
                    handler.Send(Encoding.Unicode.GetBytes(response));

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
