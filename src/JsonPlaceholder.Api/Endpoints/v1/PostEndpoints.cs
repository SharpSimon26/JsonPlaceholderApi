using JsonPlaceholder.Core.Entities;
using JsonPlaceholder.Core.Interfaces;
using JsonPlaceholder.DataAccess.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JsonPlaceholder.Api.Endpoints.v1;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        // GET ALL
        app.MapGet("/", async ([FromQuery] int? userId, [FromServices] IPostRepository postRepository) =>
        {
            IEnumerable<Post> posts;

            if (userId.HasValue)
            {
                // Recupera solo i post dell'utente
                posts = await postRepository.GetByUserIdAsync(userId.Value);
            }
            else
            {
                posts = await postRepository.GetAllAsync();
            }

            return Results.Ok(posts);
        })
        .WithSummary("Get Posts")
        .Produces<IEnumerable<Post>>(StatusCodes.Status200OK);

        // GET ID
        app.MapGet("/{id}", async([FromRoute] int id, [FromServices] IPostRepository postRepository) =>
        {
            var post = await postRepository.GetByIdAsync(id);

            return post != null ? Results.Ok(post) : Results.NotFound();
        })
        .WithSummary("Get Post by Id")
        .Produces<User>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // CREATE
        app.MapPost("/", async ([FromBody] CreatePostDto dto, [FromServices] IPostRepository postRepository) =>
        {
            var post = new Post
            {
                Id = dto.Id,
                UserId = dto.UserId,
                Title = dto.Title,
                Body = dto.Body
            };

            var newPost = await postRepository.CreateAsync(post);

            return Results.Ok(newPost);
        })
        .WithSummary("Create new Post")
        .Produces<int>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // UPDATE
        app.MapPut("/{id}", async ([FromRoute] int id, [FromBody] UpdatePostDto dto, [FromServices] IPostRepository postRepository) =>
        {
            var post = new Post
            {
                Id = id, UserId = dto.UserId, Body = dto.Body, Title = dto.Title
            };

            var updatedPost = await postRepository.UpdateAsync(post);

            return updatedPost != null ? Results.Ok(updatedPost) : Results.NotFound();
        })
        .WithSummary("Update Post")
        .Produces<int>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // DELETE
        app.MapDelete("/{id}", async ([FromRoute] int id, IPostRepository postRepository) =>
        {
            var numRows = await postRepository.DeleteAsync(id);

            return numRows > 0 ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete Post")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}