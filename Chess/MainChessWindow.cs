using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Chess.Networking;

namespace Chess
{    
    //enums
    public enum chessPieces
    {
        KING = 0,
        QUEEN,
        BISHOP,
        ROOK,
        KNIGHT,
        PAWN,
        CLEAR
    };
    public enum chessColour
    {
        BLACK = 0,
        WHITE = 1
    };

    public partial class MainChessWindow : Form
    {
        public const int MAX_CHESS_SQUARES = 8;

        //display declarations
        private bool[,] squareIsWhite;
        public bool selectionMade, opponentSelMade;
        public static chessColour firstPlayerColour = chessColour.WHITE;        //by default, first player is white
        public int selectedX, selectedY, opponentSelX, opponentSelY;

        //Created to notify when a chess move has been completed
        public event EventHandler<chessMoveArgs> chessMoveEvent;
        public event EventHandler<selectionMadeArgs> selectionMadeEvent;

        public static bool isNetworked;                 //records true if this player is playing against another person over the network, false otherwise
        public static chessColour thisPlayerColour;     //used to record which colour the current player is playing as

        //Allows the game clock to be created
        private static ChessClockWindow _gameClock = null;
        private static NetworkingWindow _gameNetwork = null;

        //chess logic declarations
        private ChessPiece[,] chessBoard;

        //gameplay declarations
        Gameplay currentGame = new Gameplay();

        public MainChessWindow()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            InitializeComponent();

            selectionMade = false;
            opponentSelMade = false;
            selectedX = 0;
            selectedY = 0;
            opponentSelX = 0;
            opponentSelY = 0;
        }

        //Initial values when main window loads
        private void Form1_Load(object sender, EventArgs e)
        {
            //create the chess board display
            dgv_chessBoard.Rows.Add(MAX_CHESS_SQUARES);
            squareIsWhite = new bool[MAX_CHESS_SQUARES, MAX_CHESS_SQUARES];


            //create the logical chessboard
            chessBoard = ChessPiece.CreateChessBoard();

            //set the first player based on user input
            currentGame.Turn = firstPlayerColour;

            DrawCheckeredPattern();
            updateDisplay();

            isNetworked = false;    //by default, person is just playing with themselves
        }

        //sets up new game
        public void newGame()
        {
            //Reset game
            currentGame.reset();

            //Display Board
            DrawCheckeredPattern();
            updateDisplay();
        }

        //Event driven logic - user clicking on a chess piece drives the whole program
        private void dgv_chessBoard_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //We can only move a piece if we've selected one to move
            if (selectionMade)
            {
                ChessPiece selectedPiece = chessBoard[selectedX, selectedY];
                chessColour selectedColour = chessBoard[selectedX, selectedY].Colour;

                // check that the move is legal for the piece selected,
                // and that the piece is the correct colour to move,
                if (currentGame.Turn == selectedColour                                &&
                    selectedPiece.isValidMove(e.ColumnIndex, e.RowIndex, chessBoard)  )
  
                {
                    //If we're playing single player with no networking we can move black or white pieces
                    if (!isNetworked)
                    {
                        movePiece(selectedX, selectedY, e.ColumnIndex, e.RowIndex);
                    }
                    else if (thisPlayerColour == currentGame.Turn)  //If we're playing a networked game, we can only move our own pieces
                    {
                        movePiece(selectedX, selectedY, e.ColumnIndex, e.RowIndex);

                        // Use an event to call the networkingWindow to record and send the move to other player
                        if (isNetworked)
                        {
                            //"MOVE|oldCol,oldRow|newCol,newRow\n"
                            chessMoveArgs args = new chessMoveArgs();
                            args.oldX = selectedX;
                            args.oldY = selectedY;
                            args.newX = e.ColumnIndex;
                            args.newY = e.RowIndex;

                            OnChessMoveEvent(args);
                        }
                    }
                }

                //if the move wasn't valid, clear selection for next use
                selectionMade = false;

                //report selection to other player
                if (isNetworked)
                {
                    selectionMadeArgs args = new selectionMadeArgs();
                    args.selection = selectionMade;
                    args.intX = selectedX;
                    args.intY = selectedY;
                    OnSelectionMadeEvent(args);
                }
            }
            else
            {
                selectionMade = true;           //saves the selection
                selectedX = e.ColumnIndex;
                selectedY = e.RowIndex;

                //report selection to other player
                if (isNetworked)
                {
                    selectionMadeArgs args = new selectionMadeArgs();
                    args.selection = selectionMade;
                    args.intX = selectedX;
                    args.intY = selectedY;
                    OnSelectionMadeEvent(args);
                }
            }

            updateSelectionSquare();               //updates the gameboard if a square has been selected/released
        }

        //Moves chess piece from old to new location on the board
        public void movePiece(int oldCol, int oldRow, int newCol, int newRow)
        {
            //Finds the type and colour of piece that we're moving
            chessPieces selectedType = chessBoard[oldCol, oldRow].Type();
            chessColour selectedColour = chessBoard[oldCol, oldRow].Colour;

            //You can't move a piece if it doesn't exist
            if (selectedType != chessPieces.CLEAR)
            {
                //Variables to correct display after enPassent move
                bool enPassant = false ;
                Pawn enPassantPawn = null ;
                if (ChessPiece.DblMovePawn != null)
                {
                    enPassant = ChessPiece.isEnPassant(oldCol, oldRow, newCol, newRow, chessBoard);
                    enPassantPawn = (Pawn)(ChessPiece.DblMovePawn).Clone();
                }

                //move the piece
                ChessPiece.Move(oldCol, oldRow, newCol, newRow, chessBoard);

                //Every time a valid move is made, switch turns
                currentGame.changeTurns();

                //DISPLAY - Update only the piece that just moved:
                displayPieceAt(newCol, newRow, selectedType, selectedColour);

                //DISPLAY - Special case: If the move was enPassent, update the possible pawn locations (+/- one from the pawn that just moved
                if (enPassant)
                {
                    displayPieceAt( enPassantPawn.PositionX, 
                                    enPassantPawn.PositionY, 
                                    chessBoard[enPassantPawn.PositionX, enPassantPawn.PositionY].Type(),
                                    chessBoard[enPassantPawn.PositionX, enPassantPawn.PositionY].Colour);
                }
            }
        }


//TOOL MENU ITEMS-------------------------------------------------------------------
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame();
        }

        private void playAsBlackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (playAsBlackToolStripMenuItem.Checked == true)
            {
                firstPlayerColour = chessColour.BLACK;
            }
            else
            {
                firstPlayerColour = chessColour.WHITE;
            }

            newGame();
        }

        private void clockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if the clock hasn't been created yet, create it
            if (null == _gameClock)
            {
                _gameClock = new ChessClockWindow();
                _gameClock.FormBorderStyle = FormBorderStyle.FixedDialog;
            }

            //if the clock has been created, toggle visibility on and off
            if (null != _gameClock)
            {
                if (_gameClock.Visible)
                {
                    _gameClock.SendToBack();
                    _gameClock.Hide();
                }
                else
                {
                    _gameClock.BringToFront();
                    _gameClock.Show();
                }
            }
        }

        private void networkingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if the networking window hasn't been created yet, create it
            if (null == _gameNetwork)
            {
                _gameNetwork = new NetworkingWindow(this);
                _gameNetwork.FormBorderStyle = FormBorderStyle.FixedDialog;
            }

            //if the clock has been created, toggle visibility on and off
            if (null != _gameNetwork)
            {
                if (_gameNetwork.Visible)
                {
                    _gameNetwork.SendToBack();
                    _gameNetwork.Hide();
                }
                else
                {
                    _gameNetwork.BringToFront();
                    _gameNetwork.Show();
                }
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (firstPlayerColour == chessColour.BLACK)
            {
                playAsBlackToolStripMenuItem.Checked = true;
            }
            else
            {
                playAsBlackToolStripMenuItem.Checked = false;
            }
        }
 //END OF TOOL MENU ITEMS-------------------------------------------------------------

//DISPLAY FUNCTIONS---------------------------------------------------------------

        //Draws initial checkered pattern and sets the width and height of the spaces
        private void DrawCheckeredPattern()
        {
            bool white = false;
            for (int i = 0; i < MAX_CHESS_SQUARES; i++)
            {
                white = !white;
                dgv_chessBoard.Rows[i].Height = 80;
                dgv_chessBoard.Columns[i].Width = 87;
                for (int j = 0; j < MAX_CHESS_SQUARES; j++)
                {
                    white = !white;
                    if (white)
                    {
                        dgv_chessBoard[i, j].Value = Chess.Properties.Resources.WhiteSquare;
                        squareIsWhite[i, j] = true;
                    }
                    else
                    {
                        dgv_chessBoard[i, j].Value = Chess.Properties.Resources.BlackSquare;
                        squareIsWhite[i, j] = false;
                    }
                }
            }
        }

        //Displays the chess piece at the given position
        private void displayPieceAt(int newPosX, int newPosY, chessPieces type, chessColour colour)
        {
            //update title bar whenever you move a piece
            updateTitleBar();

            switch (type)
            {
                case chessPieces.KING:
                    if (colour == chessColour.WHITE)    //Display White King
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteKingW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteKingB;
                        }
                    }
                    else  //Display Black King
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackKingW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackKingB;
                        }
                    }
                    break;

                case chessPieces.QUEEN:
                    if (colour == chessColour.WHITE)    //Display White Queen
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteQueenW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteQueenB;
                        }
                    }
                    else  //Display Black Queen
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackQueenW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackQueenB;
                        }
                    }
                    break;

                case chessPieces.BISHOP:
                    if (colour == chessColour.WHITE)    //Display White Bishop
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteBishopW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteBishopB;
                        }
                    }
                    else  //Display Black Bishop
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackBishopW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackBishopB;
                        }
                    }
                    break;

                case chessPieces.KNIGHT:
                    if (colour == chessColour.WHITE)    //Display White Knight
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteKnightW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteKnightB;
                        }
                    }
                    else  //Display Black Knight
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackKnightW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackKnightB;
                        }
                    }
                    break;

                case chessPieces.ROOK:
                    if (colour == chessColour.WHITE)    //Display White Rook
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteRookW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteRookB;
                        }
                    }
                    else  //Display Black Rook
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackRookW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackRookB;
                        }
                    }
                    break;

                case chessPieces.PAWN:
                    if (colour == chessColour.WHITE)    //Display White Pawn
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhitePawnW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhitePawnB;
                        }
                    }
                    else  //Display Black Pawn
                    {
                        if (squareIsWhite[newPosX, newPosY])
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackPawnW;
                        }
                        else
                        {
                            dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackPawnB;
                        }
                    }
                    break;
                case chessPieces.CLEAR:
                    blankSquareAt(newPosX, newPosY);
                    break;
            }
        }

        //Displays no chess piece at the given position
        private void blankSquareAt(int newPosX, int newPosY)
        {
            if (squareIsWhite[newPosX, newPosY])
            {
                dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.WhiteSquare;
            }
            else
            {
                dgv_chessBoard[newPosX, newPosY].Value = Chess.Properties.Resources.BlackSquare;
            }
        }

        //Sets all chess pieces to display in the startup positions
        public void updateDisplay()
        {
            updateTitleBar();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    chessColour tmpColour = chessBoard[i, j].Colour;
                    chessPieces tmpType = chessBoard[i, j].Type();
                    displayPieceAt(i, j, tmpType, tmpColour);
                }
            }

            updateSelectionSquare();
        }

        private void updateTitleBar()
        {
            //update the main window title depending on which colour is playing
            if (currentGame.Turn == chessColour.BLACK)
            {
                this.Text = "Chess - Black Move";
            }
            else
            {
                this.Text = "Chess - White Move";
            } 
        }

        private void updateSelectionSquare()
        {
            //special case to draw the selected square if there is one
            if (selectionMade)
            {
                dgv_chessBoard[selectedX, selectedY].Value = Chess.Properties.Resources.BlueSquare;     //Highlights the selected object
            }
            else
            {
                ChessPiece updatePiece = chessBoard[selectedX, selectedY];
                displayPieceAt(updatePiece.PositionX, updatePiece.PositionY, updatePiece.Type(), updatePiece.Colour);
            }

            //Make sure that we can't override what the main player is trying to select
            if (!(opponentSelX == selectedX && opponentSelY == selectedY))
            {
                if (opponentSelMade)
                {

                    dgv_chessBoard[opponentSelX, opponentSelY].Value = Chess.Properties.Resources.Orange;   //Highlights the object the opponent is selecting

                }
                else
                {
                    ChessPiece updatePiece = chessBoard[opponentSelX, opponentSelY];
                    displayPieceAt(updatePiece.PositionX, updatePiece.PositionY, updatePiece.Type(), updatePiece.Colour);
                }
            }
        }

//END OF DISPLAY FUNCTIONS------------------------------------------------------

//CUSTOM EVENTS-----------------------------------------------------------------
        //When a chess move has been made, this event is called to update the other player on network
        protected virtual void OnChessMoveEvent(chessMoveArgs e)
        {
            EventHandler<chessMoveArgs> handler = chessMoveEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        //When a user has selected a square this event is called to update the other player on network
        protected virtual void OnSelectionMadeEvent(selectionMadeArgs e)
        {
            EventHandler<selectionMadeArgs> handler = selectionMadeEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    //holds the arguments to be passed by the OnChessMoveEvent
    public class chessMoveArgs : EventArgs
    {
        public int oldX { get; set; }
        public int oldY { get; set; }
        public int newX { get; set; }
        public int newY { get; set; }
    }

    //Sends true when selected, false when deselected
    public class selectionMadeArgs : EventArgs
    {
        public bool selection { get; set; }
        public int intX { get; set; }
        public int intY { get; set; }
    }
 //END OF CUSTOM EVENTS-----------------------------------------------------------------
}