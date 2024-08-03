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
    public class GetInvoiceGenerationController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();

        private readonly MySqlConnection _connection;
        public GetInvoiceGenerationController()
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


        //for region and vendorcode
        [HttpGet]
        public IHttpActionResult GetInvoice (string startdate, string enddate, string vendorcode)
        {
            try
            {
                string sql = "SELECT * FROM invoice_generation_table WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate  AND VendorCode = @vendorCode ";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@vendorCode", $"{vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceGenerationOutput> listInvoiceGenerationOutputs = new List<InvoiceGenerationOutput>();

                while (reader.Read())
                {
                    InvoiceGenerationOutput invoiceGenerationOutput = new InvoiceGenerationOutput();

                    invoiceGenerationOutput.InvoiceNumber = reader.GetString("InvoiceNumber");
                    invoiceGenerationOutput.InvoiceDate = reader.GetString("InvoiceDate");
                    invoiceGenerationOutput.SPUNumber = reader.GetString("SPUNumber");
                    invoiceGenerationOutput.CRMTicketNumber = reader.GetString("CRMTicketNumber");
                    invoiceGenerationOutput.DocumentNumber = reader.GetString("DocumentNumber");
                    invoiceGenerationOutput.DocumentDate = reader.GetString("DocumentDate");
                    invoiceGenerationOutput.VendorName = reader.GetString("VendorName");
                    invoiceGenerationOutput.VendorCode = reader.GetString("VendorCode");
                    invoiceGenerationOutput.CustomerName = reader.GetString("CustomerName");
                    invoiceGenerationOutput.CustomerCode = reader.GetString("CustomerCode");
                    invoiceGenerationOutput.ShipToPartyNumber = reader.GetString("ShipToPartyNumber");
                    invoiceGenerationOutput.ShipToPartyName = reader.GetString("ShipToPartyName");
                    invoiceGenerationOutput.SubTotal = reader.GetString("SubTotal");
                    invoiceGenerationOutput.SGST = reader.GetString("SGST");
                    invoiceGenerationOutput.CGST = reader.GetString("CGST");
                    invoiceGenerationOutput.IGST = reader.GetString("IGST");
                    invoiceGenerationOutput.UGST = reader.GetString("UGST");
                    invoiceGenerationOutput.RoundOff = reader.GetString("RoundOff");
                    invoiceGenerationOutput.GrandTotal = reader.GetString("GrandTotal");
                    invoiceGenerationOutput.InvoiceNumberStatus = reader.GetString("InvoiceNumberStatus");
                    invoiceGenerationOutput.InvoicePdfStatus = reader.GetString("InvoicePdfStatus");
                    invoiceGenerationOutput.InvoicePdfDigitalSigStatus = reader.GetString("InvoicePdfDigitalSigStatus");
                    invoiceGenerationOutput.InvoicePdfLocation = reader.GetString("InvoicePdfLocation");
                    invoiceGenerationOutput.regionCode = reader.GetString("regionCode");
                    invoiceGenerationOutput.segmentCode = reader.GetString("segmentCode");

                    invoiceGenerationOutput.InvoicePdfDuplicateStatus = reader.GetString("InvoicePdfDuplicateStatus");
                    invoiceGenerationOutput.InvoicePdfDuplicateLocation = reader.GetString("InvoicePdfDuplicateLocation");

                    invoiceGenerationOutput.InvoicePdfTriplicateStatus = reader.GetString("InvoicePdfTriplicateStatus");
                    invoiceGenerationOutput.InvoicePdfTriplicateLocation = reader.GetString("InvoicePdfTriplicateLocation");

                    invoiceGenerationOutput.branchGSTIN = reader.GetString("branchgstin");
                    invoiceGenerationOutput.shiptopartyaddress = reader.GetString("shiptopartyaddress");


                    listInvoiceGenerationOutputs.Add(invoiceGenerationOutput);
                }

                reader.Close();
                return Ok(listInvoiceGenerationOutputs);
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

        //segment and vendor
        [HttpGet]
        public IHttpActionResult GetInvoiceSeg(string startdate, string enddate, string segment, string vendorcode)
        {
            try
            {
                string sql = "SELECT * FROM invoice_generation_table WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate AND segmentCode = @segmentCode AND VendorCode = @vendorCode ";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@segmentCode", $"{segment}");
                command.Parameters.AddWithValue("@vendorCode", $"{vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceGenerationOutput> listInvoiceGenerationOutputs = new List<InvoiceGenerationOutput>();

                while (reader.Read())
                {
                    InvoiceGenerationOutput invoiceGenerationOutput = new InvoiceGenerationOutput();

                    invoiceGenerationOutput.InvoiceNumber = reader.GetString("InvoiceNumber");
                    invoiceGenerationOutput.InvoiceDate = reader.GetString("InvoiceDate");
                    invoiceGenerationOutput.SPUNumber = reader.GetString("SPUNumber");
                    invoiceGenerationOutput.CRMTicketNumber = reader.GetString("CRMTicketNumber");
                    invoiceGenerationOutput.DocumentNumber = reader.GetString("DocumentNumber");
                    invoiceGenerationOutput.DocumentDate = reader.GetString("DocumentDate");
                    invoiceGenerationOutput.VendorName = reader.GetString("VendorName");
                    invoiceGenerationOutput.VendorCode = reader.GetString("VendorCode");
                    invoiceGenerationOutput.CustomerName = reader.GetString("CustomerName");
                    invoiceGenerationOutput.CustomerCode = reader.GetString("CustomerCode");
                    invoiceGenerationOutput.ShipToPartyNumber = reader.GetString("ShipToPartyNumber");
                    invoiceGenerationOutput.ShipToPartyName = reader.GetString("ShipToPartyName");
                    invoiceGenerationOutput.SubTotal = reader.GetString("SubTotal");
                    invoiceGenerationOutput.SGST = reader.GetString("SGST");
                    invoiceGenerationOutput.CGST = reader.GetString("CGST");
                    invoiceGenerationOutput.IGST = reader.GetString("IGST");
                    invoiceGenerationOutput.UGST = reader.GetString("UGST");
                    invoiceGenerationOutput.RoundOff = reader.GetString("RoundOff");
                    invoiceGenerationOutput.GrandTotal = reader.GetString("GrandTotal");
                    invoiceGenerationOutput.InvoiceNumberStatus = reader.GetString("InvoiceNumberStatus");
                    invoiceGenerationOutput.InvoicePdfStatus = reader.GetString("InvoicePdfStatus");
                    invoiceGenerationOutput.InvoicePdfDigitalSigStatus = reader.GetString("InvoicePdfDigitalSigStatus");
                    invoiceGenerationOutput.InvoicePdfLocation = reader.GetString("InvoicePdfLocation");
                    invoiceGenerationOutput.regionCode = reader.GetString("regionCode");
                    invoiceGenerationOutput.segmentCode = reader.GetString("segmentCode");

                    invoiceGenerationOutput.InvoicePdfDuplicateStatus = reader.GetString("InvoicePdfDuplicateStatus");
                    invoiceGenerationOutput.InvoicePdfDuplicateLocation = reader.GetString("InvoicePdfDuplicateLocation");

                    invoiceGenerationOutput.InvoicePdfTriplicateStatus = reader.GetString("InvoicePdfTriplicateStatus");
                    invoiceGenerationOutput.InvoicePdfTriplicateLocation = reader.GetString("InvoicePdfTriplicateLocation");

                    invoiceGenerationOutput.branchGSTIN = reader.GetString("branchgstin");
                    invoiceGenerationOutput.shiptopartyaddress = reader.GetString("shiptopartyaddress");

                    listInvoiceGenerationOutputs.Add(invoiceGenerationOutput);
                }

                reader.Close();
                return Ok(listInvoiceGenerationOutputs);
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

        //only region
        [HttpGet]
        public IHttpActionResult GetInvoiceReg(string startdate, string enddate, string region)
        {
            try
            {
                string sql = "SELECT * FROM invoice_generation_table WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate AND regionCode = @regionCode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@regionCode", $"{region}");
                
                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceGenerationOutput> listInvoiceGenerationOutputs = new List<InvoiceGenerationOutput>();

                while (reader.Read())
                {
                    InvoiceGenerationOutput invoiceGenerationOutput = new InvoiceGenerationOutput();

                    invoiceGenerationOutput.InvoiceNumber = reader.GetString("InvoiceNumber");
                    invoiceGenerationOutput.InvoiceDate = reader.GetString("InvoiceDate");
                    invoiceGenerationOutput.SPUNumber = reader.GetString("SPUNumber");
                    invoiceGenerationOutput.CRMTicketNumber = reader.GetString("CRMTicketNumber");
                    invoiceGenerationOutput.DocumentNumber = reader.GetString("DocumentNumber");
                    invoiceGenerationOutput.DocumentDate = reader.GetString("DocumentDate");
                    invoiceGenerationOutput.VendorName = reader.GetString("VendorName");
                    invoiceGenerationOutput.VendorCode = reader.GetString("VendorCode");
                    invoiceGenerationOutput.CustomerName = reader.GetString("CustomerName");
                    invoiceGenerationOutput.CustomerCode = reader.GetString("CustomerCode");
                    invoiceGenerationOutput.ShipToPartyNumber = reader.GetString("ShipToPartyNumber");
                    invoiceGenerationOutput.ShipToPartyName = reader.GetString("ShipToPartyName");
                    invoiceGenerationOutput.SubTotal = reader.GetString("SubTotal");
                    invoiceGenerationOutput.SGST = reader.GetString("SGST");
                    invoiceGenerationOutput.CGST = reader.GetString("CGST");
                    invoiceGenerationOutput.IGST = reader.GetString("IGST");
                    invoiceGenerationOutput.UGST = reader.GetString("UGST");
                    invoiceGenerationOutput.RoundOff = reader.GetString("RoundOff");
                    invoiceGenerationOutput.GrandTotal = reader.GetString("GrandTotal");
                    invoiceGenerationOutput.InvoiceNumberStatus = reader.GetString("InvoiceNumberStatus");
                    invoiceGenerationOutput.InvoicePdfStatus = reader.GetString("InvoicePdfStatus");
                    invoiceGenerationOutput.InvoicePdfDigitalSigStatus = reader.GetString("InvoicePdfDigitalSigStatus");
                    invoiceGenerationOutput.InvoicePdfLocation = reader.GetString("InvoicePdfLocation");
                    invoiceGenerationOutput.regionCode = reader.GetString("regionCode");
                    invoiceGenerationOutput.segmentCode = reader.GetString("segmentCode");

                    invoiceGenerationOutput.InvoicePdfDuplicateStatus = reader.GetString("InvoicePdfDuplicateStatus");
                    invoiceGenerationOutput.InvoicePdfDuplicateLocation = reader.GetString("InvoicePdfDuplicateLocation");

                    invoiceGenerationOutput.InvoicePdfTriplicateStatus = reader.GetString("InvoicePdfTriplicateStatus");
                    invoiceGenerationOutput.InvoicePdfTriplicateLocation = reader.GetString("InvoicePdfTriplicateLocation");

                    invoiceGenerationOutput.branchGSTIN = reader.GetString("branchgstin");
                    invoiceGenerationOutput.shiptopartyaddress = reader.GetString("shiptopartyaddress");




                    listInvoiceGenerationOutputs.Add(invoiceGenerationOutput);
                }

                reader.Close();
                return Ok(listInvoiceGenerationOutputs);
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


        //only segment
        [HttpGet]
        public IHttpActionResult GetInvoiceSeg(string startdate, string enddate, string segment)
        {
            try
            {
                string sql = "SELECT * FROM invoice_generation_table WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate AND segmentCode = @segmentCode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@segmentCode", $"{segment}");

                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceGenerationOutput> listInvoiceGenerationOutputs = new List<InvoiceGenerationOutput>();

                while (reader.Read())
                {
                    InvoiceGenerationOutput invoiceGenerationOutput = new InvoiceGenerationOutput();

                    invoiceGenerationOutput.InvoiceNumber = reader.GetString("InvoiceNumber");
                    invoiceGenerationOutput.InvoiceDate = reader.GetString("InvoiceDate");
                    invoiceGenerationOutput.SPUNumber = reader.GetString("SPUNumber");
                    invoiceGenerationOutput.CRMTicketNumber = reader.GetString("CRMTicketNumber");
                    invoiceGenerationOutput.DocumentNumber = reader.GetString("DocumentNumber");
                    invoiceGenerationOutput.DocumentDate = reader.GetString("DocumentDate");
                    invoiceGenerationOutput.VendorName = reader.GetString("VendorName");
                    invoiceGenerationOutput.VendorCode = reader.GetString("VendorCode");
                    invoiceGenerationOutput.CustomerName = reader.GetString("CustomerName");
                    invoiceGenerationOutput.CustomerCode = reader.GetString("CustomerCode");
                    invoiceGenerationOutput.ShipToPartyNumber = reader.GetString("ShipToPartyNumber");
                    invoiceGenerationOutput.ShipToPartyName = reader.GetString("ShipToPartyName");
                    invoiceGenerationOutput.SubTotal = reader.GetString("SubTotal");
                    invoiceGenerationOutput.SGST = reader.GetString("SGST");
                    invoiceGenerationOutput.CGST = reader.GetString("CGST");
                    invoiceGenerationOutput.IGST = reader.GetString("IGST");
                    invoiceGenerationOutput.UGST = reader.GetString("UGST");
                    invoiceGenerationOutput.RoundOff = reader.GetString("RoundOff");
                    invoiceGenerationOutput.GrandTotal = reader.GetString("GrandTotal");
                    invoiceGenerationOutput.InvoiceNumberStatus = reader.GetString("InvoiceNumberStatus");
                    invoiceGenerationOutput.InvoicePdfStatus = reader.GetString("InvoicePdfStatus");
                    invoiceGenerationOutput.InvoicePdfDigitalSigStatus = reader.GetString("InvoicePdfDigitalSigStatus");
                    invoiceGenerationOutput.InvoicePdfLocation = reader.GetString("InvoicePdfLocation");
                    invoiceGenerationOutput.regionCode = reader.GetString("regionCode");
                    invoiceGenerationOutput.segmentCode = reader.GetString("segmentCode");

                    invoiceGenerationOutput.InvoicePdfDuplicateStatus = reader.GetString("InvoicePdfDuplicateStatus");
                    invoiceGenerationOutput.InvoicePdfDuplicateLocation = reader.GetString("InvoicePdfDuplicateLocation");

                    invoiceGenerationOutput.InvoicePdfTriplicateStatus = reader.GetString("InvoicePdfTriplicateStatus");
                    invoiceGenerationOutput.InvoicePdfTriplicateLocation = reader.GetString("InvoicePdfTriplicateLocation");

                    invoiceGenerationOutput.branchGSTIN = reader.GetString("branchgstin");
                    invoiceGenerationOutput.shiptopartyaddress = reader.GetString("shiptopartyaddress");

                    listInvoiceGenerationOutputs.Add(invoiceGenerationOutput);
                }

                reader.Close();
                return Ok(listInvoiceGenerationOutputs);
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