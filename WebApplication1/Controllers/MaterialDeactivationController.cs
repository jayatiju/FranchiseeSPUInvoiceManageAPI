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
    public class MaterialDeactivationController : ApiController
    {
        
        ResponseCode response = new ResponseCode();
        private readonly MySqlConnection _connection;
        public MaterialDeactivationController()
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
                string sql = "UPDATE material_master_table SET isactive = '' WHERE materialcode = @materialcode;";
                using (var command = new MySqlCommand(sql, _connection))
                {

                    command.Parameters.AddWithValue("@materialcode", deactivation.materialcode);


                    var reader = command.ExecuteReader();
                    response.messageCode = "S";
                    response.messageString = "Material Deactivated";
                    
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