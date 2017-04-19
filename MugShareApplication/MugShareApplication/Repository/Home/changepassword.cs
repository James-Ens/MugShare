using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MugShareApplication.Models;
using MugShareApplication.Controllers;

namespace MugShareApplication.Repository.Home
{
    public class changepassword
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -QueryProcessor()
                -UpdatePasswordQuery()

        ----------------------------------------------------------------------------------------*/
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
         *  Function: UpdatePasswordQuery
         * 
         *  Creates a sql command string based on given parameters then sends string
         *  to a separate function to be executed by the server.
         *  
         *  Parameters:
         *  
         *      Password - password to be updated to
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool UpdatePasswordQuery(string Email, string Password)
        {
            string passwordQuery = "UPDATE Security SET " +
                "Password = '" + Password + "' " +
                "WHERE Email = '" + Email + "'";

            bool updatePasswordStatus = QueryProcessor(passwordQuery);
            return updatePasswordStatus;
        }
    }
}