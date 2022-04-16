using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace VerifiableCredentials.Web;

public static class VerifiableCredentialsServiceCollectionExtensions
{
    public static VerifiableCredentialsBuilder AddVerifiableCredentials(
        this IServiceCollection services)
    {
        services.AddSingleton<IVerifiableCredentialsService, VerifiableCredentialsService>();
        return new VerifiableCredentialsBuilder(services);
    }

    public static IMvcBuilder UseVerifiableCredentials(this IMvcBuilder builder)
    {
        builder.Services.AddMvc().PartManager.FeatureProviders.Add(new VerifiableCredentialsFeatureProvider());
        return builder;
    }
    
    public static IApplicationBuilder UseVerifiableCredentials(
        this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        /*if (app is WebApplication)
        {
            var webapp = app as WebApplication;
            webapp?.MapAreaControllerRoute(
                name: "issuance",
                areaName: "VerifiableCredentials",
                pattern: "{controller=Issuance}/{action=request}/{credentialType?}");
        }*/

        return app.MapWhen(
            context => context.Request.Path.Value.Contains(Constants.IssuancePath, StringComparison.InvariantCultureIgnoreCase),
            HandleIssuance);
        //return app.UseMiddleware<VerifiableCredentialsMiddleware>();
    }

    internal static void HandleIssuance(IApplicationBuilder app)
    {
        var x = 2;
        //throw new NotImplementedException();
    }
}