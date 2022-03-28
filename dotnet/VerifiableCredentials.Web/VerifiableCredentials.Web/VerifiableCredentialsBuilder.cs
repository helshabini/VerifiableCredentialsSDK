using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    private readonly List<IssuanceRequestOptions> _issuanceRequestOptionsList;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="services">The service collection being configured.</param>
    public VerifiableCredentialsBuilder(IServiceCollection services)
    {
        Services = services;
        _issuanceRequestOptionsList = new List<IssuanceRequestOptions>();
    }

    public IVerifiableCredentialsBuilder WithIssuanceRequest(Action<IssuanceRequestOptions> configureOptions)
    {
        throw new NotImplementedException();
        //Services.Configure<IssuanceRequestOptions>(configureOptions);
        //return this;
    }

    public IVerifiableCredentialsBuilder WithIssuanceRequest(IConfiguration config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }
        
        var options = new IssuanceRequestOptions();
        config.Bind(options);
        _issuanceRequestOptionsList.Add(options);
        
        Services.Configure<IssuanceRequestOptions>(config);
        return this;
    }

    public IVerifiableCredentialsService Build()
    {
        return new VerifiableCredentialsService(_issuanceRequestOptionsList);
    }
}
