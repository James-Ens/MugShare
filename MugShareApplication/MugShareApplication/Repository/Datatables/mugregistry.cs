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
    public class mugregistry
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -GetList()
                -GetRecord()
                -QueryProcessor()
                -CreateQuery()
                -EditQuery()
                -DeleteQuery()
                -MugIDValidator()

        ----------------------------------------------------------------------------------------*/
        /*
         *  Function: GetList
         * 
         *  Get data for mug registry table display on Mug Registry page.
         * 
         *  Returns:
         *  
         *      list - a list of MugsDatatableModel models which contain each record's
         *             mug ID, last borrowed by and buttons connected to their
         *             respective pKey
        */
        public static List<MugsDatatableModel> GetList()
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MugRegistry ORDER BY pKey";
            List<MugsDatatableModel> list = new List<MugsDatatableModel>();
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
                    MugsDatatableModel model = new MugsDatatableModel();
                    if (!string.IsNullOrEmpty(dbreader["MugID"].ToString())) { model.MugID = dbreader["MugID"].ToString(); }

                    //if (!string.IsNullOrEmpty(dbreader["LastBorrowedBy"].ToString())) { model.LastBorrowedBy = dbreader["LastBorrowedBy"].ToString(); }
                    //else { model.LastBorrowedBy = "New Mug"; }

                    if (dbreader["CurrentlyInUse"].ToString() == "True") { model.CurrentlyInUse = "<div class='text-center'><span class='glyphicon glyphicon-ok' style='color:green'></span></div>"; }
                    else model.CurrentlyInUse = "<div class='text-center'><span class='glyphicon glyphicon-remove' style='color:red'></span></div>";

                    string pKey = dbreader["pKey"].ToString();

                    model.buttons = "<div class='row'><div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='MR_read(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#MR_read'>View</a></div>" +
                                    "<div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='MR_edit(\"" + pKey + "\")' class='btn btn-primary btn-sm' data-toggle='modal' data-target='#MR_edit'>Edit</a></div>";

                    if (Controllers.BaseController.SessionStorage.AdminPermission == "Admin")
                    {
                        model.buttons += "<div class='col-sm-offset-1 col-sm-2 text-center'>" +
                                    "<a onclick='MR_delete(\"" + pKey + "\")' class='btn btn-danger btn-sm' data-toggle='modal' data-target='#MR_delete'>Delete</a></div></div>";
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
         *  Get data for mug registry modals.
         * 
         *  Returns:
         *  
         *      model - a MugsModalModel model of the record with the primary key equal to the MugKey
        */
        public static MugsModalModel GetRecord(string MugKey)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MugRegistry WHERE pKey = " + MugKey;
            MugsModalModel model = new MugsModalModel();
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
                    if (!string.IsNullOrEmpty(dbreader["pKey"].ToString())) { model.MugKey = dbreader["pKey"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["MugID"].ToString())) { model.MugID = dbreader["MugID"].ToString(); }
                    if (!string.IsNullOrEmpty(dbreader["LastBorrowedBy"].ToString())) { model.LastBorrowedBy = dbreader["LastBorrowedBy"].ToString(); }

                    if (dbreader["CurrentlyInUse"].ToString() == "True") { model.CurrentlyInUse = true; }
                    else model.CurrentlyInUse = false;

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
         *      MugID - Mug ID of new mug
         *      CurrentlyInUse - set to false since mug is just being added to the MugRegistry datatable
         * 
         *  Returns:
         *  
         *      True - if record is created successfully
         *      False - if query fails
        */
        public static bool CreateQuery(string MugID, bool CurrentlyInUse)
        {
            string createQuery = "INSERT INTO MugRegistry (MugID, CurrentlyInUse) VALUES " + "('" + MugID + "', '" + CurrentlyInUse + "')";

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
         *      MugKey - the primary key of the mug record being updated
         *      MugID - Mug ID of mug record
         *      CurrentlyInUse - Currently In Use status of mug record
         *      Notes - Special notes for the mug
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool EditQuery(string MugKey, string MugID, bool CurrentlyInUse, string Notes)
        {
            string editQuery = "UPDATE MugRegistry SET " +
                "MugID = '" + MugID + "', " +
                "CurrentlyInUse = '" + CurrentlyInUse + "', " +
                "Notes = " + Notes + " " +
                "WHERE pKey = " + MugKey;

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
         *      MugKey - the primary key of the mug record being deleted
         * 
         *  Returns:
         *  
         *      True - if record is updated successfully
         *      False - if query fails
        */
        public static bool DeleteQuery(string MugKey)
        {
            string deleteQuery = "DELETE FROM MugRegistry WHERE pKey = '" + MugKey + "'";
            bool deleteStatus = QueryProcessor(deleteQuery);
            return deleteStatus;
        }

        /*
         *  Function: MugIDValidator
         * 
         *  Check to see if mug ID is unique.
         * 
         *  Returns:
         *  
         *      True - if the email does not exist in the MugRegistry datatable
         *      False - if the email exists in the MugRegistry datatable
        */
        public static bool MugIDValidator(string MugID)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM MugRegistry WHERE MugID = '" + MugID + "'";
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
                throw new Exception(@"Mug-Share Application MugIDValidator() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }
    }
}