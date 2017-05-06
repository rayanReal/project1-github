using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class InitialBoardTests : ChessTests {
		[Fact]
		public void VerifyPlayerPiecesForInitialBoard() {
			ChessBoard b = new ChessBoard();
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
		}

		[Fact]
		public void TestStart() {//Tests out starting state of the board
			ChessBoard b = new ChessBoard();
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var KingMove = GetMovesAtPosition(poss, Pos("e8"));
			var QueenMove = GetMovesAtPosition(poss, Pos("d8"));
			var RookMove = GetMovesAtPosition(poss, Pos("h8"));
			var BishopMove = GetMovesAtPosition(poss, Pos("c8"));
			KingMove.Should().HaveCount(0, "King can only move one space and is blocked in starting state.");
			QueenMove.Should().HaveCount(0, "Queen is blocked in starting state and can't move.");
			RookMove.Should().HaveCount(0, "Rook is blocked in starting state and can't move.");
			BishopMove.Should().HaveCount(0, "Bishop is blocked in starting state and can't move");
		}

		/// <summary>
		/// Board test to make sure the queen and king are in the right 
		/// starting positions
		/// </summary>
		[Fact]
		public void InitialBoardTest() {
			ChessBoard b = new ChessBoard();
			var whiteKing = b.GetPieceAtPosition(new BoardPosition(7, 4));
			whiteKing.PieceType.Should().Be(ChessPieceType.King,
				 "White's King should be in this position");

			var whiteQueen = b.GetPieceAtPosition(new BoardPosition(7, 3));
			whiteQueen.PieceType.Should().Be(ChessPieceType.Queen,
				 "White's Queen should be in this position");
		}

		[Fact]
		public void ChessBoardState() {
			ChessBoard b = new ChessBoard();
			// Find white rooks.
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Black's rook at position (0,0)");
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Black's rook at position (0,7)");

			// Find black rooks.
			b.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "White's rook at position (7,0)");
			b.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "White's rook at position (7,7)");

			// Test other properties
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			b.Value.Should().Be(0, "the starting game has value 0");
		}

		// Checks to see if the initial starting board is in check
		[Fact]
		public void InitialStartingBoard() {
			ChessBoard b = new ChessBoard();
			for (int i = 0; i < 8; i++) {
				b.GetPieceAtPosition(Pos(6, i)).PieceType.Should().Be(ChessPieceType.Pawn);
				b.GetPieceAtPosition(Pos(1, i)).PieceType.Should().Be(ChessPieceType.Pawn);
				switch (i) {
					case 0:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.RookQueen);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.RookQueen);
						break;
					case 1:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Knight);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Knight);
						break;
					case 2:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Bishop);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Bishop);
						break;
					case 3:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Queen);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Queen);
						break;
					case 4:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.King);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.King);
						break;
					case 5:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Bishop);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Bishop);
						break;
					case 6:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.Knight);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.Knight);
						break;
					case 7:
						b.GetPieceAtPosition(Pos(7, i)).PieceType.Should().Be(ChessPieceType.RookKing);
						b.GetPieceAtPosition(Pos(0, i)).PieceType.Should().Be(ChessPieceType.RookKing);
						break;
				}
			}


		}

		/// <summary>
		/// Checking the initial state of the black piece in the beginning
		/// </summary>
		[Fact]
		public void initialState() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("d8")).Player.Should().Be(2, "This is a black piece");
			b.GetPieceAtPosition(Pos("d8")).PieceType.Should()
				 .Be(ChessPieceType.Queen, "A queen should start here");
		}

		/// <summary>
		/// GetInitPos: Test for pieces' starting positions on board.
		/// </summary>
		[Fact]
		public void GetInitPos() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "Black King");
			b.GetPieceAtPosition(Pos("e1")).PieceType.Should().Be(ChessPieceType.King, "White King");
		}
		[Fact]
		public void TestNewBoardState() {
			ChessBoard b = new ChessBoard();
			var threatWhite = b.GetThreatenedPositions(1)
				 .Where(p => !ChessBoard.PositionInBounds(p));
			var threatBlack = b.GetThreatenedPositions(2)
				 .Where(p => !ChessBoard.PositionInBounds(p));
			threatWhite.Should().BeEmpty("All threatened positions must be in bounds");
			threatBlack.Should().BeEmpty("All threatened positions must be in bounds");
			b.Value.Should().Be(0);
		}

		[Fact]
		public void InitialState2() {
			ChessBoard board = new ChessBoard();
			//test for the side of Rooks of each player
			board.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Player 2's Queen-side Rook");
			board.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Player 1's King-side Rook");
			//test for the player's position
			board.GetPlayerAtPosition(Pos(0, 0)).Should().Be(2, "Player 2");
			//test for the value at certain position test two thing at once:
			//  1. check the value of the piece
			//  2. check the piece at position
			board.GetPieceValue(board.GetPieceAtPosition(Pos(0, 0)).PieceType).Should().Be(5, "Player 2's Queen-side Rook");
			board.GetPieceValue(board.GetPieceAtPosition(Pos(0, 3)).PieceType).Should().Be(9, "Player 2's Queen");
			board.GetPieceValue(board.GetPieceAtPosition(Pos(7, 3)).PieceType).Should().Be(9, "Player 1's Queen");
			//test for the player at certain position
			board.GetPlayerAtPosition(Pos(0, 3)).Should().Be(2, "Player 2's Queen");
			//test for the enemy's location in perspective of certain player
			board.PositionIsEnemy(Pos(0, 3), 2).Should().BeFalse("Player 2's Queen, so it is not the enemy");
			board.PositionIsEnemy(Pos(3, 3), 2).Should().BeFalse("Empty position, so it is not the enemy");
			//test for empty position
			board.PositionIsEmpty(Pos(3, 3)).Should().BeTrue("Empty Position");
			board.PositionIsEmpty(Pos(0, 0)).Should().BeFalse("Player 2");
			//test for value
			board.Value.Should().Be(0, "Initial state should be tie game");
			//test for possible moves
			board.GetPossibleMoves().Should().HaveCount(20);
			//test for move history
			board.MoveHistory.Should().HaveCount(0);
		}

		/// <summary>
		/// create the new board and check the queens and knight positions. Also, check the initial value for the bord.
		/// </summary>
		[Fact]
		public void StartingBoardTest() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White's queen at position (7,3)"); //create the new board
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black's queen at position (0,3)");
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().NotBe(ChessPieceType.King, "Should Be Knight");
			b.Value.Should().Be(0, "the starting game should have value 0");
		}

		//tests a freshly initialized board
		[Fact]
		public void StartUpTest() {
			ChessBoard board = new ChessBoard();
			//starting positions
			board.GetPlayerAtPosition(Pos(3, 3)).Should().Be(0, "Center of board should be empty.");
			board.GetPlayerAtPosition(Pos(7, 7)).Should().Be(1, "Player 1 should be in the bottom two rows.");
			board.GetPlayerAtPosition(Pos(0, 7)).Should().Be(2, "Player 2 should be in the top two rows.");
			//piece specific positions
			board.GetPieceAtPosition(Pos(3, 3)).PieceType.Should().Be(ChessPieceType.Empty, "A space in the middle of the board should have nothing in the start.");
			board.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "White Rook to the left of the King. Left corner of the board.");
			board.GetPieceAtPosition(Pos(0, 2)).PieceType.Should().Be(ChessPieceType.Bishop, "Bishop on the black side of the board.");
			board.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Queen on the black side of the board.");
			board.CurrentPlayer.Should().Be(1, "Game starts on player one.");
			board.Value.Should().Be(0, "Starting value has a value of 0.");
		}

		/// <summary>
		/// Tests the initial board state by checking if the white rooks are in the right place
		/// </summary>
		[Fact]
		public void InitialBoardTest2() {
			ChessBoard b = new ChessBoard();
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.RookQueen, "A1 should be a rook to the left of the Queen.");
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.RookKing, "H1 should be a rook to the right of the King.");
			b.MoveHistory.Count.Should().Be(0, "There should be nothing in the move history.");

		}

		//Test 1 - Tests that there is only 1 queen for player 1 and for player 2 on the initial board
		[Fact]
		public void InitialBoardState() {
			ChessBoard b = new ChessBoard();
			var test = GetAllPiecesForPlayer(b, 1).Where(x => x.PieceType == ChessPieceType.Queen);
			test.Count().Should().Be(1, "because each player can only have 1 queen");

			var test2 = GetAllPiecesForPlayer(b, 2).Where(x => x.PieceType == ChessPieceType.Queen);
			test2.Count().Should().Be(1, "because each player can only have 1 queen");
		}

		/// <summary>
		/// Testing a newly created board
		/// </summary>
		[Fact]
		public void NewBoardTest() {
			ChessBoard b = new ChessBoard();
			b.GetPlayerAtPosition(Pos(7, 7)).Should().Be(1, "Player 1 should be in lower right of board");
			b.GetPlayerAtPosition(Pos(0, 7)).Should().Be(2, "Player 2 should be in upper right of board");
			b.GetPlayerAtPosition(Pos(3, 4)).Should().Be(0, "Middle of board should be empty");
			// Test a few select piece locations.
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White's queen at position (7,3)");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Black's queen at position (0,3)");
			// Test other properties
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			b.Value.Should().Be(0, "the starting game has value 0");
		}

		/// <summary>
		/// Tests all non pawn pieces to check if they are in the correct positions
		/// </summary>
		[Fact]
		public void InitialBoardState2() {
			ChessBoard board = new ChessBoard();

			//Test white player pieces
			board.GetPieceAtPosition(Pos(7, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Whites rook at 7,0");
			board.GetPieceAtPosition(Pos(7, 1)).PieceType.Should().Be(ChessPieceType.Knight, "Whites knight at 7,1");
			board.GetPieceAtPosition(Pos(7, 2)).PieceType.Should().Be(ChessPieceType.Bishop, "Whites bishop at 7,2");
			board.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Whites queen at 7,3");
			board.GetPieceAtPosition(Pos(7, 4)).PieceType.Should().Be(ChessPieceType.King, "Whites king at 7,4");
			board.GetPieceAtPosition(Pos(7, 5)).PieceType.Should().Be(ChessPieceType.Bishop, "Whites bishop at 7,5");
			board.GetPieceAtPosition(Pos(7, 6)).PieceType.Should().Be(ChessPieceType.Knight, "Whites knight at 7,6");
			board.GetPieceAtPosition(Pos(7, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Whites rook at 7,7");

			//Test black player pieces
			board.GetPieceAtPosition(Pos(0, 0)).PieceType.Should().Be(ChessPieceType.RookQueen, "Blacks rook at 0,0");
			board.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Knight, "Blacks knight at 0,1");
			board.GetPieceAtPosition(Pos(0, 2)).PieceType.Should().Be(ChessPieceType.Bishop, "Blacks bishop at 0,2");
			board.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Blacks queen at 0,3");
			board.GetPieceAtPosition(Pos(0, 4)).PieceType.Should().Be(ChessPieceType.King, "Blacks king at 0,4");
			board.GetPieceAtPosition(Pos(0, 5)).PieceType.Should().Be(ChessPieceType.Bishop, "Blacks bishop at 0,5");
			board.GetPieceAtPosition(Pos(0, 6)).PieceType.Should().Be(ChessPieceType.Knight, "Blacks knight at 0,6");
			board.GetPieceAtPosition(Pos(0, 7)).PieceType.Should().Be(ChessPieceType.RookKing, "Blacks rook at 0,7");

			//Test other properties
			board.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			board.Value.Should().Be(0, "the starting game has value 0");


		}

		[Fact]
		public void CheckInitialBoard() {
			ChessBoard b = new ChessBoard();
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("a2"));
			expected.Should().Contain(Move("a2", "a3")).And.Contain(Move("a2", "a4"));
		}

		[Fact]
		public void InitialEmptyPositionState() {
			ChessBoard board = new ChessBoard();
			for (int i = 2; i < 6; i++) {
				for (int j = 0; j < 8; j++) {
					board.GetPlayerAtPosition(Pos(i, j)).Should().Be(0, "Starting position (%d, %d) should be empty", i, j);
				}
			}
		}

		/// <summary>
		/// Checks that a brand new board, the rooks and bishops of both players have no moves
		/// </summary>
		[Fact]
		public void RookAndBishopStart() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var rook1 = GetMovesAtPosition(possMoves, Pos("a1"));
			rook1.Should().HaveCount(0, "Rook should have no initial moves at the beginning");

			var rook2 = GetMovesAtPosition(possMoves, Pos("h1"));
			rook2.Should().HaveCount(0, "Rook should have no initial moves at the beginning");

			var bishop1 = GetMovesAtPosition(possMoves, Pos("c1"));
			bishop1.Should().HaveCount(0, "Bishop should have no initial moves at the beginning");

			var bishop2 = GetMovesAtPosition(possMoves, Pos("e1"));
			bishop2.Should().HaveCount(0, "Bishop should have no initial moves at the beginning");
		}



		/// <summary>
		/// Test initial state by checking if rooks are in their correct places/have correct types.
		/// This tests involves both White and Black players.
		/// </summary>
		[Fact]
		public void InitialStartingStateWhiteAndBlack() {
			ChessBoard b = new ChessBoard();

			b.CurrentPlayer.Should().Be(1, "First player should be White.");
			b.Value.Should().Be(0, "New game, so score should be 0.");

			b.GetPlayerAtPosition(Pos("a1")).Should().Be(1, "This is Player 1's Rook.");
			b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.RookQueen, "This should be the Rook to the left of the Queen.");

			b.GetPlayerAtPosition(Pos("a8")).Should().Be(2, "This is Player 2's Rook.");
			b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.RookQueen, "This should be the Rook to the left of the Queen.");

			b.GetPlayerAtPosition(Pos("h1")).Should().Be(1, "This is Player 1's Rook.");
			b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.RookKing, "This should be the Rook to the right of the King.");

			b.GetPlayerAtPosition(Pos("h8")).Should().Be(2, "This is Player 2's Rook.");
			b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.RookKing, "This should be the Rook to the right of the King.");

		}

		[Fact]
		public void InitialSetup4() {
			ChessBoard b = new ChessBoard();
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			b.GetPieceAtPosition(Pos(7, 3)).PieceType.Should().Be(ChessPieceType.Queen, "White's queen at position (7,3)");
			var whitequeenmoves = GetMovesAtPosition(possMoves, Pos("d1"));
			whitequeenmoves.Should().HaveCount(0, "White Queen should have no possible moves at the start.");
			b.GetPieceAtPosition(Pos(0, 3)).PieceType.Should().Be(ChessPieceType.Queen, "Blacks's queen at position (7,3)");
			var blackqueenmoves = GetMovesAtPosition(possMoves, Pos("d8"));
			blackqueenmoves.Should().HaveCount(0, "Black Queen should have no possible moves at the start.");
		}

	}
}
