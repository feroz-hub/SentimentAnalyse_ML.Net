using Microsoft.ML.Data;

namespace SentimentAPI;

public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public bool PredictedLabel { get; set; }
}