using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
namespace API.Utils
{
    public static class Mask
    {
        static Mask()
        {
            InitializePath();
            InitializeBishopMask();
            InitializeRookMask();
            InitializeRankMasks();
            InitializeFileMasks();
            InitialzePawnMoves();
            InitializeIsolatedPawnMasks();
            InitializePassedPawnMasks();
        }
        public static ulong[] WhitePassedPawnMasks = new ulong[64];
        public static ulong[] BlackPassedPawnMasks = new ulong[64];
        public static ulong[] IsolatedPawnMask = new ulong[64];
        public static readonly ulong[][] PawnAttacks = new ulong[2][];
        public static readonly ulong[][] PawnSingleMoves = new ulong[2][];
        public static readonly ulong[][] PawnDoubleMoves = new ulong[2][];
        public static ulong[] RankMask = new ulong[64];
        public static ulong[] FileMask = new ulong[64];
        public static readonly ulong[][] Path = new ulong[64][];

        public static readonly ulong[] FullRookMasks = new ulong[64];
        public static readonly ulong[] FullBishopMasks = new ulong[64];
        public static void InitialzePawnMoves()
        {
            PawnAttacks[0] = new ulong[64];
            PawnAttacks[1] = new ulong[64];
            PawnSingleMoves[0] = new ulong[64];
            PawnSingleMoves[1] = new ulong[64];
            PawnDoubleMoves[0] = new ulong[64];
            PawnDoubleMoves[1] = new ulong[64];
            InitializeWhitePawnSingleMove();
            InitializeBlackPawnSingleMoves();
            InitializeWhitePawnDoubleMoves();
            InitializeBlackPawnDoubleMoves();
            InitializeWhitePawnAttacks();
            InitializeBlackPawnAttacks();

        }
        private static void InitializeIsolatedPawnMasks()
        {
            for (int square = 0; square < 64; square++)
            {
                int rank = square / 8;
                int file = square % 8;
                ulong mask = 0UL;
                if (file > 0)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        mask |= 1UL << (r * 8 + file - 1);
                    }
                }
                if (file < 7)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        mask |= 1UL << (r * 8 + file + 1);
                    }
                }

                IsolatedPawnMask[square] = mask;
            }
        }
        private static void InitializeRankMasks()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                ulong mask = 0UL;
                for (int file = 0; file < 8; file++)
                {
                    int bitPosition = rank * 8 + file;
                    mask |= 1UL << bitPosition;
                }
                for (int file = 0; file < 8; file++)
                {
                    RankMask[rank * 8 + file] = mask;
                }
            }
        }
        private static void InitializePassedPawnMasks()
        {
            for (int square = 0; square < 64; square++)
            {
                int rank = square / 8;  // Calculates the rank from 0 (a8) to 7 (a1)
                int file = square % 8;

                ulong whiteMask = 0UL;
                ulong blackMask = 0UL;

                // Calculate masks for white passed pawns (moving upwards from a1 to a8)
                for (int r = rank + 1; r < 8; r++)
                {
                    int currentSquare = r * 8 + file;
                    whiteMask |= 1UL << currentSquare;  // Current file ahead for white

                    if (file > 0)  // Left adjacent file (from white's perspective)
                        whiteMask |= 1UL << (currentSquare - 1);
                    if (file < 7)  // Right adjacent file
                        whiteMask |= 1UL << (currentSquare + 1);
                }

                // Calculate masks for black passed pawns (moving downwards from h8 to h1)
                for (int r = rank - 1; r >= 0; r--)
                {
                    int currentSquare = r * 8 + file;
                    blackMask |= 1UL << currentSquare;  // Current file behind for black

                    if (file > 0)  // Left adjacent file (from black's perspective)
                        blackMask |= 1UL << (currentSquare - 1);
                    if (file < 7)  // Right adjacent file
                        blackMask |= 1UL << (currentSquare + 1);
                }

                WhitePassedPawnMasks[square] = whiteMask;
                BlackPassedPawnMasks[square] = blackMask;
            }
        }

        private static void InitializeFileMasks()
        {
            for (int file = 0; file < 8; file++)
            {
                ulong mask = 0UL;
                for (int rank = 0; rank < 8; rank++)
                {
                    int bitPosition = rank * 8 + file;
                    mask |= 1UL << bitPosition;
                }
                for (int rank = 0; rank < 8; rank++)
                {
                    FileMask[rank * 8 + file] = mask;
                }
            }
        }




        public static readonly ulong[] RookMasks = new ulong[]
{
    0x000101010101017Eul, 0x000202020202027Cul, 0x000404040404047Aul, 0x0008080808080876ul,
    0x001010101010106Eul, 0x002020202020205Eul, 0x004040404040403Eul, 0x008080808080807Eul,
    0x0001010101017E00ul, 0x0002020202027C00ul, 0x0004040404047A00ul, 0x0008080808087600ul,
    0x0010101010106E00ul, 0x0020202020205E00ul, 0x0040404040403E00ul, 0x0080808080807E00ul,
    0x00010101017E0100ul, 0x00020202027C0200ul, 0x00040404047A0400ul, 0x0008080808760800ul,
    0x00101010106E1000ul, 0x00202020205E2000ul, 0x00404040403E4000ul, 0x00808080807E8000ul,
    0x000101017E010100ul, 0x000202027C020200ul, 0x000404047A040400ul, 0x0008080876080800ul,
    0x001010106E101000ul, 0x002020205E202000ul, 0x004040403E404000ul, 0x008080807E808000ul,
    0x0001017E01010100ul, 0x0002027C02020200ul, 0x0004047A04040400ul, 0x0008087608080800ul,
    0x0010106E10101000ul, 0x0020205E20202000ul, 0x0040403E40404000ul, 0x0080807E80808000ul,
    0x00017E0101010100ul, 0x00027C0202020200ul, 0x00047A0404040400ul, 0x0008760808080800ul,
    0x00106E1010101000ul, 0x00205E2020202000ul, 0x00403E4040404000ul, 0x00807E8080808000ul,
    0x007E010101010100ul, 0x007C020202020200ul, 0x007A040404040400ul, 0x0076080808080800ul,
    0x006E101010101000ul, 0x005E202020202000ul, 0x003E404040404000ul, 0x007E808080808000ul,
    0x7E01010101010100ul, 0x7C02020202020200ul, 0x7A04040404040400ul, 0x7608080808080800ul,
    0x6E10101010101000ul, 0x5E20202020202000ul, 0x3E40404040404000ul, 0x7E80808080808000ul
};

        public static readonly ulong[] BishopMasks = new ulong[]
 {
    0x0040201008040200ul, 0x0000402010080400ul, 0x0000004020100A00ul, 0x0000000040221400ul,
    0x0000000002442800ul, 0x0000000204085000ul, 0x0000020408102000ul, 0x0002040810204000ul,
    0x0020100804020000ul, 0x0040201008040000ul, 0x00004020100A0000ul, 0x0000004022140000ul,
    0x0000000244280000ul, 0x0000020408500000ul, 0x0002040810200000ul, 0x0004081020400000ul,
    0x0010080402000200ul, 0x0020100804000400ul, 0x004020100A000A00ul, 0x0000402214001400ul,
    0x0000024428002800ul, 0x0002040850005000ul, 0x0004081020002000ul, 0x0008102040004000ul,
    0x0008040200020400ul, 0x0010080400040800ul, 0x0020100A000A1000ul, 0x0040221400142200ul,
    0x0002442800284400ul, 0x0004085000500800ul, 0x0008102000201000ul, 0x0010204000402000ul,
    0x0004020002040800ul, 0x0008040004081000ul, 0x00100A000A102000ul, 0x0022140014224000ul,
    0x0044280028440200ul, 0x0008500050080400ul, 0x0010200020100800ul, 0x0020400040201000ul,
    0x0002000204081000ul, 0x0004000408102000ul, 0x000A000A10204000ul, 0x0014001422400000ul,
    0x0028002844020000ul, 0x0050005008040200ul, 0x0020002010080400ul, 0x0040004020100800ul,
    0x0000020408102000ul, 0x0000040810204000ul, 0x00000A1020400000ul, 0x0000142240000000ul,
    0x0000284402000000ul, 0x0000500804020000ul, 0x0000201008040200ul, 0x0000402010080400ul,
    0x0002040810204000ul, 0x0004081020400000ul, 0x000A102040000000ul, 0x0014224000000000ul,
    0x0028440200000000ul, 0x0050080402000000ul, 0x0020100804020000ul, 0x0040201008040200ul
 };
        public static readonly ulong[] KnightMasks = new ulong[]{
            132096ul,
            329728ul,
            659712ul,
            1319424ul,
            2638848ul,
            5277696ul,
            10489856ul,
            4202496ul,
            33816580ul,
            84410376ul,
            168886289ul,
            337772578ul,
            675545156ul,
            1351090312ul,
            2685403152ul,
            1075839008ul,
            8657044482ul,
            21609056261ul,
            43234889994ul,
            86469779988ul,
            172939559976ul,
            345879119952ul,
            687463207072ul,
            275414786112ul,
            2216203387392ul,
            5531918402816ul,
            11068131838464ul,
            22136263676928ul,
            44272527353856ul,
            88545054707712ul,
            175990581010432ul,
            70506185244672ul,
            567348067172352ul,
            1416171111120896ul,
            2833441750646784ul,
            5666883501293568ul,
            11333767002587136ul,
            22667534005174272ul,
            45053588738670592ul,
            18049583422636032ul,
            145241105196122112ul,
            362539804446949376ul,
            725361088165576704ul,
            1450722176331153408ul,
            2901444352662306816ul,
            5802888705324613632ul,
            11533718717099671552ul,
            4620693356194824192ul,
            288234782788157440ul,
            576469569871282176ul,
            1224997833292120064ul,
            2449995666584240128ul,
            4899991333168480256ul,
            9799982666336960512ul,
            1152939783987658752ul,
            2305878468463689728ul,
            1128098930098176ul,
            2257297371824128ul,
            4796069720358912ul,
            9592139440717824ul,
            19184278881435648ul,
            38368557762871296ul,
            4679521487814656ul,
            9077567998918656ul,
    };
        public static readonly ulong[] KingMasks = new ulong[]
        {
            770ul,
            1797ul,
            3594ul,
            7188ul,
            14376ul,
            28752ul,
            57504ul,
            49216ul,
            197123ul,
            460039ul,
            920078ul,
            1840156ul,
            3680312ul,
            7360624ul,
            14721248ul,
            12599488ul,
            50463488ul,
            117769984ul,
            235539968ul,
            471079936ul,
            942159872ul,
            1884319744ul,
            3768639488ul,
            3225468928ul,
            12918652928ul,
            30149115904ul,
            60298231808ul,
            120596463616ul,
            241192927232ul,
            482385854464ul,
            964771708928ul,
            825720045568ul,
            3307175149568ul,
            7718173671424ul,
            15436347342848ul,
            30872694685696ul,
            61745389371392ul,
            123490778742784ul,
            246981557485568ul,
            211384331665408ul,
            846636838289408ul,
            1975852459884544ul,
            3951704919769088ul,
            7903409839538176ul,
            15806819679076352ul,
            31613639358152704ul,
            63227278716305408ul,
            54114388906344448ul,
            216739030602088448ul,
            505818229730443264ul,
            1011636459460886528ul,
            2023272918921773056ul,
            4046545837843546112ul,
            8093091675687092224ul,
            16186183351374184448ul,
            13853283560024178688ul,
            144959613005987840ul,
            362258295026614272ul,
            724516590053228544ul,
            1449033180106457088ul,
            2898066360212914176ul,
            5796132720425828352ul,
            11592265440851656704ul,
            4665729213955833856ul,
        };
        public static readonly ulong[] WhitePawnSingleMoves = new ulong[64];
        public static readonly ulong[] WhitePawnDoubleMoves = new ulong[64];
        public static readonly ulong[] WhitePawnAttacks = new ulong[64];


        public static readonly ulong[] BlackPawnSingleMoves = new ulong[64];
        public static readonly ulong[] BlackPawnDoubleMoves = new ulong[64];

        public static readonly ulong[] BlackPawnAttacks = new ulong[64];


        public static void InitializeBlackPawnAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                int leftAttackPosition = i + 7;
                int rightAttackPosition = i + 9;

                if (i % 8 != 0 && leftAttackPosition < 64)
                {
                    PawnAttacks[Black][i] |= 1UL << leftAttackPosition;
                }
                if (i % 8 != 7 && rightAttackPosition < 64)
                {
                    PawnAttacks[Black][i] |= 1UL << rightAttackPosition;
                }
            }
        }
        public static void InitializeWhitePawnAttacks()
        {
            for (int i = 0; i < 64; i++)
            {
                int leftAttackPosition = i - 9;
                int rightAttackPosition = i - 7;
                if (i % 8 != 0 && leftAttackPosition >= 0)
                {
                    PawnAttacks[White][i] |= 1UL << leftAttackPosition;
                }
                if (i % 8 != 7 && rightAttackPosition >= 0)
                {
                    PawnAttacks[White][i] |= 1UL << rightAttackPosition;
                }
            }
        }
        public static void InitializeBlackPawnDoubleMoves()
        {
            for (int i = 8; i <= 15; i++)
            {
                int doubleMovePosition = i + 16;
                PawnDoubleMoves[Black][i] = 1UL << doubleMovePosition;
            }
        }
        public static void InitializeWhitePawnDoubleMoves()
        {
            for (int i = 48; i <= 55; i++)
            {
                int doubleMovePosition = i - 16;
                PawnDoubleMoves[White][i] = 1UL << doubleMovePosition;
            }
        }

        public static void InitializeWhitePawnSingleMove()
        {
            for (int i = 8; i <= 55; i++)
            {
                int nextPosition = i - 8;
                PawnSingleMoves[White][i] = 1UL << nextPosition;
            }
        }
        public static void InitializeBlackPawnSingleMoves()
        {
            for (int i = 8; i <= 55; i++)
            {
                int nextPosition = i + 8;
                PawnSingleMoves[Black][i] = 1UL << nextPosition;
            }
        }


        public static void InitializePath()
        {
            for (int startSquare = 0; startSquare < 64; startSquare++)
            {
                Path[startSquare] = new ulong[64];
                for (int endSquare = 0; endSquare < 64; endSquare++)
                {

                    Path[startSquare][endSquare] = 0UL;


                    int startRank = startSquare / 8, startFile = startSquare % 8;
                    int endRank = endSquare / 8, endFile = endSquare % 8;


                    int rankDirection = Math.Sign(endRank - startRank);
                    int fileDirection = Math.Sign(endFile - startFile);


                    if (startRank == endRank || startFile == endFile || Math.Abs(startRank - endRank) == Math.Abs(startFile - endFile))
                    {

                        int currentRank = startRank + rankDirection;
                        int currentFile = startFile + fileDirection;


                        while (currentRank != endRank || currentFile != endFile)
                        {
                            int currentIndex = currentRank * 8 + currentFile;
                            Path[startSquare][endSquare] |= 1UL << currentIndex;

                            currentRank += rankDirection;
                            currentFile += fileDirection;
                        }


                    }
                }
            }
        }

        public static void InitializeBishopMask()
        {
            for (int square = 0; square < 64; square++)
            {
                ulong mask = 0UL;
                int rank = square / 8, file = square % 8;
                for (int r = rank + 1, f = file + 1; r <= 7 && f <= 7; r++, f++)
                    mask |= 1UL << (r * 8 + f);
                for (int r = rank + 1, f = file - 1; r <= 7 && f >= 0; r++, f--)
                    mask |= 1UL << (r * 8 + f);
                for (int r = rank - 1, f = file + 1; r >= 0 && f <= 7; r--, f++)
                    mask |= 1UL << (r * 8 + f);
                for (int r = rank - 1, f = file - 1; r >= 0 && f >= 0; r--, f--)
                    mask |= 1UL << (r * 8 + f);

                FullBishopMasks[square] = mask;
            }
        }
        public static void InitializeRookMask()
        {
            for (int square = 0; square < 64; square++)
            {
                ulong mask = 0UL;
                int rank = square / 8, file = square % 8;
                for (int r = rank + 1; r <= 7; r++) // Up
                    mask |= 1UL << (r * 8 + file);
                for (int r = rank - 1; r >= 0; r--) // Down
                    mask |= 1UL << (r * 8 + file);
                for (int f = file + 1; f <= 7; f++) // Right
                    mask |= 1UL << (rank * 8 + f);
                for (int f = file - 1; f >= 0; f--) // Left
                    mask |= 1UL << (rank * 8 + f);

                FullRookMasks[square] = mask;
            }
        }


    }
}