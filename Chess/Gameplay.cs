using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace Chess
{
    public class Gameplay
    {
        //variable declarations and accessor methods
        private chessColour _turn;
        public chessColour Turn
        {
            get { return _turn; }
            set 
            { 
                _turn = value;
            }
        }

        private Stopwatch _blackSW = new Stopwatch();
        private Stopwatch _whiteSW = new Stopwatch();

        //constructor
        public Gameplay()
        {
            reset();
        }

        public void reset()
        {
            _turn = chessColour.WHITE;      //Default player is white
        }

        //Handles the ending of the current player's turn
        public void changeTurns()
        {
            if (_turn == chessColour.BLACK)
            {
                _blackSW.Stop();
                _turn = chessColour.WHITE;
                _whiteSW.Start();
            }
            else
            {
                _whiteSW.Stop();
                _turn = chessColour.BLACK;
                _blackSW.Start();
            }
        }

        public string getTime(chessColour colour)
        {
            TimeSpan ts;
            if (colour == chessColour.BLACK)
            {
                ts = _blackSW.Elapsed;
            }
            else
            {
                ts = _whiteSW.Elapsed;
            }

            String retVal = String.Format("{0:00}:{1:00}:{2:00}",ts.Hours, ts.Minutes, ts.Seconds);
            return retVal;
        }
    }
}
