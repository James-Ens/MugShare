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
    public class locationsupply
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -GetList()
                -GetRecord()
                -GetHourlyChartData()
                -QueryProcessor()
                -CreateQuery()
                -EditQuery()
                -DeleteQuery()
                -MachineIDValidator()

        ----------------------------------------------------------------------------------------*/
        /*
         *  Function: GetList
         * 
         *  Get data for location supply table display on Location Supply page.
         * 
         *  Returns:
         *  
         *      list - a list of LocationDatatableModel models which contain each record's
         *             machine id, machine location, current supply percentage, out of order status
         *             and buttons connected to their respective pKey
        */
        public static List<LocationDatatableModel> GetList()
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM LocationSupply ORDER BY MachineID";

            List<LocationDatatableModel> list = new List<LocationDatatableModel>();
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
                    LocationDatatableModel model = new LocationDatatableModel();
                    if (!string.IsNullOrEmpty(dbreader["MachineID"].ToString())) { model.MachineID = dbreader["MachineID"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["MachineLocation"].ToString())) { model.MachineLocation = dbreader["MachineLocation"].ToString(); }

                    //Only implemented to input into chart button
                    if (!string.IsNullOrEmpty(dbreader["OpeningHour"].ToString())) { model.OpeningHour = Convert.ToInt32(dbreader["OpeningHour"]); }
                    if (!string.IsNullOrEmpty(dbreader["ClosingHour"].ToString())) { model.ClosingHour = Convert.ToInt32(dbreader["ClosingHour"]); }

                    if (!string.IsNullOrEmpty(dbreader["CurrentSupply"].ToString())) { model.CurrentSupplyPercentage = dbreader["CurrentSupply"].ToString() + "%"; }

                    if (dbreader["OutOfOrder"].ToString() == "True") { model.OutOfOrder = "<div class='text-center'><span class='glyphicon glyphicon-remove' style='color:red'></span></div>"; }
                    else model.OutOfOrder = "<div class='text-center'><span class='glyphicon glyphicon-ok' style='color:green'></span></div>";

                    string pKey = dbreader["pKey"].ToString();

                    model.buttons = "<div class='row'><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='LS_read(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#LS_read'>View</a></div>" +
                                    "<div class='col-sm-2 text-center'>" +
                                    "<a onclick='LS_chart(\"" + model.MachineID + "\", " + model.OpeningHour + ", " + model.ClosingHour + ")'" +
                                    " class='btn btn-primary btn-sm' data-toggle='modal' data-target='#LS_chart'>Chart</a></div><div class='col-sm-2 text-center'>" +
                                    "<a onclick='LS_edit(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#LS_edit'>Edit</a></div>";

                    if (Controllers.BaseController.SessionStorage.AdminPermission == "Admin")
                    {
                        model.buttons += "<div class='col-sm-2 text-center'>" +
                                    "<a onclick='LS_delete(\"" + pKey + "\")' class='btn btn-danger btn-sm' data-toggle='modal' data-target='#LS_delete'>Delete</a></div></div>";
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
         *  Get data for location supply modals.
         * 
         *  Returns:
         *  
         *      model - a LocationModalModel model of the record with the primary key equal to the MachineKey
        */
        public static LocationModalModel GetRecord(string MachineKey)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM LocationSupply WHERE pKey = " + MachineKey;
            LocationModalModel model = new LocationModalModel();
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
                    if (!string.IsNullOrEmpty(dbreader["pKey"].ToString())) { model.MachineKey = dbreader["pKey"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["MachineID"].ToString())) { model.MachineID = dbreader["MachineID"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["MachineLocation"].ToString())) { model.MachineLocation = dbreader["MachineLocation"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["OpeningHour"].ToString())) { model.OpeningHour = Convert.ToInt32(dbreader["OpeningHour"]); }
                    if (!string.IsNullOrEmpty(dbreader["ClosingHour"].ToString())) { model.ClosingHour = Convert.ToInt32(dbreader["ClosingHour"]); }
                    if (!string.IsNullOrEmpty(dbreader["CurrentSupply"].ToString())) { model.CurrentSupply = dbreader["CurrentSupply"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["TotalCapacity"].ToString())) { model.TotalCapacity = dbreader["TotalCapacity"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["TotalMugsDispensed"].ToString())) { model.TotalMugsDispensed = dbreader["TotalMugsDispensed"].ToString(); }

                    if (dbreader["OutOfOrder"].ToString() == "True") { model.OutOfOrder = true; }
                    else model.OutOfOrder = false;

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
         *  Function: GetHourlyChartData
         * 
         *  Get number of mugs borrowed during each hour for a specified machine.
         * 
         *  Returns:
         *  
         *      list - list of records containing number of mugs borrowed during each hour for a specified machine
        */
        public static List<HourlyDataModel> GetHourlyChartData(string MachineID, int openingHour, int closingHour)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM HourlyStatistics WHERE MachineID = '" + MachineID + "'";
            List<HourlyDataModel> list = new List<HourlyDataModel>();
            string hourString = null;
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                if (dbreader.Read())
                {
                    while (openingHour <= closingHour)
                    {
                        HourlyDataModel model = new HourlyDataModel();
                        model.Hour = openingHour.ToString();

                        if (openingHour <= 9)
                        {
                            hourString = "0" + openingHour.ToString();
                        }
                        else
                        {
                            hourString = openingHour.ToString();
                        }

                        if (!string.IsNullOrEmpty(dbreader[hourString].ToString())) { model.TotalMugsBorrowed = Convert.ToInt32(dbreader[hourString]); }

                        openingHour++;
                        list.Add(model);
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetHourlyChartData() failed : ", e);
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
         *      MachineID - Machine ID of new machine
         *      MachineLocation - Location of new machine
         *      OpeningHour - Hour of the day that the machine is open for use
         *      ClosingHour - Hour of the day that the maching is closed from use
         *      TotalCapacity - Total capacity of new machine
         *      OutOfOrder - Out of order status of machine
         *      Notes - Special notes for this machine
         * 
         *  Returns:
         *  
         *      True - if record is created successfully
         *      False - if query fails
        */
        public static bool CreateQuery(string MachineID, string MachineLocation, int OpeningHour, int ClosingHour, string TotalCapacity, bool OutOfOrder, string Notes)
        {
            // query for creating new record in the LocationSupply table
            string createLocationQuery = "INSERT INTO LocationSupply (MachineID, MachineLocation, OpeningHour, ClosingHour, CurrentSupply, TotalCapacity, TotalMugsDispensed, OutOfOrder, Notes)" +
                " VALUES ('" + MachineID + "', '" + MachineLocation + "', " + OpeningHour + ", " + ClosingHour + ", 0, " + TotalCapacity + ", 0, '" + OutOfOrder + "', " + Notes + ")";

            // query for creating new record in HourlyStatistics table for the new machine
            string createHourlyQuery = "INSERT INTO HourlyStatistics (MachineID, [00], [01], [02], [03], [04], [05], [06], [07], [08], [09], [10], [11], [12], [13], [14], [15], [16], [17], [18], [19], [20], [21], [22], [23])" +
                " VALUES ('" + MachineID + "', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";

            bool createStatus = QueryProcessor(createLocationQuery);

            //if query for creating new record in LocationSupply table was successful, run the query for the HourlyStatistics table
            if (createStatus)
            {
                createStatus = QueryProcessor(createHourlyQuery);
            }

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
         *      MachineKey - the primary key of the user record being updated
         *      MachineID - Machine ID of machine
         *      MachineLocation - Location of machine
         *      OpeningHour - Hour of the day that the machine is open for use
         *      ClosingHour - Hour of the day that the maching is closed from use
         *      CurrentSupply - Current supply of machine
         *      TotalCapacity - Total capacity of machine
         *      OutOfOrder - Out of order status of machine
         *      Notes - Special notes for this machine
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool EditQuery(string MachineKey, string MachineID, string MachineLocation, int OpeningHour, 
            int ClosingHour, string TotalCapacity, bool OutOfOrder, string Notes)
        {
            string editQuery = "UPDATE LocationSupply SET " +
                "MachineID = '" + MachineID + "', " +
                "MachineLocation = '" + MachineLocation + "', " +
                "OpeningHour = " + OpeningHour + ", " +
                "ClosingHour = " + ClosingHour + ", " +
                "TotalCapacity = " + TotalCapacity + ", " +
                "OutOfOrder = '" + OutOfOrder + "', " +
                "Notes = " + Notes + " " +
                "WHERE pKey = " + MachineKey;

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
         *      MachineKey - the primary key of the machine record being deleted
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool DeleteQuery(string MachineID)
        {
            // query for deleting record in the LocationSupply table
            string deleteLocationQuery = "DELETE FROM LocationSupply WHERE MachineID = '" + MachineID + "'";

            // query for deleting record in the HourlyStatistics table
            string deleteHourlyQuery = "DELETE FROM HourlyStatistics WHERE MachineID = '" + MachineID + "'";

            bool deleteStatus = QueryProcessor(deleteLocationQuery);

            //if query for deleting record in LocationSupply table was successful, run the query for the HourlyStatistics table
            if (deleteStatus)
            {
                deleteStatus = QueryProcessor(deleteHourlyQuery);
            }

            return deleteStatus;
        }

        /*
         *  Function: MachineIDValidator
         * 
         *  Check to see if machine ID is unique.
         * 
         *  Returns:
         *  
         *      True - if the email does not exist in the LocationSupply datatable
         *      False - if the email exists in the LocationSupply datatable
        */
        public static bool MachineIDValidator(string MachineID)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM LocationSupply WHERE MachineID = '" + MachineID + "'";
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
                throw new Exception(@"Mug-Share Application MachineIDValidator() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }
    }
}