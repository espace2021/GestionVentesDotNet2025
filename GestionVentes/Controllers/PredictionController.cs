using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using GestionVentes.Models;
using GestionVentes.Entity;

namespace GestionVentes.Controllers
{
    public class PredictionController : Controller
    {
        private readonly ILigneOrdersRepository _ligneOrdersRepository; 
        private static readonly MLContext mlContext = new MLContext(seed: 42); // Fixed seed for reproducibility
        private static ITransformer _model;

        public PredictionController(ILigneOrdersRepository ligneOrdersRepository )
        {
            _ligneOrdersRepository = ligneOrdersRepository;
                }

        // Méthode pour entraîner le modèle
        private async Task<ITransformer> TrainModelAsync()
        {
            // Charger les données des produits et des lignes de commande
            var ligneOrders = await _ligneOrdersRepository.GetAllLigneOrdersAsync();

            if (!ligneOrders.Any())
            {
                throw new InvalidOperationException("Aucune donnée de ligne de commande disponible pour l'entraînement.");
            }

            // Transformer les données en un format compatible ML.NET
            var trainData = mlContext.Data.LoadFromEnumerable(
                ligneOrders.Select(l => new ProductOrderPrediction
                {
                    ProductID = l.ProductID,
                    OrderQty = l.OrderQty,
                    ListPrice = (float)l.Product.ListPrice,
                    Category = l.Product.Category,
                    LineItemTotal = (float)l.LineItemTotal

                })
            );

            // Build the pipeline
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "OrderQty")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "ProductIDEncoded", inputColumnName: "ProductID"))
                .Append(mlContext.Transforms.NormalizeMeanVariance("ListPriceNormalized", "ListPrice"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CategoryEncoded", inputColumnName: "Category"))
                .Append(mlContext.Transforms.NormalizeMeanVariance("LineItemTotalNormalized", "LineItemTotal"))
                .Append(mlContext.Transforms.Concatenate("Features", "ListPriceNormalized", "ProductIDEncoded" , "CategoryEncoded" , "LineItemTotalNormalized"))
                .Append(mlContext.Regression.Trainers.FastTree());

          
            // Entraîner le modèle
            _model = pipeline.Fit(trainData);

            // Evaluate the model
            var testData = mlContext.Data.TrainTestSplit(trainData, testFraction: 0.2);
            var predictions = _model.Transform(testData.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"R² Score: {metrics.RSquared}");
            Console.WriteLine($"Erreur moyenne absolue: {metrics.MeanAbsoluteError}");

            return _model;
        }

        // Action POST pour la prédiction
        [HttpPost]
        public async Task<IActionResult> Predict(int productId)
        {
            try
            {
                // Vérifier si le modèle est déjà entraîné
                if (_model == null)
                {
                    _model = await TrainModelAsync();
                }

                // Récupérer les dnnées
                 var ligneOrder = await _ligneOrdersRepository.GetProductsByIdAsync(productId);

                Console.WriteLine($"productId: {productId}");

                // Create a prediction engine that uses the same data schema as during training
                var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductOrderPrediction, ProductOrderOutput>(_model);

                // Predict using the same structure as training data
                var prediction = predictionEngine.Predict(new ProductOrderPrediction
                {
                    ProductID = productId,
                    Category = ligneOrder.Product.Category,
                    ListPrice = (float)ligneOrder.Product.ListPrice,
                    LineItemTotal = (float)ligneOrder.LineItemTotal,
                    OrderQty = 0 // This value doesn't matter for prediction
                });

                // Arrondir la prédiction pour avoir un nombre entier
                int predictedQty = (int)Math.Round(prediction.PredictedOrderQty);
                // Assurer que la quantité prédite est au moins 1
                predictedQty = Math.Max(1, predictedQty);

                // Afficher la quantité prédite
                ViewBag.PredictedOrderQty = predictedQty;

                // Afficher e nom du produit
                ViewBag.ProductName = ligneOrder.Product.ProductName;

                // Get list of products for dropdown
                var products = await _ligneOrdersRepository.GetAllLigneOrdersAsync();
                ViewData["Products"] = products;

                return View("Index");
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Erreur de prédiction: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                ViewBag.ErrorMessage = "Une erreur s'est produite lors de la prédiction.";
                return View("Error");
            }
        }

        // Page d'accueil avec le formulaire
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get list of products for dropdown
            var products = await _ligneOrdersRepository.GetAllLigneOrdersAsync();
            ViewData["Products"] = products;
            return View();
        }
    }
}


