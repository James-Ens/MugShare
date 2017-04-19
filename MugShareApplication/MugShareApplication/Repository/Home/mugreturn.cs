using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MugShareApplication.Models;

namespace MugShareApplication.Repository.Home
{
    public class mugreturn
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -QueryProcessor()
                -UpdateMugStatusQuery()
                -UpdateUserStatusQuery()
                -GetUBC_ID()
                -GetMonthlyChartData()
                -GetYearlyChartData()

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
         *  Function: UpdateMugStatusQuery
         * 
         *  Creates a sql command string based on given parameters then sends string
         *  to a separate function to be executed by the server.
         *  
         *  Parameters:
         *  
         *      MugID - mug ID of mug being returned
         * 
         *  Returns:
         *  
         *      True - if record is created successfully
         *      False - if query fails
        */
        public static bool UpdateMugStatusQuery(string MugID)
        {
            string mugStatusQuery = "UPDATE MugRegistry SET " +
                "CurrentlyInUse = '" + false + "' " +
                "WHERE MugID = '" + MugID + "'";

            bool returnStatus = QueryProcessor(mugStatusQuery);
            return returnStatus;
        }

        /*
         *  Function: UpdateUserStatusQuery
         * 
         *  Creates a sql command string based on given parameters then sends string
         *  to a separate function to be executed by the server.
         *  
         *  Parameters:
         *  
         *      LastBorrowedBy - ubc ID of user who returned mug
         * 
         *  Returns:
         *  
         *      True - if record is created successfully
         *      False - if query fails
        */
        public static bool UpdateUserStatusQuery(string LastBorrowedBy)
        {
            string userStatusQuery = "UPDATE MugShareUsers SET " +
                "MugInUse = '" + false + "', " +
                "DateOfRental = NULL " +
                "WHERE StudentNumber = " + LastBorrowedBy;

            bool returnStatus = QueryProcessor(userStatusQuery);
            return returnStatus;
        }

        /*
         *  Function: GetUBC_ID
         * 
         *  Get UBC ID of user returning mug.
         * 
         *  Returns:
         *  
         *      LastBorrowedBy - UBC ID of user returning mug
        */
        public static string GetUBC_ID(string MugID)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT LastBorrowedBy FROM MugRegistry WHERE MugID = '" + MugID + "'";
            string LastBorrowedBy = null;
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
                    if (!string.IsNullOrEmpty(dbreader["LastBorrowedBy"].ToString())) { LastBorrowedBy = dbreader["LastBorrowedBy"].ToString(); }
                }
                return LastBorrowedBy;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetUBC_ID() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: GetMonthlyChartData
         * 
         *  Get total number of mugs borrowed for each month.
         * 
         *  Returns:
         *  
         *      model - total number of mugs borrowed for each month
        */
        public static MonthlyDataModel GetMonthlyChartData()
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MonthlyStatistics WHERE pKey = 1";
            MonthlyDataModel model = new MonthlyDataModel();
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
                    if (!string.IsNullOrEmpty(dbreader["January"].ToString())) { model.January = Convert.ToInt32(dbreader["January"]); }
                    if (!string.IsNullOrEmpty(dbreader["February"].ToString())) { model.February = Convert.ToInt32(dbreader["February"]); }
                    if (!string.IsNullOrEmpty(dbreader["March"].ToString())) { model.March = Convert.ToInt32(dbreader["March"]); }
                    if (!string.IsNullOrEmpty(dbreader["April"].ToString())) { model.April = Convert.ToInt32(dbreader["April"]); }
                    if (!string.IsNullOrEmpty(dbreader["May"].ToString())) { model.May = Convert.ToInt32(dbreader["May"]); }
                    if (!string.IsNullOrEmpty(dbreader["June"].ToString())) { model.June = Convert.ToInt32(dbreader["June"]); }
                    if (!string.IsNullOrEmpty(dbreader["July"].ToString())) { model.July = Convert.ToInt32(dbreader["July"]); }
                    if (!string.IsNullOrEmpty(dbreader["August"].ToString())) { model.August = Convert.ToInt32(dbreader["August"]); }
                    if (!string.IsNullOrEmpty(dbreader["September"].ToString())) { model.September = Convert.ToInt32(dbreader["September"]); }
                    if (!string.IsNullOrEmpty(dbreader["October"].ToString())) { model.October = Convert.ToInt32(dbreader["October"]); }
                    if (!string.IsNullOrEmpty(dbreader["November"].ToString())) { model.November = Convert.ToInt32(dbreader["November"]); }
                    if (!string.IsNullOrEmpty(dbreader["December"].ToString())) { model.December = Convert.ToInt32(dbreader["December"]); }
                }
                return model;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetMonthlyChartData() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: GetYearlyChartData
         * 
         *  Get a list of records containing number of mugs borrowed for each year.
         * 
         *  Returns:
         *  
         *      list - list of records containing number of mugs borrowed for each year
        */
        public static List<YearlyDataModel> GetYearlyChartData()
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM YearlyStatistics";
            List<YearlyDataModel> list = new List<YearlyDataModel>();
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
                    YearlyDataModel model = new YearlyDataModel();
                    if (!string.IsNullOrEmpty(dbreader["Year"].ToString())) { model.Year = dbreader["Year"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["TotalMugsBorrowed"].ToString())) { model.TotalMugsBorrowed = Convert.ToInt32(dbreader["TotalMugsBorrowed"]); }

                    list.Add(model);
                }
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetYearlyChartData() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }
    }
}