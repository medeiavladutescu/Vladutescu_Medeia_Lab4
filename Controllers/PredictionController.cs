using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PriceModel = Vladutescu_Medeia_Lab4.PricePredictionModel;
using DurationModel = Vladutescu_Medeia_Lab4.DurationPredictionModel;

namespace Vladutescu_Medeia_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        public IActionResult Price(PriceModel.ModelInput input)
        {
            // Load the model
            MLContext mlContext = new MLContext();
            // Create predection engine related to the loaded train model
           ITransformer mlModel = mlContext.Model.Load("PricePredictionModel.mlnet", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<PriceModel.ModelInput, PriceModel.ModelOutput>(mlModel);
            PriceModel.ModelOutput result = predEngine.Predict(input);
            ViewBag.Price = result.Score;
            return View(input);
        }
        public IActionResult Duration(DurationModel.ModelInput input)
        {
            MLContext mlContext=new MLContext();
            ITransformer mlModel = mlContext.Model.Load("DurationPredictionModel.mlnet", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<DurationModel.ModelInput, DurationModel.ModelOutput>(mlModel);

            DurationModel.ModelOutput result = predEngine.Predict(input);
            ViewBag.Duration = result.Score;
            return View(input);
        }
    }
}