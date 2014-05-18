using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RoboAppMono;

namespace RoboAppMono
{
    class RoboPathCalc
    {

        #region variables

        //public int[] param = new int[4];
        public List<int> ajmatrix = new List<int>();
        //public static bool[,] bmatrix;
        public static string genstring = "";
        public const int MAXNODES = 6500;
        public const int MAX1 = 500;
        public const int INFINITY = 50000;

       // public int[,] weight = new int[MAXNODES, MAXNODES];
        
        public int i, j;
        public int[] distancex = new int[MAXNODES];
        public int[] visit = new int[MAXNODES];
        public int[] precede = new int[MAXNODES];
        public int final = 0;
        public int[] path = new int[MAX1];
        public int smalldist, newdist, k, s, d, current,  distcurr;
        public static int n=0;


        public List<int> xscords = new List<int>();
        public List<int> yscords = new List<int>();
        public List<int> xdcords = new List<int>();
        public List<int> ydcords = new List<int>();
        public List<string> cords = new List<string>();
        public List<string> displays = new List<string>();

        #endregion



        public int parmcount = 0;

        //public static void readbmatrix()
        //{
        //    string aline;
        //    int blcount = 0;
        //    int bscount = 0;
        //    try
        //    {
        //        //StreamReader file = new StreamReader("bmatrix.txt");


        //        //while ((aline = file.ReadLine()) != null)
        //        //{
        //        //    blcount++;
        //        //    string[] value = aline.Split(' ');
        //        //    bscount=value.Length;
        //        //}

        //        //bmatrix = new bool[4001, 4001];
        //        StreamReader fileb = new StreamReader("bmatrix.txt");
        //        blcount = 0;
        //        string aaline;

        //        while ((aaline = fileb.ReadLine()) != null)
        //        {


        //            string[] value = aaline.Split(' ');

        //            for (int i = 0; i < value.Length; i++)
        //            {
        //                if (value[i].Equals("1"))
        //                {
        //                    Data.bmatrix[i, blcount] = true;

        //                }
        //                else
        //                {
        //                    Data.bmatrix[i, blcount] = false;
        //                }
        //            }

        //            blcount++;

        //        }
        //        fileb.Close();
        //    }
        //    catch (IOException x)
        //    {
        //        Console.WriteLine(x);
        //    }
        //    //RoboApp.Genstring="Reading of bit matrix done";
        //    genstring = "Reading of bit matrix done";
            
        //}


        public static void printbmatrix()
        {
            for (int i = 0; i < Data.bmatrix.GetLength(0); i++)
            {
                for (int j = 0; j < Data.bmatrix.GetLength(1); j++)
                {
                    if (Data.bmatrix[i, j] == true) { Console.Write("1"); }
                    else { Console.Write("0"); }
                }
                Console.WriteLine();
            }
        }

        //public void readAJMatrix()
        //{
        //    string aline;

        //    try
        //    {
        //        StreamReader file = new StreamReader("adj_matrix.txt");
        //        while ((aline = file.ReadLine()) != null)
        //        {

        //            string[] value = aline.Split(' ');
        //            //ajmatrix.Add(Convert.ToInt32(value[0]));
        //            //ajmatrix.Add(Convert.ToInt32(value[1]));
        //            Data.weight[Convert.ToInt32(value[0]), Convert.ToInt32(value[1])] = 1;

        //        }
        //        file.Close();
        //    }
        //    catch (IOException x)
        //    {
        //        Console.WriteLine(x);
        //    }

        //    genstring="Reading of adjecency matrix done";
        //}

        public void adjCopy () {
        
        
        }

        //public void readParam()
        //{
        //    string pline;

        //    try
        //    {
        //        StreamReader pfile = new StreamReader("param.txt");

        //        while ((pline = pfile.ReadLine()) != null)
        //        {

        //            Data.param[parmcount] = Convert.ToInt32(pline);
        //            parmcount++;
        //        }
        //        pfile.Close();
        //    }
        //    catch (IOException ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    genstring="Reading of parameters done";
        //}



        public void calc()
        {
            //readParam();
            //readbmatrix();
            ////cout<<"\nEnter the number of nodes(Less than 50)in the matrix : ";
            //n = param[3];

            //for (i = 0; i < n; i++)
            //{
            //    for (j = 0; j < n; j++)
            //    {
            //        weight[i, j] = INFINITY;
            //    }
            //}

            //readAJMatrix();

            /*weight[0][1]=2;
            weight[1][0]=2;

            weight[6][1]=2;
            weight[1][6]=2;

            weight[6][5]=2;
            weight[5][6]=2;

            weight[6][7]=4;
            weight[7][6]=4;

            weight[7][3]=2;
            weight[3][7]=2;

            weight[7][9]=1;
            weight[9][7]=1;

            weight[7][8]=2;
            weight[8][7]=2;

            weight[3][7]=2;
            weight[7][3]=2;

            weight[3][2]=2;
            weight[2][3]=2;

            weight[3][4]=2;
            weight[4][3]=2;

            weight[8][14]=6;
            weight[14][8]=6;

            weight[13][14]=2;
            weight[14][13]=2;

            weight[13][12]=4;
            weight[12][13]=4;

            weight[13][11]=4;
            weight[11][13]=4;

            weight[10][11]=4;
            weight[11][10]=4;

            weight[10][12]=4;
            weight[12][10]=4;*/

            //cout << "\nEnter the source node ";

            //scanf_s("%d",&s);


            //printf("\nEnter the destination node (0 to %d) : ",n-1);

            //scanf_s("%d",&d);

            for (i = 0; i < n; i++)
            {
                distancex[i] = INFINITY;
                precede[i] = INFINITY;
            }
            distancex[s] = 0;
            current = s;
            visit[current] = 1;
            while (current != d)
            {
                //cout<<"calc loop"<<endl;
                distcurr = distancex[current];
                smalldist = INFINITY;
                for (i = 0; i < n; i++)
                    if (visit[i] == 0)
                    {
                        newdist = distcurr + Data.weight[current, i];
                        if (newdist < distancex[i])
                        {
                            distancex[i] = newdist;
                            precede[i] = current;
                        }
                        if (distancex[i] < smalldist)
                        {
                            smalldist = distancex[i];
                            k = i;
                        }
                    }
                current = k;
                visit[current] = 1;
            }
            //cout<<"calc done"<<endl;
            Display_Result();
            //printlist();
            //_getch();
        }

        public void Display_Result()
        {
            //cout<<"display"<<endl;
            i = d;
            path[final] = d;
            final++;
            while (precede[i] != s)
            {
                j = precede[i];
                i = j;
                path[final] = i;
                final++;
            }
            path[final] = s;

            //cout << "\nThe shortest path followed is :\n\n";
            for (i = final; i > 0; i--)
            {

                displays.Add(path[i].ToString() + "---->" + path[i - 1].ToString() + "with cost=" + Data.weight[path[i], path[i - 1]].ToString() + "   " + Convert.ToString((1 + (path[i] / Data.param[1])) * Data.param[0]) + "," + Convert.ToString((path[i] - (Data.param[1] * ((path[i] / Data.param[1])))) * Data.param[0]) + "   " + Convert.ToString((1 + (path[i - 1] / Data.param[1])) * Data.param[0]) + "," + Convert.ToString((path[i - 1] - (Data.param[1] * ((path[i - 1] / Data.param[1])))) * Data.param[0]) + "\n\n");



                xscords.Add((1 + (path[i] / Data.param[1])) * Data.param[0]);
                yscords.Add((path[i] - (Data.param[1] * ((path[i] / Data.param[1])))) * Data.param[0]);
                xdcords.Add((1 + (path[i - 1] / Data.param[1])) * Data.param[0]);
                ydcords.Add((path[i - 1] - (Data.param[1] * ((path[i - 1] / Data.param[1])))) * Data.param[0]);






                //printf("\nFor total cost = %d",distancex[d]);
            }
            displays.Add("\nFor total cost =" + distancex[d].ToString());
        }





    }
}
