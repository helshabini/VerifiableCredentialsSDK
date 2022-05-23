using Microsoft.AspNetCore.Http;
using VerifiableCredentials.Web.Handlers;

namespace VerifiableCredentials.Web.Middleware;

public class VerifiableCredentialsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IssuanceHandler _handler;
    
    public VerifiableCredentialsMiddleware(RequestDelegate next, IssuanceHandler handler)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _handler = handler;
    }
    
    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path != null && path.Contains(Constants.IssuancePath, StringComparison.InvariantCultureIgnoreCase))
        {
            if (path.Contains(Constants.IssuancePath + "/request", StringComparison.InvariantCultureIgnoreCase))
            {
                await _handler.Request(context);
            }
            if (path.Contains(Constants.IssuancePath + "/status", StringComparison.InvariantCultureIgnoreCase))
            {
                await _handler.Status(context);
            }
            if (path.Contains(Constants.IssuancePath + "/callback", StringComparison.InvariantCultureIgnoreCase))
            {
                await _handler.Callback(context);
            }
        }

        await _next(context);
    }
}