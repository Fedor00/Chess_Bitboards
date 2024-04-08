using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using API.Interfaces;
using API.Logic;
using API.Models.Dtos;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Stockfish.NET;
using static API.Utils.ChessHelpers;
using static API.Utils.BitbboardUtils;
using Microsoft.AspNetCore.SignalR;
using API.Hubs;
using System.Diagnostics;
using DeviceMicroservice.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using API.Repository;
namespace API.Services
{
    public class StockfishService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IChessEngineRepository _chessEngineRepository;
        private readonly IHubContext<ChessHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        private Process _stockfishProcess;

        public StockfishService(IGameRepository gameRepository,

                                IChessEngineRepository chessEngineRepository,
                                IHubContext<ChessHub> hubContext, IServiceProvider serviceProvider)
        {
            _stockfishProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\fedor\OneDrive\Documents\Personal\Chess\Stockfish\stockfish-windows-x86-64.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            _stockfishProcess.Start();
            _gameRepository = gameRepository;
            _chessEngineRepository = chessEngineRepository;
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }



        private async Task MakeStockfishMove(Game game, long playerId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var gameRepo = scope.ServiceProvider.GetRequiredService<GameRepository>();

                string move = GetBestMoveFromStockfish(game.Fen);
                Console.WriteLine(move);
                if (move == null) throw new InvalidOperationException("Stockfish could not find a move.");
                int sourceFile = move[0] - 'a';
                int sourceRank = 8 - (move[1] - '1') - 1;
                int targetFile = move[2] - 'a';
                int targetRank = 8 - (move[3] - '1') - 1;
                int from = sourceRank * 8 + sourceFile;
                int to = targetRank * 8 + targetFile;
                Console.WriteLine(from);
                Console.WriteLine(to);

                Board board = new(game.Fen);
                int moveCount = 0;
                int[] moves = board.GenerateLegalMoves(ref moveCount);
                int moveToMake = moves.SingleOrDefault(m => GetMoveSource(m) == from && GetMoveTarget(m) == to);

                if (moveToMake == 0) throw new InvalidOperationException("Stockfish move is not legal.");
                board.MakeMove(moveToMake, AllMoves);
                game.Fen = Fen.UpdateFen(board);
                var bothSideMoves = board.GenerateMovesForBothSides();
                var check = board.IsInCheck();
                var outOfMoves = bothSideMoves[0].Count() == 0;
                var checkMate = check && outOfMoves;
                var stalemate = !check && outOfMoves;
                var draw = board.IsDraw();
                if (draw) { game.Status = Draw; }
                if (checkMate) { game.Status = board.Side == White ? BlackWin : WhiteWin; }
                if (stalemate) { game.Status = Stalemate; }
                GameDto gameDto = MapToDto.CreateGameDto(game, board, bothSideMoves[0], bothSideMoves[1]);
                MoveDto moveDto = MapToDto.CreateMoveDto(gameDto, moveToMake, check, checkMate, stalemate, draw);
                await _hubContext.Clients.User(game.FirstPlayer.ToString()).SendAsync("MoveMade", moveDto);

                gameRepo.UpdateGame(game);
                await gameRepo.SaveChangesAsync();
            }
        }

        private string GetBestMoveFromStockfish(string fen)
        {
            if (!_stockfishProcess.StartInfo.FileName.EndsWith("exe") || !_stockfishProcess.Start())
            {
                throw new InvalidOperationException("Failed to start Stockfish process.");
            }
            _stockfishProcess.StandardInput.WriteLine($"position fen {fen}");
            _stockfishProcess.StandardInput.WriteLine("go movetime 2000");

            string output;
            string bestMoveLine = "bestmove";
            while ((output = _stockfishProcess.StandardOutput.ReadLine()) != null)
            {
                if (output.StartsWith(bestMoveLine))
                {
                    return output.Replace(bestMoveLine, "").Trim().Split(' ')[0];
                }
            }
            throw new InvalidOperationException("Stockfish did not return a best move.");
        }


    }
}