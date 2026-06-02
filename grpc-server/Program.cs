using grpc_server.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

var serverPort = ushort.Parse(builder.Configuration.GetValue<string>("ServerPort") ?? "50000");

// Add services to the container.
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
  // Setup an HTTPS endpoint for production (Recommended)
  options.ListenLocalhost(serverPort, listenOptions =>
  {
    listenOptions.Protocols = HttpProtocols.Http2;
    // listenOptions.UseHttps(); // Uncomment for TLS/HTTPS support
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
