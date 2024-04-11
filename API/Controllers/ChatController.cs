using API.Models.Dtos;
using API.Models.Entities;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ChatController : BaseApiController<ChatController>
    {
        private readonly ChatService _chatService;
        private readonly IMapper _mapper;
        public ChatController(ILogger<ChatController> logger, ChatService chatService, IMapper mapper) : base(logger)
        {
            _chatService = chatService;
            _mapper = mapper;
        }
        [HttpGet("{gameId}")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetChatMessagesByGameId(string gameId)
        {

            return Ok(await _chatService.GetChatMessagesByGameIdAsync(gameId));
        }

        [HttpPost]
        public async Task<ActionResult<ChatDto>> AddChatMessage(AddMessageDto addChatMessage)
        {
            var chatMessage = _mapper.Map<ChatMessage>(addChatMessage);
            await _chatService.AddChatMessageAsync(chatMessage);
            return Ok();
        }
    }
}