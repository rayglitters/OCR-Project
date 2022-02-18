<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OCR_Project._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>ASP.NET C# Application</h1>
        <p class="lead">This is a project for R&D with Azure Cognitive Services with OCR.</p>
    </div>
    <div class="row">
        <h2>Rahman OCR Project</h2>
        <p>
            Upload your file to be processed.
        </p>
        <p>
            <asp:FileUpload ID="FileUpLoad1" runat="server" />
            <asp:Button ID="UploadBtn" Text="Upload File" OnClick="UploadBtn_Click" runat="server" Width="105px" />
            <asp:Button ID="Extract" Text="Extract File" OnClick="ExtractBtn_Click" runat="server" Width="105px" />
            <asp:Image ID="Image1" runat="server"  />
            <br />
            <asp:Label ID="lblUploadResult" runat="server"></asp:Label>
        </p>
        <br /> <asp:RegularExpressionValidator   
                id="FileUpLoadValidator" runat="server"   
                ErrorMessage="Upload Jpegs and Gifs only."   
                ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.jpg|.JPG|.gif|.GIF|.JPEG|.jpeg|.PNG|.png|.pdf|.PDF)$"   
                ControlToValidate="FileUpload1">  
</asp:RegularExpressionValidator> <br />
        <asp:Label id="LengthLabel"
           runat="server">
        </asp:Label>  
        
        <br /><br />
       
        <asp:Label id="ContentsLabel"
           runat="server">
        </asp:Label>  
        <br /><br />
       
        <asp:PlaceHolder id="PlaceHolder1"
            runat="server">
        </asp:PlaceHolder>   
        <div>

            <asp:TextBox ID="TextBox1" runat="server" ForeColor="Black" Height="348px" TextMode="MultiLine" Width="315px">Extracted file</asp:TextBox>
            <br />
            
        </div>

    </div>
</asp:Content>
