using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Windows;
using MySql.Data.MySqlClient;
using WebApplication1.Models;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
   // [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class NewLoginController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public NewLoginController()
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
        
        public IHttpActionResult Post([FromBody] Login login)
        {
            try
            {
                // Save the product to the database.
                User user = new User();
                string sql = "SELECT * FROM user_table WHERE email = @email AND password = @password";
                var command = new MySqlCommand(sql, _connection);

                command.Parameters.AddWithValue("@password", login.password);
                command.Parameters.AddWithValue("@email", login.email);


                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    user.userid = reader.GetString("userid");

                    user.usertype = reader.GetString("usertype");
                    user.firstname = reader.GetString("firstname");
                    user.lastname = reader.GetString("lastname");
                    user.regioncode = reader.GetString("regioncode");
                    user.region_desc = reader.GetString("region_desc");
                    user.isactive = reader.GetString("isactive");
                    user.password = reader.GetString("password");

                    user.regioncode = reader.GetString("regioncode");
                    user.email = reader.GetString("email");
                    user.phnum = reader.GetString("phnum");
                    //user.isActive = reader.GetString("isActive");
                    //user.newPassword = reader.GetString("newPassword");
                    user.region_desc = reader.GetString("region_desc");
                    user.branchcode = reader.GetString("branchcode");
                    user.segment = reader.GetString("segment");
                }
                reader.Close();

                

                if (user.usertype.Equals("C") && user.isactive.Equals("X"))
                {
                    string sqlCustomer = "SELECT customers.branchcode, customers.branchname as reference_desc  FROM user_table users RIGHT OUTER JOIN customer_master_table customers ON users.refid = customers.branchcode where users.email = @email";
                    using (var commandCustomer = new MySqlCommand(sqlCustomer, _connection))
                    {
                        commandCustomer.Parameters.AddWithValue("@email", login.email);

                        var readerCustomer = commandCustomer.ExecuteReader();

                        while (readerCustomer.Read())
                        {


                            user.refid = readerCustomer.GetString("branchcode");
                            //user.reference_desc = readerCustomer.GetString("reference_desc");

                        }
                        readerCustomer.Close();
                    }

                    return Ok(user);

                }

                else if (user.usertype.Equals("V") && user.isactive.Equals("X"))
                {
                    string sqlVendor = "SELECT users.refid, vendors.vendorname as reference_desc  FROM user_table users LEFT OUTER JOIN vendor_master_table vendors ON users.refid = vendors.vendorcode where email = @email";
                    using (var commandVendor = new MySqlCommand(sqlVendor, _connection))
                    {
                        commandVendor.Parameters.AddWithValue("@email", login.email);

                        var readerVendor = commandVendor.ExecuteReader();

                        while (readerVendor.Read())
                        {


                            user.refid = readerVendor.GetString("refid");
                            //user.reference_desc = readerVendor.GetString("reference_desc");

                        }
                        readerVendor.Close();

                    }

                    return Ok(user);
                }

                else if (user.usertype.Equals("A") && user.isactive.Equals("X"))
                {
                    user.refid = null;
                    //user.reference_desc = null;
                    return Ok(user);
                }

                else
                {
                    user = null;
                    return Ok(user);
                }
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