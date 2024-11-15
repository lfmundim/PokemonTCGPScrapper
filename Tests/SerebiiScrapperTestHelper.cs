using System.Net;
using Moq;
using Moq.Protected;

namespace PokemonTCGPocketScrapper.Tests
{
    public static class SerebiiScrapperTestHelper
    {
        public static HttpClient GetHttpClient(string sample)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Samples", sample);

            return CreateMockHttpClient(File.ReadAllText(filePath));
        }

        private static HttpClient CreateMockHttpClient(string responseContent)
        {
            // Create a mock for HttpMessageHandler
            var mockHandler = new Mock<HttpMessageHandler>();

            // Setup the protected SendAsync method to return a custom response
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Create HttpClient with the mocked handler
            return new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://localhost") };
        }
    }
}