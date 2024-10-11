using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime;
using System.Security.Cryptography;

namespace fraction
{
	static class Utility
	{
		/// <summary>
		/// Konvertiert einen FEN-String zu einer Position die einem Board gegeben werden kann
		/// </summary>
		/// <param name="fen"></param>
		/// <returns></returns>
		public static Dictionary<int, Piece> FENtoPosition(string fen)
		{
			Dictionary<int, Piece> pos = new Dictionary<int, Piece> { };
			int currPos = 0;

			foreach (char c in fen)
			{
				switch (c)
				{
					case 'r':
						pos[convert(currPos)] = Piece.bRook;
						break;
					case 'R':
						pos[convert(currPos)] = Piece.wRook;
						break;
					case 'n':
						pos[convert(currPos)] = Piece.bKnight;
						break;
					case 'N':
						pos[convert(currPos)] = Piece.wKnight;
						break;
					case 'b':
						pos[convert(currPos)] = Piece.bBishop;
						break;
					case 'B':
						pos[convert(currPos)] = Piece.wBishop;
						break;
					case 'k':
						pos[convert(currPos)] = Piece.bKing;
						break;
					case 'K':
						pos[convert(currPos)] = Piece.wKing;
						break;
					case 'q':
						pos[convert(currPos)] = Piece.bQueen;
						break;
					case 'Q':
						pos[convert(currPos)] = Piece.wQueen;
						break;
					case 'p':
						pos[convert(currPos)] = Piece.bPawn;
						break;
					case 'P':
						pos[convert(currPos)] = Piece.wPawn;
						break;
					case '/':
						currPos--;
						break;

					default://c ist eine zahl
						currPos += c - '0' - 1;
						break;
				}

				currPos++;
			}

			return pos;
		}


		/// <summary>
		/// Konvertiert einen index auf dem board zu einem weirden FEN-Index (0=a8, 63=h1)
		/// </summary>
		/// <returns></returns>
		private static int convert(int i)
		{
			return (i % 8) * 2 + 56 - i;
		}

		/// <summary>
		/// Generiert den FEN string zu einer entsprechenden Position, "positionToFEN()"
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public static string BoardToFEN(Dictionary<int, Piece> pos)
		{
			string fen = "";
			int currNum = 0;

			for (int i = 0; i < 64; i++)
			{
				//das sqr mit dem FEN-Index das wir abrufen wollen, muss konvertiert wegen der komischen FEN-Konvention
				int sqr = convert(i);

				//wenn ein linebreak gehittet wurde
				if (i % 8 == 0 && i > 0)
				{
					if (currNum > 0) fen += currNum;
					fen += "/";
					currNum = 0;
				}

				Piece piece;
				if (pos.TryGetValue(sqr, out piece))
				{
					//das piece an den fen-string appenden
					if (currNum > 0) fen += currNum;
					fen += piece.getSymbol();
					currNum = 0;
				}
				else
				{
					currNum++;
				}

			}

			if (currNum > 0) fen += currNum;
			return fen;
		}



		/// <summary>
		/// Konvertiert einen index (0-63) zu einem 2er array mit x,y (0-7)
		/// </summary>
		/// <param name="index">Der index des Pieces in einem 64er array</param>
		/// <returns></returns>
		public static int[] IndexToPos(int index)
		{
			//y * 8 + x = index

			int y = index >> 3;
			int x = index & 7;
			return new int[] { x, y };
		}

		/// <summary>
		/// Konvertiert ein 2er array mit x,y zu einem index in einem 64er array
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public static int PosToIndex(int[] pos)
		{
			return pos[1] * 8 + pos[0];
		}

		public static int PosToIndex(int i1, int i2)
		{
			return 8 * i2 + i1;
		}


		public static void printPieces(Dictionary<int, Piece> pos)
		{
			for (int i = 0; i < 64; i++)
			{
				Piece currPiece;
				if (pos.TryGetValue(i, out currPiece))//wenn hier ein piece existiert
				{
					Console.Write(currPiece + " , ");
				}

			}

			Console.Write("\n");
		}

		public static Piece SymbolToPiece(string symbol)
		{
			switch (symbol)
			{
				case "r":
					return Piece.bRook;
				case "n":
					return Piece.bKnight;
				case "b":
					return Piece.bBishop;
				case "q":
					return Piece.bQueen;
				case "k":
					return Piece.bKing;
				case "p":
					return Piece.bPawn;

				case "R":
					return Piece.wRook;
				case "N":
					return Piece.wKnight;
				case "B":
					return Piece.wBishop;
				case "Q":
					return Piece.wQueen;
				case "K":
					return Piece.wKing;
				case "P":
					return Piece.wPawn;

				default:
					Console.WriteLine("Something went terribly wrong in (Utility.SymbolToPiece(...)");
					return Piece.bKing;
			}


		}


		/// <summary>
		/// position zu Algebraic Notation; hat bis jetzt nur den zweck einer leichteren visualisierung
		/// </summary>
		/// <returns></returns>
		public static string posToAN(int[] pos)
		{
			return "abcdefgh"[pos[0]].ToString() + pos[1];//funktionert, frag nicht wieso
		}

		public static string posToAN(int pos)
		{
			return "abcdefgh"[pos & 7].ToString() + ((pos >> 3) + 1);
			//das sieht echt bodenlos aus, aber es klappt
		}

		/// <summary>
		/// Algebraic Notation -> position als int
		/// </summary>
		/// <param name="AN"></param>
		/// <returns></returns>
		public static int ANtoPos(string AN)
		{
			int x = "abcdefgh".IndexOf(AN[0]);
			int y = "12345678".IndexOf(AN[1]);

			return 8 * y + x;
		}

		/// <summary>
		/// Addiert 2 Positionen in Koordinatenform
		/// </summary>
		/// <returns></returns>
		public static int[] Sum(int[] a, int[] b)
		{
			return new int[2] { a[0] + b[0], a[1] + b[1] };
		}

		/// <summary>
		/// Generiert aus einem array mit positionsIndizes ein bitboard
		/// </summary>
		/// <param name="sqrs"></param>
		/// <returns></returns>
		public static ulong sqrArrToBB(int[] sqrs)
		{
			ulong bb = 0ul;
			foreach (int sqr in sqrs)
			{
				if (sqr >= 0) bb += 1ul << sqr;//ul ist notwendig da die 1 ansonsten zu einer int konvertiert wird
			}

			return bb;
		}


		//notwendig für bitmanipulations in MoveSets.cs, sollte funktionen
		//Ich verstehe nicht was hier passiert, aber es funktioniert bis jetzt einwandfrei
		//https://stackoverflow.com/questions/31374628/fast-way-of-finding-most-and-least-significant-bit-set-in-a-64-bit-integer
		private const ulong Magic = 0x37E84A99DAE458F;//was ist das

		private static readonly int[] MagicTable =
		{
		0, 1, 17, 2, 18, 50, 3, 57,
		47, 19, 22, 51, 29, 4, 33, 58,
		15, 48, 20, 27, 25, 23, 52, 41,
		54, 30, 38, 5, 43, 34, 59, 8,
		63, 16, 49, 56, 46, 21, 28, 32,
		14, 26, 24, 40, 53, 37, 42, 7,
		62, 55, 45, 31, 13, 39, 36, 6,
		61, 44, 12, 35, 60, 11, 10, 9,
		};

		//Werden nur von importierten Funktionen verwendet
		public static int BitScanForward(ulong b)
		{
			return MagicTable[((ulong)((long)b & -(long)b) * Magic) >> 58]; //digga was
		}

		public static int BitScanReverse(ulong b)
		{
			b |= b >> 1;
			b |= b >> 2;
			b |= b >> 4;
			b |= b >> 8;
			b |= b >> 16;
			b |= b >> 32;
			b = b & ~(b >> 1); //wtf
			return MagicTable[b * Magic >> 58];
		}

		public static void printBitBoard(ulong bb, int specialIndex = -1)
		{
			byte[] bytes = BitConverter.GetBytes(bb);
			int[] pos = specialIndex >= 0 ? Utility.IndexToPos(specialIndex) : new int[2];

			for (int i = 0; i < 8; i++)
			{
				byte x = bytes[7 - i];
				string s = Convert.ToString(x, 2);
				if (x < 128) s = "0" + s;
				if (x < 64) s = "0" + s;
				if (x < 32) s = "0" + s;
				if (x < 16) s = "0" + s;
				if (x < 8) s = "0" + s;
				if (x < 4) s = "0" + s;
				if (x < 2) s = "0" + s;

				if (specialIndex >= 0 && 7 - i == pos[1])
				{
					int a = 7 - pos[0];
					s = s.Substring(0, a) + "X" + s.Substring(a + 1, 7 - a);
				}

				Console.WriteLine(reverse(s));
			}

			Console.WriteLine(" ");
		}

		//https://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
		public static string reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}

		public static void printIntArr(int[] sqrs)
		{
			string str = "";
			for (int i = 0; i < sqrs.Length; i++)
			{
				str += sqrs[i] + ", ";
			}

			Console.WriteLine(str);
		}

		/// <summary>
		/// Generiert das Bitboard einer pieceArt aus der positionsdarstellung
		/// </summary>
		/// <param name="pieces"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public static ulong getBBofPosition(Dictionary<int, Piece> pieces, Piece p)
		{
			ulong retBB = 0;
			for (int i = 0; i < 64; i++)
			{
				Piece currPiece;
				if (pieces.TryGetValue(i, out currPiece))
				{
					if (currPiece == p)
					{
						retBB |= 1ul << i;
					}
				}
			}

			return retBB;
		}


		public static ulong setBBtoNullAt(ulong bb, int index)
		{
			return bb & ~(1ul << index);
		}

		//noch nicht getestet, könnte fehlerquelle sein
		//index 1 wird auf null gesetzt, index 2 auf 1
		public static ulong updateBB(ulong bb, int i1, int i2)
		{
			return setBBtoNullAt(bb, i1) + (1ul << i2);
		}

		public static ulong updateBBFast(ulong bb, int i1, int i2)
		{
			return bb ^ (ulong)(i1 | i2);
		}

		public static void DisplayAllBoards(Chessboard[][] b)
		{
			foreach (var b1 in b)
			{
				if (b1 == null)
				{
					continue;
				}
				foreach (var b2 in b1)
				{
					Program.DisplayBoard(b2);
				}
			}
		}


		public static List<int> FindSetBits(ulong bb)
		{
			List<int> setBits = new List<int>();

			while (bb != 0)
			{
				int index = BitScanForward(bb);
				setBits.Add(index);

				// Clear the least significant set bit
				bb &= bb - 1;
			}

			return setBits;
		}

		/// <summary>
		/// wenn bekannt ist dass maximal n bits gesetzt sind
		/// |Doppelt so schnell wie die Methode von ChatGPT
		/// </summary>
		/// <param name="bb"></param>
		/// <returns></returns>
		public static int[] FindSetBitsMax(ulong bb, int max)
		{
			int[] setBits = new int[max];
			int i = 0;

			while (bb != 0)
			{
				int index = BitScanForward(bb);
				setBits[i] = index;
				i++;

				// Clear the least significant set bit
				bb &= bb - 1;
			}

			return setBits;
		}

		public static void FindTwoSetBits(ulong bb, out int index1, out int index2)
		{
			index1 = BitScanForward(bb);
			bb &= bb - 1; // Clear the least significant set bit
			index2 = BitScanForward(bb);
		}

		public static int FindSingleSetBit(ulong bb)
		{
			return BitScanForward(bb);
		}
	}
}


