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
using System.Text;

namespace WebApplication1.Controllers
{
    public class PdfGenerationController : ApiController
    {
        
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public PdfGenerationController()
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
        public IHttpActionResult Post([FromBody] InvoiceGenerationInput invoiceGenerationInput)
        {

            try
            {
                string monthYear = convertToMonthYear(invoiceGenerationInput.startDate);



                // Make the API call to generate PDF

                string generatePdfUrl = $"https://frspuinv.ifbsupport.com:9000/generatePDF/{invoiceGenerationInput.startDate}/{invoiceGenerationInput.endDate}/{invoiceGenerationInput.segment}";

                //var requestBody = new
                //{
                 //   startDate = invoiceGenerationInput.startDate,
                   // endDate = invoiceGenerationInput.endDate,
                   // regionCode = invoiceGenerationInput.regionCode
                //};

                //string jsonRequest = JsonConvert.SerializeObject(requestBody);
                string jsonRequest = "";
                HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {

                    httpClient.Timeout = TimeSpan.FromMinutes(30);
                    

                    string statusSqlPdfIp = "UPDATE invoice_monthly_status SET Invoice_PDF_Generation_Flag = 'IP' WHERE Month_Year = @Month_Year AND Segment = @Segment;";

                    using (var statuscommandPdfIp = new MySqlCommand(statusSqlPdfIp, _connection))
                    {
                        statuscommandPdfIp.Parameters.AddWithValue("@Month_Year", monthYear);
                        statuscommandPdfIp.Parameters.AddWithValue("@Segment", invoiceGenerationInput.segment);

                        statuscommandPdfIp.ExecuteNonQuery();


                    }

                    try
                    {
                        HttpResponseMessage apiResponse = httpClient.PostAsync(generatePdfUrl, content).Result;

                        if (apiResponse.IsSuccessStatusCode)
                        {
                            // Handle the successful API response if needed
                            responseCode.messageCode = "S";
                            responseCode.messageString = apiResponse.ReasonPhrase;
                            return Ok(responseCode);
                        }
                        else
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = apiResponse.ToString();

                            return Ok(responseCode);
                        }
                    }
                    catch (Exception e)
                    {
                        responseCode.messageCode = "E";
                        responseCode.messageString = e.Message;
                    }
                    
                }
                

                return Ok(responseCode);
            }

            catch (Exception e)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = e.Message;
                return Content(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseCode));
            }
            finally
            {
                _connection.Close();
            }

        }

        public static string convertToMonthYear(string inputDate)
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