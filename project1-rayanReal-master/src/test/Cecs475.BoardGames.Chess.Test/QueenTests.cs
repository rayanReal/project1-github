using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class QueenTests : ChessTests {
		/// <summary>
		/// Check the movement of the queens piece in chess. 
		/// </summary>
		[Fact]
		public void QueenMovementTest() {
			ChessBoard CB = CreateBoardFromMoves(new ChessMove[]{
					Move("d2","d4"),  //player 1 turn
               Move("h7","h6"),  //player2 turn
               Move("d1","d3"), //player 1 turn
               Move("a7","a6"), //player 2 turn
               Move("d4","d5"), //player 1 turn 
               Move ("h6","h5")});//player 2 turn 
			var possMoves = CB.GetPossibleMoves() as IEnumerable<ChessMove>;
			var queen = GetMovesAtPosition(possMoves, Pos("d3"));
			CB.CurrentPlayer.Should().Be(1, "Player 2 was the last player to make a turn so it is player 1 turn");
			queen.Should().Contain(Move("d3", "c3")).And.Contain(Move("d3", "b3")).And.Contain(Move("d3", "a3"))
							  .And.Contain(Move("d3", "c4")).And.Contain(Move("d3", "b5")).And.Contain(Move("d3", "a6"))
							  .And.Contain(Move("d3", "d4"))
							  .And.Contain(Move("d3", "e4")).And.Contain(Move("d3", "f5")).And.Contain(Move("d3", "g6")).And.Contain(Move("d3", "h7"))
							  .And.Contain(Move("d3", "e3")).And.Contain(Move("d3", "f3")).And.Contain(Move("d3", "g3")).And.Contain(Move("d3", "h3"))
							  .And.Contain(Move("d3", "d2")).And.Contain(Move("d3", "d1"), "The queen should have 17 possiable moves from the postion d3 on the board").And.HaveCount(17);
			CB.Value.Should().Be(0, "Board value should be 0 since no piece has been captured");
		}

		/// <summary>
		/// Test that will place the queen near a corner with opposing pieces
		/// producing 13 possible moves
		/// </summary>
		[Fact]
		public void QueenTest() {
			ChessBoard b = CreateBoardWithPositions(
				 Pos("b7"), ChessPieceType.Queen, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("a5"), ChessPieceType.Pawn, 1,
				 Pos("b6"), ChessPieceType.Queen, 2,
				 Pos("e8"), ChessPieceType.King, 2,
				 Pos("a8"), ChessPieceType.Pawn, 2,
				 Pos("c6"), ChessPieceType.Pawn, 2
				 );

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			GetMovesAtPosition(possMoves, Pos("b7")).Should().HaveCount(13, "there should be 13 possible moves");

		}

		[Fact]
		public void QueenPossibleMoves() {
			ChessBoard b = new ChessBoard();
			ApplyMove(b, Move("c2", "c4"));
			ApplyMove(b, Move("d7", "d6"));
			ApplyMove(b, Move("e2", "e4"));
			ApplyMove(b, Move("h7", "h5"));
			ApplyMove(b, Move("d1", "h5"));
			ApplyMove(b, Move("g7", "g5"));
			ApplyMove(b, Move("h5", "g6"));
			ApplyMove(b, Move("g8", "f6"));
			ApplyMove(b, Move("g6", "f7"));
			ApplyMove(b, Move("e8", "d7"));
			ApplyMove(b, Move("f7", "e8"));
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var queen = GetMovesAtPosition(possMoves, Pos("d8"));
			queen.Should().Contain(Move("d8", "e8"), "should contain only 1 move after applying");
			b.CurrentPlayer.Should().Be(2);


		}

		[Fact]
		public void QueenPossibleMoves2() {
			ChessBoard b = CreateBoardWithPositions(
				Pos("e1"), ChessPieceType.King, 1,
				Pos("e2"), ChessPieceType.Queen, 1);
			b.GetPossibleMoves().Should().Contain(Move("e2", "d3"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "c4"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "b5"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "a6"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "f3"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "g4"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "h5"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "d1"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "f1"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "e3"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "e8"), "Queen's valid moves should include this move.");
			b.GetPossibleMoves().Should().Contain(Move("e2", "f2"), "Queen's valid moves should include this move.");

			b = CreateBoardWithPositions(
				Pos("e1"), ChessPieceType.King, 1,
				Pos("e2"), ChessPieceType.Queen, 1,
				Pos("e7"), ChessPieceType.RookKing, 2
				);
			b.GetPossibleMoves().Should().NotContain(Move("e2", "d3")).And
				 .NotContain(Move("e2", "f3"), "Queen cannot move to a piece that will place king in check");
		}

		[Fact]
		// Test 5 - Test possible moves for the queen for the player 2
		public void BlackQueenPossibleMoves() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
				Move("d2", "d3"), //move pawn player 1
            Move("d7", "d6"), //move pawn player 2
            Move("c2", "c3"), //move pawn player 1
            Move("d8", "d7"), //move queen player 2
            Move("c3", "c4"), //move pawn player 1
            Move("d7", "c6"), //move queen player 2
				Move("h2", "h3"), //move pawn player 1
			});

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var queenBlack = GetMovesAtPosition(possMoves, Pos("c6"));


			queenBlack.Should().Contain(Move("c6", "d7")).And.Contain(Move("c6", "b6")).And.Contain(Move("c6", "a6"))
				.And.Contain(Move("c6", "c5")).And.Contain(Move("c6", "b5")).And.Contain(Move("c6", "a4")).And.Contain(Move("c6", "d5"))
				.And.Contain(Move("c6", "e4")).And.Contain(Move("c6", "f3")).And.Contain(Move("c6", "c4")).And.Contain(Move("c6", "g2"))
				.And.HaveCount(11, "because the queen only has 11 moves");
		}

		/// <summary>
		/// Undo Black Queens Capture of White Pawn with preset board
		/// </summary>
		[Fact]
		public void checkUndoQueen() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("e2", "e3"), // pawn to e3
                Move("d7", "d6"), // pawn to d6
                Move("f1", "e2"), // bishop tp e2
                Move("d8", "d7"), // queen to d7
				Move("e2", "d3"), // bishop to d3
                Move("d7", "b5"), // queen test
                Move("g2", "g3") // pawn to g3
			});
			//moves black queen to capture white bishop
			ApplyMove(b, Move("b5,d3"));

			b.Value.Should().Be(b.GetPieceValue(ChessPieceType.Bishop) * -1, "captured a white bishop"); //value should be negative since -3 for a white bishop 
			b.GetPieceAtPosition(Pos("d3")).PieceType.Should().Be(ChessPieceType.Queen, "a queen should be at this location d3");
			b.UndoLastMove();
			b.Value.Should().Be(0);
			b.GetPieceAtPosition(Pos("d3")).PieceType.Should().Be(ChessPieceType.Bishop, "a bishop should be at this location d3");
			b.GetPieceAtPosition(Pos("b5")).PieceType.Should().Be(ChessPieceType.Queen, "a queen should be at this location b5");
		}

		[Fact]
		public void QueenPossibleMoves3() {
			ChessBoard b = CreateBoardWithPositions(
			Pos("a5"), ChessPieceType.Queen, 1,
			Pos("e1"), ChessPieceType.King, 1,
			Pos("b7"), ChessPieceType.Pawn, 2,
			Pos("a4"), ChessPieceType.Queen, 2);
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var whitequeenmoves = GetMovesAtPosition(possMoves, Pos("a5"));
			whitequeenmoves.Should().HaveCount(17, "White Queen should have 17 possible moves at the start.");
		}
	}
}
