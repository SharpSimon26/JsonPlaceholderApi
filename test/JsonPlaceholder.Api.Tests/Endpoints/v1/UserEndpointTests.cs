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
    public async Task GetUsers_Returns200AndUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new()
            {
                Id = 1, 
                Name = "Mario Rossi", 
                Username = "mariorossi", 
                Email = "mariorossi@gmail.com", 
                Phone = "123456", 
                Website = "mariorossi.com" 
            },
            new()
            {
                Id = 2,
                Name = "Luigi Verdi",
                Username = "luigiverdi",
                Email = "luigiverdi@gmail.com",
                Phone = "7891011",
                Website = "luigiverdi.com"
            },
            new()
            {
                Id = 3,
                Name = "Davide Bianchi",
                Username = "davidebianchi",
                Email = "davidebianchi@gmail.com",
                Phone = "12131415",
                Website = "davidebianchi.com"
            }
        };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedUsers);

        var client = _factory.CreateClient();
    
        // Act
        var response = await client.GetAsync("/api/v1/users");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
        Assert.IsType<IEnumerable<User>>(users, exactMatch: false);
        Assert.Equal(3, users.Count());
        Assert.All(users, p => {
            Assert.True(p.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(p.Name));
            Assert.False(string.IsNullOrWhiteSpace(p.Username));
            Assert.False(string.IsNullOrWhiteSpace(p.Email));
            Assert.False(string.IsNullOrWhiteSpace(p.Phone));
            Assert.False(string.IsNullOrWhiteSpace(p.Website));
        });
        Assert.Equal(1, users.First().Id);
        Assert.Equal("Mario Rossi", users.First().Name);
        Assert.Equal("mariorossi@gmail.com", users.First().Email);
        Assert.Equal(3, users.Last().Id); 
        Assert.Equal("Davide Bianchi", users.Last().Name);
        Assert.Equal("davidebianchi.com", users.Last().Website);

        // Verifica che il repo venga chiamato
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
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

        // Verifica che il repo venga chiamato
        _mockRepo.Verify(r => r.GetByIdAsync(It.Is<int>(i => i == 99)), Times.Once);
    }

    [Fact]
    public async Task CreateUser_Returns200AndUser()
    {
        // Arrange
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

    [Fact]
    public async Task UpdateUser_Resturns200AndUser()
    {
        // Arrange
        var updateUserDto = new UpdateUserDto
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

        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                              .ReturnsAsync(expectedUser);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.PutAsJsonAsync("/api/v1/users/1", updateUserDto);
    
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

        _mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => 
            u.Name == updateUserDto.Name &&
            u.Username == updateUserDto.Username &&
            u.Email == updateUserDto.Email &&
            u.Phone == updateUserDto.Phone &&
            u.Website == updateUserDto.Website
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_Returns204_WhenUserExists()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                              .ReturnsAsync(1);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.DeleteAsync("/api/v1/users/1");
    
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_Returns404_WhenUserNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                              .ReturnsAsync(0);
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.DeleteAsync("/api/v1/users/99");
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
    }
}
