using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboAppMono
{
    public class Commands
    {

        int _id;
        double _xCodinate;
        double _yCodinate;
        double _speed;
        double _bateryCondition;
        bool _obstaclse;
        string _command;



        public Commands (int id, string command)
        {
            this._id = id;
            this._command = command;
        }

        public Commands (int id, string command, double xCod, double yCod, double speed, bool obstacle)
        {
            this._id = id;
            this._command = command;
            this._xCodinate = xCod;
            this._yCodinate = yCod;
            this._speed = speed;
            this._obstaclse = obstacle;
        }


        public int Id { get { return _id; } }
        public double XCodinate { get { return _xCodinate; } }
        public double YCodinate { get { return _yCodinate; } }
        public double Speed { get { return _speed; } }
        public double BateryCondition { get { return _bateryCondition; } }
        public bool Obstaclse { get { return _obstaclse; } }
        public string Command { get { return _command; } }

    }
}
