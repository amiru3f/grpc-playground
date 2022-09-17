using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
});
builder.Services.AddGrpcReflection();

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.ListenAnyIP(80, opt => opt.Protocols = HttpProtocols.Http1);
    kestrel.ListenAnyIP(8080, opt => opt.Protocols = HttpProtocols.Http2);
});
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.MapGrpcService<ProductAppService>();
app.MapGrpcReflectionService();
app.MapGet("/", () => "Grpc trasncoding is enabled. Use Postman please");

await app.RunAsync();