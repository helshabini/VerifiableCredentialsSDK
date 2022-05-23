using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VerifiableCredentials.Web.Handlers;
using VerifiableCredentials.Web.Middleware;

namespace VerifiableCredentials.Web;

public static class VerifiableCredentialsServiceCollectionExtensions
{
    public static VerifiableCredentialsBuilder AddVerifiableCredentials(
        this IServiceCollection services)
    {
        services.AddSingleton<IVerifiableCredentialsService, VerifiableCredentialsService>();
        services.AddSingleton<IssuanceHandler>();
        return new VerifiableCredentialsBuilder(services);
    }
    
    public static IApplicationBuilder UseVerifiableCredentials(
        this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<VerifiableCredentialsMiddleware>();
    }
}