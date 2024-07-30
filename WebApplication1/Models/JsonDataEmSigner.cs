using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Serializable]
    public class JsonDataEmSigner
    {
        public string ReferenceNumber { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public object Data { get; set; }
        public int SignatureType { get; set; }
        public string SelectPage { get; set; }
        public string SignaturePosition { get; set; }
        public string AuthToken { get; set; }
        public object File { get; set; }
        public object Filepath { get; set; }
        public string PageNumber { get; set; }
        public int Noofpages { get; set; }
        public bool PreviewRequired { get; set; }
        public string PagelevelCoordinates { get; set; }
        public string CustomizeCoordinates { get; set; }
        public string AadhaarNumber { get; set; }
        public string SUrl { get; set; }
        public string FUrl { get; set; }
        public string CUrl { get; set; }
        public bool Enableuploadsignature { get; set; }
        public bool Enablefontsignature { get; set; }
        public bool EnableDrawSignature { get; set; }
        public bool EnableeSignaturePad { get; set; }
        public bool IsCompressed { get; set; }
        public bool IsCosign { get; set; }
        public int eSignAuthmode { get; set; }
        public bool EnableViewDocumentLink { get; set; }
        public bool Storetodb { get; set; }
        public bool IsGSTN { get; set; }
        public bool IsGSTN3B { get; set; }
        public string SignatureImage { get; set; }
        public bool IsCustomized { get; set; }
        public string SignerID { get; set; }
        public object Documentdetails { get; set; }
        public object FileData { get; set; }
        public object Doctype { get; set; }
        public object PostbackUrl { get; set; }
        public object Objky { get; set; }
        public object TransactionNumber { get; set; }
        public object FinancialYear { get; set; }
        public object EmployeeId { get; set; }
        public string SignatureMode { get; set; }
        public int AuthenticationMode { get; set; }
        public bool EnableInitials { get; set; }
        public bool IsInitialsCustomized { get; set; }
        public object SelectInitialsPage { get; set; }
        public object InitialsPosition { get; set; }
        public string InitialsCustomizeCoordinates { get; set; }
        public string InitialsPagelevelCordinates { get; set; }
        public string InitialsPageNumbers { get; set; }
        public bool ValidateAllPlaceholders { get; set; }
        public object Searchtext { get; set; }
        public string Anchor { get; set; }
        public object InitialSearchtext { get; set; }
        public object InitialsAnchor { get; set; }
        public bool HDFCflag { get; set; }
        public object Reason { get; set; }
        public string eSignGatewayCustomUI { get; set; }
        public int eMudhraV2SignatureType { get; set; }
        public object eMudhraV2SignatureImage { get; set; }
        public object eMudhraV2SignatureImageExtension { get; set; }
    }
}