using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Logic;
using API.Models.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using API.Utils;
using System.Security.Claims;
using API.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
namespace API.Controllers
{
    [Authorize]
    public class GameController : BaseApiController<GameController>
    {
        private readonly GameService _gameService;

        public GameController(ILogger<GameController> logger, GameService gameService) : base(logger)
        {
            _gameService = gameService;

        }

        [HttpPut("make-move")]
        public async Task<ActionResult<MoveDto>> MakeMove(Move move)
        {
            long userId = GetUserId();
            MoveDto moveDto = await _gameService.MakeMove(userId, move);

            return Ok(moveDto);
        }
        [HttpGet]
        public async Task<ActionResult<GameDto>> GetCurrentGame()
        {
            long userId = GetUserId();
            return Ok(await _gameService.GetCurrentGameForPlayer(userId));
        }
        [HttpPost("match-game")]
        public async Task<ActionResult<GameDto>> MatchGame(CreateGameDto createGameDto)
        {
            long userId = GetUserId();
            GameDto gameDto = await _gameService.MatchGame(userId, createGameDto);

            return Ok(gameDto);
        }
        [HttpPost("create-private-game")]
        public async Task<ActionResult<GameDto>> CreatePrivateGame(CreateGameDto createGameDto)
        {
            long userId = GetUserId();
            Game game = await _gameService.CreatePrivateGame(userId, createGameDto);
            var board = new Board(game.Fen);
            var moves = board.GenerateMovesForBothSides();

            return Ok(MapToDto.CreateGameDto(game, board, moves[0], moves[1]));
        }
        [HttpPut("join-private-game")]
        public async Task<ActionResult<GameDto>> JoinPrivateGame(JoinPrivateGameDto joinPrivateGameDto)
        {
            long userId = GetUserId();
            GameDto gameDto = await _gameService.JoinPrivateGame(joinPrivateGameDto.GameId, userId);

            return Ok(gameDto);
        }

        [HttpDelete("delete-game/{gameId}")]
        public async Task<ActionResult> DeleteGame(string gameId)
        {
            long userId = GetUserId();
            await _gameService.DeleteGame(gameId);
            return Ok();
        }

        [HttpPut("resign-game")]
        public async Task<ActionResult> ResignGame()
        {
            long userId = GetUserId();
            bool resigned = await _gameService.ResignGame(userId);
            if (!resigned) return BadRequest("No game to resign.");
            return Ok();
        }
        [HttpPost("create-engine-game")]
        public async Task<ActionResult<GameDto>> CreateEngineGame(CreateEngineGameDto createEngineGameDto)
        {
            long userId = GetUserId();
            Game game = await _gameService.CreateEngineGame(userId, createEngineGameDto);
            var board = new Board(game.Fen);
            var moves = board.GenerateMovesForBothSides();

            return Ok(MapToDto.CreateGameDto(game, board, moves[0], moves[1]));
        }
        private long GetUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }


    }
}