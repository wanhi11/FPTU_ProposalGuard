namespace FPTU_ProposalGuard.API.Extensions;

public static class HttpContextExtensions
{
    public static async Task<string> ReadRequestBodyAsStringAsync(this HttpContext ctx)
    {
        using var reader = new StreamReader(ctx.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync(); 
        ctx.Request.Body.Position = 0;
        return body;
    }
}