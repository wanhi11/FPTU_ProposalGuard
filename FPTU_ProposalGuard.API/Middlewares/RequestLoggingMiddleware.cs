using FPTU_ProposalGuard.API.Extensions;
using ILogger = Serilog.ILogger;

namespace FPTU_ProposalGuard.API.Middlewares;

public class RequestLoggingMiddleware(ILogger logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        var correlationGenerator = new CorrelationIdGenerator();
        string request = string.Empty;
        if (ctx.Request.ContentType == "application/json")
        {
            request = await ctx.ReadRequestBodyAsStringAsync();
        }
        
        logger.Information("[{RequestTime}][{Correlation}][{Method}]: Path: {Path}, Request {Request}",
            DateTime.UtcNow,
            correlationGenerator.Get(),
            ctx.Request.Method,
            ctx.Request.Path + ctx.Request.QueryString,
            request);
    }    
}

public class CorrelationIdGenerator
{
    private string _correlationId = Guid.NewGuid().ToString();
    public string Get() => _correlationId;
    public void Set(string correlationId)
    {
        _correlationId = correlationId;
    }
}