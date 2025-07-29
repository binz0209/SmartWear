using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartWear.Hubs;

namespace SmartWear.Controllers
{
    [Route("api/chatbotapi")]
    [ApiController]
    public class ChatbotApiController : ControllerBase
    {
        private readonly GeminiClientService _geminiClientService;
        private readonly IChatLogService _chatLogService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatbotApiController(GeminiClientService geminiClientService, IChatLogService chatLogService, IHubContext<ChatHub> hubContext)
        {
            _geminiClientService = geminiClientService;
            _chatLogService = chatLogService;
            _hubContext = hubContext;
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Thiếu câu hỏi.");

            // Gửi ngay tin nhắn user lên các client (realtime)
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", question);

            // Lấy phản hồi bot
            var response = await _geminiClientService.GetGeminiResponse(question);

            // Gửi phản hồi bot về client
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "bot", response);

            // Lưu chat log nếu có user login
            var userIdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaims != null)
            {
                var chatLog = new ChatLog
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userIdClaims.Value),
                    UserQuestion = question,
                    BotResponse = response,
                    CreatedOn = DateTime.Now
                };
                await _chatLogService.AddChatLogAsync(chatLog);
            }

            return Ok(new { reply = response });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory()
        {
            var userIdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaims?.Value))
                return Unauthorized("Bạn cần đăng nhập để xem lịch sử trò chuyện.");

            var userId = Guid.Parse(userIdClaims.Value);
            var chatLogs = await _chatLogService.GetChatLogsByUserIdAsync(userId);

            var history = chatLogs.Select(log => new
            {
                question = log.UserQuestion,
                response = log.BotResponse
            });

            return Ok(history);
        }
    }
}
