using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Business.Data;
using Business.Models;
using System.Collections.Generic;

namespace Services
{
    public class GeminiClientService
    {
        private readonly string _apiKey;
        private static readonly HttpClient _client = new HttpClient();
        private readonly ApplicationDbContext _context;

        public GeminiClientService(IConfiguration configuration, ApplicationDbContext context)
        {
            _apiKey = configuration.GetValue<string>("Gemini:ApiKey");
            _context = context;  // Inject ApplicationDbContext
        }

        public string GetApiKey()
        {
            return _apiKey;
        }

        // Cập nhật phương thức để lấy sản phẩm từ DB và truyền câu hỏi cho Gemini
        public async Task<string> GetGeminiResponse(string userMessage)
        {
            // Truy vấn các sản phẩm còn hàng
            var allProducts = await _context.Products
                .Where(x => x.StockQuantity > 0)
                .ToListAsync();

            // Chuyển thông tin sản phẩm thành chuỗi mô tả cho prompt
            var productText = string.Join("\n", allProducts.Select(p =>
                $"* **{p.Name}**: {p.Description} Giá {p.Price}đ, còn {p.StockQuantity} chiếc"));

            // Tạo prompt cho Gemini API, bao gồm câu hỏi của người dùng và danh sách sản phẩm
            var prompt = $@"
Chào bạn! Bên mình có mấy món này đang được nhiều người ưa chuộng lắm nè:

{productText}

Hãy dựa vào các sản phẩm có trong danh sách trên để trả lời câu hỏi sau:

Người dùng hỏi: {userMessage}
";

            // Gửi prompt vào Gemini API để lấy câu trả lời
            var apiEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[] {
            new {
                parts = new[] {
                    new {
                        text = prompt
                    }
                }
            }
        }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
            {
                Content = content
            };

            var response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                dynamic responseData = JsonConvert.DeserializeObject(result);
                string responseText = responseData.candidates[0].content.parts[0].text;

                // Xử lý chuỗi trả về, giữ lại dấu sao và in đậm các đoạn văn bản trong dấu ** 
                responseText = ProcessResponseText(responseText);

                return responseText;
            }
            else
            {
                return "Xin lỗi, có lỗi xảy ra khi kết nối đến server. Bạn thử lại nhé!";
            }
        }

        // Phương thức xử lý chuỗi trả về từ Gemini API
        private string ProcessResponseText(string responseText)
        {
            responseText = System.Text.RegularExpressions.Regex.Replace(responseText, @"\*\*(.*?)\*\*", "<strong>$1</strong>");

            responseText = System.Text.RegularExpressions.Regex.Replace(responseText, @"\* \*\*(.*?)\*\*", "<br><strong>$1</strong>");

            responseText = System.Text.RegularExpressions.Regex.Replace(responseText, @"\* (?!\*)", "<br>");

            return responseText;
        }


    }
}
