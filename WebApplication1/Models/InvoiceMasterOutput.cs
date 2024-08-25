using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class InvoiceMasterOutput
    {
        public string Segment { get; set; }

        public string Region_Code { get; set; }

        public string Plant_Code { get; set; }

        public string Financial_Year { get; set; }

        public string GSTIN { get; set; }

        public string Document_Number { get; set; }

        public string Document_Date { get; set; }

        public string Document_Posting_Date { get; set; }

        public string Sales_Doc_Number { get; set; }

        public string Ship_To_Party_Number { get; set; }

        public string Ship_To_Party_Name { get; set; }


        public string Pin { get; set; }

        public string City { get; set; }

        public string Address_1 { get; set; }

        public string Address_2 { get; set; }

        public string Address_3 { get; set; }

        public string Vendor_Code { get; set; }

        public string Vendor_Name { get; set; }

        public string Spu_Number { get; set; }

        public string CRM_Ticket_Number { get; set; }

        public string Machine_Status { get; set; }

        public string COGS { get; set; }
        public string Material_Code { get; set; }
        public string Material_Group { get; set; }
     
        public string Material_Description { get; set; }
        public string HSN { get; set; }
        public string Tax_Percentage { get; set; }
        public string Quantity { get; set; }
        public string UOM { get; set; }
        public string Spare_Value { get; set; }
        public string Currency { get; set; }
        public string Assignment_Date { get; set; }

        public string Tax_Code { get; set; }
        public string CGST_Percentage { get; set; }
        public string CGST_RCM { get; set; }
        public string CGST { get; set; }
        public string IGST_Percentage { get; set; }
        public string IGST { get; set; }
        public string Import_IGST { get; set; }
        public string IGST_RCM { get; set; }
        public string SGST_Percentage { get; set; }
        public string SGST { get; set; }
        public string UGST_Percentage { get; set; }
        public string UGST { get; set; }
        public string UGST_RCM { get; set; }
        public string SGST_RCM { get; set; }
        public string Invoice_Number { get; set; }
        public string FG_Product_Code { get; set; }
        public string FG_Product_Name { get; set; }
        public string Ship_To_Party_MobileNumber { get; set; }
        public string Ship_To_Party_Region { get; set; }
        public string Ship_To_Party_Region_Desc { get; set; }
     


    }
}