using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using PokemonTCGPScrapper;

namespace PokemonTCGPocketScrapper
{
    public class Program
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public static async Task Main(string[] args)
        {
            // Check for the argument passed to the program
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run <scrapperName>");
                Console.WriteLine("Supported scrappers: serebii");
                return;
            }

            string scrapperName = args[0].ToLowerInvariant();
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var appsettings = configuration.Get<AppSettings>();

            using HttpClient httpClient = new();

            BaseScrapper scrapper;

            // Choose scrapper based on the provided argument
            switch (scrapperName)
            {
                case "serebii":
                    scrapper = new SerebiiScrapper();
                    break;

                default:
                    Console.WriteLine($"Unknown scrapper: {scrapperName}");
                    return;
            }

            var cards = await scrapper.RunAsync(appsettings!.Collections, httpClient);

            string json = JsonSerializer.Serialize(cards, _jsonSerializerOptions);
            
            await File.WriteAllTextAsync("cards.json", json);
            Console.WriteLine("Data extraction complete. Output saved to cards.json.");
        }
    }
}