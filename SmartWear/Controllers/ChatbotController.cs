using Microsoft.AspNetCore.Mvc;
using Business.Models;
using Services;
using Business.Data;
using System;
using System.Threading.Tasks;
using Services.Interfaces;

namespace SmartWear.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly GeminiClientService _geminiClientService;
        private readonly IChatLogService _chatLogService;

        public ChatbotController(GeminiClientService geminiClientService, IChatLogService chatLogService)
        {
            _geminiClientService = geminiClientService;
            _chatLogService = chatLogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetChatResponse(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                ViewBag.ChatResponse = "Vui lòng nhập câu hỏi!";
                return View("Index");
            }

            // Gọi Gemini API để lấy phản hồi
            var response = await _geminiClientService.GetGeminiResponse(question);
            ViewBag.ChatResponse = response;

            return View("Index");
        }
    }
}
