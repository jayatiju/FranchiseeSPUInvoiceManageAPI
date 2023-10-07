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
    public class CustomerDeactivationController : ApiController
    {
        
        private readonly MySqlConnection _connection;
        ResponseCode response = new ResponseCode();
        public CustomerDeactivationController()
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
            //deactivate user
            try
            {
                string sql = "UPDATE customer_master_table SET isactive = '' WHERE branchcode = @branchcode;";
                using (var command = new MySqlCommand(sql, _connection))
                {

                    command.Parameters.AddWithValue("@branchcode", deactivation.branchcode);


                    var reader = command.ExecuteReader();

                    response.messageCode = "S";
                    response.messageString = "Customer Deactivated";

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