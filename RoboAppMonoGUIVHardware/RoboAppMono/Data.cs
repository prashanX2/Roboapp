using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoboAppMono
{
    class Data
    {
        public static  int[,] weight = new int[RoboPathCalc.MAXNODES, RoboPathCalc.MAXNODES];
        public static bool[,] bmatrix = new bool[4001, 4001];
        public static  int[] param = new int[4];

        public static int parmcount = 0;




        public  static void readbmatrix ()
        {
            string aline;
            int blcount = 0;
            int bscount = 0;
            try
            {
                //StreamReader file = new StreamReader("bmatrix.txt");


                //while ((aline = file.ReadLine()) != null)
                //{
                //    blcount++;
                //    string[] value = aline.Split(' ');
                //    bscount=value.Length;
                //}

                //bmatrix = new bool[4001, 4001];
                StreamReader fileb = new StreamReader("bmatrix.txt");
                blcount = 0;
                string aaline;

                while((aaline = fileb.ReadLine()) != null)
                {


                    string[] value = aaline.Split(' ');

                    for(int i = 0; i < value.Length; i++)
                    {
                        if(value[i].Equals("1"))
                        {
                            Data.bmatrix[i, blcount] = true;

                        }
                        else
                        {
                            Data.bmatrix[i, blcount] = false;
                        }
                    }

                    blcount++;

                }
                fileb.Close();
            }
            catch(IOException x)
            {
                Console.WriteLine(x);
            }
            //RoboApp.Genstring="Reading of bit matrix done";
            //genstring = "Reading of bit matrix done";

        }

        public static  void readAJMatrix ()
        {
            string aline;

            try
            {
                StreamReader file = new StreamReader("adj_matrix.txt");
                while((aline = file.ReadLine()) != null)
                {

                    string[] value = aline.Split(' ');
                    //ajmatrix.Add(Convert.ToInt32(value[0]));
                    //ajmatrix.Add(Convert.ToInt32(value[1]));
                    weight[Convert.ToInt32(value[0]), Convert.ToInt32(value[1])] = 1;

                }
                file.Close();
            }
            catch(IOException x)
            {
                Console.WriteLine(x);
            }

           // genstring = "Reading of adjecency matrix done";
        }


        public static  void readParam ()
        {
            string pline;

            try
            {
                StreamReader pfile = new StreamReader("param.txt");

                while((pline = pfile.ReadLine()) != null)
                {

                    param[parmcount] = Convert.ToInt32(pline);
                    parmcount++;
                }
                pfile.Close();
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex);
            }
            //genstring = "Reading of parameters done";
        }

    }
}
