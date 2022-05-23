using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web.Handlers;

public class IssuanceHandler
{
    private readonly IVerifiableCredentialsService _service;

    public IssuanceHandler(IVerifiableCredentialsService service)
    {
        _service = service;
    }
    public async Task Request(HttpContext context)
    {
        var path = context.Request.Path.Value;
        var credentialType = path?[(path.LastIndexOf('/') + 1)..];
        if (credentialType == null)
        {
            throw new Exception();
            //Should return 404 not found
        }
        
        var baseUrl = new Uri(context.Request.Scheme + "://" + context.Request.Host);
        
        var json = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        IssuanceStatus? status;
        
        if (claims == null)
            status = await _service.IssueCredentialAsync(credentialType, baseUrl);
        else
            status = await _service.IssueCredentialAsync(credentialType, baseUrl, claims);
        
        if (status == null)
            throw new Exception();
        //handle this
        
        status.ApiKey = null;
        context.Response.Clear();
        await context.Response.WriteAsJsonAsync(status.ToJson());
        await context.Response.CompleteAsync();
    }
    
    public async Task Status(HttpContext context)
    {
        string requestId = context.Request.Query["id"];

        if (string.IsNullOrEmpty(requestId))
        {
            throw new Exception();
            //should return 404 not found
        }
        
        var status = await _service.RequestStatusAsync(requestId);
        if (status == null)
            throw new Exception();
            //handle this
        
        status.ApiKey = null;
        context.Response.Clear();
        await context.Response.WriteAsJsonAsync(status.ToJson());
        await context.Response.CompleteAsync();
    }
    
    public async Task Callback(HttpContext context)
    {
        context.Request.Headers.TryGetValue("api-key", out var apiKey);
        
        var json = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var callback = IssuanceCallback.FromJson(json);

        if (callback == null)
        {
            //Should return a Bad Request
        }

        var service = _service as VerifiableCredentialsService;
        //Should try/catch this call and return equivalent http code
        var status = await service?.UpdateStatusAsync(callback, apiKey);

        context.Response.Clear();
        await context.Response.CompleteAsync();
        //Raise event for callback received.
    }
}