using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Simulator : MetroForm
    {
        #region varibales
        public static Bitmap b1;
        public static Bitmap b2;
        public static string mainbmpName;
        public static Bitmap map;


        public static int thrdtime = 20;
        public static int modulo = 1;
        public static int sonatThrd = 500;
        public static int dirThrd = 100;

        public PictureBoxZoom _pictureBoxZoom;

        public SerialPort emup1=new SerialPort();
        public SerialPort empu2=new SerialPort();

        public int RoboCurrentx;
        public int RoboCurrenty;

        public static Thread drawpath;
        public static Thread snrMiddle;
        public static Thread snrRight;
        public static Thread snrLeft;

        public static SerialPortRead sp;
        public static bool comsConnected = false;
        public static bool navigate = false;
        public static bool obs1 = false;

        #endregion

        public Simulator()
        {

            InitializeComponent();
           
            try
            {
                pictureBox.Image = new Bitmap("4000x4000_anchor_marked_extended.bmp");
             //   pictureBox2.ImageLocation = "sonar.png";
                pictureBox4.ImageLocation = "sonar_top.png";
                _pictureBoxZoom.Reset();
            }
            catch
            {
            }

            _pictureBoxZoom = new PictureBoxZoom(pictureBox);
            _pictureBoxZoom.OnZoomChange += UpdateZoomComboBox;
            UpdateZoomComboBox();
            ///////////////end zoom

            Form.CheckForIllegalCrossThreadCalls = false;

          
        }

        
        public Simulator (SerialPort _serialPort, SerialPort _serialPort2)
        {

            InitializeComponent();
            emup1 = _serialPort;
            empu2 = _serialPort2;
           
            try
            {
                pictureBox.Image = new Bitmap("4000x4000_anchor_marked_extended.bmp");
            //    pictureBox2.ImageLocation = "sonar.png";
                pictureBox4.ImageLocation = "sonar_top.png";
                _pictureBoxZoom.Reset();
            }
            catch
            {
            }

            _pictureBoxZoom = new PictureBoxZoom(pictureBox);
            _pictureBoxZoom.OnZoomChange += UpdateZoomComboBox;
            UpdateZoomComboBox();
            ///////////////end zoom

            Form.CheckForIllegalCrossThreadCalls = false;


        }


        private void PictureBox_Paint(object sender, PaintEventArgs e)///////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            Rectangle vvv = new Rectangle(200, 200, 40, 40);
            Pen pppp = new Pen(Color.Blue, 1);
            e.Graphics.DrawRectangle(pppp, vvv);
        }

        public void drawline(PictureBox pb, Point p1, Point p2, float Bwidth, Color c1)
        {
            //refresh the picture box
            pb.Refresh();
            //create a new Bitmap object
            Bitmap map11 = (Bitmap)pb.Image;
            //create a graphics object
            Graphics g = Graphics.FromImage(map);
            //create a pen object and setting the color and width for the pen
            Pen p = new Pen(c1, Bwidth);
            //draw line between  point p1 and p2
            g.DrawLine(p, p1, p2);
            pb.Image = map11;
            //dispose pen and graphics object
            p.Dispose();
            g.Dispose();
        }
        public void drawEllipse(PictureBox pb, int x, int y, int w, int h, float Bwidth, Color col)
        {
            //refresh the picture box
            //pb.Refresh();
            Bitmap mapTemp = (Bitmap)pb.Image;
            //create a graphics object
            Graphics g = pb.CreateGraphics();
            //create a pen object
            Pen p = new Pen(col, Bwidth);
            //draw Ellipse
            g.DrawEllipse(p, x, y, w, h);
            //pb.Image = map;
            //pictureBox.Invalidate();
            pictureBox.Image = mapTemp;
            //dispose pen and graphics object
            p.Dispose();
            g.Dispose();
        }

      
        private void Form_Load(object sender, EventArgs e)
        {
           
            String number = "30";
            

        }

        Bitmap myBitmap;



        delegate void SetTextCallback(string text);

   

        private void pictureBox_OnHover(object sender, MouseEventArgs e)
        {
           
        }

 
        public void button1cick() { }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
           
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (PreClosingConfirmation() == System.Windows.Forms.DialogResult.Yes)
            {
                Dispose(true);
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private DialogResult PreClosingConfirmation()
        {
            // System.Windows.Forms.MessageBox.Show(" ", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            DialogResult res = MetroFramework.MetroMessageBox.Show(this, "Do you want to quit?          ", "Quit...", MessageBoxButtons.YesNo, MessageBoxIcon.Question); ;
            return res;
        }

        private void Simulator_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


        public bool ObjectToFile(object _Object, string _FileName)
        {
            try
            {
                // create new memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream();
                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                // Serializes an object, or graph of connected objects, to the given stream.
                _BinaryFormatter.Serialize(_MemoryStream, _Object);
                // convert stream to byte array
                byte[] _ByteArray = _MemoryStream.ToArray();
                // Open file for writing
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray.ToArray(), 0, _ByteArray.Length);
                // close file stream
                _FileStream.Close();
                // cleanup
                _MemoryStream.Close();
                _MemoryStream.Dispose();
                _MemoryStream = null;
                _ByteArray = null;
                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                //listBox1.Items.Add(_Exception.ToString());
               // MessageBox.Show();
            }
            // Error occured, return null
            return false;
        }


        public object FileToObject(string _FileName)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);
                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(_FileName).Length;
                // read entire file into buffer
                byte[] _ByteArray = _BinaryReader.ReadBytes((Int32)_TotalBytes);
                // close file reader and do some cleanup
                _FileStream.Close();
                _FileStream.Dispose();
                _FileStream = null;
                _BinaryReader.Close();
                // convert byte array to memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_ByteArray);
                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                // set memory stream position to starting point
                _MemoryStream.Position = 0;
                // Deserializes a stream into an object graph and return as a object.
                return _BinaryFormatter.Deserialize(_MemoryStream);
            }
            catch (Exception _Exception)
            {
                // Error
                MessageBox.Show("file read error");
            }
            // Error occured, return null
            return null;
        }


    

     
        public void WriteListString(List<string> listt, string file)
        {
            using (FileStream fs = File.OpenWrite(file))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Put count.
                writer.Write(listt.Count);
                // Write pairs.
                foreach (var pair in listt)
                {
                    writer.Write(pair);
                    
                }
            }
        }
   
        
   

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Returns the filter with all image file extensions that
        /// can be open on the current system via C#
        /// </summary>
        /// <returns>Filter for OpenFileDialog containing image extensions</returns>
        private string getImageFilters()
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            string filters = "";

            foreach (var codec in codecs)
            {
                var codecName = codec.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filters += String.Format("{0} ({1})|{1}", codecName, codec.FilenameExtension);
                filters += "|";
            }
            filters += String.Format("{0} ({1})|{1}", "All Files", "*.*");
            return filters;
        }

        /// <summary>
        /// Fills the zoom combobox with the default zoom values 
        /// and the current zoom value.
        /// </summary>
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
            comboBoxZoom.Items.AddRange( Array.ConvertAll<float, object>(range.ToArray(),
                new Converter<float, object>(element => element)));

            comboBoxZoom.SelectedItem = _pictureBoxZoom.ZoomFactor;
        }

        private void comboBoxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pictureBoxZoom.Zoom((float)comboBoxZoom.SelectedItem);
        }

     

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void button1_Click (object sender, EventArgs e)
        {
            Robot.currentDir = Convert.ToInt32(textBox2.Text);
            Robot.startx = Convert.ToInt32(textBox3.Text);
            Robot.starty = Convert.ToInt32(textBox4.Text);
            Robot.currentx = Robot.startx;
            Robot.currenty = Robot.starty;

            Thread simu = new Thread(simulate);
            simu.Start();
            //sonars();
            Sonartoggle();

        }


        public void simulate ()
        {
            Thread.Sleep(500);
            sp.direction();

            Thread rd1 = new Thread(rthread);
            rd1.Start();
            Thread nav = new Thread(navThrd);
            nav.Start();
            sendDirnUpadte();
        }


        public void navThrd ()
        {

            while(true)
            {
                if(navigate==true)
                {
                    navigate = false;
                    drawrobo();


                }
                Thread.Sleep(100);
            }

        }


        private void rthread ()
        {

            while(true)
            {

                //textBox4.AppendText(roboctrl.Com.serialtty.ReadData());
                try
                {
                    textBox1.AppendText(sp.ReadCom1());
                    Thread.Sleep(100);
                }
                catch(Exception read)
                {
                    textBox1.AppendText("could not read from port" + read.ToString());

                }
                //textBox1.AppendText("test\n");

            }

        }

        private void button2_Click (object sender, EventArgs e)
        {
            sp.turnAck();
        }

        private void button3_Click (object sender, EventArgs e)
        {
            sp.turnReached();
        }

        private void button4_Click (object sender, EventArgs e)
        {
            sp.goForwardAck();
        }

        private void button5_Click (object sender, EventArgs e)
        {
            sp.destReached();
        }

        private void button6_Click (object sender, EventArgs e)
        {
            Console.WriteLine();
        }

        private void SimulaterE_Load (object sender, EventArgs e)
        {
            button10.ForeColor = Color.Black;
          //b1 = new Bitmap("bmp1.bmp");
          b2 = new Bitmap("bmp2.bmp");

          sp = new SerialPortRead(emup1, empu2);
          if(SerialPortRead.com1Open == true)
          {
              textBox8.Text = emup1.PortName;
              textBox8.BackColor = Color.LimeGreen;
          }
          else
          {
              textBox8.Text = emup1.PortName;
              textBox8.BackColor = Color.Red;

          }

          if(SerialPortRead.com2Open == true)
          {
              textBox9.Text = empu2.PortName;
              textBox9.BackColor = Color.LimeGreen;
          }
          else
          {
              textBox9.Text = empu2.PortName;
              textBox9.BackColor = Color.Red;

          }

          if(textBox8.BackColor == Color.LimeGreen && textBox9.BackColor == Color.LimeGreen)
          {
              groupBox2.Enabled = true;
              comsConnected = true;
          }
          else
          {

              groupBox2.Enabled = false;
          }
        }




        public void calcobsticleoppath (int x1, int y1, int x2, int y2)
        {

            DrawRobot dr = new DrawRobot();

            bool obs = false;
            double y = 0;
            double m = 0;
            double x = 0;
            double c = 0;

            Console.WriteLine(x1 + "," + y1 + "********" + x2 + "," + y2);
            if(x2 - x1 != 0)
            {
                m = (double)(y2 - y1) / (double)(x2 - x1);
            }

            c = y1 - (m * x1);
            Console.WriteLine(m);

            if(Math.Abs(x2 - x1) >= Math.Abs(y2 - y1) && x2 >= x1)
            {
                for(int i = x1; i <= x2; i++)
                {

                    y = m * i + c;

                    if(i%modulo==0){
                    pictureBox.Image = dr.SuperimposeImage(i,(int)y);
                    }
                    Robot.currentx = i;
                    Robot.currenty = (int)y;
                    Thread.Sleep(thrdtime);
                    //if(Data.bmatrix[i, (int)y] == true)
                    //{
                    //    obs = true;
                    //    Console.WriteLine("obsticle");
                    //    return obs;


                    //}

                }

            }
            else if(Math.Abs(x2 - x1) < Math.Abs(y2 - y1) && y2 >= y1)
            {
                for(int i = y1; i <= y2; i++)
                {

                    y = m * i + c;

                    if(m == 0)
                    {
                        x = x1;
                    }
                    else
                    {
                        x = (i - c) / m;
                    }

                    if(i % modulo == 0)
                    {
                        pictureBox.Image = dr.SuperimposeImage((int)x, i);
                    }
                    Robot.currentx = (int)x;
                    Robot.currenty = i;
                    Thread.Sleep(thrdtime);
                    //if(Data.bmatrix[(int)x, i] == true)
                    //{
                    //    obs = true;
                    //    Console.WriteLine("obsticle");
                    //    return obs;


                    //}
                }

            }
            else if(Math.Abs(x2 - x1) >= Math.Abs(y2 - y1) && x1 > x2)
            {
                for(int i = x1; i >= x2; i--)
                {

                    y = m * i + c;
                    if(i % modulo == 0)
                    {
                        pictureBox.Image = dr.SuperimposeImage(i, (int)y);
                    }
                    Robot.currentx = i;
                    Robot.currenty = (int)y;
                    Thread.Sleep(thrdtime);
                    //if(Data.bmatrix[i, (int)y] == true)
                    //{
                    //    obs = true;
                    //    Console.WriteLine("obsticle");
                    //    return obs;


                    //}
                }

            }

            else if(Math.Abs(x2 - x1) < Math.Abs(y2 - y1) && y2 < y1)
            {
                for(int i = y1; i >= y2; i--)
                {

                    y = m * i + c;

                    if(m == 0)
                    {
                        x = x1;
                    }
                    else
                    {
                        x = (i - c) / m;
                    }
                    if(i % modulo == 0)
                    {
                        pictureBox.Image = dr.SuperimposeImage((int)x, i);
                    }
                    Robot.currentx = (int)x;
                    Robot.currenty = i;
                    Thread.Sleep(thrdtime);
                    //if(Data.bmatrix[(int)x, i] == true)
                    //{
                    //    obs = true;
                    //    Console.WriteLine("obsticle");
                    //    return obs;


                    //}

                    



                }

            }
            Robot.startx = Robot.destinationx;
            Robot.starty = Robot.destinationy;
            if(comsConnected==true)
            {
                sp.destReached();
            }
            //Console.WriteLine("*****@@no Obstacle@@****");
           
        }

        private void button7_Click (object sender, EventArgs e)
        {
            drawrobo();
            simulate();
        }

        public void drawrobo ()
        {

            drawpath = new Thread(pathdraw);
            drawpath.Start();
        }

        public void pathdraw ()
        {
            calcobsticleoppath(Robot.startx, Robot.starty, Robot.destinationx, Robot.destinationy);
        }

        private void button8_Click (object sender, EventArgs e)
        {

            Sonartoggle();
        }


        public void Sonartoggle()
        {
            if(button8.Text == "Sonar Off")
            {
                try
                {
                    sonars();
                    button8.Text = "Sonar On";
                }
                catch { }
            }
            else
            {
                try
                {
                    snrMiddle.Abort();
                    snrLeft.Abort();
                    snrRight.Abort();
                    button8.Text = "Sonar Off";
                }
                catch { }
            }

        }
        public static void middlesonar ()
        {
            try
            {
                Simulator.snrMiddle.Resume();
            }
            catch
            {
            }
            try 
            {
                snrMiddle.Start();
            }
            catch
            { 
            }
            
        
        }

        public void sonars ()
        { 
            //Timer for the sonar image blink
            t = new System.Windows.Forms.Timer();
            t.Interval = 1000;
            t.Enabled = false;

            t.Start();
             snrMiddle = new Thread(sonarMiddleloop);
           

             snrRight = new Thread(sonarRightloop);
            snrRight.Start();

             snrLeft = new Thread(sonarLeftloop);
            snrLeft.Start();

           
             

        
        }
        System.Windows.Forms.Timer t;

        public void sonarMiddleloop ()
        {
            while(true)
            {
                if(Sonar.chkMiddle() == true)
                {
                    textBox5.BackColor = Color.Red;

                    t.Tick += new EventHandler(toptimer_Tick);
                    textBox5.Text = "Obstacle";
                    sp.sonarMiddle(50);
                }
                else
                {
                    t.Tick -= new EventHandler(toptimer_Tick);
                    textBox5.BackColor = Color.LimeGreen;
                    textBox5.Text = "Clear";
                    pictureBox4.Visible = false;
                    sp.sonarMiddle(200);

                }
                Thread.Sleep(sonatThrd);
            }
        
            }

        private void toptimer_Tick(object sender, EventArgs e)
        {
            if (pictureBox4.Visible == true)
            {
                pictureBox4.Visible = false;
            }
            else
            {
                pictureBox4.Visible = true;

            }
        }
        private void lefttimer_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Visible == true)
            {
                pictureBox1.Visible = false;
            }
            else
            {
                pictureBox1.Visible = true;

            }
        }
        private void righttimer_Tick(object sender, EventArgs e)
        {
            if (pictureBox5.Visible == true)
            {
                pictureBox5.Visible = false;
            }
            else
            {
                pictureBox5.Visible = true;

            }
        }
        public void sonarRightloop ()
        {
            while(true)
            {

                if(Sonar.chkRight() == true)
                {
                    textBox6.BackColor = Color.Red;
                    textBox6.Text = "Obstacle";

                    t.Tick += new EventHandler(righttimer_Tick);
                    sp.sonarRight(50);
                }
                else
                {
                    textBox6.BackColor = Color.LimeGreen;
                    textBox6.Text = "Clear";
                    t.Tick -= new EventHandler(toptimer_Tick);
                    sp.sonarRight(150);
                    pictureBox5.Visible = false;
                }
                Thread.Sleep(sonatThrd);
            }

        }

        public void sonarLeftloop ()
        {
            while(true)
            {

                if(Sonar.chkLeft() == true)
                {
                    textBox7.BackColor = Color.Red;
                    textBox7.Text = "Obstacle";
                    t.Tick += new EventHandler(lefttimer_Tick);


                    sp.sonarLeft(50);
                }
                else
                {
                    textBox7.BackColor = Color.LimeGreen;
                    textBox7.Text = "Clear";
                    t.Tick -= new EventHandler(lefttimer_Tick);
                    pictureBox1.Visible = false;
                    sp.sonarLeft(150);
                }
                Thread.Sleep(sonatThrd);
            }

        }

        private void pictureBox_Click (object sender, EventArgs e)
        {
            
        }

        public static Image SuperimposeObs (int x, int y)
        {
            //load both images

            
            Image mainImage = b1;
            Image imposeImage = Simulator.b2;
            try
            {
                //create graphics from main image
                using(Graphics g = Graphics.FromImage(mainImage))
                {
                    //draw other image on top of main Image
                    g.DrawImage(imposeImage, new Point(x , y));
                   // b1 = new Bitmap(mainImage);
                    
                    //save new image
                    mainImage.Save("obs.bmp");
                    return mainImage;
                }
            }
            catch(Exception uu)
            {
                return null;
                //label2.Text = uu.ToString();
            }
            //return temp;
        }


        private void pictureBox_MouseClick (object sender, MouseEventArgs e)
        {
            if(button10.ForeColor == Color.Black)
            {
                if(e.Button == MouseButtons.Left)
                {
                    Robot.startx = e.X;
                    Robot.starty = e.Y;
                    label2.Text = "Start Cords : " + Robot.startx + "," + Robot.starty;
                }
                else if(e.Button == MouseButtons.Right)
                {
                    Robot.destinationx = e.X;
                    Robot.destinationy = e.Y;
                    label3.Text = "Destination Cords : " + Robot.destinationx + "," + Robot.destinationy;
                }
            }
            else
            {
               //b1= new Bitmap(SuperimposeObs(e.X,e.Y));
               pictureBox.Image = SuperimposeObs(e.X, e.Y);
            }
        }

        private void button9_Click (object sender, EventArgs e)
        {
          // MessageBox.Show("   "+ Methods.calcDir().ToString());
            sendDirnUpadte();
        }

        public void sendDirnUpadte ()
        {
            Thread dirclc = new Thread(updateDir);
            dirclc.Start();
        }

        public void updateDir ()
        {
            while(true)
            {
                if(Robot.destinationx != 0 && Robot.destinationy != 0 )
                {
                    if(Robot.currentx != Robot.destinationx || Robot.currenty != Robot.destinationy)
                    {
                        Robot.currentDir = Methods.calcDir();
                        Console.WriteLine("current heading "+ Robot.currentDir);
                        sp.direction();

                    }
                }
                label1.Text = "Current Direction : " + Robot.currentDir.ToString();
                label2.Text = "Start Cords : " + Robot.startx + "," + Robot.starty;
                label3.Text = "Destination Cords : " + Robot.destinationx + "," + Robot.destinationy;
                label4.Text = "Current Cords : " + Robot.currentx + "," + Robot.currenty;
               
                Thread.Sleep(dirThrd);
            }
        }

        public static  void roboStop ()
        {

            try
            {
                drawpath.Abort();
                try
                {
                    Simulator.snrMiddle.Suspend();
                }
                catch
                {
                }


                Robot.destinationx = 0;
                Robot.destinationy = 0;
                
                sp.stopped(Methods.getDistance(Robot.startx,Robot.starty,Robot.currentx,Robot.currenty));
                Robot.startx = Robot.currentx;
                Robot.starty = Robot.currenty;
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~aborted");

            }catch(Exception thrd)
            {
            
            }
        }




        private void label1_Click (object sender, EventArgs e)
        {

        }

        private void button10_Click (object sender, EventArgs e)
        {
            if(button10.ForeColor == Color.Black)
            {
                button10.ForeColor = Color.Red;
            }
            else
            {
                button10.ForeColor = Color.Black;
                b1 = new Bitmap("obs.bmp");
                Sonar.setBitMap("obs.bmp");
                Simulator.mainbmpName = "obs.bmp";
            }
           
        }

        private void configureToolStripMenuItem_Click (object sender, EventArgs e)
        {
            
        }

        private void button11_Click (object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void metroLink1_Click(object sender, EventArgs e)
        {
            metroContextMenu1.Show(metroLink1, new Point(0, metroLink1.Height));
        }

        private void metroLink2_Click(object sender, EventArgs e)
        {
            metroContextMenu2.Show(metroLink2, new Point(0, metroLink2.Height));
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = getImageFilters();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mainbmpName = ofd.FileName;
                    b1 = new Bitmap(ofd.FileName);
                    map = new Bitmap(ofd.FileName);
                    Sonar.setBitMap(mainbmpName);
                    pictureBox.Image = new Bitmap(ofd.FileName);

                    // reset the size
                    _pictureBoxZoom.Reset();
                }
                catch
                {
                    toolStripStatusLabel.Text = "Failed to load the image: " + ofd.FileName;
                }
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void configureToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Configure confg = new Configure();
            confg.Show();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            //if (metroToggle1.Checked == true) {
            //  //  MessageBox.Show("adasd");
                
                
            //    try
            //        {
            //            sonars();
            //            button8.Text = "Sonar On";
            //        }
            //        catch { }
            //}
            //else
            //{
            //    try
            //    {
            //        snrMiddle.Abort();
            //        snrLeft.Abort();
            //        snrRight.Abort();
            //        button8.Text = "Sonar Off";
            //    }
            //    catch { }
            //}
        }

    }


    public class SerialPortRead
    {

        #region variables
      
        public SerialPort Semup1 = new SerialPort();
        public SerialPort Semup2 = new SerialPort();

        public static bool com1Open = false;
        public static bool com2Open = false;

        public byte[] tmpbytebff = new byte[12];

        #endregion

        public SerialPortRead (SerialPort serialport1, SerialPort serialport2)
        {
            Semup1 = serialport1;
            Semup2 = serialport2;

            try
            {
                Semup1.Open();
                com1Open = true;
            }
            catch(Exception pe)
            { 
            }
            try
            {
                
                Semup2.Open();
                com2Open = true;
            }
            catch(Exception pe)
            {
            }
        }




        public string ReadCom1 ()
        {
            int byteindcount = 0;
            // int byteindcount2 = 0;
            byte tmpByte = 0x00;
            //byte tmpByte2 = 0x00;
            string rxString = "";

            if(Semup1.BytesToRead > 0)
            {
                try
                {
                    
                    while(tmpByte != 255)
                    {
                        tmpByte = (byte)this.Semup1.ReadByte();
                        tmpbytebff[byteindcount] = tmpByte;
                        byteindcount++;
                        
                    }

                    rxString = System.Text.Encoding.Default.GetString(tmpbytebff);
                }
                catch(Exception tt)
                {
                    rxString = "";
                }

            }
            else
            {
               // rxString = "no bytes";
            }


            if(tmpbytebff[1]==0xC2)
            {
                rxString = "turn command received\n";

                Robot.turnDir = Methods.byteToInt(tmpbytebff[5], tmpbytebff[6]);

                if(tmpbytebff[3]==0x01)
                {

                     Robot.turnSide="right";
                     if((Robot.currentDir + Robot.turnDir) < 360)
                     {
                         Robot.currentDir = Robot.currentDir + Robot.turnDir;
                     }
                     else
                     {
                         Robot.currentDir = (Robot.currentDir + Robot.turnDir) - 360;
                     }
                }
               
                else
                {
                    Robot.turnSide="left";
                    if((Robot.currentDir - Robot.turnDir) > 0)
                    {
                        Robot.currentDir = Robot.currentDir - Robot.turnDir;
                    }
                    else
                    {
                        Robot.currentDir = 360 -  (Robot.currentDir - Robot.turnDir) ;
                    }

                }


                rxString = Robot.turnSide + "   " + Robot.turnDir+"\n";

                turnAck();
                Thread.Sleep(500);
                turnReached();
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
            }

            if(tmpbytebff[1] == 0xC1)
            {
                rxString = "goforward command received\n";
                Methods.calcNextCord(Robot.startx,Robot.starty,Robot.currentDir,Methods.byteToInt(tmpbytebff[4], tmpbytebff[5]));
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                rxString = Robot.startx + "," + Robot.starty + "   " + Robot.destinationx + "," + Robot.destinationy + "\n";
                goForwardAck();
                Simulator.navigate = true;
                
                    Simulator.obs1 = true;
                    //SimulaterE.middlesonar();
                
            }


            if(tmpbytebff[1] == 0xC3)
            {
                stopAck();
                Simulator.roboStop();
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);

            }








            return rxString;
        }

        public void writeCom1 (byte[] command)
        {
            try
            {
                Semup1.Write(command, 0, command.Length);
                Semup1.DiscardOutBuffer();
                
            }
            catch(Exception write)
            {
                
            }

        }
        public void writeCom2 (byte[] command)
        {
            try
            {
                Semup2.Write(command, 0, command.Length);
                Semup2.DiscardOutBuffer();
            }
            catch(Exception write)
            {

            }

        }



        #region Commands to send by Com1

        public void turnAck ()
        {
            byte[] temp = new byte[8];
            temp[0] = 0x02;
            temp[1] = 0xF2;
            temp[2] = 0x01;
            temp[3] = 0x01;
            temp[4] = 0x00;
            temp[5] = 0x00;
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom1(temp);  

        }

        public void  turnReached()
        {
            byte[] temp = new byte[8];
            temp[0] = 0x02;
            temp[1] = 0xD2;
            temp[2] = 0x01;
            temp[3] = 0x01;
            temp[4] = 0x00;
            temp[5] = 0x00;
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom1(temp);

        }

        public void goForwardAck ()
        {
            byte[] temp = new byte[8];
            temp[0] = 0x02;
            temp[1] = 0xF1;
            temp[2] = 0x01;
            temp[3] = 0x01;
            temp[4] = 0x00;
            temp[5] = 0x00;
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom1(temp);
        }

        public void destReached ()
        {
            byte[] temp = new byte[8];
            temp[0] = 0x02;
            temp[1] = 0xD1;
            temp[2] = 0x01;
            temp[3] = 0x01;
            temp[4] = 0x00;
            temp[5] = 0x00;
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom1(temp);
        }

        public void stopAck ()
        {
            byte[] temp = new byte[8];
            temp[0] = 0x02;
            temp[1] = 0xF3;
            temp[2] = 0x01;
            temp[3] = 0x01;
            temp[4] = 0x00;
            temp[5] = 0x00;
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom1(temp);

        }

        public void stopped (int value)
        {
            byte[] distance = Methods.intToByte(value);
            byte[] temp = new byte[8];
            temp[0] = 0x02;
            temp[1] = 0xD3;
            temp[2] = 0x03;
            temp[3] = 0x01;
            temp[4] = distance[0];
            temp[5] = distance[1];
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom1(temp);
        }
        #endregion

        #region commands to send by Com2

        public void direction ()
        {
            byte[] temp = new byte[8];
            byte[] dir = new byte[2];
            dir = Methods.intToByte(Robot.currentDir);

            temp[0] = 0x02;
            temp[1] = 0xE2;
            temp[2] = 0x03;
            temp[3] = dir[0];
            temp[4] = dir[1];
            temp[5] = 0x00;
            temp[6] = 0xFF;
            temp[7] = 0xFF;

            this.writeCom2(temp);
        }


        public void sonarMiddle (int value)
        {
            byte[] temp = new byte[8];
            byte[] dir = new byte[2];
            dir = Methods.intToByte(value);

            temp[0] = 0x03;
            temp[1] = 0xE0;
            temp[2] = 0x03;
            temp[3] = 0x02;
            temp[4] = dir[0];
            temp[5] = dir[1];
            temp[6] = 0x00;
            temp[7] = 0xFF;

            this.writeCom2(temp);
        }

        public void sonarRight (int value)
        {
            byte[] temp = new byte[8];
            byte[] dir = new byte[2];
            dir = Methods.intToByte(value);

            temp[0] = 0x03;
            temp[1] = 0xE0;
            temp[2] = 0x03;
            temp[3] = 0x01;
            temp[4] = dir[0];
            temp[5] = dir[1];
            temp[6] = 0x00;
            temp[7] = 0xFF;
           

            this.writeCom2(temp);
        }

        public void sonarLeft (int value)
        {
            byte[] temp = new byte[8];
            byte[] dir = new byte[2];
            dir = Methods.intToByte(value);

            temp[0] = 0x03;
            temp[1] = 0xE0;
            temp[2] = 0x03;
            temp[3] = 0x03;
            temp[4] = dir[0];
            temp[5] = dir[1];
            temp[6] = 0x00;
            temp[7] = 0xFF;

            this.writeCom2(temp);
        }


        #endregion


    }

    public static class Methods
    {
        public static int byteToInt (byte l,byte h)
        {
            byte[] ar = new byte[2];
            ar[0] = h;
            ar[1] = l;
            int i = BitConverter.ToInt16(ar, 0);
            return i;
        }

        public static byte[] intToByte (int value)
        {
            int value16 = Convert.ToInt16(value);
             
            byte[]   intBytes = BitConverter.GetBytes(value16);
            //Array.Reverse(intBytes);

            byte[] result= new byte[2];
            result[0]= intBytes [1];
            result[1] = intBytes[0];
           
            return result;
        }


        public static  void calcNextCord (int x1, int y1, int sdir, int dist)
        {

            int dir;

            if(sdir < 0)
            {
                dir = 360 + sdir;
            }
            else
            {
                dir = sdir;
            }

            //Console.WriteLine("input cords for calc   " + x1 + "," + y1 + "direction = " + dir + "dsiatnce  = " + dist);
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


            Robot.destinationx = Convert.ToInt32(Math.Abs(x2));
            Robot.destinationy = Convert.ToInt32(Math.Abs(y2));
           // Console.WriteLine(Robot.destinationx + "," + Robot.destinationy + " direc " + tandir + "distance" + dist);
            

        }

        public static int calcDir ()
        {
            int dir = 0;
            int x1 = Robot.currentx;
            int y1 = Robot.currenty;
            int x2 = Robot.destinationx;
            int y2 = Robot.destinationy;
            Console.WriteLine(x1+"  "+y1+"  "+x2+"  "+y2);

            if(x2-x1==0 && y2<y1 )
            {
                dir = 0;
            }
            else if(x2 - x1 == 0 && y2 > y1)
            {
                dir = 180;
            }

            else if(y2 - y1 == 0 && x2<x1)
            {
                dir = 270;
            }
            else if(y2 - y1 == 0 && x2 > x1)
            {
                dir = 90;
            }
            else
            {
                float m = ((float)y2 - (float)y1) / ((float)x2 - (float)x1);
                Console.WriteLine(m);
                if(y1>y2 &&  x1 < x2)
                {
                dir =90+ (int)(Math.Atan(m) * (180.0 / Math.PI));
                }
                if(y1 < y2 && x1 < x2)
                {
                    dir = 90+ (int)(Math.Atan(m) * (180.0 / Math.PI));
                }
                if(y1 < y2 && x1 > x2)
                {
                    dir = 270+(int)(Math.Atan(m) * (180.0 / Math.PI));
                }
                if(y1 > y2 && x1 > x2)
                {
                    dir = 270 + (int)(Math.Atan(m) * (180.0 / Math.PI));
                }

            }

            return dir;
        
        }

        public static int getDistance (int x1,int y1,int x2,int y2)
        {
            Console.WriteLine("stoped******** cordsssss " + x1 +" "+ y1+" "+x2+" "+y2);
            int i =0;
            i = (int)Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
            i = i * 2;
            Console.WriteLine("stoped******** "+i);
            return i;
        }

    }

    public static class Robot
    {
        #region robot attributes

        public static int currentDir;
        public static int turnDir;
        public static string turnSide;




        public static int startx;
        public static int starty;

        public static int currentx;
        public static int currenty;

        public static int destinationx;
        public static int destinationy;

        public static int sonarRange;

        #endregion


    }


    



    public class DrawRobot
    {

        public static void pntTopntDraw (int x1,int y1,int x2,int y2)
        {
        
        
        }


      public  Image SuperimposeImage (int x, int y)
        {
            //load both images
            GC.Collect();
            Bitmap bmpImage = new Bitmap(Simulator.mainbmpName);  
            
                Image mainImage = bmpImage;

                Image imposeImage = Simulator.b2;
                try
                {
                    //create graphics from main image
                    using(Graphics g = Graphics.FromImage(mainImage))
                    {
                        //draw other image on top of main Image
                        g.DrawImage(imposeImage, new Point(x - 15, y - 15));
                        // nnb1.Dispose();
                        GC.Collect();
                        g.Dispose();
                        //save new image
                        //mainImage.Save("bmp3.bmp");
                        return mainImage;
                    }
                }
                catch(Exception uu)
                {
                    GC.Collect();
                    return null;
                    //label2.Text = uu.ToString();
                }
                //SimulaterE.map = SimulaterE.b1;
               
                //return temp;
                GC.Collect();
        }
    
    }

    public static class Sonar
    {
        static Bitmap sb1 = new Bitmap(Simulator.mainbmpName);
        public static int sonarx;
        public static int sonary;

        public static int rightsnrDir;


        public static int leftsnrDir;

        public static void setBitMap (string name)
        {
            sb1 = Simulator.b1;
        
        }

        public static bool chkMiddle ()
        {
            sonarCords(Robot.currentx, Robot.currenty, Robot.currentDir,160);

            if(sonarRangeLoop(Robot.currentx, Robot.currenty, sonarx, sonary) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
          
        }



        public static bool chkRight ()
        {

            if(Robot.currentDir + 90 >= 360)
            {
                rightsnrDir = Math.Abs(360 - Robot.currentDir + 90);
            }
            else
            {
                rightsnrDir = Robot.currentDir + 90;
            }

            sonarCords(Robot.currentx, Robot.currenty, rightsnrDir, 160);

            if(sonarRangeLoop(Robot.currentx, Robot.currenty, sonarx, sonary) == true)
            {
                return true;
            }
            else
            {
                return false;
            } 

        }

        public static bool chkLeft ()
        {

            if(Robot.currentDir - 90 >= 0)
            {
                leftsnrDir = Math.Abs(Robot.currentDir - 90);
            }
            else
            {
                leftsnrDir =360 -Math.Abs( Robot.currentDir - 90);
            }

            sonarCords(Robot.currentx, Robot.currenty, leftsnrDir, 160);

            if(sonarRangeLoop(Robot.currentx, Robot.currenty, sonarx, sonary) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public static void sonarCords (int x1, int y1, int dir, int dist)
        {

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

          
           sonarx = Convert.ToInt32(Math.Abs(x2));
            sonary = Convert.ToInt32(Math.Abs(y2));
          
        }

        public static bool sonarRangeLoop (int x1, int y1, int x2, int y2)
        {
            bool obs = false;
            double y = 0;
            double m = 0;
            double x = 0;
            double c = 0;
  
            if(x2 - x1 != 0)
            {
                m = (double)(y2 - y1) / (double)(x2 - x1);
            }

            c = y1 - (m * x1);
           
            if(Math.Abs(x2 - x1) >= Math.Abs(y2 - y1) && x2 >= x1)
            {
                for(int i = x1; i <= x2; i++)
                {

                    y = m * i + c;
                    try
                    {
                        if(sb1.GetPixel(i, (int)y).ToArgb() != -1)
                        {
                            return obs = true;
                        }
                    }
                    catch { }
                 

                }

            }
            else if(Math.Abs(x2 - x1) < Math.Abs(y2 - y1) && y2 >= y1)
            {
                for(int i = y1; i <= y2; i++)
                {

                    y = m * i + c;

                    if(m == 0)
                    {
                        x = x1;
                    }
                    else
                    {
                        x = (i - c) / m;
                    }

                    try
                    {
                        if(sb1.GetPixel((int)x, i).ToArgb() != -1)
                        {
                            return obs = true;
                        }
                    }
                    catch { }
                   
                }

            }
            else if(Math.Abs(x2 - x1) >= Math.Abs(y2 - y1) && x1 > x2)
            {
                for(int i = x1; i >= x2; i--)
                {

                    y = m * i + c;
                    try
                    {
                        if(sb1.GetPixel(i, (int)y).ToArgb() != -1)
                        {
                            return obs = true;
                        }
                    }
                    catch { }
                    
                }

            }

            else if(Math.Abs(x2 - x1) < Math.Abs(y2 - y1) && y2 < y1)
            {
                for(int i = y1; i >= y2; i--)
                {

                    y = m * i + c;

                    if(m == 0)
                    {
                        x = x1;
                    }
                    else
                    {
                        x = (i - c) / m;
                    }
                    try
                    {
                        if(sb1.GetPixel((int)x, i).ToArgb() != -1)
                        {
                            return obs = true;
                        }
                    }
                    catch { }

                }

            }
           
            return obs;

        }
    
    }


    public class Maps
    { 
    
    
    }

}
