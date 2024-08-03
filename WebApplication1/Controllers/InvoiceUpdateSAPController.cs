using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.InvoiceUpdateReference;

namespace WebApplication1.Controllers
{
    public class InvoiceUpdateSAPController : ApiController
    {
        List<String> failureItems = new List<String>();
        List<DbToSapOutput> dbToSapOutputs = new List<DbToSapOutput>();
        ResponseCode responseCode = new ResponseCode();

        [HttpPost]
        public IHttpActionResult SAPInvoiceUpdate([FromBody] List<DbToSapInput> dbToSapInput)
        {
            try
            {
                using (ZWS_SPU_INVOICE_POST_SRVClient client = new ZWS_SPU_INVOICE_POST_SRVClient("postlist_soap12"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    client.ClientCredentials.UserName.UserName = "RFCUSER";
                    client.ClientCredentials.UserName.Password = "Init#1234";

                //   client.ClientCredentials.UserName.UserName = "BIRAJ";
                //   client.ClientCredentials.UserName.Password = "Ifb-123";


                    ZSPU_INVOICE_UPD_STR[] invoiceArray = new ZSPU_INVOICE_UPD_STR[dbToSapInput.Count];
                    for (int i = 0; i < dbToSapInput.Count; i++)
                    {
                        invoiceArray[i] = new ZSPU_INVOICE_UPD_STR
                        {
                            DOCUMENT = dbToSapInput[i].document,
                            INVOICE = dbToSapInput[i].invoice,
                            FY = dbToSapInput[i].fy
                            //FY = "2023"
                        };
                    }


                    var request = new ZFM_SPU_INVOICE_POSTRequest
                    {
                        ZFM_SPU_INVOICE_POST = new ZFM_SPU_INVOICE_POST()
                        {
                            IT_INVOICE = invoiceArray

                        }
                    };



                    ZFM_SPU_INVOICE_POSTResponse response = client.ZFM_SPU_INVOICE_POST(request.ZFM_SPU_INVOICE_POST);

                    ZFM_SPU_INVOICE_POSTResponse1 response1 = new ZFM_SPU_INVOICE_POSTResponse1
                    {
                        ZFM_SPU_INVOICE_POSTResponse = response
                    };

                    if (response1 != null && response1.ZFM_SPU_INVOICE_POSTResponse != null)
                    {
                        // Handle the response data
                        //var responseData = response1.ZFM_SPU_INVOICE_POSTResponse;

                        // Print response details
                        foreach (var item in response.ET_RESPONSE)
                        {
                            DbToSapOutput dbToSapOutput = new DbToSapOutput();
                            dbToSapOutput.document = item.DOCUMENT;
                            dbToSapOutput.message = item.MESSAGE;

                            dbToSapOutputs.Add(dbToSapOutput);
                        }

                        foreach (var outputItem in dbToSapOutputs)
                        {
                            if (outputItem.message != "Success")
                            {
                                //responseCode.messageCode = "E";
                                //responseCode.messageString = "SAP syncing did not occur for some items";
                                //return Ok(responseCode);
                                failureItems.Add(outputItem.document);

                            }

                        }

                        if (failureItems.Count == 0)
                        {
                            responseCode.messageCode = "S";
                            responseCode.messageString = "All invoice data synced with SAP";
                        }
                        else
                        {
                            responseCode.messageCode = "E";
                            responseCode.messageString = "Some invoice data could not be synced with SAP. Here is the list : \n " + String.Join("\n", failureItems);
                        }
                    }
                    else
                    {

                        responseCode.messageCode = "E";
                        responseCode.messageString = "No response received from SAP.";
                    }


                }



                return Ok(responseCode);
            }
            catch (Exception e)
            {
                responseCode.messageCode = "E";
                responseCode.messageString = e.Message;
                return Content(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseCode));
            }
            finally
            {
                // _connection.Close();
            }
        }

        public static string convertToYear(string inputDate)
        {
            try
            {
                // Parse the input date string to a DateTime object
                DateTime date = DateTime.ParseExact(inputDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                // Format the DateTime object to yyyyMM (month-year) string
                //string result = date.ToString("yyyy");
                string result = "" + (date.Month < 4 ? date.Year - 1 : date.Year);

                return result;
            }
            catch (FormatException)
            {
                // Handle invalid input date format
                return "Invalid Date Format";
            }
        }
    }
}