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
using System.Text;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class getDigitalSignatureStatusController : ApiController
    {
        ResponseCode responseCode = new ResponseCode();

        private readonly MySqlConnection _connection;
        public getDigitalSignatureStatusController()
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


        [HttpPost]
        public IHttpActionResult GetDigitalSignatureStatus([FromBody] vendorBulkPDFInput vendorBulkPDFInput)
        {
            try
            {
                string sql = "select distinct MonthYear, RegionCode, VendorCode, FlePath,FileName,DsStatus,TransactionNum,ErrorMessage, ReferenceNum, InvoicePdfDSStatus FROM franchiseeinvoicedb.vendor_ds_table where MonthYear = @MonthYear AND VendorCode = @VendorCode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                string MonthYear = convertToMonthYearVendor(vendorBulkPDFInput.startDate);

                command.Parameters.AddWithValue("@MonthYear", $"{MonthYear}");                
                //command.Parameters.AddWithValue("@regionCode", $"{vendorBulkPDFInput.region}");
                command.Parameters.AddWithValue("@vendorCode", $"{vendorBulkPDFInput.vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();


                List<VendorDS> ListVendorDS = new List<VendorDS>();                         

                while (reader.Read())
                {
                    VendorDS vendorDSItem = new VendorDS();

                    vendorDSItem.monthYear = reader.GetString("MonthYear"); ;
                    vendorDSItem.region = reader.GetString("RegionCode"); ;
                    vendorDSItem.vendorcode = reader.GetString("VendorCode"); ;
                    vendorDSItem.filePath = reader.GetString("FlePath"); ;
                    vendorDSItem.fileName = reader.GetString("FileName"); ;
                    vendorDSItem.DigitalSignatureStatus = reader.GetString("DsStatus"); ;
                    vendorDSItem.TransactionNumber = reader.GetString("TransactionNum"); ;
                    vendorDSItem.ErrorMessage = reader.GetString("ErrorMessage"); ;
                    vendorDSItem.ReferenceNum = reader.GetString("ReferenceNum"); ;
                    vendorDSItem.InvoicePdfDSStatus = reader.GetString("InvoicePdfDSStatus"); ;

                    ListVendorDS.Add(vendorDSItem);

                   
                }

                reader.Close();

      

                return (Ok(ListVendorDS));
            }
            catch (Exception ex)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = ex.Message;
                return (Content(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseCode)));

            }
            finally
            {
                _connection.Close();
            }

        }

       

        private void SplitAndSavePdf(PdfDocument mergedPdfDocument, string folderPath)
        {
            int maxFileSize = 10 * 1024 * 1024; // 10MB
            int fileIndex = 1;

            for (int pageNum = 1; pageNum <= mergedPdfDocument.GetNumberOfPages(); pageNum++)
            {
                using (MemoryStream singlePagePdfStream = new MemoryStream())
                {
                    using (PdfWriter writer = new PdfWriter(singlePagePdfStream))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            // Get a specific page from the mergedPdfDocument
                            PdfPage page = mergedPdfDocument.GetPage(pageNum).CopyTo(pdf);

                            pdf.AddPage(page);
                        }
                    }

                    // Check if adding the page to the current file exceeds the max file size
                    if (singlePagePdfStream.Length > maxFileSize)
                    {
                        // Save the current file and create a new one
                        SavePdfToFile(singlePagePdfStream, fileIndex++, folderPath);
                        singlePagePdfStream.Seek(0, SeekOrigin.Begin);
                    }

                    // Append the page to the current file
                    SavePdfToFile(singlePagePdfStream, fileIndex, folderPath);
                }
            }
        }



        private void SavePdfToFile(MemoryStream pdfStream, int fileIndex, string folderPath)
        {
            string fileName = $"MergedPDF_Part_{fileIndex}.pdf";
            string filePath = Path.Combine(folderPath, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                pdfStream.Seek(0, SeekOrigin.Begin); // Reset the stream position before writing to the file
                pdfStream.CopyTo(fileStream);
            }
        }

        public static string convertToMonthYearVendor(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                // Format the DateTime object to yyyyMM (month-year) string
                string result = date.ToString("yyyyMM");

                return result;
            }
            catch (FormatException)
            {
                // Handle invalid input date format
                return "Invalid Date Format";
            }
        }


    }
}