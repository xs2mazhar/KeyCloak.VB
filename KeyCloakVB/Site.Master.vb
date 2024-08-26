Imports System.Security.Permissions
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.Owin

Public Class SiteMaster
    Inherits MasterPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub login_Click(sender As Object, e As EventArgs)
        If Not Request.IsAuthenticated Then
            HttpContext.Current.GetOwinContext().Authentication.Challenge()
        End If
        Response.Redirect("~/About.aspx")
    End Sub

    Protected Sub Unnamed_LoggingOut(sender As Object, e As EventArgs)
        If Request.IsAuthenticated Then
            HttpContext.Current.GetOwinContext().Authentication.SignOut("keycloak_sso_auth")
        End If

        Dim authority As String = ConfigurationManager.AppSettings("oidc:Authority")
        Dim clientId As String = ConfigurationManager.AppSettings("oidc:ClientId")
        Dim postLogoutRedirectUri As String = ConfigurationManager.AppSettings("oidc:PostLogoutRedirectUri")

        ' Construct the logout URL
        Dim logoutUrl As String = $"{authority}/protocol/openid-connect/logout?client_id={clientId}&post_logout_redirect_uri={HttpUtility.UrlEncode(postLogoutRedirectUri)}"

        ' Redirect to Keycloak logout
        Response.Redirect(logoutUrl)

    End Sub
End Class