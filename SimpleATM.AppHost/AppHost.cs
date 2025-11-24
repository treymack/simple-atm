using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<SimpleATM_Api>("api")
    .WithUrl("/swagger");

builder.Build().Run();
