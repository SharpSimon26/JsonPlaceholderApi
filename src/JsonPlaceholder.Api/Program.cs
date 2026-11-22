using JsonPlaceholder.Api.Endpoints;
using JsonPlaceholder.Api.Exceptions;
using JsonPlaceholder.Core.Interfaces;
using JsonPlaceholder.DataAccess.Database;
using JsonPlaceholder.DataAccess.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

// Registra il supporto standard ProblemDetails e l'handler custom
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Abilita i servizi di validazione
builder.Services.AddValidation();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "JSONPlaceholder API";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Inserisce il middleware di gestione eccezioni nella pipeline
app.UseExceptionHandler();

app.MapEndpoints();

await app.RunAsync();
