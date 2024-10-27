using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

using System.Threading;
using System.Net;
using Android.Content;
using Android.Provider;
using Android.Graphics;
using Android.Media;
using System.Runtime.Serialization.Formatters.Binary;

using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Speech.Tts;
using Java.Util;

namespace Eve_Client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
   // [Android.Runtime.Register("android/media/MediaRecorder", DoNotGenerateAcw = true)]
    [assembly: Dependency(typeof(TextToSpeechImplementation))]
    [assembly: Application(Icon = "@drawable/icon")]

    public class MainActivity : AppCompatActivity, TextToSpeech.IOnInitListener
    {
        //"192.168.43.124"
        public TextToSpeech Speecher;
        public const string Current_IP = "192.168.43.124";
        ImageView imgView;
        public bool alreadyPressed = false;
        public static string FilePath;
        public static string user = "";
        public static TcpClient clientSocket;
        public static NetworkStream stream;
        public int count_back = 0;
        public string currentButton = "";
        public bool canI = false;
        protected MediaRecorder recorder;
        protected MediaPlayer player;
        public const int REQUEST_pERMISSION_CODE = 1000;
        public int sentMesg = 0;
        private bool isGrantedPermission = false;
        public bool recorded = false;
        private TcpClient clientChat;
        private NetworkStream serverChatStream;
        private string IPForChat;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_pERMISSION_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            Toast.MakeText(this, "Granted", ToastLength.Short).Show();
                            isGrantedPermission = true;
                        }
                        else
                        {
                            Toast.MakeText(this, "Granted", ToastLength.Short).Show();
                            isGrantedPermission = false;
                        }
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            txt = "Welcome, Enter your name and connect";
            Speecher = new TextToSpeech(this, this, "com.google.android.tts");            
            SpeekOut();
            if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage)!=Android.Content.PM.Permission.Granted && 
                CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] {
                   Manifest.Permission.WriteExternalStorage, Manifest.Permission.RecordAudio
                }, REQUEST_pERMISSION_CODE);
            }           
            else
            {
                isGrantedPermission = true;
            }

            //FilePath = System.Environment.CurrentDirectory;
            //FilePath = FilePath.Substring(0, FilePath.LastIndexOf("bin")) + "pic.png";
            if (FindViewById<EditText>(Resource.Id.UserNameText).Text.ToString() != "" || FindViewById<EditText>(Resource.Id.UserNameText).Text.ToString() != "Username")
            {
                user = FindViewById<EditText>(Resource.Id.UserNameText).Text.ToString();
                FindViewById<Button>(Resource.Id.ServerConnectButton).Click += (o, e) =>
              OpenNewWindow();
            }
        }

        private void OpenNewWindow()
        {
            user = FindViewById<EditText>(Resource.Id.UserNameText).Text.ToString();
            if (count_back == 0)
            {                
                txt = user + " You have been Connected succsesfully";
                SpeekOut();
            }
            string hostName = "";           
            SetContentView(Resource.Layout.MenuScreen);
            NetworkStream serverStream = null;
            if (count_back == 0)
            {
                clientSocket = new TcpClient();
                hostName = Dns.GetHostName();
                clientSocket.Connect(Current_IP, 13000);
                serverStream = clientSocket.GetStream();
                string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(user + "+" + IP);
                serverStream.Write(data, 0, data.Length);
            }
            else
            {
                serverStream = clientSocket.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(user + " Came Back To Menu");
                serverStream.Write(data, 0, data.Length);
            }
            Button btn = FindViewById<Button>(Resource.Id.ColorRecogniserButton);
            btn.SetBackgroundColor(Color.Aqua);
            btn = FindViewById<Button>(Resource.Id.ChatRoomButton);
            btn.SetBackgroundColor(Color.BlueViolet);
            btn = FindViewById<Button>(Resource.Id.BillRecogniserButton);
            btn.SetBackgroundColor(Color.BlueViolet);
            btn = FindViewById<Button>(Resource.Id.HelpButton);
            btn.SetBackgroundColor(Color.Aqua);
            //////btn= FindViewById<Button>(Resource.Id.btnCamera);
            //////btn.SetBackgroundColor(Color.Blue);
            //    /*
            //     *The user(the client) need to choose something in the menu, the request is sending to the server right after
            //     */
            int count = 0;
            //string request = "None";
            FindViewById<Button>(Resource.Id.ColorRecogniserButton).Click += (o, e) =>
              Action("RecColor", count);
            FindViewById<Button>(Resource.Id.BillRecogniserButton).Click += (o, e) =>
              Action("RecBill", count);
            FindViewById<Button>(Resource.Id.ChatRoomButton).Click += (o, e) =>
              Action("ChatRoom", count);
            FindViewById<Button>(Resource.Id.HelpButton).Click += (o, e) =>
              Action("Help", count);
        }

        private void Action(string request, int count)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(request);
            try
            {
                stream = clientSocket.GetStream();
                // MessageBox.Show("#3");
                //string user = 
                string dataRequest = request;
                byte[] data1 = System.Text.Encoding.ASCII.GetBytes(user + " wants to " + dataRequest);
                stream.Write(data1, 0, data1.Length);
                //  MessageBox.Show("#4");



                byte[] GetFServer = new byte[clientSocket.ReceiveBufferSize];
                // textBox1.Text+=GetFServer.ToString();
                int length = stream.Read(GetFServer, 0, clientSocket.ReceiveBufferSize);
                // MessageBox.Show("#5");
                string getD = System.Text.Encoding.ASCII.GetString(GetFServer, 0, length);
                //string temp = textBox1.Text + "\n" + getD;
                //this.textBox1.Text = temp;

                if (getD.Contains("colors!"))
                    CameraScreen();
                if (getD.Contains("Bills!"))
                    AboutScreen();
                if (getD.Contains("help!"))
                    HelpScreen();
                if(getD.Contains("Chat Room!"))
                {
                    ConnectWindow();            
                }
                    
                count++;
            }
            catch (Exception ex)
            {
                string t = ex.ToString();
            }
        }

        private void ConnectWindow()
        {
            txt = "Connect to the chat room";
            SpeekOut();
            SetContentView(Resource.Layout.ConnectToChatRoom);
            FindViewById<Button>(Resource.Id.ConnectBtnChat).Click += (o, e) => ConnectRoom();
        }

        private void ConnectRoom()
        {
            txt = "Connected to the chat room";
            SpeekOut();
            clientChat = new TcpClient();
            string hostName = Dns.GetHostName();
            clientChat.Connect(Current_IP, 9060);
            serverChatStream = clientChat.GetStream();
            IPForChat = Dns.GetHostByName(hostName).AddressList[0].ToString();
            FullChatRoomScreen();
        }

        private void FullChatRoomScreen()
        {
            SetContentView(Resource.Layout.ChatRoomScreen);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(user + "+" + IPForChat);
            serverChatStream.Write(data, 0, data.Length);

            string getD = "";
            byte[] GetFServer = new byte[clientChat.ReceiveBufferSize];
            int length = serverChatStream.Read(GetFServer, 0, clientChat.ReceiveBufferSize);
            getD = System.Text.Encoding.ASCII.GetString(GetFServer, 0, length);
            if(!getD.Contains("no messages."))
            {
                string[] arrForList = getD.Split('|');
                List<string> Content = new List<string>();
                Content = arrForList.ToList<string>();
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1,Content);
                FindViewById<ListView>(Resource.Id.listMess).Adapter = adapter;

            }
            FindViewById<Button>(Resource.Id.SendBtnChat).Click += delegate
            {
                txt = "Send Button";
                SpeekOut();
                if (FindViewById<EditText>(Resource.Id.InsertMessageText).Text.ToString() != "" || FindViewById<EditText>(Resource.Id.InsertMessageText).Text.ToString() != "Write Something...")
                {
                    byte[] data1 = System.Text.Encoding.ASCII.GetBytes(" ssent by "+user + ": "+ FindViewById<EditText>(Resource.Id.InsertMessageText).Text.ToString());
                    serverChatStream.Write(data1, 0, data1.Length);
                    txt = "Sent a messege "+ FindViewById<EditText>(Resource.Id.InsertMessageText).Text.ToString();
                    SpeekOut();
                    byte[] GetFServer1 = new byte[clientChat.ReceiveBufferSize];
                    int length1 = serverChatStream.Read(GetFServer1, 0, clientChat.ReceiveBufferSize);
                    getD = System.Text.Encoding.ASCII.GetString(GetFServer1, 0, length1);
                    if (!getD.Contains("no messages."))
                    {
                        string[] arrForList = getD.Split('|');
                        List<string> Content = new List<string>();
                        Content = arrForList.ToList<string>();
                        ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Content);
                        FindViewById<ListView>(Resource.Id.listMess).Adapter = adapter;
                    }
                }
            };
            FindViewById<Button>(Resource.Id.RefreshBtnChat).Click += delegate
            {
                txt = "Refresh Button";
                SpeekOut();
                byte[] data1 = System.Text.Encoding.ASCII.GetBytes(user + " wants to Refresh");
                serverChatStream.Write(data1, 0, data1.Length);

                byte[] GetFServer1 = new byte[clientChat.ReceiveBufferSize];
                int length1 = serverChatStream.Read(GetFServer1, 0, clientChat.ReceiveBufferSize);
                getD = System.Text.Encoding.ASCII.GetString(GetFServer1, 0, length1);
                if (!getD.Contains("no messages."))
                {
                    string[] arrForList = getD.Split('|');
                    List<string> Content = new List<string>();
                    Content = arrForList.ToList<string>();
                    ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Content);
                    FindViewById<ListView>(Resource.Id.listMess).Adapter = adapter;
                }
            };
            FindViewById<Button>(Resource.Id.backBtnChatRoom).Click += delegate
            {
                txt = "Back Button";
                SpeekOut();
                count_back++;
                byte[] dataBack = System.Text.Encoding.ASCII.GetBytes(user + " wants to go Back");
                serverChatStream.Write(dataBack, 0, dataBack.Length);
                serverChatStream.Close();
                MenuAfterBack();
            };

            FindViewById<ListView>(Resource.Id.listMess).ItemClick += (o,e)=>
            {
                txt = FindViewById<ListView>(Resource.Id.listMess).GetItemAtPosition(e.Position).ToString();
                SpeekOut();
            };

        }

        private void HelpScreen()
        {
            txt = "Help Screen";
            SpeekOut();
            SetContentView(Resource.Layout.Help);
            txt = FindViewById<TextView>(Resource.Id.HelpText).Text;
            SpeekOut();
        }

        private void AboutScreen()
        {
            txt = "About Screen";
            SpeekOut();
            SetContentView(Resource.Layout.About);
            txt = FindViewById<TextView>(Resource.Id.AboutText).Text;
            SpeekOut();
        }

        private void ChatRoomScreen()
        {
            txt = "Chat Room Screen";
            SpeekOut();
            SetContentView(Resource.Layout.ChatRooms);
            FindViewById<Button>(Resource.Id.RecordBtnChat).Click += (o, e) =>
              ActionChatRoom("RecordButton");           
            FindViewById<Button>(Resource.Id.backBtnChat).Click += (o, e) =>
              ActionChatRoom("BackButton");
            
        }
        
        public void ActionChatRoom(string request)
        {

            //stream = clientSocket.GetStream();
            //byte[] data = System.Text.Encoding.ASCII.GetBytes(user + " wants to " + request);
            //stream.Write(data, 0, data.Length);                        
            txt = request;
            SpeekOut();
            string getD = request;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(user + " wants to " + request);
            serverChatStream.Write(data, 0, data.Length);


            byte[] GetFServer = new byte[clientChat.ReceiveBufferSize];
            int length = serverChatStream.Read(GetFServer, 0, clientChat.ReceiveBufferSize);
            getD = System.Text.Encoding.ASCII.GetString(GetFServer, 0, length);

            if (getD.Contains("Record"))
            {
                byte[] started = System.Text.Encoding.ASCII.GetBytes(user + " started a Record...");
                serverChatStream.Write(started, 0, started.Length);
                SetContentView(Resource.Layout.StopRecord);
                FilePath = Directorypath;
                RecordAudio(FilePath);
                FindViewById<Button>(Resource.Id.StopBtnChat).Click += (o, e) =>
                    ActionChatRoom("StopButton");
                //byte[] GetDataRecords = new byte[clientSocket.ReceiveBufferSize];
                //// textBox1.Text+=GetFServer.ToString();
                //int lengthRecs = stream.Read(GetDataRecords, 0, clientSocket.ReceiveBufferSize);
                //// MessageBox.Show("#5");
                //string getDtRec = System.Text.Encoding.ASCII.GetString(GetDataRecords, 0, lengthRecs);

                //if (getDtRec.Contains("Record"))
                //{

                //}
            }

            if (getD.Contains("Stop"))
            {
                try
                {
                    
                    recorder.Stop();
                    //recorder.Release();
                    SetContentView(Resource.Layout.ChatRooms);
                    /*
                     * שליחת ההודעה הקולית
                     */
                    //stream = clientSocket.GetStream();
                    byte[] mp3Data;
                    mp3Data = ObjectToByteArray(recorder);

                    serverChatStream.Write(mp3Data, 0, mp3Data.Length);
                }
                catch(Exception ex)
                {
                    FindViewById<Button>(Resource.Id.StopBtnChat).Text = ex.ToString();
                }
            }
            if (getD.Contains("Back"))
            {
                count_back++;
                OpenNewWindow();
                byte[] dataBack = System.Text.Encoding.ASCII.GetBytes(user + " wants to " + request);
                serverChatStream.Write(dataBack, 0, dataBack.Length);
            }
        }

        

        private void BtnRecord_Click(object sender, EventArgs e)
        {
            currentButton = "Record";
            if (canI)
            {
                if (!recorded)
                {
                    FilePath = Directorypath;
                    FilePath = FilePath + "/Record.mp3";
                    RecordAudio(FilePath);
                }
                else
                {
                    recorder.Stop();
                    recorder.Release();
                    /*
                     * שליחת ההודעה הקולית
                     */
                }
            }
        }

        public static string Directorypath
        {
            get
            {
                return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/test.3gpp";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            currentButton = "Back";
            if (canI)
            {
                count_back++;
                OpenNewWindow();
            }
        }

        private void CameraScreen()
        {
            txt = "Color Recognizer Screen, Please take a picture";
            SpeekOut();
            SetContentView(Resource.Layout.Camera);
            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            imgView = FindViewById<ImageView>(Resource.Id.imgView);

            btnCamera.Click += BtnCamera_Click;

        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Bitmap bitmap = (Bitmap)data.Extras.Get("data");
            imgView.SetImageBitmap(bitmap);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(Current_IP), 9050);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ipep);
            }
            catch (SocketException e)
            {
                string s = e.ToString();
            }
            int sent;

            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                bitmapData = stream.ToArray();
            }
            sent = SendVarData(server, bitmapData);
            server.Shutdown(SocketShutdown.Both);
            server.Close();

            stream = clientSocket.GetStream();
            byte[] GetFServer = new byte[clientSocket.ReceiveBufferSize];
            int length = stream.Read(GetFServer, 0, clientSocket.ReceiveBufferSize);
            string getD = System.Text.Encoding.ASCII.GetString(GetFServer, 0, length);
            TextView txtColor = FindViewById<TextView>(Resource.Id.ColorRGBText);
            txtColor.Text = getD;
            txt = txtColor.Text;
            SpeekOut();
            FindViewById<Button>(Resource.Id.SpeekBtn).Click += delegate
            {
                txt = txtColor.Text;
                SpeekOut();               
            };
        }

        private void BtnCamera_Click(object sender, EventArgs e)
        {
            txt = "Open Camera";
            SpeekOut();
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        private int SendVarData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }

       
        public void RecordAudio(String filePath)
        {
            if (isGrantedPermission)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    recorder = new MediaRecorder(); // Initial state.

                    recorder.Reset();
                    recorder.SetAudioSource(AudioSource.Mic);
                    recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                    recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                    // Initialized state.
                    recorder.SetOutputFile(filePath);
                    // DataSourceConfigured state.
                    recorder.Prepare(); // Prepared state
                    recorder.Start(); // Recording state.   

                }
                catch (Exception ex)
                {
                    string exst = ex.ToString();
                }
                recorded = true;
            }
        }

        public byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                BinaryFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }
        public string txt = "default";

        public void OnInit([GeneratedEnum] OperationResult status)
        {
            
            if (status == OperationResult.Success)
            {
                Speecher.SetLanguage(Locale.Us);
                SpeekOut();
            }
            if (status == OperationResult.Error)
                Speecher.SetLanguage(Java.Util.Locale.Default);
        }

        private void SpeekOut()
        {
            try
            {
                //Speecher = new TextToSpeech(this, this, "com.google.android.tts");
                if (!String.IsNullOrEmpty(txt))
                {
                    Speecher.Speak(txt, QueueMode.Flush,null);
                }
            }
            catch(Exception ex)
            {
                FindViewById<Button>(Resource.Id.SpeekBtn).Text = ex.ToString();
                string s = ex.ToString();
            }
        }

        public void MenuAfterBack()
        {
            txt = user + "Came Back to the menu";
            SpeekOut();
            string hostName = " ";

            SetContentView(Resource.Layout.MenuScreen);

            NetworkStream serverStream = clientSocket.GetStream();
            string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(user + "+" + IP);
            serverStream.Write(data, 0, data.Length);

            Button btn = FindViewById<Button>(Resource.Id.ColorRecogniserButton);
            btn.SetBackgroundColor(Color.Aqua);
            btn = FindViewById<Button>(Resource.Id.ChatRoomButton);
            btn.SetBackgroundColor(Color.BlueViolet);
            btn = FindViewById<Button>(Resource.Id.BillRecogniserButton);
            btn.SetBackgroundColor(Color.BlueViolet);
            btn = FindViewById<Button>(Resource.Id.HelpButton);
            btn.SetBackgroundColor(Color.Aqua);

            int count = 0;
            //string request = "None";
            FindViewById<Button>(Resource.Id.ColorRecogniserButton).Click += (o, e) =>
              Action("RecColor", count);
            FindViewById<Button>(Resource.Id.BillRecogniserButton).Click += (o, e) =>
              Action("RecBill", count);
            FindViewById<Button>(Resource.Id.ChatRoomButton).Click += (o, e) =>
              Action("ChatRoom", count);
            FindViewById<Button>(Resource.Id.HelpButton).Click += (o, e) =>
              Action("Help", count);
        }
    }
}