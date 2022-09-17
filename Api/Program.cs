using Microsoft.AspNetCore.Server.Kestrel.Core;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.ListenAnyIP(80, opt => opt.Protocols = HttpProtocols.Http1);
    kestrel.ListenAnyIP(8080, opt => opt.Protocols = HttpProtocols.Http2);
});
var app = builder.Build();


app.MapGrpcService<ProductAppService>();
app.MapGrpcReflectionService();

await app.RunAsync();