using IdentityServer4.Models;

public  class Config
{
    public static IEnumerable<Client> Clients =>

           new Client[]
           {
                new Client
                {
                    ClientId = "Avinexuser",
                     AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("am@avinex".Sha256())
                        },
                        AllowedScopes = { "revampApi" }

                }
           };
    public static IEnumerable<ApiScope> ApiScopes =>
          new ApiScope[]
          {
                       new ApiScope("revampApi", "Revamp_Ank_App")
          };
}