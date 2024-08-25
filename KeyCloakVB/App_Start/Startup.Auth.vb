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
                .ResponseType = OpenIdConnectResponseType.Code,
                .Scope = "openid",
                .RedirectUri = OidcRedirectUrl,
                .RequireHttpsMetadata = False,
                .Notifications = New OpenIdConnectAuthenticationNotifications() With {
                    .SecurityTokenValidated = Function(context)
                                                  ' Additional claims or logic
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
