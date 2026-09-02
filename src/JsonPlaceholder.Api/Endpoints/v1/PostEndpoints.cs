using JsonPlaceholder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JsonPlaceholder.Api.Endpoints.v1;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async ([FromServices] IPostRepository postRepository) =>
        {
            var posts = await postRepository.GetAllAsync();

            return posts;
        })
        .WithSummary("Get Posts");

        return app;
    }
}