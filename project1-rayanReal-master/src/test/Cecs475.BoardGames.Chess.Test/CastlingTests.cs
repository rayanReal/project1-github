using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class CastlingTests : ChessTests {
		/// <summary>
		/// Confirm that you cannot castle with pieces between king and castle
		/// </summary>
		[Fact]
		public void ConfirmPiecesRemovedBeforeCastle() {
			ChessBoard b = new ChessBoard();
			foreach (ChessMove cm in b.GetPossibleMoves()) {
				cm.ToString().Should().NotBe(Move("e1", "g1").ToString(), "Should not move whites in accordance with castling in beginning");
				cm.ToString().Should().NotBe(Move("e8", "g8").ToString(), "Should not move blacks in accordance with castling in beginning");
			}
		}

		/// <summary>
		/// Confirm Castling can be undone appropriately
		/// </summary>
		[Fact]
		public void ConfirmCastleUndo() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[]
			{
					 Move("a2", "a3"),
					 Move("e7", "e5"),
					 Move("b2", "b3"),
					 Move("f8", "a3"),
					 Move("a1", "a3"),
					 Move("g8", "e7"),
					 Move("a3", "a1"),
					 Move("e8", "g8")
			});
			b.GetPieceAtPosition(Pos(0, 5)).PieceType.Should().Be(ChessPieceType.RookKing, "Rook should have moved in accordance to with castling");
			b.GetPieceAtPosition(Pos(0, 6)).PieceType.Should().Be(ChessPieceType.King, "King should have moved in accordance to with castling");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "King should return to original position");
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Rook should return to original position");
			var possible = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			possible.Should().Contain(Move("e8", "g8"), "Castle option should've returned to list");
		}

		/// <summary>
		/// Castling rooks. Testing a "tricky" move.
		/// </summary>
		[Fact]
		public void Castling() {
			// Create board with only rooks and king for castling
			ChessBoard b = CreateBoardWithPositions(
				Pos("a1"), ChessPieceType.RookQueen, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("e8"), ChessPieceType.King, 2,
				Pos("h1"), ChessPieceType.RookKing, 1);

			var possMoves = b.GetPossibleMoves();

			// Moves for RookQueen forward and left of King -> 10
			// Moves for RookKing forward and right of King -> 9
			// Moves for King all directions PLUS two Castling moves -> 7
			// Total moves: 26
			possMoves.Should().HaveCount(26, "there should be 26 options to move rooks or king").And
				.Contain((m => (((ChessMove)m).MoveType == ChessMoveType.CastleKingSide)
				|| ((ChessMove)m).MoveType == ChessMoveType.CastleQueenSide));

			// Castle king to c1
			ApplyMove(b, Move("e1, c1"));
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.RookQueen, "queen rook castled to position");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.King, "king castled to position");
		}

		/// <summary>
		/// Undoes castling. Tests UndoLastMove() with special castle move.
		/// </summary>
		[Fact]
		public void UndoCastle() {
			// Create board with only rooks and king for castling
			ChessBoard b = CreateBoardWithPositions(
				Pos("a1"), ChessPieceType.RookQueen, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("h1"), ChessPieceType.RookKing, 1);

			// Castle king to c1
			ApplyMove(b, Move("e1, c1"));
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.RookQueen, "queen rook castled to position");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.King, "king castled to position");

			// Undo castle move
			b.UndoLastMove();

			// Check that the pieces properly moved back
			b.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Empty, "after undo space should be empty");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Empty, "after undo space should be empty");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.RookQueen, "queen rook undone castle to position");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "king undone castle to position");
		}

		/// <summary>
		/// Checks castle validation after moving pieces. Tests complexity of castle move
		/// </summary>
		[Fact]
		public void CastleAfterMove() {
			// Create board with rooks and king for castling
			ChessBoard b = CreateBoardWithPositions(
				Pos("a1"), ChessPieceType.RookQueen, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("h1"), ChessPieceType.RookKing, 1,
				Pos("c7"), ChessPieceType.Pawn, 2,
				Pos("e8"), ChessPieceType.King, 2);

			// Move king up then down so that castling move should be invalid
			ApplyMove(b, Move("e1, e2"));
			ApplyMove(b, Move("c7, c6"));
			ApplyMove(b, Move("e2, e1"));
			ApplyMove(b, Move("c6, c5"));

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingMoves = GetMovesAtPosition(possMoves, Pos("e1"));

			// King moves should be 5 since you cannot castle with rooks after moving
			kingMoves.Should().HaveCount(5, "king should not be able to castle after moving");
		}

		//Test 7: Castling (white)
		[Fact]
		public void CastlingTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("g1", "h3"),
				Move("e7", "e6"),
				Move("e2", "e4"),
				Move("b8", "a6"),
				Move("f1", "a6"),
				Move("h7", "h6")
			});

			b.CurrentPlayer.Should().Be(1);
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingMoves = GetMovesAtPosition(poss, Pos("e1"));
			kingMoves.Should().Contain(Move("e1", "g1"), "able to castle");

			ApplyMove(b, Move("e1", "g1")); //Castling move
			b.CurrentPlayer.Should().Be(2);
			b.GetPieceAtPosition(Pos(7, 5)).PieceType.Should().Be(ChessPieceType.RookKing, "Castled rook at Position (7, 5)");
			b.GetPieceAtPosition(Pos(7, 6)).PieceType.Should().Be(ChessPieceType.King, "Castled king at Position (7, 6)");
		}

		[Fact]
		public void Castle() {
			ChessBoard b1 = CreateBoardWithPositions(
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("a7"), ChessPieceType.Pawn, 2,
				 Pos("b7"), ChessPieceType.Pawn, 2,
				 Pos("g7"), ChessPieceType.Pawn, 2,
				 Pos("h7"), ChessPieceType.Pawn, 2,
				 Pos("a2"), ChessPieceType.Pawn, 1,
				 Pos("b2"), ChessPieceType.Pawn, 1,
				 Pos("g2"), ChessPieceType.Pawn, 1,
				 Pos("h2"), ChessPieceType.Pawn, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1,
				 Pos("d4"), ChessPieceType.Bishop, 2
				 );
			ApplyMove(b1, Move("e1", "c1"));
			b1.GetPieceAtPosition(Pos(7, 2)).PieceType.Should().Be(ChessPieceType.King, "White's King at position c1");
			b1.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.RookQueen, "White's RookQueen at position d1");
		}

		[Fact]
		public void Castling2() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1);

			b.GetPossibleMoves().Should().Contain(Move("e1", "c1"), "King should be able to castle with king side rook")
				 .And.Contain(Move("e1", "g1"), "King should be able to castle with queen side rook");

			//castle left
			ApplyMove(b, Move("e1", "c1"));
			b.GetPieceAtPosition(new BoardPosition(7, 3)).PieceType.Should().Be(ChessPieceType.RookQueen,
				 "Queen side rook should have moved right");
			b.PositionIsEmpty(new BoardPosition(7, 4)).Should().BeTrue("King no longer where it was after castling");
			b.GetPieceAtPosition(new BoardPosition(7, 2)).PieceType.Should().Be(ChessPieceType.King, "King should be to left of rook");

			b.UndoLastMove();
			b.GetPieceAtPosition(new BoardPosition(7, 4)).PieceType.Should().Be(ChessPieceType.King,
				 "King should be returned to correct position after undoing castling");
			b.GetPieceAtPosition(new BoardPosition(7, 0)).PieceType.Should().Be(ChessPieceType.RookQueen,
				 "Rook should be returned to correct position after undoing castling");

			//castle right
			ApplyMove(b, Move("e1", "g1"));
			b.GetPieceAtPosition(new BoardPosition(7, 5)).PieceType.Should().Be(ChessPieceType.RookKing,
				 "King side rook should have moved left");
			b.PositionIsEmpty(new BoardPosition(7, 4)).Should().BeTrue("King no longer where it was after castling");
			b.GetPieceAtPosition(new BoardPosition(7, 6)).PieceType.Should().Be(ChessPieceType.King, "King should be to right of rook");

			b.UndoLastMove();
			b.GetPieceAtPosition(new BoardPosition(7, 4)).PieceType.Should().Be(ChessPieceType.King,
				 "King should be returned to correct position after undoing castling");
			b.GetPieceAtPosition(new BoardPosition(7, 0)).PieceType.Should().Be(ChessPieceType.RookQueen,
				 "Rook should be returned to correct position after undoing castling");

			//castle with pieces in way
			b = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1,
				 Pos("b1"), ChessPieceType.Knight, 1,
				 Pos("g1"), ChessPieceType.Knight, 1);

			b.GetPossibleMoves().Should().NotContain(Move("e1", "c1"), "King should not be able to castle with king side rook with piece in the way")
				 .And.NotContain(Move("e1", "g1"), "King should not be able to castle with queen side rook with piece in the way");
		}

		[Fact]
		public void Castling3() {
			//King-Side Castling
			ChessBoard boardK = new ChessBoard();
			//move White pawn forward to open up the king-side bishop
			ApplyMove(boardK, Move("g2", "g4"));
			boardK.CurrentPlayer.Should().Be(2, "Black's Turn");
			boardK.GetPlayerAtPosition(Pos(4, 6)).Should().Be(1, "White Pawn");
			//move Black Pawn forward to open up the queen-side bishop
			ApplyMove(boardK, Move("b7", "b5"));
			//move White bishop to open up the space in between King and K-Rook
			ApplyMove(boardK, Move("f1", "h3"));
			//move Black bishop to open up the space in between King and Q-Rook
			ApplyMove(boardK, Move("c8", "a6"));
			//move White Knight to open up the space in between King and K-Rook
			ApplyMove(boardK, Move("g1", "f3"));
			//move Black Knight to open up the space in between King and Q-Rook
			ApplyMove(boardK, Move("b8", "c6"));
			//White Castling King and K-Rook
			ApplyMove(boardK, Move("e1", "g1"));
			boardK.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.King, "White King");
			boardK.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.RookKing, "White King-Side Rook");
			//Queen-Side Castling
			ChessBoard boardQ = new ChessBoard();
			//White Pawn
			ApplyMove(boardQ, Move("b2", "b4"));
			//Black Pawn
			ApplyMove(boardQ, Move("g7", "g6"));
			//White Pawn
			ApplyMove(boardQ, Move("c2", "c4"));
			//Black Pawn
			ApplyMove(boardQ, Move("d7", "d5"));
			//White Bishop
			ApplyMove(boardQ, Move("c1", "a3"));
			//Black Pawn capturing White Pawn
			ApplyMove(boardQ, Move("d5", "c4"));
			boardQ.Value.Should().Be(-1);
			//White Knight
			ApplyMove(boardQ, Move("b1", "c3"));
			//Black Pawn
			ApplyMove(boardQ, Move("e7", "e5"));
			//White Queen
			ApplyMove(boardQ, Move("d1", "a4"));
			//Black Knight
			ApplyMove(boardQ, Move("b8", "d7"));
			//Queen-Side Castling
			ApplyMove(boardQ, Move("e1", "c1"));
			boardQ.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.King, "White King");
			boardQ.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.RookQueen, "White Queen-Side Rook");
		}

		[Fact]
		public void UndoCastling() {
			//King-Side Castling: used same King-side Castling formation above
			ChessBoard board = new ChessBoard();
			ApplyMove(board, Move("g2", "g4"));
			ApplyMove(board, Move("b7", "b5"));
			ApplyMove(board, Move("f1", "h3"));
			ApplyMove(board, Move("c8", "a6"));
			ApplyMove(board, Move("g1", "f3"));
			ApplyMove(board, Move("b8", "c6"));
			ApplyMove(board, Move("e1", "g1"));

			//Check current status
			board.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.King, "White-King After Castling");
			board.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.RookKing, "White-Rook After Castling");
			board.CurrentPlayer.Should().Be(2, "Black's Turn");

			//Undo Castling
			board.UndoLastMove();
			board.CurrentPlayer.Should().Be(1, "Undoing should change the current player back to White");
			board.PositionIsEmpty(Pos("g1")).Should().Be(true, "Undoing castling should replace king to original position");
			board.PositionIsEmpty(Pos("f1")).Should().Be(true, "Undoing castling should replace Rook to original position");
		}

		/// <summary>
		/// Catling changed the postion of the king and rook
		/// </summary>
		[Fact]
		public void Castling4() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1); //create a board with possible castling position

			ApplyMove(b, Move("e1", "g1")); //castling king normally cant move this way
			ApplyMove(b, Move("e8", "e7"));
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;


			//var possible = b.GetPossibleMoves();
			//  possible.Should().BeEmpty();

			var Rooking = GetMovesAtPosition(poss, Pos("f1"));
			Rooking.Should().HaveCount(12, "After Castling"); // 


			var King = b.GetPieceAtPosition(Pos("g1")); //king should position at g1 
			var RookKing = b.GetPieceAtPosition(Pos("f1")); // rook should position at f1
			King.Player.Should().Be(1, "King Castling"); //verify player of the castling king
			RookKing.Player.Should().Be(1, "RookKing Castling");//verify player of the castling rook
			King.PieceType.Should().Be(ChessPieceType.King);
			RookKing.PieceType.Should().Be(ChessPieceType.RookKing);

			b.UndoLastMove(); // undo twice undo kings move
			b.UndoLastMove(); //undo castling
			King = b.GetPieceAtPosition(Pos("e1"));
			King.Player.Should().Be(1);
			RookKing = b.GetPieceAtPosition(Pos("h1"));
			RookKing.Player.Should().Be(1);

			GetAllPiecesForPlayer(b, 1).Should().HaveCount(2, "white controls a King and RookKing");
			GetAllPiecesForPlayer(b, 2).Should().HaveCount(1, "black controls a King");
		}

		//testing Castling
		[Fact]
		public void Castling5() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a2"), ChessPieceType.RookKing, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("f7"), ChessPieceType.Pawn, 2,
				 Pos("g7"), ChessPieceType.Pawn, 2,
				 Pos("h7"), ChessPieceType.Pawn, 2);
			//put the king in check
			ApplyMove(b, Move("a2", "e2"));
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var checkedKingMoves = GetMovesAtPosition(possMoves, Pos("e8"));
			checkedKingMoves.Should().NotContain(Move("e8", "g8")).And.HaveCount(3, "The king can not castle in a check.");

			b.UndoLastMove();
			//Keeping the rook away this time
			ApplyMove(b, Move("a2", "a3"));

			var availRookMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedCastleRook = GetMovesAtPosition(availRookMoves, Pos("h8"));
			expectedCastleRook.Should().Contain(Move("h8", "f8")).And.HaveCount(2, "Rook only has the three spaces to its right when locked in by pawns and a king.");

			var availKingMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedCastleKing = GetMovesAtPosition(availKingMoves, Pos("e8"));
			expectedCastleKing.Should().Contain(Move("e8", "d7")).And.Contain(Move("e8", "f8")).And.
				 Contain(Move("e8", "e7")).And.Contain(Move("e8", "g8")).And.HaveCount(5, "The standard 1-space king moves with the addition with the available castling location.");

			//Castle applied
			ApplyMove(b, Move("e8", "g8"));
			var castledRook = b.GetPieceAtPosition(Pos("f8"));
			castledRook.PieceType.Should().Be(ChessPieceType.RookKing, "Rook moves to the right of the of the king's old position.");
			var castledKing = b.GetPieceAtPosition(Pos("g8"));
			castledKing.PieceType.Should().Be(ChessPieceType.King, "King moved 2 spaces to the right to hide behind the rook.");
		}

		/// <summary>
		/// This tests checks to see that you are able to castle 
		/// </summary>
		[Fact]
		public void CastleTest() {

			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("g1", "h3"),
					 Move("b8", "a6"),
					 Move("e2", "e4"),
					 Move("e7", "e5"),
					 Move("f1", "e2"),
					 Move("d8", "e7"),
					 Move("a2", "a4"),
					 Move("d7", "d6"),
					 Move("b2", "b3"),
					 Move("c8", "d7")

				});
			b.CurrentPlayer.Should().Be(1, "because it is white's turn");

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var whiteKing = GetMovesAtPosition(possMoves, Pos("e1"));
			whiteKing.Should().Contain(Move("e1", "g1")).And.Contain(Move("e1", "f1"))
				 .And.HaveCount(2, "because you can move once and then you can castle");


			//Apply castle
			ApplyMove(b, Move("e1", "g1"));
			b.CurrentPlayer.Should().Be(2, "because after white moves it is then black's turn");


			var expectedRook = b.GetPieceAtPosition(Pos("f1"));
			expectedRook.PieceType.Should()
				 .Be(ChessPieceType.RookKing, "because after you castle the rook is this position");

			var expectedKing = b.GetPieceAtPosition(Pos("g1"));
			expectedKing.PieceType.Should()
				 .Be(ChessPieceType.King, "because when you castle the king moves to this position");

		}

		/// <summary>
		/// This test uses the castle move and then we undo it. The king should still be
		/// able to castle after the undo. 
		/// </summary>
		[Fact]
		public void CastlingUndo() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("g1", "h3"),
					 Move("b8", "a6"),
					 Move("e2", "e4"),
					 Move("e7", "e5"),
					 Move("f1", "e2"),
					 Move("d8", "e7"),
					 Move("a2", "a4"),
					 Move("d7", "d6"),
					 Move("b2", "b3"),
					 Move("c8", "d7")

				});
			b.CurrentPlayer.Should().Be(1, "because after the set of moves it is white's turn again");

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var whiteKing = GetMovesAtPosition(possMoves, Pos("e1"));
			whiteKing.Should().Contain(Move("e1", "g1")).And.Contain(Move("e1", "f1"))
				 .And.HaveCount(2, "because you can move once and then you can castle");


			//Apply castle 
			ApplyMove(b, Move("e1", "g1"));
			b.CurrentPlayer.Should().Be(2, "because after the castle it is black's turn");


			var expectedRook = b.GetPieceAtPosition(Pos("f1"));
			expectedRook.PieceType.Should()
				 .Be(ChessPieceType.RookKing, "because after you castle the rook is this position");

			var expectedKing = b.GetPieceAtPosition(Pos("g1"));
			expectedKing.PieceType.Should()
				 .Be(ChessPieceType.King, " because when you castle the king moves to this position");

			b.UndoLastMove();

			b.CurrentPlayer.Should().Be(1, "because we undid the move so it is white's turn again");

			//king and castle need to be in their starting positions

			var originalKing = b.GetPieceAtPosition(Pos("e1"));
			originalKing.PieceType.Should().Be(ChessPieceType.King, "because we undid the move the king goes back to the original position");
			var originalRook = b.GetPieceAtPosition(Pos("h1"));
			originalRook.PieceType.Should().Be(ChessPieceType.RookKing, "because we undid the move the rook goes back to the original position");

			//king should still be able to castle after undo

			var king = GetMovesAtPosition(possMoves, Pos("e1"));
			king.Should().Contain(Move("e1", "g1"), "because the king should still be able to castle after the undo");

		}

		//Test 2 - Tests the rules of castling from a new board after moving a couple of pieces for both players
		[Fact]
		public void Castling6() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("g1", "h3"), //move knight player 1
				Move("g8", "h6"), //move knight player 2
				Move("g2", "g3"), //move pawn player 1
				Move("g7", "g6"), //move pawn player 2
				Move("f1", "g2"), //move bishop player 1
				Move("f8", "g7"), //move bishop player 2
			});

			//Test that there are no pieces in between the king and rook for player 1 and 2
			b.GetPieceAtPosition(Pos(7, 5)).PieceType.Should().Be(ChessPieceType.Empty, "because player 1 moved the piece");
			b.GetPieceAtPosition(Pos(7, 6)).PieceType.Should().Be(ChessPieceType.Empty, "because player 1 moved the piece");
			b.GetPieceAtPosition(Pos(0, 5)).PieceType.Should().Be(ChessPieceType.Empty, "because player 2 moved the piece");
			b.GetPieceAtPosition(Pos(0, 6)).PieceType.Should().Be(ChessPieceType.Empty, "because player 2 moved the piece");

			//Test the king is not in check in its current position and the position it will end up at
			var threatened = b.GetThreatenedPositions(2);
			b.IsCheck.Should().BeFalse();
			// Player 1 castle
			ApplyMove(b, Move("e1, g1"));
			b.GetPieceAtPosition(Pos(7, 6)).PieceType.Should().Be(ChessPieceType.King, "King spot for player 1 after castling");
			b.GetPieceAtPosition(Pos(7, 5)).PieceType.Should().Be(ChessPieceType.RookKing, "RookKing spot for player 1 after castling");
			b.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.Empty, "Empty spot where rook was for player 1 after castling");
			b.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.Empty, "Empty spot where king was for player 1 after castling");

		}

		/// <summary>
		/// Testing a castling case and undoing the move
		/// King moves twice towards the Rook and the Rook slides to the opposite side, next to the King
		/// </summary>
		[Fact]
		public void CastlingTest7() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1);

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var castlingExpected = GetMovesAtPosition(possMoves, Pos("e1"));
			castlingExpected.Should().HaveCount(6, "king can move (1) left, (2) right, (3) up, (4) diagonal left, (5) diagonal right, or (6) castling")
				 .And.Contain(Move("e1", "d1"))
				 .And.Contain(Move("e1", "f1"))
				 .And.Contain(Move("e1", "e2"))
				 .And.Contain(Move("e1", "d2"))
				 .And.Contain(Move("e1", "f2"))
				 .And.Contain(Move("e1", "c1"));

			// Apply the castling
			ApplyMove(b, Move("e1", "c1"));
			var king = b.GetPieceAtPosition(Pos("c1"));
			king.Player.Should().Be(1, "king performed castling move");
			king.PieceType.Should().Be(ChessPieceType.King);
			var rook = b.GetPieceAtPosition(Pos("d1"));
			rook.Player.Should().Be(1, "the queenside rook should take place next to the king on his right");

			// Undo the move and check the board state
			b.UndoLastMove();
			king = b.GetPieceAtPosition(Pos("e1"));
			king.Player.Should().Be(1);
			rook = b.GetPieceAtPosition(Pos("a1"));
			rook.Player.Should().Be(1);
			var empty = b.GetPieceAtPosition(Pos("b1"));
			empty.Player.Should().Be(0);
		}

		[Fact]
		public void CannotCastleIntoCheck() {
			var board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("a7"), ChessPieceType.Pawn, 2,
				 Pos("f5"), ChessPieceType.Knight, 1,
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("a2"), ChessPieceType.Pawn, 1
			);

			// Cannot castle if the square the king will land on is threatened
			// white to move: knight threatens king's castling positions
			ApplyMove(board, Move("f5", "e7"));

			// black to move: cannot put king in check after castling
			var possible = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(possible, Pos("e8"));
			expected.Should()
				 .NotContain(Move("e8", "c8"))
				 .And.NotContain(Move("e8", "g8"))
				 .And
				 .HaveCount(5, "to castle, the king cannot be in check after castling");
		}

		[Fact]
		public void CannotCastleWhenInCheck() {
			var board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("a7"), ChessPieceType.Pawn, 2,
				 Pos("f5"), ChessPieceType.Knight, 1,
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("a2"), ChessPieceType.Pawn, 1
			);

			// Cannot castle while in check
			// white to move: knight checks king
			ApplyMove(board, Move("f5", "g7"));

			// black to move: cannot castle while king is in check
			var possible = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(possible, Pos("e8"));
			expected.Should()
				 .NotContain(Move("e8", "c8"))
				 .And.NotContain(Move("e8", "g8"))
				 .And
				 .HaveCount(5, "to castle, the king cannot be in check");
		}

		[Fact]
		public void CannotCastleThroughCheck() {
			var board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("a7"), ChessPieceType.Pawn, 2,
				 Pos("f5"), ChessPieceType.Knight, 1,
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("a2"), ChessPieceType.Pawn, 1
			);

			// Cannot castle if the squares the king would pass through are threatened
			ApplyMovesToBoard(board, new[] {
					 Move("f5", "d4"),
					 Move("a7", "a6"),
					 Move("d4", "e6")
				});
			var possible = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(possible, Pos("e8"));
			expected.Should()
				 .NotContain(Move("e8", "c8"))
				 .And.NotContain(Move("e8", "g8"))
				 .And
				 .HaveCount(3, "to castle, the king cannot pass through squares where he would be in check");
		}

		[Fact]
		public void NoPiecesBetweenCastleTest() {
			var board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("e7"), ChessPieceType.Pawn, 2,
				 Pos("f7"), ChessPieceType.Pawn, 2,
				 Pos("a1"), ChessPieceType.RookQueen, 1,
				 Pos("b1"), ChessPieceType.Knight, 1,
				 Pos("c1"), ChessPieceType.Bishop, 1,
				 Pos("d1"), ChessPieceType.Queen, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("f1"), ChessPieceType.Bishop, 1,
				 Pos("g1"), ChessPieceType.Knight, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1
			);

			var movesToApply = new[] {
					 Move("b1", "c3"),
					 Move("e7", "e6"),
					 Move("c1", "a3"),
					 Move("e6", "e5"),
					 Move("d1", "b3"),
					 Move("e5", "e4"),
					 Move("f1", "h3"),
					 Move("f7", "f6"),
					 Move("g1", "f3"),
					 Move("f6", "f5")
				};
			String msg = "to castle, no pieces should be between castling rook and king";
			IEnumerable<ChessMove> possible;
			IEnumerable<ChessMove> expected;

			// Ensure while pieces are between, castling not allowed
			for (int i = 0; i < movesToApply.Length; i++) {
				ApplyMove(board, movesToApply[i]);

				if (i % 2 == 0) {
					possible = board.GetPossibleMoves() as IEnumerable<ChessMove>;
					expected = GetMovesAtPosition(possible, Pos("e1"));

					if (i < 6)
						expected.Should().NotContain(Move("e1", "c1")).And.NotContain(Move("e1", "g1"), msg);
					else
						expected.Should().NotContain(Move("e1", "g1"), msg);
				}
			}

			// After all moves are applied recheck castling allowed
			possible = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			expected = GetMovesAtPosition(possible, Pos("e1"));
			expected.Should().Contain(Move("e1", "c1")).And.Contain(Move("e1", "g1")).And
				 .HaveCount(7, "king should be able to castle at this state");
		}

		/// <summary>
		/// Performs castling and undoes the move
		/// </summary>
		[Fact]
		public void UndoCastling2() {
			//Setup castling with both rooks and king for player 1.
			ChessBoard board = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1);

			//apply castling whites King on RookQueen
			board.CurrentPlayer.Should().Be(1, "Start of game should be whites move");
			ApplyMove(board, Move("e1", "c1"));
			board.GetPieceAtPosition(Pos(7, 2)).PieceType.Should().Be(ChessPieceType.King, "King should be at c1 after castling with RookQueen");
			board.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.RookQueen, "RookQueen should be at d1 after castling");

			//verify player
			board.CurrentPlayer.Should().Be(2, "Second turn is blacks move");

			//undo castling
			board.UndoLastMove();

			//verify player
			board.CurrentPlayer.Should().Be(1, "Start of game should be whites move");

			//verify pieces have returned to orginal position after undoing castling
			board.GetPieceAtPosition(Pos(7, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Starting position of RookQueen");
			board.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "Staring position of King");
			board.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Starting position of RookKing");

		}

		/// <summary>
		/// White player castling both King and Queen side
		/// </summary>
		[Fact]
		public void WhiteCastling() {
			//Setup castling with both rooks and king for player 1.
			ChessBoard board = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1);

			//apply castling whites King on RookQueen
			board.CurrentPlayer.Should().Be(1, "Start of game should be whites move");
			ApplyMove(board, Move("e1", "c1"));
			board.GetPieceAtPosition(Pos(7, 2)).PieceType.Should().Be(ChessPieceType.King, "King should be at c1 after castling with RookQueen");
			board.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.RookQueen, "RookQueen should be at d1 after castling");

			//re-initialize board
			board = CreateBoardWithPositions(
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("h1"), ChessPieceType.RookKing, 1,
				 Pos("a1"), ChessPieceType.RookQueen, 1);

			//apply castling whites King on RookKing
			board.CurrentPlayer.Should().Be(1, "Start of game should be whites move");
			ApplyMove(board, Move("e1", "g1"));
			board.GetPieceAtPosition(Pos(7, 6)).PieceType.Should().Be(ChessPieceType.King, "King should be at g1 after castling with RookKing");
			board.GetPieceAtPosition(Pos(7, 5)).PieceType.Should().Be(ChessPieceType.RookKing, "RookKing should be at d1 after castling");


		}

		/// <summary>
		/// Black player castling both King and Queen side
		/// </summary>
		[Fact]
		public void BlackCastling() {
			//Setup castling with both rooks and king for player 1.
			ChessBoard board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("a2"), ChessPieceType.Pawn, 1);


			//apply white move to switch to blacks turn to test logic
			ApplyMove(board, Move("a2", "a3"));

			//apply castling blacks King on RookQueen
			board.CurrentPlayer.Should().Be(2, "Second turn belongs to black");
			ApplyMove(board, Move("e8", "c8"));
			board.GetPieceAtPosition(Pos(0, 2)).PieceType.Should().Be(ChessPieceType.King, "King should be at c1 after castling with RookQueen");
			board.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.RookQueen, "RookQueen should be at d1 after castling");

			//re-initialize board
			board = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("h8"), ChessPieceType.RookKing, 2,
				 Pos("a8"), ChessPieceType.RookQueen, 2,
				 Pos("a2"), ChessPieceType.Pawn, 1);

			//apply white move to switch to blacks turn to test logic
			ApplyMove(board, Move("a2", "a3"));

			//apply castling blacks King on RookKing
			board.CurrentPlayer.Should().Be(2, "Second turn belongs to black");
			ApplyMove(board, Move("e8", "g8"));
			board.GetPieceAtPosition(Pos(0, 6)).PieceType.Should().Be(ChessPieceType.King, "King should be at g1 after castling with RookKing");
			board.GetPieceAtPosition(Pos(0, 5)).PieceType.Should().Be(ChessPieceType.RookKing, "RookKing should be at d1 after castling");


		}

		[Fact]
		public void CastlingLHSTest() {
			ChessBoard board = CreateBoardWithPositions(
				 Pos("a1"), ChessPieceType.RookQueen, 1, // Player 1's RookQueen at position a1
				 Pos("e1"), ChessPieceType.King, 1,      // Player 1's King at position e1
				 Pos("e8"), ChessPieceType.King, 2);     // Player 2's King at position e8

			// D1 row is empty
			board.GetPlayerAtPosition(Pos("d1")).Should().Be(0, "Position e1 should be empty");
			// C1 row is empty
			board.GetPlayerAtPosition(Pos("c1")).Should().Be(0, "Position c1 should be empty");
			// B1 row is empty
			board.GetPlayerAtPosition(Pos("b1")).Should().Be(0, "Position b1 should be empty");

			// get possible moves
			var moves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			// get King moves at position e1
			var kingMovesExpected = GetMovesAtPosition(moves, Pos("e1"));
			// get Rook moves at position a1
			var rookMovesExpected = GetMovesAtPosition(moves, Pos("a1"));

			// get King's 6 Possible moves
			// make sure it contains the move (e1, c1)
			kingMovesExpected.Should().Contain(Move("e1", "d1")).And.Contain(Move("e1", "d2")).And.
				 Contain(Move("e1", "e2")).And.Contain(Move("e1", "f2")).And.Contain(Move("e1", "f1")).
				 And.Contain(Move("e1", "c1")).
				 And.HaveCount(6, "King is in its original states and can castle");

			// get Rook's 10 possible moves
			// make sure it contains the move (a1, d1)
			rookMovesExpected.Should().Contain(Move("a1", "a2")).And.Contain(Move("a1", "a3")).And.
				 Contain(Move("a1", "a4")).And.Contain(Move("a1", "a5")).And.Contain(Move("a1", "a6")).
				 And.Contain(Move("a1", "a7")).And.Contain(Move("a1", "a8")).And.Contain(Move("a1", "b1")).
				 And.Contain(Move("a1", "c1")).And.Contain(Move("a1", "d1")).
				 And.HaveCount(10, "Rook is in its original states and can castle");
		}

		/// <summary>
		/// Both players castle consecutively and we undo it and check the positions and wha pieces are there
		/// </summary>
		[Fact]
		public void UndoCastleTwiceTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
                // Move all pawns forward
                Move ("a2", "a4"),
					 Move ("a7", "a5"),
					 Move ("b2", "b4"),
					 Move ("b7", "b5"),
					 Move ("c2", "c4"),
					 Move ("c7", "c5"),
					 Move ("d2", "d4"),
					 Move ("d7", "d5"),
					 Move ("e2", "e4"),
					 Move ("e7", "e5"),
					 Move ("f2", "f4"),
					 Move ("f7", "f5"),
					 Move ("g2", "g4"),
					 Move ("g7", "g5"),
					 Move ("h2", "h4"),
					 Move ("h7", "h5"),

                //Move Other pieces forward
                Move ("b1", "a3"),
					 Move ("b8", "a6"),
					 Move ("g1", "h3"),
					 Move ("g8", "h6"),
					 Move ("c1", "e3"),
					 Move ("c8", "e6"),
					 Move ("f1", "d3"),
					 Move ("f8", "d6"),
                // Move Queens Forward
                Move ("d1", "d2"),
					 Move ("d8", "d7"),
				});

			// Castle both sides
			ApplyMove(b, Move("e1", "g1"));
			var WhiteKing = b.GetPieceAtPosition(Pos("g1"));
			var WhiteRook = b.GetPieceAtPosition(Pos("f1"));

			b.GetPieceAtPosition(Pos(7, 6)).PieceType.Should().Be(ChessPieceType.King, "White's king at position (7,6) or (g1) after castle");
			b.GetPieceAtPosition(Pos(7, 5)).PieceType.Should().Be(ChessPieceType.RookKing, "White's Rook at position (7,5) or (f1) after castle");

			ApplyMove(b, Move("e8", "c8"));
			var BlackKing = b.GetPieceAtPosition(Pos("c8"));
			var BlackRook = b.GetPieceAtPosition(Pos("d8"));

			b.GetPieceAtPosition(Pos(0, 2)).PieceType.Should().Be(ChessPieceType.King, "White's king at position (0, 2) or (c8)");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.RookQueen, "White's king at position (0, 3) or (d8)");

			b.UndoLastMove();

			b.GetPieceAtPosition(Pos(0, 2)).Player.Should().Be(0, "Should be clear after castle is undone");
			b.GetPieceAtPosition(Pos(0, 3)).Player.Should().Be(0, "Should be clear after castle is undone");

			b.UndoLastMove();

			b.GetPieceAtPosition(Pos(7, 6)).Player.Should().Be(0, "Should be clear after castle is undone");
			b.GetPieceAtPosition(Pos(7, 5)).Player.Should().Be(0, "Should be clear after castle is undone");
		}

		/// <summary>
		/// Test castling on Black player.
		/// </summary>
		[Fact]
		public void TrickyCastlingBlack() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[]
			{
					 Move("g2", "g3"),
					 Move("g7", "g6"),
					 Move("g1", "f3"),
					 Move("g8", "f6"),
					 Move("f1", "g2"),
					 Move("f8", "g7"),
					 Move("b2", "b3")
			});

			b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().Contain(m => ((ChessMove)m).MoveType == ChessMoveType.CastleKingSide, "Castling is possible."); // If available, King hasn't moved yet.

			ApplyMove(b, Move("e8", "g8")); // Execute Black Player's King-side Castle

			b.CurrentPlayer.Should().Be(1, "Castling is a move so player should have changed after castling.");

			var rook = b.GetPieceAtPosition(Pos("f8"));
			rook.PieceType.Should().Be(ChessPieceType.RookKing, "Piece should be King-side rook.");
			rook.Player.Should().Be(2, "Rook should be Black's piece.");

			var king = b.GetPieceAtPosition(Pos("g8"));
			king.PieceType.Should().Be(ChessPieceType.King, "Piece should be King.");
			king.Player.Should().Be(2, "King should be Black's piece.");

			b.UndoLastMove(); // Undo castling.

			b.CurrentPlayer.Should().Be(2, "Current player should switch back to Black after undo.");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Empty, "Space should be empty after undo.");
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Empty, "Space should be empty after undo.");

			rook = b.GetPieceAtPosition(Pos("h8"));
			rook.PieceType.Should().Be(ChessPieceType.RookKing, "Piece should be King-side rook.");
			rook.Player.Should().Be(2, "Rook should be Black's piece.");

			king = b.GetPieceAtPosition(Pos("e8"));
			king.PieceType.Should().Be(ChessPieceType.King, "Piece should be King.");
			king.Player.Should().Be(2, "King should be Black's piece.");

		}

		/// <summary>
		/// Checks for whtie king castling
		/// </summary>
		[Fact]
		public void checkWhiteKingCastling() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("g1", "f3"), //white knight
                Move("a7", "a6"), //black pawn 
                Move("e2", "e3"), //white pawn 
                Move("b7", "b6"), //black pawn 
                Move("f1", "e2"), //white bishop 
                Move("c7", "c6") //black pawn 
            });

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			var expected = GetMovesAtPosition(poss, Pos("h1"));
			expected.Should().Contain(Move("h1", "f1"), "rook should be able to move to f1");

			expected = GetMovesAtPosition(poss, Pos("e1"));
			expected.Should().Contain(Move("e1", "g1"), "king should be able to move to g1");
			poss.Should().Contain(m => ((ChessMove)m).MoveType == ChessMoveType.CastleKingSide, "there should be king side castling move available");
			ApplyMove(b, Move("e1", "g1")); // applying the castling move
			b.CurrentPlayer.Should().Be(2, "current player should be player 2"); // checks that the player is now player 2
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.King, "a king should be at this location g1");
			b.GetPieceAtPosition(Pos("f1")).PieceType.Should().Be(ChessPieceType.RookKing, "a king should be at this location f1");
		}
	}
}
