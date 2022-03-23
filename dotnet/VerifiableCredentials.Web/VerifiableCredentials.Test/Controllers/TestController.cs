using Microsoft.AspNetCore.Mvc;
using VerifiableCredentials.Web;


namespace VerifiableCredentials.Test.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    //private readonly IVerifiableCredentialsIssuer _issuer;
    private readonly IVerifiableCredentialsService _vcService;

    public TestController(ILogger<TestController> logger, IVerifiableCredentialsService vcService)
    {
        _logger = logger;
        _vcService = vcService;
    }

    [HttpGet(Name = "Get")]
    public async Task<IActionResult> Get()
    {
        return Ok(_vcService.ToString());
    }
}