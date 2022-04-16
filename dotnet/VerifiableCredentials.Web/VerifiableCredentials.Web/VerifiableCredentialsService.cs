using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifiableCredentials.Web.Helpers;
using VerifiableCredentials.Web.Issuance;

namespace VerifiableCredentials.Web;

public class VerifiableCredentialsService : IVerifiableCredentialsService
{
    public Dictionary<string, IssuanceRequest> IssuanceRequests { get; }
    private IDistributedCache _cache { get; set; }

    public VerifiableCredentialsService(IOptionsMonitor<List<IssuanceRequestOptions>> options, IDistributedCache cache)
    {
        IssuanceRequests = new Dictionary<string, IssuanceRequest>();
        foreach (var option in options.CurrentValue)
        {
            IssuanceRequests.Add(option.CredentialType, new IssuanceRequest(option));
        }

        _cache = cache;
    }

    public async Task<IssuanceResponse> IssueCredentialAsync(string credentialType)
    {
        if (!IssuanceRequests.ContainsKey(credentialType))
        {
            throw new ArgumentException(message: "Credential type not found.", paramName: credentialType);
        }

        var template = IssuanceRequests[credentialType];
        
        var authResult = AcquireToken(template);
        Console.WriteLine(authResult);
        
        var request = template.Clone() as IssuanceRequest ?? throw new InvalidOperationException();
        
        request.Callback = new Callback()
        {
            Url = new Uri("https://299a-197-237-248-192.ngrok.io/verifiablecredentials/issuance"),
            State = Guid.NewGuid().ToString(),
            Headers = new Headers()
            {
                ApiKey = Guid.NewGuid().ToString()
            }
        };
        
        if (authResult != null)
        {
            var httpClient = new HttpClient();
            var result = await CallWebApiAndProcessResultAsync(httpClient, string.Format(Constants.Endpoint, request.Options.TenantId), authResult.Result.AccessToken, request);
            return result;
        }

        return null;
    }

    public Task<IssuanceResponse> IssueCredentialAsync(string credentialType, Dictionary<string, string> claims)
    {
            throw new NotImplementedException();

        throw new ArgumentException(message: "Credential type not found.", paramName: credentialType);
    }

    private async Task<AuthenticationResult?> AcquireToken(IssuanceRequest request)
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
    
    public async Task<IssuanceResponse> CallWebApiAndProcessResultAsync(HttpClient httpClient, string webApiUrl, string accessToken, IssuanceRequest request)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            var defaultRequestHeaders = httpClient.DefaultRequestHeaders;
            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            string body = string.Empty;
            try
            {
                body = JsonConvert.SerializeObject(request);
                await _cache.SetAsync(request.Callback.State, Converters.JsonToByteArray(body), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine(e);
                throw;
            }
            StringContent httpContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(webApiUrl, httpContent);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                IssuanceResponse? result = IssuanceResponse.FromJson(json);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (result != null) return result;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to call the web API: {response.StatusCode}");
                string content = await response.Content.ReadAsStringAsync();

                // Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
                // this is because the tenant admin as not granted consent for the application to call the Web API
                Console.WriteLine($"Content: {content}");
            }
            Console.ResetColor();
        }

        return null;
    }
}