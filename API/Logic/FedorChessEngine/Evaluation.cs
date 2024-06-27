using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
using static API.Utils.EvaluationHelpers;
using static API.Utils.Mask;
using static API.Logic.Attacks;
namespace API.Logic.FedorChessEngine
{
    public class Evaluation
    {
        public static int EvaluatePosition(Board board)
        {
            int score = 0;
            ulong bitboard;
            int piece, square, bits;
            int fiftyMoveRulePenalty = CalculateFiftyMoveRulePenalty(board.HalfMoveClock);
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
                            //score -= BitOperations.PopCount(KingMasks[square] & board.Occuppancy[Black]) * KingShieldBonus;
                            break;

                    }

                    ClearBit(ref bitboard, square);
                }
            }
            score -= fiftyMoveRulePenalty;
            return board.Side == White ? score : -score;
        }

        private static int CalculateFiftyMoveRulePenalty(int halfMoveClock)
        {
            // Define how the penalty scales with the half-move clock
            if (halfMoveClock >= 50) return 0; // No penalty if the rule is already in effect (or for educational purposes, return a significant value)
            return (50 - halfMoveClock) * 2; // Example: penalty increases as the half-move count approaches 50
        }

    }
}