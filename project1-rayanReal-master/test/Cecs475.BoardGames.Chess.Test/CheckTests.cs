using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class CheckTests : ChessTests {
		//Test for getting moves for a king in check
		//Test 6: Get possible moves for king in check (black)
		[Fact]
		public void KingInCheckTest() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("e7"), ChessPieceType.King, 2,
				Pos("c6"), ChessPieceType.Pawn, 2,
				Pos("b4"), ChessPieceType.Knight, 1,
				Pos("f4"), ChessPieceType.Queen, 1
			);

			b.CurrentPlayer.Should().Be(1);
			//Move applied to make black the current player
			ApplyMove(b, Move("b4", "d5")); //Knight places king in check
			b.CurrentPlayer.Should().Be(2);

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingMoves = GetMovesAtPosition(poss, Pos("e7"));
			kingMoves.Should().HaveCount(4, "King in check must move out of check.");
		}

		/// <summary>
		/// Check the kings piece movement when in check. The king has 6 avaiable spaces as the bishop blocks two of them. 
		/// </summary>
		[Fact]
		public void KingInCheckTest2() {
			ChessBoard CB = CreateBoardWithPositions(
				 Pos("d5"), ChessPieceType.King, 1,
				 Pos("d8"), ChessPieceType.King, 2,
				 Pos("d7"), ChessPieceType.Bishop, 2);
			ApplyMove(CB, Move("d5", "e4"));
			ApplyMove(CB, Move("d7", "c6"));
			var possMoves = CB.GetPossibleMoves() as IEnumerable<ChessMove>;
			var king = GetMovesAtPosition(possMoves, Pos("e4"));
			king.Should().Contain(Move("e4", "e5"))
				 .And.Contain(Move("e4", "f5"))
				 .And.Contain(Move("e4", "f4"))
				 .And.Contain(Move("e4", "e3"))
				 .And.Contain(Move("e4", "d3"))
				 .And.Contain(Move("e4", "d4"), "6 possiable moves for the king to get out of check").And.HaveCount(6);
			CB.Value.Should().Be(0, "No pieces have been captured so value is 0");
		}

		/// <summary>
		/// Verifies checking the King is working and valid moves to avoid a check mate
		/// </summary>
		[Fact]
		public void ConfirmCheckAndValidMoves() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[]
			{
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
			var possible = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			possible.Count().Should().Be(3, "There should be 3 possible moves to cancel the checking of King");
			possible.Should().Contain(Move("e8", "d8")).And.Contain(Move("f8", "e7")).And.
				Contain(Move("g8", "e7"), "the king cannot move into check");
		}

		[Fact]
		public void KingCheck1() {
			// Create board with the king about to be in check
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("e2", "e3"),
				Move("b7", "b6"),
				Move("e1", "e2")
			});

			b.GetPieceAtPosition(Pos("e2")).PieceType.Should().Be(ChessPieceType.King, "space should have king before check");
			b.GetPieceAtPosition(Pos("a6")).PieceType.Should().Be(ChessPieceType.Empty, "space should be empty before check");

			// Move bishop to put king in check
			ApplyMove(b, Move("c8, a6"));

			b.GetPieceAtPosition(Pos("e2")).PieceType.Should().Be(ChessPieceType.King, "space should have king after check");
			b.GetPieceAtPosition(Pos("a6")).PieceType.Should().Be(ChessPieceType.Bishop, "space should have bishop after check");

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			// Since king is in check, the king has to move out of check so either king moves downward or pawns move up to block
			// diagonal path to king from bishop
			possMoves.Should().HaveCount(4, "king can only move down/diagonally or pawns move up");
		}


		[Fact]
		public void ForceKingMovesWhenInCheck() {//Force player to move piece to save the King
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("f7"), ChessPieceType.RookKing, 2,
				 Pos("h6"), ChessPieceType.Bishop, 1,
				 Pos("f2"), ChessPieceType.RookKing, 1,
				 Pos("f1"), ChessPieceType.Queen, 1,
				 Pos("d2"), ChessPieceType.RookKing, 1,
				 Pos("a1"), ChessPieceType.King, 1
				 );
			ApplyMove(b, Move("f1", "e1"));
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			poss.Should().Contain(Move("f7", "e7")).And.HaveCount(1, "Rook must move to block the queen from taking the king");
		}

		/// <summary>
		/// Test 6 black puts white king in check on black's 2nd turn.
		/// </summary>
		[Fact]
		public void PutInCheck() {
			ChessBoard b = new ChessBoard();
			// Move white's pawn 1 space.
			ApplyMove(b, Move("e2", "e3"));
			b.GetPieceAtPosition(Pos(5, 4)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at postition (5,4)");
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			// Move black's pawn 1 space.
			ApplyMove(b, Move("e7", "e6"));
			b.GetPieceAtPosition(Pos(2, 4)).PieceType.Should().Be(ChessPieceType.Pawn, "Black's pawn at postition (2,4)");
			// Player 1's turn.
			b.CurrentPlayer.Should().Be(1);
			// Move white's pawn 1 space.
			ApplyMove(b, Move("d2", "d3"));
			b.GetPieceAtPosition(Pos(5, 3)).PieceType.Should().Be(ChessPieceType.Pawn, "White's pawn at postition (5,3)");
			// Player 2's turn.
			b.CurrentPlayer.Should().Be(2);
			// Move black's bishop 4 spaces and puts white king in check.
			ApplyMove(b, Move("f8", "b4"));
			b.GetPieceAtPosition(Pos(4, 1)).PieceType.Should().Be(ChessPieceType.Bishop, "Black's bishop at postition (4,1)");
			// Get all the possible moves for white's king that's in check.
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedMoves = GetMovesAtPosition(poss, Pos("e1"));
			expectedMoves.Should().Contain(Move("e1", "e2"));
		}

		// puts the king in check and shows the available moves to put the king out of check if there are any
		[Fact]
		public void InCheck() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("e2", "e4"));
			ApplyMove(b, Move("e7", "e6"));
			ApplyMove(b, Move("e1", "e2"));
			ApplyMove(b, Move("f7", "f5"));
			ApplyMove(b, Move("e2", "e3"));
			ApplyMove(b, Move("f5", "f4"));
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var king = GetMovesAtPosition(possMoves, Pos("e3"));
			king.Should().Contain(Move("e3", "f4")).And.Contain(Move("e3", "f3")).And.Contain(Move("e3", "e2")).And.
				 Contain(Move("e3", "d3")).And.Contain(Move("e3", "d4"), "possile moves for king after he has been put in check");
			b.CurrentPlayer.Should().Be(1);

		}

		/// <summary>
		/// Checks for possible moves for white king after placed to check
		/// </summary>
		[Fact]
		public void KingAfterCheck() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a1"), ChessPieceType.RookQueen, 1,
				 Pos("h8"), ChessPieceType.RookKing, 1,
				 Pos("e8"), ChessPieceType.King, 1,
				 Pos("e2"), ChessPieceType.King, 2
				 );

			b.Value.Should().Be(0);
			ApplyMove(b, Move("h8", "h2"));
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var king = GetMovesAtPosition(possMoves, Pos("e2"));
			b.IsCheck.Should().BeTrue("the rook should have placed the king to check");

			king.Should().Contain(Move("e2", "e3")).And.Contain(Move("e2", "d3")).And
				 .Contain(Move("e2", "f3")).And.HaveCount(3, "there are thee moves to get out from check");
		}

		/// <summary>
		/// InCheckPossibleMoves: This function tests all possible moves for a king in check.
		/// </summary>
		[Fact]
		public void InCheckPossibleMoves() {
			ChessBoard b1 = CreateBoardWithPositions(
				 Pos("f1"), ChessPieceType.King, 1,
				 Pos("c5"), ChessPieceType.Pawn, 1,
				 Pos("b7"), ChessPieceType.Pawn, 2,
				 Pos("g8"), ChessPieceType.King, 2,
				 Pos("a1"), ChessPieceType.RookKing, 1
				 );
			ApplyMove(b1, Move("a1", "a8"));
			var possMoves = b1.GetPossibleMoves();
			possMoves.Should().HaveCount(3, "The King can only move to 3 squares");

		}

		[Fact]
		public void KingInCheckPossMoves() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("a4"), ChessPieceType.Bishop, 1,
				 Pos("e1"), ChessPieceType.RookKing, 1);
			b.GetThreatenedPositions(1).Should().Contain(new BoardPosition(0, 4), "King should be in threatened list");
			ApplyMove(b, Move("e1", "e2"));

			b.GetPossibleMoves().Count().Should().Be(3, "Only 3 possible moves for king in this position");
			b.GetPossibleMoves().Should().Contain(Move("e8", "d8")).And
				 .NotContain(Move("e2", "f3"), "King should have this possible move in this position.");
			b.GetPossibleMoves().Should().Contain(Move("e8", "f8")).And
				 .NotContain(Move("e2", "f3"), "King should have this possible move in this position.");
			b.GetPossibleMoves().Should().Contain(Move("e8", "f7")).And
				 .NotContain(Move("e2", "f3"), "King should have this possible move in this position.");
		}

		[Fact]
		public void checkKing() {
			ChessBoard board = new ChessBoard();
			//Quick-Check to Black
			//White pawn
			ApplyMove(board, Move("e2", "e4"));
			//Black pawn
			ApplyMove(board, Move("e7", "e5"));
			//White Knight
			ApplyMove(board, Move("g1", "f3"));
			//Black Pawn
			ApplyMove(board, Move("d7", "d6"));
			//White Bishop calling Check
			ApplyMove(board, Move("f1", "b5"));
			board.IsCheck.Should().Be(true, "Black King is checked by White Bishop");
			board.CurrentPlayer.Should().Be(2, "Black's turn in check position");
			board.GetPossibleMoves().Should().HaveCount(6)
				.And.Contain(Move("b8", "c6")) //Knight moving in front of Pawn
				.And.Contain(Move("b8", "d7")) //Knight moving in front of Queen
				.And.Contain(Move("c7", "c6")) //Pawn moving one forward
				.And.Contain(Move("c8", "d7")) //Bishop moving in front of Queen
				.And.Contain(Move("d8", "d7")) //Queen moving one forward
				.And.Contain(Move("e8", "e7")); //King moving one forward
		}

		
		/// <summary>
		/// Check the king
		/// </summary>
		[Fact]
		public void KingCheck() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("h1"), ChessPieceType.King, 1,
				 Pos("a8"), ChessPieceType.King, 2,
				 Pos("b5"), ChessPieceType.Bishop, 2,
				 Pos("h6"), ChessPieceType.RookKing, 2,
				 Pos("f5"), ChessPieceType.Queen, 2);


			ApplyMove(b, Move("h1", "g1")); //king move
			ApplyMove(b, Move("f5", "f1")); //queen check
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var KingMoves = GetMovesAtPosition(poss, Pos("g1"));
			KingMoves.Should().HaveCount(0, "Check");
		}

		/// <summary>
		/// Knight can not move becaue king would be in check
		/// </summary>
		[Fact]
		public void CantMove() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("e6"), ChessPieceType.RookKing, 2,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("e2"), ChessPieceType.Knight, 1,
				 Pos("a3"), ChessPieceType.Pawn, 1); //create the board

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var KnightMove = GetMovesAtPosition(poss, Pos("e2")); //knight move
			var PawnMove = GetMovesAtPosition(poss, Pos("a3"));
			KnightMove.Should().HaveCount(0, "king would be in check if the knight moved"); // knight cannot move
			PawnMove.Should().HaveCount(1, "Pawn can move");
		}

		/// <summary>
		/// Puts the king in check and makes sure there are only
		/// 4 poss moves for the black player in this scenario
		/// </summary>
		[Fact]
		public void KingCheckTest() {

			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("e2", "e4"),
					 Move("f7", "f5"),
					 Move("a2", "a3"),
					 Move("a7", "a6"),
					 Move("e4", "f5"),
					 Move("e7", "e6"),
					 Move("a3", "a4"),
					 Move("e6", "f5"),
				});

			//The next move should check black
			ApplyMove(b, Move("d1", "e2"));

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			b.GetPieceAtPosition(Pos("e8")).PieceType.Should().Be(ChessPieceType.King, "because we have not moved the king, it should be in original position");

			possMoves.Should().Contain(Move("d8", "e7")).And.Contain(Move("f8", "e7"))
				 .And.Contain(Move("e8", "f7")).And.Contain(Move("g8", "e7"))
				 .And.HaveCount(4, "becase the bishop and the queen have to protect the king");
		}

		/// <summary>
		/// This test also places the king in check and checks the moves that are possible. 
		/// </summary>
		[Fact]
		public void SecondKingCheckTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("f2", "f4"),
					 Move("e7", "e6"),
				});

			b.CurrentPlayer.Should().Be(1);

			ApplyMove(b, Move("a2", "a4"));
			b.CurrentPlayer.Should().Be(2);

			//This move puts white in check
			ApplyMove(b, Move("d8", "h4"));


			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			possMoves.Should().Contain(Move("g2", "g3")).And
				 .HaveCount(1, "because this is the only possible move when black checks white");
		}

		/// <summary>
		/// Test to check for all possible moves to save the black king. 
		/// </summary>
		[Fact]
		public void BlackKingCheckTest() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("d2", "d4"),
					 Move("e7", "e5"),
					 Move("d1", "d3"),
					 Move("e5", "e4"),
					 Move("d3", "e4"),
				});
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(poss, Pos("d8"));
			expected.Should().Contain(Move("d8,e7"), "Black Queen position can save the king by moving to E7.");
			expected = GetMovesAtPosition(poss, Pos("f8"));
			expected.Should().Contain(Move("f8,e7"), "Black Bishop at F8 can save the king by moving to E7.");
			expected = GetMovesAtPosition(poss, Pos("g8"));
			expected.Should().Contain(Move("g8,e7"), "Black Knight at G8 can save the king by moving to E7.");
			ApplyMove(b, Move("d8", "e7"));
			ApplyMove(b, Move("e4", "e7"));
			poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			expected = GetMovesAtPosition(poss, Pos("e8"));
			expected.Should().Contain(Move("e8", "e7"), "Black King can save itself by moving to E7.");
			expected = GetMovesAtPosition(poss, Pos("f8"));
			expected.Should().Contain(Move("f8", "e7"), "Black Bishop at F8 can save the king by moving to E7.");
			expected = GetMovesAtPosition(poss, Pos("g8"));
			expected.Should().Contain(Move("g8,e7"), "Black Knight at G8 can save the king by moving to E7.");
		}

		[Fact]
		//Test 6 - Test that places player 2's king in check and checks the possible moves that result
		public void KingCheckMoves() {
			//Board makes player 2's king in check
			ChessBoard b = CreateBoardWithPositions(
			Pos("e8"), ChessPieceType.King, 2,
			Pos("e1"), ChessPieceType.King, 1,
			Pos("c7"), ChessPieceType.RookQueen, 1);

			ApplyMove(b, Move("c7, c8"));

			b.IsCheck.Should().Be(true, "because the move put the king in check");

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingCheck = GetMovesAtPosition(possMoves, Pos("e8"));

			kingCheck.Should().Contain(Move("e8", "d7")).And.Contain(Move("e8", "e7")).And.Contain(Move("e8", "f7"))
				 .And.HaveCount(3, "because the king only has 3 moves");
		}

		/// <summary>
		/// Test possible moves after a check
		/// </summary>
		[Fact]
		public void CheckTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("d4"), ChessPieceType.King, 2,
				 Pos("g3"), ChessPieceType.Queen, 2,
				 Pos("a3"), ChessPieceType.RookQueen, 1,
				 Pos("g5"), ChessPieceType.RookKing, 1,
				 Pos("d2"), ChessPieceType.Knight, 1,
				 Pos("c1"), ChessPieceType.Bishop, 1);

			// Player 1 puts player 2 in check
			ApplyMove(b, Move("c1", "b2"));
			b.IsCheck.Should().BeTrue();

			// Check the moves for player 2's King and Queen
			var possKingMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedKingMoves = GetMovesAtPosition(possKingMoves, Pos("d4"));
			expectedKingMoves.Should().HaveCount(0, "king has no where to go");
			var possQueenMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expectedQueenMoves = GetMovesAtPosition(possQueenMoves, Pos("g3"));
			expectedQueenMoves.Should().HaveCount(1, "queen has only one way to attempt to save king")
				 .And.Contain(Move("g3", "c3"));
		}

		/// <summary>
		/// Places a king in check and checks the possible moves.
		/// </summary>
		[Fact]
		public void KingInCheckPossibleMoves() {
			ChessBoard board = CreateBoardWithPositions(
				 Pos("c1"), ChessPieceType.King, 1,
				 Pos("a1"), ChessPieceType.Queen, 2);

			//Verify possible moves of King when in check
			var moves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var expected = GetMovesAtPosition(moves, Pos("c1"));
			expected.Should().NotContain(Move("c1", "b1")).And.NotContain(Move("c1", "d1")).And.
				 HaveCount(2, "A king cannot move itself to a threating space when in check");


		}

		[Fact]
		public void CheckKing() {
			ChessBoard b = new ChessBoard();
			/*ApplyMove(b, Move("b2", "b4"));
			ApplyMove(b, Move("b7", "b5"));
			ApplyMove(b, Move("d2", "d4"));
			ApplyMove(b, Move("b8", "a6"));
			ApplyMove(b, Move("e2", "e4"));
			ApplyMove(b, Move("b5", "a4"));
			ApplyMove(b, Move("d1", "h5"));
			ApplyMove(b, Move("c7", "c5"));
			//Black King is in Check
			//can only be saved by checking the queen
			ApplyMove(b, Move("h5", "f7"));
			b.GetPossibleMoves().Should().Contain(Move("e8", "f7")).And.HaveCount(1, "Must take quen for check");*/
		}

		[Fact]
		public void KingIsCheckTest() {
			ChessBoard board = CreateBoardWithPositions(
				 Pos("d6"), ChessPieceType.Bishop, 1,    // Player 1's bishop at d6
				 Pos("e2"), ChessPieceType.RookKing, 1,  // Player 1's RookKing at e2
				 Pos("g3"), ChessPieceType.King, 1,      // Plyer 1's King at g3
				 Pos("b4"), ChessPieceType.Knight, 2,    // Player 2's Knight at b4
				 Pos("b5"), ChessPieceType.King, 2);     // Player 2's Kinga t b5


			board.CurrentPlayer.Should().Be(1);  // Player 1's turn
			ApplyMove(board, Move("e2", "e5")); // Player 1's RookKing moves forward to position e5
			board.CurrentPlayer.Should().Be(2); // Player 2's turn
			board.IsCheck.Should().Be(true, "Player's 2 King is checked");  // Player 2's King is Checked
																								 // Get all possible moves
			var moves = board.GetPossibleMoves();
			// 7 Possible moves for Player 2's King to be unchecked
			moves.Should().HaveCount(6, "There are 7 possible moves for Player 2's King to get uncheck");

		}

		/// <summary>
		/// A pawn is promoted to queen and then the king is put into check
		/// </summary>
		[Fact]
		public void PromoteToCheck() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("b7"), ChessPieceType.Pawn, 1,
				 Pos("a2"), ChessPieceType.Knight, 1,
				 Pos("c1"), ChessPieceType.Knight, 1,
				 Pos("a1"), ChessPieceType.Knight, 2,
				 Pos("c2"), ChessPieceType.Knight, 2,
				 Pos("b1"), ChessPieceType.King, 2
				 );

			b.GetPieceAtPosition(Pos(6, 0)).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos(7, 0)).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos(6, 2)).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos(7, 2)).PieceType.Should().Be(ChessPieceType.Knight);
			b.GetPieceAtPosition(Pos(7, 1)).PieceType.Should().Be(ChessPieceType.King);

			ApplyMove(b, Move("b7", "b8"));
			ApplyMove(b, Move("(b8, Queen)"));
			b.GetPieceAtPosition(Pos(0, 1)).PieceType.Should().Be(ChessPieceType.Queen, "Pawn should've been promoted to a Queen");

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var KingInCheck = GetMovesAtPosition(possMoves, Pos("b1"));

			b.GetPieceAtPosition(Pos(6, 1)).Player.Should().Be(0, "This spot should be empty");
			b.GetPieceAtPosition(Pos(7, 1)).PieceType.Should().Be(ChessPieceType.King);
			KingInCheck.Should().HaveCount(0, "king cannot move to get out of check");
			b.IsCheck.Should().BeTrue();
		}

		/// <summary>
		/// Puts a king in check and then counting the only escape options that it has
		/// </summary>
		[Fact]
		public void KingInCheck() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("h2", "h4"),
					 Move("e7", "e5"),
					 Move("h1", "h3"),
					 Move("e8", "e7"),
					 Move("h3", "e3"),
					 Move("e5", "e4"),
					 Move("e3", "e4")
				});

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var KingInCheck = GetMovesAtPosition(possMoves, Pos("e7"));

			KingInCheck.Should().Contain(Move("e7", "d6")).And.Contain(Move("e7", "f6")).And.HaveCount(2, "The King in check should only have moves to escape being in check");
		}

		


		/// <summary>
		/// Checks if check detection is working correctly.
		/// </summary>
		[Fact]
		public void KingCheckBlack() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("c2"), ChessPieceType.RookQueen, 1,
				 Pos("c6"), ChessPieceType.King, 2,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("g2"), ChessPieceType.Pawn, 1
			);

			ApplyMove(b, Move("g2", "g3")); // This is to make sure player is now Black (to switch players from White to Black).

			b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

			var possMoves = b.GetPossibleMoves();

			possMoves.Count().Should().Be(6, "There should only be six moves available.");

			possMoves.Should().Contain(Move("c6", "b7"), "Should contain C6 to B7.")
				 .And.Contain(Move("c6", "d7"), "Should contain C6 to D7.")
				 .And.Contain(Move("c6", "b6"), "Should contain C6 to B6.")
				 .And.Contain(Move("c6", "d6"), "Should contain C6 to D6.")
				 .And.Contain(Move("c6", "d5"), "Should contain C6 to B5.")
				 .And.Contain(Move("c6", "d5"), "Should contain C6 to D5.");

			b.IsCheck.Should().Be(true, "Black's king should be checked.");
			b.IsCheckmate.Should().Be(false, "Should NOT be checkmate.");
			b.IsStalemate.Should().Be(false, "Not a stalemate.");

		}


		/// <summary>
		/// Check if black king is in check and makes sure that all possible peices are going to defend the king
		/// </summary>
		[Fact]
		public void checkBlackKingCheck() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("a2", "a4"), //white pawn to a4
                Move("e7", "e5"), //black pawn to e5
                Move("d2", "d4"), //white pawn to d4
                Move("a7", "a5"), //black pawn to a5
                Move("a1", "a3"), //white rook to a3
                Move("e5", "d4"), //black pawn capture white pawn
                Move("a3", "e3"), //white bishop to e6
            });

			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;

			//checks black king is in check
			b.IsCheck.Should().Be(true, "black king should be in check");
			poss.Should().HaveCount(4, "there should only be 4 moves to protect king");

			//check queen protecting king
			var expected = GetMovesAtPosition(poss, Pos("d8"));
			expected.Should().Contain(Move("d8", "e7")).And.HaveCount(1, "Queen must protect king");

			//check bishop protecting king
			expected = GetMovesAtPosition(poss, Pos("f8"));
			expected.Should().Contain(Move("f8", "e7")).And.HaveCount(1, "Bishop must protect king");

			//check knight protecting king
			expected = GetMovesAtPosition(poss, Pos("g8"));
			expected.Should().Contain(Move("g8", "e7")).And.HaveCount(1, "knight must protect king");

			//check pawn capture rook to protect king
			expected = GetMovesAtPosition(poss, Pos("d4"));
			expected.Should().Contain(Move("d4", "e3")).And.HaveCount(1, "pawn must protect king");

			//check pawn can not move vertically to protect king
			expected = GetMovesAtPosition(poss, Pos("d7"));
			expected.Should().HaveCount(0, "pawn can not move that way");
			expected = GetMovesAtPosition(poss, Pos("f7"));
			expected.Should().HaveCount(0, "pawn can not move that way");

			//check king can not move
			expected = GetMovesAtPosition(poss, Pos("e8"));
			expected.Should().HaveCount(0, "king can not move");
		}

		[Fact]
		public void KinginCheck() {
			ChessBoard b = CreateBoardWithPositions(
			Pos("d6"), ChessPieceType.Bishop, 1,
			Pos("f3"), ChessPieceType.Queen, 2,
			Pos("c2"), ChessPieceType.Queen, 1,
			Pos("a4"), ChessPieceType.King, 1,
			Pos("h8"), ChessPieceType.King, 2,
			Pos("e3"), ChessPieceType.Bishop, 2);
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			b.CurrentPlayer.Should().Be(1, "Player 1 starts the game");
			ApplyMove(b, Move("d6", "f4"));
			var d6 = b.GetPlayerAtPosition(Pos("d6"));
			b.CurrentPlayer.Should().Be(2, "Player 2 should have his turn in this instance");
			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var blackkingmoves = GetMovesAtPosition(possMoves, Pos("h8"));
			blackkingmoves.Should().HaveCount(2, "Black King should have 2 possible moves since he is currently in check with white queen");
			ApplyMove(b, Move("h8", "g8"));
			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var whitekingmoves = GetMovesAtPosition(possMoves, Pos("a4"));
			whitekingmoves.Should().HaveCount(5, "White King should have 5 possible moves since he is NOT in check with any piece");
		}

		/// <summary>
		/// Test places the king is able to move to after being put in check 
		/// at his starting spot by a pawn that was promoted to a queen
		/// </summary>
		[Fact]
		public void CheckTest2() {
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
			b.IsCheck.Should().Be(true);// player 2 is in check
			var checkMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			GetMovesAtPosition(checkMoves, Pos("e8")).Should().HaveCount(3,
				 "only 3 moves are possible when the king is in check here");
		}

		

		//Places a king in check and checks his possible moves
		[Fact]
		public void KingCheck2() {
			//Boxes in the king so all but one move is in a check
			ChessBoard board = CreateBoardWithPositions(
				 Pos("d8"), ChessPieceType.King, 2,
				 Pos("g4"), ChessPieceType.Pawn, 2,
				 Pos("a7"), ChessPieceType.RookKing, 1,
				 Pos("b1"), ChessPieceType.RookQueen, 1,
				 Pos("a1"), ChessPieceType.King, 1);
			ApplyMove(board, Move("b1", "d1")); //Move our Queen over to make the king uncomfortable
			var kingMoves = board.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingMovesExpected = GetMovesAtPosition(kingMoves, Pos("d8"));
			bool check = board.IsCheck;
			check.Should().BeTrue("The queen threatens the king, but our king can still escape.");
			kingMovesExpected.Should().Contain(Move("d8", "e8")).And.Contain(Move("d8", "c8")).And.
				 HaveCount(2, "King is in check and can only move to the left or the right, but not in the diagonals due to the rook.");
			var pawnMovesExpected = GetMovesAtPosition(kingMoves, Pos("g4"));
			pawnMovesExpected.Should().HaveCount(0, "The king is forced to move, all other pieces should be unable to move.");
		}

	}
}
