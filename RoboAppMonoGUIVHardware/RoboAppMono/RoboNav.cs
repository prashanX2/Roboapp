using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboAppMono
{
    class RoboNav
    {
        public RoboPathCalc pathx = new RoboPathCalc();

        public static int currentdir;
        public static int currendHd = 0;
        //public static int currentx;
        //public static int currenty;
        public byte[] comchar = new byte[8];
        public byte[,] commands = new byte[1000, 8];

        public List<int> xopscords = new List<int>();
        public List<int> yopscords = new List<int>();
        public List<int> xopdcords = new List<int>();
        public List<int> yopdcords = new List<int>();
        public List<string> opdisplay = new List<string>();


        public void pathcompute()
        {

            pathx.calc();
            optimizepath();

        }


        public void makeCommand(int i)
        {
            byte dire = 0x00;
            //commands = new byte[pathx.xscords.Count,8];

            double x1 = (double)pathx.xscords[i];
            double x2 = (double)pathx.xdcords[i];

            double y1 = (double)pathx.yscords[i];
            double y2 = (double)pathx.ydcords[i];

            double dtsq = Math.Abs((x2 - x1) * (x2 - x1) - (y2 - y1) * (y2 - y1));


            int dist = Convert.ToInt16(Math.Abs(Math.Sqrt(dtsq)) * 2);
            //cout<<dist<<endl;


            //memcpy(ch,(char*)&dist,2);
            byte[] ch = BitConverter.GetBytes(dist);

            if (pathx.xscords[i] < pathx.xdcords[i] && pathx.yscords[i] == pathx.ydcords[i])
            {

                dire = 0x45;
            }


            if (pathx.xscords[i] > pathx.xdcords[i] && pathx.yscords[i] == pathx.ydcords[i])
            {

                dire = 0x57;
            }

            if (pathx.xscords[i] == pathx.xdcords[i] && pathx.yscords[i] < pathx.ydcords[i])
            {

                dire = 0x53;
            }

            if (pathx.xscords[i] == pathx.xdcords[i] && pathx.yscords[i] > pathx.ydcords[i])
            {

                dire = 0x4E;
            }




            //com={0x01,0xC1,0x04,0x11,0x10,0x10,0xFF};

            commands[i, 0] = 0x01;
            commands[i, 1] = 0xC1;
            commands[i, 2] = 0x04;
            commands[i, 3] = 0x11;
            commands[i, 4] = ch[1];
            commands[i, 5] = ch[0];
            commands[i, 6] = dire;
            commands[i, 7] = 0xFF;

            //commands[i]=*comchar;




        }
        public void optimizepath()
        {
            int startx = 0;
            int starty = 0;
            int corner1x = 0;
            int corner1y = 0;
            int corner2x = 0;
            int corner2y = 0;
            int abstartx = 0;
            int abstarty = 0;
            int cordpathcnt = 0;
            bool corner1set = false;
            bool corner2set = false;
            abstartx = pathx.xscords[0];
            abstarty = pathx.yscords[0];
            bool valadded = false;
            startx = abstartx;
            starty = abstarty;
            bool last = false;
            int lastx = 0;
            int lasty = 0;

            for (int i = 0; i < pathx.xdcords.Count; i++)
            {

                if (cordpathcnt <= i)
                {


                    for (int j = cordpathcnt; j < pathx.xdcords.Count; j++, cordpathcnt++)
                    {
                        Console.WriteLine(cordpathcnt + "   pathcount   " + pathx.xdcords.Count);
                        if (pathx.xdcords[j] - startx == 0 || pathx.ydcords[j] - starty == 0)
                        {
                            Console.WriteLine("straight way");

                        }


                        else
                        {
                            if (corner1set == false)
                            {
                                corner1x = pathx.xdcords[j - 1];
                                corner1y = pathx.ydcords[j - 1];
                                corner1set = true;
                                startx = corner1x;
                                starty = corner1y;
                                Console.WriteLine("********crner1   " + corner1x + "," + corner1y);
                            }

                            else
                            {
                                corner2x = pathx.xdcords[j - 1];
                                corner2y = pathx.ydcords[j - 1];
                                corner2set = true;
                                corner1set = false;
                                Console.WriteLine("----------CORNER 2   " + corner2x + "," + corner2y);
                                startx = pathx.xdcords[j - 1];
                                starty = pathx.ydcords[j - 1];
                                break;
                            }

                            Console.WriteLine("in else");
                        }

                        if (cordpathcnt == pathx.xdcords.Count - 1 && corner1set == true)
                        {

                            Console.WriteLine("last ");
                            last = true;
                            lastx = pathx.xdcords[j];
                            lasty = pathx.ydcords[j];
                        }

                    }
                }


                if (last == true && corner2set == true && calcobsticleoppath(corner2x, corner2y, lastx, lasty) == false)
                {

                    Console.WriteLine("LLLLLLLLLLLast BY CORNER 2");
                    xopscords.Add(corner2x);
                    yopscords.Add(corner2y);
                    xopdcords.Add(lastx);
                    yopdcords.Add(lasty);
                    last = false;
                    corner2set = false;
                    lastx = 0;
                    lasty = 0;
                    break;

                }
                if (last == true && corner1set == true && calcobsticleoppath(abstartx, abstarty, lastx, lasty) == false)
                {
                    Console.WriteLine("LLLLLLLLLLLast");
                    xopscords.Add(abstartx);
                    yopscords.Add(abstarty);
                    xopdcords.Add(lastx);
                    yopdcords.Add(lasty);
                    last = false;
                    lastx = 0;
                    lasty = 0;
                    break;

                }

                if (corner2set == true && calcobsticleoppath(abstartx, abstarty, corner2x, corner2y) == false)
                {

                    xopscords.Add(abstartx);
                    yopscords.Add(abstarty);
                    xopdcords.Add(corner2x);
                    yopdcords.Add(corner2y);
                    Console.WriteLine("optimize added");
                    valadded = true;
                    abstartx = corner2x;
                    abstarty = corner2y;
                    startx = abstartx;
                    starty = abstarty;
                    corner2x = 0;
                    corner2y = 0;
                    corner1x = 0;
                    corner1y = 0;
                    corner1set = false;
                    corner2set = false;
                    i = cordpathcnt;

                }







                else
                {
                    xopscords.Add(pathx.xscords[i]);
                    yopscords.Add(pathx.yscords[i]);
                    xopdcords.Add(pathx.xdcords[i]);
                    yopdcords.Add(pathx.ydcords[i]);
                    Console.WriteLine("XXXXXXXXXXXXXXX " + pathx.xscords[i] + "," + pathx.yscords[i] + "------" + pathx.xdcords[i] + "," + pathx.ydcords[i]);

                }

                valadded = false;




            }
            addtodisplay();
        }


        public static bool calcobsticleoppath (int x1, int y1, int x2, int y2)
        {
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


                    if(Data.bmatrix[i, (int)y] == true)
                    {
                        obs = true;
                        Console.WriteLine("obsticle");
                        return obs;


                    }

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

                    if(Data.bmatrix[(int)x, i] == true)
                    {
                        obs = true;
                        Console.WriteLine("obsticle");
                        return obs;


                    }
                }

            }
            else if(Math.Abs(x2 - x1) >= Math.Abs(y2 - y1) && x1 > x2)
            {
                for(int i = x1; i >= x2; i--)
                {

                    y = m * i + c;

                    if(Data.bmatrix[i, (int)y] == true)
                    {
                        obs = true;
                        Console.WriteLine("obsticle");
                        return obs;


                    }
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

                    if(Data.bmatrix[(int)x, i] == true)
                    {
                        obs = true;
                        Console.WriteLine("obsticle");
                        return obs;


                    }
                }

            }
            Console.WriteLine("*****@@no Obstacle@@****");
            return obs;
        }

        public void addtodisplay()
        {
            for (int i = 0; i < xopdcords.Count; i++)
            {


                opdisplay.Add(xopscords[i].ToString() + "," + yopscords[i].ToString() + "--------->" + xopdcords[i].ToString() + "," + yopdcords[i].ToString());
            }

        }



        public byte[] makecomtest(int i)
        {



            byte[] temp = new byte[9];
            byte dire = 0x00;
            //commands = new byte[pathx.xscords.Count,8];

            int x1 = xopscords[i];
            int x2 = xopdcords[i];

            int y1 = yopscords[i];
            int y2 = yopdcords[i];
            //Console.WriteLine(x1+","+y1+"----->"+x2+","+y2);

            int p = x2 - x1;
            int q = y2 - y1;
            int r = p > 0 ? p : checked(-p);
            int s = q > 0 ? q : checked(-q);

            int dtsq = (r * r) + (s * s);
            int X = dtsq;
            Console.WriteLine("dist in pixel " + X);

            int d = X > 0 ? X : checked(-X);

            double distsr = RoboNav.Sqrtandabs(d);


            Console.WriteLine("sqr root  " + distsr);


            int dist = (int)(distsr * 2);
            //cout<<dist<<endl;

            Console.WriteLine(dist);
            //memcpy(ch,(char*)&dist,2);
            byte[] ch = BitConverter.GetBytes(dist);

            // Console.WriteLine(x1+","+y1+"    "+x2+","+y2);
            double dx1 = (double)x1;
            double dy1 = (double)y1;
            double dx2 = (double)x2;
            double dy2 = (double)y2;
            double tann = 0;
            if (x2 - x1 != 0)
            {
                tann = (dy2 - dy1) / (dx2 - dx1);
                //Console.WriteLine("tan  "+tann);
            }
            int tanint = 0;

            if (tann != 0)
            {
                double tandig = Math.Atan(tann) * 180 / 3.1415;
                //Console.WriteLine("tan dig  "+tandig);
                tanint = (int)tandig;
                Console.WriteLine("tan int " + tanint);
            }
            int tand = tanint > 0 ? tanint : checked(-tanint);
            //Console.WriteLine(tand);
            if (x2 - x1 == 0 && y2 - y1 < 0) { tand = 0; }
            if (x2 - x1 == 0 && y2 - y1 > 0) { tand = 180; }
            if (x2 - x1 < 0 && y2 - y1 == 0) { tand = 270; }
            if (x2 - x1 > 0 && y2 - y1 == 0) { tand = 90; }
            if (x2 - x1 > 0 && y2 - y1 > 0) { tand += 90; }
            if (x2 - x1 < 0 && y2 - y1 > 0) { tand += 180; }
            if (x2 - x1 < 0 && y2 - y1 < 0) { tand += 270; }
            if (x2 - x1 > 0 && y2 - y1 < 0) { tand =90-tand; }

            Console.WriteLine(tand);
            byte[] dir = BitConverter.GetBytes(tand);





            //com={0x01,0xC1,0x04,0x11,0x10,0x10,0xFF};

            temp[0] = 0x01;
            temp[1] = 0xC1;
            temp[2] = 0x04;
            temp[3] = 0x00;
            temp[4] = ch[1];
            temp[5] = ch[0];
            temp[6] = 0x0A;
            temp[7] = 0xFF;
            temp[8] = 0xFF;
            //temp[6] = dir[1];
            //temp[7] = dir[0];
            

            RoboApp.currentx = x1;
            RoboApp.currenty = y1;
            //RoboNav.currentdir = tand;
            return temp;
        }


        public static  byte[] makecomtest2 (int ix1,int iy1,int ix2,int iy2)
        {



            byte[] temp = new byte[9];
            byte dire = 0x00;
            //commands = new byte[pathx.xscords.Count,8];

            int x1 = ix1;
            int x2 = ix2;

            int y1 = iy1;
            int y2 = iy2;
            //Console.WriteLine(x1+","+y1+"----->"+x2+","+y2);

            int p = x2 - x1;
            int q = y2 - y1;
            int r = p > 0 ? p : checked(-p);
            int s = q > 0 ? q : checked(-q);

            int dtsq = (r * r) + (s * s);
            int X = dtsq;
            Console.WriteLine("dist in pixel " + X);

            int d = X > 0 ? X : checked(-X);

            double distsr = RoboNav.Sqrtandabs(d);


            Console.WriteLine("sqr root  " + distsr);


            int dist = (int)(distsr * 2);
            //cout<<dist<<endl;

            Console.WriteLine(dist);
            //memcpy(ch,(char*)&dist,2);
            byte[] ch = BitConverter.GetBytes(dist);

            // Console.WriteLine(x1+","+y1+"    "+x2+","+y2);
            double dx1 = (double)x1;
            double dy1 = (double)y1;
            double dx2 = (double)x2;
            double dy2 = (double)y2;
            double tann = 0;
            if(x2 - x1 != 0)
            {
                tann = (dy2 - dy1) / (dx2 - dx1);
                //Console.WriteLine("tan  "+tann);
            }
            int tanint = 0;

            if(tann != 0)
            {
                double tandig = Math.Atan(tann) * 180 / 3.1415;
                //Console.WriteLine("tan dig  "+tandig);
                tanint = (int)tandig;
                Console.WriteLine("tan int " + tanint);
            }
            int tand = tanint > 0 ? tanint : checked(-tanint);
            //Console.WriteLine(tand);
            if(x2 - x1 == 0 && y2 - y1 < 0) { tand = 0; }
            if(x2 - x1 == 0 && y2 - y1 > 0) { tand = 180; }
            if(x2 - x1 < 0 && y2 - y1 == 0) { tand = 270; }
            if(x2 - x1 > 0 && y2 - y1 == 0) { tand = 90; }
            if(x2 - x1 > 0 && y2 - y1 > 0) { tand += 90; }
            if(x2 - x1 < 0 && y2 - y1 > 0) { tand += 180; }
            if(x2 - x1 < 0 && y2 - y1 < 0) { tand += 270; }
            if(x2 - x1 > 0 && y2 - y1 < 0) { tand = 90 - tand; }

            Console.WriteLine(tand);
            byte[] dir = BitConverter.GetBytes(tand);





            //com={0x01,0xC1,0x04,0x11,0x10,0x10,0xFF};

            temp[0] = 0x01;
            temp[1] = 0xC1;
            temp[2] = 0x04;
            temp[3] = 0x00;
            temp[4] = ch[1];
            temp[5] = ch[0];
            temp[6] = 0x0A;
            temp[7] = 0xFF;
            temp[8] = 0xFF;
            //temp[6] = dir[1];
            //temp[7] = dir[0];


            RoboApp.currentx = x1;
            RoboApp.currenty = y1;
            //RoboNav.currentdir = tand;
            return temp;
        }


        public byte[] makecomtestDir (int i)
        {



            byte[] temp = new byte[12];
            byte dire = 0x00;
            //commands = new byte[pathx.xscords.Count,8];

            int x1 = xopscords[i];
            int x2 = xopdcords[i];

            int y1 = yopscords[i];
            int y2 = yopdcords[i];
            //Console.WriteLine(x1+","+y1+"----->"+x2+","+y2);

            int p = x2 - x1;
            int q = y2 - y1;
            int r = p > 0 ? p : checked(-p);
            int s = q > 0 ? q : checked(-q);

            int dtsq = (r * r) + (s * s);
            int X = dtsq;
            Console.WriteLine("dist in pixel " + X);

            int d = X > 0 ? X : checked(-X);

            double distsr = RoboNav.Sqrtandabs(d);


            Console.WriteLine("sqr root  " + distsr);


            int dist = (int)(distsr * 2);
            //cout<<dist<<endl;

            Console.WriteLine(dist);
            //memcpy(ch,(char*)&dist,2);
            byte[] ch = BitConverter.GetBytes(dist);

            // Console.WriteLine(x1+","+y1+"    "+x2+","+y2);
            double dx1 = (double)x1;
            double dy1 = (double)y1;
            double dx2 = (double)x2;
            double dy2 = (double)y2;
            double tann = 0;
            if(x2 - x1 != 0)
            {
                tann = (dy2 - dy1) / (dx2 - dx1);
                //Console.WriteLine("tan  "+tann);
            }
            int tanint = 0;

            if(tann != 0)
            {
                double tandig = Math.Atan(tann) * 180 / 3.1415;
                //Console.WriteLine("tan dig  "+tandig);
                tanint = (int)tandig;
                Console.WriteLine("tan int " + tanint);
            }
            int tand = tanint > 0 ? tanint : checked(-tanint);
            //Console.WriteLine(tand);
            if(x2 - x1 == 0 && y2 - y1 < 0) { tand = 0; }
            if(x2 - x1 == 0 && y2 - y1 > 0) { tand = 180; }
            if(x2 - x1 < 0 && y2 - y1 == 0) { tand = 270; }
            if(x2 - x1 > 0 && y2 - y1 == 0) { tand = 90; }
            if(x2 - x1 > 0 && y2 - y1 > 0) { tand += 90; }
            if(x2 - x1 < 0 && y2 - y1 > 0) { tand += 180; }
            if(x2 - x1 < 0 && y2 - y1 < 0) { tand += 270; }
            if(x2 - x1 > 0 && y2 - y1 < 0) { tand = 90 - tand; }

            Console.WriteLine(tand);
            byte turn = 0x00;
            int temptand = 0;
            temptand = tand;
            string turnn = "";
           
                
                if(Math.Abs(RoboNav.currentdir - tand) < 180)
                {
                    if(RoboNav.currentdir <= tand)
                    {
                        turn = 0x01;//right
                        turnn = "right";
                    }
                    else
                    {
                        turn = 0x00;//left
                         turnn = "left";
                    }
                    
                    tand = Math.Abs(RoboNav.currentdir - tand);
                }
                else
                {
                    if(RoboNav.currentdir <= tand)
                    {
                        turn = 0x00;//left
                         turnn = "left";
                    }
                    else
                    {
                        turn = 0x01;//right
                         turnn = "right";
                    }
                    tand = 360 - Math.Abs(RoboNav.currentdir - tand);

                }



                Console.WriteLine("Current dir final Direction and degrees sent to RObo = "+RoboNav.currentdir+"   " + turnn + "  "+ tand  );
            byte[] dir = BitConverter.GetBytes(tand);


            


            //com={0x01,0xC1,0x04,0x11,0x10,0x10,0xFF};

            temp[0] = 0x01;
            temp[1] = 0xC2;
            temp[2] = 0x07;
            temp[3] = turn;
            temp[4] = 0x00;
            temp[5] = dir[1];
            temp[6] = dir[0];
            temp[7] = 0x0A;
            temp[8] = 0x00;
            temp[9] = 0x00;
            temp[10] = 0xFF;
            temp[11] = 0xFF;
            //temp[6] = dir[1];
            //temp[7] = dir[0];



            //RoboNav.currentdir = temptand;
            return temp;
        }


        public static  byte[] makecomtestDir2 (int ix1,int iy1,int ix2,int iy2,int cdir)
        {



            byte[] temp = new byte[12];
            byte dire = 0x00;
            //commands = new byte[pathx.xscords.Count,8];

            int x1 = ix1;
            int x2 = ix2;

            int y1 = iy1;
            int y2 = iy2;
            //Console.WriteLine(x1+","+y1+"----->"+x2+","+y2);

            int p = x2 - x1;
            int q = y2 - y1;
            int r = p > 0 ? p : checked(-p);
            int s = q > 0 ? q : checked(-q);

            int dtsq = (r * r) + (s * s);
            int X = dtsq;
            Console.WriteLine("dist in pixel " + X);

            int d = X > 0 ? X : checked(-X);

            double distsr = RoboNav.Sqrtandabs(d);


            Console.WriteLine("sqr root  " + distsr);


            int dist = (int)(distsr * 2);
            //cout<<dist<<endl;

            Console.WriteLine(dist);
            //memcpy(ch,(char*)&dist,2);
            byte[] ch = BitConverter.GetBytes(dist);

            // Console.WriteLine(x1+","+y1+"    "+x2+","+y2);
            double dx1 = (double)x1;
            double dy1 = (double)y1;
            double dx2 = (double)x2;
            double dy2 = (double)y2;
            double tann = 0;
            if(x2 - x1 != 0)
            {
                tann = (dy2 - dy1) / (dx2 - dx1);
                //Console.WriteLine("tan  "+tann);
            }
            int tanint = 0;

            if(tann != 0)
            {
                double tandig = Math.Atan(tann) * 180 / 3.1415;
                //Console.WriteLine("tan dig  "+tandig);
                tanint = (int)tandig;
                Console.WriteLine("tan int " + tanint);
            }
            int tand = tanint > 0 ? tanint : checked(-tanint);
            //Console.WriteLine(tand);
            if(x2 - x1 == 0 && y2 - y1 < 0) { tand = 0; }
            if(x2 - x1 == 0 && y2 - y1 > 0) { tand = 180; }
            if(x2 - x1 < 0 && y2 - y1 == 0) { tand = 270; }
            if(x2 - x1 > 0 && y2 - y1 == 0) { tand = 90; }
            if(x2 - x1 > 0 && y2 - y1 > 0) { tand += 90; }
            if(x2 - x1 < 0 && y2 - y1 > 0) { tand += 180; }
            if(x2 - x1 < 0 && y2 - y1 < 0) { tand += 270; }
            if(x2 - x1 > 0 && y2 - y1 < 0) { tand = 90 - tand; }

            Console.WriteLine(tand);
            byte turn = 0x00;
            int temptand = 0;
            temptand = tand;

            if(RoboNav.currentdir - tand < 0)
            {
                turn = 0x01;
                tand = Math.Abs(RoboNav.currentdir - tand);
            }
            else
            {
                turn = 0x00;
                tand = Math.Abs(RoboNav.currentdir - tand);
            }

            if(cdir < 0)
            {
                turn = 0x01;
            }
            else
            {
                turn = 0x00;
            }


            byte[] dir = BitConverter.GetBytes(Math.Abs(cdir));





            //com={0x01,0xC1,0x04,0x11,0x10,0x10,0xFF};

            temp[0] = 0x01;
            temp[1] = 0xC2;
            temp[2] = 0x07;
            temp[3] = turn;
            temp[4] = 0x00;
            temp[5] = dir[1];
            temp[6] = dir[0];
            temp[7] = 0x0A;
            temp[8] = 0x00;
            temp[9] = 0x00;
            temp[10] = 0xFF;
            temp[11] = 0xFF;
            //temp[6] = dir[1];
            //temp[7] = dir[0];



            //RoboNav.currentdir = temptand;
            return temp;
        }

        public static double Sqrtandabs(int number)
        {

            double x = number / 2;

            for (int i = 0; i < 100; i++) x = (x + number / x) / 2d;

            return x;

        }
    }
}
