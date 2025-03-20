/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.CustomerMasterSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using System.Data;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class SapToDbCustomerController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public SapToDbCustomerController()
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
        public ResponseCode Get()
        {


            try
            {
                using (ZWS_SPU_CUSTOMER_LIST_SRVClient client = new ZWS_SPU_CUSTOMER_LIST_SRVClient("list_soap12"))
                {
                    try
                    {
                        
                        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                        
                        client.ClientCredentials.UserName.UserName = "RFCUSER";
                        client.ClientCredentials.UserName.Password = "Init#1234";

                     //   client.ClientCredentials.UserName.UserName = "BIRAJ";
                     //   client.ClientCredentials.UserName.Password = "Ifb-123";

                        var request = new ZFM_SPU_CUSTOMERSRequest
                        {
                            ZFM_SPU_CUSTOMERS = new ZFM_SPU_CUSTOMERS()
                            {
                                // Set properties of the request object if needed
                            }
                        };

                        //call sap service operation
                        ZFM_SPU_CUSTOMERSResponse response = client.ZFM_SPU_CUSTOMERS(request.ZFM_SPU_CUSTOMERS);
                        


                        // Check if response is null or has no data
                        if (response == null || response.CUSTOMERS == null || !response.CUSTOMERS.Any())
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = "No data in SAP";
                        }
                        else
                        {
                            //delete everything from database
                            string deleteSql = "delete from customer_master_table";
                            var deleteCommand = new MySqlCommand(deleteSql, _connection);
                            deleteCommand.ExecuteNonQuery();

                            foreach (var sapCustomer in response.CUSTOMERS)
                            {
                                CustomerMaster customerMaster = new CustomerMaster();

                                String cust = sapCustomer.BRANCHCODE;
                                if (cust.Length != 0)
                                {

                                    customerMaster.branchcode = cust.Substring(cust.Length - 4);
                                }
                                else
                                {
                                    customerMaster.branchcode = cust;
                                }

                                customerMaster.branchname = sapCustomer.BRANCHNAME;
                                customerMaster.gstinnum = sapCustomer.GSTINNUM;
                                customerMaster.address = sapCustomer.ADDRESS;
                                customerMaster.pincode = sapCustomer.PINCODE;
                                customerMaster.regioncode = sapCustomer.REGIONCODE;
                                customerMaster.regiondesc = sapCustomer.REGIONDESC;
                                customerMaster.pannum = sapCustomer.PAN;
                                customerMaster.mobilenum = sapCustomer.MOBILENO;
                                customerMaster.emailid = sapCustomer.EMAILID;
                                customerMaster.isactive = "X";

                                String seg = sapCustomer.SEGMENT;
                                if (seg.Length != 0)
                                {
                                    customerMaster.segment = seg.Substring(seg.Length - 4);
                                }
                                else
                                {
                                    customerMaster.segment = sapCustomer.SEGMENT;
                                }

                                customerMaster.customercin = sapCustomer.CIN;

                                string plantdescsap = sapCustomer.PLANT_DESCR;
                                if (plantdescsap.Length != 0)
                                {
                                    customerMaster.plantdesc = plantdescsap;
                                }
                                else
                                {
                                    customerMaster.plantdesc = "";
                                }


                                string sql = "insert into customer_master_table (branchcode, branchname, gstinnum, address, pincode, regioncode, regiondesc, pannum, mobilenum, emailid, isactive, segment, customer_cin, plantdesc)   " +
                                    "VALUES (@branchcode, @branchname, @gstinnum, @address, @pincode, @regioncode, @regiondesc, @pannum, @mobilenum, @emailid, @isactive, @segment, @customercin, @plantdesc)";
                                using (var command = new MySqlCommand(sql, _connection))
                                {
                                    command.Parameters.AddWithValue("@branchcode", customerMaster.branchcode);
                                    command.Parameters.AddWithValue("@branchname", customerMaster.branchname);
                                    command.Parameters.AddWithValue("@gstinnum", customerMaster.gstinnum);
                                    command.Parameters.AddWithValue("@address", customerMaster.address);
                                    command.Parameters.AddWithValue("@pincode", customerMaster.pincode);
                                    command.Parameters.AddWithValue("@regioncode", customerMaster.regioncode);
                                    command.Parameters.AddWithValue("@regiondesc", customerMaster.regiondesc);
                                    command.Parameters.AddWithValue("@pannum", customerMaster.pannum);
                                    command.Parameters.AddWithValue("@mobilenum", customerMaster.mobilenum);
                                    command.Parameters.AddWithValue("@emailid", customerMaster.emailid);
                                    command.Parameters.AddWithValue("@isactive", customerMaster.isactive);
                                    command.Parameters.AddWithValue("@segment", customerMaster.segment);
                                    command.Parameters.AddWithValue("@customercin", customerMaster.customercin);
                                    command.Parameters.AddWithValue("@plantdesc", customerMaster.plantdesc);

                                    command.ExecuteNonQuery();


                                }
                            }
                            responseCode.messageCode = "S";
                            responseCode.messageString = "Data successfully inserted from SAP to Database for customer";
                        
                        }
                    }
                    catch (Exception ex)
                    {
                        responseCode.messageCode = "E";
                        responseCode.messageString = ex.Message;
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
        
    }
}*/


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
    public class SapToDbCustomerController : ApiController
    {

        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public SapToDbCustomerController()
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
        public ResponseCode Get()
        {
            try
            {
                DataTable dtCsv = ExecuteDataTable();

                if (dtCsv == null || dtCsv.Rows.Count == 0)

                {

                    responseCode.messageCode = "E";
                    responseCode.messageString = "No data in to sync for Customers";
                }


                else
                {
                    //delete everything from database
                    string deleteSql = "delete from customer_master_table";
                    var deleteCommand = new MySqlCommand(deleteSql, _connection);
                    deleteCommand.ExecuteNonQuery();



                    //foreach (DataRow row in dtCsv.Rows)
                    foreach (DataRow row in dtCsv.Rows.Cast<DataRow>().Skip(1))
                        {

                        // Extract values from CSV
                        string branchcode = row["0"].ToString();
                        string segment = row["10"].ToString();
                       // string customerCIN = row["11"].ToString();
                        string plantDesc = row["12"].ToString();

                        // Apply the logic to extract last 4 characters if length > 0
                        string formattedBranchCode = branchcode.Length != 0 ? branchcode.Substring(Math.Max(0, branchcode.Length - 4)) : branchcode;
                        string formattedSegment = segment.Length != 0 ? segment.Substring(Math.Max(0, segment.Length - 4)) : segment;
                        string formattedPlantDesc = plantDesc.Length != 0 ? plantDesc : "";


                        string sql = "INSERT INTO customer_master_table (branchcode,branchname, gstinnum, address, pincode, regioncode, regiondesc, pannum, mobilenum, emailid, isactive, segment, customer_cin, plantdesc) " +
                                   "VALUES (@branchcode, @branchname, @gstinnum, @address, @pincode, @regioncode, @regiondesc, @pannum, @mobilenum, @emailid, @isactive, @segment, @customer_cin, @plantdesc);";
                        using (var command = new MySqlCommand(sql, _connection))
                        {
                            command.Parameters.AddWithValue("@branchcode", row[0]);
                            command.Parameters.AddWithValue("@branchname", row[1]);
                            command.Parameters.AddWithValue("@gstinnum", row[2]);
                            command.Parameters.AddWithValue("@address", row[3]);
                            command.Parameters.AddWithValue("@pincode", row[4]);
                            command.Parameters.AddWithValue("@regioncode", row[5]);
                            command.Parameters.AddWithValue("@regiondesc", row[6]);
                            command.Parameters.AddWithValue("@pannum", row[7]);
                            command.Parameters.AddWithValue("@mobilenum", row[8]);
                            command.Parameters.AddWithValue("@emailid", row[9]);
                            command.Parameters.AddWithValue("@isactive", 'X');
                            command.Parameters.AddWithValue("@segment", row[11]);
                            command.Parameters.AddWithValue("@customer_cin", row[12]);
                            command.Parameters.AddWithValue("@plantdesc", row[13]);

                            command.ExecuteNonQuery();
                        }

                    }
                }
                responseCode.messageCode = "S";
                responseCode.messageString = "Data successfully inserted from FTP to Database for Customer";

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
        public static DataTable ExecuteDataTable()
        {
            string host = "192.168.52.237";

            int port = 21;

            string username = "SPUINT";

            string password = "$J$#2501j";
            // string encodedPassword = Uri.EscapeDataString(password);

            string remoteDirectory = @"/home/SPUINT/SPUINTDATA/CustomerDownloadData/";

            string remoteFileName = "Customers.csv"; // Updated filename format

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

                                string url = "ftp://" + host + ":" + port + "/" + remoteDirectory + "/" + file;

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

                                            string[] rowValues = rows[i].Split(';'); //split each row with tab separator to get individual values

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
    }
}
