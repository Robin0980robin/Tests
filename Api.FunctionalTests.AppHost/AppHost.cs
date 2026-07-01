var builder = DistributedApplication.CreateBuilder(args);

builder.AddSqlServer("bdserver").WithLifetime(ContainerLifetime.Session).AddDatabase("bd");

builder.Build().Run();
