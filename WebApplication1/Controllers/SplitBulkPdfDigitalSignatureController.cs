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
using iText.Layout;
using iText.Layout.Element;


namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class splitBulkPdfDigitalSignatureController : ApiController
    {
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public splitBulkPdfDigitalSignatureController()
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
        //   public async Task<IHttpActionResult> SplitBulkPdfDigitalSignatureAsync([FromBody] vendorSplitBulkPDFInput vendorSplitBulkPDFInput)
        public HttpResponseMessage SplitBulkPdfDigitalSignatureAsync([FromBody] vendorSplitBulkPDFInput vendorSplitBulkPDFInput)
        {
            try
            {
                string sql = "select MonthYear, RegionCode, VendorCode, FlePath, DocumentNumber, FilePathOriginal, FileName FROM franchiseeinvoicedb.vendor_ds_table where MonthYear = @MonthYear AND VendorCode = @VendorCode AND FileName = @FileName";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                string MonthYear = convertToMonthYearVendor(vendorSplitBulkPDFInput.startDate);

                command.Parameters.AddWithValue("@MonthYear", $"{MonthYear}");
                // command.Parameters.AddWithValue("@RegionCode", $"{vendorSplitBulkPDFInput.region}");
                command.Parameters.AddWithValue("@VendorCode", $"{vendorSplitBulkPDFInput.vendorcode}");
                command.Parameters.AddWithValue("@FileName", $"{vendorSplitBulkPDFInput.fileName}");
                MySqlDataReader reader = command.ExecuteReader();


                List<VendorSplitDS> ListVendorDS = new List<VendorSplitDS>();

                while (reader.Read())
                {
                    VendorSplitDS vendorDSItem = new VendorSplitDS();

                    vendorDSItem.monthYear = reader.GetString("MonthYear");
                    vendorDSItem.region = reader.GetString("RegionCode");
                    vendorDSItem.vendorcode = reader.GetString("VendorCode");
                    vendorDSItem.filePath = reader.GetString("FlePath");
                    vendorDSItem.documentNumber = reader.GetString("DocumentNumber");
                    vendorDSItem.filePathOriginal = reader.GetString("FilePathOriginal");
                    vendorDSItem.fileName = reader.GetString("FileName");
                    // vendorDSItem.DigitalSignatureStatus = reader.GetString("DsStatus"); 
                    // vendorDSItem.TransactionNumber = reader.GetString("TransactionNum"); 
                    // vendorDSItem.ErrorMessage = reader.GetString("ErrorMessage"); 

                    ListVendorDS.Add(vendorDSItem);


                }

                reader.Close();

                String fileLocation = ListVendorDS[0].filePath;



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

                //PdfDocument inputPdf = new PdfDocument(new PdfReader(ListVendorDS[0].filePath));
                /*
                PdfReader pdfReader = new PdfReader(ListVendorDS[0].filePath);
                pdfReader.SetUnethicalReading(true);

                PdfDocument mergedpdf = new PdfDocument(pdfReader);

                int pageCount = mergedpdf.GetNumberOfPages();

                for (int i = 1; i <= pageCount; i++)
                {
                    string outputFilePath = ListVendorDS[i - 1].filePathOriginal;

                    using (MemoryStream splitPdfStream = new MemoryStream())
                    {
                        // Initialize PDF writer
                        PdfWriter writer = new PdfWriter(splitPdfStream);
                        PdfDocument splitPdf = new PdfDocument(writer);


                        mergedpdf.CopyPagesTo(i, i, splitPdf);

                        splitPdf.Close();
                        writer.Close();

                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(splitPdfStream.ToArray())
                        };

                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = ListVendorDS[i - 1].documentNumber + ".pdf"
                        };

                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                        var responseContent = await response.Content.ReadAsByteArrayAsync();

                        File.WriteAllBytes(ListVendorDS[i - 1].filePathOriginal, responseContent);


                        string statusSql = "UPDATE vendor_ds_table SET InvoicePdfDSStatus=@InvoicePdfDSStatus WHERE DocumentNumber = @DocumentNumber ;";
                        // string monthYear = convertToMonthYear(invoiceGenerationInput.startDate);
                        using (var statuscommand = new MySqlCommand(statusSql, _connection))
                        {
                            statuscommand.Parameters.AddWithValue("@InvoicePdfDSStatus", "X");
                            statuscommand.Parameters.AddWithValue("@DocumentNumber", ListVendorDS[i - 1].documentNumber);
                           

                            statuscommand.ExecuteNonQuery();
                        }

                        string statusSqlInvGen = "UPDATE invoice_generation_table SET InvoicePdfDigitalSigStatus=@InvoicePdfDigitalSigStatus WHERE DocumentNumber = @DocumentNumber ;";
                        // string monthYear = convertToMonthYear(invoiceGenerationInput.startDate);
                        using (var statuscommandInvGen = new MySqlCommand(statusSqlInvGen, _connection))
                        {
                            statuscommandInvGen.Parameters.AddWithValue("@InvoicePdfDigitalSigStatus", "X");
                            statuscommandInvGen.Parameters.AddWithValue("@DocumentNumber", ListVendorDS[i - 1].documentNumber);


                            statuscommandInvGen.ExecuteNonQuery();
                        }
                    }
                   
                }

                mergedpdf.Close();
               

                responseCode.messageCode = "S";
                responseCode.messageString = "All PDFs saved with Digital Signature!";
                return Ok(responseCode);*/

            }
            catch (Exception ex)
            {
                /*
                responseCode.messageCode = "E";
                responseCode.messageString = ex.Message;
                return (Content(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseCode)));
                */
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request.", ex);


            }
            finally
            {
                _connection.Close();
            }

        }


        static MemoryStream ConvertPdfToMemoryStream(string filePath)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (PdfReader pdfReader = new PdfReader(filePath))
                {
                    using (PdfWriter pdfWriter = new PdfWriter(memoryStream))
                    {
                        using (PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter))
                        {
                            // You can perform additional operations on the pdfDocument if needed

                            // Optional: Save changes to the pdfDocument if modifications were made
                            // pdfDocument.Close();
                        }
                    }
                }

                return memoryStream;
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