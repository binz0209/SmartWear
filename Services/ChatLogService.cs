using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ChatLogService : IChatLogService
    {
        private readonly IChatLogRepository _chatLogRepository;

        public ChatLogService(IChatLogRepository chatLogRepository)
        {
            _chatLogRepository = chatLogRepository;
        }

        public async Task<IEnumerable<ChatLog>> GetAllChatLogsAsync()
        {
            return await _chatLogRepository.GetAllChatLogsAsync();
        }

        public async Task<ChatLog> GetChatLogByIdAsync(Guid id)
        {
            return await _chatLogRepository.GetChatLogByIdAsync(id);
        }

        public async Task AddChatLogAsync(ChatLog chatLog)
        {
            await _chatLogRepository.AddChatLogAsync(chatLog);
        }

        public async Task UpdateChatLogAsync(ChatLog chatLog)
        {
            await _chatLogRepository.UpdateChatLogAsync(chatLog);
        }

        public async Task DeleteChatLogAsync(Guid id)
        {
            await _chatLogRepository.DeleteChatLogAsync(id);
        }
        public async Task<IEnumerable<ChatLog>> GetChatLogsByUserIdAsync(Guid userId)
        {
            return await _chatLogRepository.GetChatLogsByUserIdAsync(userId);
        }

    }
}
