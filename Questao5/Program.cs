using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Questao5;

public class Program
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main()
    {
        ContaBancaria conta = new ContaBancaria(123, "Fulano", 1000.0);
        Console.WriteLine(conta);

        conta.Sacar(200.0);
        Console.WriteLine(conta);

        conta.Depositar(500.0);
        Console.WriteLine(conta);

        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);
    }

    public static async Task<int> getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;

        try
        {
            // API endpoint - Endpoint ficticio
            string apiUrl = $"https://api.football-data.org/v2/teams/{team}/matches?season={year}";
            client.DefaultRequestHeaders.Add("X-Auth-Token", "API_KEY"); // Adicionar API key correta

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            foreach (var match in json["matches"])
            {
                string homeTeam = match["homeTeam"]["name"].ToString();
                string awayTeam = match["awayTeam"]["name"].ToString();
                int homeGoals = int.Parse(match["score"]["fullTime"]["homeTeam"].ToString());
                int awayGoals = int.Parse(match["score"]["fullTime"]["awayTeam"].ToString());

                if (homeTeam.Equals(team, StringComparison.OrdinalIgnoreCase))
                {
                    totalGoals += homeGoals;
                }
                else if (awayTeam.Equals(team, StringComparison.OrdinalIgnoreCase))
                {
                    totalGoals += awayGoals;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        return totalGoals;
    }
}
