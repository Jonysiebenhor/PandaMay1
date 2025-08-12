<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Ventas.aspx.cs" Inherits="PandaMay.Empleados.Ventas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <br />
              <div class="container text-center" style="   width:75%; height: 30%;  border-color:black;background-repeat :no-repeat;  position:initial; z-index: auto;" >
<p class="centrado">
  <asp:Button ID="Button1" CssClass="greenbutton" runat="server" OnClick="Button1_Click"  Text="Nueva Venta"/>
  <asp:Button ID="Button2" CssClass="greenbutton" runat="server" OnClick="Button2_Click"  Text="Cierre diario"/>
       <asp:HyperLink ID="lnkRegresar" runat="server"
CssClass="orangebutton"
Text="Regresar"
NavigateUrl="~/empleados/menuempl.aspx" />
</p>
                  </div>
</asp:Content>
