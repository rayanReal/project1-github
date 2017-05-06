using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class GeneralTests : ChessTests {
		/// <summary>
		/// Undo board completely and confirm it is restored to proper state
		/// </summary>
		[Fact]
		public void ConfirmUndoBoardAndVerifyBoardReset() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("d2", "d4"),
					 Move("e7", "e5"),
					 Move("d1", "d3"),
					 Move("d8", "e7"),
					 Move("d3", "e4"),
					 Move("e7", "a3"),
					 Move("b2", "a3"),
					 Move("a7", "a6"),
					 Move("e4", "e5")
			});
			for (int i = 0; i < 9; i++)
				b.UndoLastMove();
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					if (i < 2) {
						b.GetPlayerAtPosition(Pos(i, j)).Should().Be(2, "First two rows should be player 2");
					}
					else if (i > 5) {
						b.GetPlayerAtPosition(Pos(i, j)).Should().Be(1, "Last two rows should be player 1");
					}
					else {
						b.GetPlayerAtPosition(Pos(i, j)).Should().Be(0, "Middle of the board should have no player assigned");
					}
				}
			}
			b.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "White's king at position (7,4)");
			b.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Black's king at position (0,4)");

		}

		/// <summary>
		/// Test that will undo capture move and test the positions are as they were prior
		/// to the capture
		/// </summary>
		[Fact]
		public void UndoTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a7"), ChessPieceType.Queen, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a5"), ChessPieceType.Pawn, 1,
				 Pos("b6"), ChessPieceType.Queen, 2,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("c8"), ChessPieceType.Pawn, 2
				 );

			ApplyMove(b, Move("a5", "b6"));
			b.UndoLastMove();
			b.GetPieceAtPosition(Pos(2, 1)).PieceType.Should().Be(ChessPieceType.Queen);
			b.GetPieceAtPosition(Pos(3, 0)).PieceType.Should().Be(ChessPieceType.Pawn);

		}

		/// <summary>
		/// Checks for position value and value of current player
		/// </summary>
		[Fact]
		public void CheckWhiteValue() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			ApplyMove(b, Move("g2", "g3"));
			b.GetPieceAtPosition(Pos("g3")).Player.Should().Be(1, "white pawn should be here");
			b.CurrentPlayer.Should().Be(2, "the player is now black");

			b.UndoLastMove();
			b.GetPieceAtPosition(Pos("g3")).Player.Should().Be(0, "this location should be empty");
			b.GetPieceAtPosition(Pos("g2")).Player.Should().Be(1, "the pawn should be here");
			b.CurrentPlayer.Should().Be(1, "the player is white to start");
		}

		[Fact]
		public void TestSimpleUndo() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("b1", "a3"));
			ApplyMove(b, Move("b7", "b5"));
			ApplyMove(b, Move("a3", "b5"));

			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "Player should be white as white move just undone");
			b.GetPieceAtPosition(new BoardPosition(5, 0)).PieceType.Should().Be(ChessPieceType.Knight,
				 "Knight should be at correct position after undoing");

			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(2, "Player should be black as white move just undone");
			b.GetPieceAtPosition(new BoardPosition(1, 1)).PieceType.Should().Be(ChessPieceType.Pawn,
				 "Knight should be at correct position after undoing");

			b.UndoLastMove();
			b.ShouldBeEquivalentTo(new ChessBoard(), "All moves should be undone, board should resemble a new board");
		}

		[Fact]
		public void ValueTests() {
			ChessBoard b = new ChessBoard();
			b.Value.Should().Be(0, "New chessboard should have value of 0");
			ApplyMove(b, Move("b1", "a3"));
			ApplyMove(b, Move("b7", "b5"));
			ApplyMove(b, Move("a3", "b5"));
			b.Value.Should().Be(1, "Value should be 1 as pawn was just captured");
			ApplyMove(b, Move("c7", "c6"));
			b.Value.Should().Be(1, "Value should still be 1 as no new pieces have been captured");
			ApplyMove(b, Move("c2", "c4"));
			ApplyMove(b, Move("c6", "b5"));
			b.Value.Should().Be(-2, "Value should be -2 as knight was just captured");
			ApplyMove(b, Move("c4", "b5"));
			b.Value.Should().Be(-1, "Value should be -1 as pawn was just captured");
		}

		[Fact]
		public void ChainUndoMove() {
			ChessBoard board = CreateBoardWithPositions(
				 Pos("d6"), ChessPieceType.Pawn, 2,
				 Pos("f6"), ChessPieceType.Pawn, 2,
				 Pos("g5"), ChessPieceType.RookKing, 2,
				 Pos("a8"), ChessPieceType.King, 2,
				 Pos("e5"), ChessPieceType.Queen, 1,
				 Pos("c3"), ChessPieceType.Bishop, 1,
				 Pos("f3"), ChessPieceType.Knight, 1,
				 Pos("a1"), ChessPieceType.King, 1);
			GetAllPiecesForPlayer(board, 1).Should().HaveCount(4, "Queen, Bishop, Knight, King");
			GetAllPiecesForPlayer(board, 2).Should().HaveCount(4, "Pawn, Pawn, Rook, King");
			board.CurrentPlayer.Should().Be(1, "White plays first");
			ApplyMove(board, Move("a1", "a2"));

			//Chain Starts with Black's turn
			//Black Pawn captures White Queen
			ApplyMove(board, Move("d6", "e5"));
			board.Value.Should().Be(-9, "White Queen is captured");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Pawn, "by Pawn");
			//White Knight captures Black Pawn
			ApplyMove(board, Move("f3", "e5"));
			board.Value.Should().Be(-8, "Black Pawn is captured");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Knight, "by Knight");
			//Black Rook captures White Knight
			ApplyMove(board, Move("g5", "e5"));
			board.Value.Should().Be(-11, "White Knight is captured");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.RookKing, "by Rook");
			//White Bishop captures Black Rook
			ApplyMove(board, Move("c3", "e5"));
			board.Value.Should().Be(-6, "Black Rook is captured");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Bishop, "by Bishop");
			//Black Pawn captures White Bishop
			ApplyMove(board, Move("f6", "e5"));
			board.Value.Should().Be(-9, "White Bishop is captured");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Pawn, "by Pawn");
			GetAllPiecesForPlayer(board, 1).Should().HaveCount(1, "King");
			GetAllPiecesForPlayer(board, 2).Should().HaveCount(2, "King, Pawn");

			//Start Undo
			//White Bishop comes back to the board
			board.UndoLastMove();
			board.Value.Should().Be(-6, "White Bishop is alive");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Bishop);
			//Black Rook comes back to the board
			board.UndoLastMove();
			board.Value.Should().Be(-11, "Black Rook is alive");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.RookKing);
			//White Knight comes back to the board
			board.UndoLastMove();
			board.Value.Should().Be(-8, "White Knight is alive");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Knight);
			//Black Pawn comes back to the board
			board.UndoLastMove();
			board.Value.Should().Be(-9, "Black Pawn is alive");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Pawn);
			//White Queen comes back to the board
			board.UndoLastMove();
			board.Value.Should().Be(0, "White Queen is alive");
			board.GetPieceAtPosition(Pos("e5")).PieceType.Should().Be(ChessPieceType.Queen);

		}

		/// <summary>
		/// stalemate draw. Not check but cannot move
		/// </summary>
		[Fact]
		public void Stalemate() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e4"), ChessPieceType.King, 2,
				 Pos("b4"), ChessPieceType.Pawn, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e2"), ChessPieceType.Pawn, 2);
			ApplyMove(b, Move("b4", "b5")); //white move
			ApplyMove(b, Move("e4", "e3")); // black move
			b.CurrentPlayer.Should().Be(1); //white turn

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var KingMove = GetMovesAtPosition(poss, Pos("e1"));
			KingMove.Should().HaveCount(0, "Not Check, but King cant move and cant suicide");
		}

		/// <summary>
		/// Player one captures a pawn. Checks for current player and value of the board then undoes the capture. It then checks for 
		/// the current player and value of the board again.
		/// </summary>
		[Fact]
		public void UndoPawnCapture() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("a2", "a4"),
					 Move("a7", "a5"),
					 Move("b2", "b4"),
					 Move("g7", "g5"),
					 Move("b4", "a5"),
				});
			b.CurrentPlayer.Should().Be(2, "It should be black's turn.");
			b.Value.Should().Be(1, "The value of the board should be 1.");
			b.UndoLastMove();
			b.CurrentPlayer.Should().Be(1, "It should be white's turn.");
			b.Value.Should().Be(0, "The value of the board should be 0.");
		}

		/// <summary>
		/// Tests the possible moves for a knight before and after undoing two moves
		/// </summary>
		[Fact]
		public void UndoKnightMoves() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.Knight, "Piece at G1 should be knight.");
			ApplyMove(b, Move("g1", "f3"));
			b.GetPieceAtPosition(Pos("f3")).PieceType.Should().Be(ChessPieceType.Knight, "Piece at F3 should be knight.");
			ApplyMove(b, Move("a7", "a6"));
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("f3"));
			expected.Should().Contain(Move("f3", "e5")).And.Contain(Move("f3", "g5")).And.HaveCount(5, "There should be 5 possible moves.");
			b.UndoLastMove();
			b.UndoLastMove();
			poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			expected = GetMovesAtPosition(poss, Pos("g1"));
			expected.Should().Contain(Move("g1", "f3")).And.Contain(Move("g1", "h3")).And.HaveCount(2, "Knight back at original position. There should only be 2 possible moves.");

		}

		/// <summary>
		/// Tests by undoing multiple times and testing the value of the board
		/// </summary>
		[Fact]
		public void UndoTestThree() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("a2","a4"),
				Move("a7","a5"),
				Move("b2","b4"),
				Move("b7","b5"),
				Move("c2","c4"),
				Move("c7","c5"),
				Move("d2","d4"),
				Move("d7","d5"),
				Move("e2","e4"),
				Move("e7","e5"),
				Move("f2","f4"),
				Move("f7","f5"),
				Move("g2","g4"),
				Move("g7","g5"),
				Move("h2","h4"),
				Move("h7","h5"),
				Move("a4","b5"),
				Move("a5","b4"),
				Move("c4","d5"),
				Move("c5","d4"),
				Move("e4","f5"),
				Move("e5","f4"),
				Move("g4","h5"),
				Move("g5","h4"),
			});
			b.Value.Should().Be(0, "The value of the board should be 0 because each player lost the same amount of pawns.");
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();
			b.Value.Should().Be(0, "The value of the board should be 0 because each player has all of their pawns back.");
		}

		[Fact]
		//Test 4 - Test for UndoLastMove for undoing multiple moves involing captures
		// First test that the pieces are back to where they were before the undo moves
		// Second test that there are 2 kings, 2 pawns, and 1 queen after undoing moves
		public void UndoLastMoveTest2() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("e8"), ChessPieceType.King, 2,
				Pos("e1"), ChessPieceType.King, 1,
				Pos("c6"), ChessPieceType.Pawn, 2,
				Pos("g6"), ChessPieceType.Pawn, 2,
				Pos("d1"), ChessPieceType.Queen, 1);

			ApplyMove(b, Move("d1, c1"));
			ApplyMove(b, Move("c6, c5"));
			ApplyMove(b, Move("c1, c5"));
			ApplyMove(b, Move("g6, g5"));
			ApplyMove(b, Move("c5, g5"));

			b.UndoLastMove();
			b.UndoLastMove();
			b.UndoLastMove();

			b.GetPieceAtPosition(Pos(3, 2)).PieceType.Should().Be(ChessPieceType.Pawn,
				 "because the first undo move put the pawn back at (3,2)");

			b.GetPieceAtPosition(Pos(2, 6)).PieceType.Should().Be(ChessPieceType.Pawn,
				 "because the second undo move put the pawn back at (2,6)");

			b.GetPieceAtPosition(Pos(7, 2)).PieceType.Should().Be(ChessPieceType.Queen,
				 "because the third undo move put the queen back at (7,2");

			var queenCount = GetAllPiecesForPlayer(b, 1).Where(x => x.PieceType == ChessPieceType.Queen);
			queenCount.Count().Should().Be(1, "because player 1 should have 1 queen on the board");

			var kingCount1 = GetAllPiecesForPlayer(b, 1).Where(x => x.PieceType == ChessPieceType.King);
			kingCount1.Count().Should().Be(1, "because player 1 should have 1 king on the board");

			var kingCount2 = GetAllPiecesForPlayer(b, 2).Where(x => x.PieceType == ChessPieceType.King);
			kingCount2.Count().Should().Be(1, "because player 2 should have 1 king on the board");

			var pawnCount2 = GetAllPiecesForPlayer(b, 2).Where(x => x.PieceType == ChessPieceType.Pawn);
			pawnCount2.Count().Should().Be(2, "because player 2 should have 2 pawns on the board");
		}

		/// <summary>
		/// Test to see if the game recognizes a draw
		/// </summary>
		[Fact]
		public void DrawTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("b4"), ChessPieceType.Queen, 1,
				 Pos("f4"), ChessPieceType.King, 1,
				 Pos("a8"), ChessPieceType.King, 2);

			// Player 1 moves to corner player 2's king
			ApplyMove(b, Move("b4", "b6"));

			// Player 2's turn, should not be in check, should be a stalemate
			b.IsCheck.Should().Be(false);
			var possMove = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedMove = GetMovesAtPosition(possMove, Pos("a8"));
			expectedMove.Should().HaveCount(0, "no place for player 2's king to go");
			b.IsStalemate.Should().Be(true);
		}

		[Fact]
		public void PlayerSwitchTest() {
			var board = new ChessBoard();
			ApplyMovesToBoard(board, new ChessMove[] {
					 Move("g1", "f3"),
					 Move("a7", "a6"),
					 Move("g2", "g3"),
					 Move("b7", "b5"),
					 Move("f1", "h3"),
					 Move("c7", "c6")
				});

			// Check if values swapped correctly
			board.Value.Should().Be(0);
			board.CurrentPlayer.Should().Be(1, "player doesn't switch every move");

			// Check if a special move swaps
			ApplyMove(board, Move("e1", "g1"));
			board.CurrentPlayer.Should().Be(2, "player doesn't switch after castling");
		}

		[Fact]
		public void PlayerUndoSwitchTest() {
			var board = new ChessBoard();
			ApplyMovesToBoard(board, new ChessMove[] {
					 Move("g1", "f3"),
					 Move("a7", "a6"),
					 Move("g2", "g3"),
					 Move("b7", "b5"),
					 Move("f1", "h3"),
					 Move("c7", "c6")
				});

			// Check if a special move swaps
			ApplyMove(board, Move("e1", "g1"));
			board.CurrentPlayer.Should().Be(2, "player doesn't switch after castling");
			board.UndoLastMove();
			board.CurrentPlayer.Should().Be(1, "player doesn't switch after undoing castling");
		}

		[Fact]
		public void Player1UndoLastMoveTest() {
			ChessBoard board = CreateBoardFromMoves(new ChessMove[] {
					 Move("g2", "g4"),   // Player 1's pawn moves 2 spaces
                Move("d7", "d5"),   // Player 2's pawn moves 2 spaces
                Move("g1", "f3"),   // Player 1's Knight moves one time to the left
                Move("b8", "c6"),   // Player 2's Knight moves one time to the right
                Move("h1", "g1"),   // Player 1's RookKing moves 1 space to the left
                Move("c8", "g4"),   // Player 2's Bishop captured Player 1's pawn
                Move("g1", "g4")}); // Player 1's Rook captured Player 2's Bishop

			// Player 1's Rook at g4
			board.GetPieceAtPosition(Pos("g4")).Player.Should().Be(1, "Player 1's Rook captured Player 2's Bishop vertically");
			// Board Value = 3
			board.Value.Should().Be(board.GetPieceValue(ChessPieceType.Bishop) - board.GetPieceValue(ChessPieceType.Pawn),
			"Player 2's Bishop is captured");

			// Undo Previous Move
			board.UndoLastMove();
			// Board Value = -1
			board.Value.Should().Be(-1, "Player 2 is currently winning after undid Bishop captured");

			// Get all possible moves
			var moves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			// Get all Player 1's rook expected moves
			var rookMovesExpected = GetMovesAtPosition(moves, Pos("g1"));
			// Player 1's Rook at g1
			board.GetPieceAtPosition(Pos("g1")).Player.Should().Be(1,
				 "Player 1's Chess piece after undid Bishop captured");
			board.GetPieceAtPosition(Pos("g1")).PieceType.Should().Be(ChessPieceType.RookKing,
				 "Chess Piece is RookKing after undid Bishop captured");
			// Player 2's Bishop at g4
			board.GetPieceAtPosition(Pos("g4")).Player.Should().Be(2, "Player 2's Chess piece after undid Bishop catpured");
			board.GetPieceAtPosition(Pos("g4")).PieceType.Should().Be(ChessPieceType.Bishop,
				 "Chess Piece is Bishop after undid Bishop captured");
			// Test all rook expected moves
			rookMovesExpected.Should().Contain(Move("g1", "h1")).And.Contain(Move("g1", "g2")).
				 And.Contain(Move("g1", "g3")).And.Contain(Move("g1", "g4")).
				 And.HaveCount(4, "RookKing is back to position g1 before captured Player 2's Bishop");

		}

		[Fact]
		public void Player2UndoLastMoveTest() {
			ChessBoard board = CreateBoardFromMoves(new ChessMove[] {
					 Move("b2", "b4"),   // Player 1's pawn moves two spaces
                Move("a7", "a5"),   // Player 2's pawn moves two spaces
                Move("b4", "a5"),   // Player 1's pawn captured Player 2's pawn
                Move("a8", "a5"),   // Player 2's RookKing captured Player 1's pawn 
                Move("b1", "a3"),   // Player 1's Knight moves
                Move("a5", "a3")}); // Player 2's RookQueen captured Player 1's Knight

			// Player 2's turn
			board.GetPieceAtPosition(Pos("a3")).Player.Should().Be(2, "Player 2's RookQueen captured Player 1's Knight vertically");
			// Board Value = -3
			board.Value.Should().Be(board.GetPieceValue(ChessPieceType.Empty) - board.GetPieceValue(ChessPieceType.Knight),
			"Player 1's Knight is captured");

			// Undo Previous Move
			board.UndoLastMove();
			// Board Value = 0
			board.Value.Should().Be((int)ChessPieceType.Empty, "Noe one is winning after undid Knight captured");

			// Get all possible moves
			var moves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			// Get all rook expected moves
			var rookMovesExpected = GetMovesAtPosition(moves, Pos("a5"));
			// Player 2's RookQueen at a5
			board.GetPieceAtPosition(Pos("a5")).Player.Should().Be(2, "Player 2's Chess piece after undid Knight captured");
			board.GetPieceAtPosition(Pos("a5")).PieceType.Should().Be(ChessPieceType.RookQueen,
				 "Chess Piece is RookQueen after undid Knight captured");
			// Player 1's Knight at a3
			board.GetPieceAtPosition(Pos("a3")).Player.Should().Be(1, "Players 1's Chess piece after undid Knight captured");
			board.GetPieceAtPosition(Pos("a3")).PieceType.Should().Be(ChessPieceType.Knight,
				 "Chess Piece is Knight after undid Knight captured");
			// Test all Rook expected moves
			rookMovesExpected.Should().Contain(Move("a5", "a6")).And.Contain(Move("a5", "a7")).
				 And.Contain(Move("a5", "a8")).And.Contain(Move("a5", "a4")).
				 And.Contain(Move("a5", "b5")).And.Contain(Move("a5", "c5")).
				 And.Contain(Move("a5", "d5")).And.Contain(Move("a5", "e5")).
				 And.Contain(Move("a5", "f5")).And.Contain(Move("a5", "g5")).
				 And.Contain(Move("a5", "h5")).And.Contain(Move("a5", "a3")).
				 And.HaveCount(12, "RookQueen is back to position a5 before captured Player 1's Knight");
		}
	}
}
