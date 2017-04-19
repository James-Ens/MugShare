using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MugShareApplication.Models;
using MugShareApplication.Controllers;

namespace MugShareApplication.Repository.Datatables
{
    public class security
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -GetList()
                -GetRecord()
                -QueryProcessor()
                -CreateQuery()
                -EditQuery()
                -DeleteQuery()
                -StaffCardIDValidator()
                -UsernameValidator()
                -EmailValidator()

        ----------------------------------------------------------------------------------------*/
        /*
         *  Function: GetList
         * 
         *  Get data for security table display on Security page.
         * 
         *  Returns:
         *  
         *      list - a list of SecurityDatatableModel models which contain each record's
         *             username, email, admin permission status and buttons connected to their
         *             respective pKey
        */
        public static List<SecurityDatatableModel> GetList()
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM Security ORDER BY LastName";
            List<SecurityDatatableModel> list = new List<SecurityDatatableModel>();
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                while (dbreader.Read())
                {
                    SecurityDatatableModel model = new SecurityDatatableModel();
                    if (!string.IsNullOrEmpty(dbreader["Username"].ToString())) { model.Username = StringCipher.Decrypt(dbreader["Username"].ToString()); }
                    //if (!string.IsNullOrEmpty(dbreader["Username"].ToString())) { model.Username = dbreader["Username"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["Email"].ToString())) { model.Email = dbreader["Email"].ToString(); }
                    
                    if (dbreader["AdminPermission"].ToString() == "True") { model.AdminPermission = "<div class='text-center'><span class='glyphicon glyphicon-ok' style='color:green'></span></div>"; }
                    else model.AdminPermission = "<div class='text-center'><span class='glyphicon glyphicon-remove' style='color:red'></span></div>";

                    string pKey = dbreader["pKey"].ToString();

                    if (Controllers.BaseController.SessionStorage.Username != model.Username)
                    {
                        model.buttons = "<div class='row'><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='S_read(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#S_read'>View</a>" +
                                    "</div><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='S_edit(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#S_edit'>Edit</a>" +
                                    "</div><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='S_delete(\"" + pKey + "\")' class='btn btn-danger btn-sm' data-toggle='modal' data-target='#S_delete'>Delete</a>" +
                                    "</div></div>";
                    }
                    else
                    {
                        model.buttons = "<div class='row'><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='S_read(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#S_read'>View</a>" +
                                    "</div></div>";
                    }

                    list.Add(model);
                }
                return (list);
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetList() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: GetRecord
         * 
         *  Get data for security modals.
         * 
         *  Returns:
         *  
         *      model - a SecurityModalModel model of the record with the primary key equal to the SecurityKey
        */
        public static SecurityModalModel GetRecord(string SecurityKey)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM Security WHERE pKey = " + SecurityKey;
            SecurityModalModel model = new SecurityModalModel();
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                if (dbreader.HasRows)
                {
                    dbreader.Read();
                    if (!string.IsNullOrEmpty(dbreader["pKey"].ToString())) { model.SecurityKey = dbreader["pKey"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["FirstName"].ToString())) { model.FirstName = dbreader["FirstName"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["LastName"].ToString())) { model.LastName = dbreader["LastName"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["StaffCardID"].ToString())) { model.StaffCardID = dbreader["StaffCardID"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["Username"].ToString())) { model.Username = StringCipher.Decrypt(dbreader["Username"].ToString()); }
                    if (!string.IsNullOrEmpty(dbreader["Email"].ToString())) { model.Email = dbreader["Email"].ToString(); }

                    if (dbreader["AdminPermission"].ToString() == "True") { model.AdminPermission = true; }
                    else model.AdminPermission = false;
                }
                return model;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetRecord() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: QueryProcessor
         * 
         *  Used to process create, edit and delete queries.
         *  
         *  Parameters:
         *  
         *      queryString - the sql command string to be executed by the server
         * 
         *  Returns:
         *  
         *      True - if query is processed successfully and one row is affected
         *      False - if query is not processed successfully and zero rows are affected
        */
        public static bool QueryProcessor(string queryString)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                return (dbcommand.ExecuteNonQuery() > 0);
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application QueryProcessor() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: CreateQuery
         * 
         *  Creates a sql command string based on given parameters then sends string
         *  to a separate function to be executed by the server.
         *  
         *  Parameters:
         *  
         *      FirstName - First Name of new staff user
         *      LastName - Last Name of new staff user
         *      Username - Username of new staff user
         *      StaffCardID - Staff Card ID of new staff user
         *      Password - Password of new staff user
         *      Email - Email of new staff user
         *      AdminPermission - detemines the new staff user's level of security
         * 
         *  Returns:
         *  
         *      True - if record is created successfully
         *      False - if query fails
        */
        public static bool CreateQuery(string FirstName, string LastName, string StaffCardID, string Username, string Password, string Email, bool AdminPermission)
        {            
            string createQuery = "INSERT INTO Security (FirstName, LastName, StaffCardID, Username, Password, Email, AdminPermission)" +
                " VALUES ('" + FirstName + "', '" + LastName + "', " + StaffCardID + ", '" + Username + "', '" + Password + "', '" + Email + "', '" + AdminPermission + "')";

            bool createStatus = QueryProcessor(createQuery);
            return createStatus;
        }

        /*
         *  Function: EditQuery
         * 
         *  Creates a sql command string based on given parameters then sends string
         *  to a separate function to be executed by the server.
         *  
         *  Parameters:
         *  
         *      SecurityKey - the primary key of the security record being updated
         *      FirstName - First Name of staff user
         *      LastName - Last Name of staff user
         *      StaffCardID - Staff Card ID of staff user
         *      Username - Username of staff user
         *      Password - Password of staff user
         *      Email - Email of staff user
         *      AdminPermission - detemines the staff user's level of security
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool EditQuery(string SecurityKey, string FirstName, string LastName, string StaffCardID, string Username, string Email, bool AdminPermission)
        {
            string editQuery = "UPDATE Security SET " +
                "FirstName = '" + FirstName + "', " +
                "LastName = '" + LastName + "', " +
                "StaffCardID = " + StaffCardID + ", " +
                "Username = '" + Username + "', " +
                "Email = '" + Email + "', " +
                "AdminPermission = '" + AdminPermission + "' " +
                "WHERE pKey = " + SecurityKey;

            bool editStatus = QueryProcessor(editQuery);
            return editStatus;
        }

        /*
         *  Function: DeleteQuery
         * 
         *  Creates a sql command string based on given parameters then sends string
         *  to a separate function to be executed by the server.
         *  
         *  Parameters:
         *  
         *      SecurityKey - the primary key of the security record being deleted
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool DeleteQuery(string SecurityKey)
        {
            string deleteQuery = "DELETE FROM Security WHERE pKey = '" + SecurityKey + "'";
            bool deleteStatus = QueryProcessor(deleteQuery);
            return deleteStatus;
        }

        /*
         *  Function: StaffCardIDValidator
         * 
         *  Check to see if staff card ID is unique.
         * 
         *  Returns:
         *  
         *      True - if the staff card ID does not exist in the Security datatable
         *      False - if the staff card ID exists in the Security datatable
        */
        public static bool StaffCardIDValidator(string StaffCardID)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM Security WHERE StaffCardID = " + StaffCardID;
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                if (dbreader.HasRows)
                    return false;
                else
                    return true;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application StaffCardIDValidator() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: UsernameValidator
         * 
         *  Check to see if username is unique.
         * 
         *  Returns:
         *  
         *      True - if the student number does not exist in the MugShareUsers datatable
         *      False - if the student number exists in the MugShareUsers datatable
        */
        public static bool UsernameValidator(string Username)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT Username FROM Security";
            string decryptUsername = null;
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                while (dbreader.Read())
                {
                    if (!string.IsNullOrEmpty(dbreader["Username"].ToString())) { decryptUsername = StringCipher.Decrypt(dbreader["Username"].ToString()); }

                    if (Username == decryptUsername) { return false; }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application UsernameValidator() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: EmailValidator
         * 
         *  Check to see if email is unique.
         * 
         *  Returns:
         *  
         *      True - if the email does not exist in the MugShareUsers datatable
         *      False - if the email exists in the MugShareUsers datatable
        */
        public static bool EmailValidator(string Email)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM Security WHERE Email = '" + Email + "'";
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                if (dbreader.HasRows)
                    return false;
                else
                    return true;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application EmailValidator() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }
    }
}