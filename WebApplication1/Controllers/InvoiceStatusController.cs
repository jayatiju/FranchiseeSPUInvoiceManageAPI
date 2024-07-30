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

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class InvoiceStatusController : ApiController
    {
        
        ResponseCode response = new ResponseCode();
        private readonly MySqlConnection _connection;
        public InvoiceStatusController()
        {
            try
            {
                _connection = new MySqlConnection(ConnectionString.connString);
                _connection.Open();
            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                response.messageCode = "E";
                response.messageString = ex.Message;
            }
        }

        [HttpGet]
        public ResponseCode GetStatus(string monthYear, string segment)
        {
            try
            {
                string sql = "SELECT Data_Sync_Flag, Invoice_Number_Generation_Flag, Invoice_PDF_Generation_Flag FROM invoice_monthly_status WHERE Month_Year = @monthYear AND Segment = @segment";

                using (var command = new MySqlCommand(sql, _connection))
                {
                    command.Parameters.AddWithValue("@monthYear", $"{monthYear}");
                    command.Parameters.AddWithValue("@segment", $"{ segment}");

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string dataSyncFlag = reader.GetString("Data_Sync_Flag");
                            string invoiceNumberFlag = reader.GetString("Invoice_Number_Generation_Flag");
                            string invoicePdfFlag = reader.GetString("Invoice_PDF_Generation_Flag");

                            response = GetStatusMessage(dataSyncFlag, invoiceNumberFlag, invoicePdfFlag);


                        }
                        else
                        {
                            response.messageCode = "00";
                            response.messageString = "No data synced";// No entry found in the table
                        }
                    }
                }
            }
            catch(Exception e)
            {
                response.messageCode = "E";
                response.messageString = e.Message;
            }
            return response;
        }


        private ResponseCode GetStatusMessage(string dataSyncFlag, string invoiceNumberFlag, string invoicePdfFlag)
        {
            ResponseCode response = new ResponseCode();

            // Implement your logic to determine the status message based on the criteria provided
            if (dataSyncFlag == "X" && invoiceNumberFlag == "X" && invoicePdfFlag == "")
            {
                response.messageCode = "04";
                response.messageString = "Data synced and Invoice generation is completed";
            }

            else if (dataSyncFlag == "X" && invoiceNumberFlag == "X" && invoicePdfFlag == "IP")
            {
                response.messageCode = "05";
                response.messageString = "Data synced, Invoice generation is completed, and PDF generation is in progress";
            }
            else if (dataSyncFlag == "X" && invoiceNumberFlag == "IP" && invoicePdfFlag == "")
            {
                response.messageCode = "03";
                response.messageString = "Data synced and Invoice generation is in progress";
            }
            else if (dataSyncFlag == "X" && invoiceNumberFlag == "" && invoicePdfFlag == "")
            {
                response.messageCode = "02";
                response.messageString = "Data synced";
            }
            else if (dataSyncFlag == "IP")
            {
                response.messageCode = "01";
                response.messageString = "Request in progress for data sync";
            }
            else if (dataSyncFlag == "X" && invoiceNumberFlag == "X" && invoicePdfFlag == "X")
            {
                response.messageCode = "06";
                response.messageString = "Data synced and Invoice  generation is completed, and PDF generation is completed";
            }
            else
            {
                response.messageCode = "00";
                response.messageString = "No data synced";
            }

            return response;
        }
        
    }
}
