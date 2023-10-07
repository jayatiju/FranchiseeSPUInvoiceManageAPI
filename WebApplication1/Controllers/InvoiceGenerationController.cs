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


namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class InvoiceGenerationController : ApiController
    {
        
        List<String> failureItems = new List<String>();
        List<DbToSapOutput> dbToSapOutputs = new List<DbToSapOutput>();
        public static int serialNumber;
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public String message = "";
        public InvoiceGenerationController()
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
                string statusSql = "UPDATE invoice_monthly_status SET Invoice_Number_Generation_Flag = 'IP', Segment = @Segment  WHERE Month_Year = @Month_Year;";
                string monthYear = convertToMonthYear(invoiceGenerationInput.startDate);
                using (var statuscommand = new MySqlCommand(statusSql, _connection))
                {
                    statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);
                    statuscommand.Parameters.AddWithValue("@Segment", invoiceGenerationInput.segment);


                    statuscommand.ExecuteNonQuery();


                }


                string getCounter = "select * from counter";
                MySqlCommand sqlCommand = new MySqlCommand(getCounter, _connection);
                MySqlDataReader counterReader = sqlCommand.ExecuteReader();
                counterReader.Read();
                serialNumber = counterReader.GetInt32("serialCounter");
                counterReader.Close();

                List<InvoiceGenerationOutput> listOutput = new List<InvoiceGenerationOutput>();




                string sql = "SELECT MAX(Document_Posting_Date) AS InvoiceDate, MAX(Spu_Number) AS SPUNumber, MAX(CRM_Ticket_Number) AS CRMTicketNumber, Document_Number AS DocumentNumber, MAX(Document_Date) AS DocumentDate, MAX(Vendor_Name) AS VendorName, MAX(Vendor_Code) AS VendorCode, MAX(Plant_Code) AS CustomerCode, MAX(Ship_To_Party_Number) AS ShipToPartyNumber, MAX(Ship_To_Party_Name) AS ShipToPartyName, SUM(Spare_Value) AS Sub_Total, SUM(SGST) AS Total_SGST, SUM(CGST) AS Total_CGST, SUM(IGST) AS Total_IGST, MAX(branchname) AS branchname,  MAX(Region_Code) AS regionCode, MAX(invoice_master_table.Segment) AS segmentCode FROM invoice_master_table LEFT OUTER JOIN customer_master_table ON invoice_master_table.Plant_Code =  customer_master_table.branchcode WHERE Document_Date >= @startDate AND Document_Date <= @endDate AND invoice_master_table.Segment = @segment GROUP BY(Document_Number);";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@startDate", invoiceGenerationInput.startDate);
                command.Parameters.AddWithValue("@endDate", invoiceGenerationInput.endDate);
                command.Parameters.AddWithValue("@segment", invoiceGenerationInput.segment);

                MySqlDataReader reader = command.ExecuteReader();



                while (reader.Read())
                {
                    InvoiceGenerationOutput invoiceGenerationOutput = new InvoiceGenerationOutput();

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
                    invoiceGenerationOutput.CustomerName = reader.GetString("branchname");
                    invoiceGenerationOutput.regionCode = reader.GetString("regionCode");
                    invoiceGenerationOutput.segmentCode = reader.GetString("segmentCode");

                    invoiceGenerationOutput.InvoiceNumber = invoiceNumberGeneration(invoiceGenerationOutput.VendorCode);

                    double sum = Convert.ToDouble(invoiceGenerationOutput.SubTotal) +
                                 Convert.ToDouble(invoiceGenerationOutput.SGST) +
                                 Convert.ToDouble(invoiceGenerationOutput.CGST) +
                                 Convert.ToDouble(invoiceGenerationOutput.IGST);

                    double roundedOffSum = Math.Round(sum);

                    double roundOff = Math.Round(sum - roundedOffSum, 2);

                    invoiceGenerationOutput.RoundOff = roundOff.ToString();
                    invoiceGenerationOutput.GrandTotal = roundedOffSum.ToString();

                    listOutput.Add(invoiceGenerationOutput);
                }

                reader.Close();


                string insertSql = "insert into invoice_generation_table (InvoiceNumber, InvoiceDate, SPUNumber, CRMTicketNumber, DocumentNumber, DocumentDate, VendorName, VendorCode, CustomerName, CustomerCode, ShipToPartyNumber, ShipToPartyName, SubTotal, SGST, CGST, IGST, RoundOff, GrandTotal, InvoiceNumberStatus, InvoicePdfStatus, InvoicePdfDigitalSigStatus, InvoicePdfLocation, regionCode, segmentCode) " +
                       "values (@InvoiceNumber, @InvoiceDate, @SPUNumber, @CRMTicketNumber, @DocumentNumber, @DocumentDate, @VendorName, @VendorCode, @CustomerName, @CustomerCode, @ShipToPartyNumber, @ShipToPartyName, @SubTotal, @SGST, @CGST, @IGST, @RoundOff, @GrandTotal, @InvoiceNumberStatus, @InvoicePdfStatus, @InvoicePdfDigitalSigStatus, @InvoicePdfLocation, @regionCode, @segmentCode)";

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
                        insertCommand.Parameters.AddWithValue("@RoundOff", invoiceGenerationOutput.RoundOff);
                        insertCommand.Parameters.AddWithValue("@GrandTotal", invoiceGenerationOutput.GrandTotal);
                        insertCommand.Parameters.AddWithValue("@InvoiceNumberStatus", 'X');
                        insertCommand.Parameters.AddWithValue("@InvoicePdfStatus", "");
                        insertCommand.Parameters.AddWithValue("InvoicePdfDigitalSigStatus", "");
                        insertCommand.Parameters.AddWithValue("@InvoicePdfLocation", "");
                        insertCommand.Parameters.AddWithValue("@regionCode", invoiceGenerationOutput.regionCode);
                        insertCommand.Parameters.AddWithValue("@segmentCode", invoiceGenerationOutput.segmentCode);

                        insertCommand.ExecuteNonQuery();
                    }
                }


                string counterSql = "update counter set serialCounter = @serialCounter";
                var counterCommand = new MySqlCommand(counterSql, _connection);
                counterCommand.Parameters.AddWithValue("@serialCounter", serialNumber);
                counterCommand.ExecuteNonQuery();



                string statusSqlLast = "UPDATE invoice_monthly_status SET Invoice_Number_Generation_Flag = 'X' WHERE Month_Year = @Month_Year;";

                using (var statuscommand = new MySqlCommand(statusSqlLast, _connection))
                {
                    statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);


                    statuscommand.ExecuteNonQuery();


                }

                
                //getting data from table to send to sap and storing it in a list
                string sqlDbToSap = "SELECT * FROM invoice_generation_table WHERE InvoiceDate BETWEEN @startD AND @endD AND segmentCode = @segment;";
                MySqlCommand commandDbToSap = new MySqlCommand(sqlDbToSap, _connection);
                commandDbToSap.Parameters.AddWithValue("@startD", invoiceGenerationInput.startDate);
                commandDbToSap.Parameters.AddWithValue("@endD", invoiceGenerationInput.endDate);
                commandDbToSap.Parameters.AddWithValue("@segment", invoiceGenerationInput.segment);

                MySqlDataReader readerDbToSap = commandDbToSap.ExecuteReader();
                List<DbToSapInput> dbToSapInputs = new List<DbToSapInput>();

                while (readerDbToSap.Read())
                {
                    DbToSapInput dbToSapInput = new DbToSapInput();

                    dbToSapInput.document = readerDbToSap.GetString("DocumentNumber");
                    dbToSapInput.invoice = readerDbToSap.GetString("InvoiceNumber");
                    dbToSapInput.fy = convertToYear(invoiceGenerationInput.startDate);

                    dbToSapInputs.Add(dbToSapInput);
                }

                readerDbToSap.Close();

                using (ZWS_SPU_INVOICE_POST_SRVClient client = new ZWS_SPU_INVOICE_POST_SRVClient("postlist_soap12"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    client.ClientCredentials.UserName.UserName = "BIRAJ";
                    client.ClientCredentials.UserName.Password = "Ifb-12345";


                    ZSPU_INVOICE_UPD_STR[] invoiceArray = new ZSPU_INVOICE_UPD_STR[dbToSapInputs.Count];
                    for (int i = 0; i < dbToSapInputs.Count; i++)
                    {
                        invoiceArray[i] = new ZSPU_INVOICE_UPD_STR
                        {
                            DOCUMENT = dbToSapInputs[i].document,
                            INVOICE = dbToSapInputs[i].invoice,
                            FY = dbToSapInputs[i].fy
                        };
                    }


                    var request = new ZFM_SPU_INVOICE_POSTRequest
                    {
                        ZFM_SPU_INVOICE_POST = new ZFM_SPU_INVOICE_POST()
                        {
                            IT_INVOICE = invoiceArray
                            
                        }
                    };

                    
                    
                    ZFM_SPU_INVOICE_POSTResponse response = client.ZFM_SPU_INVOICE_POST(request.ZFM_SPU_INVOICE_POST);
                    
                    ZFM_SPU_INVOICE_POSTResponse1 response1 = new ZFM_SPU_INVOICE_POSTResponse1
                    {
                        ZFM_SPU_INVOICE_POSTResponse = response
                    };
                    
                    if (response1 != null && response1.ZFM_SPU_INVOICE_POSTResponse != null)
                    {
                        // Handle the response data
                        //var responseData = response1.ZFM_SPU_INVOICE_POSTResponse;

                        // Print response details
                        foreach (var item in response.ET_RESPONSE)
                        {
                            DbToSapOutput dbToSapOutput = new DbToSapOutput();
                            dbToSapOutput.document = item.DOCUMENT;
                            dbToSapOutput.message = item.MESSAGE;

                            dbToSapOutputs.Add(dbToSapOutput);
                        }

                        foreach(var outputItem in dbToSapOutputs)
                        {
                            if(outputItem.message != "Success")
                            {
                                //responseCode.messageCode = "E";
                                //responseCode.messageString = "SAP syncing did not occur for some items";
                                //return Ok(responseCode);
                                failureItems.Add(outputItem.document);
                                string errorSql = "INSERT INTO invoice_update_error_sap (document, message) VALUES(@document, @message)";
                                using (var errorcommand = new MySqlCommand(errorSql, _connection))
                                {
                                    errorcommand.Parameters.AddWithValue("@document", outputItem.document);
                                    errorcommand.Parameters.AddWithValue("@message", outputItem.message);
                                    

                                    errorcommand.ExecuteNonQuery();
                                }
                            }

                        }

                        if(failureItems.Count == 0)
                        {
                            responseCode.messageCode = "S";
                            responseCode.messageString = "All invoice data synced with SAP";
                        }
                        else
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = "Some invoice data could not be synced with SAP. Here is the list : \n " + String.Join("\n", failureItems);
                        }
                    }
                    else
                    {

                        responseCode.messageCode = "E";
                        responseCode.messageString = "No response received from SAP.";
                    }


                }

                

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
                DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                // Format the DateTime object to yyyyMM (month-year) string
                string result = date.ToString("yyyy");

                return result;
            }
            catch (FormatException)
            {
                // Handle invalid input date format
                return "Invalid Date Format";
            }
        }


        public String invoiceNumberGeneration(string vendorcode)
        {
            String invoiceNumber = "SP";

            int n = vendorcode.Length;
            if (n != 0)
            {
             
                invoiceNumber = invoiceNumber + vendorcode[n - 4] + vendorcode[n - 3] + vendorcode[n - 2] + vendorcode[n - 1] ;
            }


            
            String temp = String.Format("{0:D5}", serialNumber);
            invoiceNumber = invoiceNumber + temp;
            

            String Fy = ((DateTime.Now.Year)%100).ToString() + ((DateTime.Now.Year + 1)%100).ToString();

            invoiceNumber = invoiceNumber + "/" + Fy;
            serialNumber++;//got from database
            return invoiceNumber;
        }
        
    }
}