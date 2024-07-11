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
        public int Ply { get; set; }
        public int BestMove { get; set; }
        public int[,] KillerMoves { get; set; }
        public int[,] HistoryMoves { get; set; }
        public int[] PvLength { get; set; }
        public int[,] PvTable { get; set; }
        public ulong[] RepetitionTable { get; set; }
        public int RepetitionIndex { get; set; }
        public bool FollowPV { get; set; }
        public bool ScorePv { get; set; }

        public TranspositionTable TranspositionTable { get; private set; }

        public Search()
        {
            KillerMoves = new int[2, 128];
            HistoryMoves = new int[12, 64];
            PvLength = new int[128];
            PvTable = new int[128, 128];
            RepetitionTable = new ulong[1000];

            TranspositionTable = new TranspositionTable();
        }
        public int SearchPosition(Board board, int depth)
        {
            ClearHelperData();
            int score = 0, alpha = -Infinity, beta = Infinity;
            // Iterative deepening
            for (int i = 1; i <= depth; i++)
            {
                Nodes = 0;
                FollowPV = true;
                score = NegaMaxSearch(board, i, alpha, beta);
                if ((score <= alpha) || (score >= beta))
                {
                    alpha = -Infinity;
                    beta = Infinity;
                    continue;
                }
                alpha = score - 50;
                beta = score + 50;
                Console.WriteLine(Nodes);
            }
            for (int i = 0; i < PvLength[0]; i++)
            {
                Console.WriteLine(MoveToString(PvTable[0, i]));
            }
            BestMove = PvTable[0, 0];
            return score;
        }
        private void UpdateRepetitionTable(ulong hashkey)
        {
            RepetitionIndex++;
            RepetitionTable[RepetitionIndex] = hashkey;
        }
        private int Quiescence(Board board, int alpha, int beta)
        {

            Nodes++;
            if (Ply > 127)
                return Evaluation.EvaluatePosition(board);
            int eval = Evaluation.EvaluatePosition(board);
            if (eval >= beta) return beta;
            if (alpha < eval) alpha = eval;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            SortMoves(moves, moveCount);
            for (int count = 0; count < moveCount; count++)
            {
                if (!IsMoveCapture(moves[count])) continue;
                int castle = board.Castle, halfMoveClock = board.HalfMoveClock;
                ulong hashkey = board.Hashkey;
                board.MakeMove(moves[count], AllMoves);
                Ply++;
                UpdateRepetitionTable(board.Hashkey);
                int score = -Quiescence(board, -beta, -alpha);
                board.UnmakeMove(moves[count], AllMoves);
                board.HalfMoveClock = halfMoveClock;
                board.Castle = castle;
                board.Hashkey = hashkey;
                Ply--;
                RepetitionIndex--;
                if (score > alpha)
                {
                    alpha = score;
                    if (score >= beta) return beta;
                }
            }

            return alpha;
        }
        private bool IsRepetition(ulong hashkey)
        {
            for (int i = 0; i < RepetitionIndex; i++)
            {
                if (RepetitionTable[i] == hashkey)
                {
                    return true;
                }
            }
            return false;
        }
        public int NegaMaxSearch(Board board, int depth, int alpha, int beta)
        {

            int hashFlag = HashFlagAlpha, score;
            PvLength[Ply] = Ply;

            if (Ply > 0 && IsRepetition(board.Hashkey) || board.HalfMoveClock >= 100) return 0;
            int transpositionMove = 0;
            if ((score = TranspositionTable.ReadHashEntry(board.Hashkey, alpha, beta, depth, Ply, ref transpositionMove)) != NoHashEntry)
                return score;
            if (depth == 0) return Quiescence(board, alpha, beta);
            if (Ply > 127) return Evaluation.EvaluatePosition(board);
            Nodes++;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            bool kingInCheck = board.IsInCheck();
            if (EligibleForNullMove(depth, kingInCheck, Ply))
                if (NullMovePruning(board, depth, alpha, beta) >= beta) return beta; // Null move pruning

            if (kingInCheck) depth++; // Check extension
            if (FollowPV) EnablePVScore(moves, moveCount);
            SortMoves(moves, moveCount);
            int movesSearched = 0;
            for (int count = 0; count < moveCount; count++)
            {
                int castle = board.Castle, halfMoveClock = board.HalfMoveClock;
                ulong hashkey = board.Hashkey;
                //Console.WriteLine();
                //Console.WriteLine(board.Hashkey);
                board.MakeMove(moves[count], AllMoves);
                Ply++;
                UpdateRepetitionTable(board.Hashkey);
                if (movesSearched == 0)
                    score = -NegaMaxSearch(board, depth - 1, -beta, -alpha);
                else
                {
                    // Late move reduction condition
                    if (EligibleForLateMoveReduction(moves[count], movesSearched, depth, kingInCheck))
                        score = -NegaMaxSearch(board, depth - 2, -alpha - 1, -alpha);
                    else
                        score = alpha + 1;
                    //Principal Variation Search
                    if (score > alpha)
                        score = PVSearch(board, depth, alpha, beta);
                }
                board.UnmakeMove(moves[count], AllMoves);
                board.HalfMoveClock = halfMoveClock;
                board.Hashkey = hashkey;
                board.Castle = castle;
                //Console.WriteLine(board.Hashkey);
                Ply--;
                RepetitionIndex--;
                movesSearched++;
                if (score >= beta)
                {
                    TranspositionTable.Store(board.Hashkey, moves[count], depth, score, HashFlagBeta, Ply);
                    if (!IsMoveCapture(moves[count]))
                    {
                        KillerMoves[1, Ply] = KillerMoves[0, Ply];
                        KillerMoves[0, Ply] = moves[count];
                    }
                    return beta;
                }
                if (score > alpha)
                {
                    hashFlag = HashFlagExact;
                    if (!IsMoveCapture(moves[count]))
                        HistoryMoves[GetMovePiece(moves[count]), GetMoveTarget(moves[count])] += depth;
                    alpha = score;
                    PvTable[Ply, Ply] = moves[count];
                    for (int next = Ply + 1; next < PvLength[Ply + 1]; next++)
                    {
                        PvTable[Ply, next] = PvTable[Ply + 1, next];
                    }
                    PvLength[Ply] = PvLength[Ply + 1];
                }
            }
            if (moveCount == 0)
            {
                if (kingInCheck) return -MateValue + Ply;
                else return 0;
            }
            TranspositionTable.Store(board.Hashkey, PvTable[Ply, Ply], depth, alpha, hashFlag, Ply);

            return alpha;



        }
        public void EnablePVScore(int[] moves, int moveCount)
        {
            FollowPV = false;
            for (int count = 0; count < moveCount; count++)
            {
                if (moves[count] == PvTable[0, Ply])
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

        public int ScoreMove(int move)
        {
            if (ScorePv == true)
            {
                ScorePv = false;
                if (move == PvTable[0, Ply]) return 20000;
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
                if (KillerMoves[0, Ply] == move) return 9000;
                else if (KillerMoves[1, Ply] == move) return 8000;
                else return HistoryMoves[GetMovePiece(move), GetMoveTarget(move)];
            }
        }
        private bool EligibleForNullMove(int depth, bool kingInCheck, int Ply)
        {
            return depth >= 3 && !kingInCheck && Ply > 0;
        }
        private bool EligibleForLateMoveReduction(int move, int movesSearched, int depth, bool kingInCheck)
        {
            return movesSearched >= FullDepthMoves
                        && depth >= ReductionLimit
                        && !kingInCheck
                        && !IsMoveCapture(move)
                        && GetMovePromoted(move) == 0;

        }
        private int PVSearch(Board board, int depth, int alpha, int beta)
        {
            int score = -NegaMaxSearch(board, depth - 1, -alpha - 1, -alpha);
            if (score > alpha && score < beta)
            {
                score = -NegaMaxSearch(board, depth - 1, -beta, -alpha);
            }
            return score;

        }
        private int NullMovePruning(Board board, int depth, int alpha, int beta)
        {
            ulong hashkey = board.Hashkey;
            Ply++;
            UpdateRepetitionTable(board.Hashkey);
            board.Side ^= 1;
            int castle = board.Castle;
            int enpasant = board.Enpassant;
            if (board.Enpassant != None)
                board.Hashkey ^= board.EnpassantKeys[board.Enpassant];
            board.Hashkey ^= board.SideKey;
            board.Enpassant = None;
            int nullScore = -NegaMaxSearch(board, depth - 2 - 1, -beta, -beta + 1);
            board.Side ^= 1;
            board.Castle = castle;
            board.Enpassant = enpasant;
            Ply--;
            RepetitionIndex--;
            board.Hashkey = hashkey;
            return nullScore;
        }
        private void ClearHelperData()
        {
            Array.Clear(KillerMoves, 0, KillerMoves.Length);
            Array.Clear(HistoryMoves, 0, HistoryMoves.Length);
            Array.Clear(PvLength, 0, PvLength.Length);
            Array.Clear(PvTable, 0, PvTable.Length);
            Array.Clear(RepetitionTable, 0, RepetitionTable.Length);
            TranspositionTable = new TranspositionTable();
            Nodes = 0;
            Ply = 0;
            RepetitionIndex = 1;
            BestMove = 0;
            FollowPV = false;
            ScorePv = false;
        }
    }
}