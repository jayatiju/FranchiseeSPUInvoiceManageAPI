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


namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class GetPdfController : ApiController
    {
        
        [HttpPost]

        public HttpResponseMessage GetPdfFile([FromBody] PdfFileLocation pdfFileLocation)
        {
            try
            {
                // Read the request content as a string, which should contain the file location
                //string fileLocation = await Request.Content.ReadAsStringAsync();
                String fileLocation = pdfFileLocation.InvoicePdfLocation;

                

                if (string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid file location.");
                }

                // Read the PDF file into a byte array
                byte[] pdfBytes = File.ReadAllBytes(fileLocation);

                // Create a HttpResponseMessage with the PDF data
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(fileLocation)
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request.", ex);
            }
        }
        
    }


}