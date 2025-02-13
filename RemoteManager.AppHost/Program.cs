var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.RemoteMangerAPI>("remotemangerapi");

builder.Build().Run();
