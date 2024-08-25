<%@ Page Title="Claims" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.vb" Inherits="KeyCloakVB.About" %>
<%@ Import Namespace="System.Security.Claims" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>OpenID Connect Claims</h2>
    <dl>
        <asp:DataList runat="server" ID="dlClaims">
            <ItemTemplate>
                <dt><%# CType(Container.DataItem, Claim).Type %></dt>
                <dd><%# CType(Container.DataItem, Claim).Value %></dd>
            </ItemTemplate>
        </asp:DataList>
    </dl>
</asp:Content>
