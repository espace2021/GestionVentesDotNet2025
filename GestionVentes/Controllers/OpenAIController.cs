using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace GestionVentes.Controllers
{
    public class OpenAIController : Controller
    {
        private static readonly string apiKey = "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";  // Remplacez par votre clé API

        private static readonly string apiUrl = "https://api.openai.com/v1/completions"; // Endpoint pour GPT-3

        // Action pour afficher le formulaire de saisie du prompt
        public IActionResult Index()
        {
            return View();
        }

        // Action pour traiter le formulaire et appeler l'API OpenAI
        [HttpPost]
        public async Task<IActionResult> AskGPT(string prompt)
        {
            var response = await CallOpenAIAsync(prompt);
            ViewBag.Response = response;  // Passer la réponse à la vue
            return View("Index"); // Renvoyer la vue Index avec la réponse
        }

        private static async Task<string> CallOpenAIAsync(string prompt)
        {
            using (var client = new HttpClient())
            {
                // Configurez les en-têtes
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                // Créez la demande JSON
                var jsonRequest = new
                {
                    model = "gpt-4o-mini-transcribe",
                    prompt = prompt,
                    max_tokens = 100
                };

                // Sérialiser le corps de la demande en JSON
                var content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, "application/json");

                // Envoyez la requête POST
                var result = await client.PostAsync(apiUrl, content);

                // Lisez et retournez la réponse
                var resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }
        }
    }
}
