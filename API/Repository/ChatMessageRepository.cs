using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Models.Entities;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public ChatMessageRepository(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<int> Add(ChatMessage chatMessage)
        {
            var context = _contextFactory.CreateDbContext();
            await context.ChatMessages.AddAsync(chatMessage);
            return await context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var context = _contextFactory.CreateDbContext();
            var chatMessage = context.ChatMessages.Find(id);
            if (chatMessage != null)
            {
                context.ChatMessages.Remove(chatMessage);
                await context.SaveChangesAsync();
            }

        }

        public async Task<ChatMessage> GetChatMessageByIdAsync(long id)
        {
            var context = _contextFactory.CreateDbContext();
            return await context.ChatMessages.FindAsync(id);
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessagesAsync()
        {
            var context = _contextFactory.CreateDbContext();
            return await context.ChatMessages.ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessagesByGameIdAsync(string gameId)
        {
            var context = _contextFactory.CreateDbContext();
            return await context.ChatMessages.Where(chatMessage => chatMessage.GameId == gameId).OrderBy(chatMessage => chatMessage.Timestamp).ToListAsync();
        }

        public Task Update(ChatMessage chatMessage)
        {
            var context = _contextFactory.CreateDbContext();
            context.Entry(chatMessage).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }
    }
}