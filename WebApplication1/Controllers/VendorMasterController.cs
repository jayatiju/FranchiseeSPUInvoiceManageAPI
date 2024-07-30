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
    public class VendorMasterController : ApiController
    {
        
        private readonly MySqlConnection _connection;
        ResponseCode responseCode = new ResponseCode();
        public VendorMasterController()
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
        public IHttpActionResult GetVendorMasters()
        {
            try
            {
                string sql = "SELECT * FROM vendor_master_table WHERE isactive = 'X'";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<VendorMaster> vendormasterList = new List<VendorMaster>();

                while (reader.Read())
                {
                    VendorMaster vendorMaster = new VendorMaster();

                    vendorMaster.vendorcode = reader.GetString("vendorcode");
                    vendorMaster.vendorname = reader.GetString("vendorname");
                    vendorMaster.address = reader.GetString("address");
                    vendorMaster.branchcode = reader.GetString("branchcode");
                    vendorMaster.gstinnum = reader.GetString("gstinnum");
                    vendorMaster.pannum = reader.GetString("pannum");
                    vendorMaster.mobilenum = reader.GetString("mobilenum");
                    vendorMaster.emailid = reader.GetString("emailid");
                    vendorMaster.pincode = reader.GetString("pincode");
                    vendorMaster.gstinregtype = reader.GetString("gstinregtype");
                    vendorMaster.effectivedate = reader.GetString("effectivedate");
                    vendorMaster.state = reader.GetString("state");
                    vendorMaster.regioncode = reader.GetString("regioncode");
                    vendorMaster.city = reader.GetString("city");
                    vendorMaster.isactive = reader.GetString("isactive");
                    vendorMaster.branchname = reader.GetString("branchname");
                    vendorMaster.regiondesc = reader.GetString("regiondesc");
                    vendorMaster.statedesc = reader.GetString("statedesc");
                    vendorMaster.vendorcin = reader.GetString("vendor_cin");

                    vendormasterList.Add(vendorMaster);
                }

                reader.Close();
                return Ok(vendormasterList);
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
        public IHttpActionResult GetVendorMastersVendor(string vendorcode)
        {
            try
            {
                string sql = "SELECT * FROM vendor_master_table where vendorcode = @vendorcode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@vendorcode", $"{vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<VendorMaster> vendormasterList = new List<VendorMaster>();

                while (reader.Read())
                {

                    VendorMaster vendorMaster = new VendorMaster();

                    vendorMaster.vendorcode = reader.GetString("vendorcode");
                    vendorMaster.vendorname = reader.GetString("vendorname");
                    vendorMaster.address = reader.GetString("address");
                    vendorMaster.branchcode = reader.GetString("branchcode");
                    vendorMaster.gstinnum = reader.GetString("gstinnum");
                    vendorMaster.pannum = reader.GetString("pannum");
                    vendorMaster.mobilenum = reader.GetString("mobilenum");
                    vendorMaster.emailid = reader.GetString("emailid");
                    vendorMaster.pincode = reader.GetString("pincode");
                    vendorMaster.gstinregtype = reader.GetString("gstinregtype");
                    vendorMaster.effectivedate = reader.GetString("effectivedate");
                    vendorMaster.state = reader.GetString("state");
                    vendorMaster.regioncode = reader.GetString("regioncode");
                    vendorMaster.city = reader.GetString("city");
                    vendorMaster.isactive = reader.GetString("isactive");
                    vendorMaster.branchname = reader.GetString("branchname");
                    vendorMaster.regiondesc = reader.GetString("regiondesc");
                    vendorMaster.statedesc = reader.GetString("statedesc");
                    vendorMaster.vendorcin = reader.GetString("vendor_cin");

                    vendormasterList.Add(vendorMaster);
                }

                reader.Close();
                return Ok(vendormasterList);
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
        public IHttpActionResult GetVendorMastersBranch(string branchcode)
        {
            try
            {
                /*
                string sqlgetRegion = "SELECT regioncode FROM customer_master_table where branchcode = @branchcode";
                MySqlCommand commandgetRegion = new MySqlCommand(sqlgetRegion, _connection);
                commandgetRegion.Parameters.AddWithValue("@branchcode", $"{branchcode}");
                MySqlDataReader readergetRegion = commandgetRegion.ExecuteReader();
                readergetRegion.Read();
                string regioncode = readergetRegion.GetString("regioncode");
                readergetRegion.Close();

                */
                string sql = "select * from vendor_master_table where branchcode in(select branchcode FROM franchiseeinvoicedb.customer_master_table where segment = (select segment from franchiseeinvoicedb.customer_master_table where branchcode = @branchcode))";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@branchcode", $"{branchcode}");
                MySqlDataReader reader = command.ExecuteReader();



                List<VendorMaster> vendormasterList = new List<VendorMaster>();

                while (reader.Read())
                {

                    VendorMaster vendorMaster = new VendorMaster();

                    vendorMaster.vendorcode = reader.GetString("vendorcode");
                    vendorMaster.vendorname = reader.GetString("vendorname");
                    vendorMaster.address = reader.GetString("address");
                    vendorMaster.branchcode = reader.GetString("branchcode");
                    vendorMaster.gstinnum = reader.GetString("gstinnum");
                    vendorMaster.pannum = reader.GetString("pannum");
                    vendorMaster.mobilenum = reader.GetString("mobilenum");
                    vendorMaster.emailid = reader.GetString("emailid");
                    vendorMaster.pincode = reader.GetString("pincode");
                    vendorMaster.gstinregtype = reader.GetString("gstinregtype");
                    vendorMaster.effectivedate = reader.GetString("effectivedate");
                    vendorMaster.state = reader.GetString("state");
                    vendorMaster.regioncode = reader.GetString("regioncode");
                    vendorMaster.city = reader.GetString("city");
                    vendorMaster.isactive = reader.GetString("isactive");
                    vendorMaster.branchname = reader.GetString("branchname");
                    vendorMaster.regiondesc = reader.GetString("regiondesc");
                    vendorMaster.statedesc = reader.GetString("statedesc");
                    vendorMaster.vendorcin = reader.GetString("vendor_cin");

                    vendormasterList.Add(vendorMaster);
                }

                reader.Close();
                return Ok(vendormasterList);
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
        public IHttpActionResult GetVendorMastersBoth(string branchcode, string vendorcode)
        {
            try
            {
                string sql = "SELECT * FROM vendor_master_table where branchcode = @branchcode and vendorcode = @vendorcode";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                command.Parameters.AddWithValue("@branchcode", $"{branchcode}");
                command.Parameters.AddWithValue("@vendorcode", $"{vendorcode}");
                MySqlDataReader reader = command.ExecuteReader();

                List<VendorMaster> vendormasterList = new List<VendorMaster>();

                while (reader.Read())
                {

                    VendorMaster vendorMaster = new VendorMaster();

                    vendorMaster.vendorcode = reader.GetString("vendorcode");
                    vendorMaster.vendorname = reader.GetString("vendorname");
                    vendorMaster.address = reader.GetString("address");
                    vendorMaster.branchcode = reader.GetString("branchcode");
                    vendorMaster.gstinnum = reader.GetString("gstinnum");
                    vendorMaster.pannum = reader.GetString("pannum");
                    vendorMaster.mobilenum = reader.GetString("mobilenum");
                    vendorMaster.emailid = reader.GetString("emailid");
                    vendorMaster.pincode = reader.GetString("pincode");
                    vendorMaster.gstinregtype = reader.GetString("gstinregtype");
                    vendorMaster.effectivedate = reader.GetString("effectivedate");
                    vendorMaster.state = reader.GetString("state");
                    vendorMaster.regioncode = reader.GetString("regioncode");
                    vendorMaster.city = reader.GetString("city");
                    vendorMaster.isactive = reader.GetString("isactive");
                    vendorMaster.branchname = reader.GetString("branchname");
                    vendorMaster.regiondesc = reader.GetString("regiondesc");
                    vendorMaster.statedesc = reader.GetString("statedesc");
                    vendorMaster.vendorcin = reader.GetString("vendor_cin");

                    vendormasterList.Add(vendorMaster);
                }

                reader.Close();
                return Ok(vendormasterList);
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
