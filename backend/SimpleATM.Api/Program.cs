using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using SimpleATM.Domain.DTOs;
using SimpleATM.Domain.Services;
using SimpleATM.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

builder.Services.AddTransient<IAccountsService, AccountsService>();
builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();
builder.AddMySqlDataSource("bank");
// This may leak connections, I'm not sure.
builder.Services.AddScoped(sp => sp.GetRequiredService<MySqlDataSource>().OpenConnection());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<IAccountsRepository>().WipeAndSeedDatabaseAsync();
}

app.MapDefaultEndpoints();
app.MapGet("/", () => "Hello World!");

app.MapGet("/accounts", async ([FromServices] IAccountsService accountsService) =>
{
    return (await accountsService.GetAccountsAsync()).ToMinimalApiResult();
});

app.MapGet("/accounts/{id}", async ([FromServices] IAccountsService accountsService, int id) =>
{
    return (await accountsService.GetAccountDetailsAsync(id)).ToMinimalApiResult();
});

app.MapPost("/accounts/{accountId}/deposit",
    async ([FromServices] IAccountsService accountsService, int accountId, DepositRequest depositRequest) =>
{
    return (await accountsService.DepositAsync(accountId, depositRequest)).ToMinimalApiResult();
});

app.MapPost("/accounts/{accountId}/withdrawal",
    async ([FromServices] IAccountsService accountsService, int accountId, WithdrawalRequest withdrawalRequest) =>
{
    return (await accountsService.WithdrawAsync(accountId, withdrawalRequest)).ToMinimalApiResult();
});

app.MapPost("/accounts/transfer",
    async ([FromServices] IAccountsService accountsService, TransferRequest transferRequest) =>
{
    return (await accountsService.TransferAsync(transferRequest));
});

app.Run();
