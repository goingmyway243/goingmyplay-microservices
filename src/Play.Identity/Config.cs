using Duende.IdentityServer.Models;

namespace Play.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("catalog.fullaccess", "Catalog Full Access"),
            new ApiScope("payment.fullaccess", "Payment Full Access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // interactive client
            new Client
            {
                ClientId = "postman",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                
                // where to redirect to after login
                RedirectUris = { "https://oauth.pstmn.io/v1/callback" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://oauth.pstmn.io/v1/callback" },

                AllowOfflineAccess = true,

                AllowedScopes = { "openid", "profile", "catalog.fullaccess", "payment.fullaccess" }
            },
            // machine to machine client
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "catalog.fullaccess", "payment.fullaccess" }
            }
        };
}
