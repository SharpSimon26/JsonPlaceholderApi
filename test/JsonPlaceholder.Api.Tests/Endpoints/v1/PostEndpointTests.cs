using JsonPlaceholder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace JsonPlaceholder.Api.Tests.Endpoints.v1;

public class PostEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IPostRepository> _mockRepo = new();

    public PostEndpointTests(WebApplicationFactory<Program> factory)
    {
        // Sostituzione del repository reale con un Mock tramite Dependency Injection
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => _mockRepo.Object);
            });
        });
    }

}
