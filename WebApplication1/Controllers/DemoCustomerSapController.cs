using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.CustomerMasterSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class DemoCustomerSapController : ApiController
    {
        [HttpGet]
        public List<CustomerMaster> Get()
        {
            List<CustomerMaster> customerMastersList = new List<CustomerMaster>();


            using (ZWS_SPU_CUSTOMER_LIST_SRVClient client = new ZWS_SPU_CUSTOMER_LIST_SRVClient("list_soap12"))
            {
                try
                {

                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    client.ClientCredentials.UserName.UserName = "RFCUSER";
                    client.ClientCredentials.UserName.Password = "Init#1234";
                    var request = new ZFM_SPU_CUSTOMERSRequest
                    {
                        ZFM_SPU_CUSTOMERS = new ZFM_SPU_CUSTOMERS()
                        {
                            // Set properties of the request object if needed
                        }
                    };

                    //call sap service operation
                    ZFM_SPU_CUSTOMERSResponse response = client.ZFM_SPU_CUSTOMERS(request.ZFM_SPU_CUSTOMERS);

                    foreach (var sapCustomer in response.CUSTOMERS)
                    {
                        CustomerMaster customerMaster = new CustomerMaster();

                        String cust = sapCustomer.BRANCHCODE;
                        customerMaster.branchcode = cust.Substring(cust.Length - 4);
                        
                        customerMaster.branchname = sapCustomer.BRANCHNAME;
                        customerMaster.gstinnum = sapCustomer.GSTINNUM;
                        customerMaster.address = sapCustomer.ADDRESS;
                        customerMaster.pincode = sapCustomer.PINCODE;
                        customerMaster.regioncode = sapCustomer.REGIONCODE;
                        customerMaster.regiondesc = sapCustomer.REGIONDESC;
                        customerMaster.pannum = sapCustomer.PAN;
                        customerMaster.mobilenum = sapCustomer.MOBILENO;
                        customerMaster.emailid = sapCustomer.EMAILID;
                        customerMaster.isactive = "X";
                        customerMaster.segment = sapCustomer.SEGMENT;

                        customerMastersList.Add(customerMaster);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
            
            return customerMastersList;
        }
    }
}