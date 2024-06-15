using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
namespace API.Utils
{
    public class EvaluationHelpers
    {
        public static int[] MaterialScore ={
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
        };
        public static int[] PawnScore =
        {
            90,  90,  90,  90,  90,  90,  90,  90,
            30,  30,  30,  40,  40,  30,  30,  30,
            20,  20,  20,  30,  30,  30,  20,  20,
            10,  10,  10,  20,  20,  10,  10,  10,
            5,   5,  10,  20,  20,   5,   5,   5,
            0,   0,   0,   5,   5,   0,   0,   0,
            0,   0,   0, -10, -10,   0,   0,   0,
            0,   0,   0,   0,   0,   0,   0,   0
        };

        // knight positional score
        public static int[] KnightScore =
        {
            -5,   0,   0,   0,   0,   0,   0,  -5,
            -5,   0,   0,  10,  10,   0,   0,  -5,
            -5,   5,  20,  20,  20,  20,   5,  -5,
            -5,  10,  20,  30,  30,  20,  10,  -5,
            -5,  10,  20,  30,  30,  20,  10,  -5,
            -5,   5,  20,  10,  10,  20,   5,  -5,
            -5,   0,   0,   0,   0,   0,   0,  -5,
            -5, -10,   0,   0,   0,   0, -10,  -5
        };

        // bishop positional score
        public static int[] BishopScore =
        {
            0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   0,  10,  10,   0,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,  10,   0,   0,   0,   0,  10,   0,
            0,  30,   0,   0,   0,   0,  30,   0,
            0,   0, -10,   0,   0, -10,   0,   0

        };

        // rook positional score
        public static int[] RookScore =
        {
            50,  50,  50,  50,  50,  50,  50,  50,
            50,  50,  50,  50,  50,  50,  50,  50,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,  10,  20,  20,  10,   0,   0,
            0,   0,   0,  20,  20,   0,   0,   0

        };

        // king positional score
        public static int[] KingScore =
        {
            0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   5,   5,   5,   5,   0,   0,
            0,   5,   5,  10,  10,   5,   5,   0,
            0,   5,  10,  20,  20,  10,   5,   0,
            0,   5,  10,  20,  20,  10,   5,   0,
            0,   0,   5,  10,  10,   5,   0,   0,
            0,   5,   5,  -5,  -5,   0,   5,   0,
            0,   0,   5,   0, -15,   0,  10,   0
        };

        // mirror positional score tables for opposite side
        public static int[] MirrorScore =
        {
            a1, b1, c1, d1, e1, f1, g1, h1,
            a2, b2, c2, d2, e2, f2, g2, h2,
            a3, b3, c3, d3, e3, f3, g3, h3,
            a4, b4, c4, d4, e4, f4, g4, h4,
            a5, b5, c5, d5, e5, f5, g5, h5,
            a6, b6, c6, d6, e6, f6, g6, h6,
            a7, b7, c7, d7, e7, f7, g7, h7,
            a8, b8, c8, d8, e8, f8, g8, h8
        };
    }
}