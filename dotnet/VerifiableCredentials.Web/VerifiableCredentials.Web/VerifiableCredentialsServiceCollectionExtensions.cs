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
}