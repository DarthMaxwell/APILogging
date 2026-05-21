using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config // can put this in appsettings
    .MinimumLevel.Information() 
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")); // put in developmnt and make example one for github

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.MapReverseProxy();

app.Run();
