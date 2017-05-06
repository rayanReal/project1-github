using System;
using System.Linq;

using Cecs475.BoardGames.Chess;
namespace Cecs475.BoardGames {
	class Game {
		static void Main(string[] args) {
			// Even though this project only involves chess, it is written in a way that any other implementation of
			// IGameBoard could be used in this main.
			IGameBoard board = new ChessBoard();
			IGameView view = new ChessView();

			while (true) {
				// Print the view.
				Console.WriteLine();
				Console.WriteLine();
				view.PrintView(Console.Out, board);
				Console.WriteLine();
				Console.WriteLine();

				// Print the possible moves.
				var possMoves = board.GetPossibleMoves();
				Console.WriteLine("Possible moves:");
				Console.WriteLine(String.Join(", ", possMoves));

				// Print the turn indication.
				Console.WriteLine();
				Console.Write("{0}'s turn: ", view.GetPlayerString(board.CurrentPlayer));

				string input = Console.ReadLine();
				if (input.StartsWith("move ")) {
					IGameMove move = view.ParseMove(input.Substring(5));
					bool foundMove = false;
					foreach (var poss in possMoves) {
						if (poss.Equals(move)) {
							board.ApplyMove(poss);
							foundMove = true;
							break;
						}
					}
					if (!foundMove) {
						Console.WriteLine("That is not a possible move, please try again.");
					}
				}
				else if (input.StartsWith("undo ")) {
					// Parse the number of moves to undo and repeatedly undo one move.
					int undoCount = Convert.ToInt32(input.Substring(5));
					while (undoCount > 0 && board.MoveHistory.Count > 0) {
						board.UndoLastMove();
						undoCount--;
					}
				}
				else if (input == "showHistory") {
					// Show the move history in reverse order.
					Console.WriteLine("History:");
					int player = -board.CurrentPlayer;
					foreach (var move in board.MoveHistory.Reverse()) {
						Console.WriteLine("{0}: {1}", player == 1 ? "Black" : "White", move);
						player = -player;
					}
				}
				else if (input == "showValue") {
					Console.WriteLine("Value: {0}", board.Value);
				}
			}

		}
	}
}
