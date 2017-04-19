using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MugShareApplication.Models;

namespace MugShareApplication.Repository.Datatables
{
    public class mugshareusers
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -GetList()
                -GetRecord()
                -QueryProcessor()
                -CreateQuery()
                -EditQuery()
                -DeleteQuery()
                -StudentNumberValidator()
                -EmailValidator()

        ----------------------------------------------------------------------------------------*/
        /*
         *  Function: GetList
         * 
         *  Get data for Mug-Share users table display on Mug Share Users page.
         * 
         *  Returns:
         *  
         *      list - a list of UsersDatatableModel models which contain each record's
         *             student number, first name, last name and buttons connected to their
         *             respective pKey
        */
        public static List<UsersDatatableModel> GetList()
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MugShareUsers ORDER BY LastName";
            List<UsersDatatableModel> list = new List<UsersDatatableModel>();
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
                    UsersDatatableModel model = new UsersDatatableModel();
                    if (!string.IsNullOrEmpty(dbreader["StudentNumber"].ToString())) { model.StudentNumber = dbreader["StudentNumber"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["FirstName"].ToString())) { model.FirstName = dbreader["FirstName"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["LastName"].ToString())) { model.LastName = dbreader["LastName"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["TotalMugsBorrowed"].ToString())) { model.TotalMugsBorrowed = dbreader["TotalMugsBorrowed"].ToString(); }

                    string pKey = dbreader["pKey"].ToString();

                    model.buttons = "<div class='row'><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='MSU_read(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#MSU_read'>View</a></div>" +
                                    "<div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='MSU_edit(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#MSU_edit'>Edit</a></div>";

                    if (Controllers.BaseController.SessionStorage.AdminPermission == "Admin")
                    {
                        model.buttons += "<div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='MSU_delete(\"" + pKey + "\")' class='btn btn-danger btn-sm' data-toggle='modal' data-target='#MSU_delete'>Delete</a></div></div>";
                    }
                    else
                    {
                        model.buttons += "</div>";
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
         *  Get data for mug share users modals.
         * 
         *  Returns:
         *  
         *      model - a UsersModalModel model of the record with the primary key equal to the UserKey
        */
        public static UsersModalModel GetRecord(string UserKey)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MugShareUsers WHERE pKey = " + UserKey;
            UsersModalModel model = new UsersModalModel();
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
                    if (!string.IsNullOrEmpty(dbreader["pKey"].ToString())) { model.UserKey = dbreader["pKey"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["StudentNumber"].ToString())) { model.StudentNumber = dbreader["StudentNumber"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["FirstName"].ToString())) { model.FirstName = dbreader["FirstName"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["LastName"].ToString())) { model.LastName = dbreader["LastName"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["Email"].ToString())) { model.Email = dbreader["Email"].ToString(); }

                    if (dbreader["MugInUse"].ToString() == "True") { model.MugInUse = true; }
                    else model.MugInUse = false;

                    if (!string.IsNullOrEmpty(dbreader["DateOfRental"].ToString()))
                    {
                        string temp = dbreader["DateOfRental"].ToString();
                        model.DateOfRental = temp.Substring(0, temp.Length - 12);
                    }

                    if (!string.IsNullOrEmpty(dbreader["TotalMugsBorrowed"].ToString())) { model.TotalMugsBorrowed = dbreader["TotalMugsBorrowed"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["Notes"].ToString())) { model.Notes = dbreader["Notes"].ToString(); }
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
         *      StudentNumber - Student Number of new user
         *      FirstName - First Name of new user
         *      LastName - Last Name of new user
         *      Email - Email of new user
         *      Notes - Special notes about the user
         * 
         *  Returns:
         *  
         *      True - if record is created successfully
         *      False - if query fails
        */
        public static bool CreateQuery(string StudentNumber, string FirstName, string LastName, string Email, string Notes)
        {
            string createQuery = "INSERT INTO MugShareUsers (StudentNumber, FirstName, LastName, Email, MugInUse, TotalMugsBorrowed, Notes)" +
                " VALUES (" + StudentNumber + ", '" + FirstName + "', '" + LastName + "', '" + Email + "', 'false', 0, " + Notes + ")";

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
         *      UserKey - the primary key of the user record being updated
         *      StudentNumber - Student Number of user record
         *      FirstName - First Name of user record
         *      LastName - Last Name of user record
         *      Email - Email of user record
         *      MugInUse - status of whether a user is currently renting a mug or not
         *      DateOfRental - date of mug rental (if currently renting a mug)
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool EditQuery(string UserKey, string StudentNumber, string FirstName, string LastName, string Email,
            bool MugInUse, string DateOfRental, string Notes)
        {
            string editQuery = "UPDATE MugShareUsers SET " +
                "StudentNumber = " + StudentNumber + ", " +
                "FirstName = '" + FirstName + "', " +
                "LastName = '" + LastName + "', " +
                "Email = '" + Email + "', " +
                "MugInUse = '" + MugInUse + "', " +
                "DateOfRental = " + DateOfRental + ", " +
                "Notes = " + Notes + " " +
                "WHERE pKey = " + UserKey;

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
         *      UserKey - the primary key of the user record being deleted
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool DeleteQuery(string UserKey)
        {
            string deleteQuery = "DELETE FROM MugShareUsers WHERE pKey = '" + UserKey + "'";
            bool deleteStatus = QueryProcessor(deleteQuery);
            return deleteStatus;
        }

        /*
         *  Function: StudentNumberValidator
         * 
         *  Check to see if student number is unique.
         * 
         *  Returns:
         *  
         *      True - if the student number does not exist in the MugShareUsers datatable
         *      False - if the student number exists in the MugShareUsers datatable
        */
        public static bool StudentNumberValidator(string StudentNumber)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MugShareUsers WHERE StudentNumber = " + StudentNumber;
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
                throw new Exception(@"Mug-Share Application StudentNumberValidator() failed : ", e);
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
            string queryString = "SELECT * FROM MugShareUsers WHERE Email = '" + Email + "'";
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