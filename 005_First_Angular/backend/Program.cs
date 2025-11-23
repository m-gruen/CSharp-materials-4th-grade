using TankMuseum.Core;
using TankMuseum.Core.Exhibits;
using TankMuseum.Core.News;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterServices();
builder.Services.ConfigureServices(builder.Environment.IsDevelopment());
builder.Services.ConfigureCors();

var app = builder.Build();

app.UseCors(Const.CorsPolicyName);

app.MapNewsEndpoint();
app.MapExhibitEndpoint();

await app.RunAsync();