using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Chess
{
    [Serializable]
    public class ChessPiece
    {
        public const int MAX_CHESS_VALUE = 7;
        public const int MIN_CHESS_VALUE = 0;
        public const int WHT_PAWN_START = MIN_CHESS_VALUE + 1;
        public const int BLK_PAWN_START = MAX_CHESS_VALUE - 1;
        public const int WHT_PAWN_DBLMV = WHT_PAWN_START + 2;
        public const int BLK_PAWN_DBLMV = BLK_PAWN_START - 2;
        public const int KNT_BIG_INC = 2;
        public const int KNT_SML_INC = 1;

        private int _positionX;
        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }

        private int _positionY;
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }

        private bool _canCastle;
        public bool CanCastle
        {
            get { return _canCastle; }
            set { _canCastle = value; }
        }

        public King _king;                  //holds reference to the king for this piece

        //variable declarations
        private chessColour _colour;
        public chessColour Colour
        {
            get { return _colour; }
            set { _colour = value; }
        }

        private static Pawn _doubleMove;     //variable keeps track of the last pawn to move forward two steps
        public static Pawn DblMovePawn
        {
            get { return _doubleMove; }
            set { _doubleMove = value; }
        }

        //parameterless constructor - required for serialization
        public ChessPiece ()
        {
            _positionX = 0;
            _positionY = 0;
            _canCastle = true;
            _colour = chessColour.BLACK;
        }

        //constructor
        public ChessPiece (int xPos, int yPos)
        {
            _positionX = xPos;
            _positionY = yPos;
            _canCastle = true;
            _colour = chessColour.BLACK;
        }

        //Creates and returns a chessboard with all the pieces in the starting locations
        public static ChessPiece[,] CreateChessBoard()
        {
            ChessPiece[,] chessBoard = new ChessPiece[8, 8];
            
            //sets up blank spaces
            for (int i = 0; i < 8; i++)
            {
                for (int j = 2; j <= 6; j++)
                {
                    chessBoard[i, j] = new ChessPiece(i,j);
                }
            }

            //sets up pawns
            for (int i = 0; i < 8; i++)
            {
                chessBoard[i, 1] = new Pawn(i, 1);
                chessBoard[i, 6] = new Pawn(i, 6);
            }

            for (int i = 0; i < 8; i = i + 7)
            {
                //kings and queens
                chessBoard[4, i] = new King(4, i);
                chessBoard[3, i] = new Queen(3, i);

                //Rooks
                chessBoard[0, i] = new Rook(0, i);
                chessBoard[7, i] = new Rook(7, i);

                //Knights
                chessBoard[1, i] = new Knight(1, i);
                chessBoard[6, i] = new Knight(6, i);

                //Bishops
                chessBoard[2, i] = new Bishop(2, i);
                chessBoard[5, i] = new Bishop(5, i);
            }

            //record the colour of all chess piece objects, set up all kings
            for (int i = 0; i <= MAX_CHESS_VALUE; i++)
            {
                chessBoard[i, 0].Colour = chessColour.WHITE;
                chessBoard[i, 1].Colour = chessColour.WHITE;
                chessBoard[i, 0]._king = (King)chessBoard[4, 0];
                chessBoard[i, 1]._king = (King)chessBoard[4, 0];

                chessBoard[i, 6].Colour = chessColour.BLACK;
                chessBoard[i, 7].Colour = chessColour.BLACK;
                chessBoard[i, 6]._king = (King)chessBoard[4, 7];
                chessBoard[i, 7]._king = (King)chessBoard[4, 7];
            }

            return chessBoard;
        }

        //default type of chess piece is a blank square
        public virtual chessPieces Type()
        {
            return chessPieces.CLEAR;
        }

        //blank chess pieces cannot be moved
        public virtual bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            return false;
        }

        //checks if a chess piece has moved
        public bool hasMoved(int finalX, int finalY)
        {
            if (_positionX == finalX && _positionY == finalY)
            {
                return false;
            }
            return true;
        }

        //checks if a chess piece is moving up or down
        public bool isMovingLeftRight(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            int start = 0;
            int end = 0;

            if (_positionY == finalY)
            {
                //moving left
                if (finalX < _positionX)
                {
                    start = finalX + 1; //add 1 to keep from recognizing the target piece
                    end = _positionX;
                }
                else if (finalX > _positionX)//moving right
                {
                    start = _positionX + 1; //add 1 to keep from recognizing your own piece
                    end = finalX;
                }

                //loop through the spaces travelled to find if there are any pieces blocking this move
                for (int i = start; i < end; i++)
                {
                    //if any piece is in the way, then the move is invalid - return false
                    if (chessBoard[i, _positionY].Type() != chessPieces.CLEAR)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        //checks if a chess piece is moving left or right
        public bool isMovingUpDown(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            int start = 0;
            int end = 0;

            if (_positionX == finalX)
            {
                //moving down
                if (finalY < _positionY)
                {
                    start = finalY + 1; //add 1 to keep from recognizing the target piece;
                    end = _positionY;
                }
                else if (finalY > _positionY)//moving up
                {
                    start = _positionY + 1; //add 1 to keep from recognizing your own piece
                    end = finalY;
                }

                //loop through the spaces travelled to find if there are any pieces blocking this move
                for (int i = start; i < end; i++)
                {
                    //if any piece is in the way, then the move is invalid - return false
                    if (chessBoard[_positionX, i].Type() != chessPieces.CLEAR)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool notAttackingFriendly(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //If there is a chess piece at your destination and the colour is the same as your own, it's an invalid move
            if (chessBoard[finalX, finalY].Colour == _colour &&
                chessBoard[finalX, finalY].Type() != chessPieces.CLEAR)
            {
                return false;
            }
            return true;
        }

        //checks for diagonal movement from a chess piece
        public bool isDiagonal(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            bool addX = true;
            bool addY = true;

            //if a movement is diagonal, the slope of the line should always be 1 or -1
            //which means that (abs)y - y1 == (abs)x - x1
            int yVals = Math.Abs(_positionY - finalY);
            int xVals = Math.Abs(_positionX - finalX);

            //If movement is not diagonal, then return false
            if (yVals != xVals)
            {
                return false;
            }

            //figure out which way the piece is moving
            if(_positionY < finalY)         //going down
            {
                addY = true;
            }
            else if (_positionY > finalY)   //going up
            {
                addY = false;
            }

            if(_positionX < finalX)         //going right
            {
                addX = true;
            }
            else if (_positionX > finalX)   //going left
            {
                addX = false;
            }

            int x = _positionX, y = _positionY;
            while (x != finalX )
            {
                //update variables to keep from counting the position of the piece you're moving
                if (addX)
                {
                    x++;
                }
                else
                {
                    x--;
                }

                if (addY)
                {
                    y++;
                }
                else
                {
                    y--;
                }
                
                //end the loop once the final square has been found
                if (x == finalX || y == finalY)
                {
                    //final value reached, we do not check the piece being attacked
                }
                else if (chessBoard[x, y].Type() != chessPieces.CLEAR) //check for blocking pieces
                {
                    return false;
                }
            }

            return true;
        }

        //checks for L shaped movement
        public bool isMovingL(int finalX, int finalY)
        {
            //L shaped movement must take the form (X +/- 1, Y +/- 2) or (X +/-2, Y +/-1)
            if ((Math.Abs(_positionX - finalX) == KNT_BIG_INC && Math.Abs(_positionY - finalY) == KNT_SML_INC) ||
                 (Math.Abs(_positionX - finalX) == KNT_SML_INC && Math.Abs(_positionY - finalY) == KNT_BIG_INC))
            {
                return true;
            }
            return false;
        }

        //checks for a single space movement (King's movement patterns)
        public bool isMovingSingle(int finalX, int finalY)
        {
            //single space movement x and y cannot be increased/decreased more than 1 space
            if ((Math.Abs(_positionX - finalX) <= 1 && Math.Abs(_positionY - finalY) <= 1))
            {
                return true;
            }
            return false;
        }

        //checks for three possible movement conditions: pawn attacking, pawn advancing, and enpassant
        public bool isMovingPawn(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //pawns cannot move into the space occupied by any other piece, pawns cannot move side to side
            if (chessBoard[finalX, finalY].Type() != chessPieces.CLEAR ||
                finalX != _positionX)
            {
                return false;
            }

            //pawns can move two spaces up from their initial spot
            if ((  _colour == chessColour.WHITE && finalY == WHT_PAWN_DBLMV && _positionY == WHT_PAWN_START    ) ||
                (  _colour == chessColour.BLACK && finalY == BLK_PAWN_DBLMV && _positionY == BLK_PAWN_START    ))
            {
                //pawns cannot jump any chess pieces
                if ((_colour == chessColour.WHITE && chessBoard[_positionX, WHT_PAWN_START+1].Type() == chessPieces.CLEAR) ||
                    (_colour == chessColour.BLACK && chessBoard[_positionX, BLK_PAWN_START-1].Type() == chessPieces.CLEAR))
                {
                    return true;
                }
            }
            //checks for regular (single space) pawn advancement
            else if ( ( _colour == chessColour.BLACK && finalY - _positionY == -1) ||        //black pawns go up
                      ( _colour == chessColour.WHITE && finalY - _positionY == 1))           //white pawns go down
            {
                return true;
            }
            return false;
        }

        public static bool isEnPassant(int oldX, int oldY, int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            chessColour colour = chessBoard[oldX, oldY].Colour;

            //EnPassant move is up one and across one from the initial pawn position
            if (colour == chessColour.WHITE && oldY != BLK_PAWN_DBLMV)            //White pawns can attack enPassent only from row4
            {
                return false;
            }
            else if (colour == chessColour.BLACK && oldY != WHT_PAWN_DBLMV)   //black can only attack enpassent from position 3
            {
                return false;
            }

            //Check that the defending pawn is the same as the last pawn moved up two squares
            if (_doubleMove != null                                         &&
                chessBoard[finalX, oldY].Colour == _doubleMove.Colour       &&
                chessBoard[finalX, oldY].PositionX == _doubleMove.PositionX &&
                chessBoard[finalX, oldY].PositionY == _doubleMove.PositionY  )
            {
                //Yes!  This is an enPassent move, giv'er.
                return true;
            }

            return false;
        }

        public bool isAttackingPawn(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //checks for enPassant condition
            if ( isEnPassant(_positionX, _positionY, finalX, finalY, chessBoard) )
            {
                return true;
            }

            //checks for normal pawn attacking (pawn must advance and move one space to the right or left
            if ( (Math.Abs(finalX - _positionX) == 1) &&
                 ((_colour == chessColour.BLACK && finalY - _positionY == -1) ||
                  (_colour == chessColour.WHITE && finalY - _positionY == 1)))
            {
                //can't advance into a space that is not occupied by an enemy piece
                if (chessBoard[finalX, finalY].Type() != chessPieces.CLEAR && 
                    chessBoard[finalX, finalY].Colour != _colour)
                {
                    return true;
                }
            }
            return false;
        }

        //returns true if the king is in check after the proposed move
        //takes in the piece being moved, the king in question, the final x/y positions of the piece, and the chessBoard
        public bool isKingInCheckAfterMove(King king, int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            int oldX = _positionX;
            int oldY = _positionY;

            ChessPiece[,] chessBoardAfterMove = new ChessPiece[MAX_CHESS_VALUE + 1, MAX_CHESS_VALUE+1];

            //Create duplicate chessboard to examine future move
            for (int i = 0; i <= MAX_CHESS_VALUE; i++)
            {
                for (int j = 0; j <= MAX_CHESS_VALUE; j++)
                {
                    chessBoardAfterMove[i, j] = chessBoard[i, j].Clone();
                }
            }

            //Make the move on the chessboard
            chessBoardAfterMove[finalX, finalY] = chessBoardAfterMove[oldX, oldY];
            chessBoardAfterMove[oldX,oldY] = new ChessPiece(oldX,oldY);

            //is the king is in check after this move?
            return king.isInCheck(king.PositionX, king.PositionY, chessBoardAfterMove);
        }

        //Returns true if the piece is a king or rook
        public static bool isCastling(ChessPiece firstPiece, ChessPiece secondPiece, ChessPiece[,] chessBoard)
        {
            bool retVal = false;

            //Check that we're dealing with a King and a Rook, and that they're the same colour
            if (((firstPiece.Type() == chessPieces.KING && secondPiece.Type() == chessPieces.ROOK) ||
                 (secondPiece.Type() == chessPieces.KING && firstPiece.Type() == chessPieces.ROOK)) &&
                secondPiece.Colour == firstPiece.Colour)
            {
                //Check that the pieces can Castle
                if (firstPiece.CanCastle && secondPiece.CanCastle)
                {
                    bool emptySpace = true;

                    //check that there is empty space between the king and rook
                    int max;
                    int min;
                    if (firstPiece.PositionX > secondPiece.PositionX)
                    {
                        max = firstPiece.PositionX;
                        min = secondPiece.PositionX;
                    }
                    else
                    {
                        max = secondPiece.PositionX;
                        min = firstPiece.PositionX;
                    }

                    for (int i = min + 1; i < max; i++)
                    {
                        if (chessBoard[i, firstPiece.PositionY].Type() != chessPieces.CLEAR)
                        {
                            emptySpace = false;
                        }
                    }

                    if (emptySpace)
                    {
                        retVal = true;
                    }
                }
            }
            return retVal;
        }

        //Moves a piece from one location to another on the chess board
        public static void Move(int oldX, int oldY, int newX, int newY, ChessPiece[,] chessBoard)
        {
            //You can't move a piece if it doesn't exist
            if (chessBoard[oldX,oldY].Type() != chessPieces.CLEAR)
            {
                chessColour pieceColor = chessBoard[oldX, oldY].Colour;

                //Special cases for pawns
                if (chessBoard[oldX, oldY].Type() == chessPieces.PAWN)
                {
                    //We must remove the enemy pawn killed via enPassent if applicable
                    if (isEnPassant(oldX, oldY, newX, newY, chessBoard))
                    {
                        chessBoard[newX, oldY] = new ChessPiece(newX, newY - 1);
                    }

                    //When a white pawn moves to the bottom (8) or a black pawn moves to the top (0) it is promoted
                    if ( (pieceColor == chessColour.BLACK && newY == MIN_CHESS_VALUE)   ||
                         (pieceColor == chessColour.WHITE && newY == MAX_CHESS_VALUE)   )
                    {
                        //replace the pawn with a queen
                        King oldKing = chessBoard[oldX, oldY]._king;
                        chessBoard[oldX, oldY] = new Queen(oldX, oldY);
                        chessBoard[oldX, oldY].Colour = pieceColor;
                        chessBoard[oldX, oldY]._king = oldKing;
                    }

                    //Whenever a pawn moves two spaces, we record this for processing later
                    if (Math.Abs(newY - oldY) > 1)
                    {
                        ChessPiece.DblMovePawn = (Pawn)chessBoard[oldX, oldY];
                    }
                    else
                    {
                        //if the pawn didn't move two spaces, then clear DblMove
                        ChessPiece.DblMovePawn = null;
                    }
                }

                //Special case - If castling, we move the pieces to a different location
                if ( isCastling(chessBoard[oldX, oldY], chessBoard[newX, newY], chessBoard ))
                {
                    //Find the king and rook
                    ChessPiece king;
                    ChessPiece rook;
                    if (chessBoard[oldX, oldY].Type() == chessPieces.KING)
                    {
                        king = chessBoard[oldX, oldY];
                        rook = chessBoard[newX, newY];
                    }
                    else
                    {
                        rook = chessBoard[oldX, oldY];
                        king = chessBoard[newX, newY];
                    }

                    //Move the king two spaces X towards the rook
                    //Move the rook next to the king, on the opposite side
                    if (king.PositionX < rook.PositionX)
                    {
                        king.PositionX = king.PositionX + 2;
                        rook.PositionX = king.PositionX - 1;
                    }
                    else
                    {
                        king.PositionX = king.PositionX - 2;
                        rook.PositionX = king.PositionX + 1;
                    }

                    //Update positions on chessboard - put pieces in new spots, clear old spots
                    chessBoard[king.PositionX, king.PositionY] = king;
                    chessBoard[rook.PositionX, rook.PositionY] = rook;
                    chessBoard[oldX, oldY] = new ChessPiece(oldX, oldY);
                    chessBoard[newX, newY] = new ChessPiece(newX, newY);

                    //Once moved, the king and rook can never castle again
                    king.CanCastle = false;
                    rook.CanCastle = false;
                }
                else    // regular move takes place
                {
                    //move the piece to the new location
                    chessBoard[newX, newY] = chessBoard[oldX, oldY];

                    //record the new location in the piece
                    chessBoard[newX, newY].PositionX = newX;
                    chessBoard[newX, newY].PositionY = newY;

                    //replace the piece you moved
                    chessBoard[oldX, oldY] = new ChessPiece(oldX, oldY);
                }

                //If the king or roook have moved, they can never castle again
                if (chessBoard[newX, newY].Type() == chessPieces.KING ||
                    chessBoard[newX, newY].Type() == chessPieces.ROOK)
                {
                    chessBoard[newX, newY].CanCastle = false;
                }
            }
        }

        //Used for creating a duplicate object
        public virtual ChessPiece Clone()
        {
            ChessPiece returnPiece = new ChessPiece(_positionX, _positionY);
            returnPiece.Colour = Colour;
            return returnPiece;
        }
    }

    [Serializable]
    public class Rook : ChessPiece
    {
        //constructors
        public Rook(): base()
        {
        }
        public Rook (int xPos, int yPos) : base (xPos, yPos)
        {
        }

        public override bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //Check that the pience has moved
            if (hasMoved(finalX, finalY))
            {
                if (notAttackingFriendly(finalX, finalY, chessBoard))
                {
                    //Rooks can move up/down or left/right
                    if (isMovingUpDown(finalX, finalY, chessBoard) || 
                        isMovingLeftRight(finalX, finalY, chessBoard)  )
                    {
                        //Check if the move places the king in check
                        if ( !isKingInCheckAfterMove(_king, finalX, finalY, chessBoard) )
                        {
                            //safe to move!
                            return true;
                        }
                    }
                }
                //Check if the rook is castling
                else if (isCastling(chessBoard[PositionX, PositionY], chessBoard[finalX, finalY], chessBoard))
                {
                    return true;
                }
            }
            return false;
        }

        public override chessPieces Type()
        {
            return chessPieces.ROOK;
        }

        //Used for creating a duplicate object
        public override ChessPiece Clone()
        {
            Rook returnPiece = new Rook(PositionX, PositionY);
            returnPiece.Colour = Colour;
            returnPiece.CanCastle = CanCastle;
            return returnPiece;
        }
    }

    [Serializable]
    public class Queen : ChessPiece
    {
        //constructors
        public Queen(): base()
        {
        }
        public Queen (int xPos, int yPos) : base (xPos, yPos)
        {
        }

        public override bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //Check that the piece has moved and is not attempting to take a piece of the same colour
            if (hasMoved(finalX, finalY) && notAttackingFriendly(finalX, finalY, chessBoard))
            {
                //Queens can move diagonally, up/down, or left/right
                if (isDiagonal(finalX, finalY, chessBoard) ||
                    isMovingUpDown(finalX, finalY, chessBoard) ||
                    isMovingLeftRight(finalX, finalY, chessBoard))
                {
                    //Check if the move places the king in check
                    if (!isKingInCheckAfterMove(_king, finalX, finalY, chessBoard))
                    {
                        //safe to move!
                        return true;
                    }
                }
            }
            return false;
        }

        public override chessPieces Type()
        {
            return chessPieces.QUEEN;
        }

        //Used for creating a duplicate object
        public override ChessPiece Clone()
        {
            Queen returnPiece = new Queen(PositionX, PositionY);
            returnPiece.Colour = Colour;
            return returnPiece;
        }
    }

    [Serializable]
    public class Pawn : ChessPiece
    {
        //constructors
        public Pawn(): base()
        {
        }
        public Pawn (int xPos, int yPos) : base (xPos, yPos)
        {
        }

        public override bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //check for legal pawn move or attack
            if (isMovingPawn(finalX, finalY, chessBoard) || isAttackingPawn(finalX, finalY, chessBoard))
            {
                //Check if the move places the king in check
                if (!isKingInCheckAfterMove(_king, finalX, finalY, chessBoard))
                {
                    //safe to move!
                    return true;
                }
            }
            return false;
        }

        public override chessPieces Type()
        {
            return chessPieces.PAWN;
        }

        //Used for creating a duplicate object
        public override ChessPiece Clone()
        {
            Pawn returnPiece = new Pawn(PositionX, PositionY);
            returnPiece.Colour = Colour;
            return returnPiece;
        }
    }

    [Serializable]
    public class King : ChessPiece
    {
        //constructors
        public King(): base()
        {
        }
        public King (int xPos, int yPos) : base (xPos, yPos)
        {

        }

        public override bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //Check that the piece has moved
            if (hasMoved(finalX, finalY))
            {
                if ( notAttackingFriendly(finalX, finalY, chessBoard) )
                {
                    //Special case - since we are moving the king, the king position is changing.  We must 
                    //create a new king and pass his future position to the check function
                    King newKing = new King(finalX, finalY);
                    newKing.Colour = _king.Colour;

                    //King can move a single spot in any direction, but not into check
                    if (isMovingSingle(finalX, finalY) && !isKingInCheckAfterMove(newKing, finalX, finalY, chessBoard))
                    {
                        return true;
                    }
                }
                //Check if the rook is castling
                else if (isCastling(chessBoard[PositionX, PositionY], chessBoard[finalX, finalY], chessBoard))
                {
                    return true;
                }
            }
            return false;
        }

        //Test to see if the king is currently in check
        public bool isInCheck(int kingX, int kingY, ChessPiece[,] chessBoard)
        {
            chessColour kingColour = chessBoard[kingX,kingY].Colour;

            //CHECK KNIGHT LOCATIONS
            if (searchForKnightAttacks(kingX, kingY, chessBoard, kingColour))
            {
                return true;
            }

            //CHECK FOR LINEAR ATTACKS - eight possible locations
            if ( searchForLinearAttacks(1, 0, kingX, kingY, chessBoard, kingColour)  ||    //horizontal attack
                 searchForLinearAttacks(0, 1, kingX, kingY, chessBoard, kingColour) ||    //vertical attack
                 searchForLinearAttacks(1, -1, kingX, kingY, chessBoard, kingColour) ||    //diagonal right
                 searchForLinearAttacks(1, 1, kingX, kingY, chessBoard, kingColour))     //diagonal left 
            {
                return true;
            }

            //CHECK FOR PAWN ATTACKS
            if ( searchForPawnAttacks(kingX, kingY, chessBoard, kingColour) )
            {
                return true;
            }

            //If we got here, nothing is putting the king in check!
            return false;
        }

        //Takes in the expected position of the king, the chessBoard, and the colour of the king.
        //Checks all eight positions that a knight could possibly attack from.
        private bool searchForKnightAttacks(int kingX, int kingY, ChessPiece[,] chessBoard, chessColour kingColour)
        {
            int xAdd, yAdd, tmpX, tmpY;

            //Loop through -2,-1, 0, 1, 2
            for (int i = -KNT_BIG_INC; i <= KNT_BIG_INC; i++)
            {
                //ignore zero
                if (i != 0)
                {
                    xAdd = i;
                    tmpX = kingX + xAdd;

                    //Check validity of the position
                    if (tmpX <= MAX_CHESS_VALUE &&
                        tmpX >= MIN_CHESS_VALUE)
                    {
                        //Good x value, now let's find corresponding y values
                        //Loop through -2,-1, 0, 1, 2
                        for (int j = -KNT_BIG_INC; j <= KNT_BIG_INC; j++)
                        {
                            //ignore zero
                            if (j != 0)
                            {
                                yAdd = j;
                                tmpY = kingY + yAdd;

                                //Check validity of y position
                                if (tmpY <= MAX_CHESS_VALUE &&
                                    tmpY >= MIN_CHESS_VALUE)
                                {
                                    // Knights always move two spaces in one direction and one space in another.
                                    // yAdd and xAdd cannot be the same value if the move is valid
                                    if (Math.Abs(yAdd) != Math.Abs(xAdd))
                                    {
                                        //if the colour of the chesspiece at this location is attacking, and 
                                        //it is a knight
                                        if (chessBoard[tmpX, tmpY].Colour != kingColour &&
                                            chessBoard[tmpX, tmpY].Type() == chessPieces.KNIGHT)
                                        {
                                            //we are in check
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //No knight putting the king in check!
            return false;
        }

        //Takes in the expected position of the king, the chessBoard, and the colour of the king.
        //Checks both positions a pawn could attack from.
        private bool searchForPawnAttacks(int kingX, int kingY, ChessPiece[,] chessBoard, chessColour kingColour)
        {
            int tmpX, tmpY, incY;

            //White is at the top, so always attacked by pawns below
            if (chessBoard[kingX, kingY].Colour == chessColour.WHITE)
            {
                incY = 1;
            }
            else //Black king, always attacked by pawns above
            {
                incY = -1;
            }

            //There are two pawn locations that the king could be attacked from, start 
            //by checking the right, then loop through to check the left
            tmpX = kingX + 1;
            for (int i = 0; i <= 1; i++)
            {
                //Check that the pawn is in a valid location
                if (tmpX <= MAX_CHESS_VALUE &&
                    tmpX >= MIN_CHESS_VALUE )
                {
                    tmpY = kingY + incY;
                    if (tmpY <= MAX_CHESS_VALUE &&
                        tmpY >= MIN_CHESS_VALUE  )
                    {
                        //Check for attacking colour, and that the piece is a pawn
                        if (chessBoard[tmpX, tmpY].Colour != kingColour    &&
                            chessBoard[tmpX, tmpY].Type() == chessPieces.PAWN                        )
                        {
                            //This pawn is attacking
                            return true;
                        }
                    }
                }

                //Once we've checked one side, check the other
                tmpX = kingX-1;
            }

            //no attacking pawns
            return false;
        }

        // Takes in the expected position of the king, the chessBoard, and the colour of the king.
        // The incY or incX values passed to this function determine which direction to seach in.
        // You can pass in either a -1 (negative direction), 0, or 1 (positive direction)
        // One of either incY or incX must always be set to a value, or this function will never exit the while loop
        private bool searchForLinearAttacks(int incX, int incY, int kingX, int kingY, ChessPiece[,] chessBoard, chessColour kingColour)
        {
            int tmpY, tmpX;

            for (int i = 0; i <= 1; i++)
            {
                tmpY = kingY;
                tmpX = kingX;

                bool bPieceFound = false;

                //First time through we check for vertical attacks going up
                if (i % 2 == 0)
                {
                    //incY and incX should be initially set by caller
                }
                else //Second time through we check for vertical going down
                {
                    incY = -incY;
                    incX = -incX;
                }

                //Check that the values are all on the chessboard
                while ( (tmpY + incY) <= MAX_CHESS_VALUE &&
                        (tmpY + incY) >= MIN_CHESS_VALUE &&
                        (tmpX + incX) <= MAX_CHESS_VALUE &&
                        (tmpX + incX) >= MIN_CHESS_VALUE &&
                        bPieceFound != true)
                {
                    tmpY += incY; 
                    tmpX += incX;

                    //Stop looping when we've found something.  Ignore clear squares
                    //and squares that contain the king we're currently moving.
                    if ( chessBoard[tmpX, tmpY].Type() != chessPieces.CLEAR )
                    {

                        //is the piece an attacking colour?
                        if (chessBoard[tmpX, tmpY].Colour != chessBoard[kingX, kingY].Colour)
                        {
                            //Rooks can only attack in straight lines
                            if ( chessBoard[tmpX, tmpY].Type() == chessPieces.ROOK )    
                            {
                                //Either incY or incX must be zero for a straight line
                                if (incY == 0 || incX == 0)
                                {
                                    return true;
                                }
                            }
                            //Bishops can only attack in diagonals
                            else if (chessBoard[tmpX, tmpY].Type() == chessPieces.BISHOP)
                            {
                                //Both incY and incX must be one for a diagonal attack
                                if (Math.Abs(incY) == Math.Abs(incX))
                                {
                                    return true;
                                }
                            }
                            else if (chessBoard[tmpX, tmpY].Type() == chessPieces.QUEEN)
                            {
                                //Any linear attack is valid for the queen
                                return true;
                            }
                            else if (chessBoard[tmpX, tmpY].Type() == chessPieces.KING)
                            {
                                //king only attacks from one space away in any direction
                                if ( (Math.Abs(Math.Abs(tmpX) - Math.Abs(kingX) ) < 1 ) &&
                                     (Math.Abs(Math.Abs(tmpY) - Math.Abs(kingY) ) < 1 ) )
                                {
                                    return true;
                                }
                            }
                        }

                        //We found a blocking piece, so no need to further check positions
                        bPieceFound = true;
                    }
                }
            }

            return false;
        }

        public override chessPieces Type()
        {
            return chessPieces.KING;
        }

        

        //Used for creating a duplicate object
        public override ChessPiece Clone()
        {
            King returnPiece = new King(PositionX, PositionY);
            returnPiece.Colour = Colour;
            returnPiece.CanCastle = CanCastle;
            return returnPiece;
        }
    }

    [Serializable]
    public class Knight : ChessPiece
    {
        //constructors
        public Knight(): base()
        {
        }
        public Knight (int xPos, int yPos) : base (xPos, yPos)
        {
        }

        public override bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //Check that the piece has moved
            if (hasMoved(finalX, finalY) && notAttackingFriendly(finalX, finalY, chessBoard))
            {
                //Knights can move in L shapes on the board, and can jump any piece
                if ( isMovingL(finalX, finalY) )
                {
                    //Check if the move places the king in check
                    if (!isKingInCheckAfterMove(_king, finalX, finalY, chessBoard))
                    {
                        //safe to move!
                        return true;
                    }
                }
            }
            return false;
        }

        public override chessPieces Type()
        {
            return chessPieces.KNIGHT;
        }

        //Used for creating a duplicate object
        public override ChessPiece Clone()
        {
            Knight returnPiece = new Knight(PositionX, PositionY);
            returnPiece.Colour = Colour;
            return returnPiece;
        }
    }

    [Serializable]
    public class Bishop : ChessPiece
    {
        //constructors
        public Bishop(): base()
        {
        }
        public Bishop (int xPos, int yPos) : base (xPos, yPos)
        {
            //Bishop can only move in diagonals
        }

        public override bool isValidMove(int finalX, int finalY, ChessPiece[,] chessBoard)
        {
            //Check that the piece has moved and is not attempting to take a piece of the same colour
            if (hasMoved(finalX, finalY) && notAttackingFriendly(finalX, finalY, chessBoard))
            {
                //bishops can move diagonally only
                if ( isDiagonal(finalX, finalY, chessBoard) )
                {
                    //Check if the move places the king in check
                    if (!isKingInCheckAfterMove(_king, finalX, finalY, chessBoard))
                    {
                        //safe to move!
                        return true;
                    }
                }
            }
            return false;
        }

        public override chessPieces Type()
        {
            return chessPieces.BISHOP;
        }

        //Used for creating a duplicate object
        public override ChessPiece Clone()
        {
            Bishop returnPiece = new Bishop(PositionX, PositionY);
            returnPiece.Colour = Colour;
            return returnPiece;
        }
    }
}
