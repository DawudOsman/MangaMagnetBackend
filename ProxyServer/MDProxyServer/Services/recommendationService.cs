using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

public class RecommendationService
{
    private readonly Dictionary<string, List<string>> _recommendations;

    public RecommendationService(string csvFilePath)
    {
        _recommendations = LoadRecommendations(csvFilePath);
    }

    private Dictionary<string, List<string>> LoadRecommendations(string csvFilePath)
    {
        var recommendations = new Dictionary<string, List<string>>();

        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
        {
            // Dynamically read records
            var records = csv.GetRecords<dynamic>();

            foreach (var record in records)
            {
                // Access properties of the dynamic object using casting
                string mangaId = ((IDictionary<string, object>)record)["mangaID"].ToString();

                var similarMangaIds = new List<string>();

                // Loop through columns for recommended manga IDs
                for (int i = 1; i <= 50; i++)
                {
                    string columnName = $"Recommended_mangaID_{i}";
                    if (((IDictionary<string, object>)record).ContainsKey(columnName))
                    {
                        var recommendedId = ((IDictionary<string, object>)record)[columnName]?.ToString();
                        if (!string.IsNullOrEmpty(recommendedId))
                        {
                            similarMangaIds.Add(recommendedId);
                        }
                    }
                }

                recommendations[mangaId] = similarMangaIds;
            }
        }

        return recommendations;
    }

    public MangaRecommendation GetRecommendations(string mangaId)
    {
        if (_recommendations.ContainsKey(mangaId))
        {
            return new MangaRecommendation
            {
                MangaId = mangaId,
                SimilarMangaIds = _recommendations[mangaId]
            };
        }

        return null;
    }
}

public class MangaRecommendation
{
    public string MangaId { get; set; }
    public List<string> SimilarMangaIds { get; set; }
}