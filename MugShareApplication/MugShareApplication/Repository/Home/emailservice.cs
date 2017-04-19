using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using MugShareApplication.Models;
using MugShareApplication.Controllers;

namespace MugShareApplication.Repository.Home
{
    public class emailservice
    {
        /*--------------------------------------------------------------------------------------
            FUNCTIONS:

                -SendEmail()
                -GetContactList()

        ----------------------------------------------------------------------------------------*/
        /*
         *  Function: SendEmail
         * 
         *  Used for sending email
         *  
         *  Parameters:
         *  
         *      email_subject - Subject of the email to be sent
         *      email_body - Body of the email to be sent
         *      email_recipients - Recipients of the email separated by semicolon
         *  
         *  Returns:
         *  
         *      True - Email was sent successfully
         *      False - Email was not sent successfully
         *  
         *  Note: Need to turn on access for less secure apps at https://www.google.com/settings/security/lesssecureapps
        */
        public static bool SendEmail(string email_recipients, string email_subject, string email_body)
        {
            // Construct the email
            MailMessage message = new MailMessage();
            foreach (var recipient in email_recipients.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.Bcc.Add(recipient);
            }
            message.Subject = email_subject;
            message.Body = email_body;

            // Set up SmtpClient
            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Timeout = 10000;

            try
            {
                // Send Email
                client.Send(message);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /*
         *  Function: GetContactList
         * 
         *  Used for getting the contact list of the specified group
         *  
         *  Parameters:
         *  
         *      contact_list_group - Contact list group to be obtained
         *  
         *  Returns:
         *  
         *      ContactList - Email addresses of the specified contact list
         *  
        */
        public static string GetContactList(string contact_list_group, string mug_share_users_type)
        {
            SqlConnection dbconnection = new SqlConnection();
            SqlCommand dbcommand = new SqlCommand();
            SqlDataReader dbreader;
            string connectionstring = ConfigurationManager.ConnectionStrings["MugShareDB"].ToString();
            string queryString = null;
            switch (contact_list_group)
            {
                case "Admin":
                    queryString = "SELECT Email FROM Security WHERE AdminPermission = 'True'";
                    break;
                case "Staff":
                    queryString = "SELECT Email FROM Security WHERE AdminPermission = 'False'";
                    break;
                case "Mug-Share Users":
                    if (mug_share_users_type == "WithMug,WithoutMug" || mug_share_users_type == "WithoutMug,WithMug")
                    {
                        queryString = "SELECT Email FROM MugShareUsers";
                    }
                    else if (mug_share_users_type == "WithMug")
                    {
                        queryString = "SELECT Email FROM MugShareUsers WHERE MugInUse = 'True'";
                    }
                    else if (mug_share_users_type == "WithoutMug")
                    {
                        queryString = "SELECT Email FROM MugShareUsers WHERE MugInUse = 'False'";
                    }
                    else
                    {
                        return "";
                    }
                    break;
                default:
                    return "";
            }
            string contact_list = null;

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
                    if (!string.IsNullOrEmpty(dbreader["Email"].ToString())) { contact_list += dbreader["Email"].ToString() + ';'; }
                }
                return contact_list;
            }
            catch (Exception e)
            {
                throw new Exception(@"Mug-Share Application GetContactList() failed : ", e);
            }
            finally
            {
                if (dbconnection.State == ConnectionState.Open) { dbconnection.Close(); }
            }
        }
    }
}