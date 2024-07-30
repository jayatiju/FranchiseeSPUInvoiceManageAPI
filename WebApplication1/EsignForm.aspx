<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsignForm.aspx.cs" Inherits="WebApplication1.EsignForm" Async="true" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Esign Form</title>
   <style>
        /* CSS to hide the parameters */
        .hidden-parameter {
            display: none;
        }
        /* CSS to center the button */
        .center-button {
            display: block;
            margin: 0 auto;
            text-align: center;
        }
    </style>
    <script>
        // Function to submit the form when the page loads
      //  function submitFormOnLoad() {
            // Get a reference to the form
       //     var form = document.getElementById('frmdata');

            // Submit the form
       //     form.submit();
       // }

        // Attach the submitFormOnLoad function to the window.onload event
       // window.onload = submitFormOnLoad;
    </script>
</head>
<body>
    <form id="frmdata" action="https://gateway.emsigner.com/eMsecure/V3_0/Index" method="post">
        <!--<iframe src="<%= dataUri %>"" width="100%" height="600"></iframe>-->
        <input id="Parameter1" name="Parameter1" value="" runat="server" class="hidden-parameter"/>
        <input id="Parameter2" name="Parameter2" value="" runat="server" class="hidden-parameter"/>
        <input id="Parameter3" name="Parameter3" value="" runat="server" class="hidden-parameter"/>
        <button style="height: 50px; background-color: #FFCC33; display: block; margin: 0 auto;" type="submit" name="formAction2" id="btnEsignKYC" value="EsignWithASP">E-Sign</button>
    </form>
    
    
</body>
</html>
