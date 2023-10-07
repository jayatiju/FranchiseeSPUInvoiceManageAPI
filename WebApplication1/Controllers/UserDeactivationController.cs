using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Windows;
using MySql.Data.MySqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class UserDeactivationController : ApiController
    {
        
        ResponseCode response = new ResponseCode();
        private readonly MySqlConnection _connection;
        public UserDeactivationController()
        {
            try
            {
                _connection = new MySqlConnection(ConnectionString.connString);
                _connection.Open();
            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                response.messageCode = "E";
                response.messageString = ex.Message;
            }
        }

        [HttpPost]
        public ResponseCode Post([FromBody] Deactivation deactivation)
        {
            try
            {
                //deactivate user
                string sql = "UPDATE user_table SET isactive = '' WHERE email = @email;";
                using (var command = new MySqlCommand(sql, _connection))
                {

                    command.Parameters.AddWithValue("@email", deactivation.email);


                    var reader = command.ExecuteReader();
                    response.messageCode = "S";
                    response.messageString = "User Deactivated";
                    
                }
            }
            catch (Exception ex)
            {
                response.messageCode = "E";
                response.messageString = ex.Message;
            }
            finally
            {
                _connection.Close();
            }
            return response;
           
        }
        
    }
}