using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Vladutescu_Medeia_Lab4.Models;
using Vladutescu_Medeia_Lab4.Data;
using DurationModel = Vladutescu_Medeia_Lab4.DurationPredictionModel;
using PriceModel = Vladutescu_Medeia_Lab4.PricePredictionModel;

namespace Vladutescu_Medeia_Lab4.Controllers
{

    public class PredictionController : Controller
    {
        private readonly AppDbContext _context;

        public PredictionController (AppDbContext context)
        {
            _context = context;
        }
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

        [HttpGet]
       
        public async Task<IActionResult> Dashboard(DateTime? fromDate, DateTime? toDate)
        {
           
            var query = _context.PredictionHistories.AsQueryable();

            if (fromDate.HasValue)
            {
               
                query = query.Where(p => p.CreatedAt.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
               
                query = query.Where(p => p.CreatedAt.Date <= toDate.Value.Date);
            }

           

            
            var totalPredictions = await query.CountAsync();

            
            var paymentTypeStats = await query
                .GroupBy(p => p.PaymentType)
                .Select(g => new PaymentTypeStat
                {
                    PaymentType = g.Key,
                    AveragePrice = g.Average(x => x.PredictedPrice),
                    Count = g.Count()
                })
                .ToListAsync();

            
            var allPredictions = await query
                .Select(p => p.PredictedPrice)
                .ToListAsync();

            
            var buckets = new List<PriceBucketStat>
    {
        new PriceBucketStat { Label = "0 - 10" },
        new PriceBucketStat { Label = "10 - 20" },
        new PriceBucketStat { Label = "20 - 30" },
        new PriceBucketStat { Label = "30 - 50" },
        new PriceBucketStat { Label = "> 50" }
    };

            foreach (var price in allPredictions)
            {
                if (price < 10) buckets[0].Count++;
                else if (price < 20) buckets[1].Count++;
                else if (price < 30) buckets[2].Count++;
                else if (price < 50) buckets[3].Count++;
                else buckets[4].Count++;
            }

            var vm = new DashboardViewModel
            {
                TotalPredictions = totalPredictions,
                PaymentTypeStats = paymentTypeStats,
                PriceBuckets = buckets,
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(vm);
        }
    }
}