using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SimpleATM.Domain.Services;
using SimpleATM.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

builder.Services.AddTransient<IAccountsService, AccountService>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGet("/", () => "Hello World!");
app.MapGet("/accounts", ([FromServices] IAccountsService accountsService) =>
{
    return accountsService.GetAccounts().ToMinimalApiResult();
});

app.Run();
