Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Security
Imports System.Security.Claims
Imports System.Security.Permissions
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.Owin

Partial Public Class About
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        If Not Request.IsAuthenticated Then
            HttpContext.Current.GetOwinContext().Authentication.Challenge()
        End If

        Dim claims = ClaimsPrincipal.Current.Claims
        dlClaims.DataSource = claims
        dlClaims.DataBind()

    End Sub
End Class


Partial Public Class Claims

    ''' <summary>
    ''' dlClaims control.
    ''' </summary>
    ''' <remarks>
    ''' Auto-generated field.
    ''' To modify move field declaration from designer file to code-behind file.
    ''' </remarks>
    Protected dlClaims As Global.System.Web.UI.WebControls.DataList
End Class