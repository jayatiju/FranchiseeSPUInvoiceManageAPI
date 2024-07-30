using System;
using System.IO;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class emSignerController : ApiController
    {
        [HttpGet]
        [Route("api/emSigner/index")]
        public IHttpActionResult Index()
        {
            return Ok("Welcome to emSigner API");
        }

        [HttpGet]
        [Route("api/emSigner/emSignerRequest")]
        public IHttpActionResult emSignerRequest(string pdflocation, string referencenum, string signaturename)
        {
            #region Resources

            emSignerRequest emSignerRequest = new emSignerRequest();
             String fileLocation = pdflocation;
             if (string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
             {
           // create error response
             }

            byte[] pdfBytes = File.ReadAllBytes(fileLocation);
            //byte[] pdfBytes = Convert.FromBase64String(pdfbase64);
            string base64Pdf = Convert.ToBase64String(pdfBytes);

            JsonDataEmSigner jsonDataEmSigner = new JsonDataEmSigner();

            jsonDataEmSigner.ReferenceNumber = referencenum;
            jsonDataEmSigner.Name = signaturename;
            jsonDataEmSigner.FileType = "PDF";
            //jsonDataEmSigner.Data = null;
            jsonDataEmSigner.SignatureType = 3;
            jsonDataEmSigner.SelectPage = "ALL";
            jsonDataEmSigner.SignaturePosition = "Bottom-Right";
            jsonDataEmSigner.AuthToken = "cfed1b82-2b5a-4cb8-8ac2-7b212bc19b89";
                                            
            jsonDataEmSigner.File = base64Pdf;
            //jsonDataEmSigner.Filepath = null;
            jsonDataEmSigner.PageNumber = "";
            //jsonDataEmSigner.Noofpages = 0;
            jsonDataEmSigner.PreviewRequired = true;
            jsonDataEmSigner.PagelevelCoordinates = "";
            jsonDataEmSigner.CustomizeCoordinates = "";
            //jsonDataEmSigner.AadhaarNumber = "";
            jsonDataEmSigner.SUrl = "http://192.168.52.172/api/emSigner/emSignerResponse";
            jsonDataEmSigner.FUrl = "http://192.168.52.172/api/emSigner/emSignerResponse";
            jsonDataEmSigner.CUrl = "http://192.168.52.172/api/emSigner/emSignerResponse";
            jsonDataEmSigner.Enableuploadsignature = false;
            jsonDataEmSigner.Enablefontsignature = false;
            jsonDataEmSigner.EnableDrawSignature = false;
            jsonDataEmSigner.EnableeSignaturePad = false;
            jsonDataEmSigner.Storetodb = false;
            jsonDataEmSigner.EnableViewDocumentLink = false;
            //jsonDataEmSigner.IsCompressed = false;
            jsonDataEmSigner.IsCosign = false;
            jsonDataEmSigner.SignerID = "";
            /*
            jsonDataEmSigner.IsCompressed = false;
            jsonDataEmSigner.IsCosign = false;
            jsonDataEmSigner.eSignAuthmode = 0;
            jsonDataEmSigner.EnableViewDocumentLink = false;
            jsonDataEmSigner.Storetodb = false;
            jsonDataEmSigner.IsGSTN = false;
            jsonDataEmSigner.IsGSTN3B = false;
            jsonDataEmSigner.SignatureImage = "";
            jsonDataEmSigner.IsCustomized = false;
            jsonDataEmSigner.eSign_SignerId = "";
            jsonDataEmSigner.Documentdetails = null;
            jsonDataEmSigner.FileData = null;
            jsonDataEmSigner.Doctype = null;
            jsonDataEmSigner.PostbackUrl = null;
            jsonDataEmSigner.Objky = null;
            jsonDataEmSigner.TransactionNumber = null;
            jsonDataEmSigner.FinancialYear = null;
            jsonDataEmSigner.EmployeeId = null;
            jsonDataEmSigner.SignatureMode = "2,12";
            jsonDataEmSigner.AuthenticationMode = 1;
            jsonDataEmSigner.EnableInitials = false;
            jsonDataEmSigner.IsInitialsCustomized = false;
            jsonDataEmSigner.SelectInitialsPage = null;
            jsonDataEmSigner.InitialsPosition = null;
            jsonDataEmSigner.InitialsCustomizeCoordinates = "";
            jsonDataEmSigner.InitialsPagelevelCordinates = "";
            jsonDataEmSigner.InitialsPageNumbers = "";
            jsonDataEmSigner.ValidateAllPlaceholders = true;
            jsonDataEmSigner.Searchtext = null;
            jsonDataEmSigner.Anchor = "Middle";
            jsonDataEmSigner.InitialSearchtext = null;
            jsonDataEmSigner.InitialsAnchor = null;
            jsonDataEmSigner.HDFCflag = false;
            jsonDataEmSigner.Reason = null;
            jsonDataEmSigner.eSignGatewayCustomUI = "";
            jsonDataEmSigner.eMudhraV2SignatureType = 1;
            jsonDataEmSigner.eMudhraV2SignatureImage = null;
            jsonDataEmSigner.eMudhraV2SignatureImageExtension = null;*/

            //string jsonRequest = File.ReadAllText(@"C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\Request\\PDF_EmSignerRequest.json");
            string certificate = @"C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\Resources\\certificate.cer";
            //string certificate = @"C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\Resources\\Certificate_C3.cer";
            //string certificate = @"C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\Resources\\Encryption.CER";
           // string certificate = @"C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\Resources\\Signature.CER";

            var json = JsonConvert.SerializeObject(jsonDataEmSigner);
            string jsonRequest = json;

            byte[] sessionKey = CertificateEncryption.GetNewSessionKey();
            HttpContext.Current.Session["SessionKey"] = Convert.ToBase64String(sessionKey);
            string base64PublicKeyEncryption = CertificateEncryption.EncryptWithPublicKey(sessionKey, certificate);
            emSignerRequest.Parameter1 = base64PublicKeyEncryption;
            string base64InputData = Convert.ToBase64String(CertificateEncryption.EncryptDataAES(CertificateEncryption.ConvertStringToByteArray(jsonRequest), sessionKey));
            emSignerRequest.Parameter2 = base64InputData;
            string base64SignedHash = Convert.ToBase64String(CertificateEncryption.Generatehash256(jsonRequest));
            string signedHash = Convert.ToBase64String(CertificateEncryption.EncryptDataAES(Convert.FromBase64String(base64SignedHash), sessionKey));
            emSignerRequest.Parameter3 = signedHash;
            #endregion
            return Ok(emSignerRequest);
        }

        [HttpPost]
        [Route("api/emSigner/emSignerResponse")]
        public IHttpActionResult emSignerResponse()
        {
            emSignerResponse response = new emSignerResponse();
            try
            {
               // HttpContext.Current.Session["pdfBase64"] = "";
                //HttpContext.Current.Session["refNumber"] = "";
                string returnValue = HttpContext.Current.Request.Form["Returnvalue"];
                string referenceNumber = HttpContext.Current.Request.Form["Referencenumber"];
                string txnNumber = HttpContext.Current.Request.Form["Transactionnumber"];
                string errorMessage = HttpContext.Current.Request.Form["ErrorMessage"];
                string status = HttpContext.Current.Request.Form["ReturnStatus"];
                string fileType = HttpContext.Current.Request.Form["FileType"];
                string sessionKey = HttpContext.Current.Session["SessionKey"] as string;
                if (status == "Success")
                {
                    #region Decrypt the Hash Response        
                    IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7");
                    cipher.Init(false, new KeyParameter(Convert.FromBase64String(sessionKey)));
                    byte[] decResponse = cipher.DoFinal(Convert.FromBase64String(returnValue));
                    string base64Response = Convert.ToBase64String(decResponse);
                    #endregion Decrypt the Hash Responses   
                    HttpContext.Current.Session["pdfBase64"] = base64Response;
                    HttpContext.Current.Session["refNumber"] = referenceNumber;
                    response.Status = Status.Success;
                }
                else
                {
                    response.Status = Status.Failed;
                    response.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                response.Status = Status.Failed;
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }
        /*
        [HttpGet]
        [Route("api/emSigner/DownloadPDF")]
        public IHttpActionResult DownloadPDF()
        {
            string pdfBase64 = HttpContext.Current.Session["pdfBase64"] as string;
            string refNumber = HttpContext.Current.Session["refNumber"] as string;
            if (!string.IsNullOrEmpty(pdfBase64))
            {
                byte[] pdfBytes = Convert.FromBase64String(pdfBase64);
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + refNumber + ".pdf");
                return new FileResult(pdfBytes, "application/pdf");
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=empty_" + refNumber + ".pdf");
                return new FileResult(new byte[0], "application/pdf");
            }
        }*/
    }
}
