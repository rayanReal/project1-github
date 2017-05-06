using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class RookTests : ChessTests {
		/// <summary>
		/// Initial board rook count. Testing initial starting board state.
		/// </summary>
		[Fact]
		public void InitialBoardRookCount() {
			ChessBoard b = new ChessBoard();


			var player1Pieces = GetAllPiecesForPlayer(b, 1);
			var player2Pieces = GetAllPiecesForPlayer(b, 2);

			var rookCount1 = player1Pieces.Where(p => p.PieceType == ChessPieceType.RookKing || p.PieceType == ChessPieceType.RookQueen).Count();
			var rookCount2 = player2Pieces.Where(p => p.PieceType == ChessPieceType.RookKing || p.PieceType == ChessPieceType.RookQueen).Count();

			(rookCount1 + rookCount2).Should().Be(4, "the board starts out with 4 rooks");
		}

		/// <summary>
		/// Rook's possible moves. Tests the possible moves of different rooks in several situations.
		/// </summary>
		[Fact]
		public void RookPossibleMoves() {
			// Create board with rooks and pawns in different positions for rook testing
			ChessBoard b = CreateBoardWithPositions(
				Pos("a1"), ChessPieceType.RookQueen, 1,
				Pos("h1"), ChessPieceType.RookKing, 1,
				Pos("c3"), ChessPieceType.Pawn, 1,
				Pos("d6"), ChessPieceType.Pawn, 1,
				Pos("c6"), ChessPieceType.RookQueen, 2,
				Pos("g3"), ChessPieceType.RookKing, 2,
				Pos("h5"), ChessPieceType.Pawn, 2,
				Pos("e8"), ChessPieceType.King, 2);

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var possMovesRQ1 = GetMovesAtPosition(possMoves, Pos("a1"));
			var possMovesRK1 = GetMovesAtPosition(possMoves, Pos("h1"));

			// Arbitrary move to change players
			ApplyMove(b, Move("a1, a2"));
			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			var possMovesRQ2 = GetMovesAtPosition(possMoves, Pos("c6"));
			var possMovesRK2 = GetMovesAtPosition(possMoves, Pos("g3"));

			// RookQueen1 (RQ1) moves for forward until end and right until rook -> 13
			possMovesRQ1.Should().HaveCount(13, "queen rook for player 1 should be able to move forward until end and right to other rook");

			// RookKing1 (RK1) moves forward until pawn and left until rook -> 10
			possMovesRK1.Should().HaveCount(10, "king rook for player 1 should be able to move forward until hitting pawn and right until other rook");

			// RQ2 moves up and left until end, then right and down until pawns -> 8
			possMovesRQ2.Should().HaveCount(8, "queen rook for player 2 should be able to move up and left until end, then right and down until pawns");

			// RK2 moves up, down, and right until end and left until pawn -> 12
			possMovesRK2.Should().HaveCount(12, "king rook for player 2 should be able to move up, down, and right until end and left until pawn");
		}

		[Fact]
		public void CastlingAndEnPassantWhite() {//Tricky moves:En passant & castling for white side  with queen side rook
			ChessBoard c = CreateBoardFromMoves(new ChessMove[]
			{
				Move("c2" , "c4"),
				Move("c7" , "c5"),
				Move("b2" , "b4"),
				Move("b7" , "b5"),
				Move("d2" , "d4"),
				Move("d7" , "d5"),
				Move("d1" , "a4"),
				Move("d8" , "a5"),
				Move("c1" , "f4"),
				Move("c8" , "f5"),
				Move("b1" , "a3"),
				Move("b8" , "a6")
			});
			var poss1 = c.GetPossibleMoves() as IEnumerable<ChessMove>;
			var castling = GetMovesAtPosition(poss1, Pos("e1"));
			castling.Should().Contain(Move("e1", "c1"), "King and rook both haven't moved and the spaces between them are clear to allow castling");
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("g2", "g4"),
					 Move("a7", "a6"),
					 Move("g4", "g5"),
					 Move("h7", "h5")
				});
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var enpassant = GetMovesAtPosition(poss, Pos("g5"));
			enpassant.Should().Contain(Move("g5", "h6"), "Enemy pawn moved 2 spaces and fits criteria for en passant");
		}

		[Fact]
		public void CastlingAndEnPassantBlack() {//Tricky moves2:En passant & castling for black side with king side rook and undoes it.
			ChessBoard c = CreateBoardFromMoves(new ChessMove[]
			{
					 Move("b2" , "b4"),
					 Move("f7" , "f5"),
					 Move("b4", "b5"),
					 Move("f5", "f4"),
					 Move("a2", "a3"),
					 Move("e7", "e5"),
					 Move("h2", "h4"),
					 Move("f8", "a3")
			});
			c.Value.Should().Be(-1, "White pawn was captured.");
			ApplyMove(c, Move("b1", "a3"));
			c.Value.Should().Be(2, "Black bishop was captured.");
			ApplyMove(c, Move("g8", "h6"));
			ApplyMove(c, Move("a3", "c4"));
			ApplyMove(c, Move("e8", "g8"));
			var king = c.GetPieceAtPosition(Pos("g8"));
			king.PieceType.Should().Be(ChessPieceType.King, "King has applied castling with king side rook.");
			var rook = c.GetPieceAtPosition(Pos("f8"));
			rook.PieceType.Should().Be(ChessPieceType.RookKing, "King side Rook has applied castling with king.");
			c.UndoLastMove();
			c.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "King returns to original position after undo");
			c.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.RookKing, "Rook returns to original position after undo");
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("g2", "g4"),
					 Move("a7", "a5"),
					 Move("g4", "g5"),
					 Move("a5", "a4"),
					 Move("b2", "b4"),
				});
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var enpassant = GetMovesAtPosition(poss, Pos("a4"));
			enpassant.Should().Contain(Move("a4", "b3"), "Enemy pawn moved 2 spaces and fits criteria for en passant").And.HaveCount(2, "Two available moves for pawn: forward and en passant");
		}

		//Test of initial state
		//Test 1: Check starting position of rooks (white)
		[Fact]
		public void RookStartingPositionsTest() {
			ChessBoard b = new ChessBoard();
			//Queen-side Rooks should be on column 0 (a)
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Rook at position (0, 0)");
			b.GetPieceAtPosition(Pos(7, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Rook at position (7, 0)");
			//King-side Rooks shoyuld be on column 7 (h)
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Rook at position (0, 7)");
			b.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Rook at position (7, 7)");
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var rookMove = GetMovesAtPosition(poss, Pos("a1"));
			rookMove.Should().HaveCount(0, "Rook at start of game cannot move");
			poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			rookMove = GetMovesAtPosition(poss, Pos("h1"));
			rookMove.Should().BeEmpty("Rook at start of game cannot move");
		}

		//Test using GetPossibleMoves
		//Test 5: Get possible moves for rook in the given position (white)
		[Fact]
		public void GetMovesForRookTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("a2", "a4"),
				Move("c7", "c5"),
				Move("a1", "a3"),
				Move("b7", "b6"),
				Move("a3", "e3"),
				Move("c8", "a6"),
			});

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var rookMoves = GetMovesAtPosition(poss, Pos("e3"));
			rookMoves.Should().HaveCount(11, "The rook should have 11 possible moves after the given moves.");
		}

		/// <summary>
		/// Test 5 Get the possible move for white's rook in position a1.
		/// </summary>
		[Fact]
		public void RookGetMove() {
			ChessBoard b = new ChessBoard();
			// Move white's pawn 2 spaces.
			ApplyMove(b, Move("a2", "a4"));
			b.GetPieceAtPosition(Pos(4, 0)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at postition (4,0)");
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			// Move black's pawn 1 space.
			ApplyMove(b, Move("b7", "b6"));
			b.GetPieceAtPosition(Pos(2, 1)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at postition (2,1)");
			// Player 1's turn.
			b.CurrentPlayer.Should().Be(1);
			// Get all the possible moves for white's rook which should be a2 and a3.
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var twoMovesExpected = GetMovesAtPosition(poss, Pos("a1"));
			twoMovesExpected.Should().Contain(Move("a1", "a2")).And.Contain(Move("a1", "a3"));
		}

		/// <summary>
		/// RookCaptureTest for Rook Capture
		/// </summary>
		[Fact]
		public void RookCapture() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("f5"), ChessPieceType.Pawn, 1,
				 Pos("e7"), ChessPieceType.Pawn, 2,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("f2"), ChessPieceType.RookKing, 2,
                 Pos("d4"), ChessPieceType.Pawn, 2,
                 Pos("c5"), ChessPieceType.Queen, 1
                 );
            ApplyMove(b, Move("c5, d5"));
            b.CurrentPlayer.Should().Be(2);
            ApplyMove(b, Move("f2, f5"));

        }

		/// <summary>
		/// Check the Bishop catch the rook with right value ,and undo move.
		/// </summary>
		[Fact]
		public void RookCapturingBishopWithUndo() {
			ChessBoard b = CreateBoardWithPositions( // create the board
				 Pos("b2"), ChessPieceType.Bishop, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("g8"), ChessPieceType.RookKing, 2,
				 Pos("e8"), ChessPieceType.King, 2);

			b.Value.Should().Be(0, "Not captured yet");
			ApplyMove(b, Move("b2, g7"));
			ApplyMove(b, Move("g8, g7"));
			b.Value.Should().Be(-b.GetPieceValue(ChessPieceType.Bishop), "captured a White Bishop");
			b.UndoLastMove();
			b.Value.Should().Be(0, "undid the capture");
		}

		/// <summary>
		/// This function checks to see that the rooks
		/// are in their initial position, the 4 corners. 
		/// </summary>
		[Fact]
		public void RooksInitialBoardTest() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			var whiteQueenRook = b.GetPieceAtPosition(Pos("a1")).PieceType.Should()
				 .Be(ChessPieceType.RookQueen, "because the rook is on the king's left side");
			var whiteKingRook = b.GetPieceAtPosition(Pos("h1")).PieceType.Should()
				 .Be(ChessPieceType.RookKing, "because the rook is on the king's right side");

			var blackQueenRook = b.GetPieceAtPosition(Pos("a8")).PieceType.Should()
				 .Be(ChessPieceType.RookQueen, "because the rook is on the king's left side");
			var blackKingRook = b.GetPieceAtPosition(Pos("h8")).PieceType.Should()
				 .Be(ChessPieceType.RookKing, "because the rook is on the king's right side");


		}

		/// <summary>
		/// Test for the possible moves of a Rook
		/// </summary>
		[Fact]
		public void RookPossibleMoveTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("a4"), ChessPieceType.Pawn, 1,
				 Pos("e8"), ChessPieceType.King, 2);

			// Player 1 moves
			ApplyMove(b, Move("a4", "a5"));

			// Player 2's turn
			// Check possible moves for player 2's queenside Rook
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedMoves = GetMovesAtPosition(possMoves, Pos("a8"));
			expectedMoves.Should().HaveCount(6, "rook can move (1) b8, (2) c8, (3) d8, (4) a7, (5) a6, (6) a5, or (7) do a castling move with the king")
				 .And.Contain(Move("a8", "b8"))
				 .And.Contain(Move("a8", "c8"))
				 .And.Contain(Move("a8", "d8"))
				 .And.Contain(Move("a8", "a7"))
				 .And.Contain(Move("a8", "a6"))
				 .And.Contain(Move("a8", "a5"));
		}

		/// <summary>
		/// Gets the possible moves for RookKing if moving puts the king in check
		/// </summary>
		[Fact]
		public void GetPossibleMovesForRook() {

			//Create chess board with blacks RookKing guarding its King from whites Queen
			ChessBoard board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("f7"), ChessPieceType.RookKing, 2,
				 Pos("g4"), ChessPieceType.Bishop, 1);

			//Verify current player
			board.CurrentPlayer.Should().Be(1, "Start of game should be whites move");

			//Use whites bishop to threaten enemy rook, which cant leave its position otherwise the King will be in check
			ApplyMove(board, Move("g4", "h5"));

			//Verify current player
			board.CurrentPlayer.Should().Be(2, "Second turn belongs to black");

			//There should be no possible moves for the rook guarding the King from danger
			var moves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(moves, Pos("f7"));
			expected.Should().HaveCount(0, "Rook cannot move if it puts King in check");

		}

		[Fact]
		public void RookPossibleMovesTest() {
			ChessBoard board = new ChessBoard();
			ApplyMove(board, Move("b2", "b4")); // Pawn moves 2 space
			board.CurrentPlayer.Should().Be(2); // Player 2's turn
			ApplyMove(board, Move("h7", "h5")); // Pawn moves 2 spaces
			board.CurrentPlayer.Should().Be(1); // Player 1's turn
			ApplyMove(board, Move("d2", "d3")); // Pawn moves 1 space
			board.CurrentPlayer.Should().Be(2); // Player 2's turn
			ApplyMove(board, Move("h8", "h6")); // RookKing moves 2 spaces forward
			board.CurrentPlayer.Should().Be(1); // Player 1's turn
			ApplyMove(board, Move("c1", "b2")); // Bishop moves 1 left diagonal
			board.CurrentPlayer.Should().Be(2); // Player 2's turn

			// Get all possible moves
			var moves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			// Get all rooks expected moves
			var rookMovesExpected = GetMovesAtPosition(moves, Pos("h6"));

			// Test all rooks expected moves
			rookMovesExpected.Should().Contain(Move("h6", "h7")).And.Contain(Move("h6", "h8")).
				 And.Contain(Move("h6", "g6")).And.Contain(Move("h6", "f6")).And.Contain(Move("h6", "e6")).
				 And.Contain(Move("h6", "d6")).And.Contain(Move("h6", "c6")).And.Contain(Move("h6", "b6")).
				 And.Contain(Move("h6", "a6")).And.HaveCount(9,
				 "Rook is in its orginal states and can only move to the left and backward");
		}

		/// <summary>
		/// Check possible black rook moves.
		/// </summary>
		[Fact]
		public void getPossibleRook() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("b2", "b4"), //white pawn to b4
            Move("a7", "a5"), //black pawn to a5
            Move("e2", "e4"), //white pawn to e4
            Move("a5", "b4"), //black pawn cap white pawn   
            Move("f1", "c4"), //white bishop to c4
            Move("a8", "a6"), //black rook to a6
            Move("c4", "e6"), //white bishop to e6
         });
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("a6"));
			expected.Should().Contain(Move("a6", "a5")).And.Contain(Move("a6", "a4")).And.Contain(Move("a6", "a3")).And.Contain(Move("a6", "b6")).And.Contain(Move("a6", "c6")).And.Contain(Move("a6", "d6")).And.Contain(Move("a6", "a7")).And.Contain(Move("a6", "a8")).And.Contain(Move("a6", "e6")).And.Contain(Move("a6", "a2")).And.HaveCount(10, "a rook can move horizontally or vertically to unoccupied spaces or capture piece vertically/horizontally");
		}
	}
}
