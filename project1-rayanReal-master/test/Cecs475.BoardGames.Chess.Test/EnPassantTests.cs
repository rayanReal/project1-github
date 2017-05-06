using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class EnPassantTests : ChessTests {

		/// <summary>
		/// Test for en passant capture that will put king in check
		/// </summary>
		[Fact]
		public void EnPassantCheck() {
			ChessBoard b = CreateBoardWithPositions(
			  Pos("e2"), ChessPieceType.Pawn, 1,
			  Pos("d2"), ChessPieceType.King, 1,
			  Pos("f4"), ChessPieceType.Pawn, 2,
			  Pos("e8"), ChessPieceType.King, 2);
			ApplyMove(b, Move("e2,e4"));
			ApplyMove(b, Move("f4", "e3"));
			b.IsCheck.Should().Be(true, "Player 1 is on check");
		}

		//Test of special move
		//Test 2: An en passant can only be performed on the very next turn after
		//        an enemy pawn is placed right next to yours (black)
		[Fact]
		public void EnPassantRightAfterTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("c2", "c3"),
				Move("e7", "e5"),
				Move("c3", "c4"),
				Move("e5", "e4"),
				Move("d2", "d4")
			});

			b.CurrentPlayer.Should().Be(2);
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var enPassant = GetMovesAtPosition(poss, Pos("e4"));
			enPassant.Should().Contain(Move("e4", "d3"), "Black pawn should be able to perform an en passant.");

			ApplyMove(b, Move("d7", "d6"));
			ApplyMove(b, Move("f2", "f3"));
			b.CurrentPlayer.Should().Be(2);
			poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			enPassant = GetMovesAtPosition(poss, Pos("e4"));
			enPassant.Should().NotContain(Move("e4", "d3"),
				"Black pawn should not be able to perform an en passant now.");

			//Undo, then check if en passant is still possible
			b.UndoLastMove();
			b.UndoLastMove();
		}

		/// <summary>
		/// Test 2 check for en passant as a move.
		/// </summary>
		[Fact]
		public void EnPassant() {
			ChessBoard b = new ChessBoard();
			// Move white's pawn 2 spaces.
			ApplyMove(b, Move("a2", "a4"));
			b.GetPieceAtPosition(Pos(4, 0)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at postition (4,0)");
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			// Move black's pawn 1 space.
			ApplyMove(b, Move("h7", "h6"));
			b.GetPieceAtPosition(Pos(2, 7)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at postition (2,7)");
			// Player 1's turn.
			b.CurrentPlayer.Should().Be(1);
			// Move white's pawn 1 space.
			ApplyMove(b, Move("a4", "a5"));
			b.GetPieceAtPosition(Pos(3, 0)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at postition (3,0)");
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			// Move black's pawn 2 spaces.
			ApplyMove(b, Move("b7", "b5"));
			b.GetPieceAtPosition(Pos(3, 1)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at postition (3,1)");
			// Player 1's turn.
			b.CurrentPlayer.Should().Be(1);

			// Check which moves the white pawn has. Should have 2 possible moves to either eat or move a space up.
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var enPassantMove = GetMovesAtPosition(possMoves, Pos("a5"));
			enPassantMove.Should().HaveCount(2, "White's pawn can move forward one or capture").And.Contain(Move("a5", "a6")).And.Contain(Move("a5", "b6"));
		}

		// creating a scenario in which a en passant move should be possible
		[Fact]
		public void EnPassat() {
			ChessBoard b = CreateBoardWithPositions(
					  Pos("a2"), ChessPieceType.Pawn, 1,
					  Pos("b2"), ChessPieceType.Pawn, 1,
					  Pos("a5"), ChessPieceType.Pawn, 2,
					  Pos("b4"), ChessPieceType.Pawn, 2);

			ApplyMove(b, Move("a2", "a4"));
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var blackPawn = GetMovesAtPosition(possMoves, Pos("b4"));
			blackPawn.Should().Contain(Move("b4", "a3"), " black pawn should be able to capture the white pawn after moving 2 spaces");
			ApplyMove(b, Move("b4", "a3"));
			b.GetPlayerAtPosition(Pos(4, 0)).Should().Be(0, "Captured!");
		}

		/// <summary>
		/// Check black pawn for available en passant move
		/// </summary>
		[Fact]
		public void EnPassant2() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("b2", "b4"),
					 Move("b7", "b6"),
					 Move("b4", "b5"),
					 Move("c7", "c5"),
					 Move("c2", "c3"),
					 Move("c5", "c4"),
					 Move("d2", "d4")
				});

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var pawn = GetMovesAtPosition(possMoves, Pos("c4"));
			pawn.Should().Contain(Move("c4", "d3")).And
				 .HaveCount(1, "a pawn will only have en passant move available");
			b.Value.Should().Be(0, "No piece is captured yet");

			ApplyMove(b, Move("c4", "d3"));
			b.GetPieceAtPosition(Pos("d3")).Player.Should().Be(2, "black piece should end up here");
			b.Value.Should().Be(-1, "white lost a pawn");
		}

		[Fact]
		public void EnPassant3() {
			ChessBoard b1 = CreateBoardWithPositions(
				 Pos("f1"), ChessPieceType.King, 1,
				 Pos("c5"), ChessPieceType.Pawn, 1,
				 Pos("b7"), ChessPieceType.Pawn, 2,
				 Pos("g8"), ChessPieceType.King, 2
				 );
			b1.CurrentPlayer.Should().Be(1, "Player 1 turn");
			ApplyMove(b1, Move("f1", "e1"));
			b1.CurrentPlayer.Should().Be(2, "Player 2 turn");
			ApplyMove(b1, Move("b7", "b5"));
			b1.CurrentPlayer.Should().Be(1, "Player 1 turn");
			var possMoves = b1.GetPossibleMoves();
			possMoves.Should().HaveCount(7, "There are 2 possible moves for the pawn, and 5 for the King");
			ApplyMove(b1, Move("c5", "b6")); //en passant

		}

		/// <summary>
		/// A Simple scenario where an En Passant can occur. Tests to capture that 
		/// piece and then tests that the piece is there after the move is undone
		/// </summary>
		[Fact]
		public void EnPassant4() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("e2", "e4"),
				Move("a7", "a6"),
				Move("e4", "e5"),
				Move("d7", "d5"),
				Move("e5", "d6"),
			});
			var captured = b.GetPieceAtPosition(Pos("d5"));
			captured.Player.Should().Be(0, "Piece is captured and should be empty");

			b.UndoLastMove();
			captured = b.GetPieceAtPosition(Pos("d5"));
			captured.Player.Should().Be(2, "THe piece is returned because of the undo");
		}
	}
}
