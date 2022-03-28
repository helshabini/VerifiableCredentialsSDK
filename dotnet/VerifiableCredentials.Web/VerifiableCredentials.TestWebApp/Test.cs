using VerifiableCredentials.Web;

namespace VerifiableCredentials.TestWebApp;

public interface ITest
{
    
}

public class Test : ITest
{
    public readonly IVerifiableCredentialsIssuer _issuer;

    public Test(IVerifiableCredentialsIssuer issuer)
    {
        _issuer = issuer;
    }

    public void aykalam()
    {
        _issuer
    }
    
}