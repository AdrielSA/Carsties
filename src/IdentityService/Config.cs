using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("auctionApp")
        ];

    public static IEnumerable<Client> Clients =>
        [
            new() {
                ClientId = "postman",
                ClientName = "postman",
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "https://www.getpostam.com/oauth2/callback" },
                ClientSecrets = [new Secret("NotASecret".Sha256())],
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword }
            },
            new() {
                ClientId = "nextApp",
                ClientName = "nextApp",
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "http://localhost:3000/api/auth/callback/id-server" },
                ClientSecrets = [new Secret("secret".Sha256())],
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequirePkce = false,
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600*24*30
            }
        ];
}
