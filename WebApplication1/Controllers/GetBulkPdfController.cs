using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.InvoiceSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class GetBulkPdfController : ApiController
    {
        
        [HttpPost]
        public HttpResponseMessage MergePdfFiles([FromBody] PdfFileLocationList pdfFileLocationList)
        {
            try
            {
                //List<string> fileLocations = pdfFileLocationList
                
                List<String> fileLocations = pdfFileLocationList?.FileLocations;
                if (fileLocations == null || fileLocations.Count == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No file locations provided.");
                }

                // Create a MemoryStream to hold the merged PDF data
                using (MemoryStream mergedPdfStream = new MemoryStream())
                {
                    // Initialize PDF writer
                    PdfWriter writer = new PdfWriter(mergedPdfStream);
                    PdfDocument mergedPdf = new PdfDocument(writer);

                    // Iterate through each file location and merge the PDFs into the output PDF
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
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"File not found at location: {fileLocation}");
                        }
                    }

                    mergedPdf.Close();
                    writer.Close();

                    // Set appropriate headers for the response
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(mergedPdfStream.ToArray())
                    };

                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "MergedPDF.pdf"
                    };

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    return response;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request.", ex);
            }
        }
        
    }
}