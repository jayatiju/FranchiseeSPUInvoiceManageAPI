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
using WebApplication1.InvoiceUpdateReference;
using System.Globalization;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class InvoiceGenerationDataMigrationController : ApiController
    {
        
        List<String> failureItems = new List<String>();
        List<DbToSapOutput> dbToSapOutputs = new List<DbToSapOutput>();
        public static int serialNumber;
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public String message = "";
        public InvoiceGenerationDataMigrationController()
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
        public IHttpActionResult PostDataMigration([FromBody] InvoiceGenerationDataMigrationInput invoiceGenerationDataMigrationInput)
        {
            try
            {
                string monthYear = convertToMonthYear(invoiceGenerationDataMigrationInput.startDate);

                List<InvoiceGenerationOutput> listOutput = new List<InvoiceGenerationOutput>();




                string sql = "SELECT MAX(Invoice_Number) AS InvoiceNumber, MAX(Document_Posting_Date) AS InvoiceDate, MAX(Spu_Number) AS SPUNumber, MAX(CRM_Ticket_Number) AS CRMTicketNumber, Document_Number AS DocumentNumber, MAX(Document_Date) AS DocumentDate, MAX(Vendor_Name) AS VendorName, MAX(Vendor_Code) AS VendorCode, MAX(Plant_Code) AS CustomerCode, MAX(Ship_To_Party_Number) AS ShipToPartyNumber, MAX(Ship_To_Party_Name) AS ShipToPartyName, round( SUM(Spare_Value),2) AS Sub_Total, round( SUM(SGST),2) AS Total_SGST, round( SUM(CGST),2) AS Total_CGST, round( SUM(IGST),2) AS Total_IGST,round( SUM(UGST),2) AS Total_UGST, MAX(branchname) AS branchname,  MAX(Region_Code) AS regionCode, MAX(invoice_master_table.Segment) AS segmentCode, MAX(invoice_master_table.GSTIN) as branchGSTIN, MAX(CONCAT(Address_1, Address_2, Address_3)) as ShipToPartyAddress FROM invoice_master_table LEFT OUTER JOIN customer_master_table ON invoice_master_table.Plant_Code =  customer_master_table.branchcode WHERE Document_Date >= @startDate AND Document_Date <= @endDate GROUP BY(Document_Number);";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", invoiceGenerationDataMigrationInput.startDate);
                command.Parameters.AddWithValue("@endDate", invoiceGenerationDataMigrationInput.endDate);
               // command.Parameters.AddWithValue("@segment", invoiceGenerationInput.segment);

                MySqlDataReader reader = command.ExecuteReader();



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
                    invoiceGenerationOutput.CustomerCode = reader.GetString("CustomerCode");
                    invoiceGenerationOutput.ShipToPartyNumber = reader.GetString("ShipToPartyNumber");
                    invoiceGenerationOutput.ShipToPartyName = reader.GetString("ShipToPartyName");
                    invoiceGenerationOutput.SubTotal = reader.GetString("Sub_Total");
                    invoiceGenerationOutput.SGST = reader.GetString("Total_SGST");
                    invoiceGenerationOutput.CGST = reader.GetString("Total_CGST");
                    invoiceGenerationOutput.IGST = reader.GetString("Total_IGST");
                    invoiceGenerationOutput.UGST = reader.GetString("Total_UGST");
                    invoiceGenerationOutput.CustomerName = reader.GetString("branchname");
                    invoiceGenerationOutput.regionCode = reader.GetString("regionCode");
                    invoiceGenerationOutput.segmentCode = reader.GetString("segmentCode");
                    invoiceGenerationOutput.branchGSTIN = reader.GetString("branchGSTIN");
                    invoiceGenerationOutput.shiptopartyaddress = reader.GetString("ShipToPartyAddress");

                    
                    double sum = Convert.ToDouble(invoiceGenerationOutput.SubTotal) +
                                 Convert.ToDouble(invoiceGenerationOutput.SGST) +
                                 Convert.ToDouble(invoiceGenerationOutput.CGST) +
                                 Convert.ToDouble(invoiceGenerationOutput.IGST) +
                                 Convert.ToDouble(invoiceGenerationOutput.UGST);

                    double roundedOffSum = Math.Round(sum);

                    double roundOff = Math.Round(sum - roundedOffSum, 2);

                    invoiceGenerationOutput.RoundOff = roundOff.ToString();
                    invoiceGenerationOutput.GrandTotal = roundedOffSum.ToString();
                    Console.Write(invoiceGenerationOutput);
                    listOutput.Add(invoiceGenerationOutput);
                }

                reader.Close();


                string insertSql = "insert into invoice_generation_table (InvoiceNumber, InvoiceDate, SPUNumber, CRMTicketNumber, DocumentNumber, DocumentDate, VendorName, VendorCode, CustomerName, CustomerCode, ShipToPartyNumber, ShipToPartyName, SubTotal, SGST, CGST, IGST,UGST, RoundOff, GrandTotal, InvoiceNumberStatus, InvoicePdfStatus, InvoicePdfDigitalSigStatus, InvoicePdfLocation, regionCode, segmentCode, InvoicePdfDuplicateStatus, InvoicePdfDuplicateLocation, InvoicePdfTriplicateStatus, InvoicePdfTriplicateLocation, branchgstin, shiptopartyaddress) " +
                       "values (@InvoiceNumber, @InvoiceDate, @SPUNumber, @CRMTicketNumber, @DocumentNumber, @DocumentDate, @VendorName, @VendorCode, @CustomerName, @CustomerCode, @ShipToPartyNumber, @ShipToPartyName, @SubTotal, @SGST, @CGST, @IGST,@UGST, @RoundOff, @GrandTotal, @InvoiceNumberStatus, @InvoicePdfStatus, @InvoicePdfDigitalSigStatus, @InvoicePdfLocation, @regionCode, @segmentCode, @InvoicePdfDuplicateStatus, @InvoicePdfDuplicateLocation, @InvoicePdfTriplicateStatus, @InvoicePdfTriplicateLocation, @branchgstin,@shiptopartyaddress)";

                using (var insertCommand = new MySqlCommand(insertSql, _connection))
                {
                    foreach (InvoiceGenerationOutput invoiceGenerationOutput in listOutput)
                    {
                        // Assuming you have an insertCommand object for your INSERT statement
                        insertCommand.Parameters.Clear();

                        insertCommand.Parameters.AddWithValue("@InvoiceNumber", invoiceGenerationOutput.InvoiceNumber);
                        insertCommand.Parameters.AddWithValue("@InvoiceDate", invoiceGenerationOutput.InvoiceDate);
                        insertCommand.Parameters.AddWithValue("@SPUNumber", invoiceGenerationOutput.SPUNumber);
                        insertCommand.Parameters.AddWithValue("@CRMTicketNumber", invoiceGenerationOutput.CRMTicketNumber);
                        insertCommand.Parameters.AddWithValue("@DocumentNumber", invoiceGenerationOutput.DocumentNumber);
                        insertCommand.Parameters.AddWithValue("@DocumentDate", invoiceGenerationOutput.DocumentDate);
                        insertCommand.Parameters.AddWithValue("@VendorName", invoiceGenerationOutput.VendorName);
                        insertCommand.Parameters.AddWithValue("@VendorCode", invoiceGenerationOutput.VendorCode);
                        insertCommand.Parameters.AddWithValue("@CustomerName", invoiceGenerationOutput.CustomerName);
                        insertCommand.Parameters.AddWithValue("@CustomerCode", invoiceGenerationOutput.CustomerCode);
                        insertCommand.Parameters.AddWithValue("@ShipToPartyNumber", invoiceGenerationOutput.ShipToPartyNumber);
                        insertCommand.Parameters.AddWithValue("@ShipToPartyName", invoiceGenerationOutput.ShipToPartyName);
                        insertCommand.Parameters.AddWithValue("@SubTotal", invoiceGenerationOutput.SubTotal);
                        insertCommand.Parameters.AddWithValue("@SGST", invoiceGenerationOutput.SGST);
                        insertCommand.Parameters.AddWithValue("@CGST", invoiceGenerationOutput.CGST);
                        insertCommand.Parameters.AddWithValue("@IGST", invoiceGenerationOutput.IGST);
                        insertCommand.Parameters.AddWithValue("@UGST", invoiceGenerationOutput.UGST);
                        insertCommand.Parameters.AddWithValue("@RoundOff", invoiceGenerationOutput.RoundOff);
                        insertCommand.Parameters.AddWithValue("@GrandTotal", invoiceGenerationOutput.GrandTotal);
                        insertCommand.Parameters.AddWithValue("@InvoiceNumberStatus", 'X');
                        insertCommand.Parameters.AddWithValue("@InvoicePdfStatus", "");
                        insertCommand.Parameters.AddWithValue("InvoicePdfDigitalSigStatus", "");
                        insertCommand.Parameters.AddWithValue("@InvoicePdfLocation", "");
                        insertCommand.Parameters.AddWithValue("@regionCode", invoiceGenerationOutput.regionCode);
                        insertCommand.Parameters.AddWithValue("@segmentCode", invoiceGenerationOutput.segmentCode);

                        insertCommand.Parameters.AddWithValue("@InvoicePdfDuplicateStatus", "");
                        insertCommand.Parameters.AddWithValue("@InvoicePdfDuplicateLocation", "");

                        insertCommand.Parameters.AddWithValue("@InvoicePdfTriplicateStatus", "");
                        insertCommand.Parameters.AddWithValue("@InvoicePdfTriplicateLocation", "");

                        insertCommand.Parameters.AddWithValue("@branchgstin", invoiceGenerationOutput.branchGSTIN);
                        insertCommand.Parameters.AddWithValue("@shiptopartyaddress", invoiceGenerationOutput.shiptopartyaddress);


                        insertCommand.ExecuteNonQuery();
                    }
                }



                /*
                        string statusSqlLast = "UPDATE invoice_monthly_status SET Invoice_Number_Generation_Flag = 'X' WHERE Month_Year = @Month_Year AND Segment = @Segment;";

                using (var statuscommand = new MySqlCommand(statusSqlLast, _connection))
                {
                    statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);
                    statuscommand.Parameters.AddWithValue("@Segment", invoiceGenerationInput.segment);
                    //statuscommand.Parameters.AddWithValue("@Region", invoiceGenerationInput.);

                    statuscommand.ExecuteNonQuery();


                }

                */

                

                return Ok(responseCode);
            }
            catch(Exception e)
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

        public static string convertToYear(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", CultureInfo.InvariantCulture);
               // DateTime date = DateTime.Parse(inputDate);


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