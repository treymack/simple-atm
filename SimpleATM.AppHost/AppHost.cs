using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
    .WithPhpMyAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var db = mysql.AddDatabase("bank");
//.WithMigrationsAssembly("SimpleATM.Infrastructure")
//.WithDbContextFactory<SimpleATM.Infrastructure.AppDbContextFactory>();

builder.AddProject<SimpleATM_Api>("api")
    .WithUrl("/swagger")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
