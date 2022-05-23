using Microsoft.AspNetCore.Http;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsService
{
    Task<IssuanceStatus?> IssueCredentialAsync(string credentialType, Uri baseUrl);
    
    Task<IssuanceStatus?> IssueCredentialAsync(string credentialType, Uri baseUrl, Dictionary<string, string> claims);

    Task<IssuanceStatus?> RequestStatusAsync(string requestId);
}