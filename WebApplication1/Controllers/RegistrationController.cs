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
                string refid = IsEmailAlreadyExists(registration.email);
                if (refid != null )
                {
                    response.messageCode = "DE";
                    response.messageString = "You have registered with this email for User "+refid;
                    return response;
                    
                }

                // Check if phone number is already present
                string refid1 = IsPhoneNumberAlreadyExists(registration.phnum);
                if (refid1 != null)
                {
                    response.messageCode = "DP";
                    response.messageString = "You have registered with this phone number for User "+refid1;
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
                    response.messageString = "Branch Head already exists for this region/segment";
                    return response;

                }
                // Check if user is already present for the same region
                if (IsUserAlreadyExistsInRegion(registration.usertype, registration.segment))
                {
                    response.messageCode = "DR";
                    response.messageString = "Branch Head already exists for this region/segment";
                    return response;

                    
                }

                if (!IsPasswordLengthValid(registration.password))
                {
                    response.messageCode = "PL";
                    response.messageString = "Password must be between 5 and 20 characters.";
                    return response;
                }

                if (!IsPasswordComplex(registration.password))
                {
                    response.messageCode = "PC";
                    response.messageString = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (@#$^_()).";
                    return response;
                }

                // Check if user is already present with the same branch


                string sql = "insert into user_table (userid, usertype, firstname, lastname, password, refid, regioncode, email, phnum, isactive, region_desc, branchcode, segment, deactivationreason)   VALUES (@userid,@usertype,@firstname,@lastname,@password,@refid,@regioncode,@email,@phnum,@isactive,@region_desc, @branchcode, @segment, @deactivationreason)";
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
                    command.Parameters.AddWithValue("@deactivationreason", registration.deactivationreason);

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

        private string IsEmailAlreadyExists(string email)
        {
            // Perform a database query to check if the email already exists
            // Replace this with your actual database query logic
            using (var command = new MySqlCommand("SELECT refid FROM user_table WHERE email = @email AND isactive = 'X'" , _connection))
            {
                command.Parameters.AddWithValue("@email", email);
                object refid = command.ExecuteScalar();

                if (refid != null && refid != DBNull.Value)
                {
                    return refid.ToString(); // Return the refid as a string
                }
                else
                {
                    return null; // Return null if the email doesn't exist
                }
            }
        }

        private string IsPhoneNumberAlreadyExists(string phoneNumber)
        {
            // Perform a database query to check if the phone number already exists
            // Replace this with your actual database query logic
            using (var command = new MySqlCommand("SELECT refid FROM user_table WHERE phnum = @phnum AND isactive = 'X'", _connection))
            {
                command.Parameters.AddWithValue("@phnum", phoneNumber);
                object refid = command.ExecuteScalar();

                if (refid != null && refid != DBNull.Value)
                {
                    return refid.ToString(); // Return the refid as a string
                }
                else
                {
                    return null; // Return null if the email doesn't exist
                }
                
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

        private bool IsUserAlreadyExistsInRegion(string userType, string segment)
        {
            // Perform a database query to check if a user with the same user ID and region code already exists
            // Replace this with your actual database query logic
            if (userType == "C")
            {
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE usertype = 'C' AND segment = @segment AND isactive = 'X'", _connection))
                {

                    command.Parameters.AddWithValue("@segment", segment);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Return true if a duplicate user in the same region exists
                }
            }
            return false; // Return false for other user types
        }

        private bool IsUserAlreadyExistsInBranch(string userType, string branchCode, string segment)
        {
            // Perform a database query to check if a user with the same user ID and branch code already exists
            // Replace this with your actual database query logic
            if (userType == "C")
            {
                using (var command = new MySqlCommand("SELECT COUNT(*) FROM user_table WHERE usertype = 'C' AND branchcode = @branchcode AND segment = @segment AND isactive = 'X'", _connection))
                {
                    command.Parameters.AddWithValue("@segment", segment);
                    command.Parameters.AddWithValue("@branchcode", branchCode);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Return true if a duplicate user in the same branch exists
                }
            }
            return false;
        }

        private bool IsPasswordLengthValid(string password)
        {
            return password.Length >= 5 && password.Length <= 20;
        }

        private bool IsPasswordComplex(string password)
        {
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => "@#$^_()".Contains(c));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;

        }



    }
}