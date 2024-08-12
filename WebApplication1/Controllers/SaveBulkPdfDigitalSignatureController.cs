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
    public class SaveBulkPdfDigitalSignatureController : ApiController
    {
        ResponseCode responseCode = new ResponseCode();

        private readonly MySqlConnection _connection;
        public SaveBulkPdfDigitalSignatureController()
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
        public async Task<IHttpActionResult> SaveMergedVendorPDFsAsync([FromBody] vendorBulkPDFInput vendorBulkPDFInput)
        {
            try
            {
                string sql = "SELECT DocumentNumber, InvoicePdfLocation FROM invoice_generation_table WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate AND regionCode = @regionCode AND VendorCode = @vendorCode ";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{vendorBulkPDFInput.startDate}");
                command.Parameters.AddWithValue("@endDate", $"{vendorBulkPDFInput.endDate}");
                //command.Parameters.AddWithValue("@regionCode", $"{vendorBulkPDFInput.region}");
                command.Parameters.AddWithValue("@vendorCode", $"{vendorBulkPDFInput.vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<String> fileLocations = new List<String>();
                List<String> listDocumentNum = new List<String>();

                while (reader.Read())
                {
                    String FileLocation;
                    String DocumentNum;
                        //, DigitalSigStatus;

                   // DigitalSigStatus = reader.GetString("InvoicePdfDigitalSigStatus");
                    FileLocation = reader.GetString("InvoicePdfLocation");
                    DocumentNum = reader.GetString("DocumentNumber");

                    //listDigitalSigStatus.Add(DigitalSigStatus);
                    fileLocations.Add(FileLocation);
                    listDocumentNum.Add(DocumentNum);
                }

                reader.Close();

                string folderPath = fileLocations[0].Substring(0, 23);
                string folderPathNew = Path.Combine(folderPath, vendorBulkPDFInput.vendorcode);

                // Check if the folder exists, and create it if not
                if (!Directory.Exists(folderPathNew))
                {
                    Directory.CreateDirectory(folderPathNew);
                }

                // Decimal documentCount1 = (decimal)(listDocumentNum.Count / 120);

                int documentCount = (listDocumentNum.Count +119) / 120;

                List<VendorDS> listVendorDS = new List<VendorDS>();

                for (var i = 1; i <= documentCount; i++)
                {
                    List<String> fileLocationsTemp = new List<String>();

                    int documentlimit =0;
                    int documentstart = 0;

                    if (listDocumentNum.Count <= 120)
                    {
                        documentstart = 1;
                        documentlimit = listDocumentNum.Count;
                    }

                    if (listDocumentNum.Count > 120)
                    {
                        documentstart = (i-1) * 120 + 1;
                        if (listDocumentNum.Count - i * 120 >= 0)
                        {
                            documentlimit = i * 120;                      
                        }
                        else
                        {
                            documentlimit = listDocumentNum.Count;
                        }
                    }

                    for (var j = documentstart-1; j < documentlimit; j++)
                    {
                        fileLocationsTemp.Add(fileLocations[j]);
                    }
                   // string jsonFileLocations = JsonConvert.SerializeObject(fileLocationsTemp);

                    // Create an instance of HttpClient
                    using (var httpClient = new HttpClient())
                    {
                        // Define the API endpoint URL
                       // var apiUrl = "https://frspuinv.ifbsupport.com/api/GetBulkPdf";
                         var apiUrl = "http://localhost:8080/api/GetBulkPdf";
                        // var apiUrl = "https://localhost:44361/api/GetBulkPdf";

                        // Create an anonymous object with the required structure
                        var payload = new
                        {
                            FileLocations = fileLocationsTemp
                        };

                        // Serialize the payload to JSON
                        var jsonPayload = JsonConvert.SerializeObject(payload);


                        // Create a StringContent with the serialized fileLocationsTemp
                        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                        // Make a POST request to the API
                        var response = await httpClient.PostAsync(apiUrl, content);

                        // Handle the API response as needed
                        if (response.IsSuccessStatusCode)
                        {
                            // Successful API call
                            var responseContent = await response.Content.ReadAsByteArrayAsync();

                            // Save the PDF to a folder location
                           
                            string fileName = $"MergedPDF_{convertToMonthYearVendor(vendorBulkPDFInput.startDate)}_{vendorBulkPDFInput.vendorcode}_{i}.pdf";
                            string filePath = Path.Combine(folderPathNew, fileName);

                           
                            File.WriteAllBytes(filePath, responseContent);

                            VendorDS vendorDSItem = new VendorDS();

                            vendorDSItem.monthYear = convertToMonthYearVendor(vendorBulkPDFInput.startDate);
                            //vendorDSItem.region = vendorBulkPDFInput.region;
                            vendorDSItem.vendorcode = vendorBulkPDFInput.vendorcode;
                            vendorDSItem.filePath = filePath;
                            vendorDSItem.fileName = fileName;
                            vendorDSItem.DigitalSignatureStatus = "";
                            vendorDSItem.TransactionNumber = "";
                            vendorDSItem.ErrorMessage = "";
                            vendorDSItem.ReferenceNum = "";
                            vendorDSItem.InvoicePdfDSStatus = "";

                            listVendorDS.Add(vendorDSItem);

                            for (var j = documentstart-1; j < documentlimit; j++)
                            {
                                
                                string insertSql = "insert into vendor_ds_table (MonthYear, RegionCode, VendorCode, FlePath, DocumentNumber, FilePathOriginal, FileName, DsStatus, TransactionNum, ErrorMessage, ReferenceNum, InvoicePdfDSStatus) " +
                           "values (@MonthYear, @RegionCode, @VendorCode, @FlePath, @DocumentNumber, @FilePathOriginal, @FileName, '', '', '', '', '')";

                                using (var insertCommand = new MySqlCommand(insertSql, _connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@MonthYear", convertToMonthYearVendor(vendorBulkPDFInput.startDate));
                                    //insertCommand.Parameters.AddWithValue("@RegionCode", vendorBulkPDFInput.region);
                                    insertCommand.Parameters.AddWithValue("@VendorCode", vendorBulkPDFInput.vendorcode);
                                    insertCommand.Parameters.AddWithValue("@FlePath", filePath);
                                    insertCommand.Parameters.AddWithValue("@DocumentNumber", listDocumentNum[j]);
                                    insertCommand.Parameters.AddWithValue("@FilePathOriginal", fileLocations[j]);
                                    insertCommand.Parameters.AddWithValue("@FileName", fileName);

                                    insertCommand.ExecuteNonQuery();
                                }
                            }

                            // Log or do additional processing as needed
                            Console.WriteLine($"PDF saved successfully at: {filePath}");
                        }
                        else
                        {
                            // Error handling for unsuccessful API call
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            // Handle error message if needed
                        }
                    }
                }

                return Ok(listVendorDS);
            }
            catch (Exception ex)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = ex.Message;
                return Content(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseCode));

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