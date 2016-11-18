using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Chess
{
    public partial class ChessClockWindow : Form
    {
        Chess.MainChessWindow _mainWindowHandle;
        chessColour activePlayer;

        public ChessClockWindow(Chess.MainChessWindow mainWindow)
        {
            _mainWindowHandle = mainWindow;

            InitializeComponent();
        }

        private void ChessClockWindow_Load(object sender, EventArgs e)
        {
            updateTime();
            refreshTimer.Start();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            updateTime();
        }

        private void updateTime()
        {
            //if the player has changed, update the main window with the player colour and the other player's time
            if (activePlayer != _mainWindowHandle.currentGame.Turn)
            {
                activePlayer = _mainWindowHandle.currentGame.Turn;

                if (activePlayer == chessColour.WHITE)
                {
                    label_turn.Text = "White Move:";
                    label_othTime.Text = "BLK - " + _mainWindowHandle.currentGame.getTime(chessColour.BLACK);
                }
                else
                {
                    label_turn.Text = "Black Move:";
                    label_othTime.Text = "WHT - " + _mainWindowHandle.currentGame.getTime(chessColour.WHITE);
                }
            }

            //Update the chessclock every tick
            if (_mainWindowHandle.currentGame.Turn == chessColour.WHITE)
            {
                label_curTime.Text = _mainWindowHandle.currentGame.getTime(_mainWindowHandle.currentGame.Turn);
            }
            else
            {
                label_curTime.Text = _mainWindowHandle.currentGame.getTime(_mainWindowHandle.currentGame.Turn);
            }
        }
    }
}
