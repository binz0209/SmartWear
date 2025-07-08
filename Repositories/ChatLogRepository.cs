using Microsoft.EntityFrameworkCore;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Data;

namespace Repository
{
    public class ChatLogRepository : IChatLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatLog>> GetAllChatLogsAsync()
        {
            return await _context.ChatLogs.ToListAsync();
        }

        public async Task<ChatLog> GetChatLogByIdAsync(Guid id)
        {
            return await _context.ChatLogs.FindAsync(id);
        }

        public async Task AddChatLogAsync(ChatLog chatLog)
        {
            await _context.ChatLogs.AddAsync(chatLog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChatLogAsync(ChatLog chatLog)
        {
            _context.ChatLogs.Update(chatLog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteChatLogAsync(Guid id)
        {
            var chatLog = await GetChatLogByIdAsync(id);
            if (chatLog != null)
            {
                _context.ChatLogs.Remove(chatLog);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<ChatLog>> GetChatLogsByUserIdAsync(Guid userId)
        {
            return await _context.ChatLogs
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();
        }

    }
}
