using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Http;
using MySql.Data.MySqlClient;
using WebApplication1.Models;
using System.Windows;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using System.Globalization;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class GetInvoiceGenErrorController : ApiController
    {
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public GetInvoiceGenErrorController()
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
        public IHttpActionResult GetInvoiceGenError([FromBody] InvoiceGenerationInput invoiceGenerationInput)
        {
            try
            {

               // string fy = convertToYear(invoiceGenerationInput.startDate);

                string sql = "SELECT InvoiceNumber, InvoiceDate, (invoice_generation_table.DocumentNumber) AS document, (invoice_update_error_sap.message) AS message FROM invoice_generation_table RIGHT OUTER JOIN invoice_update_error_sap ON invoice_generation_table.DocumentNumber = invoice_update_error_sap.document WHERE invoice_generation_table.InvoiceDate >= @startDate AND invoice_generation_table.InvoiceDate <= @endDate AND invoice_generation_table.segmentCode = @segmentCode;";
              
                MySqlCommand command = new MySqlCommand(sql, _connection);

                command.Parameters.AddWithValue("@startDate", invoiceGenerationInput.startDate);
                command.Parameters.AddWithValue("@endDate", invoiceGenerationInput.endDate);
                command.Parameters.AddWithValue("@segmentCode", invoiceGenerationInput.segment);

                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceError> errorList = new List<InvoiceError>();

                while (reader.Read())
                {
                    InvoiceError invoiceError = new InvoiceError();

                    invoiceError.DocumentNumber = reader.GetString("document");
                    invoiceError.Message = reader.GetString("message");
                    invoiceError.InvoiceNumber = reader.GetString("InvoiceNumber");
                    invoiceError.InvoiceDate = reader.GetString("InvoiceDate");
                    invoiceError.FiscalYear = convertToYear(reader.GetString("InvoiceDate")); ;
                    errorList.Add(invoiceError);
                }

                reader.Close();
                return Ok(errorList);
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

        public static string convertToYear(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                //DateTime date = DateTime.Parse(inputDate);


                int currentYear = date.Year;
                int fiscalYear;
                if (date.Month >= 4) // Assuming fiscal year starts from April
                {
                    fiscalYear = currentYear;
                }
                else
                {
                    fiscalYear = currentYear - 1;
                }

                // Format the DateTime object to yyyyMM (month-year) string
                //string result = date.ToString("yyyy");
                //string result = "" + (date.Month < 4 ? date.Year - 1 : date.Year);

                return fiscalYear.ToString();
            }
            catch (FormatException)
            {
                // Handle invalid input date format
                return "Invalid Date Format";
            }
        }

    }
}