using Microsoft.AspNetCore.Mvc;
using SentimentMVC.Services;
using System.Threading.Tasks;

namespace SentimentMVC.Controllers
{
    public class SentimentController : Controller
    {
        private readonly SentimentService _sentimentService = new();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Predict(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                ViewBag.Message = "Please enter text.";
                return View("Index");
            }

            var result = await _sentimentService.PredictSentiment(userInput);
            ViewBag.Result = result;
            ViewBag.UserInput = userInput; // ✅ Store input text

            return View("Index");
        }

    }
}