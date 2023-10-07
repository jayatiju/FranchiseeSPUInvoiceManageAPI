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
    public class UserListController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public UserListController()
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
        public IHttpActionResult GetUserList()
        {
            List<Registration> listRegistration = new List<Registration>();
            try
            {
                string sql = "select * from user_table where isactive = 'X'";

                MySqlCommand command = new MySqlCommand(sql, _connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Registration registration = new Registration();

                    registration.userid = reader.GetString("userid");
                    registration.usertype = reader.GetString("usertype");
                    registration.firstname = reader.GetString("firstname");
                    registration.lastname = reader.GetString("lastname");
                    registration.password = reader.GetString("password");
                    registration.refid = reader.GetString("refid");
                    registration.regioncode = reader.GetString("regioncode");
                    registration.email = reader.GetString("email");
                    registration.phnum = reader.GetString("phnum");
                    registration.isactive = reader.GetString("isactive");
                    registration.region_desc = reader.GetString("region_desc");
                    registration.branchcode = reader.GetString("branchcode");
                    registration.segment = reader.GetString("segment");

                    listRegistration.Add(registration);
                }

                reader.Close();
                return Ok(listRegistration);
            }
            catch(Exception ex)
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