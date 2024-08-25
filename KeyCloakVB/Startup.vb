Imports Microsoft.Owin
Imports Owin

<Assembly: OwinStartup(GetType(KeyCloakVB.Startup))>
Namespace KeyCloakVB
    Partial Public Class Startup
        Public Sub Configuration(app As IAppBuilder)
            ConfigureAuth(app)
        End Sub
    End Class
End Namespace
