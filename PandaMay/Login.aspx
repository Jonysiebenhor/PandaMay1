<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage2.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PandaMay.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Login
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <form runat="server">
         
        <div class="container text-center" style="   width:230px; height: 12%;  background-color:#ffd2cc;background-repeat :no-repeat;  position:page; z-index: auto;" >
     <p class="centrado">
        <asp:Label ID="Label1" runat="server" Text="Ingresar" Font-Bold="True" Font-Size="XX-Large" ForeColor=Black></asp:Label>
   </div>
         <br />
 <div class="container text-center" style="   width:260px; height: 50%;   background-color:#ffd2cc;background-repeat :no-repeat;  position:page; z-index: auto;" >
      <p class="centrado">
          <br />
          
     <asp:Label ID="Label2" runat="server" Text="Bienvenido" Font-Bold="True" Font-Size="X-Large" ForeColor=Black></asp:Label>
     <br />
           <br />
          <br />
     
    <asp:TextBox ID="txtusuario" CssClass="textbox" runat="server" placeholder="Usuario"  ></asp:TextBox>
<br />
    <br />
<asp:TextBox ID="txtcontraseña" CssClass="textbox" runat="server" TextMode="Password" placeholder="Contraseña" ></asp:TextBox>
   <br />
    <asp:Label ID="estado"  runat="server"></asp:Label>
  <br />
    <asp:Button ID="Button2" CssClass="greenbutton" runat="server" OnClick="Button2_Click"  Text="Ingresar"  />
           </p>
      <p class="centrado">
          <asp:Button ID="Button3" CssClass="orangebutton" runat="server" OnClick="Button3_Click" Text="Crear Cuenta" BackColor=#fca669/>
   <br />
    <br />
           </p> </div>
        </form>
</asp:Content>
