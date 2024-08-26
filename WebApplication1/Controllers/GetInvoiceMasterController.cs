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
    public class GetInvoiceMasterController : ApiController
    {

        ResponseCode responseCode = new ResponseCode();

        private readonly MySqlConnection _connection;
        public GetInvoiceMasterController()
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

        //vendorcode
        [HttpGet]
        public IHttpActionResult GetInvoice(string startdate, string enddate, string vendorcode)
        {
            try
            {
                string updateSql = @"UPDATE invoice_master_table im
                               JOIN invoice_generation_table ig ON im.Document_Number = ig.DocumentNumber
                                SET im.Invoice_Number = ig.InvoiceNumber";
                MySqlCommand updateCommand = new MySqlCommand(updateSql, _connection);

                // Execute the update command
                updateCommand.ExecuteNonQuery();
                string fetchSql = "SELECT * FROM invoice_master_table WHERE Document_Date >= @startDate AND Document_Date <= @endDate  AND Vendor_Code = @vendorCode ";
                MySqlCommand command = new MySqlCommand(fetchSql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@vendorCode", $"{vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceMasterOutput> listInvoiceMasterOutputs = new List<InvoiceMasterOutput>();

                while (reader.Read())
                {
                    InvoiceMasterOutput invoiceMasterOutput = new InvoiceMasterOutput();

                    invoiceMasterOutput.Segment = reader.GetString("Segment");
                    invoiceMasterOutput.Region_Code = reader.GetString("Region_Code");
                    invoiceMasterOutput.Plant_Code = reader.GetString("Plant_Code");
                    invoiceMasterOutput.Financial_Year = reader.GetString("Financial_Year");
                    invoiceMasterOutput.GSTIN = reader.GetString("GSTIN");
                    invoiceMasterOutput.Document_Number = reader.GetString("Document_Number");
                    invoiceMasterOutput.Document_Date = reader.GetString("Document_Date");
                    invoiceMasterOutput.Document_Posting_Date = reader.GetString("Document_Posting_Date");
                    invoiceMasterOutput.Sales_Doc_Number = reader.GetString("Sales_Doc_Number");
                    invoiceMasterOutput.Ship_To_Party_Number = reader.GetString("Ship_To_Party_Number");
                    invoiceMasterOutput.Ship_To_Party_Name = reader.GetString("Ship_To_Party_Name");
                    invoiceMasterOutput.Pin = reader.GetString("Pin");
                    invoiceMasterOutput.City = reader.GetString("City");
                    invoiceMasterOutput.Address_1 = reader.GetString("Address_1");
                    invoiceMasterOutput.Address_2 = reader.GetString("Address_2");
                    invoiceMasterOutput.Address_3 = reader.GetString("Address_3");
                    invoiceMasterOutput.Vendor_Code = reader.GetString("Vendor_Code");
                    invoiceMasterOutput.Vendor_Name = reader.GetString("Vendor_Name");
                    invoiceMasterOutput.Spu_Number = reader.GetString("Spu_Number");
                    invoiceMasterOutput.CRM_Ticket_Number = reader.GetString("CRM_Ticket_Number");
                    invoiceMasterOutput.Machine_Status = reader.GetString("Machine_Status");
                    invoiceMasterOutput.COGS = reader.GetString("COGS");
                    invoiceMasterOutput.Material_Code = reader.GetString("Material_Code");
                    invoiceMasterOutput.Material_Group = reader.GetString("Material_Group");
                    invoiceMasterOutput.Material_Description = reader.GetString("Material_Description");

                    invoiceMasterOutput.HSN = reader.GetString("HSN");
                    invoiceMasterOutput.Tax_Percentage = reader.GetString("Tax_Percentage");

                    invoiceMasterOutput.Quantity = reader.GetString("Quantity");
                    invoiceMasterOutput.UOM = reader.GetString("UOM");

                    invoiceMasterOutput.Spare_Value = reader.GetString("Spare_Value");
                    invoiceMasterOutput.Currency = reader.GetString("Currency");
                    invoiceMasterOutput.Assignment_Date = reader.GetString("Assignment_Date");
                    invoiceMasterOutput.Tax_Code = reader.GetString("Tax_Code");
                    invoiceMasterOutput.CGST_Percentage = reader.GetString("CGST_Percentage");
                    invoiceMasterOutput.CGST_RCM = reader.GetString("CGST_RCM");
                    invoiceMasterOutput.CGST = reader.GetString("CGST");
                    invoiceMasterOutput.IGST_Percentage = reader.GetString("IGST_Percentage");
                    invoiceMasterOutput.IGST = reader.GetString("IGST");
                    invoiceMasterOutput.Import_IGST = reader.GetString("Import_IGST");

                    invoiceMasterOutput.IGST_RCM = reader.GetString("IGST_RCM");
                    invoiceMasterOutput.SGST_Percentage = reader.GetString("SGST_Percentage");
                    invoiceMasterOutput.SGST = reader.GetString("SGST");
                    invoiceMasterOutput.UGST_Percentage = reader.GetString("UGST_Percentage");
                    invoiceMasterOutput.UGST = reader.GetString("UGST");

                    invoiceMasterOutput.UGST_RCM = reader.GetString("UGST_RCM");
                    invoiceMasterOutput.SGST_RCM = reader.GetString("SGST_RCM");
                    invoiceMasterOutput.Invoice_Number = reader.GetString("Invoice_Number");
                    invoiceMasterOutput.FG_Product_Code = reader.GetString("FG_Product_Code");
                    invoiceMasterOutput.FG_Product_Name = reader.GetString("FG_Product_Name");
                    invoiceMasterOutput.Ship_To_Party_MobileNumber = reader.GetString("Ship_To_Party_MobileNumber");
                    invoiceMasterOutput.Ship_To_Party_Region = reader.GetString("Ship_To_Party_Region");
                    invoiceMasterOutput.Ship_To_Party_Region_Desc = reader.GetString("Ship_To_Party_Region_Desc");



                    listInvoiceMasterOutputs.Add(invoiceMasterOutput);
                }

                reader.Close();
                return Ok(listInvoiceMasterOutputs);
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
                string updateSql = @"UPDATE invoice_master_table im
                               JOIN invoice_generation_table ig ON im.Document_Number = ig.DocumentNumber
                                SET im.Invoice_Number = ig.InvoiceNumber";
                MySqlCommand updateCommand = new MySqlCommand(updateSql, _connection);

                // Execute the update command
                updateCommand.ExecuteNonQuery();

                string fetchSql = "SELECT * FROM invoice_master_table WHERE Document_Date >= @startDate AND Document_Date <= @endDate AND Segment = @segmentCode AND Vendor_Code = @vendorCode ";
                MySqlCommand command = new MySqlCommand(fetchSql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@segmentCode", $"{segment}");
                command.Parameters.AddWithValue("@vendorCode", $"{vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceMasterOutput> listInvoiceMasterOutputs = new List<InvoiceMasterOutput>();

                while (reader.Read())
                {
                    InvoiceMasterOutput invoiceMasterOutput = new InvoiceMasterOutput();

                    invoiceMasterOutput.Segment = reader.GetString("Segment");
                    invoiceMasterOutput.Region_Code = reader.GetString("Region_Code");
                    invoiceMasterOutput.Plant_Code = reader.GetString("Plant_Code");
                    invoiceMasterOutput.Financial_Year = reader.GetString("Financial_Year");
                    invoiceMasterOutput.GSTIN = reader.GetString("GSTIN");
                    invoiceMasterOutput.Document_Number = reader.GetString("Document_Number");
                    invoiceMasterOutput.Document_Date = reader.GetString("Document_Date");
                    invoiceMasterOutput.Document_Posting_Date = reader.GetString("Document_Posting_Date");
                    invoiceMasterOutput.Sales_Doc_Number = reader.GetString("Sales_Doc_Number");
                    invoiceMasterOutput.Ship_To_Party_Number = reader.GetString("Ship_To_Party_Number");
                    invoiceMasterOutput.Ship_To_Party_Name = reader.GetString("Ship_To_Party_Name");
                    invoiceMasterOutput.Pin = reader.GetString("Pin");
                    invoiceMasterOutput.City = reader.GetString("City");
                    invoiceMasterOutput.Address_1 = reader.GetString("Address_1");
                    invoiceMasterOutput.Address_2 = reader.GetString("Address_2");
                    invoiceMasterOutput.Address_3 = reader.GetString("Address_3");
                    invoiceMasterOutput.Vendor_Code = reader.GetString("Vendor_Code");
                    invoiceMasterOutput.Vendor_Name = reader.GetString("Vendor_Name");
                    invoiceMasterOutput.Spu_Number = reader.GetString("Spu_Number");
                    invoiceMasterOutput.CRM_Ticket_Number = reader.GetString("CRM_Ticket_Number");
                    invoiceMasterOutput.Machine_Status = reader.GetString("Machine_Status");
                    invoiceMasterOutput.COGS = reader.GetString("COGS");
                    invoiceMasterOutput.Material_Code = reader.GetString("Material_Code");
                    invoiceMasterOutput.Material_Group = reader.GetString("Material_Group");
                    invoiceMasterOutput.Material_Description = reader.GetString("Material_Description");

                    invoiceMasterOutput.HSN = reader.GetString("HSN");
                    invoiceMasterOutput.Tax_Percentage = reader.GetString("Tax_Percentage");

                    invoiceMasterOutput.Quantity = reader.GetString("Quantity");
                    invoiceMasterOutput.UOM = reader.GetString("UOM");

                    invoiceMasterOutput.Spare_Value = reader.GetString("Spare_Value");
                    invoiceMasterOutput.Currency = reader.GetString("Currency");
                    invoiceMasterOutput.Assignment_Date = reader.GetString("Assignment_Date");
                    invoiceMasterOutput.Tax_Code = reader.GetString("Tax_Code");
                    invoiceMasterOutput.CGST_Percentage = reader.GetString("CGST_Percentage");
                    invoiceMasterOutput.CGST_RCM = reader.GetString("CGST_RCM");
                    invoiceMasterOutput.CGST = reader.GetString("CGST");
                    invoiceMasterOutput.IGST_Percentage = reader.GetString("IGST_Percentage");
                    invoiceMasterOutput.IGST = reader.GetString("IGST");
                    invoiceMasterOutput.Import_IGST = reader.GetString("Import_IGST");

                    invoiceMasterOutput.IGST_RCM = reader.GetString("IGST_RCM");
                    invoiceMasterOutput.SGST_Percentage = reader.GetString("SGST_Percentage");
                    invoiceMasterOutput.SGST = reader.GetString("SGST");
                    invoiceMasterOutput.UGST_Percentage = reader.GetString("UGST_Percentage");
                    invoiceMasterOutput.UGST = reader.GetString("UGST");

                    invoiceMasterOutput.UGST_RCM = reader.GetString("UGST_RCM");
                    invoiceMasterOutput.SGST_RCM = reader.GetString("SGST_RCM");
                    invoiceMasterOutput.Invoice_Number = reader.GetString("Invoice_Number");
                    invoiceMasterOutput.FG_Product_Code = reader.GetString("FG_Product_Code");
                    invoiceMasterOutput.FG_Product_Name = reader.GetString("FG_Product_Name");
                    invoiceMasterOutput.Ship_To_Party_MobileNumber = reader.GetString("Ship_To_Party_MobileNumber");
                    invoiceMasterOutput.Ship_To_Party_Region = reader.GetString("Ship_To_Party_Region");
                    invoiceMasterOutput.Ship_To_Party_Region_Desc = reader.GetString("Ship_To_Party_Region_Desc");

                    listInvoiceMasterOutputs.Add(invoiceMasterOutput);
                }

                reader.Close();
                return Ok(listInvoiceMasterOutputs);
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
                string updateSql = @"UPDATE invoice_master_table im
                               JOIN invoice_generation_table ig ON im.Document_Number = ig.DocumentNumber
                                SET im.Invoice_Number = ig.InvoiceNumber";
                MySqlCommand updateCommand = new MySqlCommand(updateSql, _connection);

                // Execute the update command
                updateCommand.ExecuteNonQuery();
                string fetchSql = "SELECT * FROM invoice_master_table WHERE Document_Date >= @startDate AND Document_Date <= @endDate AND Region_Code = @regionCode";
                MySqlCommand command = new MySqlCommand(fetchSql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@regionCode", $"{region}");

                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceMasterOutput> listInvoiceMasterOutputs = new List<InvoiceMasterOutput>();

                while (reader.Read())
                {
                    InvoiceMasterOutput invoiceMasterOutput = new InvoiceMasterOutput();

                    invoiceMasterOutput.Segment = reader.GetString("Segment");
                    invoiceMasterOutput.Region_Code = reader.GetString("Region_Code");
                    invoiceMasterOutput.Plant_Code = reader.GetString("Plant_Code");
                    invoiceMasterOutput.Financial_Year = reader.GetString("Financial_Year");
                    invoiceMasterOutput.GSTIN = reader.GetString("GSTIN");
                    invoiceMasterOutput.Document_Number = reader.GetString("Document_Number");
                    invoiceMasterOutput.Document_Date = reader.GetString("Document_Date");
                    invoiceMasterOutput.Document_Posting_Date = reader.GetString("Document_Posting_Date");
                    invoiceMasterOutput.Sales_Doc_Number = reader.GetString("Sales_Doc_Number");
                    invoiceMasterOutput.Ship_To_Party_Number = reader.GetString("Ship_To_Party_Number");
                    invoiceMasterOutput.Ship_To_Party_Name = reader.GetString("Ship_To_Party_Name");
                    invoiceMasterOutput.Pin = reader.GetString("Pin");
                    invoiceMasterOutput.City = reader.GetString("City");
                    invoiceMasterOutput.Address_1 = reader.GetString("Address_1");
                    invoiceMasterOutput.Address_2 = reader.GetString("Address_2");
                    invoiceMasterOutput.Address_3 = reader.GetString("Address_3");
                    invoiceMasterOutput.Vendor_Code = reader.GetString("Vendor_Code");
                    invoiceMasterOutput.Vendor_Name = reader.GetString("Vendor_Name");
                    invoiceMasterOutput.Spu_Number = reader.GetString("Spu_Number");
                    invoiceMasterOutput.CRM_Ticket_Number = reader.GetString("CRM_Ticket_Number");
                    invoiceMasterOutput.Machine_Status = reader.GetString("Machine_Status");
                    invoiceMasterOutput.COGS = reader.GetString("COGS");
                    invoiceMasterOutput.Material_Code = reader.GetString("Material_Code");
                    invoiceMasterOutput.Material_Group = reader.GetString("Material_Group");
                    invoiceMasterOutput.Material_Description = reader.GetString("Material_Description");

                    invoiceMasterOutput.HSN = reader.GetString("HSN");
                    invoiceMasterOutput.Tax_Percentage = reader.GetString("Tax_Percentage");

                    invoiceMasterOutput.Quantity = reader.GetString("Quantity");
                    invoiceMasterOutput.UOM = reader.GetString("UOM");

                    invoiceMasterOutput.Spare_Value = reader.GetString("Spare_Value");
                    invoiceMasterOutput.Currency = reader.GetString("Currency");
                    invoiceMasterOutput.Assignment_Date = reader.GetString("Assignment_Date");
                    invoiceMasterOutput.Tax_Code = reader.GetString("Tax_Code");
                    invoiceMasterOutput.CGST_Percentage = reader.GetString("CGST_Percentage");
                    invoiceMasterOutput.CGST_RCM = reader.GetString("CGST_RCM");
                    invoiceMasterOutput.CGST = reader.GetString("CGST");
                    invoiceMasterOutput.IGST_Percentage = reader.GetString("IGST_Percentage");
                    invoiceMasterOutput.IGST = reader.GetString("IGST");
                    invoiceMasterOutput.Import_IGST = reader.GetString("Import_IGST");

                    invoiceMasterOutput.IGST_RCM = reader.GetString("IGST_RCM");
                    invoiceMasterOutput.SGST_Percentage = reader.GetString("SGST_Percentage");
                    invoiceMasterOutput.SGST = reader.GetString("SGST");
                    invoiceMasterOutput.UGST_Percentage = reader.GetString("UGST_Percentage");
                    invoiceMasterOutput.UGST = reader.GetString("UGST");


                    invoiceMasterOutput.UGST_RCM = reader.GetString("UGST_RCM");
                    invoiceMasterOutput.SGST_RCM = reader.GetString("SGST_RCM");
                    invoiceMasterOutput.Invoice_Number = reader.GetString("Invoice_Number");
                    invoiceMasterOutput.FG_Product_Code = reader.GetString("FG_Product_Code");
                    invoiceMasterOutput.FG_Product_Name = reader.GetString("FG_Product_Name");
                    invoiceMasterOutput.Ship_To_Party_MobileNumber = reader.GetString("Ship_To_Party_MobileNumber");
                    invoiceMasterOutput.Ship_To_Party_Region = reader.GetString("Ship_To_Party_Region");
                    invoiceMasterOutput.Ship_To_Party_Region_Desc = reader.GetString("Ship_To_Party_Region_Desc");




                    listInvoiceMasterOutputs.Add(invoiceMasterOutput);
                }

                reader.Close();
                return Ok(listInvoiceMasterOutputs);
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
                string updateSql = @"UPDATE invoice_master_table im
                               JOIN invoice_generation_table ig ON im.Document_Number = ig.DocumentNumber
                                SET im.Invoice_Number = ig.InvoiceNumber";
                MySqlCommand updateCommand = new MySqlCommand(updateSql, _connection);

                // Execute the update command
                updateCommand.ExecuteNonQuery();
                string fetchSql = "SELECT * FROM invoice_master_table WHERE Document_Date >= @startDate AND Document_Date <= @endDate AND Segment = @segmentCode";
                MySqlCommand command = new MySqlCommand(fetchSql, _connection);
                command.Parameters.AddWithValue("@startDate", $"{startdate}");
                command.Parameters.AddWithValue("@endDate", $"{enddate}");
                command.Parameters.AddWithValue("@segmentCode", $"{segment}");

                MySqlDataReader reader = command.ExecuteReader();

                List<InvoiceMasterOutput> listInvoiceMasterOutputs = new List<InvoiceMasterOutput>();

                while (reader.Read())

                {

                    InvoiceMasterOutput invoiceMasterOutput = new InvoiceMasterOutput();  

                    invoiceMasterOutput.Segment = reader.GetString("Segment");
                    invoiceMasterOutput.Region_Code = reader.GetString("Region_Code");
                    invoiceMasterOutput.Plant_Code = reader.GetString("Plant_Code");
                    invoiceMasterOutput.Financial_Year = reader.GetString("Financial_Year");
                    invoiceMasterOutput.GSTIN = reader.GetString("GSTIN");
                    invoiceMasterOutput.Document_Number = reader.GetString("Document_Number");
                    invoiceMasterOutput.Document_Date = reader.GetString("Document_Date");
                    invoiceMasterOutput.Document_Posting_Date = reader.GetString("Document_Posting_Date");
                    invoiceMasterOutput.Sales_Doc_Number = reader.GetString("Sales_Doc_Number");
                    invoiceMasterOutput.Ship_To_Party_Number = reader.GetString("Ship_To_Party_Number");
                    invoiceMasterOutput.Ship_To_Party_Name = reader.GetString("Ship_To_Party_Name");
                    invoiceMasterOutput.Pin = reader.GetString("Pin");
                    invoiceMasterOutput.City = reader.GetString("City");
                    invoiceMasterOutput.Address_1 = reader.GetString("Address_1");
                    invoiceMasterOutput.Address_2 = reader.GetString("Address_2");
                    invoiceMasterOutput.Address_3 = reader.GetString("Address_3");
                    invoiceMasterOutput.Vendor_Code = reader.GetString("Vendor_Code");
                    invoiceMasterOutput.Vendor_Name = reader.GetString("Vendor_Name");
                    invoiceMasterOutput.Spu_Number = reader.GetString("Spu_Number");
                    invoiceMasterOutput.CRM_Ticket_Number = reader.GetString("CRM_Ticket_Number");
                    invoiceMasterOutput.Machine_Status = reader.GetString("Machine_Status");
                    invoiceMasterOutput.COGS = reader.GetString("COGS");
                    invoiceMasterOutput.Material_Code = reader.GetString("Material_Code");
                    invoiceMasterOutput.Material_Group = reader.GetString("Material_Group");
                    invoiceMasterOutput.Material_Description = reader.GetString("Material_Description");

                    invoiceMasterOutput.HSN = reader.GetString("HSN");
                    invoiceMasterOutput.Tax_Percentage = reader.GetString("Tax_Percentage");

                    invoiceMasterOutput.Quantity = reader.GetString("Quantity");
                    invoiceMasterOutput.UOM = reader.GetString("UOM");

                    invoiceMasterOutput.Spare_Value = reader.GetString("Spare_Value");
                    invoiceMasterOutput.Currency = reader.GetString("Currency");
                    invoiceMasterOutput.Assignment_Date = reader.GetString("Assignment_Date");
                    invoiceMasterOutput.Tax_Code = reader.GetString("Tax_Code");
                    invoiceMasterOutput.CGST_Percentage = reader.GetString("CGST_Percentage");
                    invoiceMasterOutput.CGST_RCM = reader.GetString("CGST_RCM");
                    invoiceMasterOutput.CGST = reader.GetString("CGST");
                    invoiceMasterOutput.IGST_Percentage = reader.GetString("IGST_Percentage");
                    invoiceMasterOutput.IGST = reader.GetString("IGST");
                    invoiceMasterOutput.Import_IGST = reader.GetString("Import_IGST");

                    invoiceMasterOutput.IGST_RCM = reader.GetString("IGST_RCM");
                    invoiceMasterOutput.SGST_Percentage = reader.GetString("SGST_Percentage");
                    invoiceMasterOutput.SGST = reader.GetString("SGST");
                    invoiceMasterOutput.UGST_Percentage = reader.GetString("UGST_Percentage");
                    invoiceMasterOutput.UGST = reader.GetString("UGST");


                    invoiceMasterOutput.UGST_RCM = reader.GetString("UGST_RCM");
                    invoiceMasterOutput.SGST_RCM = reader.GetString("SGST_RCM");
                    invoiceMasterOutput.Invoice_Number = reader.GetString("Invoice_Number");
                    invoiceMasterOutput.FG_Product_Code = reader.GetString("FG_Product_Code");
                    invoiceMasterOutput.FG_Product_Name = reader.GetString("FG_Product_Name");
                    invoiceMasterOutput.Ship_To_Party_MobileNumber = reader.GetString("Ship_To_Party_MobileNumber");
                    invoiceMasterOutput.Ship_To_Party_Region = reader.GetString("Ship_To_Party_Region");
                    invoiceMasterOutput.Ship_To_Party_Region_Desc = reader.GetString("Ship_To_Party_Region_Desc");


                    listInvoiceMasterOutputs.Add(invoiceMasterOutput);
                }

                reader.Close();
                return Ok(listInvoiceMasterOutputs);
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

