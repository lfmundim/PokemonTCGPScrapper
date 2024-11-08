using Microsoft.Extensions.Configuration;
using PokemonTCGPScrapper;

namespace PokemonTCGPocketScrapper
{
    public class Program
    {
        public static async Task Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var appsettings = configuration.Get<AppSettings>();

            await SerebiiScrapper.RunAsync(appsettings!.Collections);
        }
    }
}