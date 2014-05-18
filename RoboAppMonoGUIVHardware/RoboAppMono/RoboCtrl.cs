using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboAppMono
{
    class RoboCtrl
    {
        public RoboCom Com = new RoboCom();
        public RoboNav Nav = new RoboNav();

        public void startpathcalc()
        {
            Nav.pathcompute();
        }

        public void goForward(int ind)
        {

            Nav.makeCommand(ind);
            //cout<<Nav.com[0];

            //
            Com.sendCommand(Nav.commands, ind);

        }

        public void goForward()
        {
            for (int i = 0; i < Nav.pathx.xdcords.Count; i++)
            {
                Nav.makeCommand(i);
                //cout<<Nav.com[0];

                //
                Com.sendCommand(Nav.commands, i);
                Com.reply();


            }
        }


    }
}
