using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace VerifiableCredentials.Web;

public class VerifiableCredentialsService : IVerifiableCredentialsService
{
    private readonly List<IssuanceRequestOptions> _issuanceRequests;

    public VerifiableCredentialsService(IEnumerable<IOptionsMonitor<IssuanceRequestOptions>> optionsList)
    {
        _issuanceRequests = new List<IssuanceRequestOptions>();

        foreach (var options in optionsList)
        {
            _issuanceRequests.Add(options.CurrentValue);
        }
    }

    public Task<IActionResult> IssueCredentialAsync(string credentialType)
    {
        if (_issuanceRequests.Find(r => r.CredentialType == credentialType) != null)
        {
            throw new NotImplementedException();
        }

        throw new ArgumentException(message: "Credential type not found.", paramName: credentialType);
    }

    public Task<IActionResult> IssueCredentialAsync(string credentialType, Dictionary<string, string> claims)
    {
        if (_issuanceRequests.Find(r => r.CredentialType == credentialType) != null)
        {
            throw new NotImplementedException();
        }

        throw new ArgumentException(message: "Credential type not found.", paramName: credentialType);
    }
}