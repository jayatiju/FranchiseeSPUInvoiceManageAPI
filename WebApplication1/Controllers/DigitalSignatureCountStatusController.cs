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
    public class GetDigitalSignatureCountStatusController : ApiController
    {
        ResponseCode responseCode = new ResponseCode();
        private readonly MySqlConnection _connection;
        

        public GetDigitalSignatureCountStatusController()
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
        public IHttpActionResult GetDigitalSignatureCountStatus(string firstdate, string lastdate, string segment)
        {
            try
            {
               // var result = new Dictionary<string, string, string>();

                string querySyncedDigiSig  = "SELECT VendorCode, VendorName, count(InvoiceNumber) FROM franchiseeinvoicedb.invoice_generation_table where InvoiceDate >= @firstdate AND InvoiceDate <= @lastdate AND segmentCode = @segment group by VendorCode, VendorName";
                MySqlCommand cmdSyncedDigiSig = new MySqlCommand(querySyncedDigiSig, _connection);
                cmdSyncedDigiSig.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdSyncedDigiSig.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdSyncedDigiSig.Parameters.AddWithValue("@segment", $"{segment}");
                MySqlDataReader reader = cmdSyncedDigiSig.ExecuteReader();

                List<FranchiseeDigiSigCount> FranchiseeDigiSigList = new List<FranchiseeDigiSigCount>();

                while (reader.Read())
                {
                    FranchiseeDigiSigCount digicount = new FranchiseeDigiSigCount();
                    digicount.VendorCode = reader.GetString("VendorCode");
                    digicount.Franchisee = reader.GetString("VendorName");
                    digicount.Total_Invoices = reader.GetString("count(InvoiceNumber)");
                    


                    FranchiseeDigiSigList.Add(digicount);
                }
                reader.Close();


                string querySyncedDigiSig1 = "SELECT VendorCode, count(InvoicePdfStatus) FROM franchiseeinvoicedb.invoice_generation_table where InvoiceDate >= @firstdate AND InvoiceDate <= @lastdate AND segmentCode = @segment AND InvoicePdfStatus = 'X' group by VendorCode";
                MySqlCommand cmdSyncedDigiSig1 = new MySqlCommand(querySyncedDigiSig1, _connection);
                cmdSyncedDigiSig1.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdSyncedDigiSig1.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdSyncedDigiSig1.Parameters.AddWithValue("@segment", $"{segment}");
                MySqlDataReader reader1 = cmdSyncedDigiSig1.ExecuteReader();

                while (reader1.Read())
                {
                    for (int i = 0; i < FranchiseeDigiSigList.Count; i++)
                    {
                        //FranchiseeDigiSigCount digicount1 = new FranchiseeDigiSigCount();

                        if (FranchiseeDigiSigList[i].VendorCode == reader1.GetString("VendorCode"))
                        {
                            FranchiseeDigiSigList[i].Total_PDFs_Created = reader1.GetString("count(InvoicePdfStatus)");

                        }

                    }

                }
                reader1.Close();



                string querySyncedDigiSig2 = "SELECT VendorCode, count(InvoicePdfDigitalSigStatus) FROM franchiseeinvoicedb.invoice_generation_table where InvoiceDate >= @firstdate AND InvoiceDate <= @lastdate AND segmentCode = @segment AND InvoicePdfDigitalSigStatus = 'X' group by VendorCode";
                MySqlCommand cmdSyncedDigiSig2 = new MySqlCommand(querySyncedDigiSig2, _connection);
                cmdSyncedDigiSig2.Parameters.AddWithValue("@firstdate", $"{firstdate}");
                cmdSyncedDigiSig2.Parameters.AddWithValue("@lastdate", $"{lastdate}");
                cmdSyncedDigiSig2.Parameters.AddWithValue("@segment", $"{segment}");
                MySqlDataReader reader2 = cmdSyncedDigiSig2.ExecuteReader();

                while (reader2.Read())
                {
                    for (int i = 0; i < FranchiseeDigiSigList.Count; i++)
                    {
                    //FranchiseeDigiSigCount digicount1 = new FranchiseeDigiSigCount();
                        
                        if (FranchiseeDigiSigList[i].VendorCode == reader2.GetString("VendorCode"))
                        {
                            FranchiseeDigiSigList[i].Digital_Signature_Done = reader2.GetString("count(InvoicePdfDigitalSigStatus)");         

                        }
                        
                    }
                    
                }
                reader2.Close();

                return Ok(FranchiseeDigiSigList);
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