<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="IngresoPedidos.aspx.cs" Inherits="PandaMay.IngresoPedidos1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
            <!--<br />
            <div class="container text-center" style="   width:75%; height: 30%; border:black; background-repeat :no-repeat;  position:initial; z-index: auto;" >
            <asp:Label ID="Label1" runat="server" Text="Pedidos" Font-Bold="True" Font-Size="XX-Large" ForeColor=Black text-align="center"></asp:Label>
                  <br />
            </div>
            <div class="container text-center" style="   width:75%; height: 30%;  border-color:black;background-repeat :no-repeat;  position:initial; z-index: auto;" >
  <p class="centrado">
       <br />
      <asp:TextBox ID="TextBox13" CssClass="buscador" runat="server" placeholder="Buscar..." ></asp:TextBox>
      <asp:Button ID="Button1" CssClass="orangebutton" runat="server" Text="Buscar" />
      <asp:Button ID="Button2" CssClass="greenbutton" runat="server" Text="Guardar" Enabled="false" Visible="False" />
      <asp:Button ID="Button3" CssClass="redbutton" runat="server" Text="Cancelar" Enabled="false" Visible="False" />
      </p>
                  </div>
      <br />
<div class="container text-center" style="   width:90%;   background-color:#ffd2cc;" >
    <p class="centrado">
         
    <asp:Label ID="Label2" runat="server" Text="Datos del Cliente" Font-Bold="True" Font-Size="X-Large" ForeColor=Black></asp:Label>
    <br />
          <br />
  <asp:TextBox ID="txtnombre" CssClass="textbox" runat="server" placeholder="Nombre" ></asp:TextBox>
        
        <asp:DropDownList ID="ddldepartamento" CssClass="dropdownlist" runat="server" OnSelectedIndexChanged="ddldepartamento_SelectedIndexChanged" AutoPostBack="True"  AppendDataBoundItems="True" DataSourceID="SqlDataSource4" DataTextField="nombre" DataValueField="iddepartamento" >
                <asp:ListItem Selected="True" Value="0">--Departamento</asp:ListItem>
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:EnviosExpressConnectionString2 %>" SelectCommand="select* from departamento;"></asp:SqlDataSource>
    
             <asp:Label ID="Departamento0" runat="server"></asp:Label>
              
        <asp:DropDownList ID="ddlmunicipio" CssClass="dropdownlist" runat="server" OnSelectedIndexChanged="ddlmunicipio_SelectedIndexChanged" AutoPostBack="True"  AppendDataBoundItems="True" DataSourceID="SqlDataSource5" DataTextField="nombre" DataValueField="idmunicipio" >
                    <asp:ListItem Selected="True" Value="0">--Municipio</asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:EnviosExpressConnectionString3 %>" SelectCommand="SELECT [idmunicipio], [nombre] FROM [municipio] WHERE ([iddepartamento] = @iddepartamento)">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddldepartamento" Name="iddepartamento" PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
             <asp:Label ID="Municipio0" runat="server"></asp:Label>
         
        <asp:DropDownList ID="ddlzona" CssClass="dropdownlist" runat="server" OnSelectedIndexChanged="ddlzona_SelectedIndexChanged"  AppendDataBoundItems="True" DataSourceID="SqlDataSource6" DataTextField="nombre" DataValueField="idzona" AutoPostBack="True">
                    <asp:ListItem Selected="True" Value="0">--Zona, Aldea, Lugar</asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:EnviosExpressConnectionString3 %>" SelectCommand="SELECT [idzona], [nombre] FROM [zona] WHERE ([idmunicipio] = @idmunicipio)">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlmunicipio" Name="idmunicipio" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
<asp:Label ID="Zona0" runat="server"></asp:Label>
            
                    <asp:DropDownList ID="ddlzona0" runat="server" AppendDataBoundItems="True" AutoPostBack="True" DataSourceID="SqlDataSource1" DataTextField="monto" DataValueField="idzona" OnSelectedIndexChanged="ddlzona0_SelectedIndexChanged" BackColor="GhostWhite" Font-Overline="False" Font-Size="Smaller" Font-Strikeout="False" Font-Underline="False" ForeColor="GhostWhite" Visible="false">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
            
             <asp:Label ID="tarifaenvio" runat="server" Text="Tarifa de Envio:  Q" Visible="False"></asp:Label>
                        &nbsp;<asp:Label ID="montoenvio" runat="server"></asp:Label>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:EnviosExpressConnectionString %>" SelectCommand="SELECT * FROM [zona] WHERE ([idzona] = @idzona)">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlzona" Name="idzona" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
         
        <asp:TextBox ID="TextBox1" CssClass="textbox" runat="server" placeholder="Dirección" ></asp:TextBox>
        
        <asp:TextBox ID="TextBox2" CssClass="textbox" runat="server" placeholder="Teléfonos" ></asp:TextBox>
         <br /><br />

         </p></div>

            <div class="container text-center" style="   width:90%;   background-color:#ffd2cc;" >
      <p class="centrado">
       <br />
       
  <asp:Label ID="Label3" runat="server" Text="Datos del Pedido" Font-Bold="True" Font-Size="X-Large" ForeColor=Black></asp:Label>
   <br />  <br />
          <asp:TextBox ID="TextBox14" CssClass="textbox" runat="server" placeholder="No. Orden" Enabled="False"></asp:TextBox>

              <asp:TextBox ID="TextBox15" CssClass="textbox" runat="server" placeholder="Digitador" Enabled="False"></asp:TextBox>

          <asp:DropDownList ID="DropDownList4" CssClass="dropdownlist" runat="server"  AppendDataBoundItems="True" AutoPostBack="True" >
    <asp:ListItem Selected="True" Value="0">--Vendedor</asp:ListItem>
        </asp:DropDownList>
     
<asp:DropDownList ID="DropDownList1" CssClass="dropdownlist" runat="server"  AppendDataBoundItems="True" AutoPostBack="True" >
    <asp:ListItem Selected="True" Value="0">--Red Social</asp:ListItem>
        </asp:DropDownList>
       
          <asp:TextBox ID="TextBox4" CssClass="textbox" runat="server" placeholder="Usuario" ></asp:TextBox>
       
             <asp:TextBox ID="TextBox5" CssClass="textbox" runat="server" placeholder="Detalle Pedido" ></asp:TextBox>
         <br /> <br />
             <asp:TextBox ID="TextBox6" CssClass="textbox" runat="server" placeholder="Cambios" ></asp:TextBox>
        
             <asp:TextBox ID="TextBox7" CssClass="textbox" runat="server" placeholder="Observaciones" ></asp:TextBox>
        <br /><br />
        </p></div>

       <div class="container text-center" style="   width:90%;   background-color:#ffd2cc;" >
      
       <br />
       
  <asp:Label ID="Label4" runat="server" Text="Datos del Pago" Font-Bold="True" Font-Size="X-Large" ForeColor=Black></asp:Label>
   <br /> <br />
  <asp:Label ID="Label5" runat="server" Text="Monto: Q"  ForeColor=Black></asp:Label>
                <asp:TextBox ID="TextBox16" CssClass="textbox" runat="server" placeholder="Efectivo" ></asp:TextBox>
<br /> <br />
<asp:DropDownList ID="DropDownList2" CssClass="dropdownlist" runat="server"  AppendDataBoundItems="True" AutoPostBack="True" >
    <asp:ListItem Selected="True" Value="0">--Banco</asp:ListItem>
        </asp:DropDownList>
       
          <asp:TextBox ID="TextBox3" CssClass="textbox" runat="server" placeholder="Monto transferecia 1" ></asp:TextBox>
        
             <asp:TextBox ID="TextBox8" CssClass="textbox" runat="server" placeholder="Referencia" ></asp:TextBox>
        
             <asp:TextBox ID="TextBox9" CssClass="textbox" runat="server" placeholder="Fecha" ></asp:TextBox>

             <asp:TextBox ID="TextBox25" CssClass="textbox" runat="server" placeholder="Pendiente por validar" Enabled="false"></asp:TextBox>

        <br /> <br />
            <asp:DropDownList ID="DropDownList3" CssClass="dropdownlist" runat="server"  AppendDataBoundItems="True" AutoPostBack="True">
    <asp:ListItem Selected="True" Value="0">--Banco</asp:ListItem>
        </asp:DropDownList>
       
          <asp:TextBox ID="TextBox10" CssClass="textbox" runat="server" placeholder="Monto transferencia 2" ></asp:TextBox>
       
             <asp:TextBox ID="TextBox11" CssClass="textbox" runat="server" placeholder="Referencia" ></asp:TextBox>
        
             <asp:TextBox ID="TextBox12" CssClass="textbox" runat="server" placeholder="Fecha" ></asp:TextBox>

               <asp:TextBox ID="TextBox24" CssClass="textbox" runat="server" placeholder="Pendiente por validar" Enabled="false"></asp:TextBox>
   
        <br /><br />
        </div>
             <br />
                   <div class="container text-center" style="   width:90%;   background-color:#ffd2cc;" >
      
       <br />
       
  <asp:Label ID="Label6" runat="server" Text="Datos del Proceso" Font-Bold="True" Font-Size="X-Large" ForeColor=Black></asp:Label>
   <br /> <br />
                       <asp:TextBox ID="TextBox28" CssClass="textbox" runat="server" placeholder="Fecha de creación" Enabled="false" ></asp:TextBox>

                       <asp:TextBox ID="TextBox26" CssClass="textbox" runat="server" placeholder="Confirmación" Enabled="false" ></asp:TextBox>

                       <asp:TextBox ID="TextBox17" CssClass="textbox" runat="server" placeholder="Empacador" Enabled="false" ></asp:TextBox>
       
          <asp:TextBox ID="TextBox18" CssClass="textbox" runat="server" placeholder="Pendiente de elaborar" Enabled="false"></asp:TextBox>
        
             <asp:TextBox ID="TextBox19" CssClass="textbox" runat="server" placeholder="En bodega" Enabled="false"></asp:TextBox>
        <asp:TextBox ID="TextBox29" CssClass="textbox" runat="server" placeholder="Fecha salida" Enabled="false"></asp:TextBox>
        <br /><br />
        </div>
            <br />
         <div class="container text-center" style="   width:90%;   background-color:#ffd2cc;" >
    
     <br />
     
<asp:Label ID="Label7" runat="server" Text="Datos de la Mensajería" Font-Bold="True" Font-Size="X-Large" ForeColor=Black></asp:Label>
 <br /> <br />
                     <asp:TextBox ID="TextBox20" CssClass="textbox" runat="server" placeholder="Empresa" Enabled="false" ></asp:TextBox>

                     <asp:TextBox ID="TextBox21" CssClass="textbox" runat="server" placeholder="No. Guía" Enabled="false" ></asp:TextBox>
     
        <asp:TextBox ID="TextBox22" CssClass="textbox" runat="server" placeholder="Quien Realizo Guía" Enabled="false"></asp:TextBox>
      
           <asp:TextBox ID="TextBox23" CssClass="textbox" runat="server" placeholder="Portal de Guía" Enabled="false"></asp:TextBox>
      
             <asp:TextBox ID="TextBox27" CssClass="textbox" runat="server" placeholder="Estado" Enabled="false"></asp:TextBox>
      <br /><br />
      </div>

                -->
</asp:Content>
