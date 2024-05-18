using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WinformThreadingFix
{
    class Client
    {
        // адрес и порт сервера, к которому будем подключаться
        const int port = 8005; // порт сервера
        const string address = "127.0.0.1"; // адрес сервера
        Socket socket;
        public delegate void ClientDelegate(string output);
        public event ClientDelegate ReceivedMessage;

        public Client()
        {

        }
        public bool Connect()
        {
            try
            {
                //создаем конечную точку
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                //создаем сокет
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);

                Thread thread = new Thread(() => ReceiveMessages());
                thread.IsBackground = true;
                thread.Start();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    byte[] data = new byte[256];
                    int bytes = 0;
                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    string response = builder.ToString();
                    ReceivedMessage(response);
                }
            }
            catch (Exception ex)
            {
                ReceivedMessage("Disconnected.");
            }
        }
        public bool SendMessage(string message)
        {
            try
            {
                byte[] data = new byte[256];
                data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);
                ReceivedMessage("You:" + message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
