using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IChatLogService
    {
        Task<IEnumerable<ChatLog>> GetAllChatLogsAsync();
        Task<ChatLog> GetChatLogByIdAsync(Guid id);
        Task AddChatLogAsync(ChatLog chatLog);
        Task UpdateChatLogAsync(ChatLog chatLog);
        Task DeleteChatLogAsync(Guid id);
    }
}
