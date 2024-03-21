using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using API.Utils;
using static API.Utils.BitbboardUtils;
namespace API.Logic
{
    public static class Attacks
    {

        public static readonly ulong[][] BishopLookup = new ulong[64][];
        public static readonly ulong[][] RookLookup = new ulong[64][];
        static Attacks()
        {
            InitializeBishopLookupTable();
            InitializeRookLookupTable();
        }
        public static void InitializeRookLookupTable()
        {
            for (int i = 0; i < 64; i++)
            {

                ulong mask = Mask.RookMasks[i];
                int bitCount = BitOperations.PopCount(mask);
                RookLookup[i] = new ulong[(int)Math.Pow(2, bitCount)];
                int combinations = 1 << bitCount;
                for (int j = 0; j < combinations; j++)
                {
                    ulong blockers = GenerateBlockers(j, mask);
                    ulong attacks = 0;
                    int rank = GetRank(i), r;
                    int file = GetFile(i), f;
                    //up
                    for (r = rank + 1, f = file; r <= 7; r++)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }
                    //down
                    for (r = rank - 1, f = file; r >= 0; r--)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }
                    //right
                    for (r = rank, f = file + 1; f <= 7; f++)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }
                    //left
                    for (r = rank, f = file - 1; f >= 0; f--)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }

                    int key = Transform(blockers, Magic.RookMagics[i], BitOperations.PopCount(mask));
                    RookLookup[i][key] = attacks;
                }
            }
        }
        public static void InitializeBishopLookupTable()
        {
            for (int i = 0; i < 64; i++)
            {

                ulong mask = Mask.BishopMasks[i];
                int bitCount = BitOperations.PopCount(mask);
                BishopLookup[i] = new ulong[(int)Math.Pow(2, bitCount)];
                int combinations = 1 << bitCount;
                for (int j = 0; j < combinations; j++)
                {
                    ulong blockers = GenerateBlockers(j, mask);
                    ulong attacks = 0;
                    int rank = GetRank(i), r;
                    int file = GetFile(i), f;
                    //up-right
                    for (r = rank + 1, f = file + 1; r <= 7 && f <= 7; r++, f++)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }
                    //down-right
                    for (r = rank - 1, f = file + 1; r >= 0 && f <= 7; r--, f++)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }
                    //down-left
                    for (r = rank - 1, f = file - 1; r >= 0 && f >= 0; r--, f--)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }
                    //up-left
                    for (r = rank + 1, f = file - 1; r <= 7 && f >= 0; r++, f--)
                    {
                        attacks |= 1ul << GetSquare(r, f);
                        if ((blockers & (1ul << GetSquare(r, f))) != 0) break;
                    }

                    int key = Transform(blockers, Magic.BishopMagics[i], BitOperations.PopCount(mask));
                    BishopLookup[i][key] = attacks;
                }
            }
        }

        public static ulong GetBishopAttacks(int square, ulong allBlockers)
        {
            ulong blockers = allBlockers & Mask.BishopMasks[square];
            return BishopLookup[square][Transform(blockers, Magic.BishopMagics[square], BitOperations.PopCount(Mask.BishopMasks[square]))];
        }
        public static ulong GetRookAttacks(int square, ulong allBlockers)
        {
            ulong blockers = allBlockers & Mask.RookMasks[square];
            int key = Transform(blockers, Magic.RookMagics[square], BitOperations.PopCount(Mask.RookMasks[square]));
            return RookLookup[square][key];
        }
        public static ulong GetQueenAttacks(int square, ulong allBlockers)
        {

            ulong bishopBlockers = allBlockers & Mask.BishopMasks[square];
            ulong rookBlockers = allBlockers & Mask.RookMasks[square];
            int bishopKey = Transform(bishopBlockers, Magic.BishopMagics[square], BitOperations.PopCount(Mask.BishopMasks[square]));
            int rookKey = Transform(rookBlockers, Magic.RookMagics[square], BitOperations.PopCount(Mask.RookMasks[square]));
            ulong bishopAttacks = BishopLookup[square][bishopKey];
            ulong rookAttacks = RookLookup[square][rookKey];
            return bishopAttacks | rookAttacks;
        }

        private static ulong GenerateBlockers(int iteration, ulong mask)
        {
            ulong blockers = 0;
            while (iteration != 0)
            {
                if ((iteration & 1) != 0)
                {
                    int shift = BitOperations.TrailingZeroCount(mask);
                    blockers = blockers | (1ul << shift);
                }

                iteration = iteration >> 1;
                mask = mask & (mask - 1);
            }
            return blockers;
        }
        private static int Transform(ulong blockers, ulong magic, int shift)
        {
            return (int)((blockers * magic) >> (64 - shift));
        }
        private static int GetFile(int square)
        {
            return square % 8;
        }
        private static int GetRank(int square)
        {
            return square / 8;
        }
        private static int GetSquare(int rank, int file)
        {
            return rank * 8 + file;
        }
    }
}