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

        [HttpPost]
        public ActionResult<GameDto> MakeMove(Move move)
        {

            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult<GameDto>> GetGame()
        {
            long userId = GetUserId();
            Game game = await _gameService.GetGame(userId);
            return Ok(_mapper.Map<GameDto>(game));
        }
        private long GetUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

    }
}