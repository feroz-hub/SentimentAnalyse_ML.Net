using Microsoft.ML;

namespace SentimentAPI.Services;

public class SentimentService
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;
    private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

    public SentimentService()
    {
        _mlContext = new MLContext();
        _model = _mlContext.Model.Load("sentiment_model.zip", out var schema);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);
    }
    public string Predict(string text)
    {
        var prediction = _predictionEngine.Predict(new ModelInput { Text = text });
        return prediction.PredictedLabel ? "Positive" : "Negative";
    }


}