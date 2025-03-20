/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.VendorMasterSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using System.Globalization;


namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class SapToDbVendorController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public SapToDbVendorController()
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
            //List<VendorMaster> vendorMastersList = new List<VendorMaster>();
            try
            {

                using (ZWS_SPU_VENDOR_LIST_SRVClient client = new ZWS_SPU_VENDOR_LIST_SRVClient("list1"))
                {
                    try
                    {

                        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                        client.ClientCredentials.UserName.UserName = "RFCUSER";
                        client.ClientCredentials.UserName.Password = "Init#1234";

                    //    client.ClientCredentials.UserName.UserName = "BIRAJ";
                    //    client.ClientCredentials.UserName.Password = "Ifb-123";

                        var request = new ZFM_SPU_VENDORSRequest
                        {
                            ZFM_SPU_VENDORS = new ZFM_SPU_VENDORS()
                            {
                                // Set properties of the request object if needed
                            }
                        };

                        //call sap service operation
                        ZFM_SPU_VENDORSResponse response = client.ZFM_SPU_VENDORS(request.ZFM_SPU_VENDORS);
                        

                        // Check if response is null or has no data
                        if (response == null || response.ET_VENDORS == null || !response.ET_VENDORS.Any())
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = "No data in SAP";
                        }
                        else
                        {
                            //delete everything from database
                            string deleteSql = "delete from vendor_master_table";
                            var deleteCommand = new MySqlCommand(deleteSql, _connection);
                            deleteCommand.ExecuteNonQuery();

                            foreach (var sapVendor in response.ET_VENDORS)
                            {
                                VendorMaster vendorMaster = new VendorMaster();

                                vendorMaster.vendorcode = sapVendor.VENDORCODE;
                                vendorMaster.vendorname = sapVendor.VENDORNAME;
                                vendorMaster.address = sapVendor.VENDORADDRESS;
                                vendorMaster.branchcode = sapVendor.BRANCHCODE;
                                vendorMaster.gstinnum = sapVendor.GSTINNUM;
                                vendorMaster.pannum = sapVendor.PAN;
                                vendorMaster.mobilenum = sapVendor.MOBILENO;
                                vendorMaster.emailid = sapVendor.EMAILID;
                                vendorMaster.pincode = sapVendor.PINCODE;
                                vendorMaster.gstinregtype = sapVendor.GSTNREGTYPEDESC;
                                vendorMaster.effectivedate = sapVendor.EFFECTIVEDATE;
                                vendorMaster.state = sapVendor.STATE;
                                vendorMaster.regioncode = sapVendor.REGIONCODE;
                                vendorMaster.city = sapVendor.CITY;
                                vendorMaster.isactive = "X";
                                vendorMaster.branchname = sapVendor.BRANCHNAME;
                                vendorMaster.regiondesc = sapVendor.REGIONDESC;
                                vendorMaster.statedesc = sapVendor.STATEDESC;
                                vendorMaster.vendorcin = sapVendor.CIN;






                                //vendorMastersList.Add(vendorMaster);
                                string sql = "insert into vendor_master_table (vendorcode, vendorname, address, branchcode, gstinnum, pannum, mobilenum, emailid, pincode, gstinregtype, effectivedate, state, regioncode, city, isactive, branchname, regiondesc, statedesc, vendor_cin)   " +
                                    "VALUES (@vendorcode, @vendorname, @address, @branchcode, @gstinnum, @pannum, @mobilenum, @emailid, @pincode, @gstinregtype, @effectivedate, @state, @regioncode, @city, @isactive, @branchname, @regiondesc, @statedesc, @vendorcin)";
                                using (var command = new MySqlCommand(sql, _connection))
                                {
                                    command.Parameters.AddWithValue("@vendorcode", vendorMaster.vendorcode);
                                    command.Parameters.AddWithValue("@vendorname", vendorMaster.vendorname);
                                    command.Parameters.AddWithValue("@address", vendorMaster.address);
                                    command.Parameters.AddWithValue("@branchcode", vendorMaster.branchcode);
                                    command.Parameters.AddWithValue("@gstinnum", vendorMaster.gstinnum);
                                    command.Parameters.AddWithValue("@pannum", vendorMaster.pannum);
                                    command.Parameters.AddWithValue("@mobilenum", vendorMaster.mobilenum);
                                    command.Parameters.AddWithValue("@emailid", vendorMaster.emailid);
                                    command.Parameters.AddWithValue("@pincode", vendorMaster.pincode);
                                    command.Parameters.AddWithValue("@gstinregtype", vendorMaster.gstinregtype);
                                    command.Parameters.AddWithValue("@effectivedate", vendorMaster.effectivedate);
                                    command.Parameters.AddWithValue("@state", vendorMaster.state);
                                    command.Parameters.AddWithValue("@regioncode", vendorMaster.regioncode);
                                    command.Parameters.AddWithValue("@city", vendorMaster.city);
                                    command.Parameters.AddWithValue("@isactive", vendorMaster.isactive);
                                    command.Parameters.AddWithValue("@branchname", vendorMaster.branchname);
                                    command.Parameters.AddWithValue("@statedesc", vendorMaster.statedesc);
                                    command.Parameters.AddWithValue("@regiondesc", vendorMaster.regiondesc);
                                    command.Parameters.AddWithValue("@vendorcin", vendorMaster.vendorcin);

                                    command.ExecuteNonQuery();


                                }
                           
                                var VendorCode = sapVendor.VENDORCODE;
                                var ifNewVendor = 0;

                                //DateTime serverTime = DateTime.Now;

                                string getCounter = "SELECT * FROM franchiseeinvoicedb.counter where VendorCode = @vendorcode" ;
                                MySqlCommand sqlCommand = new MySqlCommand(getCounter, _connection);
                                sqlCommand.Parameters.AddWithValue("@vendorcode", VendorCode);
                                MySqlDataReader counterReader = sqlCommand.ExecuteReader();

                                //DateTime date = DateTime.UtcNow;
                                //DateTime date = DateTime.ParseExact(DateTime.Now.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                                DateTime currentDate = DateTime.UtcNow;
                                DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(currentDate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));


                                int currentYear = istNow.Year;
                                int fiscalYear;
                                if (istNow.Month >= 5) // Assuming fiscal year starts from April
                                {
                                    fiscalYear = currentYear;
                                }
                                else
                                {
                                    fiscalYear = currentYear - 1;
                                }


                                if (!counterReader.HasRows)
                                {
                                    ifNewVendor = 1;
                                }
                                else
                                {
                                    while (counterReader.Read())
                                    {
                                        DateTime vendorUpdateDate = counterReader.GetDateTime("UpdateDate");

                                        int currentYearVendorUpdate = vendorUpdateDate.Year;
                                        int fiscalYearVendorUpdate;
                                        if (vendorUpdateDate.Month >= 5) // Assuming fiscal year starts from April
                                        {
                                            fiscalYearVendorUpdate = currentYearVendorUpdate;
                                        }
                                        else
                                        {
                                            fiscalYearVendorUpdate = currentYearVendorUpdate - 1;
                                        }

                                        //String FyNow = ((date.Year) % 100).ToString() + ((date.Year + 1) % 100).ToString();
                                        //String FyVendorUpdateDate = ((vendorUpdateDate.Year) % 100).ToString() + ((vendorUpdateDate.Year + 1) % 100).ToString();
                                        
                                        if (fiscalYear > fiscalYearVendorUpdate)
                                        {
                                            ifNewVendor = 2;

                                        }
                                    }
                                }
                                
                                counterReader.Close();

                                vendorCounter iVendorCounter = new vendorCounter();
                               
                                if (ifNewVendor == 1)
                                { 
                                    vendorCounter iVendorCounternew = new vendorCounter();
                                    iVendorCounternew.vendorcode = VendorCode;
                                    iVendorCounternew.counter = 1;

                                    string sqlcounter = "insert into counter (serialCounter, VendorCode, UpdateDate)" +
                                    "VALUES (@serialcounter, @vendorcode, @UpdateDate)";
                                    using (var commandcounter = new MySqlCommand(sqlcounter, _connection))
                                    {
                                        commandcounter.Parameters.AddWithValue("@serialcounter", iVendorCounternew.counter);
                                        commandcounter.Parameters.AddWithValue("@vendorcode", iVendorCounternew.vendorcode);
                                        commandcounter.Parameters.AddWithValue("@UpdateDate", istNow);
                                        commandcounter.ExecuteNonQuery();
                                    }


                                }

                                if (ifNewVendor == 2)
                                {
                                    vendorCounter iVendorCounternew = new vendorCounter();
                                    iVendorCounternew.vendorcode = VendorCode;
                                    iVendorCounternew.counter = 1;

                                    string sqlcounter = "UPDATE `franchiseeinvoicedb`.`counter` SET `serialCounter` = @serialcounter, `UpdateDate` = @UpdateDate " +
                                        "WHERE `VendorCode` = @vendorcode; ";
                                    using (var commandcounter = new MySqlCommand(sqlcounter, _connection))
                                    {
                                        commandcounter.Parameters.AddWithValue("@serialcounter", iVendorCounternew.counter);
                                        commandcounter.Parameters.AddWithValue("@vendorcode", iVendorCounternew.vendorcode);
                                        commandcounter.Parameters.AddWithValue("@UpdateDate", istNow);
                                        commandcounter.ExecuteNonQuery();
                                    }


                                }


                            }

                            responseCode.messageCode = "S";
                            responseCode.messageString = "Data successfully inserted from SAP to Database for vendor";
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
} */


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
using System.Data.Common;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class SapToDbVendorController : ApiController
    {

        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public SapToDbVendorController()
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
                    responseCode.messageString = "No data in SAP";
                }


                else
                {
                    //delete everything from database
                    string deleteSql = "delete from vendor_master_table";
                    var deleteCommand = new MySqlCommand(deleteSql, _connection);
                    deleteCommand.ExecuteNonQuery();

                    //foreach (DataRow row in dtCsv.Rows)
                    foreach (DataRow row in dtCsv.Rows.Cast<DataRow>().Skip(1))

                    {
                        string sql = "insert into vendor_master_table (vendorcode, vendorname, address, branchcode, gstinnum, pannum, mobilenum, emailid, pincode, gstinregtype, effectivedate, state, regioncode, city, isactive, branchname, regiondesc, statedesc, vendor_cin)   " +
                                    "VALUES (@vendorcode, @vendorname, @address, @branchcode, @gstinnum, @pannum, @mobilenum, @emailid, @pincode, @gstinregtype, @effectivedate, @state, @regioncode, @city, @isactive, @branchname, @regiondesc, @statedesc, @vendorcin)";
                        using (var command = new MySqlCommand(sql, _connection))
                        {
                            command.Parameters.AddWithValue("@vendorcode", row[0]);
                            command.Parameters.AddWithValue("@vendorname", row[1]);
                            command.Parameters.AddWithValue("@address", row[2]);
                            command.Parameters.AddWithValue("@branchcode", row[3]);
                            command.Parameters.AddWithValue("@gstinnum", row[4]);
                            command.Parameters.AddWithValue("@pannum", row[5]);
                            command.Parameters.AddWithValue("@regiondesc", row[6]);
                            command.Parameters.AddWithValue("@pannum", row[7]);
                            command.Parameters.AddWithValue("@mobilenum", row[8]);
                            command.Parameters.AddWithValue("@emailid", row[9]);
                            command.Parameters.AddWithValue("@pincode", row[10]);
                            command.Parameters.AddWithValue("@gstinregtype", row[11]);
                            command.Parameters.AddWithValue("@effectivedate", row[12]);
                            command.Parameters.AddWithValue("@state", row[13]);
                            command.Parameters.AddWithValue("@regioncode", row[14]);
                            command.Parameters.AddWithValue("@city", row[15]);
                            command.Parameters.AddWithValue("@isactive", 'X');
                            command.Parameters.AddWithValue("@branchname", row[17]);

                            command.Parameters.AddWithValue("@regiondesc", row[18]);
                            command.Parameters.AddWithValue("@statedesc", row[19]);
                            command.Parameters.AddWithValue("@vendor_cin", row[20]);





                            command.ExecuteNonQuery();
                        }
                        string VendorCode = row[0].ToString(); // Use vendor code from CSV row
                        int ifNewVendor = 0;

                        //var ifNewVendor = 0;

                        //DateTime serverTime = DateTime.Now;

                        string getCounter = "SELECT * FROM franchiseeinvoicedb.counter where VendorCode = @vendorcode";
                        MySqlCommand sqlCommand = new MySqlCommand(getCounter, _connection);
                        sqlCommand.Parameters.AddWithValue("@vendorcode", VendorCode);
                        MySqlDataReader counterReader = sqlCommand.ExecuteReader();

                        //DateTime date = DateTime.UtcNow;
                        //DateTime date = DateTime.ParseExact(DateTime.Now.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                        DateTime currentDate = DateTime.UtcNow;
                        DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(currentDate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));


                        int currentYear = istNow.Year;
                        int fiscalYear;
                        if (istNow.Month >= 5) // Assuming fiscal year starts from April
                        {
                            fiscalYear = currentYear;
                        }
                        else
                        {
                            fiscalYear = currentYear - 1;
                        }


                        if (!counterReader.HasRows)
                        {
                            ifNewVendor = 1;
                        }
                        else
                        {
                            while (counterReader.Read())
                            {
                                DateTime vendorUpdateDate = counterReader.GetDateTime("UpdateDate");

                                int currentYearVendorUpdate = vendorUpdateDate.Year;
                                int fiscalYearVendorUpdate;
                                if (vendorUpdateDate.Month >= 5) // Assuming fiscal year starts from April
                                {
                                    fiscalYearVendorUpdate = currentYearVendorUpdate;
                                }
                                else
                                {
                                    fiscalYearVendorUpdate = currentYearVendorUpdate - 1;
                                }

                                //String FyNow = ((date.Year) % 100).ToString() + ((date.Year + 1) % 100).ToString();
                                //String FyVendorUpdateDate = ((vendorUpdateDate.Year) % 100).ToString() + ((vendorUpdateDate.Year + 1) % 100).ToString();

                                if (fiscalYear > fiscalYearVendorUpdate)
                                {
                                    ifNewVendor = 2;

                                }
                            }
                        }

                        counterReader.Close();

                        vendorCounter iVendorCounter = new vendorCounter();

                        if (ifNewVendor == 1)
                        {
                            vendorCounter iVendorCounternew = new vendorCounter();
                            iVendorCounternew.vendorcode = VendorCode;
                            iVendorCounternew.counter = 1;

                            string sqlcounter = "insert into counter (serialCounter, VendorCode, UpdateDate)" +
                            "VALUES (@serialcounter, @vendorcode, @UpdateDate)";
                            using (var commandcounter = new MySqlCommand(sqlcounter, _connection))
                            {
                                commandcounter.Parameters.AddWithValue("@serialcounter", iVendorCounternew.counter);
                                commandcounter.Parameters.AddWithValue("@vendorcode", iVendorCounternew.vendorcode);
                                commandcounter.Parameters.AddWithValue("@UpdateDate", istNow);
                                commandcounter.ExecuteNonQuery();
                            }


                        }

                        if (ifNewVendor == 2)
                        {
                            vendorCounter iVendorCounternew = new vendorCounter();
                            iVendorCounternew.vendorcode = VendorCode;
                            iVendorCounternew.counter = 1;

                            string sqlcounter = "UPDATE `franchiseeinvoicedb`.`counter` SET `serialCounter` = @serialcounter, `UpdateDate` = @UpdateDate " +
                                "WHERE `VendorCode` = @vendorcode; ";
                            using (var commandcounter = new MySqlCommand(sqlcounter, _connection))
                            {
                                commandcounter.Parameters.AddWithValue("@serialcounter", iVendorCounternew.counter);
                                commandcounter.Parameters.AddWithValue("@vendorcode", iVendorCounternew.vendorcode);
                                commandcounter.Parameters.AddWithValue("@UpdateDate", istNow);
                                commandcounter.ExecuteNonQuery();
                            }


                        }


                    }

                    responseCode.messageCode = "S";
                    responseCode.messageString = "Data successfully inserted from SAP to Database for vendor";
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
        public static DataTable ExecuteDataTable()
        {
            string host = "192.168.52.237";

            int port = 21;

            string username = "SPUINT";

            string password = "$J$#2501j";
            // string encodedPassword = Uri.EscapeDataString(password);

            string remoteDirectory = @"/home/SPUINT/SPUINTDATA/VendorDownloadData/";

            string remoteFileName = "Vendor.csv"; // Updated filename format

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
    }
}



         