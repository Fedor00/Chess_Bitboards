using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
namespace API.Utils
{
    public static class ChessHelpers
    {

        public static readonly string[] Coordinates =
        [
            "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
            "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
            "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
            "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
            "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
            "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
            "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
            "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1",
        ];
        public static readonly int[] CastlingRights = new int[64]
        {
            7, 15, 15, 15, 3, 15, 15, 11,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            13, 15, 15, 15, 12, 15, 15, 14,
        };
        public static readonly char[] Unicodes = new char[]
        {
            '\u2659',
            '\u2654',
            '\u2657',
            '\u2656',
            '\u2655',
            '\u2658',
            '\u265F',
            '\u265A',
            '\u265D',
            '\u265C',
            '\u265B',
            '\u265E'
        };
        public static readonly string AsciiPiece = "PKBRQNpkbrqn";
        public static string INITIAL_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static readonly Dictionary<char, int> CharToPiece = new()
        {
            { 'P', P },
            { 'N', N },
            { 'B', B },
            { 'R', R },
            { 'Q', Q },
            { 'K', K },
            { 'p', p },
            { 'n', n },
            { 'b', b },
            { 'r', r },
            { 'q', q },
            { 'k', k },
        };
        public static readonly Dictionary<int, char> PieceToChar = new()
        {
            { P, 'P' },
            { N, 'N' },
            { B, 'B' },
            { R, 'R' },
            { Q, 'Q' },
            { K, 'K' },
            { p, 'p' },
            { n, 'n' },
            { b, 'b' },
            { r, 'r' },
            { q, 'q' },
            { k, 'k' },
        };
        public static readonly string Playing = "playing";
        public static readonly string Waiting = "waiting";
        public static readonly string Draw = "draw";
        public static readonly string Stalemate = "stalemate";
        public static readonly string WhiteWin = "white-won";
        public static readonly string BlackWin = "black-won";
        public static readonly string WhiteResign = "white-resigned";
        public static readonly string BlackResign = "black-resigned";
        public const string StockfishEngineName = "stockfish";
        public const string DummyEngineName = "dummy";



    }

}