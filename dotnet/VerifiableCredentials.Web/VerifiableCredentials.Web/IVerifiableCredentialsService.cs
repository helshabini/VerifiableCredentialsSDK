using Microsoft.AspNetCore.Mvc;

namespace VerifiableCredentials.Web;

public interface IVerifiableCredentialsService
{
    Task<IActionResult> IssueCredentialAsync(string credentialType);
    
    Task<IActionResult> IssueCredentialAsync(string credentialType, Dictionary<string, string> claims);
}