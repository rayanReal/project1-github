using System;
using System.Collections.Generic;
using System.Linq;
//Things to do:
//Find out way to keep track of value of board.
//Find out how to Castle Move
//Finish Pawn: En Passant
//Finish Knight:
//Find out how to do Undo Move

/// <summary>
/// This
/// </summary>
namespace Cecs475.BoardGames.Chess {

    public class ChessBoard : IGameBoard {
        /// <summary>
        /// The number of rows and columns on the chess board.
        /// </summary>
        public const int BOARD_SIZE = 8;
        // Reminder: there are 3 different types of rooks
        private sbyte[,] mBoard = new sbyte[8, 8] {
            {-2, -4, -5, -6, -7, -5, -4, -3 },
            {-1, -1, -1, -1, -1, -1, -1, -1 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {1, 1, 1, 1, 1, 1, 1, 1 },
            {2, 4, 5, 6, 7, 5, 4, 3 }
        };

        
        private bool king1Moved = false;
        private bool king2Moved = false;

        private bool rookK1Moved = false;
        private bool rookK2Moved = false;

        private bool rookQ1Moved = false;
        private bool rookQ2Moved = false;

        public bool IsCheck {
            get {
                int player = 1; //default board state player
                int otherPlayer = 2;
                List<BoardPosition> threatenedPos = GetThreatenedPositions(otherPlayer) as List<BoardPosition>;
                if (MoveHistory.Count > 0) {
                    ChessMove lastMove = MoveHistory.Last() as ChessMove;
                    player = lastMove.Piece.Player;  
                    List<ChessMove> moves = GetPossibleMoves() as List<ChessMove>;
                    threatenedPos = GetThreatenedPositions(player) as List<BoardPosition>;
                    if (moves.Count != 0 && threatenedPos.Any((pos => GetPieceAtPosition(pos).PieceType.Equals(ChessPieceType.King))))
                        return true;
                    else return false;
                 }
                else if (threatenedPos.Any((pos => GetPieceAtPosition(pos).PieceType.Equals(ChessPieceType.King))))
                    return true;
                else return false;
            }
        }
        public bool IsCheckmate {
            get {
                if (MoveHistory.Count > 0) {
                    ChessMove lastMove = MoveHistory.Last() as ChessMove;
                    int player = lastMove.Piece.Player;
                    List<ChessMove> moves = GetPossibleMoves() as List<ChessMove>;
                    List<BoardPosition> threatenedPos = GetThreatenedPositions(player) as List<BoardPosition>;
                    if (moves.Count == 0 && threatenedPos.Any((pos => GetPieceAtPosition(pos).PieceType.Equals(ChessPieceType.King))))
                        return true;
                    else return false;
                }
                else return false;
            }
        }
        public bool IsStalemate {
            get {
                if (MoveHistory.Count > 0) {
                    ChessMove lastMove = MoveHistory.Last() as ChessMove;
                    BoardPosition endp = lastMove.EndPosition;
                    int player = GetPlayerAtPosition(endp);
                    int otherplayer = (player == 2) ? 1 : 2;
                    int cp = currentPlayer;
                    List<ChessMove> moves = GetPossibleMoves() as List<ChessMove>;
                    List<BoardPosition> threatenedPos = GetThreatenedPositions(player) as List<BoardPosition>;

                    if (!IsCheck && moves.Count == 0 && !threatenedPos.Any((pos => GetPieceAtPosition(pos).PieceType == ChessPieceType.King)))
                        return true;
                    else return false;
                }
                else return false;
            }
        }

        // TODO:
        // You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
        // if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
        // happened. There are several ways to do it and I leave it up to you.

        /// <summary>
        /// Constructs a new chess board with the default starting arrangement.
        /// </summary>
        public ChessBoard() {
            MoveHistory = new List<IGameMove>();
            // TODO:
            // Finish any other one-time setup.
            var board = mBoard;
            currentPlayer = 1; 
            Value = 0; 
        }

		/// <summary>
		/// Constructs a new chess board by only placing pieces as specified.
		/// </summary> 
		/// <param name  = "startingPositions" > a sequence of tuple pairs, where each pair specifies the starting
		/// position of a particular piece to place on the board</param>
		public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions) : this() { // NOTE THAT THIS CONSTRUCTOR CALLS YOUR DEFAULT CONSTRUCTOR FIRST
			foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
				foreach (int j in Enumerable.Range(0, 8)) {
					mBoard[i, j] = 0;
				}
			}
			foreach (var pos in startingPositions) {
				SetPosition(pos.Item1, pos.Item2);
			}
		}       

		/// <summary>
		/// A difference in piece values for the pieces still controlled by white vs. black, where
		/// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
		/// </summary>
        public ChessBoard(int value) {
            this.Value = value;
        }
        
        public int Value { get; private set; }

        private int currentPlayer;
        
		public int CurrentPlayer {
            get { return (currentPlayer > 0) ? 1 : 2; } 
            private set { }
        }
		// An auto-property suffices here.
		public IList<IGameMove> MoveHistory {
			get; private set;
		}

        /// <summary>
        /// Returns the piece and player at the given position on the board.
        /// </summary>
        public ChessPiecePosition GetPieceAtPosition(BoardPosition position) {
			var boardVal = mBoard[position.Row, position.Col];
			return new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[position.Row, position.Col]), 
                                            boardVal > 0 ? 1 : 
                                            boardVal < 0 ? 2 :
                                            0);
		}

		public void ApplyMove(IGameMove move) { 
            ChessMove m = move as ChessMove;
            var startPiece = mBoard[m.StartPosition.Row, m.StartPosition.Col]; //piece being moved
            m.StartPiece = startPiece;
            m.Piece = GetPieceAtPosition(m.StartPosition);
            MoveHistory.Add(m);
            switch (m.MoveType) {
                case ChessMoveType.Normal:
                    sbyte capturedPiece = mBoard[m.EndPosition.Row, m.EndPosition.Col]; //piece being eaten
                    m.Captured = capturedPiece; //looked at in UndoLastMove()
                    //overwrite board where piece was eaten with startpiece
                    mBoard[m.EndPosition.Row, m.EndPosition.Col] = startPiece;
                    //overwrite board at location of startpiece's startpos with zero 
                    mBoard[m.StartPosition.Row, m.StartPosition.Col] = 0;
                    
                    int val = Value;
                    ChessPieceType pT = (ChessPieceType)Math.Abs(capturedPiece);
                    val = GetPieceValue(pT);
                    Value = (capturedPiece > 0) ? Value - val :
                            (capturedPiece < 0) ? Value + val :
                             Value;                   
                    val = Value;

                    if (!(m.Piece.PieceType == ChessPieceType.Pawn && (m.EndPosition.Row == 0 || m.EndPosition.Row == 7)))
                        currentPlayer = -currentPlayer; //switch player

                    if (startPiece == 7)
                        king1Moved = true;
                    else if (startPiece == 3)
                        rookK1Moved = true;
                    else if (startPiece == 2)
                        rookQ1Moved = true;

                    if (startPiece == -7)
                        king2Moved = true;
                    else if (startPiece == -3)
                        rookK2Moved = true;
                    else if (startPiece == -2)
                        rookQ2Moved = true;

                    break;
                case ChessMoveType.EnPassant:
                    if (startPiece > 0) {
                        m.Captured = mBoard[m.EndPosition.Row + 1, m.EndPosition.Col];
                        mBoard[m.EndPosition.Row + 1, m.EndPosition.Col] = 0;
                        Value = Value + 1;
                    }
                    else if (startPiece < 0) {
                        m.Captured = mBoard[m.EndPosition.Row - 1, m.EndPosition.Col];
                        mBoard[m.EndPosition.Row - 1, m.EndPosition.Col] = 0;
                        Value = Value - 1;
                    }
                    mBoard[m.EndPosition.Row, m.EndPosition.Col] = startPiece;
                    mBoard[m.StartPosition.Row, m.StartPosition.Col] = 0;
                    currentPlayer = -currentPlayer;
                    break;
                case ChessMoveType.CastleQueenSide:
                    mBoard[m.EndPosition.Row, m.EndPosition.Col] = startPiece;
                    if (startPiece > 0) {
                        mBoard[7, 2] = 7; //white king
                        mBoard[7, 0] = 0;
                        mBoard[7, 4] = 0;
                        mBoard[7, 3] = 2; //white rook
                        king1Moved = true;
                        rookQ1Moved = true;
                    }
                    else if (startPiece < 0) {
                        mBoard[0, 3] = -2; //black rook
                        mBoard[0, 0] = 0; //where black rook once was
                        mBoard[0, 4] = 0; 
                        mBoard[0, 2] = -7; //black king
                        king2Moved = true;
                        rookQ2Moved = true;
                    }
                    currentPlayer = -currentPlayer;
                    break;
                case ChessMoveType.CastleKingSide:
                    mBoard[m.EndPosition.Row, m.EndPosition.Col] = startPiece;
                    if (startPiece > 0) {
                        mBoard[7, 6] = 7; //white king
                        mBoard[7, 7] = 0; //where white rook once was
                        mBoard[7, 5] = 3; //white rook
                        mBoard[7, 4] = 0;
                        king1Moved = true;
                        rookK1Moved = true;
                    }
                    else if (startPiece < 0) {
                        mBoard[0, 5] = -3; //black rook
                        mBoard[0, 7] = 0;
                        mBoard[0, 4] = 0;
                        mBoard[0, 6] = -7; //black king
                        king2Moved = true;
                        rookK2Moved = true;
                    }
                    currentPlayer = -currentPlayer;
                    break;
                case ChessMoveType.PawnPromote:
                    mBoard[m.StartPosition.Row, m.StartPosition.Col] = (sbyte)((startPiece > 0) ? m.EndPosition.Col : 
                                                                       (startPiece < 0) ? -m.EndPosition.Col :
                                                                       0);
                    
                    Value = (m.StartPiece > 0) ? Value + GetPieceValue((ChessPieceType)m.EndPosition.Col) - 1 :
                            (m.StartPiece < 0) ? Value - GetPieceValue((ChessPieceType)m.EndPosition.Col) + 1 :
                            Value;

                    currentPlayer = -currentPlayer;
                    break;
                default:
                    break;       
            }
            
        } 
        public IEnumerable<BoardPosition> GetPawnMoves(BoardPosition position) {
            List<BoardPosition> possPositions = new List<BoardPosition>(); //create list of possible pawn endpositions 

            BoardPosition posD = new BoardPosition(position.Row + 1, position.Col);
            BoardPosition posD2 = new BoardPosition(position.Row + 2, position.Col);
            BoardPosition posLL = new BoardPosition(position.Row + 1, position.Col - 1);
            BoardPosition posLR = new BoardPosition(position.Row + 1, position.Col + 1);
            BoardPosition posU = new BoardPosition(position.Row - 1, position.Col);
            BoardPosition posU2 = new BoardPosition(position.Row - 2, position.Col);
            BoardPosition posUL = new BoardPosition(position.Row - 1, position.Col - 1);
            BoardPosition posUR = new BoardPosition(position.Row - 1, position.Col + 1);
            BoardPosition posL = new BoardPosition(position.Row, position.Col - 1);
            BoardPosition posR = new BoardPosition(position.Row, position.Col + 1);
            BoardPosition enPassant = new BoardPosition(0,0);

            int piece = mBoard[position.Row, position.Col];
            int player = GetPlayerAtPosition(position);

            if (player == 2) { //If Black
                if (PositionInBounds(posD) && PositionIsEmpty(posD)) { //If spot in front is empty and in-bounds                                                                              
                    possPositions.Add(posD); //add this position to possible moves 
                    if (position.Row == 1) { //If at initial location
                        if (PositionInBounds(posD2) && PositionIsEmpty(posD2)) //if two spots ahead is empty
                            possPositions.Add(posD2); //add spot
                    }
                }
                if (PositionInBounds(posLR) && PositionIsEnemy(posLR, player)) 
                    possPositions.Add(posLR);
                if (PositionInBounds(posLL) && PositionIsEnemy(posLL, player))
                    possPositions.Add(posLL);
                //if black pawn is in row 4, check to see if white pawn is to its left or right, then add position in front of white pawn (plus row)
                if (position.Row == 4) {
                    if (MoveHistory.Count() > 0) {
                        ChessMove lastMove = MoveHistory.Last() as ChessMove;
                        BoardPosition lastMoveStart = lastMove.StartPosition;
                        BoardPosition lastMoveEnd = lastMove.EndPosition;
                        if (lastMove.StartPiece == 1 && lastMoveStart.Row == 6 && 
                            (PositionInBounds(posL) && PositionIsEnemy(posL, player) && lastMoveEnd.Equals(posL)) || 
                            (PositionInBounds(posR) && PositionIsEnemy(posR, player) && lastMoveEnd.Equals(posR))) {
                            enPassant = new BoardPosition(lastMoveEnd.Row + 1, lastMoveEnd.Col);
                            possPositions.Add(enPassant);
                        }
                    }
                } 
            }
            if (player == 1) { //If White
                if (PositionInBounds(posU) && PositionIsEmpty(posU)) { //If spot in front is empty and in-bounds                                                                              
                    possPositions.Add(posU); //add this position to possible moves 
                    if (position.Row == 6) { //If at initial location
                        if (PositionInBounds(posU2) && PositionIsEmpty(posU2)) //if two spots ahead is empty
                            possPositions.Add(posU2); //add spot
                    }
                }
                if (PositionInBounds(posUR) && PositionIsEnemy(posUR, player))
                    possPositions.Add(posUR);
                if (PositionInBounds(posUL) && PositionIsEnemy(posUL, player))
                    possPositions.Add(posUL);
                //if white pawn is in row 3, check to see if black pawn is to its left or right, then add position in front of white pawn (plus row)
                if (position.Row == 3) {
                    if (MoveHistory.Count() > 0) {
                        ChessMove lastMove = MoveHistory.Last() as ChessMove;
                        BoardPosition lastMoveStart = lastMove.StartPosition;
                        BoardPosition lastMoveEnd = lastMove.EndPosition;
                        if (lastMove.StartPiece == -1 && lastMoveStart.Row == 1  &&
                            (PositionInBounds(posL) && PositionIsEnemy(posL, player) && lastMoveEnd.Equals(posL)) ||
                            (PositionInBounds(posR) && PositionIsEnemy(posR, player) && lastMoveEnd.Equals(posR))) {
                            enPassant = new BoardPosition(lastMoveEnd.Row - 1, lastMoveEnd.Col);
                            possPositions.Add(enPassant);
                        }
                    }
                }
            }
            return possPositions;
        }

        public IEnumerable<BoardPosition> GetBishopMoves(BoardPosition position) {
            List<BoardPosition> possPositions = new List<BoardPosition>();
            BoardPosition posUR = new BoardPosition(position.Row - 1, position.Col + 1);
            BoardPosition posUL = new BoardPosition(position.Row - 1, position.Col - 1);
            BoardPosition posLR = new BoardPosition(position.Row + 1, position.Col + 1);
            BoardPosition posLL = new BoardPosition(position.Row + 1, position.Col - 1);
            int player = GetPlayerAtPosition(position);
            //upper right
            while (PositionInBounds(posUR)) {
                //While upper right is empty and is in-bounds
                if (PositionIsEnemy(posUR, player)) {
                    possPositions.Add(posUR); //add it
                    break;
                }
                else if (PositionIsEmpty(posUR)) {
                    possPositions.Add(posUR);
                    posUR.Row = posUR.Row - 1;
                    posUR.Col = posUR.Col + 1;
                }
                else break;
            }
            //upper left
            while (PositionInBounds(posUL)) {
                //While upper right is empty and is in-bounds
                if (PositionIsEnemy(posUL, player)) {
                    possPositions.Add(posUL); //add it
                    break;
                }
                else if (PositionIsEmpty(posUL)) {
                    possPositions.Add(posUL);
                    posUL.Row = posUL.Row - 1;
                    posUL.Col = posUL.Col - 1;
                }
                else break;
            }
            //lower right
            while (PositionInBounds(posLR)) {
                //While upper right is empty and is in-bounds
                if (PositionIsEnemy(posLR, player)) {
                    possPositions.Add(posLR); //add it
                    break;
                }
                else if (PositionIsEmpty(posLR)) {
                    possPositions.Add(posLR);
                    posLR.Row = posLR.Row + 1;
                    posLR.Col = posLR.Col + 1;
                }
                else break;
            }
            //lower left
            while (PositionInBounds(posLL)) {
                //While upper right is empty and is in-bounds
                if (PositionIsEnemy(posLL, player)) {
                    possPositions.Add(posLL); //add it
                    break;
                }
                else if (PositionIsEmpty(posLL)) {
                    possPositions.Add(posLL);
                    posLL.Row = posLL.Row + 1;
                    posLL.Col = posLL.Col - 1;
                }
                else break;
            }
            return possPositions;
        }
        
        public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType type, int player) {
<<<<<<< HEAD
            List<BoardPosition> positions = new List<BoardPosition>();
            for (int row = 0; row < 8; row++) {
                for (int col = 0; col < 8; col++) {
                    BoardPosition pos = new BoardPosition(row, col);
                    positions.Add(pos);
                }
            }
            return positions.Where(p => GetPieceAtPosition(p).PieceType == type && GetPlayerAtPosition(p) == player);        
=======
            throw new NotImplementedException();
>>>>>>> 34f886822e3755d035be3b99010268fe8fa98d78
        }

        public IEnumerable<BoardPosition> GetRookMoves(BoardPosition position) {
            List<BoardPosition> possPositions = new List<BoardPosition>();
            BoardPosition posU = new BoardPosition(position.Row - 1, position.Col);
            BoardPosition posD = new BoardPosition(position.Row + 1, position.Col);
            BoardPosition posR = new BoardPosition(position.Row, position.Col + 1);
            BoardPosition posL = new BoardPosition(position.Row, position.Col - 1);
            int player = GetPlayerAtPosition(position);
            //up
            while (PositionInBounds(posU)) {
                if (PositionIsEnemy(posU, player)) {
                    possPositions.Add(posU);
                    break;
                }
                else if (PositionIsEmpty(posU)) {
                    possPositions.Add(posU);
                    posU.Row = posU.Row - 1;
                }
                else break;
            }
            //down
            while (PositionInBounds(posD)) {
                if (PositionIsEnemy(posD, player)) {
                    possPositions.Add(posD);
                    break;
                }
                else if (PositionIsEmpty(posD)) {
                    possPositions.Add(posD);
                    posD.Row = posD.Row + 1;
                }
                else break;
            }
            //left
            while (PositionInBounds(posL)) {
                if (PositionIsEnemy(posL, player)) {
                    possPositions.Add(posL);
                    break;
                }
                else if (PositionIsEmpty(posL)) {
                    possPositions.Add(posL);
                    posL.Col = posL.Col - 1;
                }
                else break;
            }
            //right
            while (PositionInBounds(posR)) {
                if (PositionIsEnemy(posR, player)) {
                    possPositions.Add(posR);
                    break;
                }
                else if (PositionIsEmpty(posR)) {
                    possPositions.Add(posR);
                    posR.Col = posR.Col + 1;
                }
                else break;
            }
            return possPositions;
        }

        public IEnumerable<BoardPosition> GetKnightMoves(BoardPosition position) {
            List<BoardPosition> possPositions = new List<BoardPosition>();
            List<BoardPosition> poss = new List<BoardPosition>();
            //Each possible position for a knight
            BoardPosition posd2r1 = new BoardPosition(position.Row + 2, position.Col + 1);
            BoardPosition posd2l1 = new BoardPosition(position.Row + 2, position.Col - 1);
            BoardPosition posu2r1 = new BoardPosition(position.Row - 2, position.Col + 1);
            BoardPosition posu2l1 = new BoardPosition(position.Row - 2, position.Col - 1);
            BoardPosition posd1r2 = new BoardPosition(position.Row + 1, position.Col + 2);
            BoardPosition posd1l2 = new BoardPosition(position.Row + 1, position.Col - 2);
            BoardPosition posu1r2 = new BoardPosition(position.Row - 1, position.Col + 2);
            BoardPosition posu1l2 = new BoardPosition(position.Row - 1, position.Col - 2); 
            poss.Add(posd2r1);
            poss.Add(posd2l1);
            poss.Add(posu2r1);
            poss.Add(posu2l1);
            poss.Add(posd1r2); 
            poss.Add(posd1l2);
            poss.Add(posu1r2);
            poss.Add(posu1l2);
            int player = GetPlayerAtPosition(position);
            foreach (BoardPosition element in poss) {
                if (PositionInBounds(element)) {
                    if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                        possPositions.Add(element);
                }
            }
            return possPositions;
        }

        public IEnumerable<BoardPosition> GetQueenMoves(BoardPosition position) {
            List<BoardPosition> possPositions = new List<BoardPosition>(); 
            possPositions.AddRange(GetBishopMoves(position));
            possPositions.AddRange(GetRookMoves(position));
            return possPositions;
        }

        public IEnumerable<BoardPosition> GetKingMoves(BoardPosition position) {
            List<BoardPosition> possPositions = new List<BoardPosition>();
            List<BoardPosition> poss = new List<BoardPosition>();
            //Each possible position for a king
            BoardPosition posD = new BoardPosition(position.Row + 1, position.Col);
            BoardPosition posDR = new BoardPosition(position.Row + 1, position.Col + 1); 
            BoardPosition posDL = new BoardPosition(position.Row + 1, position.Col - 1);
            BoardPosition posU = new BoardPosition(position.Row - 1, position.Col);
            BoardPosition posUR = new BoardPosition(position.Row - 1, position.Col + 1);
            BoardPosition posUL = new BoardPosition(position.Row - 1, position.Col - 1);
            BoardPosition posR = new BoardPosition(position.Row, position.Col + 1);
            BoardPosition posL = new BoardPosition(position.Row, position.Col - 1);      
            poss.Add(posD);
            poss.Add(posDR);
            poss.Add(posDL);
            poss.Add(posU);
            poss.Add(posUR);
            poss.Add(posUL);
            poss.Add(posR);
            poss.Add(posL);
            int player = GetPlayerAtPosition(position);
            foreach (BoardPosition element in poss) {
                if (PositionInBounds(element)) {
                    if (PositionIsEmpty(element)) 
                        possPositions.Add(element);
                    else if (PositionIsEnemy(element, player)) 
                        possPositions.Add(element);
                }
            }
            return possPositions;
        }

        public IEnumerable<IGameMove> GetPossibleMoves() {
            List<ChessMove> moves = new List<ChessMove>(); //list of PossibleChessMoves INCLUDING the ones that leave current king in check
            List<BoardPosition> threatenedPos = new List<BoardPosition>(); //list of threatenedPos by OTHER player
            var possEndPositions = new List<BoardPosition>(); //list of possible end positions for each piece
            var a1 = new BoardPosition(7, 0); //white left rook
            var b1 = new BoardPosition(7, 1); //white left knight
            var c1 = new BoardPosition(7, 2); //white left bishop
            var d1 = new BoardPosition(7, 3); //white queen
            var f1 = new BoardPosition(7, 5); //white right bishop
            var g1 = new BoardPosition(7, 6); //white right knight
            var h1 = new BoardPosition(7, 7); //white right rook
            var e1 = new BoardPosition(7, 4); //white king
            int square_c1 = (int)GetPieceAtPosition(c1).PieceType;
            int square_b1 = (int)GetPieceAtPosition(b1).PieceType;
            int square_d1 = (int)GetPieceAtPosition(d1).PieceType;
            int square_a1 = (int)GetPieceAtPosition(a1).PieceType;
            int square_f1 = (int)GetPieceAtPosition(f1).PieceType;
            int square_h1 = (int)GetPieceAtPosition(h1).PieceType;
            int square_e1 = (int)GetPieceAtPosition(e1).PieceType;
            int square_g1 = (int)GetPieceAtPosition(g1).PieceType;
            var a8 = new BoardPosition(0, 0); //left black rook
            var b8 = new BoardPosition(0, 1); //left black knight
            var c8 = new BoardPosition(0, 2); //left black bishop
            var d8 = new BoardPosition(0, 3); //black queen
            var e8 = new BoardPosition(0, 4); //black king
            var f8 = new BoardPosition(0, 5); //right black bishop
            var g8 = new BoardPosition(0, 6); //right black knight
            var h8 = new BoardPosition(0, 7); //right black rook
            int square_c8 = (int)GetPieceAtPosition(c8).PieceType;
            int square_b8 = (int)GetPieceAtPosition(b8).PieceType;
            int square_d8 = (int)GetPieceAtPosition(d8).PieceType;
            int square_a8 = (int)GetPieceAtPosition(a8).PieceType;
            int square_f8 = (int)GetPieceAtPosition(f8).PieceType;
            int square_h8 = (int)GetPieceAtPosition(h8).PieceType;
            int square_e8 = (int)GetPieceAtPosition(e8).PieceType;
            int square_g8 = (int)GetPieceAtPosition(g8).PieceType;


            if (MoveHistory.Count > 0) { //if there was a last move
                ChessMove move = MoveHistory.Last() as ChessMove; //store it
                if (move.Piece.PieceType.Equals(ChessPieceType.Pawn) && //if the last piece moved was a pawn
                    (move.EndPosition.Row == 0 && currentPlayer == 1 || 
                     move.EndPosition.Row == 7 && currentPlayer == -1)) { //or if black, and was in initial position 
                    //return 4 Pawn Promotion Moves
                    ChessMove p1 = new ChessMove(move.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Knight), ChessMoveType.PawnPromote);
                    ChessMove p2 = new ChessMove(move.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Bishop), ChessMoveType.PawnPromote);
                    ChessMove p3 = new ChessMove(move.EndPosition, new BoardPosition(-1, (int)ChessPieceType.RookPawn), ChessMoveType.PawnPromote);
                    ChessMove p4 = new ChessMove(move.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Queen), ChessMoveType.PawnPromote);
                    moves.Add(p1);
                    moves.Add(p2);
                    moves.Add(p3);
                    moves.Add(p4);
                    return moves;
                }
            }
            int player = 0;
            int piece = 0;
            for (int row = 0; row < BOARD_SIZE; row++) { //for each row
                for (int col = 0; col < BOARD_SIZE; col++) {  //while in each row, check each column
                    //For each square, determine piece 
                    var cp = currentPlayer;
                    var startPosition = new BoardPosition(row, col);
                    piece = (int)mBoard[startPosition.Row, startPosition.Col];
                    int currentPiece = piece;
                    player = GetPlayerAtPosition(startPosition);
                    if (piece > 0 && currentPlayer > 0 || piece < 0 && currentPlayer < 0) { //if piece is Currrent piece
                        piece = Math.Abs(piece);
                        switch (piece) {
                            case 0:
                                //Empty 
                                break;
                            case 1:
                                //Pawn
                                possEndPositions = GetPawnMoves(startPosition) as List<BoardPosition>;                      
                                if (possEndPositions.Count > 0) {
                                    foreach (BoardPosition element in possEndPositions) {
                                        if (PositionIsEmpty(element) && element.Col != startPosition.Col)
                                            moves.Add(new ChessMove(startPosition, element, ChessMoveType.EnPassant));
                                        else if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                            moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                    }
                                }
<<<<<<< HEAD
                                break;
                            case 2:
                            case 3:
                            case 8:
=======

                                break;
                            case 2:
>>>>>>> 34f886822e3755d035be3b99010268fe8fa98d78
                                //RookQueen
                                possEndPositions = GetRookMoves(startPosition) as List<BoardPosition>;
                                if (possEndPositions.Count > 0) {
                                    foreach (BoardPosition element in possEndPositions) {
                                        if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                            moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                    }
                                }
                                break;
<<<<<<< HEAD
                            /*case 3:
=======
                            case 3:
>>>>>>> 34f886822e3755d035be3b99010268fe8fa98d78
                                //RookKing
                                possEndPositions = GetRookMoves(startPosition) as List<BoardPosition>;
                                if (possEndPositions.Count > 0) {
                                    foreach (BoardPosition element in possEndPositions) {
                                        if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                            moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                    }
                                }
                                break;
<<<<<<< HEAD
                            */
                                case 4:
=======
                            case 4:
>>>>>>> 34f886822e3755d035be3b99010268fe8fa98d78
                                //Knight
                                possEndPositions = GetKnightMoves(startPosition) as List<BoardPosition>;
                                foreach (BoardPosition element in possEndPositions) {
                                    if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                        moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                }
                                break;
                            case 5:
                                //Bishop
                                possEndPositions = GetBishopMoves(startPosition) as List<BoardPosition>;
                                foreach (BoardPosition element in possEndPositions) {
                                    if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                        moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                }
                                break;
                            case 6:
                                //Queen
                                possEndPositions = GetQueenMoves(startPosition) as List<BoardPosition>;
                                foreach (BoardPosition element in possEndPositions) {
                                    if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                        moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                }
                                break;
                            case 7:
                                //King
<<<<<<< HEAD
                                int otherPlayer = (currentPiece > 0) ? 2 : 1;       
=======
                                int otherPlayer = (currentPiece > 0) ? 2 : 1;
        
>>>>>>> 34f886822e3755d035be3b99010268fe8fa98d78
                                possEndPositions = GetKingMoves(startPosition) as List<BoardPosition>;
                                var cm = MoveHistory;
                                if (currentPlayer == 1) {
                                    foreach (BoardPosition element in possEndPositions) {
                                        if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                            moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                    }
                                    if (!king1Moved) {
                                        if (!rookQ1Moved) {
                                            if (startPosition.Equals(e1) && mBoard[7, 0] == 2 && square_c1 == 0 && square_b1 == 0 && square_d1 == 0) {
                                                if (!cm.Where(m => ((ChessMove)m).StartPiece == 7 || ((ChessMove)m).StartPiece == 2).Any()) {
                                                    //get all positions that the OTHER player threatens after move is applied
                                                    threatenedPos = GetThreatenedPositions(otherPlayer) as List<BoardPosition>;
                                                    //king cannot castle if other player threatenes squares of the castling king's row.
                                                    if (!threatenedPos.Contains(c1) && !threatenedPos.Contains(d1) && !threatenedPos.Contains(e1))
                                                        moves.Add(new ChessMove(startPosition, c1, ChessMoveType.CastleQueenSide));
                                                }
                                            }
                                        }
                                        if (!rookK1Moved) {
                                            //check if positions in between rook and king are empty
                                            if (startPosition.Equals(e1) && square_f1 == 0 && square_g1 == 0 && mBoard[7, 7] == 3) {
                                                if (!cm.Where(m => ((ChessMove)m).StartPiece == 7 || ((ChessMove)m).StartPiece == 3).Any()) {
                                                    threatenedPos = GetThreatenedPositions(otherPlayer) as List<BoardPosition>;
                                                    //king cannot castle if other player threatenes squares of the castling king's row.
                                                    if (!threatenedPos.Contains(f1) && !threatenedPos.Contains(e1))
                                                        moves.Add(new ChessMove(startPosition, g1, ChessMoveType.CastleKingSide));
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (currentPlayer == -1) {
                                    foreach (BoardPosition element in possEndPositions) {
                                        if (PositionIsEmpty(element) || PositionIsEnemy(element, player))
                                            moves.Add(new ChessMove(startPosition, element, ChessMoveType.Normal));
                                    }
                                    if (!king2Moved) {
                                        if (!rookK2Moved) {
                                            if (startPosition.Equals(e8) && mBoard[0, 7] == -3 && square_f8 == 0 && square_g8 == 0) {
                                                int count = MoveHistory.Count();
                                                if (!cm.Where(m => ((ChessMove)m).StartPiece == -7 || ((ChessMove)m).StartPiece == -3).Any()) {
                                                    threatenedPos = GetThreatenedPositions(otherPlayer) as List<BoardPosition>;
                                                    //king cannot castle if other player threatenes squares of the castling king's row.
                                                    if (!threatenedPos.Contains(f8) && !threatenedPos.Contains(e8))
                                                        moves.Add(new ChessMove(startPosition, g8, ChessMoveType.CastleKingSide));
                                                }
                                            }
                                        }
                                        if (!rookQ2Moved) {
                                            if (startPosition.Equals(e8) && mBoard[0, 0] == -2 && square_b8 == 0 && square_c8 == 0 && square_d8 == 0) {
                                                if (!cm.Where(m => ((ChessMove)m).StartPiece == -7 || ((ChessMove)m).StartPiece == -2).Any()) {
                                                    threatenedPos = GetThreatenedPositions(otherPlayer) as List<BoardPosition>;
                                                    //king cannot castle if other player threatenes squares of the castling king's row.
                                                    if (!threatenedPos.Contains(d8) && !threatenedPos.Contains(c8) && !threatenedPos.Contains(e8))
                                                        moves.Add(new ChessMove(startPosition, c8, ChessMoveType.CastleQueenSide));
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                //Empty
                                break;
                        }// end of switch(piece)
                    } //end of checking whether piece == CurrentPlayer                  
                } //end of column search
            } //end of row search      
            List<ChessMove> real = new List<ChessMove>(); //list of ACTUAL possible moves
             foreach (ChessMove element in moves) {
                BoardPosition start = element.StartPosition;
                BoardPosition end = element.EndPosition;
                int currentPiece = (int)this.mBoard[start.Row, start.Col];
                ApplyMove(element); //apply each move in UNFINISHED moves list
                //if the piece value is positive(WHITE) then the other player was BLACK 
                int otherPlayer = (currentPiece > 0) ? 2 : 1;
                //get all positions that the OTHER player threatens after move is applied
                threatenedPos = GetThreatenedPositions(otherPlayer) as List<BoardPosition>;
                //if the other player has no moves that could threaten your king, add the move in "moves" to the REAL moves list (it is a valid move!)
                if (!threatenedPos.Any(pos => GetPieceAtPosition(pos).PieceType.Equals(ChessPieceType.King))) {
                    real.Add(element); 
                }
                //Restore board state
                UndoLastMove();    
            }
            return real;
        }   //end of GetPossibleMoves
        /// <summary>
        /// Gets a sequence of all positions on the board that are threatened by the given player. A king
        /// may not move to a square threatened by the opponent.
        /// </summary>
        public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
            var threatPositions = new List<BoardPosition>();
            var threatenedPositions = new List<BoardPosition>();         
            for (int row = 0; row < BOARD_SIZE; row++) {
                for (int col = 0; col < BOARD_SIZE; col++) {
                    //For each square, determine piece 
                    var pos = new BoardPosition(row, col);
                    int piece = Math.Abs(mBoard[row, col]);
                    if (byPlayer == GetPlayerAtPosition(pos)) { //Look at byPlayer's pieces...
                        switch (piece) {
                            case 0:
                                //Empty
                                break;
                            case 1:
                                //Pawn
                                threatPositions = GetPawnMoves(pos) as List<BoardPosition>;                              
                                break;
                            case 2:
                                //RookQueen
                                threatPositions = GetRookMoves(pos) as List<BoardPosition>;                            
                                break;
                            case 3:
                                //RookKing
                                threatPositions = GetRookMoves(pos) as List<BoardPosition>;                          
                                break;
                            case 4:
                                //Knight
                                threatPositions = GetKnightMoves(pos) as List<BoardPosition>;                              
                                break;
                            case 5:
                                //Bishop
                                threatPositions = GetBishopMoves(pos) as List<BoardPosition>;                              
                                break;
                            case 6:
                                //Queen
                                threatPositions = GetQueenMoves(pos) as List<BoardPosition>;                          
                                break;
                            case 7: 
                                //King
                                threatPositions = GetKingMoves(pos) as List<BoardPosition>;
                                break;
                            default:
                                //Empty    
                                break;
                        }
                        threatenedPositions.AddRange(threatPositions);
                    }
                }
            }     
            // .WriteLine("# of GetThreatenedPos: " +threatenedPositions.Count);
            return threatenedPositions;
        }//end GetThreatenedPositions  
        
		public void UndoLastMove() {
            if (MoveHistory.Count == 0) {
                throw new Exception();
            } 
            ChessMove lastMove = MoveHistory.Last() as ChessMove;
            MoveHistory.RemoveAt(MoveHistory.Count - 1);
            if (lastMove.MoveType == ChessMoveType.CastleKingSide) {
                if (lastMove.StartPiece > 0) {
                    mBoard[7, 4] = 7; //return king
                    mBoard[7, 6] = 0; //spot where king went is now empty again
                    mBoard[7, 5] = 0; //spot where rook went is now empty again
                    mBoard[7, 7] = 3; //return white right rook to original position
                    king1Moved = false;
                    rookK1Moved = false; 
                }
                else {
                    mBoard[0, 4] = -7;
                    mBoard[0, 5] = 0;
                    mBoard[0, 6] = 0;
                    mBoard[0, 7] = -3;
                    king2Moved = false;
                    rookK2Moved = false;
                }
                currentPlayer = -currentPlayer;
            }
            else if (lastMove.MoveType == ChessMoveType.CastleQueenSide) {
                if (lastMove.StartPiece > 0) {
                    mBoard[7, 4] = 7; //overwrite white left rook with king
                    mBoard[7, 3] = 0; //overwrite post-castle king position with 0
                    mBoard[7, 2] = 0;
                    mBoard[7, 0] = 2; //return white right rook to original position
                    king1Moved = false;
                    rookQ1Moved = false;
                }
                else {
                    mBoard[0, 4] = -7;
                    mBoard[0, 2] = 0;
                    mBoard[0, 0] = -2;
                    mBoard[0, 3] = 0;
                    king2Moved = false;
                    rookQ2Moved = false;
                }
                currentPlayer = -currentPlayer;
            }
            else if (lastMove.MoveType == ChessMoveType.PawnPromote) {
                if (lastMove.StartPiece > 0) {
                    mBoard[lastMove.StartPosition.Row, lastMove.StartPosition.Col] = lastMove.StartPiece;
                    Value = Value - GetPieceValue((ChessPieceType)lastMove.EndPosition.Col) + 1;
                }
                else {
                    mBoard[lastMove.StartPosition.Row, lastMove.StartPosition.Col] = lastMove.StartPiece;
                    Value = Value + GetPieceValue((ChessPieceType)lastMove.EndPosition.Col) - 1;
                }
                currentPlayer = -currentPlayer;
            }   
            else if (lastMove.MoveType == ChessMoveType.EnPassant) {
                mBoard[lastMove.StartPosition.Row, lastMove.StartPosition.Col] = lastMove.StartPiece;
                mBoard[lastMove.EndPosition.Row, lastMove.EndPosition.Col] = 0;
                if (lastMove.StartPiece < 0) {
                    mBoard[lastMove.EndPosition.Row - 1, lastMove.EndPosition.Col] = (sbyte)lastMove.Captured;
                    Value = Value + 1;
                }
                else if (lastMove.StartPiece > 0) {
                    mBoard[lastMove.EndPosition.Row + 1, lastMove.EndPosition.Col] = (sbyte)lastMove.Captured;
                    Value = Value - 1;
                }
                currentPlayer = -currentPlayer;
            }
            else {
                BoardPosition lastMoveStart = lastMove.StartPosition; //last move's starting postion
                BoardPosition lastMoveEnd = lastMove.EndPosition; //last move's end position
                sbyte startPiece = mBoard[lastMoveEnd.Row, lastMoveEnd.Col]; //piece that was moved
                mBoard[lastMoveStart.Row, lastMoveStart.Col] = startPiece; //put it back to original location
                var captured = lastMove.Captured;
                //Reassign current Chessboard at the last move's End Position with the piece that was last captured
                mBoard[lastMoveEnd.Row, lastMoveEnd.Col] = (sbyte)captured;      
                Value = (captured > 0) ? Value + GetPieceValue((ChessPieceType)Math.Abs(captured)) :
                        (captured < 0) ? Value - GetPieceValue((ChessPieceType)Math.Abs(captured)) :
                         Value;
                
                if (!(lastMove.Piece.PieceType == ChessPieceType.Pawn && (lastMove.EndPosition.Row == 0 || lastMove.EndPosition.Row == 7)))
                    currentPlayer = -currentPlayer;
              
                
                switch (lastMove.StartPiece) {
                    case 7:
                        if (lastMove.StartPosition.Equals(new BoardPosition(7, 4)))
                            king1Moved = false;
                        break;
                    case -7:
                        if (lastMove.StartPosition.Equals(new BoardPosition(0, 4)))
                            king2Moved = false;
                        break;
                    case 3:
                        if (lastMove.StartPosition.Equals(new BoardPosition(7, 7)))
                            rookK1Moved = false;
                        break;
                    case -3:
                        if (lastMove.StartPosition.Equals(new BoardPosition(0, 7)))
                            rookK2Moved = false;
                        break;
                    case 2:
                        if (lastMove.StartPosition.Equals(new BoardPosition(7, 0)))
                            rookQ1Moved = false;
                        break;
                    case -2:
                        if (lastMove.StartPosition.Equals(new BoardPosition(0, 0)))
                            rookQ2Moved = false;
                        break;         
                }    
            }
        }
		/// <summary>
		/// Returns true if the given position on the board is empty.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEmpty(BoardPosition pos) {
            if (GetPieceAtPosition(pos).PieceType == 0) 
                return true;
            else return false;
		}
		/// <summary>
		/// Returns true if the given position contains a piece that is the enemy of the given player.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEnemy(BoardPosition pos, int player) {
            int p = GetPlayerAtPosition(pos);
            if ((player == 1 && p == 2) || 
                (player == 2 && p == 1)) 
                return true; 
            else return false; 
		}
		/// <summary>
		/// Returns true if the given position is in the bounds of the board.
		/// </summary> 
		public static bool PositionInBounds(BoardPosition pos) {
            if ((pos.Row < 0 || pos.Row > 7) || (pos.Col < 0 || pos.Col > 7))
                return false;
            else return true; 
		}
		/// <summary>
		/// Returns which player has a piece at the given board position, or 0 if it is empty.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition pos) {
            int boardval = (int)mBoard[pos.Row, pos.Col];
            return (boardval > 0) ? 1 :
                   (boardval < 0) ? 2 :
                   0;
		}
		/// <summary>
		/// Gets the value weight for a piece of the given type.
		/// </summary>
		/*
		 * VALUES:
		 * Pawn: 1
		 * Knight: 3
		 * Bishop: 3
		 * Rook: 5
		 * Queen: 9
		 * King: infinity (maximum integer value)
		 */
        public int GetPieceValue(ChessPieceType pieceType) {
            string pT = pieceType.ToString();
            int pieceVal = 0;
            switch (pT) {
                case "Empty":
                    pieceVal = 0;
                    break;
<<<<<<< HEAD
                case "RookPawn":
                    pieceVal = 5;
                    break;
=======
>>>>>>> 34f886822e3755d035be3b99010268fe8fa98d78
                case "Pawn":
                    pieceVal = 1;
                    break;
                case "RookQueen":
                    pieceVal = 5;
                    break;
                case "RookKing":
                    pieceVal = 5;
                    break;
                case "Knight":
                    pieceVal = 3;
                    break;
                case "Bishop":
                    pieceVal = 3;
                    break;
                case "Queen":
                    pieceVal = 9;
                    break;
                case "King":
                    pieceVal = 10;
                    break;
            }
            return pieceVal;
		}
		/// <summary>
		/// Manually places the given piece at the given position.
		/// </summary>
		// This is used in the constructor
		private void SetPosition(BoardPosition position, ChessPiecePosition piece) {
			mBoard[position.Row, position.Col] = (sbyte)((int)piece.PieceType * (piece.Player == 2 ? -1 : piece.Player));
		}
	}
}