using Microsoft.AspNetCore.Mvc;
using SentimentAPI.Services;

namespace SentimentAPI.Controllers;

[ApiController]
[Route("api/sentiment")]
public class SentimentController(SentimentService sentimentService) : ControllerBase
{
    [HttpPost("predict")]
    public IActionResult Predict([FromBody] ModelInput input)
    {
        var result = sentimentService.Predict(input.Text);
        return Ok(new { Text = input.Text, Sentiment = result });
    }
}