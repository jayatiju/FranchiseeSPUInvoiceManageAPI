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
    public class PasswordChangeController : ApiController
    {
        
        ResponseCode response = new ResponseCode();
        private readonly MySqlConnection _connection;

        //sql statement = update user_table set password = @newPassword where email = @email;
        public PasswordChangeController()
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
        public ResponseCode Post([FromBody] PasswordChange passwordChange)
        {
            try
            {
                // Save the product to the database.
                string sql = "update user_table set password = @newPassword where email = @email";
                using (var command = new MySqlCommand(sql, _connection))
                {


                    command.Parameters.AddWithValue("@newPassword", passwordChange.newPassword);

                    command.Parameters.AddWithValue("@email", passwordChange.email);


                    command.ExecuteNonQuery();

                    response.messageCode = "S";
                    response.messageString = "Password changed successfully";
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
            // Return a success message.
            return response; ;

        }
        
    }
}