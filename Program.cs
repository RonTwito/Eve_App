using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
//using Android.Support;
//using Android.Media;

namespace ServerConsole
{
    class Program
    {
        public static int countBack = 0;
        public static string ipAdress = "192.168.0.100";
        public static DataTable clientsDB = new DataTable("Clients");
        //public static string textbox_text;
        //public Server multiplate_server;
        const int port = 8888;
        int n = 0;
        public static TcpListener listener;
        //string text = " ";
        public static string FilePath;
        
        public static bool mainColorLoop = true;
        public static bool mainChatRoomLoop = true;
        public static ChatRoomHandler chatroomRecv = new ChatRoomHandler();
        public static TcpClient clientTcpChat;
        private static TcpClient client;

        static void Main(string[] args)
        {
            if (countBack == 0)
            {
                DataColumn col1 = new DataColumn();
                col1.ColumnName = "Name";
                col1.DataType = typeof(string);
                col1.Unique = true;
                clientsDB.Columns.Add(col1);
                DataColumn col2 = new DataColumn();
                col2.ColumnName = "IP Address";
                col2.DataType = typeof(string);
                clientsDB.Columns.Add(col2);
                FilePath = Environment.CurrentDirectory;
                FilePath = FilePath.Substring(0, FilePath.LastIndexOf("bin")) + "clientsDataBase.Xml";
                if (System.IO.File.Exists(FilePath))
                {
                    clientsDB.ReadXml(FilePath);
                }
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Any;
                listener = new TcpListener(IPAddress.Parse("192.168.43.124"), port);
                listener.Start();
                Console.WriteLine("Waiting for connection...");
                while (true)
                {
                    client = listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
            }
            // Server server_eve = new Server(textbox_text);       
            else
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            
        }

        public static void HandleClient(object arg)
        {
            string temp = "";
            Console.WriteLine("Client connected! ");
            // textBox.Text += "Client connected!\n";
            const int BufferLength = 100;
            TcpClient clientHandle = (TcpClient)arg;
            NetworkStream stream = clientHandle.GetStream();
            byte[] data = new byte[BufferLength];
            string messege = "";

            string connc = "You Are Connected";
            byte[] messegeByte1 = Encoding.ASCII.GetBytes(connc);
            stream.Write(messegeByte1, 0, messegeByte1.Length);
            Console.WriteLine("Messege sent to client...");

            int count = 0;

            while (clientHandle.Connected)
            {

                if (count == 0)
                {
                    byte[] GetFServer2 = new byte[clientHandle.ReceiveBufferSize];
                    int length2 = stream.Read(GetFServer2, 0, clientHandle.ReceiveBufferSize);
                    messege = Encoding.ASCII.GetString(GetFServer2, 0, length2);
                    int pos = messege.LastIndexOf('+');
                    int length = messege.Length;
                    string[] userIp = messege.Split('+');
                    if (userIp.Length >= 1)
                    {
                        string username = userIp[0];
                        string ipuser = userIp[1];
                        Console.WriteLine("Connection from " + ipuser + " with the username: " + username);
                        if (countBack==0)
                        {
                            DataRow row = clientsDB.NewRow();
                            row["Name"] = username;
                            row["IP Address"] = ipuser;
                            clientsDB.Rows.Add(row);
                            clientsDB.WriteXml(FilePath);
                        }
                    }

                }

                byte[] GetFServer1 = new byte[clientHandle.ReceiveBufferSize];
                int length1 = stream.Read(GetFServer1, 0, clientHandle.ReceiveBufferSize);
                messege = Encoding.ASCII.GetString(GetFServer1, 0, length1);

                Console.WriteLine("I got this");

                string[] lines = messege.Split('\n');
                string RequestType = lines[0];
                RequestType = messege;
                Console.WriteLine($"'{RequestType}'\n");

                string response = "Try Again";



                if (RequestType.Contains("RecColor"))
                {
                    Console.WriteLine("Color Recogniser for the client!\n");
                    response = "colors!";
                    byte[] messegeByte = Encoding.ASCII.GetBytes(response);
                    stream.Write(messegeByte, 0, messegeByte.Length);
                    Console.WriteLine("sent");

                    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.43.124"), 9050);
                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(ipep);
                    serverSocket.Listen(10);
                    Console.WriteLine("Waiting for an image");
                    Socket clientSocket = serverSocket.Accept();
                    IPEndPoint newclient = (IPEndPoint)clientSocket.RemoteEndPoint;
                    Console.WriteLine("Connected with {0} at port {1}", newclient.Address, newclient.Port);

                    while (mainColorLoop)
                    {
                        data = ReceiveVarData(clientSocket);
                        MemoryStream ms = new MemoryStream(data);
                        try
                        {
                            Image bmp = Image.FromStream(ms);
                            response = RecColor(bmp);
                            mainColorLoop = false;
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.ToString());
                            response = "problem...";
                            mainColorLoop = false;
                        }


                        if (data.Length == 0)
                            serverSocket.Listen(10);
                    }

                    Console.WriteLine(response);
                    byte[] messegeByteColor = Encoding.ASCII.GetBytes(response);
                    stream.Write(messegeByteColor, 0, messegeByteColor.Length);

                }
                if (RequestType.Contains("Help"))
                {
                    Console.WriteLine("Help screen for the client! \n");
                    response = "help!";
                    byte[] messegeByte = Encoding.ASCII.GetBytes(response);
                    stream.Write(messegeByte, 0, messegeByte.Length);
                    Console.WriteLine("sent");
                }

                if (RequestType.Contains("RecBill"))
                {
                    Console.WriteLine("About Screen for the client!\n");
                    response = "Bills!";
                    byte[] messegeByte = Encoding.ASCII.GetBytes(response);
                    stream.Write(messegeByte, 0, messegeByte.Length);
                    Console.WriteLine("sent");
                }

                if (RequestType.Contains("ChatRoom"))
                {
                    Console.WriteLine("Chat Room for the client!\n");
                    response = "Chat Room!";
                    byte[] messegeByte = Encoding.ASCII.GetBytes(response);
                    stream.Write(messegeByte, 0, messegeByte.Length);
                    Console.WriteLine("sent");

                    
                    Int32 port = 9060;
                    IPAddress localAddr = IPAddress.Any;
                    TcpListener listenerChatRoom = new TcpListener(IPAddress.Parse("192.168.43.124"), port);
                    listenerChatRoom.Start();
                    Console.WriteLine("Waiting for ChatRoom connection...");
                    while (true)
                    {
                        clientTcpChat = listenerChatRoom.AcceptTcpClient();
                        ThreadPool.QueueUserWorkItem(ChatHandler, clientTcpChat);
                    }

                }
                

                messege = string.Empty;
                count++;
            }
            client.Close();
        }

        public static string RecColor(Image imgFile)
        {
            ColorsNames clrNames = new ColorsNames();
            Color clr;
            bool main = false;
            using (Bitmap bmp = new Bitmap(imgFile))
            {
                clr = bmp.GetPixel(50, 100); // Get the color of pixel at position 50,100
                int red = clr.R;
                int green = clr.G;
                int blue = clr.B;
            }
            main = true;
            if (clr.IsKnownColor)
                return clr.Name;
            return clrNames.detectColor(clr.ToString());
        }

        private static byte[] ReceiveVarData(Socket s)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];

            recv = s.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, 0);
                if (recv == 0)
                {
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }

        public static void ChatHandler(object arg)
        {
            string messege = "", response = "";
            TcpClient clientTcpChatHandle = (TcpClient)arg;
            NetworkStream streamChat = clientTcpChatHandle.GetStream();

            while (clientTcpChatHandle.Connected)
            {
                byte[] GetFServerChatRoom = new byte[client.ReceiveBufferSize];
                int lengthch = streamChat.Read(GetFServerChatRoom, 0, clientTcpChatHandle.ReceiveBufferSize);
                messege = Encoding.ASCII.GetString(GetFServerChatRoom, 0, lengthch);
                Console.WriteLine(messege);

                byte[]dataForList= Encoding.ASCII.GetBytes(chatroomRecv.ToString());
                streamChat.Write(dataForList, 0, dataForList.Length);
                Console.WriteLine(chatroomRecv.ToString());

                GetFServerChatRoom = new byte[clientTcpChatHandle.ReceiveBufferSize];
                lengthch = streamChat.Read(GetFServerChatRoom, 0, clientTcpChatHandle.ReceiveBufferSize);
                messege = Encoding.ASCII.GetString(GetFServerChatRoom, 0, lengthch);
                Console.WriteLine(messege);

                
                if(messege.Contains("sent by"))
                {
                    //Console.WriteLine(messege);
                    string s = messege.Substring(messege.IndexOf("sent by") + 1);
                    string textmess = s.Substring(s.IndexOf(':') + 1);
                    string userChat = s.Remove(s.IndexOf(':'));
                    ClientHandlerForChat clientHandler = new ClientHandlerForChat(userChat, textmess);
                    chatroomRecv.addToList(clientHandler);
                    byte[] dataForList1 = Encoding.ASCII.GetBytes(chatroomRecv.ToString());
                    Console.WriteLine(chatroomRecv.ToString());
                    streamChat.Write(dataForList1, 0, dataForList1.Length);
                }

                if (messege.Contains("Refresh"))
                {
                    Console.WriteLine(messege);
                    byte[] dataForList1 = Encoding.ASCII.GetBytes(chatroomRecv.ToString());
                    streamChat.Write(dataForList1, 0, dataForList1.Length);
                    Console.WriteLine(chatroomRecv.ToString());
                }

                if (messege.Contains("Back"))
                {
                    response = "Go Back";
                    countBack++;
                    clientTcpChatHandle.Close();
                    string[] args = new string[1];
                    Main(args);
                }
            }
        }
    }
}
