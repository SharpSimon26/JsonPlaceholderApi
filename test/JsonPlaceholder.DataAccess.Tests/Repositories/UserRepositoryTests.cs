using System.Data;
using Dapper;
using JsonPlaceholder.Core.Entities;
using JsonPlaceholder.DataAccess.Database;
using JsonPlaceholder.DataAccess.Repositories;
using Moq;
using Moq.Dapper;

namespace JsonPlaceholder.DataAccess.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_Returns_List_Of_Users()
    {
        // 1. Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedPosts = new List<User>
        {
            new() { Id = 1, Name = "Mario Rossi", Username = "mariorossi", Email = "mariorossi@gmail.com", Phone = "0039 378 252 848", Website = "mariorossi.com" },
            new() { Id = 2, Name = "Giuseppe Verdi", Username = "giuseppeverdi", Email = "giuseppeverdi@gmail.com", Phone = "0039 233 252 849", Website = "giuseppeverdi.com" },
            new() { Id = 3, Name = "Achille Bianchi", Username = "achillebianchi", Email = "achillebianchi@gmail.com", Phone = "0039 464 252 850", Website = "achillebianchi.com" },
        };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare la query
        mockConnection.SetupDapperAsync(conn => conn.QueryAsync<User>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null
            ))
            .ReturnsAsync(expectedPosts);

        var repository = new UserRepository(mockFactory.Object);

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
            Assert.False(string.IsNullOrWhiteSpace(item.Name));
            Assert.False(string.IsNullOrWhiteSpace(item.Username));
            Assert.False(string.IsNullOrWhiteSpace(item.Email));
            Assert.False(string.IsNullOrWhiteSpace(item.Phone));
            Assert.False(string.IsNullOrWhiteSpace(item.Website));
        });

        // Verifica elementi specifici
        Assert.Equal("Mario Rossi", result.First().Name);
        Assert.Equal("achillebianchi.com", result.Last().Website);

        // Verifica che la connessione al DB sia stata invocata esattamente 1 volta
        mockFactory.Verify(db => db.CreateConnection(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenIdExists()
    {
        // 1. Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedUser = new User { Id = 43, Name = "Mario Rossi", Username = "mariorossi", Email = "mario.rossi@gmail.com", Phone = "0039 378 252 848", Website = "mariorossi.com" };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare la query
        mockConnection.SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null
            ))
            .ReturnsAsync(expectedUser);

        var repository = new UserRepository(mockFactory.Object);

        // 2. Act
        var result = await repository.GetByIdAsync(43);

        // 3. Assert
        Assert.NotNull(result);
        Assert.Equal(43, result.Id);
        Assert.Equal("Mario Rossi", result.Name);
        Assert.Equal("mariorossi", result.Username);
        Assert.Equal("mario.rossi@gmail.com", result.Email);
        Assert.Equal("0039 378 252 848", result.Phone);
        Assert.Equal("mariorossi.com", result.Website);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Dapper restituisce null se non trova nulla
        mockConnection.SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null
            ))
            .ReturnsAsync((User?)null);

        var repository = new UserRepository(mockFactory.Object);

        // Act
        var result = await repository.GetByIdAsync(999); // Id inesistente

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturn_User()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedUser = new User { Id = 4, Name = "Luigi Neri", Username = "luigineri", Email = "luigineri@gmail.com", Phone = "0039 040 366 84870", Website = "luigineri.com" };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare le query
        mockConnection
            .SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expectedUser);

        var repository = new UserRepository(mockFactory.Object);

        // Act
        var result = await repository.CreateAsync(new User { Id = 1, Name = "Luigi Neri", Username = "luigineri", Email = "luigineri@gmail.com", Phone = "0039 040 366 84870", Website = "luigineri.com" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Id);
        Assert.Equal("Luigi Neri", result.Name);
        Assert.Equal("luigineri", result.Username);
        Assert.Equal("luigineri@gmail.com", result.Email);
        Assert.Equal("0039 040 366 84870", result.Phone);
        Assert.Equal("luigineri.com", result.Website);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_User()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        var expectedUser = new User { Id = 5, Name = "Marta Viola", Username = "martaviola", Email = "martaviola@gmail.com", Phone = "0039 040 955 98830", Website = "martaviola.com" };

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        // Imposta Moq.Dapper per intercettare le query
        mockConnection
            .SetupDapperAsync(conn => conn.QueryFirstOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expectedUser);

        var repository = new UserRepository(mockFactory.Object);

        // Act
        var result = await repository.UpdateAsync(expectedUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Marta Viola", result.Name);
        Assert.Equal("martaviola", result.Username);
        Assert.Equal("martaviola@gmail.com", result.Email);
        Assert.Equal("0039 040 955 98830", result.Phone);
        Assert.Equal("martaviola.com", result.Website);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_User()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();

        // Imposta il mock per restituire la connessione mockata
        mockFactory.Setup(db => db.CreateConnection()).Returns(mockConnection.Object);

        mockConnection
            .SetupDapperAsync(conn => conn.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(1);

        var repository = new UserRepository(mockFactory.Object);

        // Act
        var result = await repository.DeleteAsync(123);

        Assert.Equal(1, result);
    }
}