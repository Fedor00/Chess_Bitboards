using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static API.Utils.EvaluationHelpers;
using static API.Utils.BitbboardUtils;
namespace API.Logic.FedorChessEngine
{
    public class Evaluation
    {
        public static int EvaluationScore(Board board)
        {
            int score = 0;
            ulong bitboard;
            int piece, square;
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
                            score += PawnScore[square]; break;
                        case N:
                            score += KnightScore[square]; break;
                        case B:
                            score += BishopScore[square]; break;
                        case R:
                            score += RookScore[square]; break;
                        case K:
                            score += KingScore[square]; break;
                        case p:
                            score -= PawnScore[MirrorScore[square]]; break;
                        case n:
                            score -= KnightScore[MirrorScore[square]]; break;
                        case b:
                            score -= BishopScore[MirrorScore[square]]; break;
                        case r:
                            score -= RookScore[MirrorScore[square]]; break;
                        case k:
                            score -= KingScore[MirrorScore[square]]; break;

                    }

                    ClearBit(ref bitboard, square);
                }
            }
            return board.Side == White ? score : -score;
        }
    }
}