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

public class UserEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IUserRepository> _mockRepo = new();

    public UserEndpointTests(WebApplicationFactory<Program> factory)
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
    public async Task GetUserById_WhenExists_Returns200AndUser()
    {
        // Arrange
        var expectedUser = new User 
        { 
            Id = 1, 
            Name = "Mario Rossi", 
            Username = "mariorossi", 
            Email = "mariorossi@gmail.com", 
            Phone = "123456", 
            Website = "mariorossi.com" 
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/users/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.Equal(expectedUser.Id, user.Id);
        Assert.Equal(expectedUser.Name, user.Name);
        Assert.Equal(expectedUser.Username, user.Username);
        Assert.Equal(expectedUser.Email, user.Email);
        Assert.Equal(expectedUser.Phone, user.Phone);
        Assert.Equal(expectedUser.Website, user.Website);
    }

    [Fact]
    public async Task GetUserById_WhenNotFound_Returns404()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/users/99");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_Returns200AndUser()
    {
        var createUserDto = new CreateUserDto
        {
            Id = 1, 
            Name = "Mario Rossi", 
            Username = "mariorossi", 
            Email = "mariorossi@gmail.com", 
            Phone = "123456", 
            Website = "mariorossi.com" 
        };

        var expectedUser = new User 
        { 
            Id = 1, 
            Name = "Mario Rossi", 
            Username = "mariorossi", 
            Email = "mariorossi@gmail.com", 
            Phone = "123456", 
            Website = "mariorossi.com" 
        };

        // Arrange
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                              .ReturnsAsync(expectedUser);
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users", createUserDto);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.Equal(expectedUser.Id, user.Id);
        Assert.Equal(expectedUser.Name, user.Name);
        Assert.Equal(expectedUser.Username, user.Username);
        Assert.Equal(expectedUser.Email, user.Email);
        Assert.Equal(expectedUser.Phone, user.Phone);
        Assert.Equal(expectedUser.Website, user.Website);

        // Verifica che il repo venga chiamato e che la mappatura dei campi sia corretta
        _mockRepo.Verify(r => r.CreateAsync(It.Is<User>(u => 
            u.Name == createUserDto.Name &&
            u.Username == createUserDto.Username &&
            u.Email == createUserDto.Email &&
            u.Phone == createUserDto.Phone &&
            u.Website == createUserDto.Website
        )), Times.Once);
    }
}
