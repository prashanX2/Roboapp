using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboAppMono
{
    class RoboCom
    {
        public SPCom serialtty = new SPCom();


        public void sendCommand(byte[,] com, int i)
        {
            serialtty.writex(com, i);
        }


        public void reply()
        {
            //serialtty.readx();
            serialtty.ReadData();
        }



    }
}
