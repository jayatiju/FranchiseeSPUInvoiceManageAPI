<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsignForm.aspx.cs" Inherits="WebApplication1.EsignForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Esign Form</title>
</head>
<body>
    <form id="frmdata" action="https://gateway.emsigner.com/eMsecure/V3_0/Index" method="post">
        <input id="Parameter1" name="Parameter1" value="" />
        <input id="Parameter2" name="Parameter2" value="" />
        <input id="Parameter3" name="Parameter3" value="" />
        <button style="height:50px; background-color:#FFCC33" type="submit" name="formAction2" id="btnEsignKYC" value="EsignWithASP">E-Sign</button>
    </form>
</body>
</html>
