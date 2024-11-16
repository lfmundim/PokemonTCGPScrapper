using System.Net;
using Moq;
using Moq.Protected;

namespace PokemonTCGPocketScrapper.Tests
{
    public static class SerebiiScrapperTestHelper
    {
        public static HttpClient GetHttpClient(string sample)
        {
            string serebiiFilePath = Path.Combine(AppContext.BaseDirectory, "Samples", $"Serebii{sample}.html");
            string limitlessFilePath = Path.Combine(AppContext.BaseDirectory, "Samples", $"Limitless{sample}.html");

            return CreateMockHttpClient(File.ReadAllText(serebiiFilePath), File.ReadAllText(limitlessFilePath));
        }

        private static HttpClient CreateMockHttpClient(string serebiiContent, string limitlessContent)
        {
            // Create a mock for HttpMessageHandler
            var mockHandler = new Mock<HttpMessageHandler>();

            // Setup the protected SendAsync method to return a custom response
            mockHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(serebiiContent)
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(limitlessContent)
                });

            // Create HttpClient with the mocked handler
            return new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://localhost") };
        }
    }
}