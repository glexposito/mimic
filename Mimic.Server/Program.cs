using Mimic.Core.Engine;
using Mimic.Core.Routing;
using Mimic.Server.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args, new Dictionary<string, string>
{
    ["--config"] = "Mimic:ConfigPath"
});

builder.Services.AddSingleton<MockApiConfigLoader>();
builder.Services.AddSingleton<IMockApiConfigProvider, FileMockApiConfigProvider>();
builder.Services.AddSingleton<PathMatcher>();
builder.Services.AddSingleton<MockEngine>();

var app = builder.Build();

app.Map("/{**path}", async (string? path, HttpContext ctx, MockEngine engine, IMockApiConfigProvider configProvider) =>
{
    var method = ctx.Request.Method;
    var requestPath = path is null ? "/" : $"/{path}";
    var config = configProvider.GetCurrent();

    var response = engine.Handle(config, method, requestPath);

    if (response is null)
    {
        ctx.Response.StatusCode = StatusCodes.Status404NotFound;
        await ctx.Response.WriteAsync("No mock matched");
        return;
    }

    ctx.Response.StatusCode = response.Status;

    foreach (var header in response.Headers)
    {
        ctx.Response.Headers[header.Key] = header.Value;
    }

    if (HttpMethods.IsHead(method))
    {
        return;
    }

    await ctx.Response.WriteAsJsonAsync(response.Body);
});

app.Run();

public abstract partial class Program;
