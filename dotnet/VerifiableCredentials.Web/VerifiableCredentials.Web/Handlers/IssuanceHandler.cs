using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web.Controllers;

/// <summary>
/// Controller used in web apps to manage accounts.
/// </summary>
[NonController]
[AllowAnonymous]
[Area("VerifiableCredentials")]
[Route("[area]/[controller]/[action]")]
public class IssuanceController : Controller
{
    private readonly IVerifiableCredentialsService _service;

    /// <summary>
    /// Constructor of <see cref="IssuanceController"/>
    /// This constructor is used by dependency injection.
    /// </summary>
    /// <param name="service">IVerifiableCredentialsService object containing a list of credentials requests available to be issued.</param>
    public IssuanceController(IVerifiableCredentialsService service)
    {
        _service = service;
    }

    /// <summary>
    /// Issues a verifiable credential
    /// </summary>
    /// <param name="credentialType">The type (i.e. name) of the requested credential</param>
    /// <returns>Issuance response containing the result of the issuance.</returns>
    [HttpGet("{credentialType?}")]
    async Task<ActionResult<IssuanceResponse>> Request([FromRoute] string credentialType)
    {
        var response = await _service.IssueCredentialAsync(credentialType);
        return Ok(response);
    }
}