using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartWear.Controllers
{
    [Route("api/chatbotapi")]
    [ApiController]
    public class ChatbotApiController : ControllerBase
    {
        private readonly GeminiClientService _geminiClientService;
        private readonly IChatLogService _chatLogService;

        public ChatbotApiController(GeminiClientService geminiClientService, IChatLogService chatLogService)
        {
            _geminiClientService = geminiClientService;
            _chatLogService = chatLogService;
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Thiếu câu hỏi.");

            // Gọi GeminiClientService để trả lời câu hỏi của người dùng
            var response = await _geminiClientService.GetGeminiResponse(question);


            // Tạo đối tượng ChatLog
            var userIdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            // Nếu null thì không lưu trong cơ sở dữ liệu
            if (userIdClaims == null)
                return Ok(new { reply = response });

            var chatLog = new ChatLog
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userIdClaims.Value),
                UserQuestion = question,
                BotResponse = response,
                CreatedOn = DateTime.Now
            };

            // In ra Console để kiểm tra dữ liệu
            Console.WriteLine("User Question: " + chatLog.UserQuestion);
            Console.WriteLine("Bot Response: " + chatLog.BotResponse);

            // Lưu chat log vào cơ sở dữ liệu
            await _chatLogService.AddChatLogAsync(chatLog);

            // Trả về phản hồi cho frontend
            return Ok(new { reply = response });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory()
        {
            // get currently logged-in user ID from claims
            var userIdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaims?.Value))
                return Unauthorized("Bạn cần đăng nhập để xem lịch sử trò chuyện.");

            var userId = Guid.Parse(userIdClaims.Value);

            // Console print
            Console.WriteLine("User ID: " + userId);

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
