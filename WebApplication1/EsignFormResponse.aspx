<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsignFormResponse.aspx.cs" Inherits="WebApplication1.EsignFormResponse" Async="true" %>

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
 <form id="frmdata" action="Web Page" method="post">
     <input type="hidden" id="referenceNumberField" runat="server" />
        <button style="height: 50px; background-color: #FFCC33; display: block; margin: 0 auto;" type="submit" name="formAction2" id="btnContinue" value="valueContinue">Continue</button>
    </form>

    <script>
        document.getElementById("frmdata").addEventListener("submit", function (event) {
            // Prevent the default form submission
            event.preventDefault();

            var referenceNumber = document.getElementById("referenceNumberField").value;

            // Define the destination URL
            var destinationURL = "https://frspuinv.ifbsupport.com:8080/#/digitalsignature/" + referenceNumber; // Replace with the actual URL

            // Redirect to the destination URL
            window.location.href = destinationURL;
        });
    </script>
    
</body>
</html>
