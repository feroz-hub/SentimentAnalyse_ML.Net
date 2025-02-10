using Microsoft.ML.Data;

namespace SentimentTrainer;

public class ModelInput
{
    [LoadColumn(0)]
    public string Text { get; set; }
    [LoadColumn(1)]
    public bool Label { get; set; }
}