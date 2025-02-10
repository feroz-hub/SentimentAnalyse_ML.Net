using Microsoft.ML;
using Microsoft.ML.Transforms.Text;
using Microsoft.ML.Trainers;

namespace SentimentTrainer;

public class ModelTrainer
{
    private static readonly string DataPath = "sentiment_data.csv";
   
    // ✅ Get absolute path to src/SentimentAPI/
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../../"));
    private static readonly string SentimentAPIPath = Path.Combine(ProjectRoot, "src/SentimentAPI");
    private static readonly string ModelPath = Path.Combine(SentimentAPIPath, "sentiment_model.zip");



    public static void TrainAndSaveModel()
    {
        Console.WriteLine($"Saving model to: {ModelPath}");
        // ✅ Ensure SentimentAPI directory exists
        if (!Directory.Exists(SentimentAPIPath))
        {
            Console.WriteLine($"Error: SentimentAPI directory does not exist at {SentimentAPIPath}");
            return;
        }

        // ✅ Delete existing model if it exists
        if (File.Exists(ModelPath))
        {
            Console.WriteLine("ℹ️ Existing model found. Deleting...");
            File.Delete(ModelPath);
        }

        var mlContext = new MLContext();
        //Load data
        var data = mlContext.Data.LoadFromTextFile<ModelInput>(DataPath,separatorChar: ',',hasHeader:true, allowQuoting:true);
        
        // Define Training Pipeline
        var pipeline = mlContext.Transforms.Text.NormalizeText("Text", caseMode: TextNormalizingEstimator.CaseMode.Lower)
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Text"))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features"));

        
        //Train Model
        var model = pipeline.Fit(data);
        
        // Split data for testing
        var trainTestSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
        var testData = trainTestSplit.TestSet;
        
        // Ensure test data has both positive & negative samples
        var labelColumn = mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
            .Select(x => x.Label).ToList();

        if (!labelColumn.Contains(true) || !labelColumn.Contains(false))
        {
            Console.WriteLine("⚠️ Warning: Test data does not contain both positive and negative samples.");
        }
        else
        {
            Console.WriteLine("Test data contains both positive and negative samples.");
        }

        var predictions = model.Transform(testData);
        var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
        Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");
        Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");

        //Save Model
        mlContext.Model.Save(model, data.Schema, ModelPath);
        Console.WriteLine("Model saved");
        // Test the model with a sample input
        var sampleText = "she is bad";
        var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
        var prediction = predictionEngine.Predict(new ModelInput { Text = sampleText });
        Console.WriteLine($"Prediction for '{sampleText}': {(prediction.PredictedLabel ? "Positive" : "Negative")}");

    }
}
public class ModelOutput
{
    public bool PredictedLabel { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}