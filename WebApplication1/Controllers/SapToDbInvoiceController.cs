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
using System.Globalization;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class SapToDbInvoiceController : ApiController
    {
        
        private readonly MySqlConnection _connection;
        ResponseCode responseCode = new ResponseCode();
        public SapToDbInvoiceController()
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
        public ResponseCode Post([FromBody] InvoiceInput invoiceInput)
        {

            try
            {
                string prevMonthYear = ConvertToPreviousMonthYear(invoiceInput.StartDate);
                string sql1 = "SELECT Data_Sync_Flag, Invoice_Number_Generation_Flag, Invoice_PDF_Generation_Flag FROM invoice_monthly_status WHERE Month_Year = @monthYear AND Segment = @segment";

                string dataSyncFlag = "";
                string invoiceNumberFlag = "";
                string invoicePdfFlag = "";
                using (var command1 = new MySqlCommand(sql1, _connection))
                {
                    command1.Parameters.AddWithValue("@monthYear", $"{prevMonthYear}");
                    command1.Parameters.AddWithValue("@segment", $"{ invoiceInput.SegmentCode}");

                    using (var reader = command1.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dataSyncFlag = reader.GetString("Data_Sync_Flag");
                            invoiceNumberFlag = reader.GetString("Invoice_Number_Generation_Flag");
                            invoicePdfFlag = reader.GetString("Invoice_PDF_Generation_Flag");
                        }
                        else
                        {
                            responseCode.messageCode = "00";
                            responseCode.messageString = "Last Month Invoice is not synced yet!";// No entry found in the table
                        }
                    }
                }

                if (dataSyncFlag != "" || invoiceNumberFlag != "" || invoicePdfFlag != "")
                {

                    string statusSql = "INSERT INTO invoice_monthly_status (Month_Year, Region, Data_Sync_Flag, Invoice_Number_Generation_Flag, Invoice_PDF_Generation_Flag, Segment) VALUES(@Month_Year, @Region, 'IP', '', '', @Segment)";
                    string monthYear = convertToMonthYear(invoiceInput.StartDate);
                    using (var statuscommand = new MySqlCommand(statusSql, _connection))
                    {
                        statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);
                        statuscommand.Parameters.AddWithValue("@Segment", invoiceInput.SegmentCode);
                        statuscommand.Parameters.AddWithValue("@Region", invoiceInput.Region);


                        statuscommand.ExecuteNonQuery();


                    }

                    String FyNow = convertToYear(invoiceInput.StartDate);

                    //data sync
                    using (ZWS_SPU_PUR_SRVClient client = new ZWS_SPU_PUR_SRVClient("invoices_soap12"))
                    {
                        try
                        {

                            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                            client.ClientCredentials.UserName.UserName = "RFCUSER";
                            client.ClientCredentials.UserName.Password = "Init#1234";

                        //       client.ClientCredentials.UserName.UserName = "BIRAJ";
                        //       client.ClientCredentials.UserName.Password = "Ifb-123";


                            // DateTime now = DateTime.Now;
                            



                            var requestObject = new ZfmSpuPurRequest
                            {

                                ZfmSpuPur = new ZfmSpuPur()
                                {

                                    CompanyCode = invoiceInput.CompanyCode,
                                    DocumentNumber = invoiceInput.DocumentNumber,
                                    EndDate = invoiceInput.EndDate,
                                   // FiscalYear = invoiceInput.FiscalYear,
                                    FiscalYear = FyNow,
                                   // FiscalYear = convertToYear(invoiceInput.StartDate),
                                    SegmentCode = invoiceInput.SegmentCode,
                                    StartDate = invoiceInput.StartDate

                                }
                            };

                            ZfmSpuPurResponse response = client.ZfmSpuPur(requestObject.ZfmSpuPur);

                            // Check if response is null or has no data
                            if (response == null || response.EtSpu == null || !response.EtSpu.Any())
                            {
                                responseCode.messageCode = "E";
                                responseCode.messageString = "No data to sync for invoice";

                                string statusDelSql = "DELETE FROM invoice_monthly_status WHERE Month_Year = @Month_Year AND Segment = @Segment AND Region = @Region";

                                using (var statusDelcommand = new MySqlCommand(statusDelSql, _connection))
                                {
                                    statusDelcommand.Parameters.AddWithValue("@Month_Year", monthYear);
                                    statusDelcommand.Parameters.AddWithValue("@Segment", invoiceInput.SegmentCode);
                                    statusDelcommand.Parameters.AddWithValue("@Region", invoiceInput.Region);


                                    statusDelcommand.ExecuteNonQuery();


                                }

                                return responseCode;
                            }
                            else
                            {
                                foreach (var sapInvoice in response.EtSpu)
                                {


                                    string sql = "INSERT INTO invoice_master_table (Segment, Region_Code, Plant_Code, Financial_Year, GSTIN, Document_Number, Document_Date, Document_Posting_Date, Sales_Doc_Number, Ship_To_Party_Number, Ship_To_Party_Name, Pin, City, Address_1, Address_2, Address_3, Vendor_Code, Vendor_Name, Spu_Number, CRM_Ticket_Number, Machine_Status, COGS, Material_Code, Material_Group, Material_Description, HSN, Tax_Percentage, Quantity, UOM, Spare_Value, Currency, Assignment_Date, Tax_Code, CGST_Percentage, CGST_RCM, CGST, IGST_Percentage, IGST, Import_IGST, IGST_RCM, SGST_Percentage, SGST, UGST_Percentage, UGST, UGST_RCM, SGST_RCM, Invoice_Number, FG_Product_Code, FG_Product_Name, Ship_To_Party_MobileNumber, Ship_To_Party_Region, Ship_To_Party_Region_Desc) " +
                                        "VALUES (@Segment, @Region_Code, @Plant_Code, @Financial_Year, @GSTIN, @Document_Number, @Document_Date, @Document_Posting_Date, @Sales_Doc_Number, @Ship_To_Party_Number, @Ship_To_Party_Name, @Pin, @City, @Address_1, @Address_2, @Address_3, @Vendor_Code, @Vendor_Name, @Spu_Number, @CRM_Ticket_Number, @Machine_Status, @COGS, @Material_Code, @Material_Group, @Material_Description, @HSN, @Tax_Percentage, @Quantity, @UOM, @Spare_Value, @Currency, @Assignment_Date, @Tax_Code, @CGST_Percentage, @CGST_RCM, @CGST, @IGST_Percentage, @IGST, @Import_IGST, @IGST_RCM, @SGST_Percentage, @SGST, @UGST_Percentage, @UGST, @UGST_RCM, @SGST_RCM, @Invoice_Number, @FG_Product_Code, @FG_Product_Name, @Ship_To_Party_MobileNumber, @Ship_To_Party_Region, @Ship_To_Party_Region_Desc);";

                                    using (var command = new MySqlCommand(sql, _connection))
                                    {
                                        // Assuming you have a SqlCommand object named 'command' and a 'sapInvoice' object containing the values
                                        String segment = sapInvoice.Segment;
                                        segment = segment.TrimStart('0');
                                        command.Parameters.AddWithValue("@Segment", segment);
                                        command.Parameters.AddWithValue("@Region_Code", sapInvoice.Region);
                                        command.Parameters.AddWithValue("@Plant_Code", sapInvoice.Plant);
                                        command.Parameters.AddWithValue("@Financial_Year", sapInvoice.Fy);
                                        command.Parameters.AddWithValue("@GSTIN", sapInvoice.Gstin);
                                        command.Parameters.AddWithValue("@Document_Number", sapInvoice.Document);
                                        command.Parameters.AddWithValue("@Document_Date", sapInvoice.DocDate);
                                        command.Parameters.AddWithValue("@Document_Posting_Date", sapInvoice.PostDate);
                                        command.Parameters.AddWithValue("@Sales_Doc_Number", sapInvoice.SalesDoc);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Number", sapInvoice.ShipToParty);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Name", sapInvoice.ShipToPartyName);
                                        command.Parameters.AddWithValue("@Pin", sapInvoice.Pincode);
                                        command.Parameters.AddWithValue("@City", sapInvoice.City);
                                        command.Parameters.AddWithValue("@Address_1", sapInvoice.Street2);
                                        command.Parameters.AddWithValue("@Address_2", sapInvoice.Street);
                                        command.Parameters.AddWithValue("@Address_3", sapInvoice.Street3);
                                        command.Parameters.AddWithValue("@Vendor_Code", sapInvoice.Vendor);
                                        command.Parameters.AddWithValue("@Vendor_Name", sapInvoice.VendorName);
                                        command.Parameters.AddWithValue("@Spu_Number", sapInvoice.SpuNo);
                                        command.Parameters.AddWithValue("@CRM_Ticket_Number", sapInvoice.CrmTicket);
                                        command.Parameters.AddWithValue("@Machine_Status", sapInvoice.MachStat);
                                        command.Parameters.AddWithValue("@COGS", sapInvoice.Cogs);
                                        command.Parameters.AddWithValue("@Material_Code", sapInvoice.Material);
                                        command.Parameters.AddWithValue("@Material_Group", sapInvoice.MaterialGrp);
                                        command.Parameters.AddWithValue("@Material_Description", sapInvoice.MatDes);
                                        command.Parameters.AddWithValue("@HSN", sapInvoice.Hsn);
                                        command.Parameters.AddWithValue("@Tax_Percentage", sapInvoice.Taxpercent);
                                        command.Parameters.AddWithValue("@Quantity", sapInvoice.Quantity);
                                        command.Parameters.AddWithValue("@UOM", sapInvoice.Unit);
                                        command.Parameters.AddWithValue("@Spare_Value", sapInvoice.Spare);
                                        command.Parameters.AddWithValue("@Currency", sapInvoice.Currency);
                                        command.Parameters.AddWithValue("@Assignment_Date", sapInvoice.Assignment);
                                        command.Parameters.AddWithValue("@Tax_Code", sapInvoice.TaxCode);
                                        command.Parameters.AddWithValue("@CGST_Percentage", sapInvoice.CgstPer);
                                        command.Parameters.AddWithValue("@CGST_RCM", sapInvoice.CgstRcm);
                                        command.Parameters.AddWithValue("@CGST", sapInvoice.Cgst);
                                        command.Parameters.AddWithValue("@IGST_Percentage", sapInvoice.IgstPer);
                                        command.Parameters.AddWithValue("@IGST", sapInvoice.Igst);
                                        command.Parameters.AddWithValue("@Import_IGST", sapInvoice.ImportIgst);
                                        command.Parameters.AddWithValue("@IGST_RCM", sapInvoice.IgstRcm);
                                        command.Parameters.AddWithValue("@SGST_Percentage", sapInvoice.SgstPer);
                                        command.Parameters.AddWithValue("@SGST", sapInvoice.Sgst);
                                        command.Parameters.AddWithValue("@UGST_Percentage", sapInvoice.UgstPer);
                                        command.Parameters.AddWithValue("@UGST", sapInvoice.Ugst);
                                        command.Parameters.AddWithValue("@UGST_RCM", sapInvoice.UgstRcm);
                                        command.Parameters.AddWithValue("@SGST_RCM", sapInvoice.SgstRcm);
                                        command.Parameters.AddWithValue("@Invoice_Number", sapInvoice.Invoice);
                                        command.Parameters.AddWithValue("@FG_Product_Code", sapInvoice.ZzproductId);
                                        command.Parameters.AddWithValue("@FG_Product_Name", sapInvoice.ZzproductDesc);
                                        command.Parameters.AddWithValue("@Ship_To_Party_MobileNumber", sapInvoice.Mobileno);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Region", sapInvoice.StpartyRegion);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Region_Desc", sapInvoice.StpartyRegiondesc);

                                        if (!string.IsNullOrWhiteSpace(sapInvoice.Material))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        //command.ExecuteNonQuery();


                                    }

                                }
                            }
                            responseCode.messageCode = "S";
                            responseCode.messageString = "Data successfully inserted from SAP to Database for invoice";
                        }
                        catch (Exception ex)
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = ex.Message;
                        }
                    }



                    string statusSqlLast = "UPDATE invoice_monthly_status SET Data_Sync_Flag = 'X' WHERE Month_Year = @Month_Year AND Segment = @Segment AND Region = @Region;";


                    using (var statuscommand = new MySqlCommand(statusSqlLast, _connection))
                    {
                        statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);
                        statuscommand.Parameters.AddWithValue("@Segment", invoiceInput.SegmentCode);
                        statuscommand.Parameters.AddWithValue("@Region", invoiceInput.Region);

                        statuscommand.ExecuteNonQuery();
                    }

                }

            }
            catch (Exception ex)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = ex.Message;
            }
            finally
            {
                _connection.Close();
            }

            return responseCode;
        }



        public static string convertToMonthYear(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                DateTime date = DateTime.ParseExact(inputDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

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

        public static string ConvertToPreviousMonthYear(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                DateTime date = DateTime.ParseExact(inputDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);


                // Calculate the previous month-year
                DateTime previousMonth = date.AddMonths(-1);

                // Format the previous month-year as yyyyMM (month-year) string
                string result = previousMonth.ToString("yyyyMM");

                return result;
            }
            catch (FormatException ex)
            {
                // Handle invalid input date format
                return "Invalid Date Format: " + ex.Message;
            }
        }

        public static string convertToYear(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                //DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime date = DateTime.Parse(inputDate);


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