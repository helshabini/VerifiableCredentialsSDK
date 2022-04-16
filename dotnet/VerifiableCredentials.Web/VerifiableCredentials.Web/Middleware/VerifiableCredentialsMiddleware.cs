using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace VerifiableCredentials.Web.Middleware;

public class VerifiableCredentialsMiddleware
{
    private readonly RequestDelegate _next;
    
    public VerifiableCredentialsMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }
    
    public async Task Invoke(HttpContext context)
    {
        context.Features.Set<IApplicationFeatureProvider<ControllerFeature>>(new VerifiableCredentialsFeatureProvider());

        await _next(context);
    }
}