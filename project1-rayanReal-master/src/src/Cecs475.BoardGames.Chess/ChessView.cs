using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cecs475.BoardGames.Chess {

	public class ChessView : IGameView {
		/// <summary>
		/// This structure maps from a ChessPieceType to a character used in the view to represent the piece.
		/// </summary>
		private static readonly Dictionary<ChessPieceType, char> PIECES =
			new Dictionary<ChessPieceType, char>();

		static ChessView() {
			PIECES[ChessPieceType.Empty] = '.';
			PIECES[ChessPieceType.Bishop] = 'B';
			PIECES[ChessPieceType.King] = 'K';
			PIECES[ChessPieceType.Knight] = 'N';
			PIECES[ChessPieceType.Pawn] = 'P';
			PIECES[ChessPieceType.Queen] = 'Q';
			PIECES[ChessPieceType.RookKing] = 'R';
			PIECES[ChessPieceType.RookQueen] = 'R';
			PIECES[ChessPieceType.RookPawn] = 'R';
		}

		public string GetPlayerString(int player) {
			return player == 1 ? "White" : "Black";
		}

		/// <summary>
		/// Parses a string representing a ChessMove into a ChessMove object.
		/// </summary>
		/// <param name = "move" > a string in the format "(start, end)", where start and end use
		/// algebraic notation for board positions. In pawn promotion moves, "end" is a string name
		/// for the piece to replace the promoted pawn with, e.g., "queen", "bishop", "knight", or "rook".
		/// </param>
		public IGameMove ParseMove(string move) {
			string[] split = move.ToLower().Trim(new char[] { '(', ')' }).Split(',');
			BoardPosition start = ParsePosition(split[0].Trim());
			ChessMoveType type = ChessMoveType.Normal;

			string second = split[1].Trim();
			BoardPosition end;
			if (second == "queen") {
				end = new BoardPosition(-1, (int)ChessPieceType.Queen);
				type = ChessMoveType.PawnPromote;
			}
			else if (second == "bishop") {
				end = new BoardPosition(-1, (int)ChessPieceType.Bishop);
				type = ChessMoveType.PawnPromote;
			}
			else if (second == "knight") {
				end = new BoardPosition(-1, (int)ChessPieceType.Knight);
				type = ChessMoveType.PawnPromote;
			}
			else if (second == "rook") {
				end = new BoardPosition(-1, (int)ChessPieceType.RookPawn);
				type = ChessMoveType.PawnPromote;
			}
			else {
				end = ParsePosition(split[1].Trim());
			}
			return new ChessMove(start, end, type);
		}

		/// <summary>
		/// Parses a string representing a chess board position in algebraic notation into a 
		/// BoardPosition object.
		/// </summary>
		public BoardPosition ParsePosition(string pos) {
			return new BoardPosition(8 - (pos[1] - '0'), pos[0] - 'a');
		}

		public void PrintView(TextWriter output, IGameBoard board) {
			ChessBoard chess = board as ChessBoard;

			foreach (int i in Enumerable.Range(0, 8)) {
				output.Write("{0}  ", 8 - i);
				foreach (int j in Enumerable.Range(0, 8)) {
					var piece = chess.GetPieceAtPosition(new BoardPosition(i, j));
					char letter = PIECES[piece.PieceType];
					output.Write("{0} ", piece.Player == 1 ? letter : Char.ToLower(letter));
				}
				output.WriteLine();
			}
			output.WriteLine();
			output.WriteLine("   a b c d e f g h");
		}
	}
}
