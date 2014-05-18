using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboAppMono
{
    class SPCom
    {
       // public SerialPort SP =new SerialPort();
        public byte[] dis=new byte[8];
        public List<byte> bBuffer = new List<byte>();
        public List<byte> sBuffer;
        public byte[] tmpbytebff=new byte[8];
        public byte[] tmpbytebff2 = new byte[8];
        public static  bool destReached = false;
        public static  bool turnReached = false;
        public static  bool fcmdRecived = false;
        public static  bool tcmdRecived = false;
        public static bool fstopRecived = false;
        public static bool renavi = false;
        public static bool renavdir = false;
        public static bool middleobs = false;
        public static bool leftobs = false;
        public static bool rightobs = false;
        public static bool obsturn = false;


        public SPCom()
        {
            //SP = RoboApp.emup;
            
            ////SP.DataReceived += new SerialDataReceivedEventHandler(port1_DataReceived);
            //try
            //{
            //    SP.Open();
            //}
            //catch (Exception ex) {
            //    Console.WriteLine(ex);
            //}

        }

        public string ReadData()
        {
            int byteindcount=0;
           // int byteindcount2 = 0;
            byte tmpByte=0x00;
            //byte tmpByte2 = 0x00;
            string rxString = "";

            if (RoboApp.emup.BytesToRead > 0 )
            {
                try
                {
                    //tmpByte = (byte)SP.ReadByte();

                    while (tmpByte != 255 )
                    {
                        tmpByte = (byte)RoboApp.emup.ReadByte();
                        tmpbytebff[byteindcount] = tmpByte;
                        byteindcount++;
                        //if (tmpByte2 != 255) { 
                        //    tmpByte2 = (byte)RoboApp.emup2.ReadByte();
                        //    tmpbytebff2[byteindcount2] = tmpByte2;
                        //    byteindcount2++;
                        //}
                       
                        
                        
                       // rxString += ((char)tmpByte);

                      
                    }
                }
                catch (Exception tt)
                {
                   
                }

            }
            else
            {
                //rxString = "no bytes";
            }

            if (tmpbytebff[1]==0xD1)
            {
                rxString = "Destination reached\n";
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                destReached = true;
                if(renavi==false){
                RoboApp.DestReached = 1;

                }
                renavi = false;
            }

            if(tmpbytebff[1] == 0xD2)
            {
                rxString = "Turn reached\n";
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                turnReached = true;
                if(RoboApp.redir == false)
                {
                    RoboApp.TurnReached = 1;
                }
                else
                {
                    renavdir = true;
                
                }
                RoboApp.redir = false;
            }

            if(tmpbytebff[1] == 0xF1)
            {
                rxString = "gofroward command recived\n";
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                fcmdRecived = true;
               
            }


            if(tmpbytebff[1] == 0xF2)
            {
                rxString = "turn command received\n";
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                tcmdRecived = true;
            }





            //if (tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x02 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) <= 60 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) != 0)
            //{
            //    rxString = "Obstacle middle  \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            //    RoboApp.ObstacleF = 1;
            
            //}

            //if (tmpbytebff2[1] == 0xE0 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) <= 80 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) != 0)
            //{
            //    rxString = "Obstacle  \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            //   // RoboApp.ObstacleF = 1;

            //}

            if (tmpbytebff[1] == 0xF3)
            {
                rxString = "stop command recived\n";
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                fstopRecived= true;
            
            }



            if(tmpbytebff[1] == 0xD3)
            {

                byte[] ar = new byte[2];
                ar[1] = tmpbytebff[4];
                ar[0] = tmpbytebff[5];


                int stopdistacne = bytetoint(ar);
                calcObstaclecord(RoboApp.currentx,RoboApp.currenty,RoboNav.currentdir,stopdistacne);
                rxString = "Stopped   "+stopdistacne+"\n";
                Array.Clear(tmpbytebff, 0, tmpbytebff.Length);
                RoboApp.Stopped = 1;

            }

            //if (tmpbytebff[1] == 0xE1 && tmpbytebff[3] == 0x03)
            //{
            //    rxString = "Halt Marker  \n";
            //    Array.Clear(tmpbytebff, 0, tmpbytebff.Length);

            //}

            //if (tmpbytebff[1] == 0xE1 && tmpbytebff[3] == 0x02)
            //{
            //    rxString = "Cordinate Marker  \n";
            //    Array.Clear(tmpbytebff, 0, tmpbytebff.Length);

            //}

            //if (tmpbytebff[1] == 0xE1 && tmpbytebff[3] == 0x01)
            //{
            //    rxString = "Section Divide marker  \n";
            //    Array.Clear(tmpbytebff, 0, tmpbytebff.Length);

            //}





            return rxString;
        }


        public int bytetointx(byte l,byte h)
        {
            byte[] ar=new byte[2];
            ar[0] = l;
            ar[1] = h;
            int i = BitConverter.ToInt16(ar, 0);
            return i;
        } 
        public string ReadData2 ()
        {

            int byteindcount = 0;
            byte tmpByte = 0x00;
            string rxString = "";
            if(RoboApp.emup2.BytesToRead > 0)
            {
                try
                {
                    //tmpByte = (byte)SP.ReadByte();

                    while(tmpByte != 255)
                    {
                        tmpByte = (byte)RoboApp.emup2.ReadByte();
                        tmpbytebff2[byteindcount] = tmpByte;
                       // rxString += ((char)tmpByte);

                        byteindcount++;
                    }
                }
                catch(Exception tt)
                {

                }

            }
            else
            {
                //rxString = "no bytes";
            }


            if (tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x02 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) <= 100 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) > 20)
            {
                rxString = "Obstacle middle  \n";
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
                middleobs = true;
                
                RoboApp.ObstacleF = 1;
                
            }

            if(tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x02 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) > 100)
            {
                
                //rxString = " No Obstacle middle  \n";
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
                middleobs = false;
                //RoboApp.ObstacleF = 1;
                
            }








            if(tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x03 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) <= 100 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) != 0)
            {
                rxString = "Obstacle left \n";
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
                // RoboApp.ObstacleF = 1;

            }
            if((tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x03 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) > 100) || (tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x03 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) ==0))
            {
                //rxString = "Left Clear\n";
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
                RoboApp.obsdir = -90;
                // RoboApp.ObstacleF = 1;

            }







            if(tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x01 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) <= 100 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) != 0)
            {
                rxString = "Obstacle right \n";
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
                rightobs = true;
                // RoboApp.ObstacleF = 1;

            }
            if((tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x01 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) > 100) || (tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x01 && bytetointx(tmpbytebff2[5], tmpbytebff2[4]) ==0))
            {
               
               // rxString = "Right Clear \n";
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
                RoboApp.obsdir = 90;
                rightobs = false;
                
                // RoboApp.ObstacleF = 1;

            }












          

            if(tmpbytebff2[1] == 0xE2)
            {

              
                if(RoboNav.currendHd != bytetointx(tmpbytebff2[4], tmpbytebff2[3]))
                {
                    rxString = "\nheading " + bytetointx(tmpbytebff2[4], tmpbytebff2[3]) + "\n";
                    RoboNav.currendHd = bytetointx(tmpbytebff2[4], tmpbytebff2[3]);
                }


                if(bytetointx(tmpbytebff2[4], tmpbytebff2[3]) - RoboApp.offset > 0 )
                {
                    RoboNav.currentdir = bytetointx(tmpbytebff2[4], tmpbytebff2[3])-RoboApp.offset;
                }
                else
                {
                    RoboNav.currentdir = 360 - Math.Abs(bytetointx(tmpbytebff2[4], tmpbytebff2[3]) - RoboApp.offset);
                }

                Console.WriteLine("Direction from Compas after Offset   "+RoboNav.currentdir);
                Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            }
            //if(tmpbytebff2[1] == 0xE0 && tmpbytebff2[3]==0x02)
            //{
            //    rxString = "Obstacle in front \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            //    RoboApp.ObstacleF = 1;

            //}


            //if(tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x01)
            //{
            //    rxString = "Obstacle in left \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            //    //RoboApp.ObstacleF = 1;

            //}

            //if(tmpbytebff2[1] == 0xE0 && tmpbytebff2[3] == 0x03)
            //{
            //    rxString = "Obstacle in right \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            //    //RoboApp.ObstacleF = 1;

            //}


            //if(tmpbytebff2[1] == 0xE3)
            //{

            //    byte[] ar = new byte[2];
            //    ar[1] = tmpbytebff2[4];
            //    ar[0] = tmpbytebff2[5];

            //    int stopdistacne = bytetoint(ar);
            //    calcObstaclecord(RoboApp.currentx, RoboApp.currenty, RoboNav.currentdir, stopdistacne);
            //    rxString = "Stopped   " + stopdistacne + "\n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);
            //    RoboApp.Stopped = 1;

            //}

            //if(tmpbytebff2[1] == 0xE1 && tmpbytebff2[3] == 0x03)
            //{
            //    rxString = "Halt Marker  \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);

            //}

            //if(tmpbytebff2[1] == 0xE1 && tmpbytebff2[3] == 0x02)
            //{
            //    rxString = "Cordinate Marker  \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);

            //}

            //if(tmpbytebff2[1] == 0xE1 && tmpbytebff2[3] == 0x01)
            //{
            //    rxString = "Section Divide marker  \n";
            //    Array.Clear(tmpbytebff2, 0, tmpbytebff2.Length);

            //}





            return rxString;
        }



        public void port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {


            while (RoboApp.emup.BytesToRead > 0)
            {
                bBuffer.Add((byte)RoboApp.emup.ReadByte());
            }
            ProcessBuffer(bBuffer);

        }
        public void calcObstaclecord (int x1, int y1, int dir, int dist)
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

            Console.WriteLine("x2 and y2   " + x2 + "," + y2);
            //if(y2 < 0) { y2 = 0; }
            //if(x2 < 0) { x2 = 0; }



            Console.WriteLine(RoboApp.currentx + "," + RoboApp.currenty + " direc " + tandir + "distance  " + dist);
            RoboApp.stoppedx = Convert.ToInt32(Math.Abs(x2));
            RoboApp.stoppedy = Convert.ToInt32(Math.Abs(y2));
            Console.WriteLine("Cordinate whre obstcle ws found");
            Console.WriteLine(RoboApp.stoppedx + "," + RoboApp.stoppedy + " direc " + tandir + "distance  " + dist);
        }

        private int bytetoint (byte[] ar)
        {


            int i = BitConverter.ToInt16(ar, 0);
            
            //MessageBox.Show(i.ToString());
            //SetText(i.ToString() + "\n");
            return i;
        }


        private void ProcessBuffer(List<byte> bBuffer)
        {
            // Look in the byte array for useful information
            // then remove the useful data from the buffer
            //MessageBox.Show(bBuffer.Count.ToString());

            if (bBuffer.Count == 8)
            {

                //ar[1] = bBuffer.ElementAt<byte>(4);
                //ar[0] = bBuffer.ElementAt<byte>(5);

                //bytetoint(ar);

                //dire[0] = bBuffer.ElementAt<byte>(6);
                //bytetodire(dire);

                //simucommadns();

               // bBuffer.Clear();

            }

            if (bBuffer.Count > 10)
            {
                bBuffer.Clear();

            }

        }








        public void readx()
        {
            //SP.ReadExisting();
        }

        public void writex(byte[,] com ,int i)
        {
            byte[] bytestowrite = new byte[8];
            bytestowrite[0] = com[i, 0];
            bytestowrite[1] = com[i, 1];
            bytestowrite[2] = com[i, 2];
            bytestowrite[3] = com[i, 3];
            bytestowrite[4] = com[i, 4];
            bytestowrite[5] = com[i, 5];
            bytestowrite[6] = com[i, 6];
            bytestowrite[7] = com[i, 7];



            RoboApp.emup.Write(bytestowrite, 0, bytestowrite.Length);
            RoboApp.emup.DiscardOutBuffer();
           

        }


        public string writetest(byte[] temp)
        {
            try
            {
                RoboApp.emup.Write(temp, 0, temp.Length);
                return "";
            }catch(Exception write)
            {
                return "could not send";
            }
        }
    
    }
}
