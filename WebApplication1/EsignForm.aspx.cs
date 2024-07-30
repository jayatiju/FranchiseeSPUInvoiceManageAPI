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


namespace WebApplication1
{
    public partial class EsignForm : System.Web.UI.Page
    {
        protected string dataUri;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["pdflocation"] != null && Request.QueryString["referenceNum"] != null && Request.QueryString["signatureName"] != null)
                {
                    string pdfLocation = Request.QueryString["pdflocation"];
                    string referenceNum = Request.QueryString["referenceNum"];
                    string signatureName = Request.QueryString["signatureName"];

                    #region Resources

                    emSignerRequest emSignerRequest = new emSignerRequest();
                    String fileLocation = pdfLocation;
                    if (string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                    {
                        // create error response
                    }

                    byte[] pdfBytes = File.ReadAllBytes(fileLocation);
                    string base64Pdf = Convert.ToBase64String(pdfBytes);

                    JsonDataEmSigner jsonDataEmSigner = new JsonDataEmSigner();

                    jsonDataEmSigner.ReferenceNumber = referenceNum;
                    jsonDataEmSigner.Name = signatureName;
                    jsonDataEmSigner.FileType = "PDF";

                    jsonDataEmSigner.SignatureType = 3;
                    jsonDataEmSigner.SelectPage = "ALL";
                    jsonDataEmSigner.SignaturePosition = "Bottom-Right";
                    //jsonDataEmSigner.AuthToken = "8a44a287-4246-4b93-92d1-fe1eaf3e6fce";

                    jsonDataEmSigner.AuthToken = "e78ec31c-49d1-4536-8e14-f288e667f4ce";

                    jsonDataEmSigner.File = base64Pdf;

                    jsonDataEmSigner.PageNumber = "";

                    jsonDataEmSigner.PreviewRequired = true;
                    jsonDataEmSigner.PagelevelCoordinates = "";
                    jsonDataEmSigner.CustomizeCoordinates = "";

                    jsonDataEmSigner.SUrl = "https://frspuinv.ifbsupport.com/EsignFormResponse.aspx/";
                    jsonDataEmSigner.FUrl = "https://frspuinv.ifbsupport.com/EsignFormResponse.aspx/";
                    jsonDataEmSigner.CUrl = "https://frspuinv.ifbsupport.com/EsignFormResponse.aspx/";
                    jsonDataEmSigner.Enableuploadsignature = false;
                    jsonDataEmSigner.Enablefontsignature = false;
                    jsonDataEmSigner.EnableDrawSignature = false;
                    jsonDataEmSigner.EnableeSignaturePad = false;
                    jsonDataEmSigner.Storetodb = false;
                    jsonDataEmSigner.EnableViewDocumentLink = false;

                    jsonDataEmSigner.IsCosign = false;
                    jsonDataEmSigner.SignerID = "";

                    //string certificate = @"C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\Resources\\certificate.cer";
                    string certificate = @"C:\\franchiseinvoicemanagementdeploy\\franchiseinvoicemanagement\\certificate.cer";

                    var json = JsonConvert.SerializeObject(jsonDataEmSigner);
                    string jsonRequest = json;

                    byte[] sessionKey = CertificateEncryption.GetNewSessionKey();
                    // HttpContext.Current.Request.Form["SessionKey"] = Convert.ToBase64String(sessionKey);
                    Application[referenceNum] = Convert.ToBase64String(sessionKey);
                    Application[referenceNum+"pdflocation"] = pdfLocation;
                    //HttpContext.Current.Session["SessionKey"] = Convert.ToBase64String(sessionKey);
                    //HttpContext.Current.Session["pdfPath"] = pdfLocation;
                    string base64PublicKeyEncryption = CertificateEncryption.EncryptWithPublicKey(sessionKey, certificate);
                    emSignerRequest.Parameter1 = base64PublicKeyEncryption;
                    string base64InputData = Convert.ToBase64String(CertificateEncryption.EncryptDataAES(CertificateEncryption.ConvertStringToByteArray(jsonRequest), sessionKey));
                    emSignerRequest.Parameter2 = base64InputData;
                    string base64SignedHash = Convert.ToBase64String(CertificateEncryption.Generatehash256(jsonRequest));
                    string signedHash = Convert.ToBase64String(CertificateEncryption.EncryptDataAES(Convert.FromBase64String(base64SignedHash), sessionKey));
                    emSignerRequest.Parameter3 = signedHash;
                    #endregion

                    Parameter1.Value = emSignerRequest.Parameter1;
                    Parameter2.Value = emSignerRequest.Parameter2;
                    Parameter3.Value = emSignerRequest.Parameter3;

                    // Generate the data URI for the PDF.
                    //byte[] pdfBytes = File.ReadAllBytes(pdfLocation);
                    string base64PdfFinal = Convert.ToBase64String(pdfBytes);
                    dataUri = "data:application/pdf;base64," + base64PdfFinal;

                    // Call the API to populate the form fields when the page loads.
                    // await PopulateFormFieldsFromApiAsync(pdfLocation, referenceNum, signatureName);
                }
            }
        }
    }
}