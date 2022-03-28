using Microsoft.Extensions.Configuration;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsBuilder
{
    IVerifiableCredentialsBuilder WithIssuanceRequest(Action<IssuanceRequestOptions> options);
    
    IVerifiableCredentialsBuilder WithIssuanceRequest(IConfiguration config);

    IVerifiableCredentialsService Build();
}