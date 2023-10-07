using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.VendorMasterSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class DemoVendorSapController : ApiController
    {
        [HttpGet]
        public List<VendorMaster> Get()
        {
            List<VendorMaster> vendorMastersList = new List<VendorMaster>();


            using (ZWS_SPU_VENDOR_LIST_SRVClient client = new ZWS_SPU_VENDOR_LIST_SRVClient("list1"))
            {
                try
                {

                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    client.ClientCredentials.UserName.UserName = "BIRAJ";
                    client.ClientCredentials.UserName.Password = "Ifb-12345";

                    var request = new ZFM_SPU_VENDORSRequest
                    {
                        ZFM_SPU_VENDORS = new ZFM_SPU_VENDORS()
                        {
                            // Set properties of the request object if needed
                        }
                    };
                   
                    //call sap service operation
                    ZFM_SPU_VENDORSResponse response = client.ZFM_SPU_VENDORS(request.ZFM_SPU_VENDORS);

                    foreach (var sapVendor in response.ET_VENDORS)
                    {
                        VendorMaster vendorMaster = new VendorMaster();

                        vendorMaster.vendorcode = sapVendor.VENDORCODE;
                        vendorMaster.vendorname = sapVendor.VENDORNAME;
                        vendorMaster.address = sapVendor.VENDORADDRESS;
                        vendorMaster.branchcode = sapVendor.BRANCHCODE;
                        vendorMaster.gstinnum = sapVendor.GSTINNUM;
                        vendorMaster.pannum = sapVendor.PAN;
                        vendorMaster.mobilenum = sapVendor.MOBILENO;
                        vendorMaster.emailid = sapVendor.EMAILID;
                        vendorMaster.pincode = sapVendor.PINCODE;
                        vendorMaster.gstinregtype = sapVendor.GSTNREGTYPE;
                        vendorMaster.effectivedate = sapVendor.EFFECTIVEDATE;
                        vendorMaster.state = sapVendor.STATE;
                        vendorMaster.regioncode = sapVendor.REGIONCODE;
                        vendorMaster.city = sapVendor.CITY;
                        vendorMaster.isactive = "X";
                        vendorMaster.branchname = sapVendor.BRANCHNAME;
                        vendorMaster.regiondesc = sapVendor.REGIONDESC;
                        vendorMaster.statedesc = sapVendor.STATEDESC;


                        vendorMastersList.Add(vendorMaster);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }

            return vendorMastersList;
        }
    }
}