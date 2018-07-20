<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CiklumProject._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Button class="btn btn-primary btn-md" Text="Parse products" OnClick="GetProducts"  runat="server"/>
    <div>
        <table id="mainTable" class="table-striped">
               <%Response.Write(myConnection());%>
        </table>
    </div>
</asp:Content>