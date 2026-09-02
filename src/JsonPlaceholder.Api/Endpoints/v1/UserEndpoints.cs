using JsonPlaceholder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JsonPlaceholder.Api.Endpoints.v1;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async ([FromServices] IUserRepository userRepository) =>
        {
            var users = await userRepository.GetAllAsync();

            return users;
        })
        .WithSummary("Get Users");

        return app;
    }
}