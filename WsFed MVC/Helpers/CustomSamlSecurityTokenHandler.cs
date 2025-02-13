using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;

namespace WsFed_MVC.Helpers;

public class CustomSamlSecurityTokenHandler : SamlSecurityTokenHandler
{
    public override async Task<TokenValidationResult> ValidateTokenAsync(string token, TokenValidationParameters validationParameters)
    {
        var configuration = await validationParameters.ConfigurationManager.GetBaseConfigurationAsync(CancellationToken.None).ConfigureAwait(false);
        var issuers = new[] { configuration.Issuer };
        validationParameters.ValidIssuers = validationParameters.ValidIssuers == null ? issuers : validationParameters.ValidIssuers.Concat(issuers);
        validationParameters.IssuerSigningKeys = validationParameters.IssuerSigningKeys == null ? configuration.SigningKeys : validationParameters.IssuerSigningKeys.Concat(configuration.SigningKeys);

        return await base.ValidateTokenAsync(token, validationParameters).ConfigureAwait(false);
    }
}