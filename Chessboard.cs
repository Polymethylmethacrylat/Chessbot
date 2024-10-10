using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace fraction
{
	public class Chessboard
	{
		//0 ist ganz rechts, 63 ist ganz links, 0=a1, 63=h8
		public ulong whitePiecesBB = 0b0000000000000000000000000000000000000000000000001111111111111111;
		public ulong blackPiecesBB = 0b1111111111111111000000000000000000000000000000000000000000000000;
		public ulong bRookBB = 0b1000000100000000000000000000000000000000000000000000000000000000;
		public ulong wRookBB = 0b0000000000000000000000000000000000000000000000000000000010000001;
		public ulong bBishopBB = 0b0010010000000000000000000000000000000000000000000000000000000000;
		public ulong wBishopBB = 0b0000000000000000000000000000000000000000000000000000000000100100;
		public ulong bKnightBB = 0b0100001000000000000000000000000000000000000000000000000000000000;
		public ulong wKnightBB = 0b0000000000000000000000000000000000000000000000000000000001000010;
		public ulong wQueenBB = 0b0000000000000000000000000000000000000000000000000000000000001000;
		public ulong bQueenBB = 0b0000100000000000000000000000000000000000000000000000000000000000;
		public ulong wKingBB = 0b0000000000000000000000000000000000000000000000000000000000010000;
		public ulong bKingBB = 0b0000100000000000000000000000000000000000000000000000000000000000;
		public ulong wPawnBB = 0b0000000000000000000000000000000000000000000000001111111100000000;
		public ulong bPawnBB = 0b0000000011111111000000000000000000000000000000000000000000000000;

		public string history = "";
		public bool afterCapturePly = false;
		public int quiescenceSearchPlies = 0;

		public Chessboard() {}

		/// <summary>
		/// Hiermit kann durch FENtoPos funktionen ein board gebaut werden
		/// </summary>
		/// <param name="pieces_"></param>
		public Chessboard(Dictionary<int, Piece> pieces_)
		{
			//bitboards müssen generiert werden
			bPawnBB = Utility.getBBofPosition(pieces_, Piece.bPawn);
			wPawnBB = Utility.getBBofPosition(pieces_, Piece.wPawn);
			bBishopBB = Utility.getBBofPosition(pieces_, Piece.bBishop);
			wBishopBB = Utility.getBBofPosition(pieces_, Piece.wBishop);
			bQueenBB = Utility.getBBofPosition(pieces_, Piece.bQueen);
			wQueenBB = Utility.getBBofPosition(pieces_, Piece.wQueen);
			bKingBB = Utility.getBBofPosition(pieces_, Piece.bKing);
			wKingBB = Utility.getBBofPosition(pieces_, Piece.wKing);
			bKnightBB = Utility.getBBofPosition(pieces_, Piece.bKnight);
			wKnightBB = Utility.getBBofPosition(pieces_, Piece.wKnight);
			bRookBB = Utility.getBBofPosition(pieces_, Piece.bRook);
			wRookBB = Utility.getBBofPosition(pieces_, Piece.wRook);

			whitePiecesBB = wPawnBB | wBishopBB | wKingBB | wKnightBB | wRookBB | wQueenBB;
			blackPiecesBB = bPawnBB | bBishopBB | bKingBB | bKnightBB | bRookBB | bQueenBB;
		}

		public Chessboard(ulong wKingBB, ulong bKingBB, ulong wKnightBB, ulong bKnightBB,
			ulong wQueenBB, ulong bQueenBB, ulong wRookBB, ulong bRookBB, ulong wBishopBB, ulong bBishopBB,
			ulong wPawnBB, ulong bPawnBB, string history, bool afterCapturePly, int qsp)
		{
			this.wKingBB = wKingBB;
			this.bKingBB = bKingBB;
			this.wKnightBB = wKnightBB;
			this.bKnightBB = bKnightBB;
			this.wQueenBB = wQueenBB;
			this.bQueenBB = bQueenBB;
			this.wRookBB = wRookBB;
			this.bRookBB = bRookBB;
			this.wBishopBB = wBishopBB;
			this.bBishopBB = bBishopBB;
			this.wPawnBB = wPawnBB;
			this.bPawnBB = bPawnBB;
			this.afterCapturePly = afterCapturePly;
			quiescenceSearchPlies = qsp;

			this.whitePiecesBB = wKingBB | wKnightBB | wQueenBB | wRookBB | wBishopBB | wPawnBB;
			this.blackPiecesBB = bKingBB | bKnightBB | bQueenBB | bRookBB | bBishopBB | bPawnBB;

			this.history = history;
		}

		public static Chessboard FromFEN(string fen)
		{
			return new Chessboard(Utility.FENtoPosition(fen));
		}

		/// <summary>
		/// Returnt immer ein Piece, dh davor muss überprüft werden ob hier überhaupt ein Piece existiert 
		/// | Sehr ineffiziente Funktion
		/// </summary>
		/// <param name="posIndex"></param>
		/// <returns></returns>
		public Piece getPieceAt(int posIndex)
		{
			//kann optimiert werden mit blackPiecesBB und whitePiecesBB, 
			//aber diese funktion ist nicht dafür gedacht in performance-critical 
			//teilen des bots ausgeführt zu werden
			if (MoveSets.IsBitSet(wPawnBB, posIndex)) return Piece.wPawn;
			if (MoveSets.IsBitSet(bPawnBB, posIndex)) return Piece.bPawn;
			if (MoveSets.IsBitSet(wKingBB, posIndex)) return Piece.wKing;
			if (MoveSets.IsBitSet(bKingBB, posIndex)) return Piece.bKing;
			if (MoveSets.IsBitSet(wKnightBB, posIndex)) return Piece.wKnight;
			if (MoveSets.IsBitSet(bKnightBB, posIndex)) return Piece.bKnight;
			if (MoveSets.IsBitSet(wQueenBB, posIndex)) return Piece.wQueen;
			if (MoveSets.IsBitSet(bQueenBB, posIndex)) return Piece.bQueen;
			if (MoveSets.IsBitSet(wRookBB, posIndex)) return Piece.wRook;
			if (MoveSets.IsBitSet(bRookBB, posIndex)) return Piece.bRook;
			if (MoveSets.IsBitSet(wBishopBB, posIndex)) return Piece.wBishop;
			if (MoveSets.IsBitSet(bBishopBB, posIndex)) return Piece.bBishop;

			return 0;
		}

		public bool hasPieceAt(int posIndex)
		{
			return MoveSets.IsBitSet(whitePiecesBB | blackPiecesBB, posIndex);
		}

		/// <summary>
		/// Kann benutzt werden um die Farbe eines Pieces auf einem Sqr zu checken, Davor muss überprüft werden ob hier überhaupt ein Piece existiert !!!
		/// </summary>
		/// <returns></returns>
		public bool hasWhitePieceAt(int index)
		{
			return MoveSets.IsBitSet(whitePiecesBB, index);
		}

		/// <summary>
		/// Generiert stumpf ein Board wo das Piece von StartIndex zu EndIndex bewegt wurde
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		public Chessboard generateBoardWithMove(int startIndex, int endIndex, Piece type)
		{
			//bool isCapture = type.isWhite() ? MoveSets.IsBitSet(blackPiecesBB, endIndex) : MoveSets.IsBitSet(whitePiecesBB, endIndex);
			bool isCapture = MoveSets.IsBitSet(blackPiecesBB | whitePiecesBB, endIndex);

			if (MoveSets.IsBitSet(blackPiecesBB | whitePiecesBB, endIndex) !=
			(type.isWhite() ? MoveSets.IsBitSet(blackPiecesBB, endIndex) : MoveSets.IsBitSet(whitePiecesBB, endIndex)))
			{
				Program.DisplayBoard(this);
				Console.WriteLine(type.getSymbol() + " " + Utility.posToAN(startIndex) + " -> " + Utility.posToAN(endIndex));
				Utility.printBitBoard(MoveSets.getPseudoLegalMoves_bb(this, startIndex, out type));
				Utility.printBitBoard(whitePiecesBB | blackPiecesBB);
				Environment.Exit(0);
			}

			ulong wKingBB_ = Utility.setBBtoNullAt(wKingBB, endIndex);
			ulong bKingBB_ = Utility.setBBtoNullAt(bKingBB, endIndex);
			ulong wKnightBB_ = Utility.setBBtoNullAt(wKnightBB, endIndex);
			ulong bKnightBB_ = Utility.setBBtoNullAt(bKnightBB, endIndex);
			ulong wQueenBB_ = Utility.setBBtoNullAt(wQueenBB, endIndex);
			ulong bQueenBB_ = Utility.setBBtoNullAt(bQueenBB, endIndex);
			ulong wRookBB_ = Utility.setBBtoNullAt(wRookBB, endIndex);
			ulong bRookBB_ = Utility.setBBtoNullAt(bRookBB, endIndex);
			ulong wBishopBB_ = Utility.setBBtoNullAt(wBishopBB, endIndex);
			ulong bBishopBB_ = Utility.setBBtoNullAt(bBishopBB, endIndex);
			ulong wPawnBB_ = Utility.setBBtoNullAt(wPawnBB, endIndex);
			ulong bPawnBB_ = Utility.setBBtoNullAt(bPawnBB, endIndex);


			//alle bitboards müssen geupdated werden
			switch (type)
			{
				case Piece.wPawn:
					wPawnBB_ = Utility.updateBB(wPawnBB, startIndex, endIndex);
					//auto queen
					if (endIndex > 55)
					{
						wPawnBB_ = Utility.setBBtoNullAt(wPawnBB_, endIndex);
						wQueenBB_ += 1ul << endIndex;
					}
					break;

				case Piece.bPawn:
					bPawnBB_ = Utility.updateBB(bPawnBB, startIndex, endIndex);

					if (endIndex < 8)
					{
						bPawnBB_ = Utility.setBBtoNullAt(bPawnBB_, endIndex);
						bQueenBB_ += 1ul << endIndex;
					}
					break;

				//funktioniert weil es nur einen king geben darf
				case Piece.wKing:
					wKingBB_ = 1ul << endIndex;
					break;

				case Piece.bKing:
					bKingBB_ = 1ul << endIndex;
					break;

				case Piece.wKnight:
					wKnightBB_ = Utility.updateBB(wKnightBB, startIndex, endIndex);
					break;

				case Piece.bKnight:
					bKnightBB_ = Utility.updateBB(bKnightBB, startIndex, endIndex);
					break;

				case Piece.wQueen:
					wQueenBB_ = Utility.updateBB(wQueenBB, startIndex, endIndex);
					break;

				case Piece.bQueen:
					bQueenBB_ = Utility.updateBB(bQueenBB, startIndex, endIndex);
					break;

				case Piece.wRook:
					wRookBB_ = Utility.updateBB(wRookBB, startIndex, endIndex);
					break;

				case Piece.bRook:
					bRookBB_ = Utility.updateBB(bRookBB, startIndex, endIndex);
					break;

				case Piece.wBishop:
					wBishopBB_ = Utility.updateBB(wBishopBB, startIndex, endIndex);
					break;

				case Piece.bBishop:
					bBishopBB_ = Utility.updateBB(bBishopBB, startIndex, endIndex);
					break;
			}


			return new Chessboard(wKingBB_, bKingBB_, wKnightBB_, bKnightBB_,
				wQueenBB_, bQueenBB_, wRookBB_, bRookBB_, wBishopBB_, bBishopBB_,
				wPawnBB_, bPawnBB_, history   /* + "; " + type.getSymbol() +" "+ Utility.posToAN(startIndex) + " -> " + Utility.posToAN(endIndex)  */
				, isCapture, quiescenceSearchPlies);
		}
	}

}

