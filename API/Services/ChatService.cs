using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Hubs;
using API.Interfaces;
using API.Models.Dtos;
using API.Models.Entities;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class ChatService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly ILogger<ChatService> _logger;
        private readonly IHubContext<ChessHub> _hubContext;

        private readonly IGameRepository _gameRepository;
        public ChatService(IChatMessageRepository chatMessageRepository, ILogger<ChatService> logger, IGameRepository gameRepository, IHubContext<ChessHub> hubContext)
        {
            _chatMessageRepository = chatMessageRepository;
            _logger = logger;
            _gameRepository = gameRepository;
            _hubContext = hubContext;
        }

        public async Task<ChatDto> GetChatMessagesByGameIdAsync(string gameId)
        {
            var chatMessages = await _chatMessageRepository.GetChatMessagesByGameIdAsync(gameId);
            var game = await _gameRepository.GetGameAsync(gameId);
            return new ChatDto
            {
                FirstPlayer = game.FirstPlayer,
                SecondPlayer = game.SecondPlayer,
                ChatMessages = chatMessages
            };
        }
        public async Task AddChatMessageAsync(ChatMessage chatMessage)
        {
            Console.WriteLine("AddChatMessageAsync");
            chatMessage.Timestamp = DateTime.UtcNow;
            int result = await _chatMessageRepository.Add(chatMessage);
            if (result > 0)
            {
                Game game = await _gameRepository.GetGameAsync(chatMessage.GameId);

                await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("ReceiveMessage", chatMessage);
                await _hubContext.Clients.User(game.SecondPlayerId.ToString()).SendAsync("ReceiveMessage", chatMessage);

            }
        }

    }
}