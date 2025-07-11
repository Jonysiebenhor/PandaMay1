<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MenuEmpl.aspx.cs" Inherits="PandaMay.Empleados.MenuEmpl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
            <br />
          <div class="container text-center" style="   width:75%; height: 30%;  border-color:black;background-repeat :no-repeat;  position:initial; z-index: auto;" >
<p class="centrado">

  <asp:Button ID="Button1" CssClass="greenbutton" runat="server" OnClick="Button1_Click"  Text="Ventas" Visible="false"/>
    <asp:Button ID="Button6" CssClass="greenbutton" runat="server" OnClick="Button6_Click"  Text="Compras" Visible="false"/>
    <asp:Button ID="Button7" CssClass="greenbutton" runat="server" OnClick="Button7_Click"  Text="Productos" Visible="false"/>
  <asp:Button ID="Button2" CssClass="greenbutton" runat="server" OnClick="Button2_Click"  Text="Producción" Visible="false"/>
  <asp:Button ID="Button3" CssClass="greenbutton" runat="server" OnClick="Button3_Click"  Text="Confirmaciones" Visible="false"/>
    <asp:Button ID="Button4" CssClass="greenbutton" runat="server" OnClick="Button4_Click"  Text="Seguimientos" Visible="false"/>
 <asp:Button ID="Button5" CssClass="greenbutton" runat="server" OnClick="Button5_Click"  Text="Cuadres" Visible="false"/>
</p>
          </div>
 </asp:Content>
