using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Windows;
using WebApplication1.Models;
using WebApplication1.InvoiceSAPReference;
using System.ServiceModel;
using System.Web.Http.Cors;
using System.Text;
using Microsoft.Web.Administration;


namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class DemoInvoiceSapController : ApiController
    {

        public DemoInvoiceSapController()
        {
            
        }


        [HttpGet]
        public List<InvoiceOutput> Get()
        {
            List<InvoiceOutput> invoiceOutputsList = new List<InvoiceOutput>();

            using (ZWS_SPU_PUR_SRVClient client = new ZWS_SPU_PUR_SRVClient("invoices_soap12"))
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    client.ClientCredentials.UserName.UserName = "BIRAJ";
                    client.ClientCredentials.UserName.Password = "Ifb-12345";

                    var requestObject = new ZfmSpuPurRequest
                    {
                        
                        ZfmSpuPur = new ZfmSpuPur()
                        {
                            
                            CompanyCode = "1000",
                            DocumentNumber = "",
                            EndDate = "2019-07-31",
                            FiscalYear = "2019",
                            SegmentCode = "1103",
                            StartDate = "2019-07-01"

                        }
                    };

                    ZfmSpuPurResponse response = client.ZfmSpuPur(requestObject.ZfmSpuPur);

                    foreach (var sapInvoice in response.EtSpu)
                    {
                        InvoiceOutput invoice = new InvoiceOutput();

                        invoice.Segment = sapInvoice.Segment;
                        invoice.Region_Code = sapInvoice.Region;
                        invoice.Plant_Code = sapInvoice.Plant;
                        invoice.Financial_Year = sapInvoice.Fy;
                        invoice.GSTIN = sapInvoice.Gstin;
                        invoice.Document_Number = sapInvoice.Document;
                        invoice.Document_Date = sapInvoice.DocDate;
                        invoice.Document_Posting_Date = sapInvoice.PostDate;
                        invoice.Sales_Doc_Number = sapInvoice.SalesDoc;
                        invoice.Ship_To_Party_Number = sapInvoice.ShipToParty;
                        invoice.Ship_To_Party_Name = sapInvoice.ShipToPartyName;
                        invoice.Pin = sapInvoice.Pincode;
                        invoice.City = sapInvoice.City;
                        invoice.Address_1 = sapInvoice.Street2;
                        invoice.Address_2 = sapInvoice.Street;
                        invoice.Address_3 = sapInvoice.Street3;
                        invoice.Vendor_Code = sapInvoice.Vendor;
                        invoice.Vendor_Name = sapInvoice.VendorName;
                        invoice.Spu_Number = sapInvoice.SpuNo;
                        invoice.CRM_Ticket_Number = sapInvoice.CrmTicket;
                        invoice.Machine_Status = sapInvoice.MachStat;
                        invoice.COGS = sapInvoice.Cogs;
                        invoice.Material_Code = sapInvoice.Material;
                        invoice.Material_Group = sapInvoice.MaterialGrp;
                        invoice.Material_Description = sapInvoice.MatDes;
                        invoice.HSN = sapInvoice.Hsn;
                        invoice.Tax_Percentage = sapInvoice.Taxpercent;
                        invoice.Quantity = sapInvoice.Quantity;
                        invoice.UOM = sapInvoice.Unit;
                        invoice.Spare_Value = sapInvoice.Spare;
                        invoice.Currency = sapInvoice.Currency;
                        invoice.Assignment_Date = sapInvoice.Assignment;
                        invoice.Tax_Code = sapInvoice.TaxCode;
                        invoice.CGST_Percentage = sapInvoice.CgstPer;
                        invoice.CGST_RCM = sapInvoice.CgstRcm;
                        invoice.CGST = sapInvoice.Cgst;
                        invoice.IGST_Percentage = sapInvoice.IgstPer;
                        invoice.IGST = sapInvoice.Igst;
                        invoice.Import_IGST = sapInvoice.ImportIgst;
                        invoice.IGST_RCM = sapInvoice.IgstRcm;
                        invoice.SGST_Percentage = sapInvoice.SgstPer;
                        invoice.SGST = sapInvoice.Sgst;
                        invoice.UGST_Percentage = sapInvoice.UgstPer;
                        invoice.UGST = sapInvoice.Ugst;
                        invoice.UGST_RCM = sapInvoice.UgstRcm;
                        invoice.SGST_RCM = sapInvoice.SgstRcm;
                        invoice.Invoice_Number = sapInvoice.Invoice;
                        invoice.FG_Product_Code = sapInvoice.ZzproductId;
                        invoice.FG_Product_Name = sapInvoice.ZzproductDesc;

                        invoiceOutputsList.Add(invoice);
                    }
                }

                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }

            return invoiceOutputsList;
        }
    }
}
