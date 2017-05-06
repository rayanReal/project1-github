using System.IO;

namespace Cecs475.BoardGames {
	/// <summary>
	/// Represents a console text-based view of a game.
	/// </summary>
	public interface IGameView {
		/// <summary>
		/// Prints a text representation of the given board to the given TextWriter output.
		/// </summary>
		void PrintView(TextWriter output, IGameBoard board);

		/// <summary>
		/// Parses a string representation of a game move.
		/// </summary>
		IGameMove ParseMove(string input);

		/// <summary>
		/// Gets a string representing the given player appropriate to the particular game.
		/// </summary>
		string GetPlayerString(int player);
	}
}
