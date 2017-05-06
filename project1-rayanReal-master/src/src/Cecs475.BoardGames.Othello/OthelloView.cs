using System;
using System.IO;

namespace Cecs475.BoardGames.Othello {
	/// <summary>
	/// Represents a console text-based view of an othello game.
	/// </summary>
	public class OthelloView : IGameView {
		public string GetPlayerString(int player) {
			return player == 1 ? "Black" : "White";
		}

		/// <summary>
		/// Parses a string representation of an OthelloMove in the format "(r, c)".
		/// </summary>
		public IGameMove ParseMove(string move) {
			string[] split = move.Trim(new char[] { '(', ')' }).Split(',');
			return new OthelloMove(new BoardPosition(Convert.ToInt32(split[0]), Convert.ToInt32(split[1])));
		}

		/// <summary>
		/// Prints a text representation of an OthelloBoard to the given TextWriter.
		/// </summary>
		public void PrintView(TextWriter output, IGameBoard b) {
			OthelloBoard board = b as OthelloBoard;
			output.WriteLine("- 0 1 2 3 4 5 6 7");
			for (int i = 0; i < OthelloBoard.BOARD_SIZE; i++) {
				output.Write("{0} ", i);
				for (int j = 0; j < OthelloBoard.BOARD_SIZE; j++) {
					sbyte space = board.Board[i, j];
					output.Write("{0} ", space == 0 ? '.' : space == 1 ? 'B' : 'W');
				}
				output.WriteLine();
			}
		}


	}
}
