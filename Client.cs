using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Socket2
{
    class Client
    {
        // адрес и порт сервера, к которому будем подключаться
        const int port = 8005; // порт сервера
        const string address = "127.0.0.1"; // адрес сервера
        int? code;
        public Client()
        {
            try
            {   
                //создаем конечную точку
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                //создаем сокет
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    Console.Write("Введите сообщение:");
                    string message = Console.ReadLine();

                    if (code.HasValue)
                    {
                        message = code + "|" + message;
                    }
                    byte[] data = Encoding.Unicode.GetBytes(message);

                    //посылаем сообщение
                    socket.Send(data);
                    // готовимся получить ответ
                    data = new byte[256]; // буфер для ответа
                    int bytes = 0; // количество полученных байт
                                   // получаем ответ
                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    string response = builder.ToString();
                    Console.WriteLine("ответ сервера: " + response);

                    // закрываем сокет
                    //socket.Shutdown(SocketShutdown.Both);
                    //socket.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
