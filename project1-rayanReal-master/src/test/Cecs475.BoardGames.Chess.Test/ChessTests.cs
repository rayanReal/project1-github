using System.Linq;
using System.Collections.Generic;
using System;
using Xunit.Abstractions;
using FluentAssertions;
using System.IO;
using System.Text;

namespace Cecs475.BoardGames.Chess.Test {
	/// <summary>
	/// This partial class implementation includes many utility methods to make writing tests easier. 
	/// </summary>
	public class ChessTests {
		// For parsing moves and positions.
		protected static ChessView BlankView = new ChessView();

		/// <summary>
		/// Creates a ChessMove from a string in the same format as ChessView.ParseMove expects.
		/// </summary>
		protected static ChessMove Move(string moveString) {
			return BlankView.ParseMove(moveString) as ChessMove;
		}

		/// <summary>
		/// Creates a ChessMove from two strings reprenting the start and end chess coordinates.
		/// </summary>
		protected static ChessMove Move(string start, string end) {
			return new ChessMove(Pos(start), Pos(end));
		}

		/// <summary>
		/// Creates a BoardPosition from a row integer and column integer, where (0,0) is chess square a8.
		/// </summary>
		protected static BoardPosition Pos(int row, int col) {
			return new BoardPosition(row, col);
		}

		/// <summary>
		/// Creates a BoardPosition from a string representing a chess coordinate in algebraic notation.
		/// </summary>
		protected static BoardPosition Pos(string position) {
			return BlankView.ParsePosition(position);
		}

		/// <summary>
		/// Creates a new ChessBoard by applying the given sequence of moves to a starting board.
		/// </summary>
		protected static ChessBoard CreateBoardFromMoves(IEnumerable<ChessMove> moves) {
			ChessBoard b = new ChessBoard();
			ApplyMovesToBoard(b, moves);
			return b;
		}

		/// <summary>
		/// Creates a new ChessBoard by manually specifying which pieces are at which positions. 
		/// </summary>
		/// <param name="positions">must provide 3 values for each piece: a BoardPosition, a ChessPieceType, and an 
		/// int player, in that order.</param>
		/// <returns></returns>
		protected static ChessBoard CreateBoardWithPositions(params object[] positions) {
			var p = new List<Tuple<BoardPosition, ChessPiecePosition>>();
			for (int i = 0; i < positions.Length; i += 3) {
				p.Add(Tuple.Create((BoardPosition)positions[i],
					new ChessPiecePosition(
						(ChessPieceType)positions[i + 1],
						(int)positions[i + 2])));
			}
			return new ChessBoard(p);
		}

		/// <summary>
		/// Applies the given move to the given board.
		/// </summary>
		protected static void ApplyMove(ChessBoard board, ChessMove move) {
			int currentValue = board.Value;
			var possMoves = board.GetPossibleMoves();
			board.Value.Should().Be(currentValue, "the board's value should not change after calling GetPossibleMoves");
			var toApply = possMoves.FirstOrDefault(m => m.Equals(move));
			if (toApply == null) {
				throw new InvalidOperationException("Could not apply the move " + move + " to the board\n" +
					ToString(board));
			}
			else {
				board.ApplyMove(toApply);
			}
		}

		/// <summary>
		/// Applies the given sequence of moves to the given board.
		/// </summary>
		protected static void ApplyMovesToBoard(ChessBoard board, IEnumerable<ChessMove> moves) {
			foreach (var move in moves) {
				ApplyMove(board, move);
			}
		}

		/// <summary>
		/// Returns all moves from the given sequence that start at the given position.
		/// </summary>
		protected static IEnumerable<ChessMove> GetMovesAtPosition(IEnumerable<ChessMove> moves, BoardPosition pos) {
			return moves.Where(m => m.StartPosition.Equals(pos));
		}

		/// <summary>
		/// Returns all chess piece positions controlled by the given player
		/// </summary>
		protected static IEnumerable<ChessPiecePosition> GetAllPiecesForPlayer(ChessBoard b, int player) {
			return
				from pos in (
					from row in Enumerable.Range(0, 8)
					from col in Enumerable.Range(0, 8)
					select b.GetPieceAtPosition(new BoardPosition(row, col))
				)
				where pos.Player == player
				select pos;

		}

		protected static string ToString(ChessBoard b) {
			using (MemoryStream s = new MemoryStream()) {
				StreamWriter writer = new StreamWriter(s, Encoding.Unicode);
				writer.WriteLine();
				writer.WriteLine("Board: ");
				BlankView.PrintView(writer, b);
				writer.WriteLine(BlankView.GetPlayerString(b.CurrentPlayer) + "'s turn");
				writer.Flush();
				var ret = Encoding.Unicode.GetString(s.ToArray());
				return ret;
			}
		}
	}
}