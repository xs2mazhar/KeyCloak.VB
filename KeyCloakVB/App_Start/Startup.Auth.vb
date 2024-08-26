Imports Microsoft.IdentityModel.Logging
Imports Microsoft.IdentityModel.Protocols.OpenIdConnect
Imports Microsoft.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Owin
Imports System.Configuration
Imports System.Net
Imports OpenAthens.Owin.Security.OpenIdConnect
Imports System.Threading.Tasks
Imports Microsoft.IdentityModel.Tokens


Namespace KeyCloakVB
    Partial Public Class Startup
        Public Shared ReadOnly Property OidcAuthority As String = ConfigurationManager.AppSettings("oidc:Authority")
        Public Shared ReadOnly Property OidcRedirectUrl As String = ConfigurationManager.AppSettings("oidc:RedirectUrl")
        Public Shared ReadOnly Property OidcClientId As String = ConfigurationManager.AppSettings("oidc:ClientId")
        Public Shared ReadOnly Property OidcClientSecret As String = ConfigurationManager.AppSettings("oidc:ClientSecret")

        Public Shared ReadOnly Property CodeChallange As String = ConfigurationManager.AppSettings("CodeChallenge")

        Public Shared ReadOnly Property CodeVerify As String = ConfigurationManager.AppSettings("CodeVerify")

        Public Sub ConfigureAuth(app As IAppBuilder)
            ServicePointManager.Expect100Continue = True
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            IdentityModelEventSource.ShowPII = True

            app.UseCookieAuthentication(New CookieAuthenticationOptions With {
                .AuthenticationType = "keycloak_sso_auth",
                .CookieHttpOnly = True,
                .CookieName = "keycloak_cookie",
                .CookieSecure = CookieSecureOption.Always,
                .CookieDomain = "localhost",
                .CookiePath = "/"
            })

            app.SetDefaultSignInAsAuthenticationType("keycloak_sso_auth")

            app.UseOpenIdConnectAuthentication(New OpenIdConnectAuthenticationOptions() With {
                .Authority = OidcAuthority,
                .ClientId = OidcClientId,
                .ClientSecret = OidcClientSecret,
                .ResponseType = OpenIdConnectResponseType.IdTokenToken,
                .Scope = "openid profile email",
                .RedirectUri = OidcRedirectUrl,
                .RequireHttpsMetadata = False,
                .TokenValidationParameters = New TokenValidationParameters() With {
                        .NameClaimType = "name",
                        .RoleClaimType = "role"
                },
                .Notifications = New OpenIdConnectAuthenticationNotifications() With {
                    .SecurityTokenValidated = Function(context)
                                                  ' Additional claims or logic
                                                  Dim idToken = context.ProtocolMessage.IdToken
                                                  Dim accessToken = context.ProtocolMessage.AccessToken

                                                  ' Add the token to the user's claims
                                                  Dim identity = context.AuthenticationTicket.Identity
                                                  identity.AddClaim(New System.Security.Claims.Claim("id_token", idToken))
                                                  identity.AddClaim(New System.Security.Claims.Claim("access_token", accessToken))
                                                  Return Task.FromResult(0)

                                              End Function,
                    .AuthorizationCodeReceived = Async Function(context)
                                                     context.TokenEndpointRequest.Parameters("code_verifier") = CodeVerify
                                                 End Function,
                    .RedirectToIdentityProvider = Async Function(context)
                                                      context.ProtocolMessage.Parameters("code_challenge") = CodeChallange
                                                      context.ProtocolMessage.Parameters("code_challenge_method") = "plain"
                                                  End Function
                }
            })
        End Sub
    End Class
End Namespace
