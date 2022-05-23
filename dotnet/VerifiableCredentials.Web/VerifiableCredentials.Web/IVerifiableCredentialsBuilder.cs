using Microsoft.Extensions.Configuration;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsBuilder
{
    IVerifiableCredentialsBuilder AddDistributedTokenCaches();
    IVerifiableCredentialsBuilder WithIssuanceRequests(IConfiguration config);
}