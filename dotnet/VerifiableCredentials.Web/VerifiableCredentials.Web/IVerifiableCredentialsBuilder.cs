using Microsoft.Extensions.Configuration;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsBuilder
{
    IVerifiableCredentialsBuilder WithIssuanceRequest(string configName, Action<IssuanceRequestOptions> options);
    
    IVerifiableCredentialsBuilder WithIssuanceRequest(string configName, IConfiguration config);

    IVerifiableCredentialsService Build();
}