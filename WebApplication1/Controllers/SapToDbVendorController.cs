using System;
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

                        client.ClientCredentials.UserName.UserName = "BIRAJ";
                        client.ClientCredentials.UserName.Password = "Ifb-12345";

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
                                vendorMaster.gstinregtype = sapVendor.GSTNREGTYPE;
                                vendorMaster.effectivedate = sapVendor.EFFECTIVEDATE;
                                vendorMaster.state = sapVendor.STATE;
                                vendorMaster.regioncode = sapVendor.REGIONCODE;
                                vendorMaster.city = sapVendor.CITY;
                                vendorMaster.isactive = "X";
                                vendorMaster.branchname = sapVendor.BRANCHNAME;
                                vendorMaster.regiondesc = sapVendor.REGIONDESC;
                                vendorMaster.statedesc = sapVendor.STATEDESC;




                                //vendorMastersList.Add(vendorMaster);
                                string sql = "insert into vendor_master_table (vendorcode, vendorname, address, branchcode, gstinnum, pannum, mobilenum, emailid, pincode, gstinregtype, effectivedate, state, regioncode, city, isactive, branchname, regiondesc, statedesc)   " +
                                    "VALUES (@vendorcode, @vendorname, @address, @branchcode, @gstinnum, @pannum, @mobilenum, @emailid, @pincode, @gstinregtype, @effectivedate, @state, @regioncode, @city, @isactive, @branchname, @regiondesc, @statedesc)";
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


                                    command.ExecuteNonQuery();


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
}