using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using API.Logic;
using static API.Utils.ChessHelpers;
namespace API.Utils
{
    public static class BitbboardUtils
    {

        public const int a8 = 0, b8 = 1, c8 = 2, d8 = 3, e8 = 4, f8 = 5, g8 = 6, h8 = 7;
        public const int a7 = 8, b7 = 9, c7 = 10, d7 = 11, e7 = 12, f7 = 13, g7 = 14, h7 = 15;
        public const int a6 = 16, b6 = 17, c6 = 18, d6 = 19, e6 = 20, f6 = 21, g6 = 22, h6 = 23;
        public const int a5 = 24, b5 = 25, c5 = 26, d5 = 27, e5 = 28, f5 = 29, g5 = 30, h5 = 31;
        public const int a4 = 32, b4 = 33, c4 = 34, d4 = 35, e4 = 36, f4 = 37, g4 = 38, h4 = 39;
        public const int a3 = 40, b3 = 41, c3 = 42, d3 = 43, e3 = 44, f3 = 45, g3 = 46, h3 = 47;
        public const int a2 = 48, b2 = 49, c2 = 50, d2 = 51, e2 = 52, f2 = 53, g2 = 54, h2 = 55;
        public const int a1 = 56, b1 = 57, c1 = 58, d1 = 59, e1 = 60, f1 = 61, g1 = 62, h1 = 63;
        public const int None = 64;
        public const int White = 0, Black = 1, Both = 2;
        public const int WK = 1, WQ = 2, BK = 4, BQ = 8;
        public const int P = 0, K = 1, B = 2, R = 3, Q = 4, N = 5, p = 6, k = 7, b = 8, r = 9, q = 10, n = 11;
        public const int AllMoves = 0, OnlyCaptures = 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref ulong bitboard, int index)
        {
            bitboard |= 1UL << index;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearBit(ref ulong bitboard, int index)
        {
            bitboard &= ~(1UL << index);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLSBIndex(ulong bitboard)
        {
            return BitOperations.TrailingZeroCount(bitboard);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ulong bitboard, int index)
        {
            return (bitboard & (1UL << index)) != 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EncodeMove(int source, int target, int piece, int promotion, int pieceCaptured, bool isEnpassant, bool isCastle, bool isCapture, bool isDoublePawnPush)
        {
            int move = 0;
            move |= source;
            move |= target << 6;
            move |= piece << 12;
            move |= promotion << 16;
            move |= (isCapture ? 1 : 0) << 20;
            move |= (isDoublePawnPush ? 1 : 0) << 21;
            move |= (isEnpassant ? 1 : 0) << 22;
            move |= (isCastle ? 1 : 0) << 23;
            move |= pieceCaptured << 24;
            return move;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveCaptured(int move) { return ((move & 0x0F000000) >> 24); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveSource(int move) { return move & 0x3f; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveTarget(int move) { return (move & 0xfc0) >> 6; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMovePiece(int move) { return ((move & 0xf000) >> 12); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMovePromoted(int move) { return ((move & 0xf0000) >> 16); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoveCapture(int move) { return (move & 0x100000) != 0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoveDoublePawnPush(int move) { return (move & 0x200000) != 0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoveEnpassant(int move) { return (move & 0x400000) != 0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoveCastle(int move) { return (move & 0x800000) != 0; }
        public static int GetRank(int square) { return square / 8; }
        public static int GetFile(int square) { return square % 8; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EncodePin(int pinnedSquare, int pinningSquare)
        {
            return (pinningSquare << 6) | pinnedSquare;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DecodePin(int pinCode, out int pinnedSquare, out int pinningSquare)
        {
            pinnedSquare = pinCode & 0x3F;
            pinningSquare = (pinCode & 0xfc0) >> 6;
        }
        public static void PrintMoves(int[] moves)
        {
            Console.WriteLine("Move    piece double enpassant castle capture");
            foreach (int move in moves)
            {
                string promotion = GetMovePromoted(move) != 0 ? PieceToChar[GetMovePromoted(move)].ToString() : "";
                if (move != 0)
                    Console.WriteLine($"{Coordinates[GetMoveSource(move)]}{Coordinates[GetMoveTarget(move)]}{promotion}      {AsciiPiece[GetMovePiece(move)]}    {IsMoveDoublePawnPush(move)}   {IsMoveEnpassant(move)}    {IsMoveCastle(move)}  {IsMoveCapture(move)}");
            }
        }
        public static string MoveToString(int move)
        {
            string promotion = GetMovePromoted(move) != 0 ? PieceToChar[GetMovePromoted(move)].ToString() : "";
            return $"{Coordinates[GetMoveSource(move)]}{Coordinates[GetMoveTarget(move)]}{promotion}";
        }
        public static void PrintBoard(Board board)
        {
            char[][] boardRepresentation = new char[8][];
            for (int i = 0; i < 8; i++)
            {
                boardRepresentation[i] = new char[8];
                for (int j = 0; j < 8; j++)
                {
                    boardRepresentation[i][j] = '.';
                }
            }

            for (int pieceType = 0; pieceType < board.Bitboards.Length; pieceType++)
            {
                ulong bitboard = board.Bitboards[pieceType];
                for (int squareIndex = 0; squareIndex < 64; squareIndex++)
                {
                    ulong squareBit = 1UL << squareIndex;
                    if ((bitboard & squareBit) != 0)
                    {

                        int row = 7 - (squareIndex / 8);
                        int col = squareIndex % 8;


                        int piece = pieceType;
                        boardRepresentation[row][col] = Unicodes[piece];
                    }
                }
            }
            Console.Write("  ");
            for (char col = 'a'; col <= 'h'; col++)
            {
                Console.Write(col + " ");
            }
            Console.WriteLine();
            for (int i = 0; i < 8; i++)
            {

                Console.Write((8 - i) + " ");

                for (int j = 0; j < 8; j++)
                {
                    Console.Write(boardRepresentation[i][j] + " ");
                }
                Console.Write((8 - i) + " ");

                Console.WriteLine();
            }
            Console.Write("  ");
            for (char col = 'a'; col <= 'h'; col++)
            {
                Console.Write(col + " ");
            }
            Console.WriteLine();

        }
        public static void PrintBitboard(ulong bitboard)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    int square = rank * 8 + file;
                    ulong mask = 1UL << square;
                    if ((bitboard & mask) != 0)
                    {
                        Console.Write("1 ");
                    }
                    else
                    {
                        Console.Write("0 ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();

        }


    }
}