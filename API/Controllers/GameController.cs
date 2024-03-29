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
        private readonly IMapper _mapper;
        public GameController(ILogger<GameController> logger, GameService gameService, IMapper mapper) : base(logger)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        [HttpPut("make-move")]
        public async Task<ActionResult<MoveDto>> MakeMove(Move move)
        {
            long userId = GetUserId();
            MoveDto moveDto = await _gameService.MakeMove(userId, move);

            return Ok(moveDto);
        }
        [HttpGet]
        public async Task<ActionResult<GameDto>> GetGame()
        {
            long userId = GetUserId();
            Game game = await _gameService.GetGame(userId);
            GameDto gameDto = _gameService.GetGameDto(game);
            return Ok(gameDto);
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
            GameDto gameDto = _gameService.GetGameDto(game);
            return Ok(gameDto);
        }
        [HttpPut("join-private-game")]
        public async Task<ActionResult<GameDto>> JoinPrivateGame(JoinPrivateGameDto joinPrivateGameDto)
        {
            long userId = GetUserId();
            GameDto gameDto = await _gameService.JoinPrivateGame(joinPrivateGameDto.GameId, userId);

            return Ok(gameDto);
        }

        [HttpDelete("delete-game")]
        public async Task<ActionResult> DeleteGame(string gameId)
        {
            long userId = GetUserId();
            await _gameService.DeleteGame(gameId);
            return Ok();
        }
        private long GetUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

    }
}