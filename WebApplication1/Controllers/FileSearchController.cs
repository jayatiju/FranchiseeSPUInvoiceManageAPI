/*
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
using System.IO;
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/FileSearch")]
    public class FileSearchController : ApiController
    {
        private const string BasePath = @"C:\spuusrftp"; // Updated Base directory

        public class FileSearchRequest
        {
            public int Year { get; set; }
            public string Month { get; set; }
            public string Segment { get; set; }
            public string VendorCode { get; set; }
            public string DigitallySigned { get; set; }
        }

        [HttpPost]
        [Route("search-files")]
        public IHttpActionResult GetFiles([FromBody] FileSearchRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request payload");

                // Determine folder based on digital signature flag
                string folderType = request.DigitallySigned == "X" ? "Franchisee_digital_signature" : "Franchisee_unsigned_files";

                // Construct the correct folder path
                string folderPath = Path.Combine(BasePath, folderType, request.Year.ToString(), request.Month, request.Segment);

                if (!Directory.Exists(folderPath))
                {
                    return Content(HttpStatusCode.NotFound, "Directory not found.");
                }
                
                string vendorCode = request.VendorCode.TrimStart('0');
                Console.WriteLine($"Vendor Code after trimming: {vendorCode}");



                // Get files matching the VendorCode pattern
                var files = Directory.GetFiles(folderPath)
                                    .Select(file => new { FileName = Path.GetFileName(file), FilePath = file })
                                   .Where(f => Regex.IsMatch(f.FileName, $"^{request.VendorCode}.*")) // Match files with VendorCode at the beginning
                                 .ToList();
                //var allFiles = Directory.GetFiles(folderPath)
                  //      .Select(file => new { FileName = Path.GetFileName(file), FilePath = file })
                    //    .ToList();

               // return Ok(JsonConvert.SerializeObject(allFiles)); // Return all files before filtering

                return Ok(JsonConvert.SerializeObject(files));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
*/

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
using System.IO;
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/FileSearch")]
    public class FileSearchController : ApiController
    {
        private const string BasePath = @"C:\spuusrftp"; // Updated Base directory

        public class FileSearchRequest
        {
            public int Year { get; set; }
            public string Month { get; set; }
            public string Segment { get; set; }
            public string VendorCode { get; set; }
            public string DigitallySigned { get; set; }
        }

        [HttpPost]
        [Route("search-files")]
        public IHttpActionResult GetFiles([FromBody] FileSearchRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request payload");

                // Determine folder based on digital signature flag
                string folderType = request.DigitallySigned == "X" ? "Franchisee_digital_signature" : "Franchisee_unsigned_files";

                // Construct the correct folder path
                string folderPath = Path.Combine(BasePath, folderType, request.Year.ToString(), request.Month, request.Segment);

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

                // Debugging: Print all filenames
                Console.WriteLine("=== All Files in Directory ===");
                foreach (var file in allFiles)
                {
                    Console.WriteLine(file.FileName);
                }

                // Apply filtering based on VendorCode
                var files = allFiles.Where(f => f.FileName.StartsWith(vendorCode)).ToList();

                // Debugging: Print matched files
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
    }
}
