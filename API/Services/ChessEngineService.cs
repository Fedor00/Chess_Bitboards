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
    public class ChessEngineService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IHubContext<ChessHub> _hubContext;


        public ChessEngineService(IGameRepository gameRepository,

                                IChessEngineRepository chessEngineRepository,
                                IHubContext<ChessHub> hubContext, IServiceProvider serviceProvider)
        {

            _gameRepository = gameRepository;
            _hubContext = hubContext;
        }
        public async Task MakeEngineMove(IChessEngine chessEngine, Game game, Board board)
        {
            string move = chessEngine.GetBestMoveAsync(game.Fen).Result;
            Console.WriteLine(move);
            if (move == null) throw new InvalidOperationException("Engine could not find a move.");
            int sourceFile = move[0] - 'a';
            int sourceRank = 8 - (move[1] - '1') - 1;
            int targetFile = move[2] - 'a';
            int targetRank = 8 - (move[3] - '1') - 1;
            int from = sourceRank * 8 + sourceFile;
            int to = targetRank * 8 + targetFile;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            int moveToMake = moves.SingleOrDefault(m => GetMoveSource(m) == from && GetMoveTarget(m) == to);

            if (moveToMake == 0) throw new InvalidOperationException("Engine move is not legal.");
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
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("MoveMade", moveDto);

            await _gameRepository.UpdateGame(game);

        }



    }
}