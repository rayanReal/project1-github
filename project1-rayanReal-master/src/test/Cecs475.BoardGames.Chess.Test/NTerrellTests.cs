using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Test {
	public class ExampleTests : ChessTests {
		/// <summary>
		/// Simple facts about "new" boards.
		/// </summary>
		[Fact]
		public void NewChessBoard() {
			ChessBoard b = new ChessBoard();
			b.GetPlayerAtPosition(Pos(7, 0)).Should().Be(1, "Player 1 should be in lower left of board");
			b.GetPlayerAtPosition(Pos(0, 0)).Should().Be(2, "Player 2 should be in upper left of board");
			b.GetPlayerAtPosition(Pos(4, 0)).Should().Be(0, "Middle left of board should be empty");
			// Test a few select piece locations.
			b.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "White's king at position (7,4)");
			b.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Black's king at position (0,4)");
			// Test other properties
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			b.Value.Should().Be(0, "the starting game has value 0");
		}

		/// <summary>
		/// Various bounds checks.
		/// </summary>
		[Fact]
		public void PositionInBounds() {
			ChessBoard.PositionInBounds(Pos(0, 0)).Should().Be(true);
			ChessBoard.PositionInBounds(Pos(7, 7)).Should().Be(true);
			ChessBoard.PositionInBounds(Pos(0, 8)).Should().Be(false);
			ChessBoard.PositionInBounds(Pos(8, 0)).Should().Be(false);
			ChessBoard.PositionInBounds(Pos(-1, 5)).Should().Be(false);
			ChessBoard.PositionInBounds(Pos(2, -4)).Should().Be(false);
			ChessBoard.PositionInBounds(Pos(9, -5)).Should().Be(false);
		}

		/// <summary>
		/// Moving pawns one or two spaces.
		/// </summary>
		[Fact]
		public void PawnTwoSpaceMove() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("a2", "a3")); // one space move
			b.CurrentPlayer.Should().Be(2);
			ApplyMove(b, Move("a7", "a5")); // player 2 response
			b.CurrentPlayer.Should().Be(1);

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var oneMoveExpected = GetMovesAtPosition(poss, Pos("a3"));
			oneMoveExpected.Should().Contain(Move("a3", "a4")).And.HaveCount(1,
				"a pawn not in its original space can only move one space forward");

			var twoMovesExpected = GetMovesAtPosition(poss, Pos("b2"));
			twoMovesExpected.Should().Contain(Move("b2", "b3")).And.Contain(Move("b2", "b4")).And.
				HaveCount(2, "a pawn in its original space can move up to two spaces forward");

			b.Value.Should().Be(0, "no change in value after moving pieces with no capture");
		}


		/// <summary>
		/// Pawn capture.
		/// </summary>
		[Fact]
		public void PawnCapture() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("a2", "a4"),
				Move("b7", "b5")
			});

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("a4"));
			expected.Should().Contain(Move("a4", "b5")).And.Contain(Move("a4", "a5")).And.
				HaveCount(2, "a pawn can capture diagonally ahead or move forward");

			b.Value.Should().Be(0, "no pieces have been captured yet");

			ApplyMove(b, Move("a4", "b5"));
			b.GetPieceAtPosition(Pos("b5")).Player.Should().Be(1, "Player 1 captured Player 2's pawn diagonally");
			b.Value.Should().Be(1, "Black lost a single pawn of 1 value");

			b.UndoLastMove();
			b.Value.Should().Be(0, "after undoing the pawn capture, the value should go down by 1");
		}

		/// <summary>
		/// Value adjustment after capturing an enemy piece, and undoing the move.
		/// </summary>
		[Fact]
		public void ValueAfterCapturing() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("a5"), ChessPieceType.Pawn, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("b6"), ChessPieceType.Knight, 2,
				Pos("e8"), ChessPieceType.King, 2);

			b.Value.Should().Be(0);
			ApplyMove(b, Move("a5, b6"));
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.Knight), "captured a Black knight");
			b.UndoLastMove();
			b.Value.Should().Be(0, "undid the knight capture");
		}

		/// <summary>
		/// Promote a pawn after reaching the final rank.
		/// </summary>
		[Fact]
		public void PawnPromoteTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("b2", "b4"),
				Move("a7", "a5"),
				Move("b4", "b5"),
				Move("a8", "a6"), // rook to a6
				Move("b5", "a6"), // capture rook with pawn
			});
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.RookKing), "a Black rook was captured");

			ApplyMovesToBoard(b, new ChessMove[] {
				Move("b7", "b6"),
				Move("a6", "a7"),
			});
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.RookKing), "no other pieces captured");

			ApplyMovesToBoard(b, new ChessMove[] {
				Move("b6", "b5"),
			});
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.RookKing), "no other pieces captured");

			ApplyMove(b, Move("a7", "a8"));
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.RookKing), "no other pieces captured");

			b.CurrentPlayer.Should().Be(1, "moving a pawn to the final rank should not change the current player");
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(4, "there are four possible promotion moves").And
				.OnlyContain(m => ((ChessMove)m).MoveType == ChessMoveType.PawnPromote);

			// Apply the promotion move
			ApplyMove(b, Move("(a8, Queen)"));
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Queen, "the pawn was replaced by a queen");
			b.GetPieceAtPosition(Pos("a8")).Player.Should().Be(1, "the queen is controlled by player 1");
			b.CurrentPlayer.Should().Be(2, "choosing the pawn promotion should change the current player");
			b.Value.Should().Be(13, "gained 9 points, lost 1 point from queen promotion");

			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "undoing a pawn promotion should change the current player");
			b.Value.Should().Be(5, "lose value of queen when undoing promotion");

			b.UndoLastMove(); // this undoes the pawn's movement to the final rank
			b.CurrentPlayer.Should().Be(1, "undoing the pawn's final movement should NOT change current player");
		}

		[Fact]
		public void EnPassantTest() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("a5"), ChessPieceType.Pawn, 1,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("b7"), ChessPieceType.Pawn, 2,
				Pos("e8"), ChessPieceType.King, 2);

			ApplyMove(b, Move("e1", "e2"));
			// move black pawn forward twice
			ApplyMove(b, Move("b7", "b5"));

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var enPassantExpected = GetMovesAtPosition(possMoves, Pos("a5"));
			enPassantExpected.Should().HaveCount(2, "pawn can move forward one or en passant").And
				.Contain(Move("a5", "a6")).And.Contain(Move("a5", "b6"));

			// Apply the en passant
			ApplyMove(b, Move("a5", "b6"));
			var pawn = b.GetPieceAtPosition(Pos("b6"));
			pawn.Player.Should().Be(1, "pawn performed en passant move");
			pawn.PieceType.Should().Be(ChessPieceType.Pawn);
			var captured = b.GetPieceAtPosition(Pos("b5"));
			captured.Player.Should().Be(0, "the pawn that moved to b5 was captured by en passant");
			b.Value.Should().Be(1);

			GetAllPiecesForPlayer(b, 1).Should().HaveCount(2, "white controls a pawn and king");
			GetAllPiecesForPlayer(b, 2).Should().HaveCount(1, "black only controls a king");

			// Undo the move and check the board state
			b.UndoLastMove();
			b.Value.Should().Be(0);
			pawn = b.GetPieceAtPosition(Pos("a5"));
			pawn.Player.Should().Be(1);
			captured = b.GetPieceAtPosition(Pos("b5"));
			captured.Player.Should().Be(2);
			var empty = b.GetPieceAtPosition(Pos("b6"));
			empty.Player.Should().Be(0);

			GetAllPiecesForPlayer(b, 1).Should().HaveCount(2, "white controls a pawn and king");
			GetAllPiecesForPlayer(b, 2).Should().HaveCount(2, "black controls a pawn and king");
		}
	}
}
