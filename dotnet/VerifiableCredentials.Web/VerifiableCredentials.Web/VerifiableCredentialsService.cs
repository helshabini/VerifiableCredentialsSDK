using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using VerifiableCredentials.Web.Helpers;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web;

public class VerifiableCredentialsService : IVerifiableCredentialsService
{
    public Dictionary<string, IssuanceRequest> IssuanceRequests { get; }
    private IDistributedCache Cache { get; }

    public VerifiableCredentialsService(IOptionsMonitor<List<IssuanceRequestOptions>> options, IDistributedCache cache)
    {
        IssuanceRequests = new Dictionary<string, IssuanceRequest>();
        foreach (var option in options.CurrentValue)
        {
            IssuanceRequests.Add(option.CredentialType, new IssuanceRequest(option));
        }

        Cache = cache;
    }

    public async Task<IssuanceStatus?> IssueCredentialAsync(string credentialType, Uri baseUrl)
    {
        if (!IssuanceRequests.ContainsKey(credentialType))
        {
            throw new ArgumentException(message: "Credential type not found.", paramName: credentialType);
        }

        var template = IssuanceRequests[credentialType];
        
        var authResult = AcquireToken(template);
        Console.WriteLine(authResult);

        if (authResult.Result == null) return null;
        
        var request = template.Clone() as IssuanceRequest ?? throw new InvalidOperationException();
        
        request.Callback = new Callback
        {
            Url = new Uri(baseUrl + Constants.IssuancePath + "/callback"),
            State = Guid.NewGuid().ToString(),
            Headers = new Headers
            {
                ApiKey = Guid.NewGuid().ToString()
            }
        };
        
        var response = await IssueRequestAsync(string.Format(Constants.Endpoint, request.Options.TenantId), authResult.Result!.AccessToken, request);
        
        if (response == null)
            throw new NullReferenceException("Invalid response.");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = IssuanceResponse.FromJson(json);

            if (result == null)
                throw new SerializationException("Could not serialize response");
                
            var status = new IssuanceStatus
            {
                RequestId = result.RequestId,
                Url = result.Url,
                QrCode = result.QrCode,
                Pin = request.Issuance.Pin?.Value ?? null,
                ApiKey = request.Callback.Headers?.ApiKey
            };
            await Cache.SetAsync(result.RequestId.ToString(), Converters.JsonToByteArray(status.ToJson()), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return status;
        }
        else
        {
            Console.WriteLine($"Failed to call the web API: {response.StatusCode}");
            var json  = await response.Content.ReadAsStringAsync();
            var result = IssuanceResponse.FromJson(json);

            if (result == null)
                throw new SerializationException("Could not serialize response.");
            
            var status = new IssuanceStatus
            {
                RequestId = result.RequestId,
                Error = result.Error
            };

            return status;
        }
    }
    
    public async Task<IssuanceStatus?> IssueCredentialAsync(string credentialType, Uri baseUrl, Dictionary<string, string> claims)
    {
        if (!IssuanceRequests.ContainsKey(credentialType))
        {
            throw new ArgumentException(message: "Credential type not found.", paramName: credentialType);
        }

        var template = IssuanceRequests[credentialType];
        
        var authResult = AcquireToken(template);
        Console.WriteLine(authResult);

        if (authResult.Result == null) return null;
        
        var request = template.Clone() as IssuanceRequest ?? throw new InvalidOperationException();
        
        request.Callback = new Callback
        {
            Url = new Uri(baseUrl + Constants.IssuancePath + "/callback"),
            State = Guid.NewGuid().ToString(),
            Headers = new Headers
            {
                ApiKey = Guid.NewGuid().ToString()
            }
        };

        request.Issuance.Claims = claims;
        request.Issuance.Pin = Pin.Generate(4);
        
        var response = await IssueRequestAsync(string.Format(Constants.Endpoint, request.Options.TenantId), authResult.Result!.AccessToken, request);
        
        if (response == null)
            throw new NullReferenceException("Invalid response.");
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsJsonAsync<IssuanceResponse>();
            //var result = IssuanceResponse.FromJson(json);

            if (result == null)
                throw new SerializationException("Could not serialize response");
                
            var status = new IssuanceStatus
            {
                RequestId = result.RequestId,
                Url = result.Url,
                QrCode = result.QrCode,
                Pin = request.Issuance.Pin?.Value ?? null,
                ApiKey = request.Callback.Headers?.ApiKey
            };
            await Cache.SetAsync(result.RequestId.ToString(), Converters.JsonToByteArray(status.ToJson()), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return status;
        }
        else
        {
            Console.WriteLine($"Failed to call the web API: {response.StatusCode}");
            var json  = await response.Content.ReadAsStringAsync();
            var result = IssuanceResponse.FromJson(json);

            if (result == null)
                throw new SerializationException("Could not serialize response.");
            
            var status = new IssuanceStatus
            {
                RequestId = result.RequestId,
                Error = result.Error
            };

            return status;
        }
    }
    
    private static async Task<AuthenticationResult?> AcquireToken(IssuanceRequest request)
    {
        IConfidentialClientApplication app;
        /*if (!string.IsNullOrWhiteSpace(request.Options.ClientId))
        {*/
            // Even if this is a console application here, a daemon application is a confidential client application
            app = ConfidentialClientApplicationBuilder.Create(request.Options.ClientId)
                .WithClientSecret(request.Options.ClientSecret)
                .WithAuthority(new Uri(string.Format(Constants.Instance, request.Options.TenantId)))
                .Build();
        /*}*/

        /*else
        {
            ICertificateLoader certificateLoader = new DefaultCertificateLoader();
            certificateLoader.LoadIfNeeded(request.Options.CertificateName);

            app = ConfidentialClientApplicationBuilder.Create(request.Options.ClientId)
                .WithCertificate(config.Certificate.Certificate)
                .WithAuthority(new Uri(string.Format(Constants.Instance, request.Options.TenantId)))
                .Build();
        }*/

        app.AddInMemoryTokenCache();
        
        // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
        // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
        // a tenant administrator
        string[] scopes = { Constants.VerifiableCredentialsServiceScope };

        AuthenticationResult? result = null;
        try
        {
            result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Token acquired \n");
            Console.ResetColor();
        }
        catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
        {
            // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
            // Mitigation: change the scope to be as expected
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Scope provided is not supported");
            Console.ResetColor();
        }

        return result;
    }

    private async Task<HttpResponseMessage?> IssueRequestAsync(string webApiUrl, string accessToken, IssuanceRequest request)
    {
        if (string.IsNullOrEmpty(accessToken)) return null;
        
        using var httpClient = new HttpClient();
        var defaultRequestHeaders = httpClient.DefaultRequestHeaders;
        defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync(webApiUrl, request);
        return response;
    }

    public async Task<IssuanceStatus?> RequestStatusAsync(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentNullException(nameof(requestId), "Request Id is required.");

        var data = await Cache.GetAsync(requestId, CancellationToken.None);
        if (data == null || data.Length == 0)
            throw new KeyNotFoundException($"Cannot find status for request with Id {requestId}");

        var json = Converters.ByteArrayToJson(data);

        if (json == null)
            throw new SerializationException($"Cannot serialize request with Id {requestId}");
                
        var cacheItem = IssuanceStatus.FromJson(json);
        return cacheItem;
    }

    internal async Task<IssuanceStatus?> UpdateStatusAsync(IssuanceCallback callback, string apiKey)
    {
        if (callback == null)
            throw new NullReferenceException($"Callback cannot be null.");
        
        var data = await Cache.GetAsync(callback.RequestId.ToString(), CancellationToken.None);
        if (data == null || data.Length == 0)
            throw new KeyNotFoundException($"Cannot find status for request with Id {callback.RequestId}");
        
        var json = Converters.ByteArrayToJson(data);

        if (json == null)
            throw new SerializationException($"Cannot serialize request status with Id {callback.RequestId}");
                
        var status = IssuanceStatus.FromJson(json);
        
        if (status == null)
            throw new SerializationException($"Cannot serialize request status with Id {callback.RequestId}");

        if (status.ApiKey != apiKey)
            throw new UnauthorizedAccessException($"Invalid ApiKey.");

        status.Code = callback.Code;
        callback.Error = callback.Error;

        await Cache.SetAsync(status.RequestId.ToString(), Converters.JsonToByteArray(status.ToJson()), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return status;
    }
}