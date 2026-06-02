using System;
using Grpc.Net.Client;
using GrpcGreeterClient;
using System.Threading;
using Microsoft.Extensions.Configuration;

string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var serverPort = config["ServerPort"] ?? "50000";

using var channel = GrpcChannel.ForAddress($"http://localhost:{serverPort}");

try
{
  using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
  await channel.ConnectAsync(cts.Token);

  if (channel.State == Grpc.Core.ConnectivityState.Ready)
  {
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "World" });
  }
}
catch (OperationCanceledException)
{
  // Server is unreachable or timed out
}
