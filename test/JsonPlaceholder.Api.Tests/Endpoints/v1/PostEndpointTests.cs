using System.Net;
using System.Net.Http.Json;
using JsonPlaceholder.Core.Entities;
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

    [Fact]
    public async Task GetPostById_WhenExists_Returns200AndPost()
    {
        // Arrange
        var expectedPost = new Post 
        { 
            Id = 1, 
            UserId = 15, 
            Title = "Test Post", 
            Body = "Test body" 
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedPost);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/posts/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var post = await response.Content.ReadFromJsonAsync<Post>();
        Assert.NotNull(post);
        Assert.Equal(expectedPost.Id, post.Id);
        Assert.Equal(expectedPost.UserId, post.UserId);
        Assert.Equal(expectedPost.Title, post.Title);
        Assert.Equal(expectedPost.Body, post.Body);
    }

    [Fact]
    public async Task GetPostById_WhenNotFound_Returns404()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Post?)null);
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/posts/99");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
