using GestionVentes.Models;
using Microsoft.Data.SqlClient;

namespace GestionVentes.Helpers
{
    public class ChatService
    {
        private readonly string _connectionString;

        public ChatService(IConfiguration configuration)
        {
            // Récupérer la chaîne de connexion depuis appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Dictionary<string, object>>> ExecuteSqlQuery(string query)
        {
            //Nettoyage des 
            var analysis = RemoveBackticks(query);
            // Vérification des données d'entrée
            if (analysis.Length==0)
                return new List<Dictionary<string, object>>();

            var results = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Utiliser la requête SQL dynamique fournie dans analysis
                using (var command = new SqlCommand(analysis, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        // Ajouter les résultats de chaque colonne dans le dictionnaire
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            return results;
        }

        public string RemoveBackticks(string query)
        {
            // Vérifier si la chaîne commence et se termine par des backticks
            if (query.StartsWith("```") && query.EndsWith("```"))
            {
                // Supprimer les backticks au début et à la fin
                query = query.Substring(3, query.Length - 6); // 3 caractères au début et 3 à la fin
            }

            return query;
        }
    }
}
