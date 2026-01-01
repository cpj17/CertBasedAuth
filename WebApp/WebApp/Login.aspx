<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApp.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Simple Login</title>
    <style>
        body {
            font-family: Arial;
            background-color: #f2f2f2;
        }
        .login-box {
            width: 300px;
            margin: 100px auto;
            padding: 20px;
            background: white;
            border-radius: 5px;
            box-shadow: 0px 0px 10px #aaa;
        }
        .login-box input {
            width: 100%;
            margin-bottom: 10px;
            padding: 8px;
        }
        .login-box button {
            width: 100%;
            padding: 8px;
        }
        .result {
            margin-top: 10px;
            color: green;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-box">
            <h3>Login</h3>

            <asp:TextBox ID="txtUsername" runat="server" 
                placeholder="Username"></asp:TextBox>

            <asp:TextBox ID="txtPassword" runat="server" 
                TextMode="Password" 
                placeholder="Password"></asp:TextBox>

            <asp:Button ID="btnLogin" runat="server" 
                Text="Login" 
                OnClick="btnLogin_Click" />

            <asp:Label ID="lblResult" runat="server" 
                CssClass="result"></asp:Label>
        </div>
    </form>
</body>
</html>
