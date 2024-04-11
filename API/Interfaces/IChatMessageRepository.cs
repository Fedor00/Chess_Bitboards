using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<IEnumerable<ChatMessage>> GetChatMessagesAsync();
        Task<IEnumerable<ChatMessage>> GetChatMessagesByGameIdAsync(string gameId);
        Task<ChatMessage> GetChatMessageByIdAsync(long id);
        Task Update(ChatMessage chatMessage);
        Task Delete(long id);
        Task<int> Add(ChatMessage chatMessage);

    }
}