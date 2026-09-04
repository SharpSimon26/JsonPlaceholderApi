using System.Data;
using Dapper;
using JsonPlaceholder.Core.Entities;
using JsonPlaceholder.DataAccess.Database;
using JsonPlaceholder.DataAccess.Repositories;
using Moq;
using Moq.Dapper;

namespace JsonPlaceholder.DataAccess.Tests.Repositories;

public class PostRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_Returns_List_Of_Posts()
    {
        // 1. Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedPosts = new List<Post>
        {
            new() { Id = 1, UserId = 10, Title = "Primo titolo", Body = "Primo Post" },
            new() { Id = 2, UserId = 20, Title = "Secondo titolo", Body = "Secondo Post" },
            new() { Id = 3, UserId = 30, Title = "Terzo titolo", Body = "Terzo Post" }
        };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare la query
        mockConnection.SetupDapperAsync(conn => conn.QueryAsync<Post>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null
            ))
            .ReturnsAsync(expectedPosts);

        var repository = new PostRepository(mockFactory.Object);

        // 2. Act
        var result = await repository.GetAllAsync();

        // 3. Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());

        // Controlla che nessun elemento sia null e che gli ID siano validi
        Assert.All(result, item => 
        {
            Assert.NotNull(item);
            Assert.True(item.Id > 0);
            Assert.True(item.UserId > 0);
            Assert.False(string.IsNullOrWhiteSpace(item.Title));
            Assert.False(string.IsNullOrWhiteSpace(item.Body));
        });

        // Verifica elementi specifici
        Assert.Equal("Primo titolo", result.First().Title);
        Assert.Equal("Terzo Post", result.Last().Body);

        // Verifica che la connessione al DB sia stata invocata esattamente 1 volta
        mockFactory.Verify(db => db.CreateConnection(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPost_WhenPostExists()
    {
        // 1. Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedPost = new Post { Id = 18, UserId = 23,  Title = "Mio Titolo", Body = "Mio Body" };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare la query
        mockConnection.SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<Post>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null
            ))
            .ReturnsAsync(expectedPost);

        var repository = new PostRepository(mockFactory.Object);

        // 2. Act
        var result = await repository.GetByIdAsync(18);

        // 3. Assert
        Assert.NotNull(result);
        Assert.Equal(18, result.Id);
        Assert.Equal(23, result.UserId);
        Assert.Equal("Mio Titolo", result.Title);
        Assert.Equal("Mio Body", result.Body);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Dapper restituisce null se non trova nulla
        mockConnection.SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<Post>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null
            ))
            .ReturnsAsync((Post?)null);

        var repository = new PostRepository(mockFactory.Object);

        // Act
        var result = await repository.GetByIdAsync(999); // Id inesistente

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturn_Post()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedPost = new Post { Id = 15, UserId = 8, Title = "Nuovo titolo", Body = "Nuovo post" };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare le query
        mockConnection
            .SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<Post>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expectedPost);

        var repository = new PostRepository(mockFactory.Object);

        // Act
        var result = await repository.CreateAsync(new Post { Id = 1, UserId = 8, Title = "Nuovo titolo", Body = "Nuovo post" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.Id);
        Assert.Equal("Nuovo titolo", result.Title);
    }
}