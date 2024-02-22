using System;
using System.Collections.Generic;
using fraction;

namespace fraction
{
    static class Eval
    {
        //31 mio iterations /second, dh kein bottleneck
        public static float basicStaticEval(Chessboard b)
        {
            float white = NumberOfSetBits(b.wKingBB) * 10000f + NumberOfSetBits(b.wRookBB) * 5f + NumberOfSetBits(b.wBishopBB) * 3f + NumberOfSetBits(b.wKnightBB) * 2.8f + NumberOfSetBits(b.wQueenBB) * 9f + NumberOfSetBits(b.wPawnBB);
            float black = NumberOfSetBits(b.bKingBB) * 10000f + NumberOfSetBits(b.bRookBB) * 5f + NumberOfSetBits(b.bBishopBB) * 3f + NumberOfSetBits(b.bKnightBB) * 2.8f + NumberOfSetBits(b.bQueenBB) * 9f + NumberOfSetBits(b.bPawnBB);

            white += relativeValue(b.wPawnBB, Piece.wPawn) + relativeValue(b.wRookBB, Piece.wRook) + relativeValue(b.wBishopBB, Piece.wBishop) + relativeValue(b.wKingBB, Piece.wKing) + relativeValue(b.wKnightBB, Piece.wKnight) + relativeValue(b.wQueenBB, Piece.wQueen);
            black += relativeValue(b.bPawnBB, Piece.bPawn) + relativeValue(b.bRookBB, Piece.bRook) + relativeValue(b.bBishopBB, Piece.bBishop) + relativeValue(b.bKingBB, Piece.bKing) + relativeValue(b.bKnightBB, Piece.bKnight) + relativeValue(b.bQueenBB, Piece.bQueen);

            return white - black;
        }

        public static int NumberOfSetBits(ulong i)
        {
            return (int)System.Runtime.Intrinsics.X86.Popcnt.X64.PopCount(i);
        }

        private static Dictionary<Piece, ulong> pieceMasks1 = new Dictionary<Piece, ulong>{
            {Piece.wRook,  0b0000000011111111000000000000000000000000000000000000000000000000},
            {Piece.bRook,  0b0000000000000000000000000000000000000000000000001111111100000000},
            {Piece.wKnight,0b0000000000011000001111000011110000111100001111000001100000000000},
            {Piece.bKnight,0b0000000000011000001111000011110000111100001111000001100000000000},
            {Piece.wPawn,0b1111111111111111111111111111111100000000000000000000000000000000},
            {Piece.bPawn,0b11111111111111111111111111111111},
            {Piece.wBishop,0b0000000000011000001111000011110000111100001111000001100000000000},
            {Piece.bBishop,0b0000000000011000001111000011110000111100001111000001100000000000},
            {Piece.wQueen,  0b1111111100000000000000000000000000000000000000000011110000000000},
            {Piece.bQueen,  0b0000000000111100000000000000000000000000000000000000000011111111},
            {Piece.wKing,  0b0000000000000000000000000000000000000000000000000000000011100111},
            {Piece.bKing,  0b1110011100000000000000000000000000000000000000000000000000000000},
        };

        private static Dictionary<Piece, ulong> pieceMasks2 = new Dictionary<Piece, ulong>{
            {Piece.wPawn,0b111111111111111100000000000000000000000000000000},
            {Piece.bPawn,0b1111111111111111},
        };

        private static Dictionary<Piece, float> pieceFightValue = new Dictionary<Piece, float>{
            {Piece.wPawn,1f},{Piece.bPawn,1f},
            {Piece.wKnight,2.7f},{Piece.bKnight,2.7f},
            {Piece.wQueen,9f},{Piece.bQueen,9f},
            {Piece.wRook,5f},{Piece.bRook,5f},
            {Piece.wBishop,3f},{Piece.bBishop,3f},
            {Piece.wKing,2f},{Piece.bKing,2f},
        };



        static float relativeValue(ulong bb, Piece type)
        {
            float value = NumberOfSetBits(bb & pieceMasks1[type]) * pieceFightValue[type] * 0.1f;

            if (type == Piece.wPawn || type == Piece.bPawn) value += NumberOfSetBits(bb & pieceMasks2[type]) * pieceFightValue[type] * 0.1f;

            return value;
        }
    }
}

