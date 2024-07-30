using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Models;
using System.Web.SessionState;
using MySql.Data.MySqlClient;

namespace WebApplication1
{
    public partial class EsignFormResponse : System.Web.UI.Page
    {
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public EsignFormResponse()
        {
            try
            {
                _connection = new MySqlConnection(ConnectionString.connString);
                _connection.Open();
            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = ex.Message;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //string sessionKey = HttpContext.Current.Session["SessionKey"] as string;
            //string pdfPath = HttpContext.Current.Session["pdfPath"] as string;
            emSignerResponse response = new emSignerResponse();
            try
            {
                HttpContext.Current.Session["pdfBase64"] = "";
                HttpContext.Current.Session["refNumber"] = "";
                string returnValue = HttpContext.Current.Request.Form["Returnvalue"];
                string referenceNumber = HttpContext.Current.Request.Form["Referencenumber"];
                string txnNumber = HttpContext.Current.Request.Form["Transactionnumber"];
                string errorMessage = HttpContext.Current.Request.Form["ErrorMessage"];
                string status = HttpContext.Current.Request.Form["ReturnStatus"];
                string fileType = HttpContext.Current.Request.Form["FileType"];

                string documentName = referenceNumber.Substring(0, 29);

                referenceNumberField.Value  = referenceNumber.Substring(10, 6);
                // string sessionKey = HttpContext.Current.Session["SessionKey"] as string;
                string sessionKey = Application[referenceNumber] as string;
                 //string sessionKey = HttpContext.Current.Request.Form["SessionKey"] as string;
                string pdfPath = Application[referenceNumber + "pdflocation"] as string;
                if (status == "Success" && errorMessage == "")
                {
                    #region Decrypt the Hash Response        
                    IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7");
                    cipher.Init(false, new KeyParameter(Convert.FromBase64String(sessionKey)));
                    byte[] decResponse = cipher.DoFinal(Convert.FromBase64String(returnValue));
                    string base64Response = Convert.ToBase64String(decResponse);
                    #endregion Decrypt the Hash Responses   
                    byte[] pdfBytes = Convert.FromBase64String(base64Response);
                    string fileName = documentName + ".pdf";

                    // Provide a physical path to save the PDF file on the server
                    //string serverPath = Path.Combine(Server.MapPath("~/App_Data/PDFs"), fileName);
                    //string serverPath = @"C:\\invoices\\" + fileName;
                    string serverPath = pdfPath;

                    // Save the PDF file to the server
                    System.IO.File.WriteAllBytes(serverPath, pdfBytes);
                    HttpContext.Current.Session["pdfBase64"] = base64Response;
                    HttpContext.Current.Session["refNumber"] = referenceNumber;
                    response.Status = Status.Success;

                    string statusSql = "UPDATE vendor_ds_table SET DsStatus=@DsStatus, TransactionNum=@TransactionNum, ErrorMessage=@ErrorMessage, ReferenceNum = @ReferenceNum WHERE FileName = @FileName ;";
                   // string monthYear = convertToMonthYear(invoiceGenerationInput.startDate);
                    using (var statuscommand = new MySqlCommand(statusSql, _connection))
                    {
                        statuscommand.Parameters.AddWithValue("@DsStatus", status);
                        statuscommand.Parameters.AddWithValue("@TransactionNum", txnNumber);
                        statuscommand.Parameters.AddWithValue("@ErrorMessage", errorMessage);
                        statuscommand.Parameters.AddWithValue("@FileName", fileName);
                        statuscommand.Parameters.AddWithValue("@ReferenceNum", referenceNumber);


                        statuscommand.ExecuteNonQuery();


                    }
                }
                else
                {
                   // byte[] pdfBytes = new byte[0];
                   // string fileName = referenceNumber + ".pdf";

                    // Provide a physical path to save the PDF file on the server
                    //string serverPath = Path.Combine(Server.MapPath("~/App_Data/PDFs"), fileName);
                    //string serverPath = @"C:\\invoices\\" + fileName;
                    //string serverPath = pdfPath;
                    // Save the PDF file to the server
                   // System.IO.File.WriteAllBytes(serverPath, pdfBytes);
                    response.Status = Status.Failed;
                    response.ErrorMessage = errorMessage;

                    string statusSql = "UPDATE vendor_ds_table SET DsStatus=@DsStatus, TransactionNum=@TransactionNum, ErrorMessage=@ErrorMessage, ReferenceNum = @ReferenceNum WHERE FlePath = @FilePath ;";
                    // string monthYear = convertToMonthYear(invoiceGenerationInput.startDate);
                    using (var statuscommand = new MySqlCommand(statusSql, _connection))
                    {
                        statuscommand.Parameters.AddWithValue("@DsStatus", "Failure");
                        statuscommand.Parameters.AddWithValue("@TransactionNum", txnNumber);
                        statuscommand.Parameters.AddWithValue("@ErrorMessage", errorMessage);
                        statuscommand.Parameters.AddWithValue("@FilePath", pdfPath);
                        statuscommand.Parameters.AddWithValue("@ReferenceNum", referenceNumber);


                        statuscommand.ExecuteNonQuery();


                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = Status.Failed;
                response.ErrorMessage = ex.Message;
            }

        }


    }
}