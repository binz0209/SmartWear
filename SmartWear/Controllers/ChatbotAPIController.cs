using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using System.Linq;
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
            var chatLog = new ChatLog
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("B179C919-553A-4598-9210-7B960FB31FE7"),
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
            var userId = Guid.Parse("B179C919-553A-4598-9210-7B960FB31FE7");

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
