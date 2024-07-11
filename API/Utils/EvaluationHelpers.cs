using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
namespace API.Utils
{
    public class EvaluationHelpers
    {
        public static int HashFlagExact = 0;
        public static int HashFlagAlpha = 1;
        public static int HashFlagBeta = 2;
        public static int[] MaterialScore = [
            100,   // White pawn 
            10000, // White king 
            350,   // White bishop
            500,   // White rook
            1000,  // White queen
            300,   // White knight
            -100,  // black pawn
            -10000,// black king
            -350,  // black bishop
            -500,  // black rook
            -1000, // black queen
            -300   // black knight
        ];
        public static int[] PawnScore =
        {
            0,  10,  10,  10,  10,  10,  10,   0,
            5,  10,  20,  30,  30,  20,  10,   5,
            5,  10,  15,  20,  20,  15,  10,   5,
            5,  10,  10,  20,  20,  10,  10,   5,
            5,   5,  10,  20,  20,  10,   5,   5,
            5,   0,   5,  10,  10,   5,   0,   5,
            5,   5,   5,  -5,  -5,   5,   5,   5,
            0,   0,   0,   0,   0,   0,   0,   0
        };


        // knight positional score
        public static int[] KnightScore =
        [
            -5,   0,   0,   0,   0,   0,   0,  -5,
            -5,   0,   0,  10,  10,   0,   0,  -5,
            -5,   5,  20,  20,  20,  20,   5,  -5,
            -5,  10,  20,  30,  30,  20,  10,  -5,
            -5,  10,  20,  30,  30,  20,  10,  -5,
            -5,   5,  20,  10,  10,  20,   5,  -5,
            -5,   0,   0,   0,   0,   0,   0,  -5,
            -5, -10,   0,   0,   0,   0, -10,  -5
        ];

        // bishop positional score
        public static int[] BishopScore =
        [
            0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   0,   0,   0,   0,   0,   0,
            0,  20,   0,  10,  10,   0,  20,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,  10,   0,   0,   0,   0,  10,   0,
            0,  30,   0,   0,   0,   0,  30,   0,
            0,   0, -10,   0,   0, -10,   0,   0

        ];

        // rook positional score
        public static int[] RookScore =
        {
            15,  15,  15,  15,  15,  15,  15,  15,
            10,  10,  10,  10,  10,  10,  10,  10,
            5,   5,  10,  15,  15,  10,   5,   5,
            0,   0,   5,  10,  10,   5,   0,   0,
            0,   0,   5,  10,  10,   5,   0,   0,
            0,   0,   5,  10,  10,   5,   0,   0,
            5,   5,  10,  20,  20,  10,   5,   5,
            10,  10,  10,  20,  20,  10,  10,  10
        };


        // king positional score
        public static int[] KingScore =
        [
            0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   5,   5,   5,   5,   0,   0,
            0,   5,   5,  10,  10,   5,   5,   0,
            0,   5,  10,  20,  20,  10,   5,   0,
            0,   5,  10,  20,  20,  10,   5,   0,
            0,   0,   5,  10,  10,   5,   0,   0,
            0,   5,   5,  -5,  -5,   0,   5,   0,
            0,   0,   5,   0, -15,   0,  10,   0
        ];

        // mirror positional score tables for opposite side
        public static int[] MirrorScore =
        [
            a1, b1, c1, d1, e1, f1, g1, h1,
            a2, b2, c2, d2, e2, f2, g2, h2,
            a3, b3, c3, d3, e3, f3, g3, h3,
            a4, b4, c4, d4, e4, f4, g4, h4,
            a5, b5, c5, d5, e5, f5, g5, h5,
            a6, b6, c6, d6, e6, f6, g6, h6,
            a7, b7, c7, d7, e7, f7, g7, h7,
            a8, b8, c8, d8, e8, f8, g8, h8
        ];

        // victim attacker value matrix
        public static int[][] VictimAttackerValueMatrix =
        [
            // White pieces (P, K, B, R, Q, N)
            [105, 605, 305, 405, 505, 205,  105, 605, 305, 405, 505, 205],
            [100, 600, 300, 400, 500, 200,  100, 600, 300, 400, 500, 200],
            [103, 603, 303, 403, 503, 203,  103, 603, 303, 403, 503, 203],
            [102, 602, 302, 402, 502, 202,  102, 602, 302, 402, 502, 202],
            [101, 601, 301, 401, 501, 201,  101, 601, 301, 401, 501, 201],
            [104, 604, 304, 404, 504, 204,  104, 604, 304, 404, 504, 204],

            // Black pieces (p, k, b, r, q, n)
            [105, 605, 305, 405, 505, 205,  105, 605, 305, 405, 505, 205],
            [100, 600, 300, 400, 500, 200,  100, 600, 300, 400, 500, 200],
            [103, 603, 303, 403, 503, 203,  103, 603, 303, 403, 503, 203],
            [102, 602, 302, 402, 502, 202,  102, 602, 302, 402, 502, 202],
            [101, 601, 301, 401, 501, 201,  101, 601, 301, 401, 501, 201],
            [104, 604, 304, 404, 504, 204,  104, 604, 304, 404, 504, 204]
        ];

        public static int DoublePawnPenalty = -10;
        public static int IsolatedPawnPenalty = -10;
        public static int[] PassedPawnBonus = [0, 5, 10, 20, 35, 60, 100, 200];
        public static int OnOpenFileBonus = 15;
        public static int OnSemiOpenFileBonus = 10;
        public static int KingShieldBonus = 5;
        public static int MateScore = 48000;
        public static int MateValue = 49000;
        public static int Infinity = 50000;
    }
}
