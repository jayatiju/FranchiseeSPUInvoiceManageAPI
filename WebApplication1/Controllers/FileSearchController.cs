using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using WebApplication1.Models; // Import the Models namespace
using System.Globalization; // Needed for month conversion
using System.Web.Http.Cors;
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/FileSearch")]
    public class FileSearchController : ApiController
    {
        private const string BasePath = @"C:\spuusrftp"; // Updated Base directory

        [HttpPost]
        [Route("search-files")]
        public IHttpActionResult GetFiles([FromBody] FileSearchRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request payload");

                // Convert numeric month (01-20) to textual month (Jan-20)
                string formattedMonth = ConvertNumericMonthToText(request.Month);

                if (formattedMonth == null)
                {
                    return BadRequest("Invalid month format. Please use MM-YY format, e.g., 01-20.");
                }

                // Determine folder based on digital signature flag
                string folderType = request.DigitallySigned == "X" ? "Franchisee_digital_signature" : "Franchisee_unsigned_files";

                // Construct the correct folder path
                string folderPath = Path.Combine(BasePath, folderType, request.Year.ToString(), formattedMonth, request.Segment);

                if (!Directory.Exists(folderPath))
                {
                    return Content(HttpStatusCode.NotFound, "Directory not found.");
                }

                // Trim leading zeros from VendorCode
                string vendorCode = request.VendorCode.TrimStart('0');
                Console.WriteLine($"Vendor Code after trimming: {vendorCode}");

                // Fetch all files from the directory
                var allFiles = Directory.GetFiles(folderPath)
                                        .Select(file => new { FileName = Path.GetFileName(file), FilePath = file })
                                        .ToList();

                
                Console.WriteLine("=== All Files in Directory ===");
                foreach (var file in allFiles)
                {
                    Console.WriteLine(file.FileName);
                }

                // Apply filtering based on VendorCode
                var files = allFiles.Where(f => f.FileName.StartsWith(vendorCode)).ToList();

                
                Console.WriteLine("=== Matching Files ===");
                foreach (var file in files)
                {
                    Console.WriteLine($"Matched: {file.FileName}");
                }

                return Ok(JsonConvert.SerializeObject(files));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        
        private string ConvertNumericMonthToText(string monthInput)
        {
            try
            {
                if (!Regex.IsMatch(monthInput, @"^\d{2}-\d{2}$")) // Validate format MM-YY
                    return null;

                string[] parts = monthInput.Split('-');
                int monthNumber = int.Parse(parts[0]); // Extract month number
                string yearPart = parts[1]; // Extract year part (e.g., "20")

                if (monthNumber < 1 || monthNumber > 12)
                    return null; // Invalid month number

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthNumber); // Convert to "Jan"
                return $"{monthName}-{yearPart}"; // Format as "Jan-20"
            }
            catch
            {
                return null; // Return null if conversion fails
            }
        }
    }
}
