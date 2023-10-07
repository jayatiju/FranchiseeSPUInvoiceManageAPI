using System;
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

                        client.ClientCredentials.UserName.UserName = "BIRAJ";
                        client.ClientCredentials.UserName.Password = "Ifb-12345";
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




                                string sql = "insert into customer_master_table (branchcode, branchname, gstinnum, address, pincode, regioncode, regiondesc, pannum, mobilenum, emailid, isactive, segment)   " +
                                    "VALUES (@branchcode, @branchname, @gstinnum, @address, @pincode, @regioncode, @regiondesc, @pannum, @mobilenum, @emailid, @isactive, @segment)";
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
}