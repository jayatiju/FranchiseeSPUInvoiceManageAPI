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

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class GetInvoiceStatusCountController : ApiController
    {
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public GetInvoiceStatusCountController()
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

        [HttpGet]
        public IHttpActionResult GetInvoiceCountStatus(string firstdate, string lastdate, string segment)
        {
            try
            {
                var result = new Dictionary<string, int>();

                // Count Number of records synced from master table
                string querySynced = "SELECT COUNT(Invoice_Number) FROM invoice_master_table WHERE Document_Date >= @firstdate AND Document_Date <= @lastdate AND Segment = @segment";
                MySqlCommand cmdSynced = new MySqlCommand(querySynced, _connection);
                cmdSynced.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdSynced.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdSynced.Parameters.AddWithValue("@segment", $"{segment}");
                result["Number of records synced"] = Convert.ToInt32(cmdSynced.ExecuteScalar());

                // Count Number of invoices generated from invoice generation table
                string queryInvoicesGenerated = "SELECT COUNT(InvoiceNumberStatus) FROM invoice_generation_table WHERE DocumentDate >= @firstdate AND DocumentDate <= @lastdate AND segmentCode = @segment";
                MySqlCommand cmdInvoicesGenerated = new MySqlCommand(queryInvoicesGenerated, _connection);
                cmdInvoicesGenerated.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdInvoicesGenerated.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdInvoicesGenerated.Parameters.AddWithValue("@segment", $"{segment}");
                result["Number of invoices generated"] = Convert.ToInt32(cmdInvoicesGenerated.ExecuteScalar());

                // Count Number of PDFs created from invoice generation table (flag = 1)
                string queryPDFsCreated = "SELECT COUNT(InvoicePdfStatus) FROM invoice_generation_table WHERE DocumentDate >= @firstdate AND DocumentDate <= @lastdate AND segmentCode = @segment";
                MySqlCommand cmdPDFsCreated = new MySqlCommand(queryPDFsCreated, _connection);
                cmdPDFsCreated.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdPDFsCreated.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdPDFsCreated.Parameters.AddWithValue("@segment", $"{segment}");
                result["Number of PDFs created"] = Convert.ToInt32(cmdPDFsCreated.ExecuteScalar());

                // Count Number of digitally signed from invoice generation table (flag = 1)
                string queryDigitallySigned = "SELECT COUNT(InvoicePdfDigitalSigStatus) FROM invoice_generation_table WHERE DocumentDate >= @firstdate AND DocumentDate <= @lastdate AND segmentCode = @segment" ;
                MySqlCommand cmdDigitallySigned = new MySqlCommand(queryDigitallySigned, _connection);
                cmdDigitallySigned.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdDigitallySigned.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdDigitallySigned.Parameters.AddWithValue("@segment", $"{segment}");
                result["Number of digitally signed"] = Convert.ToInt32(cmdDigitallySigned.ExecuteScalar());

                return Ok(result);
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
    }

}