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
    public class login
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:
            
                -GetAdminPermission()
                -CheckEmailAddress()

        ----------------------------------------------------------------------------------------*/
        /*
         *  Function: GetAdminPermission
         * 
         *  Get admin permission of user
         * 
         *  Returns:
         *  
         *      AdminPermission - Admin permission of the username
        */
        public static bool GetAdminPermission(string LogInUsername, string LogInPassword)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT Username, Password, Email, AdminPermission FROM Security";
            string Username = null, Password = null;
            bool retVal = false;
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
                    if (!string.IsNullOrEmpty(dbreader["Username"].ToString())) { Username = StringCipher.Decrypt(dbreader["Username"].ToString()); }
                    if (!string.IsNullOrEmpty(dbreader["Password"].ToString())) { Password = StringCipher.Decrypt(dbreader["Password"].ToString()); }

                    if (LogInUsername == Username && LogInPassword == Password)
                    {
                        //Set session storage username
                        BaseController.SessionStorage.Username = LogInUsername;
                        //Set session storage staff key
                        if (!string.IsNullOrEmpty(dbreader["Email"].ToString())) { BaseController.SessionStorage.Email = dbreader["Email"].ToString(); }
                        //Set session storage admin permission
                        if (!string.IsNullOrEmpty(dbreader["AdminPermission"].ToString()))
                        {
                            BaseController.SessionStorage.AdminPermission = (String.Compare(dbreader["AdminPermission"].ToString(), "True") == 0) ? "Admin" : "Staff";
                        }
                        retVal = true;
                    }

                    //if (!string.IsNullOrEmpty(dbreader["AdminPermission"].ToString()))
                    //{
                    //    AdminPermission = (String.Compare(dbreader["AdminPermission"].ToString(), "True") == 0) ? "Admin" : "Staff";
                    //}
                }
                //return AdminPermission;
                return retVal;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetAdminPermission() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }

        /*
         *  Function: CheckEmailAddress
         * 
         *  Check if email exists
         * 
         *  Returns:
         *  
         *      Email - Email address of the user
        */
        public static bool CheckEmailAddress(string ForgotPassword_Email)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = "SELECT * FROM Security WHERE Email = '" + ForgotPassword_Email + "'";
            try
            {
                if (dbconnection.State == ConnectionState.Closed) { dbconnection.ConnectionString = connectionstring; dbconnection.Open(); }
                dbcommand.Connection = dbconnection;
                dbcommand.CommandTimeout = 600;
                dbcommand.CommandText = queryString;
                dbcommand.CommandType = CommandType.Text;
                dbreader = dbcommand.ExecuteReader();
                if (dbreader.HasRows)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application CheckEmailAddress() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }
    }
}