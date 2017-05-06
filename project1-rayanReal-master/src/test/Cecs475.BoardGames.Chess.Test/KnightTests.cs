using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class KnightTests : ChessTests {
		[Fact]
		public void KnightStartingPosition() {
			ChessBoard h = new ChessBoard();
			var possMoves = h.GetPossibleMoves() as IEnumerable<ChessMove>;
			var knight1 = GetMovesAtPosition(possMoves, Pos("b1"));
			knight1.Should().Contain(Move("b1", "a3")).And.Contain(Move("b1", "c3"), "because a starting knight only has 2 moves").And.HaveCount(2);

			var knight2 = GetMovesAtPosition(possMoves, Pos("g1"));
			knight2.Should().Contain(Move("g1", "h3")).And.Contain(Move("g1", "f3"), "because a starting knight only has 2 moves").And.HaveCount(2);
		}

		[Fact]
		public void KnightMovementTest() {
			ChessBoard CB = CreateBoardWithPositions(Pos("d4"), ChessPieceType.Knight, 1,
																  Pos("d6"), ChessPieceType.Pawn, 2);
			var possMoves = CB.GetPossibleMoves() as IEnumerable<ChessMove>;
			var knight = GetMovesAtPosition(possMoves, Pos("d4"));
			knight.Should().Contain(Move("d4", "e6")).And.Contain(Move("d4", "c6"))
						  .And.Contain(Move("d4", "f5")).And.Contain(Move("d4", "f3"))
						  .And.Contain(Move("d4", "e2")).And.Contain(Move("d4", "c2"))
						  .And.Contain(Move("d4", "b3")).And.Contain(Move("d4", "b5"), "8 Possiable moves for the knight").And.HaveCount(8);
			CB.Value.Should().Be(0, "Default value since no pieces have been captured and just checking movement");
		}

		/// <summary>
		/// Undoes knight capture. Tests UndoLastMove() with knight movement and capture.
		/// </summary>
		[Fact]
		public void UndoKnightCapture() {
			// Create board with knight going to capture pawn
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("g1", "h3"),
				Move("g7", "g5")
			});

			b.Value.Should().Be(0, "nothing has been captured");
			b.GetPieceAtPosition(Pos("g5")).PieceType.Should().Be(ChessPieceType.Pawn, "space should have pawn before capture");
			b.GetPieceAtPosition(Pos("h3")).PieceType.Should().Be(ChessPieceType.Knight, "space should have knight before capture");

			// Knight captures pawn
			ApplyMove(b, Move("h3, g5"));
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.Pawn), "value should now be the value of the pawn that was captured");
			b.GetPieceAtPosition(Pos("g5")).PieceType.Should().Be(ChessPieceType.Knight, "space should have knight after capture");

			// Undo knight capture
			b.UndoLastMove();

			// Pieces should be in original places
			b.Value.Should().Be(0, "capture has been undone, nothing captured");
			b.GetPieceAtPosition(Pos("g5")).PieceType.Should().Be(ChessPieceType.Pawn, "space should have pawn after undoing capture");
			b.GetPieceAtPosition(Pos("h3")).PieceType.Should().Be(ChessPieceType.Knight, "space should have knight after undoing capture");
		}

		//Test with UndoLastMove
		//Test 4: Undo capture, check if capture move is still possible (black)
		[Fact]
		public void UndoKnightCapture2() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("e8"), ChessPieceType.King, 2,
				Pos("b6"), ChessPieceType.Pawn, 2,
				Pos("d6"), ChessPieceType.Knight, 2,
				Pos("e5"), ChessPieceType.Pawn, 2,
				Pos("c4"), ChessPieceType.Pawn, 1,
				Pos("a3"), ChessPieceType.Knight, 1,
				Pos("b3"), ChessPieceType.Pawn, 1,
				Pos("e1"), ChessPieceType.King, 1
			);

			b.Value.Should().Be(0);
			//Move applied to make black the current player
			ApplyMove(b, Move("a3", "b5"));
			b.CurrentPlayer.Should().Be(2);
			ApplyMove(b, Move("d6", "c4"));
			b.GetPieceAtPosition(Pos("c4")).PieceType.Should().Be(ChessPieceType.Knight);
			//Because it was Player 2 (Black) that captured a piece, the value of the
			// board should go down by 1.
			b.Value.Should().Be(-(b.GetPieceValue(ChessPieceType.Pawn)), "Black knight captured a White pawn");

			b.UndoLastMove();
			b.Value.Should().Be(0, "Undo reverts value to 0");
			b.CurrentPlayer.Should().Be(2);
			b.GetPieceAtPosition(Pos("d6")).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos("c4")).PieceType.Should().Be(ChessPieceType.Pawn);
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var stillCanCapture = GetMovesAtPosition(poss, Pos("d6"));
			stillCanCapture.Should().Contain(Move("d6", "c4"), "Black knight should be able to capture the White pawn.");
		}

		/// <summary>
		/// Test 3 Undo a white move.
		/// </summary>
		[Fact]
		public void UndoKnightMove() {
			ChessBoard b = new ChessBoard();
			// Move white knight to c3.
			ApplyMove(b, Move("b1", "c3"));
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			b.GetPieceAtPosition(Pos(5, 2)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (2,2)");
			// Undo the last move.
			b.UndoLastMove();
			// Back to player 1.
			b.CurrentPlayer.Should().Be(1);
			// Reset of the board.
			b.Value.Should().Be(0, "the starting game has value 0");
		}

		/// <summary>
		/// Test 4 Undo a black move.
		/// </summary>
		[Fact]
		public void UndoBlackKnightMove() {
			ChessBoard b = new ChessBoard();
			// Move white knight to c3.
			ApplyMove(b, Move("b1", "c3"));
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			b.GetPieceAtPosition(Pos(5, 2)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (5,2)");
			// Move black knight to c6.
			ApplyMove(b, Move("b8", "c6"));
			b.GetPieceAtPosition(Pos(2, 2)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (2,2)");
			// Player 1's turn.
			b.CurrentPlayer.Should().Be(1);
			// Undo the last move.
			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(2);
			// Empty at the old knight spot.
			b.GetPieceAtPosition(Pos(2, 2)).PieceType.Should().Be(ChessPieceType.Empty, "Empty peice at position (2,2)");
			// Knight should be back at B8.
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Knight, "White's knight at position (0,1)");
		}

		[Fact]
		public void KnightInitialMoves() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var knight1 = GetMovesAtPosition(possMoves, Pos("b1"));
			knight1.Should().Contain(Move("b1", "a3")).And.Contain(Move("b1", "c3")).And.HaveCount(2, "because a starting knight only has 2 moves");

		}

		// checking to see if the knight piece retains its possible moves from the previous turn after applying undolastmove()
		[Fact]
		public void UndoKnightMove2() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("d2", "d4"));
			ApplyMove(b, Move("h7", "h5"));
			ApplyMove(b, Move("g1", "f3"));
			ApplyMove(b, Move("h8", "h6"));
			ApplyMove(b, Move("f3", "e5"));
			ApplyMove(b, Move("b8", "c6"));
			ApplyMove(b, Move("e5", "c6"));
			b.UndoLastMove();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var knight = GetMovesAtPosition(possMoves, Pos("e5"));
			knight.Should().Contain(Move("e5", "c6")).And.Contain(Move("e5", "c4")).And.Contain(Move("e5", "d3")).And.
				 Contain(Move("e5", "f3")).And.Contain(Move("e5", "g4")).And.Contain(Move("e5", "g6")).And.
				 Contain(Move("e5", "f7")).And.Contain(Move("e5", "d7"), "Knight retaining its moves from previous turn");
			b.GetPlayerAtPosition(Pos(3, 4)).Should().Be(1);
			b.CurrentPlayer.Should().Be(1, "undo should revert back to previous players turn");


		}

		[Fact]
		public void KnightInitialMoves2() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var knight1 = GetMovesAtPosition(possMoves, Pos("b1"));
			knight1.Should().Contain(Move("b1", "a3")).And.Contain(Move("b1", "c3"))
				 .And.HaveCount(2, "because a starting knight only has 2 moves");

			var knight2 = GetMovesAtPosition(possMoves, Pos("g1"));
			knight2.Should().Contain(Move("g1", "f3")).And.Contain(Move("g1", "h3"))
			  .And.HaveCount(2, "because a starting knight only has 2 moves");

		}

		/// <summary>
		/// KnightPossibleMoves: This function tests all possible moves for a knight.
		/// </summary>
		[Fact]
		public void KnightPossibleMoves() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("b1", "a3"));
			ApplyMove(b, Move("g8", "f6"));
			b.GetPieceAtPosition(Pos(5, 0)).PieceType.Should().Be(ChessPieceType.Knight, "White moved knight from b1 to a3");
			b.GetPieceAtPosition(Pos(2, 5)).PieceType.Should().Be(ChessPieceType.Knight, "Black moved knight from g8 to f6");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos(2, 5)).PieceType.Should().Be(ChessPieceType.Empty, "Black Undid Knight Move");
		}

		//Checks the knight's possible moves
		[Fact]
		public void KnightMove() {
			ChessBoard board = CreateBoardWithPositions(
				 Pos("b1"), ChessPieceType.Knight, 1,
				 Pos("c3"), ChessPieceType.Knight, 2);
			//first knight (white)
			var k1 = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var k1expected = GetMovesAtPosition(k1, Pos("b1"));
			k1expected.Should().Contain(Move("b1", "a3")).And.Contain(Move("b1", "c3")).And.Contain(Move("b1", "d2")).And.HaveCount(3, "A knight in the starting position with no pawns in the way should have 3 possible movements of (+2, +1) spaces or (+1, +2).");
			//move peasants get out the way
			ApplyMove(board, Move("b1", "a3"));
			board.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Knight, "Knight post move");
			var k2 = board.GetPossibleMoves() as IEnumerable<ChessMove>;

			var k2expected = GetMovesAtPosition(k2, Pos("c3"));
			k2expected.Should().Contain(Move("c3", "b1")).And.Contain(Move("c3", "d1")).And.Contain(Move("c3", "b5")).And.
				 Contain(Move("c3", "d5")).And.Contain(Move("c3", "d5")).And.Contain(Move("c3", "a2")).And.Contain(Move("c3", "a4")).And.
				 Contain(Move("c3", "e2")).And.Contain(Move("c3", "e4")).And.
				 HaveCount(8, "A knight in the in the middle of the board should have 8 possible movements available");
		}

		//Checks the knight's movement, capturing, then undoes the capture
		[Fact]
		public void KnightCapture() {
			ChessBoard board = CreateBoardWithPositions(
				Pos("b1"), ChessPieceType.Knight, 1,
				Pos("d1"), ChessPieceType.Knight, 2);

			//knight capture
			board.Value.Should().Be(0);
			ApplyMove(board, Move("b1", "c3"));
			ApplyMove(board, Move("d1", "c3"));
			board.UndoLastMove();
			board.Value.Should().Be(0, "Undoes the capture, board value resets to prior value 0");
		}

		/// <summary>
		/// Test for value after capturing a piece and undoing it
		/// </summary>
		[Fact]
		public void ValueAfterCaptureTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e7"), ChessPieceType.Knight, 2,
				 Pos("a1"), ChessPieceType.Queen, 2,
				 Pos("d6"), ChessPieceType.King, 2,
				 Pos("c8"), ChessPieceType.King, 1,
				 Pos("g8"), ChessPieceType.Queen, 1,
				 Pos("a2"), ChessPieceType.Knight, 1);

			b.Value.Should().Be(0);
			b.IsCheck.Should().BeTrue();
			// Player 1's King moves away from check
			ApplyMove(b, Move("c8, b8"));

			// Player 2 takes player 1's Queen
			ApplyMove(b, Move("e7", "g8"));
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.Queen) * -1, "captured a white queen");

			// Undo
			b.UndoLastMove();
			b.Value.Should().Be(0, "undid the queen capture");
			b.GetPieceAtPosition(Pos("g8"))
				 .Player.Should().Be(1, "after undo last move, player 1's queen should still be at g8");
			b.GetPieceAtPosition(Pos("e7"))
				 .Player.Should().Be(2, "after undo last move, player 2's knight should still be at e7");
		}

		[Fact]
		public void KnightCaptureTest() {
			var board = CreateBoardWithPositions(
				 Pos("a1"), ChessPieceType.King, 1,
				 Pos("c3"), ChessPieceType.Pawn, 1,
				 Pos("e6"), ChessPieceType.RookKing, 1,
				 Pos("c5"), ChessPieceType.Knight, 2,
				 Pos("f8"), ChessPieceType.King, 2
			);

			ApplyMovesToBoard(board, new[] {
					 Move("c3", "c4"),
					 Move("c5", "e6")
				});

			// Check board value ((b)knight + (w)pawn = -3 + 1 = -2
			board.Value.Should().Be(-5, "board value does not properly update on capture");

			// Check another capture
			ApplyMovesToBoard(board, new[] {
					 Move("c4", "c5"),
					 Move("e6", "c5")
				});
			board.Value.Should().Be(-6, "board value does not properly update on capture");

			// Undo captures and check
			for (int i = 0; i < 4; i++) {
				board.UndoLastMove();
			}
			board.Value.Should().Be(0, "board value does not properly update on undo of captures");
			board.GetPieceAtPosition(Pos("c5")).PieceType.Should().Be(ChessPieceType.Knight, "pieces not replaced properly on undo");
			board.GetPieceAtPosition(Pos("e6")).PieceType.Should().Be(ChessPieceType.RookKing, "pieces not replaced properly on undo");
		}

		[Fact]
		public void SimpleKnightCapturePawnUndo() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("b8"), ChessPieceType.Knight, 1,
				Pos("a6"), ChessPieceType.Pawn, 2);
			ApplyMove(b, Move("b8", "a6"));
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Knight, "Last move the the horsey back in place");
		}

		[Fact]
		public void KnightCapturesAQueen() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("d2", "d4"));
			ApplyMove(b, Move("e7", "e5"));
			ApplyMove(b, Move("b1", "c3"));
			ApplyMove(b, Move("d7", "d5"));
			ApplyMove(b, Move("d4", "e5"));
			ApplyMove(b, Move("d5", "d4"));
			ApplyMove(b, Move("c3", "e4"));
			ApplyMove(b, Move("d8", "d5"));
			ApplyMove(b, Move("e4", "c3"));
			ApplyMove(b, Move("d5", "e4"));
			//KNight eats the queen
			ApplyMove(b, Move("c3", "e4"));
			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Empty, "The peice is no longer there its empty was eaten");
			b.GetPieceAtPosition(Pos("e4")).PieceType.Should().Be(ChessPieceType.Knight, "The new occupied space is a knight");

		}

		[Fact]
		public void CheckStartingKnights() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "a knight should be at this location b1");
			b.GetPieceAtPosition(Pos("b1")).Player.Should().Be(1, "Player 1's Knight peice");
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "a knight should be at this location g1");
			b.GetPieceAtPosition(Pos("g1")).Player.Should().Be(1, "Player 1's Knight peice");

			b.GetPieceAtPosition(Pos("b8")).PieceType.Should().Be(ChessPieceType.Knight, "a knight should be at this location b1");
			b.GetPieceAtPosition(Pos("b8")).Player.Should().Be(2, "Player 1's Knight peice");
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Knight, "a knight should be at this location g1");
			b.GetPieceAtPosition(Pos("g8")).Player.Should().Be(2, "Player 1's Knight peice");
		}

		/// <summary>
		/// Simple first move of white knight and undoing it.
		/// </summary>
		[Fact]
		public void CheckUndoKnight() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("b1,c3"));
			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Knight, "a knight should be at this location c3");
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Empty, "nothing should be at this location b1");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Empty, "nothing should be at this location c3");
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "knight should be at this location b1");
		}

		[Fact]
		public void InitialKnightMoves() {
			ChessBoard b = new ChessBoard();
			var possmoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var knight1 = GetMovesAtPosition(possmoves, Pos("b1"));
			knight1.Should().Contain(Move("b1", "a3")).And.Contain(Move("b1", "c3"))
				 .And.HaveCount(2, "because a starting knight only has 2 moves");

			var knight2 = GetMovesAtPosition(possmoves, Pos("g1"));
			knight2.Should().Contain(Move("g1", "h3")).And.Contain(Move("g1", "f3"))
				 .And.HaveCount(2, "because a starting knight only has 2 moves");
		}
	}
}
