using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web;

/// <summary>
/// Authentication builder specific for Microsoft identity platform.
/// </summary>
public class VerifiableCredentialsBuilder : IVerifiableCredentialsBuilder
{
    /// <summary>
    /// The services being configured.
    /// </summary>
    public IServiceCollection Services { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="services">The service collection being configured.</param>
    public VerifiableCredentialsBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Add distributed request caches.
    /// </summary>
    /// <returns>the service collection.</returns>
    public IVerifiableCredentialsBuilder AddDistributedTokenCaches()
    {
        Services.AddDistributedTokenCaches();
        return this;
    }
    
    public IVerifiableCredentialsBuilder WithIssuanceRequests(IConfiguration config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }
        
        Services.Configure<List<IssuanceRequestOptions>>(config);
        return this;
    }
}
