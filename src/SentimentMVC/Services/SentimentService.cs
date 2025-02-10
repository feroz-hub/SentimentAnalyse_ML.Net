using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SentimentMVC.Services
{
    public class SentimentService
    {
        private readonly HttpClient _httpClient = new();
        private const string ApiUrl = "http://localhost:5247/api/sentiment/predict"; // ✅ API URL

        public async Task<string> PredictSentiment(string text)
        {
            var requestData = new { Text = text };
            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SentimentResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.Sentiment ?? "Unknown";
            }
            return "Error: API request failed";
        }

        private class SentimentResponse
        {
            public string Text { get; set; }
            public string Sentiment { get; set; }
        }
    }
}