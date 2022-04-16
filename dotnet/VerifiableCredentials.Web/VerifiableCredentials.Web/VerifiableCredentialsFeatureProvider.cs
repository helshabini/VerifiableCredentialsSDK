using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using VerifiableCredentials.Web.Controllers;

namespace VerifiableCredentials.Web;

internal class VerifiableCredentialsFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (!feature.Controllers.Contains(typeof(IssuanceController).GetTypeInfo()))
        {
            feature.Controllers.Add(typeof(IssuanceController).GetTypeInfo());
        }
    }
}