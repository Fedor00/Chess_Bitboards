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
        private bool TryParseMove(string move, out int from, out int to, out int promotedPiece)
        {
            from = None;
            to = None;
            promotedPiece = 0;
            if (string.IsNullOrWhiteSpace(move) || move.Length < 4 || move.Length > 5)
            {
                Console.WriteLine("Invalid move format.");
                return false;
            }

            int sourceFile = move[0] - 'a';
            int sourceRank = 8 - (move[1] - '1') - 1;
            int targetFile = move[2] - 'a';
            int targetRank = 8 - (move[3] - '1') - 1;

            from = sourceRank * 8 + sourceFile;
            to = targetRank * 8 + targetFile;
            if (move.Length == 5)
            {
                char promotionChar = move[4];
                if (CharToPiece.TryGetValue(promotionChar, out int piece)) promotedPiece = piece;
                else { Console.WriteLine("Invalid promotion piece."); return false; };

            }

            return true;
        }
        public async Task MakeEngineMove(IChessEngine chessEngine, Game game, Board board)
        {
            string move = chessEngine.GetBestMoveAsync(game.Fen).Result;
            if (move == null) throw new InvalidOperationException("Engine could not find a move.");
            //parse the move from the engine given in format e2e4 or e7e8q
            TryParseMove(move, out int from, out int to, out int promotedPiece);
            //promted piece is always lower case, so adjust it for the other color
            if (promotedPiece != 0)
                promotedPiece = board.Side == Black ? promotedPiece : promotedPiece - 6;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            //find the move in the list of legal moves
            int moveToMake = moves.SingleOrDefault(m =>
                    GetMoveSource(m) == from &&
                    GetMoveTarget(m) == to &&
                    (promotedPiece == 0 || GetMovePromoted(m) == promotedPiece));
            //if the move is not found in the list of legal moves, throw an exception
            if (moveToMake == 0) throw new InvalidOperationException("Engine move is not legal.");
            board.MakeMove(moveToMake, AllMoves);
            game.Fen = Fen.UpdateFen(board);
            var bothSideMoves = board.GenerateMovesForBothSides();
            //get the status of the game (check, checkmate, stalemate, draw)
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
            //send the move to the client
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("MoveMade", moveDto);

            await _gameRepository.UpdateGame(game);

        }



    }
}