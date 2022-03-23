using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    public IVerifiableCredentialsBuilder WithIssuanceRequest(string configName, Action<IssuanceRequestOptions> configureOptions)
    {
        //Services.AddOptions<IssuanceRequestOptions>(configName).Bind(configureOptions);
        return this;
    }

    public IVerifiableCredentialsBuilder WithIssuanceRequest(string configName, IConfiguration config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }
        
        Services.AddOptions<IssuanceRequestOptions>(configName).Bind(config);

        return this;
    }

    public IVerifiableCredentialsService Build()
    {
        throw new NotImplementedException();
    }
}
