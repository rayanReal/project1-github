using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class CheckmateTests : ChessTests {

		[Fact]
		public void CheckmateKing() {//To test putting a king in check
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("f2", "f4"),
					 Move("e7", "e6"),
					 Move("g2", "g4"),
					 Move("d8", "h4")
				});
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingsmove = GetMovesAtPosition(poss, Pos("e1"));
			kingsmove.Should().HaveCount(0, "The king is in checkmate and can't make any moves");
			b.IsCheckmate.Should().BeTrue();
		}

		[Fact]
		public void CheckMate() {
			ChessBoard board = new ChessBoard();
			//White Pawn
			ApplyMove(board, Move("e2", "e4"));
			//Black Pawn
			ApplyMove(board, Move("e7", "e5"));
			//White Bishop
			ApplyMove(board, Move("f1", "c4"));
			//Black Bishop
			ApplyMove(board, Move("f8", "d6"));
			//White Queen
			ApplyMove(board, Move("d1", "f3"));
			//Black Pawn
			ApplyMove(board, Move("h7", "h5"));
			//White Queen to call Checkmate
			ApplyMove(board, Move("f3", "f7"));
			board.IsCheckmate.Should().Be(true, "White Queen, supported by Bishop, checkmates Black King");
			board.GetPossibleMoves().Should().HaveCount(0, "No possible move when checkmate");
		}

		/// <summary>
		/// Tests if checkmate detection is working correctly.
		/// </summary>
		[Fact]
		public void CheckMateWhite() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[]
			{
					 Move ("e2", "e4"),
					 Move ("e7", "e5"),
					 Move ("f1", "c4"),
					 Move ("b8", "c6"),
					 Move ("d1", "h5"),
					 Move ("d7", "d6")
			});

			b.CurrentPlayer.Should().Be(1, "Current player should be White.");

			ApplyMove(b, Move("h5", "f7")); // Checkmate (White's doing)!

			b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

			var possMoves = b.GetPossibleMoves();

			possMoves.Count().Should().Be(0); // There should be no available moves!

			b.IsCheck.Should().Be(false, "Should not be checked since it's already checkmate.");
			b.IsCheckmate.Should().Be(true, "Should be checkmate.");
			b.IsStalemate.Should().Be(false, "Not a stalemate");
		}

		[Fact]
		public void TestUndoCheckKing() {//Test undoing moves for chessboard.
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("f2", "f4"),
					 Move("e7", "e6"),
					 Move("g2", "g4"),
					 Move("d8", "h4")
				});
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingsmove = GetMovesAtPosition(poss, Pos("e1"));
			b.IsCheckmate.Should().Be(true, "The king is in checkmate");
			b.UndoLastMove();
			b.IsCheckmate.Should().Be(false, "King is now not in checkmate");
		}

		/// <summary>
		/// Makes move of black queen and checks at white king for moves
		/// </summary>
		[Fact]
		public void CheckMate2() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("f2", "f4"),
					 Move("e7", "e6"),
					 Move("g2", "g4")
				});
			
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			ApplyMove(b, Move("d8", "h4"));

			var realMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var king = GetMovesAtPosition(realMoves, Pos("e1"));
			king.Should().HaveCount(0, "the king is at checkmate");
			b.GetPossibleMoves().Should().BeEmpty();
			b.IsCheckmate.Should().BeTrue("king is in checkmate");
		}
	}
}
