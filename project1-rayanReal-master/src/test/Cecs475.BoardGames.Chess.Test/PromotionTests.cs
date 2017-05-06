using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class PromotionTests : ChessTests {
		/// <summary>
		/// Test to check the promote of a pawn to a queen for both players. 
		/// </summary>
		[Fact]
		public void PawnPromo1() {
			ChessBoard CB = CreateBoardWithPositions(Pos("d7"), ChessPieceType.Pawn, 1,
																  Pos("d2"), ChessPieceType.Pawn, 2,
																  Pos("a2"), ChessPieceType.King, 1,
																  Pos("a7"), ChessPieceType.King, 2);
			CB.CurrentPlayer.Should().Be(1, "Player 1 pawn promotion should be first");
			ApplyMove(CB, Move("d7", "d8"));
			ApplyMove(CB, Move("(d8,Queen)"));
			CB.CurrentPlayer.Should().Be(2, "Player 2 turn after player 1");
			ApplyMove(CB, Move("d2", "d1"));
			ApplyMove(CB, Move("(d1,Queen)"));
			CB.GetPieceAtPosition(Pos("d8")).PieceType.Should().Be(ChessPieceType.Queen, "Pawn was promoted to queen");
			CB.GetPieceAtPosition(Pos("d1")).PieceType.Should().Be(ChessPieceType.Queen, "Pawn was promoted to queen");
			CB.Value.Should().Be(0, "No pieces captured so the default value of 0 is applied");

		}

		/// <summary>
		/// Confirm a pawn cannot be promoted to pawn
		/// </summary>
		[Fact]
		public void PawnCannotBePromotedToPawn() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("h2", "h4"),
					 Move("b8", "a6"),
					 Move("h4", "h5"),
					 Move("a6", "b8"),
					 Move("h5", "h6"),
					 Move("b8", "a6"),
					 Move("h6", "g7"),
					 Move("a6", "b8"),
					 Move("g7", "h8")
				});
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be waiting to be promoted after capture");
			b.CurrentPlayer.Should().Be(1, "Player should still have it's 'turn' as a means to select promotion");
			b.GetPossibleMoves().Count().Should().Be(4, "No pawn promotion allowed");
		}

		/// <summary>
		/// Confirm a pawn has appropriate effects when promoted
		/// </summary>
		[Fact]
		public void PawnPromotionVerification() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("h2", "h4"),
					 Move("b8", "a6"),
					 Move("h4", "h5"),
					 Move("a6", "b8"),
					 Move("h5", "h6"),
					 Move("b8", "a6"),
					 Move("h6", "g7"),
					 Move("a6", "b8"),
					 Move("g7", "h8")
				});
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should be waiting to be promoted after capture");
			b.CurrentPlayer.Should().Be(1, "Player should still have it's 'turn' as a means to select promotion");
			ApplyMove(b, Move("h8, Queen"));
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Queen, "Pawn should be Queen after Promotion");
			b.CurrentPlayer.Should().Be(2, "Player should still have it's 'turn' as a means to select promotion");
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Pawn, "Queen should be Pawn after Undo");
			b.CurrentPlayer.Should().Be(1, "Player should be 1 after undo");
			ApplyMove(b, Move("h8, Bishop"));
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.Bishop, "Pawn should be Bishop after promotion");
		}

		/// <summary>
		/// Test that will promote a pawn to a queen and test that
		/// the piece on the board has updated
		/// </summary>
		[Fact]
		public void PromotionTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a7"), ChessPieceType.Pawn, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("b6"), ChessPieceType.Knight, 2,
				 Pos("e8"), ChessPieceType.King, 2);
			ApplyMove(b, Move("a7", "a8"));
			ApplyMove(b, Move("(a8, Queen)"));
			var shouldBeQueen = b.GetPieceAtPosition(Pos(0, 0)).PieceType;
			shouldBeQueen.Should().Be(ChessPieceType.Queen, "Pawn should have been promoted to queen");

		}

		/// <summary>
		/// Checks for pawn promotion 
		/// </summary>
		[Fact]
		public void PromotionAndUndo() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("h7"), ChessPieceType.Pawn, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("g8"), ChessPieceType.Queen, 2,
				 Pos("e8"), ChessPieceType.King, 2
				 );
			ApplyMove(b, Move("h7", "h8"));
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Pawn, "the pawn is not promoted yet");

			ApplyMove(b, Move("h8, Rook"));
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.RookPawn, "the pawn promted to a rook");
			b.CurrentPlayer.Should().Be(2, "it is now black's turn");

			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.Pawn, "undo should have made the rook back to a pawn");
			b.CurrentPlayer.Should().Be(1, "it should still white's turn");
		}

		/// <summary>
		/// Pawn Promotion: Test to see if count of Queen, Knight, Rook, Bishop can increase count 
		/// but not by more than the number of same side pawns. 
		/// also must be one of these four!
		/// </summary>
		[Fact]
		public void PawnPromotion() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a7"), ChessPieceType.Pawn, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e8"), ChessPieceType.King, 2
				 );

			ApplyMove(b, Move("a7", "a8"));
			var possMoves = b.GetPossibleMoves();
			possMoves.Should().HaveCount(4, "there are four possible promotion moves").And
				 .OnlyContain(m => ((ChessMove)m).MoveType == ChessMoveType.PawnPromote);
		}

		/// <summary>
		/// Tests if the pawn on the far left(a8) can get promoted to a bishop
		/// </summary>
		[Fact]
		public void PromotePawnToBishop() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("a2", "a4"),
				Move("a7", "a5"),
				Move("b2", "b4"),
				Move("a5", "b4"),
				Move("a4", "a5"),
				Move("a8", "a6"),
				Move("c2", "c4"),
				Move("a6", "h6"),
				Move("a5", "a6"),
				Move("c7", "c5"),
				Move("a6", "a7"),
				Move("b7", "b5"),
				Move("a7", "a8"),
				Move("(a8, Bishop)")
			});
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.Bishop, "Pawn starting at a8 moved to other side should be a Bishop.");

		}

		[Fact]
		// Test 7 - Test the player 1's pawn will promote to bishop and after undo will turn back into a pawn
		public void UndoPromotion() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("e8"), ChessPieceType.King, 2,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("a7"), ChessPieceType.Pawn, 1);

			ApplyMove(b, Move("a7", "a8"));
			ApplyMove(b, Move("(a8, Bishop)"));

			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Bishop,
				"because the pawn became a bishop)");

			b.UndoLastMove();
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Pawn,
				"because the promotion was undone)");
			b.UndoLastMove();

			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Empty,
				"because the undo move undid the pawn move");

			b.GetPieceAtPosition(Pos(1, 0)).PieceType.Should().Be(ChessPieceType.Pawn,
				"because the undo moves put the pawn in its original position");
		}

		[Fact]
		public void KNightPromotion() {
			ChessBoard b = CreateBoardWithPositions(
			Pos("g7"), ChessPieceType.Pawn, 1,
			Pos("b2"), ChessPieceType.Pawn, 2);
			ApplyMove(b, Move("g7", "g8"));
			ApplyMove(b, Move("g8, Knight"));
			ApplyMove(b, Move("b2", "b1"));
			ApplyMove(b, Move("b1, Knight"));
			b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Knight, "Pawn was promoted to a Knight");

		}

		[Fact]
		public void PromoteThenCheckMoves() {
			ChessBoard b = CreateBoardWithPositions(
			Pos("g7"), ChessPieceType.Pawn, 1,
			Pos("b2"), ChessPieceType.Pawn, 2);
			ApplyMove(b, Move("g7", "g8"));
			ApplyMove(b, Move("g8, Knight"));
			ApplyMove(b, Move("b2", "b1"));
			ApplyMove(b, Move("b1, Queen"));
			ApplyMove(b, Move("g8, e7"));
			//Currently Black's turn
			b.GetPossibleMoves().Should().Contain(Move("b1", "b8")).And.HaveCount(21, "There Should be 21 moves available");

		}

		/// <summary>
		/// Promote a pawn and undo
		/// </summary>
		[Fact]
		public void PawnPromotionUndoTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a7"), ChessPieceType.Pawn, 1
				 );

			b.GetPieceAtPosition(Pos(1, 0)).PieceType.Should().Be(ChessPieceType.Pawn, "Checking if the piece is actually a pawn");
			b.GetPieceAtPosition(Pos(1, 0)).Player.Should().Be(1, "Should be player 1 aka White");
			//var Pawn = b.GetPossibleMoves() as IEnumerable<ChessMove>;


			ApplyMove(b, Move("a7", "a8"));
			ApplyMove(b, Move("(a8, Queen)"));
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Queen, "Pawn should've been promoted to a Queen");

			b.UndoLastMove();
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.Pawn, "When the move is undone, the Queen returns to being a pawn");
		}

		/// <summary>
		/// Checks if pawn promotion is working.
		/// </summary>
		[Fact]
		public void PawnPromotionWhite() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a7"), ChessPieceType.Pawn, 1,
				 Pos("b7"), ChessPieceType.Queen, 2,
				 Pos("e8"), ChessPieceType.Queen, 1,
				 Pos("f5"), ChessPieceType.Pawn, 1,
				 Pos("f6"), ChessPieceType.Pawn, 2,
				 Pos("g4"), ChessPieceType.Pawn, 1,
				 Pos("g7"), ChessPieceType.Pawn, 2,
				 Pos("h4"), ChessPieceType.Pawn, 1,
				 Pos("h5"), ChessPieceType.King, 1,
				 Pos("h6"), ChessPieceType.Pawn, 2,
				 Pos("h7"), ChessPieceType.King, 2
			);

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedMoves = GetMovesAtPosition(possMoves, Pos("a7"));
			expectedMoves.Should().HaveCount(1, "There should only be one move.")
				 .And.Contain(Move("a7", "a8"), "The only possible mve is a7 to a8.");

			b.CurrentPlayer.Should().Be(1, "Current player should be White.");

			ApplyMove(b, Move("a7", "a8"));

			var piece = b.GetPieceAtPosition(Pos("a8"));

			piece.PieceType.Should().Be(ChessPieceType.Pawn, "Should be a pawn still.");

			b.CurrentPlayer.Should().Be(1, "Current player should still be White.");

			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			expectedMoves = GetMovesAtPosition(possMoves, Pos("a8"));
			expectedMoves.Should().HaveCount(4, "There should be four moves.")
				 .And.OnlyContain(m => ((ChessMove)m).MoveType == ChessMoveType.PawnPromote);

			ApplyMove(b, Move("(a8, Bishop)")); // Apply pawn promotion.

			piece = b.GetPieceAtPosition(Pos("a8"));

			piece.PieceType.Should().Be(ChessPieceType.Bishop, "Should be a bishop now.");

			b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

			b.UndoLastMove();

			b.CurrentPlayer.Should().Be(1, "Current player should be White.");

			piece = b.GetPieceAtPosition(Pos("a8"));

			piece.PieceType.Should().Be(ChessPieceType.Pawn, "Should be a pawn now.");

			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			expectedMoves = GetMovesAtPosition(possMoves, Pos("a8"));
			expectedMoves.Should().HaveCount(4, "There should be four moves.")
				 .And.OnlyContain(m => ((ChessMove)m).MoveType == ChessMoveType.PawnPromote);

		}

		[Fact]
		public void PawnPromotionTrick() {
            ChessBoard b = CreateBoardWithPositions(
            Pos("f3"), ChessPieceType.Pawn, 2,
            Pos("a7"), ChessPieceType.Pawn, 1,
            Pos("e8"), ChessPieceType.King, 2,
			Pos("e1"), ChessPieceType.King, 1);
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			ApplyMove(b, Move("a7", "a8"));
            var pawnpromotionexpected = GetMovesAtPosition(possMoves, Pos("a8"));
            possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
            b.Value.Should().Be(0);
			ApplyMove(b, Move("(a8, Rook)"));
            b.Value.Should().Be(4);
			var pawnpromotion = b.GetPieceAtPosition(Pos("a8"));
			pawnpromotion.PieceType.Should().Be(ChessPieceType.RookPawn, "White pawn should be Rook");
		}

		/// <summary>
		/// Test that will promote a pawn to a queen 
		/// and then undo the promotion and repromote to a bishop
		/// which should rmeove the check status
		/// </summary>
		[Fact]
		public void UndoPromotion2() {
			ChessBoard b = CreateBoardWithPositions(
			  Pos("a7"), ChessPieceType.Pawn, 1,
			  Pos("e1"), ChessPieceType.King, 1,
			  Pos("b6"), ChessPieceType.Knight, 2,
			  Pos("e8"), ChessPieceType.King, 2);
			ApplyMove(b, Move("a7", "a8")); //player 1
			ApplyMove(b, Move("(a8, Queen)")); //player 1
			var shouldBeQueen = b.GetPieceAtPosition(Pos(0, 0)).PieceType;
			shouldBeQueen.Should().Be(ChessPieceType.Queen,
				 "Pawn should have been promoted to queen");
			b.IsCheck.Should().Be(true, "player 2 should be on check");// player 2 is in check
			b.UndoLastMove(); //player 2 undoes
			var shouldBePawn = b.GetPieceAtPosition(Pos(0, 0)).PieceType;
			shouldBePawn.Should().Be(ChessPieceType.Pawn,
				 "Promoted pawn is back at selection of which piece to promote to");
			ApplyMove(b, Move("(a8, Bishop)")); //player 1
			b.IsCheck.Should().Be(false, "player 2 should not be on check");//player 2 no longer in check
		}

		[Fact]
		public void PawnPromoteTest() {
			ChessBoard b = CreateBoardWithPositions(
//				Pos("a1"), ChessPieceType.King, 1,
				Pos("h1"), ChessPieceType.King, 2,
				 Pos("a7"), ChessPieceType.Pawn, 1,
				 //Pos("b7"), ChessPieceType.Pawn, 1, // Comment this line out, and there should be 7 possible moves.
				 Pos("c7"), ChessPieceType.Pawn, 1,
				 Pos("d7"), ChessPieceType.Pawn, 1,
				 Pos("e7"), ChessPieceType.Pawn, 1,
				 Pos("f7"), ChessPieceType.Pawn, 1,
				 Pos("g7"), ChessPieceType.Pawn, 1,
				 Pos("h7"), ChessPieceType.Pawn, 1
			);
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			possMoves.Should().HaveCount(7);
		}

	}
}
