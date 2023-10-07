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
    public class VendorDeactivationController : ApiController
    {
        
        ResponseCode response = new ResponseCode();
        private readonly MySqlConnection _connection;
        public VendorDeactivationController()
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
                string sql = "UPDATE vendor_master_table SET isactive = '' WHERE vendorcode = @vendorcode;";
                using (var command = new MySqlCommand(sql, _connection))
                {

                    command.Parameters.AddWithValue("@vendorcode", deactivation.vendorcode);


                    var reader = command.ExecuteReader();
                    response.messageCode = "S";
                    response.messageString = "Vendor Deactivated";
                    
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