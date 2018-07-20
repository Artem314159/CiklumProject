<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="CiklumProject.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <% GetProduct(); %>
    <h1 id="notFound" style="display:none" runat="server">ProductId not found.</h1>
    <div id="mainContent" runat="server">
        <div class="Image inline-block">
            <img src=<%: CiklumProject.Product.GetElement(5) %> >   <%-- Image --%>
        </div>
        <div class="inline-block">
            <div>
                <h1 class="Name"><%: CiklumProject.Product.GetElement(1) %></h1> <%-- Name --%>
                <div class="Price"><%: CiklumProject.Product.GetElement(2) %>     <%-- Price --%>
                <%: CiklumProject.Product.GetElement(3) %>.</div>   <%-- Currency --%>
            </div>
            <div class="Description">
                <h3>Опис</h3>
                <div>
                    <% Response.Write(CiklumProject.Product.GetElement(4)); %>  <%-- Description --%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>