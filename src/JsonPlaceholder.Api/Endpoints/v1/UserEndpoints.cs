using JsonPlaceholder.Core.Entities;
using JsonPlaceholder.Core.Interfaces;
using JsonPlaceholder.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JsonPlaceholder.Api.Endpoints.v1;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // GET ALL
        app.MapGet("/", async ([FromServices] IUserRepository userRepository) =>
        {
            var users = await userRepository.GetAllAsync();

            return Results.Ok(users);
        })
        .WithSummary("Get Users")
        .Produces<IEnumerable<User>>(StatusCodes.Status200OK);

        // GET ID
        app.MapGet("/{id}", async ([FromRoute] int id, [FromServices] IUserRepository userRepository) =>
        {
            var user = await userRepository.GetByIdAsync(id);

            return user != null ? Results.Ok(user) : Results.NotFound();
        })
        .WithSummary("Get User by Id")
        .Produces<User>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // CREATE
        app.MapPost("/", async ([FromBody] CreateUserDto dto, [FromServices] IUserRepository userRepository) =>
        {
            var user = new User
            { 
                Id = dto.Id, Name = dto.Name, Username = dto.Username, 
                Email = dto.Email, Phone = dto.Phone, Website = dto.Website
            };

            var newUser = await userRepository.CreateAsync(user);

            return Results.Ok(newUser);
        })
        .WithSummary("Create new User")
        .Produces<int>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // UPDATE
        app.MapPut("/{id}", async ([FromRoute] int id, [FromBody] UpdateUserDto dto, [FromServices] IUserRepository userRepository) =>
        {
            var user = new User
            { 
                Id = dto.Id, Name = dto.Name, Username = dto.Username, 
                Email = dto.Email, Phone = dto.Phone, Website = dto.Website
            };

            var updatedUser = await userRepository.UpdateAsync(user);

            return updatedUser != null ? Results.Ok(updatedUser) : Results.NotFound();
        })
        .WithSummary("Uodate User")
        .Produces<User>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE
        app.MapDelete("/{id}", async ([FromRoute] int id, IUserRepository userRepository) =>
        {
            var numRows = await userRepository.DeleteAsync(id);

            return numRows > 0 ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete User")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}