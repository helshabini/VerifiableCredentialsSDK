using Microsoft.Extensions.Configuration;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsBuilder
{
    IVerifiableCredentialsBuilder AddDistributedTokenCaches();
    IVerifiableCredentialsBuilder WithIssuanceRequests(IConfiguration config);
}