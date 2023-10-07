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
    public class CustomerMasterController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public CustomerMasterController()
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
        public IHttpActionResult GetCustomerMasters()
        {
            try
            {
                string sql = "SELECT * FROM customer_master_table WHERE isactive = 'X'";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<CustomerMaster> customermasterList = new List<CustomerMaster>();

                while (reader.Read())
                {
                    CustomerMaster customerMaster = new CustomerMaster();

                    customerMaster.branchcode = reader.GetString("branchcode");
                    customerMaster.branchname = reader.GetString("branchname");
                    customerMaster.gstinnum = reader.GetString("gstinnum");
                    customerMaster.address = reader.GetString("address");
                    customerMaster.pincode = reader.GetString("pincode");
                    customerMaster.regioncode = reader.GetString("regioncode");
                    customerMaster.regiondesc = reader.GetString("regiondesc");
                    customerMaster.pannum = reader.GetString("pannum");
                    customerMaster.mobilenum = reader.GetString("mobilenum");
                    customerMaster.emailid = reader.GetString("emailid");
                    customerMaster.isactive = reader.GetString("isactive");
                    customerMaster.segment = reader.GetString("segment");


                    customermasterList.Add(customerMaster);

                }

                reader.Close();
                return Ok(customermasterList);
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

        [HttpGet]
        public IHttpActionResult GetCustomerMasters(string branchcode)
        {
            try
            {
                string sql = "SELECT * FROM customer_master_table WHERE branchcode = @branchcode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@branchcode", $"{branchcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<CustomerMaster> customermasterList = new List<CustomerMaster>();

                while (reader.Read())
                {
                    CustomerMaster customerMaster = new CustomerMaster();

                    customerMaster.branchcode = reader.GetString("branchcode");
                    customerMaster.branchname = reader.GetString("branchname");
                    customerMaster.gstinnum = reader.GetString("gstinnum");
                    customerMaster.address = reader.GetString("address");
                    customerMaster.pincode = reader.GetString("pincode");
                    customerMaster.regioncode = reader.GetString("regioncode");
                    customerMaster.regiondesc = reader.GetString("regiondesc");
                    customerMaster.pannum = reader.GetString("pannum");
                    customerMaster.mobilenum = reader.GetString("mobilenum");
                    customerMaster.emailid = reader.GetString("emailid");
                    customerMaster.isactive = reader.GetString("isactive");
                    customerMaster.segment = reader.GetString("segment");


                    customermasterList.Add(customerMaster);

                }

                reader.Close();
                return Ok(customermasterList);
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


        [HttpGet]
        public IHttpActionResult GetCustomerMastersregion(string regioncode)
        {
            try
            {
                string sql = "SELECT * FROM customer_master_table WHERE regioncode = @regioncode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@regioncode", $"{regioncode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<CustomerMaster> customermasterList = new List<CustomerMaster>();

                while (reader.Read())
                {
                    CustomerMaster customerMaster = new CustomerMaster();

                    customerMaster.branchcode = reader.GetString("branchcode");
                    customerMaster.branchname = reader.GetString("branchname");
                    customerMaster.gstinnum = reader.GetString("gstinnum");
                    customerMaster.address = reader.GetString("address");
                    customerMaster.pincode = reader.GetString("pincode");
                    customerMaster.regioncode = reader.GetString("regioncode");
                    customerMaster.regiondesc = reader.GetString("regiondesc");
                    customerMaster.pannum = reader.GetString("pannum");
                    customerMaster.mobilenum = reader.GetString("mobilenum");
                    customerMaster.emailid = reader.GetString("emailid");
                    customerMaster.isactive = reader.GetString("isactive");
                    customerMaster.segment = reader.GetString("segment");


                    customermasterList.Add(customerMaster);

                }

                reader.Close();
                return Ok(customermasterList);
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