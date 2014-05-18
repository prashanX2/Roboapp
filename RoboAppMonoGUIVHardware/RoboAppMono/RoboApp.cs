using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.Xml;
using MetroFramework.Forms;


namespace RoboAppMono
{
    public partial class RoboApp : MetroForm
    {
        public PictureBoxZoom _pictureBoxZoom;
        public Bitmap b1 = new Bitmap("bmp1.bmp");
        public Bitmap b2 = new Bitmap("bmp2.bmp");
        Thread drawThrd;
        public static int offset = 0;
        public static  bool started = false;
        private static RoboApp instance = new RoboApp();
        Socket _clientSocket;
        TcpClient client = new TcpClient();
        NetworkStream clientStream;
        String clientId = "1";
        public int anchorPointer = 0;
        public bool anchorRecived = false;
        public bool anchornext = false;
        List<int> anchorx = new List<int>();
        List<int> anchory = new List<int>();
        public bool connectedToServ = false;
        

        byte[] Posbuffer;
        public byte[] stopcom = {0x01,0xC3,0x02,0x00,0x00,0xFF,0xFF };
        public static int obsdir = 90;
        public static SerialPort emup = new SerialPort();
        public static SerialPort emup2 = new SerialPort();
        public static bool redir = false;
        public static bool refwrd = false;

        SPCom _comClass = new SPCom();

        RoboCtrl roboctrl ;
        Thread readthread;
        Thread readthread2;
        static Thread navthread;
        static Thread stpdtrd;
        Thread CentralControl;

        public static bool navalive = false;
        public static  int _destReached;
        public static int _TurnReached;
        public static string _genstring = "";
        public string aa;
        public static string xx;
        public static int sx1;
        public static int sx2;
        public static int ey1;
        public static int ey2;
        public static int stoppedx;
        public static int stoppedy;
        int startx1;
        int starty1;
        int endx1;
        int endy1;
        
        public static int currentx=0;
        public static int currenty = 0;
        public static int _obstacleF;
        public static int _obstacleFF;
        public static int _stopped;
        public int avoidx;
        public int avoidy;
        public static int rendx;
        public static int rendy;
        public bool ReNav = false;
        public static int test = 0;
        public static int cdir = 0;

      int index;

        public RoboApp()
        {
            InitializeComponent();
            try
            {
                pictureBox.Image = new Bitmap("bmp1.bmp");

                _pictureBoxZoom.Reset();
            }
            catch
            {
            }

            _pictureBoxZoom = new PictureBoxZoom(pictureBox);
            _pictureBoxZoom.OnZoomChange += UpdateZoomComboBox;
            UpdateZoomComboBox();
            Form.CheckForIllegalCrossThreadCalls = false;
        }
        private void UpdateZoomComboBox()
        {
            List<float> range = new List<float>(new float[] { 8, 4, 2, 1, .75f, .5f, .25f, .1f });

            if (!range.Contains(_pictureBoxZoom.ZoomFactor))
            {
                range.Add(_pictureBoxZoom.ZoomFactor);
            }

            range.Sort();
            range.Reverse();

            comboBoxZoom.FormattingEnabled = true;
            comboBoxZoom.FormatString = "##0%";
            comboBoxZoom.Items.Clear();
            comboBoxZoom.Items.AddRange(Array.ConvertAll<float, object>(range.ToArray(),
                new Converter<float, object>(element => element)));

            comboBoxZoom.SelectedItem = _pictureBoxZoom.ZoomFactor;
        }
        public void drawloop()
        {
            for (; ; )
            {
                SuperimposeImage(RoboApp.currentx, RoboApp.currenty);
                label1.Text = RoboApp.currentx.ToString() + "," + RoboApp.currentx.ToString();
                label2.Text = "Current Direction : " + RoboNav.currentdir.ToString();

                Thread.Sleep(10);
            }

        }


        void SuperimposeImage(int x, int y)
        {
            //load both images
            Bitmap nnb1 = new Bitmap(b1);
            Image mainImage = nnb1;
            Image imposeImage = b2;
            try
            {
                //create graphics from main image
                using (Graphics g = Graphics.FromImage(mainImage))
                {
                    //draw other image on top of main Image
                    g.DrawImage(imposeImage, new Point(x + 15, y + 15));
                    pictureBox.Image = mainImage;
                    //save new image
                    //mainImage.Save("bmp3.bmp");
                }
            }
            catch (Exception uu)
            {
                label2.Text = uu.ToString();
            }

        }
        public static RoboApp Instance
        {
            get { return instance; }
        }




        public RoboApp (SerialPort _serialPort, SerialPort _serialPort2)
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;
            emup = _serialPort;
            emup2 = _serialPort2;
            try
            {
                emup.Open();
                emup2.Open();
            }
            catch(Exception yy)
            {
                MessageBox.Show (yy.ToString() );
            }
        }


        public void initnet ()
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(textBox5.Text), 10000);
                client.Connect(serverEndPoint);
                clientStream = client.GetStream();
                SetText8("connected to Server");
                connectedToServ = true;
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
                
            }
            catch {
                SetText8("Error Connecting");
                connectedToServ = false;
            }
        }

        private void initWritesToServ ()
        {
            getPath();
            Thread posthrd = new Thread(updatePos);
            posthrd.Start();
        }

        

        private void getPath ()
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            try
            {
                string command = "GetPath";
                string currentXCod = textBox2.Text;
                string currentYCod = textBox3.Text;
                string destinXCod = textBox10.Text;
                string destinYCod = textBox11.Text;
                int speed = 20;
                //String currentPoss = "803";//803
                String destination = "destionation_03";//1637
                string streamEnd = "streamEnd";
                //clientId

                String clientToServerCommand = clientId + " " + command + " " + currentXCod + " " + currentYCod + " " + speed + " " + destinXCod + " " + destinYCod + " " + " " + streamEnd;

                byte[] buffer = encoder.GetBytes(clientToServerCommand);
                clientStream.Write(buffer, 0, buffer.Length);
                SetText8("Path Requested from Server\n");
                clientStream.Flush();
            }
            catch(Exception ia)
            {
                Console.WriteLine(ia.ToString());
            }
        }


        private void updatePos ()
        {
            
            ASCIIEncoding encoder = new ASCIIEncoding();
            while(true){
                try
                {
                    String command = "CurrentPossition";
                    //int xCod = Convert.ToInt32(textBox1.Text);
                    // int yCod = Convert.ToInt32(textBox2.Text);
                    int speed = 10;
                    String destination = "";
                    string streamEnd = "streamEnd";

                    String clientToServerCommand = clientId + " " + command + " " + currentx.ToString() + " " + currenty.ToString() + " " + speed + " " + "" + " " + "" + " " + " " + streamEnd;
                    Posbuffer = encoder.GetBytes(clientToServerCommand);
                    clientStream.Write(Posbuffer, 0, Posbuffer.Length);
                    clientStream.Flush();
                    Thread.Sleep(500);
                }
                catch(Exception ia) {
                    Console.WriteLine(ia.ToString());
                }
            }
        }

        private void HandleClientComm (object obj)
        {
            byte[] receivedMsg = new byte[4096];
            int bytesRead;

            while(true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(receivedMsg, 0, 4096);
                    String sev2cli = System.Text.Encoding.ASCII.GetString(receivedMsg);
                    MessageBox.Show(sev2cli);

                    //Invoke((MethodInvoker)delegate
                    //{
                    //    //MessageBox.Show(sev2cli);
                    //    listBox1.Items.Add("Received from Server===>");
                    //    listBox1.Items.Add(sev2cli);

                    //    /*ASCIIEncoding encoder = new ASCIIEncoding();
                    //    String cmd1 = "Command 01 From client to sever";
                    //    byte[] buffer = encoder.GetBytes(cmd1);
                    //    clientStream.Write(buffer, 0, buffer.Length);
                    //    clientStream.Flush();*/
                    //});


                    String[] commandArray = sev2cli.Split(' ');

                    if(commandArray[0] == "anchor_nodes_to_pass")
                    {
                        for(int i = 1; i < commandArray.Length - 1; i += 2)
                      {
                    //        //Thread sendCmdThread = new  Thread( new System.Threading.ParameterizedThreadStart(SendCommand(commandArray[i], commandArray[i + 1])));
                    //        Thread sendCmdThread = new Thread(() => SendCommand(commandArray[i], commandArray[i + 1]));
                    //        sendCmdThread.Start();
                    //        Thread.Sleep(5000);
                    //        //sleep 5000
                    //        //SendCommand(commandArray[i], commandArray[i + 1]);
                          anchorx.Add(Convert.ToInt32(commandArray[i]));
                          anchory.Add(Convert.ToInt32(commandArray[i+1]));
                       }
                        anchorRecived = true;
                        if(anchorx[anchorx.Count - 1] != Convert.ToInt32(textBox10.Text) || anchory[anchory.Count - 1] != Convert.ToInt32(textBox11.Text))
                        {
                            anchorx.Add(Convert.ToInt32(textBox10.Text));
                            anchory.Add(Convert.ToInt32(textBox11.Text));

                        }

                    }
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if(bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }
                Console.WriteLine("anchor points ");
                
                for(int i = 0; i < anchorx.Count;i++ )
                {
                    Console.WriteLine(anchorx[i]+","+anchory[i]);
                
                }
                Thread.Sleep(500);
                //message has successfully been received
                // ASCIIEncoding encoder = new ASCIIEncoding();
                // System.Diagnostics.Debug.WriteLine(encoder.GetString(receivedMsg, 0, bytesRead));
            }
        }




        Socket socket ()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public static void portstart()
        {
            try
            {
                emup.Open();
                emup2.Open();
            }
            catch (Exception yy)
            {
                //MessageBox.Show (yy.ToString() );
            }

        }

        public  static int DestReached
        {
            get { return _destReached; }
            set
            {
                _destReached = value;
                //MessageBox.Show("dest");

                if (_destReached == 1)
                {
                    
                    try
                    {
                        navthread.Resume();
                        //MessageBox.Show("dest");
                      //  MessageBox.Show(navthread.ThreadState.ToString());
                        _destReached = 0;
                    }
                    catch (ThreadStateException t) {
                        Console.WriteLine(t.ToString());
                        _destReached = 0;
                    }
                    _destReached = 0;
                }
            }
        }


        public static int TurnReached
        {
            get { return _TurnReached; }
            set
            {
                _TurnReached = value;
                //MessageBox.Show("dest");

                if(_TurnReached == 1)
                {

                    try
                    {
                        navthread.Resume();
                        //MessageBox.Show("dest");
                        //  MessageBox.Show(navthread.ThreadState.ToString());
                        _TurnReached = 0;
                    }
                    catch(ThreadStateException t)
                    {
                        Console.WriteLine(t.ToString());
                        _TurnReached = 0;
                    }
                    _TurnReached = 0;
                }
            }
        }

        public static int ObstacleF
        {
            get { return _obstacleF; }
            set
            {
                _obstacleF = value;
                _obstacleFF = _obstacleF;
                //MessageBox.Show("dest");

                if(_obstacleF == 1)
                {

                    try
                    {
                        if(started == true)
                        {
                            Instance.singtest();
                            // MessageBox.Show("obs");

                            //  MessageBox.Show(navthread.ThreadState.ToString());
                            _obstacleF = 0;
                        }
                    }
                    catch(ThreadStateException t)
                    {
                        Console.WriteLine(t.ToString());
                        _obstacleF = 0;
                    }
                    _obstacleF = 0;
                }
            }
        }


        public static int Stopped
        {
            get { return _stopped; }
            set
            {
                _stopped = value;
                
                //MessageBox.Show("dest");

                if(_stopped == 1)
                {
                    Thread.Sleep(3000);
                    try
                    {
                        
                        if(SPCom.middleobs==true)
                        {
                        Instance.roboStopped();
                        // MessageBox.Show("obs");

                        //  MessageBox.Show(navthread.ThreadState.ToString());
                        _stopped = 0;
                        }
                    }
                    catch(ThreadStateException t)
                    {
                        Console.WriteLine(t.ToString());
                        _stopped = 0;
                    }
                    _stopped = 0;
                }
            }
        }

        public void singtest () {
            //MessageBox.Show("singtest");
            Console.WriteLine("obsticle avoidance initiated");

            SetText9("obsticle avoidance initiated");
            try
            {
                navthread.Resume();
                navthread.Abort();

                try
                {
                    stpdtrd.Abort();
                }
                catch
                {
                    Console.WriteLine("cudnt abort stop thrd");
                }


                Console.WriteLine(" nav thread killed");
                
                SetText4(_comClass.writetest(stopcom));
                Console.WriteLine("stop command sent");
            }
            catch(Exception uty)
            {
                Console.WriteLine("failed to stop navthread");
                try
                {
                    stpdtrd.Abort();
                }
                catch
                {
                    Console.WriteLine("cudnt abort stop thrd");
                }
                SetText4(_comClass.writetest(stopcom));
                Console.WriteLine("stop command sent");

            }
            
        }


        public void roboStopped () {

            byte[] avoidancedir = BitConverter.GetBytes(RoboNav.currentdir+obsdir);
            byte[] atemp = new byte[9];
            byte[] ach = BitConverter.GetBytes(60);

           // atemp[0] = 0x01;
           // atemp[1] = 0xC1;
           // atemp[2] = 0x04;
           // atemp[3] = 0x11;
           // atemp[4] = ach[1];
           // atemp[5] = ach[0];
           // atemp[6] = avoidancedir[1];
           // atemp[7] = avoidancedir[0];
           // atemp[8] = 0xFF;
          
           //SetText4( _comClass.writetest(atemp));
            calcObstaclecord(RoboApp.stoppedx, RoboApp.stoppedy, RoboNav.currentdir + obsdir, 60);
            Console.WriteLine("Turn heading = " + (RoboNav.currentdir + obsdir));
           
            

                cdir = obsdir;
            
            stpdtrd = new Thread(stoppedthread);
            stpdtrd.Start();
            


        }

        public void stoppedthread ()
        {
            Console.WriteLine("stp x "+stoppedx+","+"stped y "+stoppedy+"   "+"avoidx "+avoidx+", avoid y"+ avoidy);
            SPCom.obsturn = true;
            while(SPCom.tcmdRecived == false)
            {
                SetText4(_comClass.writetest(RoboNav.makecomtestDir2(stoppedx, stoppedy, avoidx, avoidy,cdir)));
                Thread.Sleep(500);

            }
           
            redir = true;

        Console.WriteLine("stope trans for goforward");
        SPCom.tcmdRecived = false;
            while(SPCom.renavdir==false)
            {
                Thread.Sleep(10);
            }

            SPCom.renavdir = false;
        //MessageBox.Show("stope trans for goforward");

            
            while(SPCom.fcmdRecived == false )
            {
               
                SetText4(_comClass.writetest(RoboNav.makecomtest2(stoppedx, stoppedy, avoidx, avoidy)));
                
                Thread.Sleep(500);
            }
            //SPCom.obsturn = false;
            //SPCom.renavdir = false;
          SPCom.fcmdRecived = false;



          SPCom.renavi = true;
          while(SPCom.destReached==false) {
              Thread.Sleep(50);
          }
          SPCom.destReached = false;
          reNav();
          Console.WriteLine("renav started");
        }


        public  static string Genstring
        {
            get { return _genstring; }
            set
            {
                _genstring = value;
                //MessageBox.Show("dest");
                //textBox7.AppendText(value);
               
                MessageBox.Show(value.ToString());
                xx = value;
                
            }
        }
        

       
       


        delegate void SetTextCallback(string text);


        private void SetText4(string text)
        {

            if (this.textBox4.InvokeRequired)
            {

                SetTextCallback d = new SetTextCallback(SetText4);
                this.Invoke(d, new object[] { text });

            }
            else
            {
                this.textBox4.AppendText( text);
                
            }
        }

        private void SetText1(string text)
        {

            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText1);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text+"\n");
                if (text.Equals(""))
                {
                    this.textBox4.Text = text;

                }
            }
        }

        private void SetText9 (string text)
        {

            if(this.textBox9.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText9);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox9.AppendText(text + "\n");
                
            }
        }




        private void SetText6(string text)
        {

            if (this.textBox6.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText6);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox6.AppendText(text + "\n");
                if (text.Equals(""))
                {
                    this.textBox4.Text = text;
                    MessageBox.Show("text6");
                }
            }
        }



        private void SetText7(string text)
        {

            if(!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            if (this.textBox7.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText7);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                try { this.textBox7.AppendText(text + "\n");
                }
                catch { }
                
            }
        }

        private void SetText8 (string text)
        {

            if(this.textBox8.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText8);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox8.AppendText(text + "\n");
            }
        }



        public void navigate()
        {
           if(connectedToServ==false)
           { 
            if(ReNav == false)
            {
                double tmpx1 = (double)Convert.ToInt32(textBox2.Text); 
                double tmpy1 = (double)Convert.ToInt32(textBox3.Text);
                double tmpx2 = (double)Convert.ToInt32(textBox10.Text);
                double tmpy2 = (double)Convert.ToInt32(textBox11.Text);


                startx1 = (int)Math.Round((tmpx1 / Data.param[0]), 0) * Data.param[0];
                starty1 = (int)Math.Round((tmpy1 / Data.param[0]), 0) * Data.param[0];
                endx1 = (int)Math.Round((tmpx2 / Data.param[0]), 0) * Data.param[0];
                endy1 = (int)Math.Round((tmpy2 / Data.param[0]), 0) * Data.param[0];
                rendx = endx1;
               rendy = endy1;
            }

            else if(ReNav != false)
            {
                double tmpx1 = (double)avoidx;
                double tmpy1 = (double)avoidy;
                double tmpx2 = (double)rendx;
                double tmpy2 = (double)rendy;

                startx1 = (int)Math.Round((tmpx1 / Data.param[0]), 0) * Data.param[0];
                // startx1 = avoidx;
                starty1 = (int)Math.Round((tmpy1 / Data.param[0]), 0) * Data.param[0]; 
                endx1 = (int)Math.Round((tmpx2 / Data.param[0]), 0) * Data.param[0];
                endy1 = (int)Math.Round((tmpy2 / Data.param[0]), 0) * Data.param[0];
                 Console.WriteLine(startx1 + "  " + starty1 + " " + endx1+" "+endy1);
                 ReNav = false;
            }
            
            
           }
           else if(connectedToServ == true)
           {
               if(ReNav == false)
               {
                   double tmpx1 = (double)anchorx[anchorPointer];
                   double tmpy1 = (double)anchory[anchorPointer];
                   double tmpx2 = (double)anchorx[anchorPointer+1];
                   double tmpy2 = (double)anchory[anchorPointer+1];


                   startx1 = (int)Math.Round((tmpx1 / Data.param[0]), 0) * Data.param[0];
                   starty1 = (int)Math.Round((tmpy1 / Data.param[0]), 0) * Data.param[0];
                   endx1 = (int)Math.Round((tmpx2 / Data.param[0]), 0) * Data.param[0];
                   endy1 = (int)Math.Round((tmpy2 / Data.param[0]), 0) * Data.param[0];
                   rendx = endx1;
                   rendy = endy1;
                   Console.WriteLine(startx1 + "  " + starty1 + " " + endx1 + " " + endy1);
                   anchorPointer++;
               }

               else if(ReNav != false)
               {
                   double tmpx1 = (double)avoidx;
                   double tmpy1 = (double)avoidy;
                   double tmpx2 = (double)rendx;
                   double tmpy2 = (double)rendy;

                   startx1 = (int)Math.Round((tmpx1 / Data.param[0]), 0) * Data.param[0];
                   // startx1 = avoidx;
                   starty1 = (int)Math.Round((tmpy1 / Data.param[0]), 0) * Data.param[0];
                   endx1 = (int)Math.Round((tmpx2 / Data.param[0]), 0) * Data.param[0];
                   endy1 = (int)Math.Round((tmpy2 / Data.param[0]), 0) * Data.param[0];
                   Console.WriteLine(startx1 + "  " + starty1 + " " + endx1 + " " + endy1);
                   ReNav = false;
               }

           }
           // Console.Write("Enter start node :");
            //roboctrl.Nav.pathx.s =   Convert.ToInt32(textBox2.Text);
            roboctrl.Nav.pathx.s = (starty1 / Data.param[0]) + ((startx1 / Data.param[0] - 1) * Data.param[1]);

           // Console.Write("Enter destination node :");
            roboctrl.Nav.pathx.d = (endy1 / Data.param[0]) + ((endx1 / Data.param[0] - 1) * Data.param[1]);

            roboctrl.startpathcalc();
            foreach (string value in roboctrl.Nav.pathx.displays)
            {
                // textBox1.AppendText(value.ToString());
                SetText1(value.ToString());
            }
            foreach (string value in roboctrl.Nav.opdisplay)
            {
                // textBox1.AppendText(value.ToString());
                SetText6(value.ToString());
            }


            
            SetText7(xx);
            int ind = 0;
            for (int i = 0; i < roboctrl.Nav.xopdcords.Count; i++)
            {
                
                //roboctrl.goForward(i);
                while(SPCom.tcmdRecived == false)
                {
                    SetText4(_comClass.writetest(roboctrl.Nav.makecomtestDir(i)));
                    Thread.Sleep(500);

                }

                SPCom.tcmdRecived = false;
                navthread.Suspend();
                while(SPCom.fcmdRecived == false)
                {
                    SetText4(_comClass.writetest(roboctrl.Nav.makecomtest(i)));
                    Thread.Sleep(500);
                }
                SPCom.fcmdRecived = false;
                //while (roboctrl.Com.serialtty.destReached==false) {
                navthread.Suspend();
                if(i == roboctrl.Nav.xopdcords.Count - 1)
                {
                    Console.WriteLine(roboctrl.Nav.xopdcords.Count - 1);
                    anchornext = true;
                }
                //}
            }
            //MessageBox.Show("end");
           
            ////SetText1("");

            ////SetText6("");
           navthread.Abort();
           
        }



        private void rthread ()
        {

            while(true)
            {

                //textBox4.AppendText(roboctrl.Com.serialtty.ReadData());
                try
                {
                    SetText4(_comClass.ReadData());
                    Thread.Sleep(10);
                }
                catch(Exception read)
                {
                    SetText4("could not read from port" + read.ToString());

                }
                //textBox1.AppendText("test\n");

            }

        }

        private void rthread2 ()
        {

            while(true)
            {

                //textBox4.AppendText(roboctrl.Com.serialtty.ReadData());
                try
                {
                    SetText4(_comClass.ReadData2());
                    Thread.Sleep(10);
                }
                catch(Exception read) 
                {
                    SetText4("could not read from port"+read.ToString());
                
                }
                //textBox1.AppendText("test\n");

            }

        }

        public void startall()
        {

            navthread = new Thread(new ThreadStart(navigate));
            navthread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            //foreach (byte value in roboctrl.Com.serialtty.bBuffer) {

            //    textBox4.AppendText(value.ToString());
            //}
           

        }


        //private void rthread() { 
        
        //    while (true)
        //    {
                
        //        //textBox4.AppendText(roboctrl.Com.serialtty.ReadData());
        //        SetText4(roboctrl.Com.serialtty.ReadData());
        //        Thread.Sleep(10);
        //        //textBox1.AppendText("test\n");
                
        //    }

        //}

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        //public void navigategui()
        //{
        //    roboctrl = new RoboCtrl();
        //    roboctrl.Nav.pathx.s = Convert.ToInt32(textBox2.Text);
        //    roboctrl.Nav.pathx.d = Convert.ToInt32(textBox3.Text);

        //    roboctrl.startpathcalc();
        //    foreach (string value in roboctrl.Nav.pathx.displays)
        //    {
        //       // textBox1.AppendText(value.ToString());
        //        SetText1(value.ToString());
        //    }
        //    readthread = new Thread(new ThreadStart(rthread));
        //    readthread.Start();
        //    int ind = 0;
        //    for (int i = 0; i < roboctrl.Nav.pathx.xdcords.Count; i++)
        //    {

        //        //roboctrl.goForward(i);
        //        roboctrl.Com.serialtty.writetest(roboctrl.Nav.makecomtest(i));
        //        //while (roboctrl.Com.serialtty.destReached==false) {
        //        navthread.Suspend();
        //        //}
        //    }

            
        //}
        public void reNav ()
        {

            ReNav = true;
            
            roboctrl = new RoboCtrl();

            if(navalive == true)
            {
                try
                {
                    navthread.Resume();
                    navthread.Abort();
                    //SetText7("navthread running and killed");
                    Console.WriteLine("navthread running and killed in renav");
                }
                catch(Exception thr)
                {
                    //SetText7(thr.ToString());
                }
               



            }
            Console.WriteLine("renav running");
            navthread = new Thread(new ThreadStart(navigate));
            navalive = true;
            navthread.Start();







            // startall();
           
            //portstart();
        }

        public void mainthrd ()
        {
            currentx = Convert.ToInt32(textBox2.Text);
            currenty = Convert.ToInt32(textBox3.Text);

            if(connectedToServ==true)
            {

                
                initWritesToServ();

                while(true)
                {
                if(anchorRecived==true)
                {
                    anchorRecived = false;
                    break;
                }
                Thread.Sleep(500);
                }
            for(int i = 0; i < anchorx.Count-1;i++)
            {
               

                roboctrl = new RoboCtrl();
                //RoboNav.currentdir = Convert.ToInt32(textBox12.Text);
                if(navalive == true)
                {
                    try
                    {
                        navthread.Resume();
                        navthread.Abort();
                        SetText7("navthread running and killed");
                    }
                    catch(Exception thr)
                    {
                        SetText7(thr.ToString());
                    }




                }

                navthread = new Thread(new ThreadStart(navigate));
                navalive = true;








                // startall();
                navthread.Start();
                while(true)
                {
                    if(anchornext == true)
                    {
                        anchornext = false;
                        Console.WriteLine("in IF");
                        break;
                    }
                    Console.WriteLine("in while");
                    Thread.Sleep(500);
                }
                //portstart();
            }
            }
            else if(connectedToServ==false)
            {
                roboctrl = new RoboCtrl();
               // RoboApp.offset = Convert.ToInt32(textBox12.Text);
                if(navalive == true)
                {
                    try
                    {
                        navthread.Resume();
                        navthread.Abort();
                        SetText7("navthread running and killed");
                    }
                    catch(Exception thr)
                    {
                        SetText7(thr.ToString());
                    }




                }

                navthread = new Thread(new ThreadStart(navigate));
                navalive = true;








                // startall();
                navthread.Start();
            
            }


        
        
        }


        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                RoboApp.offset = Convert.ToInt32(textBox12.Text);
                Thread.Sleep(1000);
                Thread mainThread = new Thread(mainthrd);
                mainThread.Start();
                started = true;

                drawThrd = new Thread(drawloop);
                drawThrd.Start();

            }
            catch(Exception ii)
            {
                Console.WriteLine(ii.ToString());
            }
        }



        public void navstart()
        {
            try
            {
                RoboApp.offset = Convert.ToInt32(textBox12.Text);
                Thread.Sleep(1000);
                Thread mainThread = new Thread(mainthrd);
                mainThread.Start();
                started = true;

                drawThrd = new Thread(drawloop);
                drawThrd.Start();

            }
            catch (Exception ii)
            {
                Console.WriteLine(ii.ToString());
            }
        }






       

        private void RoboApp_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
           Data.readParam();
            textBox7.AppendText("parameter reading done\n");
            Data.readbmatrix();
            textBox7.AppendText("map matrix reading done\n");
            //cout<<"\nEnter the number of nodes(Less than 50)in the matrix : ";
            RoboPathCalc.n = Data.param[3];

            for (int i = 0; i < Data.param[3]; i++)
            {
                for(int j = 0; j < Data.param[3]; j++)
                {
                    Data.weight[i,j] = RoboPathCalc.INFINITY;
                }
            }

            Data.readAJMatrix();
            textBox7.AppendText("adjecency matrix reading done\n");
        }

        public void readd()
        {
            Data.readParam();
            textBox7.AppendText("parameter reading done\n");
            Data.readbmatrix();
            textBox7.AppendText("map matrix reading done\n");
            //cout<<"\nEnter the number of nodes(Less than 50)in the matrix : ";
            RoboPathCalc.n = Data.param[3];

            for (int i = 0; i < Data.param[3]; i++)
            {
                for (int j = 0; j < Data.param[3]; j++)
                {
                    Data.weight[i, j] = RoboPathCalc.INFINITY;
                }
            }

            Data.readAJMatrix();
            textBox7.AppendText("adjecency matrix reading done\n");
        }
        public void clearmem()
        {

            roboctrl.Nav.pathx.xdcords.Clear();
            roboctrl.Nav.pathx.ydcords.Clear();
            roboctrl.Nav.pathx.xscords.Clear();
            roboctrl.Nav.pathx.yscords.Clear();
            roboctrl.Nav.pathx.displays.Clear();


            Array.Clear(roboctrl.Nav.pathx.distancex, 0, roboctrl.Nav.pathx.distancex.Length);
            Array.Clear(roboctrl.Nav.pathx.visit, 0, roboctrl.Nav.pathx.visit.Length);
            Array.Clear(roboctrl.Nav.pathx.precede, 0, roboctrl.Nav.pathx.precede.Length);
            Array.Clear(roboctrl.Nav.pathx.path, 0, roboctrl.Nav.pathx.path.Length);
            roboctrl.Nav.pathx.smalldist=0;
            roboctrl.Nav.pathx.newdist=0;
            roboctrl.Nav.pathx.k=0;
            roboctrl.Nav.pathx.s=0;
            roboctrl.Nav.pathx.d=0;
            roboctrl.Nav.pathx.current = 0;
            roboctrl.Nav.pathx.distcurr=0;

            //roboctrl.Nav.pathx.readAJMatrix();
            SetText7("adjecsncy Reset done");
            roboctrl.Nav.pathx.parmcount = 0;
            //roboctrl.Nav.pathx.readParam();
            SetText7("param Reset done");

            roboctrl.Nav.opdisplay.Clear();
            roboctrl.Nav.xopdcords.Clear();
            roboctrl.Nav.yopdcords.Clear();
            roboctrl.Nav.xopscords.Clear();
            roboctrl.Nav.yopscords.Clear();

        
        
        }

        private void button1_Click_2 ( object sender , EventArgs e )
        {
            try
            {
                portstart();
                //CentralControl = new Thread(new ThreadStart(CentControl));
                //CentralControl.Start();
                initnet();
                
                readthread = new Thread(new ThreadStart(rthread));

                readthread.Start();
                readthread2 = new Thread(new ThreadStart(rthread2));


                readthread2.Start();
                SetText7("Initialization Done");
            }
            catch(Exception o)
            {
                SetText7("Initialization failed"+o.ToString());
            }
        }

        public void intd()
        {
            try
            {
                portstart();
                //CentralControl = new Thread(new ThreadStart(CentControl));
                //CentralControl.Start();
                initnet();

                readthread = new Thread(new ThreadStart(rthread));

                readthread.Start();
                readthread2 = new Thread(new ThreadStart(rthread2));


                readthread2.Start();
                SetText7("Initialization Done");
            }
            catch (Exception o)
            {
                SetText7("Initialization failed" + o.ToString());
            }
        }

        private void RoboApp_Load (object sender, EventArgs e)
        {
            _clientSocket = socket();
            textBox5.Text = "192.168.1.3"; 
        }

        public void CentControl () 
        {
       
            string clientIdStr = clientId.ToString();
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                

                try
                {
                    attempts++;
                    _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(textBox5.Text), 3));

                    //assign id for clients
                    Commands clientInitialize = new Commands(Convert.ToInt32(clientId), "connectToServer");
                    //create connection xml file to send to server
                    using (XmlWriter writer = XmlWriter.Create("id_" +clientId+"_info.xml"))
                    {
                        writer.WriteStartDocument();
                            writer.WriteStartElement("Client");
                                writer.WriteElementString("ID", clientInitialize.Id.ToString());
                                writer.WriteElementString("Command", clientInitialize.Command);                            
                            writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                   //byte[] buffer = Encoding.Default.GetBytes("AAAAAAAAAAAAAA");
                    //_clientSocket.Send(buffer, 0, buffer.Length, 0);
                }
                catch(SocketException)
                {
                    SetText8("Connection Failed!! Attempt:" + attempts);
                }
            }

            Invoke((MethodInvoker)delegate
            {
                SetText8("Client "+clientIdStr+" Started");              
            });
            MessageBox.Show("Connection Successful");


            string fileName = "id_" + clientId + "_info.xml";// "Your File Name";
            string filePath = "";//Your File Path;

            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
            byte[] fileData = File.ReadAllBytes(filePath + fileName);
            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

            fileNameLen.CopyTo(clientData, 0);
            fileNameByte.CopyTo(clientData, 4);
            fileData.CopyTo(clientData, 4 + fileNameByte.Length);

            _clientSocket.Send(clientData);
        
        
        }

        private void button2_Click_1 (object sender, EventArgs e)
        {
           
            byte[] h= new byte[1];
            h[0] = 0x01;
            _comClass.writetest(h);
        }


        public void calcObstaclecord (int x1,int y1,int sdir,int dist) {

            int dir;
            if(sdir < 0)
            {
                dir = 360 + sdir;
            }
            else
            {
                dir = sdir;

            }

            Console.WriteLine("input cords for calc   " + x1 + "," + y1 + "direction = " + dir + "dsiatnce  = " + dist);
            int rdis = dist / 2;
            double x2 = 0;
            double y2 = 0;
            double dirad = dir * 3.1415 / 180.0;
            double tandir = Math.Tan(dirad);
            if(tandir >= 0 && dir <= 90)
            {

                y2 = (rdis / Math.Sqrt(1 + (tandir * tandir))) - y1;

            }
            else if(tandir >= 0 && dir <= 270 && dir > 180)
            {
                y2 = y1 + (rdis / Math.Sqrt(1 + (tandir * tandir)));

            }
            else if(tandir < 0 && dir <= 180 && dir > 90)
            {
                y2 = y1 + (rdis / Math.Sqrt(1 + (tandir * tandir)));

            }
            else if(tandir < 0 && dir <= 360 && dir > 270)
            {
                y2 = (rdis / Math.Sqrt(1 + (tandir * tandir))) - y1;

            }

            y2 = Math.Abs(y2);
            if(dir <= 90)
            {

                x2 = x1 + (tandir * (y1 - y2));
            }
            else if(dir > 90 && dir <= 180)
            {

                x2 = x1 + (tandir * (y1 - y2));


            }
            else if(dir > 180 && dir <= 270)
            {

                x2 = x1 + (tandir * (y1 - y2));


            }
            else if(dir > 270)
            {

                x2 = x1 + (tandir * (y1 - y2));


            }


            Console.WriteLine("x2 and y2   " + x2 + "," + y2);

            //Console.WriteLine(currentx+","+currenty+ " direc " + tandir+ "distance"+ dist);
            avoidx = Convert.ToInt32(Math.Abs(x2));
            avoidy = Convert.ToInt32(Math.Abs(y2));
            Console.WriteLine(avoidx + "," + avoidy + " direc " + tandir + "distance" + dist);
            //textBox2.Text = avoidx.ToString();
            //textBox3.Text = avoidy.ToString();
            //button5.PerformClick();

           // reNav();

        }

        private void button2_Click_2 (object sender, EventArgs e)
        {
            Map vMap = new Map();
            vMap.Show();
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            try
            {
                b1 = new Bitmap(openFileDialog1.FileName);
                pictureBox.Image = b1;

                // reset the size
                _pictureBoxZoom.Reset();
            }
            catch
            {
               // toolStripStatusLabel.Text = "Failed to load the image: " + openFileDialog1.FileName;
            }
        }

        private void comboBoxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pictureBoxZoom.Zoom((float)comboBoxZoom.SelectedItem);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            drawThrd = new Thread(drawloop);
            drawThrd.Start();
        }

        private void RoboApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                drawThrd.Abort();
            }
            catch { }
        }

        private void cafeteria_Click(object sender, EventArgs e)
        {
            navstart();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            textBox10.Text = "690";
            textBox11.Text = "270";
            navstart();
        }

        private void metroTile4_Click(object sender, EventArgs e)
        {
            textBox10.Text = "120";
            textBox11.Text = "570";
            navstart();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            readd();
            intd();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }


    }
}
