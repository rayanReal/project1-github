using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cecs475.BoardGames.Chess.Test {
	public class KingTests : ChessTests {
		[Fact]
		public void KingMovesLimited() {//For test getpossiblemoves given king piece
			ChessBoard b = CreateBoardWithPositions(
				 Pos("a4"), ChessPieceType.Pawn, 1,
				 Pos("e1"), ChessPieceType.King, 1,
				 Pos("f8"), ChessPieceType.Queen, 1,
				 Pos("f5"), ChessPieceType.Pawn, 1,
				 Pos("g6"), ChessPieceType.Bishop, 1,
				 Pos("d6"), ChessPieceType.RookKing, 1,
				 Pos("f2"), ChessPieceType.RookKing, 1,
				 Pos("e8"), ChessPieceType.Knight, 1,
				 Pos("e5"), ChessPieceType.King, 2);
			ApplyMove(b, Move("a4", "a5"));
			var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingsmove = GetMovesAtPosition(poss, Pos("e5"));
			kingsmove.Should().HaveCount(1, "King can only move one space down").And
				 .Contain(Move("e5", "e4"));
		}

		/// <summary>
		/// Showing all possible moves a white king can make
		/// </summary>        
		[Fact]
		public void CheckKingMoves() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("e2", "e3"),
					 Move("e7", "e6"),
					 Move("f1", "c4"),
					 Move("d7", "d6"),
					 Move("g1", "h3"),
					 Move("f7", "f6")
				});
			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var king = GetMovesAtPosition(possMoves, Pos("e1"));
			king.Should().Contain(Move("e1", "e2")).And.Contain(Move("e1", "f1"))
				 .And.Contain(Move("e1", "g1")).And
				 .HaveCount(3, "a king has two moves plus a castling move available");
		}

		[Fact]
		public void kingsPossibleMove() {
			ChessBoard board = CreateBoardWithPositions(
				 Pos("e7"), ChessPieceType.King, 2,
				 Pos("h5"), ChessPieceType.RookKing, 1,
				 Pos("h2"), ChessPieceType.King, 1);
			ApplyMove(board, Move("h2", "h1"));
			board.CurrentPlayer.Should().Be(2, "Black's Turn");
			//Testing king's basic move
			board.GetPossibleMoves().Should().HaveCount(8)
				 .And.Contain(Move("e7", "d8"))
				 .And.Contain(Move("e7", "e8"))
				 .And.Contain(Move("e7", "f8"))
				 .And.Contain(Move("e7", "f7"))
				 .And.Contain(Move("e7", "f6"))
				 .And.Contain(Move("e7", "e6"))
				 .And.Contain(Move("e7", "d6"))
				 .And.Contain(Move("e7", "d7"));
			ApplyMove(board, Move("e7", "e6"));
			//White-Rook move to limit Black King's move
			ApplyMove(board, Move("h5", "f5"));
			board.GetPossibleMoves().Should().HaveCount(4)
				 .And.Contain(Move("e6", "f5"))
				 .And.Contain(Move("e6", "d6"))
				 .And.Contain(Move("e6", "e7"))
				 .And.Contain(Move("e6", "d7"));
		}

		/// <summary>
		/// Validate the get possible moves for the king. 
		/// </summary>
		[Fact]
		public void KingValidation() {
			ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
					 Move("c2", "c4"),
					 Move("e7", "e5"),
					 Move("g1", "f3"),
					 Move("e8", "e7"),
					 Move("f3", "g5"),

				});

			var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			var kingMovesExpected = GetMovesAtPosition(possMoves, Pos("e7"));
			kingMovesExpected.Should().Contain(Move("e7", "e8"))
				 .And.Contain(Move("e7", "d6")).And.Contain(Move("e7", "f6"))
				 .And.HaveCount(3, "because the knight is blocking the king from moving one piece down and then can only move in 3 spaces.");

			ApplyMove(b, Move("e7", "f6"));

			//this move places black king in check
			ApplyMove(b, Move("g5", "e4"));

			possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
			kingMovesExpected = GetMovesAtPosition(possMoves, Pos("f6"));
			kingMovesExpected.Should().Contain(Move("f6", "e7"))
				 .And.Contain(Move("f6", "e6")).And.Contain(Move("f6", "g6"))
				 .And.Contain(Move("f6", "f5")).And.HaveCount(4, "because the king has to get out of the checked position");

		}


	}
}
