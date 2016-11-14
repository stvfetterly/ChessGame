using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Chess
{
    class Gameplay
    {
        //variable declarations and accessor methods
        private chessColour _turn;
        public chessColour Turn
        {
            get { return _turn; }
            set { _turn = value;}
        }

        private Timer _chessTimer = new Timer();

        private int _whiteTime = 0;
        public int WhiteTime
        {
            get { return _whiteTime; }
            set { _whiteTime = value;}
        }
        private int _blackTime = 0;
        public int BlackTime
        {
            get { return _blackTime; }
            set { _blackTime = value;}
        }

        //constructor
        public Gameplay()
        {
            reset();
        }

        public void reset()
        {
            _turn = chessColour.WHITE;      //Default player is white
            _chessTimer.Interval = 1000;    //Ticks every second
            _chessTimer.Start();
            _chessTimer.Elapsed += new ElapsedEventHandler(chessTick);
            _whiteTime = 0;
            _blackTime = 0;
        }

        //Handles the ending of the current player's turn
        public void changeTurns()
        {
            if (_turn == chessColour.BLACK)
            {
                _turn = chessColour.WHITE;
            }
            else
            {
                _turn = chessColour.BLACK;
            }
        }

        //Called by chess timer every second
        private void chessTick(object source, ElapsedEventArgs e)
        {
             if (_turn == chessColour.BLACK)
            {
                _blackTime += 1;
            }
            else
            {
                _whiteTime += 1;
            }
        }
    }
}
