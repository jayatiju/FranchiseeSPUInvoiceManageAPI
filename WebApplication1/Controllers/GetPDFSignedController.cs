using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using WebApplication1.Models;
using System.Web.Http.Cors;
using System.IO;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class GetPDFSignedController : ApiController
    {
        [HttpGet] // Change the HTTP verb to GET
        public IHttpActionResult GetPDFSignedForm([FromBody] PdfFileLocationList pdfFileLocationList, string referencenum, string signaturename)
        {
            try
            {
                List<string> fileLocations = pdfFileLocationList?.FileLocations;
                if (fileLocations == null || fileLocations.Count == 0)
                {
                    return BadRequest("No file locations provided.");
                }

                using (MemoryStream mergedPdfStream = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(mergedPdfStream);
                    PdfDocument mergedPdf = new PdfDocument(writer);

                    foreach (string fileLocation in fileLocations)
                    {
                        if (File.Exists(fileLocation))
                        {
                            PdfReader pdfReader = new PdfReader(fileLocation);
                            pdfReader.SetUnethicalReading(true);

                            PdfDocument pdf = new PdfDocument(pdfReader);

                            pdf.CopyPagesTo(1, pdf.GetNumberOfPages(), mergedPdf);
                            pdf.Close();
                        }
                        else
                        {
                            return BadRequest($"File not found at location: {fileLocation}");
                        }
                    }

                    mergedPdf.Close();
                    writer.Close();

                    byte[] pdfBytes = mergedPdfStream.ToArray();
                    string base64Pdf = Convert.ToBase64String(pdfBytes);

                    string baseUrl = "https://localhost:44361";
                    string esignFormUrl = $"{baseUrl}/EsignForm.aspx?pdfbytes={base64Pdf}&referenceNum={referencenum}&signatureName={signaturename}";

                    using (HttpClient httpClient = new HttpClient())
                    {
                        HttpResponseMessage response = httpClient.GetAsync(esignFormUrl).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            // You can process the response content if needed.
                            return Ok("PDF signed successfully.");
                        }
                        else
                        {
                            return BadRequest("Call to ASPX page failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
