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
namespace API.Controllers
{
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
        public async Task<ActionResult<GameDto>> MakeMove(Move move)
        {
            long userId = GetUserId();
            Game game = await _gameService.MakeMove(userId, move);
            GameDto gameDto = _gameService.GetGameDto(game);
            return Ok(gameDto);
        }
        [HttpGet]
        public async Task<ActionResult<GameDto>> GetGame()
        {
            long userId = GetUserId();
            Game game = await _gameService.GetGame(userId);
            return Ok(_mapper.Map<GameDto>(game));
        }
        [HttpPost("join-game")]
        public async Task<ActionResult<GameDto>> CreateOrJoinGame()
        {
            long userId = GetUserId();
            Console.WriteLine("User id: " + userId);
            Game game = await _gameService.CreateOrJoinGame(userId);
            GameDto gameDto = _gameService.GetGameDto(game);
            return Ok(gameDto);
        }
        private long GetUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

    }
}