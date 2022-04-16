using Microsoft.AspNetCore.Mvc;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsService
{
    Task<IssuanceResponse> IssueCredentialAsync(string credentialType);
    
    Task<IssuanceResponse> IssueCredentialAsync(string credentialType, Dictionary<string, string> claims);
}