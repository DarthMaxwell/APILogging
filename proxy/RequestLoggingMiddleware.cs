using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch sw = Stopwatch.StartNew();
        string ip = context.Connection.RemoteIpAddress.ToString();
        string method = context.Request.Method;
        string path = context.Request.Path;
        bool hasAPIKey = context.Request.Headers["Authorization"].FirstOrDefault() != null;

        _logger.LogInformation($"IN:Proxy {method} {path} from {ip} {((hasAPIKey) ? "with key" : "without key")}");

        await _next(context);

        sw.Stop();

        _logger.LogInformation($"OUT:Proxy {method} {path} {context.Response.StatusCode} {sw.ElapsedMilliseconds}ms from {ip}");
    }
}