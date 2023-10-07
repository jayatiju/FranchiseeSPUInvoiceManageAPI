using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.MaterialMasterSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class SapToDbMaterialController : ApiController
    {
        
        
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;

        public SapToDbMaterialController()
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
        public ResponseCode Get()
        {

            try
            {

                using (ZWS_SPU_MATERIAL_LIST_SRVClient client = new ZWS_SPU_MATERIAL_LIST_SRVClient("list2"))
                {
                    try
                    {

                        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                        client.ClientCredentials.UserName.UserName = "BIRAJ";
                        client.ClientCredentials.UserName.Password = "Ifb-12345";

                        var request = new ZFM_SPU_MATERIALSRequest
                        {
                            ZFM_SPU_MATERIALS = new ZFM_SPU_MATERIALS()
                            {
                                // Set properties of the request object if needed
                            }
                        };

                        //call sap service operation
                        ZFM_SPU_MATERIALSResponse response = client.ZFM_SPU_MATERIALS(request.ZFM_SPU_MATERIALS);
                        

                        // Check if response is null or has no data
                        if (response == null || response.ET_MATERIALS == null || !response.ET_MATERIALS.Any())
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = "No data in SAP";
                        }

                        else
                        {
                            //delete everything from database
                            string deleteSql = "delete from material_master_table";
                            var deleteCommand = new MySqlCommand(deleteSql, _connection);
                            deleteCommand.ExecuteNonQuery();

                            foreach (var sapMaterial in response.ET_MATERIALS)
                            {
                                


                                string sql = "insert into material_master_table (materialcode, materialcat, categoryhierarchyid, taxtariffcode, taxtype, materialdesc, uom, productid, groupid, isactive) values (@materialcode, @materialcat, @categoryhierarchyid, @taxtariffcode, @taxtype, @materialdesc, @uom, @productid, @groupid, @isactive)";

                                using (var command = new MySqlCommand(sql, _connection))
                                {
                                    command.Parameters.AddWithValue("@materialcode", sapMaterial.MATNR);
                                    command.Parameters.AddWithValue("@materialcat", sapMaterial.MAT_CAT);
                                    command.Parameters.AddWithValue("@categoryhierarchyid", sapMaterial.MTART);
                                    command.Parameters.AddWithValue("@taxtariffcode", sapMaterial.STEUC);
                                    command.Parameters.AddWithValue("@taxtype", sapMaterial.TAX_TYPE);
                                    command.Parameters.AddWithValue("@materialdesc", sapMaterial.MTART);
                                    command.Parameters.AddWithValue("@uom", sapMaterial.MEINS);
                                    command.Parameters.AddWithValue("@productid", "null");
                                    command.Parameters.AddWithValue("@groupid", sapMaterial.MATKL);
                                    command.Parameters.AddWithValue("@isactive", "X");

                                    command.ExecuteNonQuery();


                                }
                            }
                            responseCode.messageCode = "S";
                            responseCode.messageString = "Data successfully inserted from SAP to Database for material";
                        }
                    }
                    catch (Exception ex)
                    {
                        responseCode.messageCode = "E";
                        responseCode.messageString = ex.Message;
                    }
                }

            }

            catch (Exception ex)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = ex.Message;
            }
            finally
            {
                _connection.Close();
            }
            return responseCode;
        }
        
    }
}