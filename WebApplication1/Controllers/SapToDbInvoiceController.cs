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
using System.IO;
using System.Text;
using System.Configuration;
using FluentFTP;
using System.Data;


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
                string monthYear = convertToMonthYear(invoiceInput.StartDate);
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
                    
                    using (var statuscommand = new MySqlCommand(statusSql, _connection))
                    {
                        statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);
                        statuscommand.Parameters.AddWithValue("@Segment", invoiceInput.SegmentCode);
                        statuscommand.Parameters.AddWithValue("@Region", invoiceInput.Region);


                        statuscommand.ExecuteNonQuery();


                    }

                    String FyNow = convertToYear(invoiceInput.StartDate);

                    //data sync
                    /*   using (ZWS_SPU_PUR_SRVClient client = new ZWS_SPU_PUR_SRVClient("invoices_soap12"))
                       {
                           try
                           {

                               ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                               client.ClientCredentials.UserName.UserName = "RFCUSER";
                               client.ClientCredentials.UserName.Password = "Init#1234";

                            //      client.ClientCredentials.UserName.UserName = "BIRAJ";
                            //      client.ClientCredentials.UserName.Password = "Ifb-123";


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
                    */

                    DataTable dtCsv = ExecuteDataTable(monthYear, invoiceInput.SegmentCode);

                    if (dtCsv == null || dtCsv.Rows.Count == 0)
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
                        //     string fileContent = Encoding.UTF8.GetString(fileContents);


                        // Split content by lines (assuming each line is a row in the CSV)
                        //      var lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                        //   foreach (var line in lines)
                        foreach (DataRow row in dtCsv.Rows)
                        {
                           // var columns = line.Split(',');

                            string sql = "INSERT INTO invoice_master_table (Segment, Region_Code, Plant_Code, Financial_Year, GSTIN, Document_Number, Document_Date, Document_Posting_Date, Sales_Doc_Number, Ship_To_Party_Number, Ship_To_Party_Name, Pin, City, Address_1, Address_2, Address_3, Vendor_Code, Vendor_Name, Spu_Number, CRM_Ticket_Number, Machine_Status, COGS, Material_Code, Material_Group, Material_Description, HSN, Tax_Percentage, Quantity, UOM, Spare_Value, Currency, Assignment_Date, Tax_Code, CGST_Percentage, CGST_RCM, CGST, IGST_Percentage, IGST, Import_IGST, IGST_RCM, SGST_Percentage, SGST, UGST_Percentage, UGST, UGST_RCM, SGST_RCM, Invoice_Number, FG_Product_Code, FG_Product_Name, Ship_To_Party_MobileNumber, Ship_To_Party_Region, Ship_To_Party_Region_Desc) " +
                                        "VALUES (@Segment, @Region_Code, @Plant_Code, @Financial_Year, @GSTIN, @Document_Number, @Document_Date, @Document_Posting_Date, @Sales_Doc_Number, @Ship_To_Party_Number, @Ship_To_Party_Name, @Pin, @City, @Address_1, @Address_2, @Address_3, @Vendor_Code, @Vendor_Name, @Spu_Number, @CRM_Ticket_Number, @Machine_Status, @COGS, @Material_Code, @Material_Group, @Material_Description, @HSN, @Tax_Percentage, @Quantity, @UOM, @Spare_Value, @Currency, @Assignment_Date, @Tax_Code, @CGST_Percentage, @CGST_RCM, @CGST, @IGST_Percentage, @IGST, @Import_IGST, @IGST_RCM, @SGST_Percentage, @SGST, @UGST_Percentage, @UGST, @UGST_RCM, @SGST_RCM, @Invoice_Number, @FG_Product_Code, @FG_Product_Name, @Ship_To_Party_MobileNumber, @Ship_To_Party_Region, @Ship_To_Party_Region_Desc);";

                                    using (var command = new MySqlCommand(sql, _connection))
                                    {
                                        // Assuming you have a SqlCommand object named 'command' and a 'sapInvoice' object containing the values
                                        //String segment = sapInvoice.Segment;
                                        //segment = segment.TrimStart('0');
                                        command.Parameters.AddWithValue("@Segment", row[0]);
                                        command.Parameters.AddWithValue("@Region_Code", row[1]);
                                        command.Parameters.AddWithValue("@Plant_Code", row[2]);
                                        command.Parameters.AddWithValue("@Financial_Year", row[3]);
                                        command.Parameters.AddWithValue("@GSTIN", row[4]);
                                        command.Parameters.AddWithValue("@Document_Number", row[5]);
                                        command.Parameters.AddWithValue("@Document_Date", row[6]);
                                        command.Parameters.AddWithValue("@Document_Posting_Date", row[7]);
                                        command.Parameters.AddWithValue("@Sales_Doc_Number", row[8]);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Number", row[9]);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Name", row[10]);
                                        command.Parameters.AddWithValue("@Pin", row[11]);
                                        command.Parameters.AddWithValue("@City", row[12]);
                                        command.Parameters.AddWithValue("@Address_1", row[13]);
                                        command.Parameters.AddWithValue("@Address_2", row[14]);
                                        command.Parameters.AddWithValue("@Address_3", row[15]);
                                        command.Parameters.AddWithValue("@Vendor_Code", row[16]);
                                        command.Parameters.AddWithValue("@Vendor_Name", row[17]);
                                        command.Parameters.AddWithValue("@Spu_Number", row[18]);
                                        command.Parameters.AddWithValue("@CRM_Ticket_Number", row[19]);
                                        command.Parameters.AddWithValue("@Machine_Status", row[20]);
                                        command.Parameters.AddWithValue("@COGS", row[21]);
                                        command.Parameters.AddWithValue("@Material_Code", row[22]);
                                        command.Parameters.AddWithValue("@Material_Group", row[23]);
                                        command.Parameters.AddWithValue("@Material_Description", row[24]);
                                        command.Parameters.AddWithValue("@HSN", row[25]);
                                        command.Parameters.AddWithValue("@Tax_Percentage", row[26]);
                                        command.Parameters.AddWithValue("@Quantity", row[27]);
                                        command.Parameters.AddWithValue("@UOM", row[28]);
                                        command.Parameters.AddWithValue("@Spare_Value", row[29]);
                                        command.Parameters.AddWithValue("@Currency", row[30]);
                                        command.Parameters.AddWithValue("@Assignment_Date", row[31]);
                                        command.Parameters.AddWithValue("@Tax_Code", row[32]);
                                        command.Parameters.AddWithValue("@CGST_Percentage", row[33]);
                                        command.Parameters.AddWithValue("@CGST_RCM", row[34]);
                                        command.Parameters.AddWithValue("@CGST", row[35]);
                                        command.Parameters.AddWithValue("@IGST_Percentage", row[36]);
                                        command.Parameters.AddWithValue("@IGST", row[37]);
                                        command.Parameters.AddWithValue("@Import_IGST", row[38]);
                                        command.Parameters.AddWithValue("@IGST_RCM", row[39]);
                                        command.Parameters.AddWithValue("@SGST_Percentage", row[40]);
                                        command.Parameters.AddWithValue("@SGST", row[41]);
                                        command.Parameters.AddWithValue("@UGST_Percentage", row[42]);
                                        command.Parameters.AddWithValue("@UGST", row[43]);
                                        command.Parameters.AddWithValue("@UGST_RCM", row[44]);
                                        command.Parameters.AddWithValue("@SGST_RCM", row[45]);
                                        command.Parameters.AddWithValue("@Invoice_Number", row[46]);
                                        command.Parameters.AddWithValue("@FG_Product_Code", row[47]);
                                        command.Parameters.AddWithValue("@FG_Product_Name", row[48]);
                                        command.Parameters.AddWithValue("@Ship_To_Party_MobileNumber", row[49]);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Region", row[50]);
                                        command.Parameters.AddWithValue("@Ship_To_Party_Region_Desc", row[51]);

                                       
                                            command.ExecuteNonQuery();
                                       
                                        //command.ExecuteNonQuery();


                                    }

                                }
                            }
                            responseCode.messageCode = "S";
                            responseCode.messageString = "Data successfully inserted from SAP to Database for invoice";
                        }
                /*
                        catch (Exception ex)
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = ex.Message;
                        }
                    */

                
                //string monthYear = convertToMonthYear(invoiceInput.StartDate);
                string statusSqlLast = "UPDATE invoice_monthly_status SET Data_Sync_Flag = 'X' WHERE Month_Year = @Month_Year AND Segment = @Segment AND Region = @Region;";

                
                    using (var statuscommand = new MySqlCommand(statusSqlLast, _connection))
                    {
                        statuscommand.Parameters.AddWithValue("@Month_Year", monthYear);
                        statuscommand.Parameters.AddWithValue("@Segment", invoiceInput.SegmentCode);
                        statuscommand.Parameters.AddWithValue("@Region", invoiceInput.Region);

                        statuscommand.ExecuteNonQuery();
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

        public static DataTable ExecuteDataTable(string MonthYear, string Segment)

        {
            string host = "192.168.52.237";

            int port = 21;

            string username = "SPUINT";

            string password = "$J$#2501j";
            // string encodedPassword = Uri.EscapeDataString(password);

            string remoteDirectory = @"/home/SPUINT/SPUINTDATA/InvoiceDownloadData/";
            //string remoteFileName = "Feb.csv";
            string remoteFileName = Segment + "_" + MonthYear + ".csv"; // Updated filename format

            //string localpath = "C:\\Users\\jayat\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\SAPtoDB";
            string localpath = "C:\\Users\\USER\\source\\repos\\FranchiseeSPUInvoiceManageAPI\\WebApplication1\\SAPtoDB";



            byte[] fileContent;

            StringBuilder result = new StringBuilder();

            FtpWebRequest reqFTP;


        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
 
            string ftpUrl = $"ftp://{host}:{port}/{remoteDirectory}{remoteFileName}";
    
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpUrl));

            reqFTP.UseBinary = true;
            reqFTP.UsePassive = true;
         ///   reqFTP.EnableSsl = true;

            reqFTP.Credentials = new NetworkCredential(username, password);

            reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;

            List<string> ftpfiles = new List<string>();
            WebResponse response = reqFTP.GetResponse();
            DataTable dtCsv = new DataTable();
            try

            {

                using (WebResponse listResponse = reqFTP.GetResponse())

                {

                    using (Stream listStream = listResponse.GetResponseStream())

                    {

                        using (StreamReader listReader = new StreamReader(listStream))

                        {

                            while (!listReader.EndOfStream)

                            {

                                string file = listReader.ReadLine();

                                ftpfiles.Add(file);

                            }



                            foreach (var file in ftpfiles)

                            {

                                WebClient request = new WebClient();

                                string url = "ftp://" + host+":"+port +"/"+ remoteDirectory + "/" + file;

                                request.Credentials = new NetworkCredential(username, password);



                                byte[] newFileData = request.DownloadData(url);

                                var stream = new MemoryStream(newFileData);

                                FileInfo fileInfoo = new FileInfo(localpath + file);

                                if (fileInfoo.Exists)

                                {

                                    fileInfoo.Delete();

                                }

                                using (FileStream fileStream = new FileStream(localpath + file, FileMode.Create))

                                {

                                    stream.WriteTo(fileStream);

                                }

                                fileContent = stream.ToArray();

                                // DataTable dtCsv = new DataTable();

                                using (var reader = new StreamReader(System.IO.File.OpenRead(localpath + file)))

                                {



                                    string Fulltext;

                                    while (!reader.EndOfStream)

                                    {

                                        Fulltext = reader.ReadToEnd().ToString(); //read full file text 

                                        string[] rows = Fulltext.Split('\n'); //split full file text into rows 

                                        for (int i = 0; i < rows.Count() - 1; i++)

                                        {

                                            string[] rowValues = rows[i].Split(','); //split each row with tab separator to get individual values

                                            {

                                                if (i == 0)

                                                {

                                                    for (int j = 0; j < rowValues.Count(); j++)

                                                    {

                                                        dtCsv.Columns.Add(j.ToString()).ToString().ToUpper(); //add headers 



                                                    }

                                                    DataRow dr = dtCsv.NewRow();

                                                    for (int a = 0; a < rowValues.Count(); a++)

                                                    {

                                                        dr[a] = rowValues[a].ToString();

                                                    }

                                                    dtCsv.Rows.Add(dr); //add other rows 

                                                }

                                                else

                                                {

                                                    DataRow dr = dtCsv.NewRow();

                                                    for (int k = 0; k < rowValues.Count(); k++)

                                                    {

                                                        dr[k] = rowValues[k].ToString();

                                                    }

                                                    dtCsv.Rows.Add(dr); //add other rows 

                                                }

                                            }

                                        }

                                    }



                                }



                            }

                            listReader.Close();

                        }

                        listStream.Close();

                    }

                    listResponse.Close();

                }
                return dtCsv;

            }

            catch (WebException ex)

            {
               
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.Response is FtpWebResponse ftpResponse)
                {
                    Console.WriteLine($"FTP Status Code: {ftpResponse.StatusCode}");
                }

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return dtCsv;

            }
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