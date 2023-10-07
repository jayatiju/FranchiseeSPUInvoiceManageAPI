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
    public class MaterialMasterController : ApiController
    {
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        public MaterialMasterController()
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
        public IHttpActionResult GetMaterialMasters()
        {
            try
            {
                string sql = "SELECT * FROM material_master_table WHERE isactive = 'X'";
                MySqlCommand command = new MySqlCommand(sql, _connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<MaterialMaster> materialmasterList = new List<MaterialMaster>();

                while (reader.Read())
                {
                    MaterialMaster materialMaster = new MaterialMaster();

                    materialMaster.materialcode = reader.GetString("materialcode");
                    materialMaster.materialcat = reader.GetString("materialcat");
                    materialMaster.categoryhierarchyid = reader.GetString("categoryhierarchyid");
                    materialMaster.taxtariffcode = reader.GetString("taxtariffcode");
                    materialMaster.taxtype = reader.GetString("taxtype");
                    materialMaster.materialdesc = reader.GetString("materialdesc");
                    materialMaster.uom = reader.GetString("uom");
                    materialMaster.productid = reader.GetString("productid");
                    materialMaster.groupid = reader.GetString("groupid");
                    materialMaster.isactive = reader.GetString("isactive");

                    materialmasterList.Add(materialMaster);

                }

                reader.Close();
                return Ok(materialmasterList);
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


