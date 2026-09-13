using System.Net;
using System.Net.Http.Json;
using JsonPlaceholder.Core.Dto;
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
    public async Task GetPosts_Returns200AndPosts()
    {
        // Arrange
        var expectedPosts = new List<Post>
        {
            new()
            {
                Id = 5,
                UserId = 8,
                Title = "Test Post 1",
                Body = "Test body 1"
            },
            new()
            {
                Id = 6,
                UserId = 8,
                Title = "Test Post 2",
                Body = "Test body 2"
            },
            new()
            {
                Id = 7,
                UserId = 10,
                Title = "Test Post 3",
                Body = "Test body 3"
            },
            new()
            {
                Id = 8,
                UserId = 15,
                Title = "Test Post 4",
                Body = "Test body 4"
            }
        };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedPosts);

        var client = _factory.CreateClient();        
    
        // Act
        var response = await client.GetAsync("/api/v1/posts");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
        Assert.IsType<IEnumerable<Post>>(posts, exactMatch: false);
        Assert.Equal(4, posts.Count());
        Assert.All(posts, p => {
            Assert.True(p.Id > 0);
            Assert.True(p.UserId > 0);
            Assert.False(string.IsNullOrWhiteSpace(p.Title));
            Assert.False(string.IsNullOrWhiteSpace(p.Body));
        });
        Assert.Equal(5, posts.First().Id);
        Assert.Equal(8, posts.First().UserId);
        Assert.Equal("Test Post 1", posts.First().Title);
        Assert.Equal("Test body 1", posts.First().Body);
        Assert.Equal(8, posts.Last().Id);
        Assert.Equal(15, posts.Last().UserId);   
        Assert.Equal("Test Post 4", posts.Last().Title);
        Assert.Equal("Test body 4", posts.Last().Body);

        // Verifica che il repo venga chiamato
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
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

        // Verifica che il repo venga chiamato
        _mockRepo.Verify(r => r.GetByIdAsync(It.Is<int>(i => i == 1)), Times.Once);
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

        // Verifica che il repo venga chiamato
        _mockRepo.Verify(r => r.GetByIdAsync(It.Is<int>(i => i == 99)), Times.Once);
    }

    [Fact]
    public async Task CreatePost_Returns200AndPost()
    {
        // Arrange
        var createPostDto = new CreatePostDto
        {
            Id = 100, 
            UserId = 20, 
            Title = "Test Post 100", 
            Body = "Test body 100" 
        };

        var expectedResult = new Post
        {
            Id = 10, 
            UserId = 20, 
            Title = "Test Post 100", 
            Body = "Test body 100" 
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Post>()))
                              .ReturnsAsync(expectedResult);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.PostAsJsonAsync("/api/v1/posts", createPostDto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var post = await response.Content.ReadFromJsonAsync<Post>();
        Assert.NotNull(post);
        Assert.Equal(10, expectedResult.Id);
        Assert.Equal(20, expectedResult.UserId);
        Assert.Equal("Test Post 100", expectedResult.Title);
        Assert.Equal("Test body 100", expectedResult.Body);
    
        // Assert
        _mockRepo.Verify(r => r.CreateAsync(It.Is<Post>(p =>
            p.Id == createPostDto.Id &&
            p.UserId == createPostDto.UserId &&
            p.Title == createPostDto.Title &&
            p.Body == createPostDto.Body
        )), Times.Once);
    }

    [Fact]
    public async Task CreatePost_Returns400()
    {
        // Arrange
        var createPostDto = new CreatePostDto
        {
            Id = 100, 
            UserId = 20, 
            Title = "", 
            Body = "" 
        };
    
        // Act
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Post>()));
        var client = _factory.CreateClient();
    
        // Assert
        var response = await client.PostAsJsonAsync("/api/v1/posts", createPostDto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Verifica che il repo non sia stato chiamato
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePost_Resturns200AndPost()
    {
        // Arrange
        var updatePostDto = new UpdatePostDto
        {
            Id = 1, 
            UserId = 20,
            Title = "New Title",
            Body = "New Body"
        };

        var expectedPost = new Post
        {
            Id = 1, 
            UserId = 20,
            Title = "New Title",
            Body = "New Body"
        };

        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Post>()))
                              .ReturnsAsync(expectedPost);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.PutAsJsonAsync("/api/v1/posts/1", updatePostDto);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var post = await response.Content.ReadFromJsonAsync<Post>();
        Assert.NotNull(post);
        Assert.Equal(expectedPost.Id, post.Id);
        Assert.Equal(expectedPost.UserId, post.UserId);
        Assert.Equal(expectedPost.Title, post.Title);
        Assert.Equal(expectedPost.Body, post.Body);

        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Post>(u => 
            u.Id == updatePostDto.Id &&
            u.UserId == updatePostDto.UserId &&
            u.Title == updatePostDto.Title &&
            u.Body == updatePostDto.Body
        )), Times.Once);
    }

    [Fact]
    public async Task UpdatePost_Returns404_WhenPostNotFound()
    {
        // Arrange
        var updatePostDto = new UpdatePostDto
        {
            Id = 1, 
            UserId = 20,
            Title = "New Title",
            Body = "New Body"
        };
    
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Post>()))
                              .ReturnsAsync((Post?)null);
        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/posts/1", updatePostDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Post>(r =>
            r.Id == updatePostDto.Id &&
            r.UserId == updatePostDto.UserId &&
            r.Title == updatePostDto.Title &&
            r.Body == updatePostDto.Body
        )), Times.Once);
    }

    [Fact]
    public async Task DeletePost_Returns204_WhenPostExists()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                              .ReturnsAsync(1);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.DeleteAsync("/api/v1/posts/1");
    
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _mockRepo.Verify(r => r.DeleteAsync(It.Is<int>(i => i == 1)), Times.Once);
    }

    [Fact]
    public async Task DeletePost_Returns404_WhenPostNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                              .ReturnsAsync(0);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.DeleteAsync("/api/v1/posts/99");
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        _mockRepo.Verify(r => r.DeleteAsync(It.Is<int>(i => i == 99)), Times.Once);
    }
}
