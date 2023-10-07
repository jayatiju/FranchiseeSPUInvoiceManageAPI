using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Windows;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class GetCustomerVendorController : ApiController
    {
        private readonly MySqlConnection _connection;

        public GetCustomerVendorController()
        {
            try
            {
                _connection = new MySqlConnection(ConnectionString.connString);
                _connection.Open();
            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
            }
        }

        [HttpPost]
        public List<string> Post([FromBody] CustomerVendor customerVendor)
        {
            if (customerVendor.code == "c")
            {
                List<string> result = new List<string>();
                string sql = "SELECT * FROM customer_master_table";
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



                    customermasterList.Add(customerMaster);
                    string temp = JsonConvert.SerializeObject(customerMaster, Formatting.Indented).Replace("\\", "").Replace("\r", "").Replace("\n", "");
                    result.Add(temp);

                }

                reader.Close();
                _connection.Close();
                string customermasterListString = JsonConvert.SerializeObject(customermasterList, Formatting.Indented).Replace("\\", "").Replace("\r", "").Replace("\n", "") ;
                customermasterListString = customermasterListString.Replace("\\", "");
                return result;
            }

            else if (customerVendor.code == "v")
            {
                List<string> result = new List<string>();

                string sql = "SELECT * FROM vendor_master_table";
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

                    vendormasterList.Add(vendorMaster);
                    string temp = JsonConvert.SerializeObject(vendorMaster, Formatting.Indented).Replace("\\", "").Replace("\\\\", "").Replace("\r", "").Replace("\n", "");
                    result.Add(temp);

                }

                reader.Close();
                _connection.Close();
                string vendormasterListString = JsonConvert.SerializeObject(vendormasterList, Formatting.Indented).Replace("\\", "").Replace("\r", "").Replace("\n", "");
                vendormasterListString = vendormasterListString.Replace("\\", "");
                return result;

            }


            else
            {
                List<string> result = new List<string>();
                result.Add("Please enter c or v");
                return result;
            }
        }
    }
}















        /*[HttpGet]
        public string Get(int code)
        {
            int codeF = code;
            if (codeF == 0 || codeF == 1)
            {
                if (code == 0)
                {
                    string sql = "SELECT * FROM customer_master_table";
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
                        customerMaster.isActive = reader.GetString("isActive");



                        customermasterList.Add(customerMaster);

                    }

                    reader.Close();
                    _connection.Close();
                    return customermasterList.ToString();

                }
                else
                {
                    string sql = "SELECT * FROM vendor_master_table";
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
                        vendorMaster.isActive = reader.GetString("isActive");

                        vendormasterList.Add(vendorMaster);
                    }

                    reader.Close();
                    _connection.Close();
                    return vendormasterList.ToString();
                }

            }
            else
            {
                return "please enter C or V";
            }
        }
        /*
        /*[HttpGet]
        public string Get(int code)
        {
            if (code == 0 || code == 1) { 
                if (code == 0)
            {
                string sql = "SELECT * FROM customer_master_table";
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
                    customerMaster.isActive = reader.GetString("isActive");



                    customermasterList.Add(customerMaster);

                }

                reader.Close();
                _connection.Close();
                return customermasterList.ToString();

            }
            else if(code == 1)
            {
                string sql = "SELECT * FROM vendor_master_table";
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
                    vendorMaster.isActive = reader.GetString("isActive");

                    vendormasterList.Add(vendorMaster);
                }

                reader.Close();
                _connection.Close();
                return vendormasterList.ToString();
            }

            else
            {
                return "please enter C or V";
            }
        }
        */

/*[RoutePrefix("customers")]
        public List<CustomerMaster> GetCustomerMastersForFrontend()
        {
            string sql = "SELECT * FROM customer_master_table";
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
                customerMaster.isActive = reader.GetString("isActive");



                customermasterList.Add(customerMaster);

            }

            reader.Close();
            _connection.Close();
            return customermasterList;
        }

        [RoutePrefix("vendors")]
        public List<VendorMaster> GetVendorMastersForFrontend()
        {
            string sql = "SELECT * FROM vendor_master_table";
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
                vendorMaster.isActive = reader.GetString("isActive");

                vendormasterList.Add(vendorMaster);
            }

            reader.Close();
            _connection.Close();
            return vendormasterList;
        }
        */

