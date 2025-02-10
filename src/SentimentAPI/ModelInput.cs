using Microsoft.ML.Data;

namespace SentimentAPI;

public class ModelInput
{
    [LoadColumn(0)]
    public string Text { get; set; }
}