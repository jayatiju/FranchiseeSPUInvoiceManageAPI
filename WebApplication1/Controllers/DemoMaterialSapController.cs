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

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class DemoMaterialSapController : ApiController
    {
        [HttpGet]
        public List<MaterialMaster> Get()
        {
            List<MaterialMaster> materialMastersList = new List<MaterialMaster>();


            using (ZWS_SPU_MATERIAL_LIST_SRVClient client = new ZWS_SPU_MATERIAL_LIST_SRVClient("list2"))
            {
                try
                {
                   
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    client.ClientCredentials.UserName.UserName = "RFCUSER";
                    client.ClientCredentials.UserName.Password = "Init#1234";
                    var request = new ZFM_SPU_MATERIALSRequest
                    {
                        ZFM_SPU_MATERIALS = new ZFM_SPU_MATERIALS()
                        {
                            // Set properties of the request object if needed
                        }
                    };

                    //call sap service operation
                    ZFM_SPU_MATERIALSResponse response = client.ZFM_SPU_MATERIALS(request.ZFM_SPU_MATERIALS);

                    foreach (var sapMaterial in response.ET_MATERIALS)
                    {
                        MaterialMaster materialMaster = new MaterialMaster();

                        materialMaster.materialcode = sapMaterial.MATNR;
                        materialMaster.materialcat = sapMaterial.MAT_CAT;
                        materialMaster.categoryhierarchyid = sapMaterial.MTART;
                        materialMaster.taxtariffcode = sapMaterial.STEUC;
                        materialMaster.taxtype = sapMaterial.TAX_TYPE;
                        materialMaster.materialdesc = sapMaterial.MTART;
                        materialMaster.uom = sapMaterial.MEINS;
                        materialMaster.productid = "null";
                        materialMaster.groupid = sapMaterial.MATKL;
                        materialMaster.isactive = "X";


                        materialMastersList.Add(materialMaster);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }

            return materialMastersList;
        }
    }
}
