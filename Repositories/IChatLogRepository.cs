using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IChatLogRepository
    {
        Task<IEnumerable<ChatLog>> GetAllChatLogsAsync();
        Task<ChatLog> GetChatLogByIdAsync(Guid id);
        Task AddChatLogAsync(ChatLog chatLog);
        Task UpdateChatLogAsync(ChatLog chatLog);
        Task DeleteChatLogAsync(Guid id);
        Task<IEnumerable<ChatLog>> GetChatLogsByUserIdAsync(Guid userId);

    }
}
