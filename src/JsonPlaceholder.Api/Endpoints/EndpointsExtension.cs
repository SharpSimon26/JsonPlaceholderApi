using JsonPlaceholder.Api.Endpoints.v1;

namespace JsonPlaceholder.Api.Endpoints;

public static class EndpointsExtension
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api");
        var v1Group = apiGroup.MapGroup("/v1");
        
        v1Group.MapGroup("/users").WithTags("JSONPlaceholder Users").MapUserEndpoints();
        v1Group.MapGroup("/posts").WithTags("JSONPlaceholder Posts").MapPostEndpoints();

        return app;
    }
}