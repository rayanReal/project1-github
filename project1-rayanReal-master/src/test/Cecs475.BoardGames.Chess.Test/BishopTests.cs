using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class BishopTests : ChessTests {
		//Test with UndoLastMove
		//Test 3: Undo a bishop move (white)
		[Fact]
		public void UndoBishopMove() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("b2", "b3"),
				Move("a7", "a5"),
				Move("c1", "a3") //bishop move
         });

			b.CurrentPlayer.Should().Be(2);
			b.Value.Should().Be(0, "Move without capture should not change board value");
			b.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Bishop, "Bishop moved to space a3");

			b.UndoLastMove(); //undo bishop move
			b.CurrentPlayer.Should().Be(1, "Undo should change current player");
			b.Value.Should().Be(0, "Undo should not change board value");
			b.GetPieceAtPosition(Pos("c1")).PieceType.Should().Be(ChessPieceType.Bishop, "Bishop returns to space c1");
		}

		/// <summary>
		/// Bishop Capture: Test to see if bishop captures correctly
		/// </summary>
		[Fact]
		public void BishopCapture() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("g2", "g3"));
			ApplyMove(b, Move("h7", "h6"));
			ApplyMove(b, Move("f1", "g2"));
			ApplyMove(b, Move("b7", "b6"));
			ApplyMove(b, Move("g2", "a8"));
			b.Value.Should().Be(5, "Black lost his QueenSide Rook to White Bishop");
			b.UndoLastMove();
			b.Value.Should().Be(0, "Back to zero zero");
		}

		/// <summary>
		/// Bishops possible moves
		/// </summary>
		[Fact]
		public void BishopMoves() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("c2"), ChessPieceType.Bishop, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e8"), ChessPieceType.King, 2);

			// on position c2 bishop should have 0 move
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var BishopMoveExpected = GetMovesAtPosition(poss, Pos("c2"));
			BishopMoveExpected.Should().Contain(Move("c2", "a4")).And.Contain(Move("c2", "b1")).And.Contain(Move("c2", "b3")).
				And.Contain(Move("c2", "d1")).And.Contain(Move("c2", "d3")).And.Contain(Move("c2", "e4")).
				And.Contain(Move("c2", "f5")).And.Contain(Move("c2", "g6")).And.Contain(Move("c2", "h7")).And.
				HaveCount(9, "a Bishop can move diagonally");
		}

		/// <summary>
		/// This test involves black's bishop taking the white's knight
		/// and then undoing the move. 
		/// </summary>
		[Fact]
		public void BishopMoveAndUndo() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("b1", "a3"),
					 Move("e7", "e5"),
					 Move("f2", "f4")
				});
			//black bishop captures white knight
			ApplyMove(b, Move("f8", "a3"));
			b.CurrentPlayer.Should().Be(1, "because after black took the piece it is white's turn");

			b.UndoLastMove();

			b.CurrentPlayer.Should().Be(2, "because the move was undone it is black's turn again");

			var expectedKnight = b.GetPieceAtPosition(Pos("a3"));
			expectedKnight.PieceType.Should().Be(ChessPieceType.Knight, "because when you undo the move the knight returns to the location where it was captured");

			var expectedBishop = b.GetPieceAtPosition(Pos("f8"));
			expectedBishop.PieceType.Should().Be(ChessPieceType.Bishop, "because when you undo the move the bishop goes back to the original location before capturing the knight");

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			possMoves.Should().Contain(Move("f8", "a3"), "because the bishop should still be able to capture the knight");

		}

		/// <summary>
		/// Tests the possible moves for a bishop
		/// </summary>
		[Fact]
		public void BishopTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("b2", "b4"),
				Move("b7", "b5"),
				Move("d2", "d4"),
				Move("d7", "d5"),
			});
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("c1"));
			expected.Should().Contain(Move("c1", "a3")).And.Contain(Move("c1", "f4")).And.HaveCount(7, "Bishop to the left of the queen should have 7 possible moves.");
			ApplyMove(b, Move("c1", "f4"));
			ApplyMove(b, Move("e7", "e5"));
			poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			expected = GetMovesAtPosition(poss, Pos("f4"));
			expected.Should().Contain(Move("f4", "e3")).And.Contain(Move("f4", "c1")).And.HaveCount(7, "Bishop at F4 should have 7 moves.");
		}

		/// <summary>
		/// Undo black bishop capture and check moves before
		/// </summary>
		[Fact]
		public void checkUndoBishop() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("b2", "b4"), //white pawn to b4
                Move("e7", "e6"), //black pawn to e6
                Move("d2", "d4") //white pawn to d4
            });

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("f8"));
			expected.Should().Contain(Move("f8", "e7")).And.Contain(Move("f8", "d6")).And.Contain(Move("f8", "c5")).And.Contain(Move("f8", "b4")).And.HaveCount(4, "Bishop can only move and capture diagonally ");

			ApplyMove(b, Move("f8,b4")); //black bishop capture white pawn
			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.Pawn) * -1, "captured a white pawn"); //value should be negative since -1 for a white pawn 
			b.GetPieceAtPosition(Pos("b4")).PieceType.Should().Be(ChessPieceType.Bishop, "a bishop should be at this location b4");
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Empty, "nothing should be at this location f8");
			b.UndoLastMove();
			b.Value.Should().Be(0);
			b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Bishop, "a bishop should be at this location f8");
			b.GetPieceAtPosition(Pos("b4")).PieceType.Should().Be(ChessPieceType.Pawn, "nothing should be at this location b4");

		}

		[Fact]
		public void UndoBishopCapture() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");


			ApplyMovesToBoard(b, new ChessMove[] {
					 Move("e2","e3"),
					 Move("h7","h6"),
					 Move("d2","d4"),
					 Move("b7","b6"),
					 Move("d1", "g4"),
					 Move("d7","d6"),
					 Move("g4","g5"),
				});

			b.Value.Should().Be(0, "no pieces have been captured yet");
			b.CurrentPlayer.Should().Be(2, "Player 2 will move the bishop to capture queen");
			var whitequeenmoves = GetMovesAtPosition(possMoves, Pos("c3"));
			ApplyMove(b, Move("h6", "g5"));
			var captured = b.GetPieceAtPosition(Pos("g5"));
			b.Value.Should().Be(-9, "queen was captured");
			captured.Player.Should().Be(2, "Player 1's queen was captured");
			b.UndoLastMove();
			b.Value.Should().Be(0, "no pieces should still be captured");

		}

		[Fact]
		public void UndoBishopCapture2() {

			ChessBoard b = CreateBoardWithPositions(
			Pos("d6"), ChessPieceType.Bishop, 1,
			Pos("f4"), ChessPieceType.Queen, 2,
			Pos("c7"), ChessPieceType.Pawn, 1,
			Pos("a4"), ChessPieceType.King, 2,
			Pos("h8"), ChessPieceType.King, 1,
			Pos("e3"), ChessPieceType.Bishop, 2);
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			b.Value.Should().Be(0, "no pieces should still be captured");
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			ApplyMove(b, Move("d6", "f4"));
			b.Value.Should().Be(9, "queen was captured by a bishop");
			ApplyMove(b, Move("e3", "f4"));
			b.Value.Should().Be(6, "queen was captured by a bishop"); //I'm guessing player 2 subtracts from the value?
			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(2, "Player 2 should have his turn in this instance");
			b.Value.Should().Be(9, "queen was captured by a bishop");
			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "Player 1 should have his turn in this instance");
			b.Value.Should().Be(0, "queen was captured by a bishop");

		}
	}
}
