using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
using static API.Utils.EvaluationHelpers;
using static API.Utils.Mask;
using static API.Logic.Attacks;
namespace API.Logic.FedorChessEngine
{
    public class Search
    {
        public const int FullDepthMoves = 4;
        public const int ReductionLimit = 3;
        public int Nodes { get; set; }
        public int HalfMoves { get; set; }
        public int BestMove { get; set; }
        public int[,] KillerMoves { get; set; }
        public int[,] HistoryMoves { get; set; }
        public int[] PvLength { get; set; }
        public int[,] PvTable { get; set; }
        public bool FollowPV { get; set; }
        public bool ScorePv { get; set; }

        public Search()
        {
            KillerMoves = new int[2, 128];
            HistoryMoves = new int[12, 64];
            PvLength = new int[128];
            PvTable = new int[128, 128];
        }
        public int SearchPosition(Board board, int depth)
        {
            ClearHelperData();
            int score = 0, alpha = -50000, beta = 50000;
            // Iterative deepening
            for (int i = 1; i <= depth; i++)
            {
                Nodes = 0;
                FollowPV = true;
                score = NegaMaxSearch(board, i, alpha, beta);
                if ((score <= alpha) || (score >= beta))
                {
                    alpha = -50000;
                    beta = 50000;
                    continue;
                }
                alpha = score - 50;
                beta = score + 50;
                Console.WriteLine(Nodes);
            }
            BestMove = PvTable[0, 0];
            return score;
        }
        private int Quiescence(Board board, int alpha, int beta)
        {
            Nodes++;
            if (HalfMoves > 127)
                return EvaluationScore(board);
            int eval = EvaluationScore(board);
            if (eval >= beta) return beta;
            if (alpha < eval) alpha = eval;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            SortMoves(moves, moveCount);
            for (int count = 0; count < moveCount; count++)
            {
                if (!IsMoveCapture(moves[count])) continue;
                int castle = board.Castle;
                board.MakeMove(moves[count], AllMoves);

                HalfMoves++;
                int score = -Quiescence(board, -beta, -alpha);
                board.UnmakeMove(moves[count], AllMoves);
                board.Castle = castle;
                HalfMoves--;
                if (score > alpha)
                {
                    alpha = score;
                    if (score >= beta) return beta;
                }
            }
            return alpha;
        }
        public int NegaMaxSearch(Board board, int depth, int alpha, int beta)
        {
            PvLength[HalfMoves] = HalfMoves;
            if (depth == 0) return Quiescence(board, alpha, beta);
            if (HalfMoves > 127) return EvaluationScore(board);
            Nodes++;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            ulong kingBitboard = board.Bitboards[board.Side == White ? K : k];
            bool KingInCheck = board.IsSquareAttacked(GetLSBIndex(kingBitboard), board.Side ^ 1);
            if (!KingInCheck && depth >= 3 && HalfMoves > 0) // Null move pruning
            {

                HalfMoves++;
                board.Side ^= 1;
                int castle = board.Castle;
                int enpasant = board.Enpassant;
                board.Enpassant = None;
                int nullScore = -NegaMaxSearch(board, depth - 2 - 1, -beta, -beta + 1);
                board.Side ^= 1;
                board.Castle = castle;
                board.Enpassant = enpasant;
                HalfMoves--;
                if (nullScore >= beta)
                {
                    return beta; // Prune the tree
                }
            }
            if (KingInCheck) depth++;
            if (FollowPV) EnablePVScore(moves, moveCount);
            SortMoves(moves, moveCount);
            int movesSearched = 0;
            for (int count = 0; count < moveCount; count++)
            {
                int castle = board.Castle;
                board.MakeMove(moves[count], AllMoves);
                HalfMoves++;
                int score;
                if (movesSearched == 0)
                    score = -NegaMaxSearch(board, depth - 1, -beta, -alpha);
                else
                {
                    // Late move reduction condition
                    if (movesSearched >= FullDepthMoves
                        && depth >= ReductionLimit
                        && !KingInCheck
                        && !IsMoveCapture(moves[count])
                        && GetMovePromoted(moves[count]) == 0)
                    {
                        score = -NegaMaxSearch(board, depth - 2, -alpha - 1, -alpha);
                    }
                    else
                    {
                        score = alpha + 1;
                    }
                    //Principal Variation Search
                    if (score > alpha)
                    {
                        score = -NegaMaxSearch(board, depth - 1, -alpha - 1, -alpha);
                        if (score > alpha && score < beta)
                        {
                            score = -NegaMaxSearch(board, depth - 1, -beta, -alpha);
                        }
                    }
                }
                board.UnmakeMove(moves[count], AllMoves);
                board.Castle = castle;
                HalfMoves--;
                movesSearched++;
                if (score >= beta)
                {
                    if (!IsMoveCapture(moves[count]))
                    {
                        KillerMoves[1, HalfMoves] = KillerMoves[0, HalfMoves];
                        KillerMoves[0, HalfMoves] = moves[count];
                    }
                    return beta;
                }
                if (score > alpha)
                {
                    HistoryMoves[GetMovePiece(moves[count]), GetMoveTarget(moves[count])] += depth;
                    alpha = score;
                    PvTable[HalfMoves, HalfMoves] = moves[count];
                    for (int next = HalfMoves + 1; next < PvLength[HalfMoves + 1]; next++)
                    {
                        PvTable[HalfMoves, next] = PvTable[HalfMoves + 1, next];
                    }
                    PvLength[HalfMoves] = PvLength[HalfMoves + 1];
                }
            }
            if (moveCount == 0)
            {
                if (KingInCheck) return -49000 + HalfMoves;
                else return 0;
            }

            return alpha;
        }
        public void EnablePVScore(int[] moves, int moveCount)
        {
            FollowPV = false;
            for (int count = 0; count < moveCount; count++)
            {
                if (moves[count] == PvTable[0, HalfMoves])
                {
                    FollowPV = true;
                    ScorePv = true;
                    return;
                }
            }

        }
        public void SortMoves(int[] moves, int moveCount)
        {
            var scoredMoves = new List<(int move, int score)>();
            for (int i = 0; i < moveCount; i++)
            {
                int score = ScoreMove(moves[i]);
                scoredMoves.Add((moves[i], score));
            }

            scoredMoves.Sort((a, b) => b.score.CompareTo(a.score));

            for (int i = 0; i < moveCount; i++)
            {
                moves[i] = scoredMoves[i].move;
            }

        }

        public int EvaluationScore(Board board)
        {
            int score = 0;
            ulong bitboard;
            int piece, square, bits;
            for (int i = P; i <= n; i++)
            {
                bitboard = board.Bitboards[i];
                while (bitboard != 0)
                {
                    piece = i;
                    square = GetLSBIndex(bitboard);

                    score += MaterialScore[piece];
                    switch (piece)
                    {
                        case P:
                            bits = BitOperations.PopCount(board.Bitboards[P] & FileMask[square]);
                            // Double pawn penalty and pawn positional score
                            score += PawnScore[square];
                            if (bits > 1)
                                score += bits * DoublePawnPenalty;
                            // Passed pawn bonus
                            if ((board.Bitboards[P] & IsolatedPawnMask[square]) == 0)
                                score += IsolatedPawnPenalty;
                            if ((board.Bitboards[p] & WhitePassedPawnMasks[square]) == 0)
                                score += PassedPawnBonus[GetRank[square]];
                            break;
                        case N:
                            score += KnightScore[square]; break;
                        case B:
                            score += BishopScore[square];
                            score += BitOperations.PopCount(GetBishopAttacks(square, board.Occuppancy[Both]));
                            break;
                        case R:
                            score += RookScore[square];
                            if ((board.Bitboards[P] & FileMask[square]) == 0)
                                score += OnSemiOpenFileBonus;
                            if (((board.Bitboards[P] | board.Bitboards[p]) & FileMask[square]) == 0)
                                score += OnOpenFileBonus;
                            break;
                        case Q:
                            score += BitOperations.PopCount(GetRookAttacks(square, board.Occuppancy[Both]));
                            score += BitOperations.PopCount(GetBishopAttacks(square, board.Occuppancy[Both]));
                            break;
                        case K:
                            score += KingScore[square];
                            if ((board.Bitboards[P] & FileMask[square]) == 0)
                                score -= OnSemiOpenFileBonus;
                            if (((board.Bitboards[P] | board.Bitboards[p]) & FileMask[square]) == 0)
                                score -= OnOpenFileBonus;
                            score += BitOperations.PopCount(KingMasks[square] & board.Occuppancy[White]) * KingShieldBonus;
                            break;
                        case p:
                            bits = BitOperations.PopCount(board.Bitboards[p] & FileMask[square]);
                            // Double pawn penalty and pawn positional score
                            score -= PawnScore[MirrorScore[square]];
                            if (bits > 1)
                                score -= bits * DoublePawnPenalty;
                            // Passed pawn bonus
                            if ((board.Bitboards[P] & BlackPassedPawnMasks[square]) == 0)
                                score -= PassedPawnBonus[GetRank[MirrorScore[square]]];
                            // Isolated pawn penalty
                            if ((board.Bitboards[p] & IsolatedPawnMask[square]) == 0)
                                score -= IsolatedPawnPenalty;

                            break;
                        case n:
                            score -= KnightScore[MirrorScore[square]]; break;
                        case b:
                            score -= BishopScore[MirrorScore[square]];
                            score -= BitOperations.PopCount(GetBishopAttacks(square, board.Occuppancy[Both]));
                            break;
                        case r:
                            score -= RookScore[MirrorScore[square]];
                            if ((board.Bitboards[p] & FileMask[square]) == 0)
                                score -= OnSemiOpenFileBonus;
                            if (((board.Bitboards[P] | board.Bitboards[p]) & FileMask[square]) == 0)
                                score -= OnOpenFileBonus;
                            break;
                        case q:
                            score += BitOperations.PopCount(GetRookAttacks(square, board.Occuppancy[Both]));
                            score += BitOperations.PopCount(GetBishopAttacks(square, board.Occuppancy[Both]));
                            break;
                        case k:
                            score -= KingScore[MirrorScore[square]];
                            if ((board.Bitboards[p] & FileMask[square]) == 0)
                                score += OnSemiOpenFileBonus;
                            if (((board.Bitboards[P] | board.Bitboards[p]) & FileMask[square]) == 0)
                                score += OnOpenFileBonus;
                            score -= BitOperations.PopCount(KingMasks[square] & board.Occuppancy[Black]) * KingShieldBonus;
                            break;

                    }

                    ClearBit(ref bitboard, square);
                }
            }
            return board.Side == White ? score : -score;
        }
        public int ScoreMove(int move)
        {
            if (ScorePv == true)
            {
                ScorePv = false;
                if (move == PvTable[0, HalfMoves]) return 20000;
            }
            if (IsMoveCapture(move))
            {
                int capturedPiece = GetMoveCaptured(move);
                int movedPiece = GetMovePiece(move);
                if (IsMoveEnpassant(move))
                {
                    capturedPiece = movedPiece == p ? P : p;
                }
                return VictimAttackerValueMatrix[movedPiece][capturedPiece] + 10000;
            }
            else
            {
                if (KillerMoves[0, HalfMoves] == move) return 9000;
                else if (KillerMoves[1, HalfMoves] == move) return 8000;
                else return HistoryMoves[GetMovePiece(move), GetMoveTarget(move)];
            }
        }
        private void ClearHelperData()
        {
            Array.Clear(KillerMoves, 0, KillerMoves.Length);
            Array.Clear(HistoryMoves, 0, HistoryMoves.Length);
            Array.Clear(PvLength, 0, PvLength.Length);
            Array.Clear(PvTable, 0, PvTable.Length);
            Nodes = 0;
            HalfMoves = 0;
            BestMove = 0;
            FollowPV = false;
            ScorePv = false;
        }
    }
}