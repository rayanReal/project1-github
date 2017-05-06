using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test
{
    public class PawnTests : ChessTests
    {
		/// <summary>
		/// starting board 
		/// </summary>
		[Fact]
		public void pawnIntialBoardMovement() {
			ChessBoard CB = new ChessBoard();
			var possMoves = CB.GetPossibleMoves() as IEnumerable<ChessMove>;
			var pawn = GetMovesAtPosition(possMoves, Pos("a2"));
			pawn.Should().Contain(Move("a2", "a3")).And.Contain(Move("a2", "a4"), "because a pawn can move only one space or two spaces when starting off").And.HaveCount(2);
			CB.Value.Should().Be(0, "Starting board value is 0");
		}

		/// <summary>
		/// Undo by player 2
		/// </summary>
		[Fact]
		public void UndoPawnTest1() {
			ChessBoard CB = CreateBoardFromMoves(new ChessMove[]{
					Move("d2","d4"),  //player 1 turn, pawn 1
               Move("h7","h6"),  //player2 turn, pawn 2
               Move("d1","d3"), //player 1 turn, queen 
               Move("a7","a6"), //player 2 turn, pawn 3
               Move("d4","d5"), //player 1 turn, pawn 1
               Move ("h6","h5")}); //player 2, pawn  2
			CB.UndoLastMove();
			CB.CurrentPlayer.Should().Be(2, "Player 2 just undid his last move so it is his/her turn");
			CB.GetPieceAtPosition(Pos("h6")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn 2 was moved back from h5 to h6");
			CB.GetPieceAtPosition(Pos("d3")).PieceType.Should().Be(ChessPieceType.Queen, "Queen should be in postion d3 after move and should not be undone");
			CB.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn 1 should be in postion d5 after moving twic");
			CB.GetPieceAtPosition(Pos("a6")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn 3 should be in postion a6 after only moving once");
			CB.Value.Should().Be(0, "Board value should be 0 since no piece has been captured");
		}

		/// <summary>
		/// Undo by player 1
		/// </summary>
		[Fact]
		public void UndoPawnTest2() {
			ChessBoard CB = CreateBoardWithPositions(
				Pos("e1"), ChessPieceType.King, 1,
				Pos("e8"), ChessPieceType.King, 2,
				 Pos("c3"), ChessPieceType.Knight, 1,
				 Pos("d5"), ChessPieceType.Pawn, 2);
			ApplyMove(CB, Move("c3", "d5"));
			CB.UndoLastMove();
			CB.CurrentPlayer.Should().Be(1, "Player 1 undid the move for his knight so it is his turn");
			CB.GetPieceAtPosition(Pos("c3")).PieceType.Should().Be(ChessPieceType.Knight, "Knight should have returned to its intial starting postion c3.");
			CB.GetPieceAtPosition(Pos("d5")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn was captured but move was undone so the pawn should be in its postion d5.");
			CB.Value.Should().Be(0, "Knight captured pawn in the apply move and undo move was applied");
		}

		// checking to see if the pawn piece retains its possible moves from the previous turn after applying undolastmove()
		[Fact]
		public void UndoPawnTest3() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("b2", "b4"));
			ApplyMove(b, Move("h7", "h6"));
			ApplyMove(b, Move("d2", "d4"));
			ApplyMove(b, Move("g7", "g5"));
			ApplyMove(b, Move("c1", "g5"));
			ApplyMove(b, Move("h6", "g5"));
			b.UndoLastMove();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var pawn = GetMovesAtPosition(possMoves, Pos("h6"));
			pawn.Should().Contain(Move("h6", "g5")).And.Contain(Move("h6", "h5"), "pawn should retain all its moves from previous turn");
			b.GetPlayerAtPosition(Pos(2, 7)).Should().Be(2);
			b.GetPlayerAtPosition(Pos(3, 6)).Should().Be(1);
			b.CurrentPlayer.Should().Be(2);

		}

		[Fact]
		public void Pawns2SpaceThen1() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("c2", "c4"));
			ApplyMove(b, Move("c7", "c5"));
			b.GetPossibleMoves().Should().NotContain(Move("c4", "c5"),
				 "Pawn will collide with another pawn");
			ApplyMove(b, Move("a2", "a4"));
			ApplyMove(b, Move("h7", "h5"));
			b.GetPossibleMoves().Should().NotContain(Move("a4", "a6"),
				 "Pawn should only be able to move 1 foreward");
			ApplyMove(b, Move("a4", "a5"));
			b.GetPossibleMoves().Should().NotContain(Move("h5", "h3"),
				 "Pawn should only be able to move 1 foreward");
			ApplyMove(b, Move("h5", "h4"));
		}

		[Fact]
		// Test 3 - Test for UndoLastMove for undoing player 1's turn after moving a pawn
		// First test that it is still player 1's turn 
		// Second test that the pawn is back in the previous position
		// Third test that the spot is now empty
		public void PawnUndoMove() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("g1", "h3"), //move knight player 1
				Move("g8", "h6"), //move knight player 2
				Move("g2", "g3"), //move pawn player 1
			});
			b.UndoLastMove();

			// Test that it should be player 1's turn
			b.CurrentPlayer.Should().Be(1, "because player 1 just undid moving their pawn");

			// Test that the pawn is back in its spot
			b.GetPieceAtPosition(Pos(6, 6)).PieceType.Should().Be(ChessPieceType.Pawn,
			"because the pawn should be back in the previous position");

			//Test that the position where the pawn was moved to is now empty
			b.GetPieceAtPosition(Pos(5, 6)).PieceType.Should().Be(ChessPieceType.Empty,
			"because the piece moved there was undone");

		}

		/// <summary>
		/// Pawn capture
		/// </summary>
		[Fact]
		public void PawnCaptureTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("f2", "f4"),
					 Move("e7", "e5"),
				});

			var possMove = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedMove = GetMovesAtPosition(possMove, Pos("f4"));
			expectedMove.Should().Contain(Move("f4", "e5"))
				 .And.Contain(Move("f4", "f5"))
				 .And.HaveCount(2, "a pawn can capture diagonally ahead or move forward");

			b.Value.Should().Be(0, "no pieces have been captured yet");

			ApplyMove(b, Move("f4", "e5"));
			b.GetPieceAtPosition(Pos("e5"))
				 .Player.Should().Be(1, "Player 1 captured Player 2's pawn diagonally");
			b.Value.Should().Be(1, "Black lost a single pawn of 1 value");

			b.UndoLastMove();
			b.Value.Should().Be(0, "after undoing the pawn capture, the value should go down by 1");
		}

		/// <summary>
		/// Three moves with last move being pawn capture. Each move is undone and the resulting board is checked. 
		/// </summary>
		[Fact]
		public void UndoLastMovePawn() {
			ChessBoard board = new ChessBoard();

			//move white pawn forward two spaces
			ApplyMove(board, Move("a2", "a4"));
			board.CurrentPlayer.Should().Be(2, "blacks turn");

			//move black pawn forward two spaces
			ApplyMove(board, Move("b7", "b5"));
			board.CurrentPlayer.Should().Be(1, "whites turn");

			//white pawn takes black pawn
			ApplyMove(board, Move("a4", "b5"));
			var pawn = board.GetPieceAtPosition(Pos("b5"));
			pawn.Player.Should().Be(1, "whites pawn took blacks pawn");
			board.Value.Should().Be(1, "white took one of blacks pawn, so value should be 1");
			board.CurrentPlayer.Should().Be(2, "blacks turn");

			//check for white pawn after undoing whites attack
			board.UndoLastMove();
			var whitePawn = board.GetPieceAtPosition(Pos("a4"));
			whitePawn.PieceType.Should().Be(ChessPieceType.Pawn);
			whitePawn.Player.Should().Be(1, "white pawn reverted to old position");

			//check for black pawn after undoing whites attack
			var blackPawn = board.GetPieceAtPosition(Pos("b5"));
			blackPawn.PieceType.Should().Be(ChessPieceType.Pawn);
			blackPawn.Player.Should().Be(2, "black pawn reverted to old position");

			//revert to before black made a move
			board.UndoLastMove();
			board.GetPieceAtPosition(Pos("b7")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should revert back to original position");
			board.CurrentPlayer.Should().Be(2, "both players have gone, but the last move was undone, so it should be blacks turn again");
			board.Value.Should().Be(0, "the starting game has value 0");

			//rever back to before white made a move. Board is set to starting state again. 
			board.UndoLastMove();
			board.GetPieceAtPosition(Pos("a2")).PieceType.Should().Be(ChessPieceType.Pawn, "Pawn should rever back to original position");
			board.CurrentPlayer.Should().Be(1, "After reverting to original state of game, player 1 should start");
			board.Value.Should().Be(0, "the starting game has value 0");

		}

		[Fact]
		public void FullPawnCapturePawnUndo() {
			//Make a board with only pawns 
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("a2", "a4"));
			ApplyMove(b, Move("b7", "b5"));
			//White Captured 1
			ApplyMove(b, Move("a4", "b5"));

			ApplyMove(b, Move("d7", "d5"));
			ApplyMove(b, Move("c2", "c4"));
			//Black Capurted 1
			ApplyMove(b, Move("d5", "c4"));
			ApplyMove(b, Move("b5", "b6"));
			ApplyMove(b, Move("c4", "c3"));

			//Move The horsey
			ApplyMove(b, Move("b1", "c3"));

			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("b1")).PieceType.Should().Be(ChessPieceType.Knight, "Last Move knight capture pwn was undone");
		}

		/// <summary>
		/// Every other pawn moves forward 2 spaces giving MOST of them 3 possible moves
		/// </summary>
		[Fact]
		public void PawnPossMovesTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move ("a2", "a4"),
					 Move ("b7", "b5"),
					 Move ("c2", "c4"),
					 Move ("d7", "d5"),
					 Move ("e2", "e4"),
					 Move ("f7", "f5"),
					 Move ("g2", "g4"),
					 Move ("h7", "h5")
				});

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			var pawn1p1 = GetMovesAtPosition(possMoves, Pos("a4"));
			var pawn2p1 = GetMovesAtPosition(possMoves, Pos("c4"));
			var pawn3p1 = GetMovesAtPosition(possMoves, Pos("e4"));
			var pawn4p1 = GetMovesAtPosition(possMoves, Pos("g4"));

			pawn1p1.Should().Contain(Move("a4", "a5")).And.Contain(Move("a4", "b5")).And.HaveCount(2, "Should only have 2 moves");
			pawn2p1.Should().Contain(Move("c4", "b5")).And.Contain(Move("c4", "c5")).And.Contain(Move("c4", "d5")).And.HaveCount(3, "Should only have 3 moves");
			pawn3p1.Should().Contain(Move("e4", "d5")).And.Contain(Move("e4", "e5")).And.Contain(Move("e4", "f5")).And.HaveCount(3, "Should only have 3 moves");
			pawn4p1.Should().Contain(Move("g4", "f5")).And.Contain(Move("g4", "g5")).And.Contain(Move("g4", "h5")).And.HaveCount(3, "Should only have 3 moves");

			ApplyMove(b, Move("a1", "a2"));
			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			var pawn1p2 = GetMovesAtPosition(possMoves, Pos("b5"));
			var pawn2p2 = GetMovesAtPosition(possMoves, Pos("d5"));
			var pawn3p2 = GetMovesAtPosition(possMoves, Pos("f5"));
			var pawn4p2 = GetMovesAtPosition(possMoves, Pos("h5"));

			pawn1p2.Should().Contain(Move("b5", "a4")).And.Contain(Move("b5", "b4")).And.Contain(Move("b5", "c4")).And.HaveCount(3, "Should only have 3 moves");
			pawn2p2.Should().Contain(Move("d5", "c4")).And.Contain(Move("d5", "d4")).And.Contain(Move("d5", "e4")).And.HaveCount(3, "Should only have 3 moves");
			pawn3p2.Should().Contain(Move("f5", "e4")).And.Contain(Move("f5", "f4")).And.Contain(Move("f5", "g4")).And.HaveCount(3, "Should only have 3 moves");
			pawn4p2.Should().Contain(Move("h5", "g4")).And.Contain(Move("h5", "g4")).And.HaveCount(2, "Should only have 2 moves");
		}

		/// <summary>
		/// Checks if pawn movement is working as intended.
		/// </summary>
		[Fact]
		public void PawnsPossibleMovesBlack() // (ID + 1) % 6 = 1 => Test Pawns
		{
			/*ChessBoard b = new ChessBoard();
			b.ApplyMove(Move("a2", "a3")); // Make sure we switch to player 2 (Black).
			var pawnsPosition = b.GetPositionsOfPiece(ChessPieceType.Pawn, 2);
			char c = 'a'; // Set up for later.
			foreach (BoardPosition pawns in pawnsPosition) {
				var pawn = b.GetPieceAtPosition(pawns);
				pawn.PieceType.Should().Be(ChessPieceType.Pawn, "This should be a pawn.");
				pawn.Player.Should().Be(2, "This should be Black's pawn.");
				var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
				var expectedMoves = GetMovesAtPosition(possMoves, pawns);
				c.Should().BeLessThan('i', "var c should not have gone past letter h. Check pawnPositions (including its size/contents).");
				expectedMoves.Should().HaveCount(2, "There should only be two moves in the initial state")
					 .And.Contain(Move(c + "7", c + "6"), "First move should be one space ahead")
					 .And.Contain(Move(c + "7", c + "5"), "Second move should be two spaces ahead.");
				b.ApplyMove(Move(c + "7", c + "6")); // Move Black's pawn forward one spot.
				b.ApplyMove(Move("b2", "b3")); // Dummy move to change player.
				var newPossMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
				var newExpectedMoves = GetMovesAtPosition(newPossMoves, Pos(c + "6"));
				newExpectedMoves.Should().HaveCount(1, "There should only be one move...")
					 .And.Contain(Move(c + "6", c + "5"), "Move an only go forward one spot.");
				b.UndoLastMove(); // Undo dummy move.
				b.UndoLastMove(); // Undo pawn move.
				b.ApplyMove(Move(c + "7", c + "5")); // Move Black's pawn forward two spots now.
				b.ApplyMove(Move("b2", "b3")); // Dummy move to change player.
				newPossMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
				newExpectedMoves = GetMovesAtPosition(newPossMoves, Pos(c + "5"));
				newExpectedMoves.Should().HaveCount(1, "There should only be one move...")
					 .And.Contain(Move(c + "5", c + "4"), "Move an only go forward one spot.");
				c++; // Go on to the next letter...
			}*/
			/*ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("a2", "a3")); // Make sure we switch to player 2 (Black).
			var pawnsPosition = b.GetPositionsOfPiece(ChessPieceType.Pawn, 2);
			char c = 'a'; // Set up for later.
			bool movePawnsNow = false;
			foreach (BoardPosition pawns in pawnsPosition) {
				var pawn = b.GetPieceAtPosition(pawns);
				pawn.PieceType.Should().Be(ChessPieceType.Pawn, "This should be a pawn.");
				pawn.Player.Should().Be(2, "This should be Black's pawn.");
				var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
				var expectedMoves = GetMovesAtPosition(possMoves, pawns);
				c.Should().BeLessThan('i', "var c should not have gone past letter h. Check pawnPositions (including its size/contents).");
				expectedMoves.Should().HaveCount(2, "There should only be two moves in the initial state")
					 .And.Contain(Move(c + "7", c + "6"), "First move should be one space ahead")
					 .And.Contain(Move(c + "7", c + "5"), "Second move should be two spaces ahead.");
				ApplyMove(b, Move(c + "7", c + "6")); // Move Black's pawn forward one spot.
				if (movePawnsNow) {
					ApplyMove(b, Move(c + "2", c + "3")); // Dummy move to change player.
				}
				else {
					ApplyMove(b, Move(c + "1", c + "2")); // Dummy move to change player.
					movePawnsNow = true;
				}
				var newPossMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
				var newExpectedMoves = GetMovesAtPosition(newPossMoves, Pos(c + "6"));
				newExpectedMoves.Should().HaveCount(1, "There should only be one move...")
					 .And.Contain(Move(c + "6", c + "5"), "Move an only go forward one spot.");
				b.UndoLastMove(); // Undo dummy move.
				b.UndoLastMove(); // Undo pawn move.
				ApplyMove(b, Move(c + "7", c + "5")); // Move Black's pawn forward two spots now.
				ApplyMove(b, Move(c + "2", c + "3")); // Dummy move to change player.
				newPossMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
				newExpectedMoves = GetMovesAtPosition(newPossMoves, Pos(c + "5"));
				newExpectedMoves.Should().HaveCount(1, "There should only be one move...")
					 .And.Contain(Move(c + "5", c + "4"), "Move an only go forward one spot.");
				c++; // Go on to the next letter...
			}*/
		}

		/// <summary>
		/// Checks if attacking is working.
		/// </summary>
		[Fact]
		public void EasyPeasyAttackWhite() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[]
			{
					 Move("d2", "d4"),
					 Move("e7", "e5"),
			});

			var possMoves = b.GetPossibleMoves();
			possMoves.Should().Contain(Move("d4", "e5"), "Move from d4 to e5 should exist.");

			var blackPawn = b.GetPieceAtPosition(Pos("e5"));
			blackPawn.PieceType.Should().Be(ChessPieceType.Pawn, "This spot should contain a pawn.");
			blackPawn.Player.Should().Be(2, "This piece should be Black's.");

			ApplyMove(b, Move("d4", "e5")); // Attack the pawn (White's doing).

			var oldLocation = b.GetPieceAtPosition(Pos("d4"));
			oldLocation.PieceType.Should().Be(ChessPieceType.Empty, "This spot should now be empty.");

			var newLocation = b.GetPieceAtPosition(Pos("e5"));
			newLocation.PieceType.Should().Be(ChessPieceType.Pawn, "This spot should now contain a pawn.");
			newLocation.Player.Should().Be(1, "This piece should be White's.");

		}
	}
}
