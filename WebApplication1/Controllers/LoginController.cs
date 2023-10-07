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
    public class LoginController : ApiController
    {
        private readonly MySqlConnection _connection;

        public LoginController()
        {
            try
            {
                _connection = new MySqlConnection("server=127.0.0.1;port=3306;user id=root;password=root@1234;persistsecurityinfo=True;port=3306;database=franchiseeinvoicedb");
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
        public String Post([FromBody] Login login)
        {
            // Save the product to the database.
            string sql = "SELECT * FROM user_table WHERE email = @email AND password = @password AND isActive = @isActive;";
            using (var command = new MySqlCommand(sql, _connection))
            {
                command.Parameters.AddWithValue("@password", login.password);                
                command.Parameters.AddWithValue("@email", login.email);               
                command.Parameters.AddWithValue("@isactive", login.isactive);

                var reader = command.ExecuteReader();

                if (reader.Read() && login.isactive == "Yes")
                {
                    // User found, return success message.
                    return "Login is successful. Congrats!!";
                }
                else
                {
                    // User not found, return error message.
                    return "Oops!!..Invalid username or password.";
                }
            }

            _connection.Close();
        }
    }

}
