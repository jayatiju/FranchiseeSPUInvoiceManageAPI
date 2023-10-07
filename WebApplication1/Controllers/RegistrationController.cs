using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using MySql.Data.MySqlClient;
using WebApplication1.Models;
using System.Windows;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    [EnableCors("*", "*", "*")] // Enable CORS for the entire controller
    public class RegistrationController : ApiController
    {
        
        ResponseCode response = new ResponseCode();
        private readonly MySqlConnection _connection;
        
        public RegistrationController()
        {
            try
            {
                _connection = new MySqlConnection(ConnectionString.connString);
                _connection.Open();
            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                response.messageCode = "E";
                response.messageString = ex.Message;
            }
        }

        [HttpPost]
        public ResponseCode Post([FromBody] Registration registration)
        {
            
            // Save the product to the database.
           
            try
            {
                /*
                if(CheckForMultipleCustomersForSameRegion(registration.usertype, registration.regioncode))
                {
                    response.messageCode = "MCR";
                    response.messageString = "Region Head for the region already exists";
                    return response;
                }
                */
                if (IsDeactivatedUserWithSameEmailAlreadyExists(registration.email))
                {
                    response.messageCode = "DDE";
                    response.messageString = "Deactivated user with same emailid already exists";
                    return response;

                }


                // Check if email is already present
                if (IsEmailAlreadyExists(registration.email))
                {
                    response.messageCode = "DE";
                    response.messageString = "Duplicate email";
                    return response;
                    
                }

                // Check if phone number is already present
                if (IsPhoneNumberAlreadyExists(registration.phnum))
                {
                    response.messageCode = "DP";
                    response.messageString = "Duplicate phone";
                    return response;
                    
                }

                // Check if user is already present with the same vendor
                if (IsDuplicateVendor(registration.usertype, registration.refid))
                {
                    response.messageCode = "DV";
                    response.messageString = "Duplicate vendor";
                    return response;
                    
                }
                if (IsUserAlreadyExistsInBranch(registration.usertype, registration.branchcode, registration.regioncode))
                {
                    response.messageCode = "DC";
                    response.messageString = "Segment Head already exists for this region";
                    return response;

                }
                // Check if user is already present for the same region
                if (IsUserAlreadyExistsInRegion(registration.usertype, registration.regioncode))
                {
                    response.messageCode = "DR";
                    response.messageString = "Segment Head already exists for this region";
                    return response;

                    
                }

                // Check if user is already present with the same branch
                

                string sql = "insert into user_table (userid, usertype, firstname, lastname, password, refid, regioncode, email, phnum, isactive, region_desc, branchcode, segment)   VALUES (@userid,@usertype,@firstname,@lastname,@password,@refid,@regioncode,@email,@phnum,@isactive,@region_desc, @branchcode, @segment)";
                using (var command = new MySqlCommand(sql, _connection))
                {
                    command.Parameters.AddWithValue("@userid", registration.userid);
                    command.Parameters.AddWithValue("@usertype", registration.usertype);
                    command.Parameters.AddWithValue("@firstname", registration.firstname);
                    command.Parameters.AddWithValue("@lastname", registration.lastname);
                    command.Parameters.AddWithValue("@password", registration.password);
                    command.Parameters.AddWithValue("@refid", registration.refid);
                    command.Parameters.AddWithValue("@regioncode", registration.regioncode);
                    command.Parameters.AddWithValue("@email", registration.email);
                    command.Parameters.AddWithValue("@phnum", registration.phnum);
                    command.Parameters.AddWithValue("@isactive", registration.isactive);
                    command.Parameters.AddWithValue("@region_desc", registration.region_desc);
                    command.Parameters.AddWithValue("@branchcode", registration.branchcode);
                    command.Parameters.AddWithValue("@segment", registration.segment);

                    command.ExecuteNonQuery();

                    
                    response.messageCode = "S";
                    response.messageString = "Successfully registered user";

                }

            }
            catch (Exception ex)
            {
                response.messageCode = "E";
                response.messageString = ex.Message;
                
            }

            finally
            {
                _connection.Close();
            }
            return response; ;
            
        }
        /*
        private bool CheckForMultipleCustomersForSameRegion(string usertype, string regioncode)
        {
            // Perform a database query to check if the email already exists
            // Replace this with your actual database query logic
            if (usertype.Equals('C'))
            {
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE usertype = @usertype AND regioncode = @regioncode", _connection))
                {
                    command.Parameters.AddWithValue("@usertype", usertype);
                    command.Parameters.AddWithValue("@regioncode", regioncode);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Return true if email already exists
                }
            }
            return false;
        }
        */
        private bool IsDeactivatedUserWithSameEmailAlreadyExists(string email)
        {
            // Perform a database query to check if the email already exists
            // Replace this with your actual database query logic
            using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE email = @email AND isactive = ''", _connection))
            {
                command.Parameters.AddWithValue("@email", email);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0; // Return true if email already exists
            }
        }

        private bool IsEmailAlreadyExists(string email)
        {
            // Perform a database query to check if the email already exists
            // Replace this with your actual database query logic
            using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE email = @email AND isactive = 'X'" , _connection))
            {
                command.Parameters.AddWithValue("@email", email);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0; // Return true if email already exists
            }
        }

        private bool IsPhoneNumberAlreadyExists(string phoneNumber)
        {
            // Perform a database query to check if the phone number already exists
            // Replace this with your actual database query logic
            using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE phnum = @phnum AND isactive = 'X'", _connection))
            {
                command.Parameters.AddWithValue("@phnum", phoneNumber);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0; // Return true if phone number already exists
            }
        }

        private bool IsDuplicateVendor(string userType, string refid)
        {
            if (userType == "V")
            {
                // Perform a database query to check if a vendor with the same user ID already exists
                // Replace this with your actual database query logic
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE  usertype = 'V' AND refid = @refid AND isactive = 'X'", _connection))
                {
                    
                    command.Parameters.AddWithValue("@refid", refid);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Return true if a duplicate vendor exists
                }
            }

            return false; // Return false for other user types
        }

        private bool IsUserAlreadyExistsInRegion(string userType, string regionCode)
        {
            // Perform a database query to check if a user with the same user ID and region code already exists
            // Replace this with your actual database query logic
            if (userType == "C")
            {
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE usertype = 'C' AND regioncode = @regioncode AND isactive = 'X'", _connection))
                {

                    command.Parameters.AddWithValue("@regioncode", regionCode);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Return true if a duplicate user in the same region exists
                }
            }
            return false; // Return false for other user types
        }

        private bool IsUserAlreadyExistsInBranch(string userType, string branchCode, string regionCode)
        {
            // Perform a database query to check if a user with the same user ID and branch code already exists
            // Replace this with your actual database query logic
            if (userType == "C")
            {
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE usertype = 'C' AND branchcode = @branchcode AND regioncode = @regioncode AND isactive = 'X'", _connection))
                {
                    command.Parameters.AddWithValue("@regioncode", regionCode);
                    command.Parameters.AddWithValue("@branchcode", branchCode);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Return true if a duplicate user in the same branch exists
                }
            }
            return false;
        }

        
    }
}