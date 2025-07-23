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

        public async Task<string> GetGeminiResponse(string userMessage)
        {
            // Truy vấn các sản phẩm còn hàng, kèm danh mục
            var allProducts = await _context.Products
                .Include(p => p.Category)
                .Where(x => x.StockQuantity > 0)
                .ToListAsync();

            // Nhóm sản phẩm theo danh mục
            var productsByCategory = allProducts
                .GroupBy(p => p.Category?.Name ?? "Khác")
                .Select(g => $"Danh mục: {g.Key}\n" +
                    string.Join("\n", g.Select(p =>
                        $"- {p.Name}: {p.Description} | Giá {p.Price}đ | Size: {p.Size} | Màu: {p.Color} | Còn {p.StockQuantity} chiếc")))
                .ToList();

            var productText = string.Join("\n\n", productsByCategory);

            // Prompt chỉ tư vấn sản phẩm theo danh mục, size, màu
            var prompt = $@"
            Bạn là nhân viên shop quần áo SmartWear, tự xưng là em thôi, luôn thân thiện và nhiệt tình tư vấn cho khách hàng.
            Dưới đây là các sản phẩm còn hàng, được phân theo danh mục:

            {productText}

            Hãy dựa vào danh sách sản phẩm để tư vấn cho khách hàng theo câu hỏi sau. Luôn xưng là chủ shop và tư vấn như một người bán hàng chuyên nghiệp.

            Khách hỏi: {userMessage}
            ";

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
