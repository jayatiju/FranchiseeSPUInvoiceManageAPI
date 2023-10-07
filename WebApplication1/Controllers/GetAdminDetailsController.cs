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
    public class GetAdminDetailsController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public GetAdminDetailsController()
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
        public IHttpActionResult GetAdminDetails()
        {
            try
            {
                string sql = "SELECT email, phnum  FROM user_table WHERE usertype = 'A'";

                MySqlCommand command = new MySqlCommand(sql, _connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<AdminDetails> adminmasterList = new List<AdminDetails>();

                while (reader.Read())
                {
                    AdminDetails adminDetails = new AdminDetails();

                    adminDetails.email = reader.GetString("email");
                    adminDetails.phnum = reader.GetString("phnum");
                    adminDetails.usertype = "Admin";



                    adminmasterList.Add(adminDetails);

                }

                reader.Close();
                return Ok(adminmasterList);
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