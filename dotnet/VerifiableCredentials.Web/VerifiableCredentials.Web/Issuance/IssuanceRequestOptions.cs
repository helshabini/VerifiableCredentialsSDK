namespace VerifiableCredentials.Web.Issuance;

public class IssuanceRequestOptions
{
    public string Authority { get; set; } = null!;
    public bool IncludeQrCode { get; set; }
    public string ClientName { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// Gets the OpenId Authority.
    /// </summary>
    public string OpenIdAuthority { get; set; } = null!;

    /// <summary>
    /// Gets or sets the 'client_id'.
    /// </summary>
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the 'client_secret'.
    /// </summary>
    public string ClientSecret { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the 'certificate_name'.
    /// </summary>
    public string CertificateName { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the 'CredentialType'.
    /// <example>VerifiableCredentialsExpert</example>
    /// </summary>
    public string CredentialType { get; set; } = null!;

    public string LogoUrl { get; set; } = null!;
    public string TermsOfServiceUrl { get; set; } = null!;
}