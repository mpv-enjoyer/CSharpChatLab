using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace WinformThreadingFix
{
    class Listener
    {
        Socket socket;
        Thread thread;
        string name = "";
        int id;
        public delegate void MessageFromClient(string ClientName, string message, int id);
        public event MessageFromClient Notify;
        public Listener(Socket handler, int id)
        {
            this.id = id;
            socket = handler;
            thread = new Thread(Listen);
            thread.IsBackground = true;
        }
        public void Start() 
        { 
            thread.Start();
        }
        void Listen()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(2000);

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов за 1 раз
                    int kol_bytes = 0;//количество полученных байтов
                    byte[] data = new byte[255]; // буфер для получаемых данных
                    do
                    {
                        bytes = socket.Receive(data);  // получаем сообщение
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        kol_bytes += bytes;
                    }
                    while (socket.Available > 0);

                    string input = builder.ToString();
                    if (input.StartsWith("/name"))
                    {
                        name = input.Substring("/name ".Length);
                    }
                    Notify.Invoke(name, input, id);
                    string response = input;

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + response);
                    Console.WriteLine(kol_bytes + "bytes\n");
                    // отправляем ответ клиенту, то, что получили от него
                    

                    // закрываем сокет
                    if (response == "/exit")
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public bool Send(string Message)
        {
            try
            {
                socket.Send(Encoding.Unicode.GetBytes(Message));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    internal class Server
    {
        const int port = 8005; // порт для приема входящих запросов
        List<Listener> listeners = new List<Listener>();
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
                Listener listener = new Listener(handler, listeners.Count);
                listeners.Add(listener);
                listeners[listeners.Count - 1].Notify += Resend;
                listeners[listeners.Count - 1].Start();
            }
        }
        void Resend(string ClientName, string Message, int id)
        {
            for (int i = 0; i < listeners.Count; i++) 
            {
                if (id == i) continue;
                listeners[i].Send(ClientName + ": " + Message);
            }
        }
    }
}
