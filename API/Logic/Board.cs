using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using API.Utils;
using Microsoft.Net.Http.Headers;
using static API.Utils.BitbboardUtils;
using static API.Utils.ChessHelpers;
using static API.Utils.Mask;
using static API.Logic.Attacks;
using System.Numerics;
using System.Runtime.CompilerServices;
namespace API.Logic
{

    public class Board
    {
        public ulong[] Bitboards { get; set; }
        public ulong[] Occuppancy { get; set; }
        public int[] PiecePositions { get; set; }
        public int Side { get; set; }
        public int Enpassant { get; set; }
        public int PreviousEnpassant { get; set; }
        public int PreviousCastle { get; set; }
        public int Castle { get; set; }
        public int HalfMoveClock { get; set; }
        public int FullMoveNumber { get; set; }
        public Board(string fen)
        {
            Fen.InitializeBoard(this, fen);
            InitializePiecePositions();
            PrintPiecePositions();
        }
        public Board() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int MakeMove(int move, int moveFlag)
        {
            if (moveFlag == AllMoves)
            {
                int source = GetMoveSource(move);
                int target = GetMoveTarget(move);
                int piece = GetMovePiece(move);
                int promotion = GetMovePromoted(move);
                int pieceCaptured = GetMoveCaptured(move);
                bool enpassant = IsMoveEnpassant(move);
                bool castling = IsMoveCastle(move);
                bool capture = IsMoveCapture(move);
                bool doublePawnPush = IsMoveDoublePawnPush(move);
                ClearBit(ref Bitboards[piece], source);
                SetBit(ref Bitboards[piece], target);
                PiecePositions[target] = piece;
                PiecePositions[source] = -1;
                if (capture && !enpassant)
                {
                    if (pieceCaptured != -1)
                        ClearBit(ref Bitboards[pieceCaptured], target);
                }
                if (promotion != 0)
                {
                    ClearBit(ref Bitboards[Side == White ? P : p], target);
                    SetBit(ref Bitboards[promotion], target);
                    PiecePositions[target] = promotion;
                }
                else if (enpassant)
                {

                    if (Side == White)
                    {
                        ClearBit(ref Bitboards[p], target + 8);
                        PiecePositions[target + 8] = -1;
                    }
                    else
                    {
                        ClearBit(ref Bitboards[P], target - 8);
                        PiecePositions[target - 8] = -1;
                    }
                }
                PreviousEnpassant = Enpassant;
                Enpassant = None;
                //if double pawn push, set enpassant square
                if (doublePawnPush)
                {
                    Enpassant = Side == White ? target + 8 : target - 8;
                }
                else if (castling)
                {
                    switch (target)
                    {
                        case c1: // white queen side
                            ClearBit(ref Bitboards[R], a1);
                            SetBit(ref Bitboards[R], d1);
                            PiecePositions[d1] = R;
                            PiecePositions[a1] = -1;
                            break;
                        case g1: // white king side
                            ClearBit(ref Bitboards[R], h1);
                            SetBit(ref Bitboards[R], f1);
                            PiecePositions[f1] = R;
                            PiecePositions[h1] = -1;
                            break;
                        case c8: // black queen side
                            ClearBit(ref Bitboards[r], a8);
                            SetBit(ref Bitboards[r], d8);
                            PiecePositions[d8] = r;
                            PiecePositions[a8] = -1;
                            break;
                        case g8: // black king side
                            ClearBit(ref Bitboards[r], h8);
                            SetBit(ref Bitboards[r], f8);
                            PiecePositions[f8] = r;
                            PiecePositions[h8] = -1;
                            break;
                    }
                }

                Castle &= CastlingRights[source] & CastlingRights[target];
                UpdateOccupancy();
                //Console.WriteLine(AsciiPiece[pieceCaptured]);
                //PrintPiecePositions();
                //PrintBitboard(Occuppancy[Both]);
                Side ^= 1;
            }
            else
            {
                if (IsMoveCapture(move))
                    MakeMove(move, AllMoves);
                else return 0;
            }
            return 1;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int UnmakeMove(int move, int moveFlag)
        {

            if (moveFlag == AllMoves)
            {
                int source = GetMoveSource(move);
                int target = GetMoveTarget(move);
                int piece = GetMovePiece(move);
                int promotion = GetMovePromoted(move);
                int pieceCaptured = GetMoveCaptured(move);
                bool enpassant = IsMoveEnpassant(move);
                bool castling = IsMoveCastle(move);
                bool capture = IsMoveCapture(move);
                SetBit(ref Bitboards[piece], source);
                ClearBit(ref Bitboards[piece], target);
                PiecePositions[target] = -1;
                PiecePositions[source] = piece;
                if (capture && !enpassant)
                {
                    if (pieceCaptured != -1)
                    {
                        SetBit(ref Bitboards[pieceCaptured], target);
                        PiecePositions[target] = pieceCaptured;
                    }
                }
                if (promotion != 0)
                {
                    ClearBit(ref Bitboards[promotion], target);
                }
                else if (enpassant)
                {
                    if (Side == White)
                    {
                        SetBit(ref Bitboards[P], target - 8);
                        PiecePositions[target - 8] = P;
                    }
                    else
                    {
                        SetBit(ref Bitboards[p], target + 8);
                        PiecePositions[target + 8] = p;
                    }
                }
                Enpassant = PreviousEnpassant;
                PreviousEnpassant = None;

                if (castling)
                {
                    switch (target)
                    {
                        case c1: // white queen side
                            SetBit(ref Bitboards[R], a1);
                            ClearBit(ref Bitboards[R], d1);
                            PiecePositions[d1] = -1;
                            PiecePositions[a1] = R;
                            break;
                        case g1: // white king side
                            SetBit(ref Bitboards[R], h1);
                            ClearBit(ref Bitboards[R], f1);
                            PiecePositions[f1] = -1;
                            PiecePositions[h1] = R;
                            break;
                        case c8: // black queen side
                            SetBit(ref Bitboards[r], a8);
                            ClearBit(ref Bitboards[r], d8);
                            PiecePositions[d8] = -1;
                            PiecePositions[a8] = r;
                            break;
                        case g8: // black king side
                            SetBit(ref Bitboards[r], h8);
                            ClearBit(ref Bitboards[r], f8);
                            PiecePositions[f8] = -1;
                            PiecePositions[h8] = r;
                            break;
                    }
                }
                UpdateOccupancy();
                Side ^= 1;
                return 1;
            }
            else
            {
                if (IsMoveCapture(move))
                    MakeMove(move, AllMoves);
                else return 0;
            }
            return 1;

        }
        private void GenerateCastlingMoves(int[] moves, ref int moveCount)
        {
            if (Side == White)
                GenerateWhiteCastling(moves, ref moveCount);
            else
                GenerateBlackCastling(moves, ref moveCount);
        }

        private void GenerateBlackCastling(int[] moves, ref int moveCount)
        {
            //if black king can castle
            if ((Castle & BK) != 0)
            {
                //if squares f8 and g8 are empty 
                if (!!AreSquaresEmpty(f8, g8, Occuppancy[Both]))
                {
                    //if squares e8 and f8 are not attacked
                    if (!AreSquaresAttacked(f8, g8, White))
                    {
                        moves[moveCount++] = EncodeMove(e8, g8, k, 0, -1, false, true, false, false);
                    }
                }
            }
            //if black queen can't castle return
            if ((Castle & BQ) == 0) return;
            //if squares b8, c8 and d8 are not empty return
            if (!AreSquaresEmpty(b8, d8, Occuppancy[Both])) return;
            //if squares c8 and d8 are attacked return
            if (AreSquaresAttacked(c8, d8, White)) return;
            moves[moveCount++] = EncodeMove(e8, c8, k, 0, -1, false, true, false, false);
        }

        private void GenerateWhiteCastling(int[] moves, ref int moveCount)
        {
            //if white king can castle
            if ((Castle & WK) != 0)
            {
                //if squares f1 and g1 are empty
                if (!IsBitSet(Occuppancy[Both], f1) && !IsBitSet(Occuppancy[Both], g1))
                {
                    //if squares e1 and f1 are not attacked
                    if (!AreSquaresAttacked(f1, g1, Black))
                    {
                        moves[moveCount++] = EncodeMove(e1, g1, K, 0, -1, false, true, false, false);
                    }
                }
            }
            //if white queen can't castle return
            if ((Castle & WQ) == 0) return;
            //if squares b1, c1 and d1 are not empty return
            if (!AreSquaresEmpty(b1, d1, Occuppancy[Both])) return;
            //if squares c1 and d1 are  attacked return
            if (AreSquaresAttacked(c1, d1, Black)) return;
            moves[moveCount++] = EncodeMove(e1, c1, K, 0, -1, false, true, false, false);
        }

        private bool AreSquaresEmpty(int from, int to, ulong Occuppancy)
        {
            for (int i = from; i <= to; i++)
            {
                if (IsBitSet(Occuppancy, i)) return false;
            }
            return true;

        }
        private bool AreSquaresAttacked(int from, int to, int side)
        {
            for (int i = from; i <= to; i++)
            {
                if (IsSquareAttacked(i, side)) return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsPromotionRow(int source, int side)
        {
            return side == White ? (source >= a7 && source <= h7) : (source >= a2 && source <= h2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsDoublePawnPushRow(int source, int side)
        {
            return side == White ? (source >= a2 && source <= h2) : (source >= a7 && source <= h7);
        }
        public void GeneratePromotionMoves(int[] moves, ref int moveCount, int source, int square, bool capture, int pawn, int side)
        {
            int firstIndex = side == White ? B : b;
            int lastIndex = side == White ? N : n;
            // if white: B R Q N else b r q n
            for (; firstIndex <= lastIndex; firstIndex++)
            {
                moves[moveCount++] = EncodeMove(source, square, pawn, firstIndex, PiecePositions[square], false, false, capture, false);
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSquareAttacked(int square, int side)
        {
            if ((side == White) && (PawnAttacks[Black][square] & Bitboards[P]) != 0) return true;
            if ((side == Black) && (PawnAttacks[White][square] & Bitboards[p]) != 0) return true;
            if ((KnightMasks[square] & (side == White ? Bitboards[N] : Bitboards[n])) != 0) return true;
            if ((KingMasks[square] & (side == White ? Bitboards[K] : Bitboards[k])) != 0) return true;
            if ((GetRookAttacks(square, Occuppancy[Both]) & (side == White ? (Bitboards[R] | Bitboards[Q]) : (Bitboards[r] | Bitboards[q]))) != 0) return true;
            if ((GetBishopAttacks(square, Occuppancy[Both]) & (side == White ? (Bitboards[B] | Bitboards[Q]) : (Bitboards[b] | Bitboards[q]))) != 0) return true;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] HandleKingNotInCheckMoves(ref int moveCount, int kingSquare)
        {
            int[] moves = new int[218];
            int source, target;
            int from = Side == White ? P : p;
            int to = Side == White ? N : n;
            ulong attacks, bitboard, path = 0;
            Span<int> pinnedPieces = PinnedPieces(Side, kingSquare);
            int[] pinMappings = new int[64];
            Array.Fill(pinMappings, -1);
            for (int i = 0; i < pinnedPieces.Length; i++)
            {
                DecodePin(pinnedPieces[i], out int pinnedPiece, out int pinningSquare);
                pinMappings[pinnedPiece] = pinningSquare;
            }
            for (int piece = from; piece <= to; piece++)
            {
                bitboard = Bitboards[piece];
                if (piece == N || piece == n)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        //if knight is pinned then no moves
                        if (pinMappings[source] != -1) { ClearBit(ref bitboard, source); continue; }
                        attacks = KnightMasks[source] & (~Occuppancy[Side]);
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == P || piece == p)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        target = GetLSBIndex(PawnSingleMoves[Side][source]);
                        bool isPin = pinMappings[source] != -1;
                        if (isPin) path = Mask.Path[kingSquare][pinMappings[source]];
                        if (!IsBitSet(Occuppancy[Both], target) && (!isPin || (isPin && (path & (1UL << target)) != 0)))
                        {
                            //promotion
                            if (IsPromotionRow(source, Side))
                            {
                                GeneratePromotionMoves(moves, ref moveCount, source, target, false, piece, Side);
                            }
                            else
                            {
                                //normal pawn push
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                                target = GetLSBIndex(PawnDoubleMoves[Side][source]);
                                //double pawn push
                                if (IsDoublePawnPushRow(source, Side) && !IsBitSet(Occuppancy[Both], target))
                                {
                                    moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, true);
                                }
                            }
                        }
                        attacks = Side == White ? PawnAttacks[White][source] & Occuppancy[Black] : PawnAttacks[Black][source] & Occuppancy[White];
                        if (isPin) attacks &= 1UL << pinMappings[source];
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //promotion capture
                            if (IsPromotionRow(source, Side))
                            {
                                GeneratePromotionMoves(moves, ref moveCount, source, target, true, piece, Side);
                            }
                            else
                            {
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            }
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == B || piece == b)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = GetBishopAttacks(source, Occuppancy[Both]) & (~Occuppancy[Side]);
                        if (pinMappings[source] != -1) attacks &= Mask.Path[kingSquare][pinMappings[source]] | (1UL << pinMappings[source]);
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == R || piece == r)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = GetRookAttacks(source, Occuppancy[Both]) & (~Occuppancy[Side]);
                        if (pinMappings[source] != -1) attacks &= Mask.Path[kingSquare][pinMappings[source]] | (1UL << pinMappings[source]);
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == Q || piece == q)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = GetQueenAttacks(source, Occuppancy[Both]) & (~Occuppancy[Side]);
                        if (pinMappings[source] != -1) attacks &= Mask.Path[kingSquare][pinMappings[source]] | (1UL << pinMappings[source]);
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == K || piece == k)
                {
                    attacks = KingMasks[kingSquare] & ~Occuppancy[Side]; ;
                    while (attacks != 0)
                    {
                        target = GetLSBIndex(attacks);
                        if (!IsSquareAttacked(target, Side == White ? Black : White))
                            if (IsBitSet(Occuppancy[Both], target))
                                moves[moveCount++] = EncodeMove(kingSquare, target, Side == White ? K : k, 0, PiecePositions[target], false, false, true, false);
                            else
                                moves[moveCount++] = EncodeMove(kingSquare, target, Side == White ? K : k, 0, -1, false, false, false, false);
                        ClearBit(ref attacks, target);
                    }
                }
            }
            if (Enpassant != None)
            {
                ulong pawns = Side == White ? PawnAttacks[Black][Enpassant] & Bitboards[P] : PawnAttacks[White][Enpassant] & Bitboards[p];
                while (pawns != 0)
                {
                    //for each pawn that can capture enpassant 
                    //simulate the capture and check if king is attacked
                    source = GetLSBIndex(pawns);
                    ClearBit(ref Bitboards[Side == White ? P : p], source);
                    ClearBit(ref Bitboards[Side == White ? p : P], Enpassant + (Side == White ? 8 : -8));
                    SetBit(ref Bitboards[Side == White ? P : p], Enpassant);
                    UpdateOccupancy();
                    if (!IsSquareAttacked(kingSquare, Side == White ? Black : White))
                        moves[moveCount++] = EncodeMove(source, Enpassant, Side == White ? P : p, 0, -1, true, false, true, false);
                    ClearBit(ref Bitboards[Side == White ? P : p], Enpassant);
                    SetBit(ref Bitboards[Side == White ? p : P], Enpassant + (Side == White ? 8 : -8));
                    SetBit(ref Bitboards[Side == White ? P : p], source);
                    UpdateOccupancy();
                    ClearBit(ref pawns, source);
                }
            }
            GenerateCastlingMoves(moves, ref moveCount);
            return moves;
        }
        public int[] GenerateLegalMoves(ref int moveCount)
        {
            ulong king = Side == White ? Bitboards[K] : Bitboards[k];
            int kingSquare = GetLSBIndex(king);
            ulong attacker = GetAttackers(kingSquare, Side ^ 1);
            int attackerCount = BitOperations.PopCount(attacker);
            switch (attackerCount)
            {
                case 0:
                    return HandleKingNotInCheckMoves(ref moveCount, kingSquare);
                case 1:
                    return HandleKingInCheckMoves(ref moveCount, kingSquare, attacker);
                default:
                    return HandleDoubleCheck(ref moveCount, kingSquare);
            }
        }

        private int[] HandleDoubleCheck(ref int moveCount, int kingSquare)
        {
            int[] moves = new int[8];
            ulong kingMoves = KingMasks[kingSquare] & ~Occuppancy[Side];
            int target;
            ClearBit(ref Bitboards[Side == White ? K : k], kingSquare);
            UpdateOccupancy();
            while (kingMoves != 0)
            {
                target = GetLSBIndex(kingMoves);
                if (!IsSquareAttacked(target, Side == White ? Black : White))
                    if (IsBitSet(Occuppancy[Both], target))
                        moves[moveCount++] = EncodeMove(kingSquare, target, Side == White ? K : k, 0, PiecePositions[target], false, false, true, false);
                    else
                        moves[moveCount++] = EncodeMove(kingSquare, target, Side == White ? K : k, 0, -1, false, false, false, false);
                ClearBit(ref kingMoves, target);
            }
            SetBit(ref Bitboards[Side == White ? K : k], kingSquare);
            UpdateOccupancy();
            return moves;
        }

        public int[] HandleKingInCheckMoves(ref int moveCount, int kingSquare, ulong attacker)
        {
            int[] moves = new int[218];
            int source, target, from = Side == White ? P : p, to = Side == White ? N : n;
            ulong attacks, bitboard, path = Mask.Path[kingSquare][GetLSBIndex(attacker)] | attacker;
            Span<int> pins = PinnedPieces(Side, kingSquare);
            ulong pinnedPieces = 0;
            for (int i = 0; i < pins.Length; i++)
            {
                DecodePin(pins[i], out int pinnedPiece, out _);
                pinnedPieces |= 1UL << pinnedPiece;
            }

            if ((Bitboards[Side == White ? N : n] & attacker) != 0) path = attacker;//if knight is attacking king,you can only capture it
            for (int piece = from; piece <= to; piece++)
            {
                if (piece == K || piece == k) continue;
                bitboard = Bitboards[piece] & ~pinnedPieces;
                if (piece == P || piece == p)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        target = GetLSBIndex(PawnSingleMoves[Side][source]);
                        if (!IsBitSet(Occuppancy[Both], target))
                        {
                            //if pawn is pinned then check if the move is along the pin path
                            if ((path & PawnSingleMoves[Side][source]) != 0)
                                if (IsPromotionRow(source, Side))
                                {
                                    GeneratePromotionMoves(moves, ref moveCount, source, target, false, piece, Side);
                                }
                                else
                                {
                                    moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                                }
                            //same for double pawn push
                            if ((path & PawnDoubleMoves[Side][source]) != 0)
                            {
                                target = GetLSBIndex(PawnDoubleMoves[Side][source]);
                                //double pawn push
                                if (IsDoublePawnPushRow(source, Side) && !IsBitSet(Occuppancy[Both], target))
                                {
                                    moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, true);
                                }
                            }
                        }

                        attacks = Side == White ? PawnAttacks[White][source] & Occuppancy[Black] : PawnAttacks[Black][source] & Occuppancy[White];
                        //if diagonal pin then only allow capturing the attacker
                        attacks &= attacker;
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //promotion capture
                            if (IsPromotionRow(source, Side))
                            {
                                GeneratePromotionMoves(moves, ref moveCount, source, target, true, piece, Side);
                            }
                            else
                            {
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            }
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == B || piece == b)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = GetBishopAttacks(source, Occuppancy[Both]) & (~Occuppancy[Side]);
                        attacks &= path;
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == R || piece == r)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = GetRookAttacks(source, Occuppancy[Both]) & (~Occuppancy[Side]);
                        attacks &= path;
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == Q || piece == q)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = GetQueenAttacks(source, Occuppancy[Both]) & (~Occuppancy[Side]);
                        attacks &= path;
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
                else if (piece == N || piece == n)
                {
                    while (bitboard != 0)
                    {
                        source = GetLSBIndex(bitboard);
                        attacks = KnightMasks[source] & (~Occuppancy[Side]);
                        attacks &= path;
                        while (attacks != 0)
                        {
                            target = GetLSBIndex(attacks);
                            //capture move
                            if (IsBitSet(Occuppancy[Side ^ 1], target))
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, PiecePositions[target], false, false, true, false);
                            else //quiet move
                                moves[moveCount++] = EncodeMove(source, target, piece, 0, -1, false, false, false, false);
                            ClearBit(ref attacks, target);
                        }
                        ClearBit(ref bitboard, source);
                    }
                }
            }
            ulong kingMoves = KingMasks[kingSquare] & ~Occuppancy[Side]; ;
            ClearBit(ref Bitboards[Side == White ? K : k], kingSquare);
            UpdateOccupancy();
            while (kingMoves != 0)
            {
                target = GetLSBIndex(kingMoves);
                if (!IsSquareAttacked(target, Side == White ? Black : White))
                    if (IsBitSet(Occuppancy[Both], target))
                        moves[moveCount++] = EncodeMove(kingSquare, target, Side == White ? K : k, 0, PiecePositions[target], false, false, true, false);
                    else
                        moves[moveCount++] = EncodeMove(kingSquare, target, Side == White ? K : k, 0, -1, false, false, false, false);
                ClearBit(ref kingMoves, target);
            }
            SetBit(ref Bitboards[Side == White ? K : k], kingSquare);
            UpdateOccupancy();
            if (Enpassant != None)
            {
                ulong pawns = Side == White ? PawnAttacks[Black][Enpassant] & Bitboards[P] : PawnAttacks[White][Enpassant] & Bitboards[p];
                ulong enpassantTarget = 1UL << (Side == White ? Enpassant + 8 : Enpassant - 8);
                if ((enpassantTarget & path) != 0)
                    while (pawns != 0)
                    {
                        source = GetLSBIndex(pawns);
                        ClearBit(ref Bitboards[Side == White ? P : p], source);
                        ClearBit(ref Bitboards[Side == White ? p : P], Enpassant + (Side == White ? 8 : -8));
                        SetBit(ref Bitboards[Side == White ? P : p], Enpassant);
                        UpdateOccupancy();
                        if (!IsSquareAttacked(kingSquare, Side == White ? Black : White))
                            moves[moveCount++] = EncodeMove(source, Enpassant, Side == White ? P : p, 0, -1, true, false, true, false);
                        ClearBit(ref Bitboards[Side == White ? P : p], Enpassant);
                        SetBit(ref Bitboards[Side == White ? p : P], Enpassant + (Side == White ? 8 : -8));
                        SetBit(ref Bitboards[Side == White ? P : p], source);
                        UpdateOccupancy();
                        ClearBit(ref pawns, source);

                    }
            }

            return moves;
        }
        public ulong GetAttackers(int square, int side)
        {
            ulong rookAttacks = GetRookAttacks(square, Occuppancy[Both]) & Occuppancy[side];
            ulong bishopAttacks = GetBishopAttacks(square, Occuppancy[Both]) & Occuppancy[side];
            ulong diagonalAttacks = bishopAttacks & (Bitboards[side == White ? B : b] | Bitboards[side == White ? Q : q]);
            ulong linearAttacks = rookAttacks & (Bitboards[side == White ? R : r] | Bitboards[side == White ? Q : q]);
            ulong pawnAttacks = PawnAttacks[side ^ 1][square] & Bitboards[side == White ? P : p];
            ulong knightAttacks = KnightMasks[square] & Bitboards[side == White ? N : n];
            ulong allAttacks = linearAttacks | diagonalAttacks | pawnAttacks | knightAttacks;

            return allAttacks;
        }
        public Span<int> PinnedPieces(int side, int kingSquare)
        {
            Span<int> pinnedPieces = new(new int[8]);
            int count = 0;
            ulong myPieces = side == White ? Occuppancy[White] : Occuppancy[Black];
            ulong opponentRooks = side == White ? Bitboards[r] : Bitboards[R];
            ulong opponentBishops = side == White ? Bitboards[b] : Bitboards[B];
            ulong opponentQueens = side == White ? Bitboards[q] : Bitboards[Q];
            ulong potentialRooks = (opponentRooks | opponentQueens) & FullRookMasks[kingSquare];
            ulong potentialBishops = (opponentBishops | opponentQueens) & FullBishopMasks[kingSquare];
            ulong pieces = potentialBishops | potentialRooks;
            while (pieces != 0)
            {
                int pinningSquare = GetLSBIndex(pieces);
                ulong path = Mask.Path[kingSquare][pinningSquare] & Occuppancy[Both];
                if (BitOperations.PopCount(path) == 1)
                {
                    if ((path & myPieces) != 0)
                    {

                        pinnedPieces[count++] = EncodePin(GetLSBIndex(path), pinningSquare);
                    }
                }
                ClearBit(ref pieces, pinningSquare);
            }
            return pinnedPieces.Slice(0, count);
        }
        public void PrintPiecePositions()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    char piece = PiecePositions[row * 8 + col] != -1 ? AsciiPiece[PiecePositions[row * 8 + col]] : '-';
                    Console.Write($"{piece,3}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateOccupancy()
        {
            Occuppancy[0] = Occuppancy[1] = Occuppancy[2] = 0;
            for (int i = P; i <= N; i++)
            {
                Occuppancy[White] |= Bitboards[i];
                Occuppancy[Black] |= Bitboards[i + p];
            }
            Occuppancy[Both] = Occuppancy[White] | Occuppancy[Black];
        }
        public char[][] TransformToPieces()
        {
            char[][] boardRepresentation = new char[10][];
            for (int i = 0; i < 10; i++)
            {
                boardRepresentation[i] = new char[10];
                for (int j = 0; j < 10; j++)
                {
                    if (i == 0 || i == 9)
                    {
                        if (j > 0 && j < 9)
                        {
                            boardRepresentation[i][j] = (char)('a' + j - 1);
                        }
                        else
                        {
                            boardRepresentation[i][j] = ' ';
                        }
                    }
                    else if (j == 0 || j == 9)
                    {
                        if (i > 0 && i < 9)
                        {
                            boardRepresentation[i][j] = (char)('1' + 8 - i);
                        }
                        else
                        {
                            boardRepresentation[i][j] = ' ';
                        }
                    }
                    else
                    {
                        boardRepresentation[i][j] = 'X';
                    }
                }
            }
            for (int pieceType = 0; pieceType < Bitboards.Length; pieceType++)
            {
                ulong bitboard = Bitboards[pieceType];
                for (int squareIndex = 0; squareIndex < 64; squareIndex++)
                {
                    ulong squareBit = 1UL << squareIndex;
                    if ((bitboard & squareBit) != 0)
                    {

                        int row = (squareIndex / 8) + 1;
                        int col = (squareIndex % 8) + 1;


                        int piece = pieceType;
                        boardRepresentation[row][col] = PieceToChar[pieceType];
                    }
                }
            }
            PrintBoard(this);
            return boardRepresentation;
        }

        public Board Clone()
        {
            Board copy = new()
            {
                Bitboards = (ulong[])Bitboards.Clone(),
                Occuppancy = (ulong[])Occuppancy.Clone(),
                Side = Side,
                Enpassant = Enpassant,
                Castle = Castle,
                HalfMoveClock = HalfMoveClock,
                FullMoveNumber = FullMoveNumber,
                PiecePositions = (int[])PiecePositions.Clone()

            };
            return copy;
        }

        public void RestoreBoardState(Board copiedBoard)
        {
            Bitboards = copiedBoard.Bitboards;
            Side = copiedBoard.Side;
            Enpassant = copiedBoard.Enpassant;
            Castle = copiedBoard.Castle;
            HalfMoveClock = copiedBoard.HalfMoveClock;
            FullMoveNumber = copiedBoard.FullMoveNumber;
            Occuppancy = copiedBoard.Occuppancy;
        }
        public void InitializePiecePositions()
        {
            PiecePositions = new int[64];
            Array.Fill(PiecePositions, -1);
            for (int pieceType = 0; pieceType < 12; pieceType++)
            {
                ulong bitboard = Bitboards[pieceType];
                while (bitboard != 0)
                {
                    int square = GetLSBIndex(bitboard);
                    PiecePositions[square] = pieceType;
                    ClearBit(ref bitboard, square);
                }
            }
        }
    }
}