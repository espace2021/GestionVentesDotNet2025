﻿using Microsoft.AspNetCore.Mvc;
using GestionVentes.Helpers;
using GestionVentes.Models;
using System.Text;
using Newtonsoft.Json;



namespace GestionVentes.Controllers
{
    public class ChatLigneOrdersController : Controller
    {

        private readonly ChatService _chatService;
        private readonly HttpClient _httpClient;



        public ChatLigneOrdersController(IConfiguration configuration, HttpClient httpClient)
        {
            // Injecter IConfiguration dans le constructeur du contrôleur
            _chatService = new ChatService(configuration);  // Passer IConfiguration à ChatService
            // http
            _httpClient = httpClient;

        }

        [Route("chat/ligneOrders")]
        // Cette méthode sert à afficher le formulaire de question
        [HttpGet]
        public IActionResult Chat()
        {
            return View(); // Cela rendra la vue "ChatClient.cshtml"
        }

        [HttpPost]
        public async Task<IActionResult> Questionner(string userQuestion)
        {
            if (string.IsNullOrEmpty(userQuestion))
            {
                ViewData["ResponseMessage"] = "La question est requise.";
                return View("Chat");
            }

            var analysis = await AnalyzeIntent(userQuestion);

            var data = await _chatService.ExecuteSqlQuery(analysis);

            Console.WriteLine($"réponse des données : {data}");

            ViewData["ResponseMessage"] = GenerateResponse(data);

            return View("Chat");
        }

        public async Task<string> AnalyzeIntent(string userQuestion)
        {
            var prompt = $@" 
    Tu es un assistant spécialisé en bases de données SQL Server. 
    Génère une requête SQL Server valide et optimisée pour répondre à la question suivante :
    '{userQuestion}'
    - Assure-toi que la syntaxe est correcte et compatible avec SQL Server.
    - Si la question manque de précision, propose une requête qui retourne toutes les lignes de la table LigneOrders.
    - Utilise des alias explicites pour améliorer la lisibilité si nécessaire.
    - Le nom de la table est LigneOrders.
    - Évite les requêtes dangereuses comme la suppression ou la modification massive des données.
    - Les champs sont : LigneOrderID, OrderQty, LineItemTotal, OrderID, ProductID.
    - Le champ OrderID est une clé étrangère qui fait référence à OrderID de la table Orders.
   - Le champ ProductID est une clé étrangère qui fait référence à ProductID de la table products.
  - Le champ CustomerID dans orders est une clé étrngère qui fait référence à customers et qui sert à retrouver LastName et EmailAddress.
    - Retourne uniquement la requête SQL, sans explication supplémentaire ni commentaire.
    - La requête se termine toujours par un point-virgule.
";

            var llamaResponse = await CallLlama(prompt);

            Console.WriteLine($"Réponse brute de Llama : {llamaResponse}");

            var responseText = "";

            try
            {


                // Désérialiser la réponse JSON
                dynamic responseObject = JsonConvert.DeserializeObject(llamaResponse);

                // Extraire la chaîne contenant la requête 
                responseText = responseObject.response;

                // Afficher la requête
                Console.WriteLine("La requête SQL :");
                Console.WriteLine(responseText);




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'analyse de la réponse : {ex.Message}");

            }

            return responseText;
        }


        public async Task<string> CallLlama(string prompt)
        {
            // Inclure le modèle "llama3" dans la requête
            var requestContent = new OllamaRequest
            {
                Model = "llama3",
                Prompt = prompt,
                Stream = false
            };

            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(requestContent);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Envoi de la requête à l'API Ollama
            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ollama Response: {response.Content}");
                return responseContent;
            }

            // Si la réponse n'est pas OK, afficher le code de statut
            Console.WriteLine($"Ollama Response Pb : {response.StatusCode} - {response.ReasonPhrase}");
            return "Error contacting Ollama API";
        }

        private string GenerateResponse(List<Dictionary<string, object>> data)
        {
            if (data.Count == 0)
                return "Aucune donnée trouvée.";

            var response = "<ul>";
            foreach (var row in data)
            {
                response += "<li>" + string.Join(", ", row.Select(kv => $"{kv.Key}: {kv.Value}")) + "</li>";
            }
            response += "</ul>";

            return response;
        }



    }
}
