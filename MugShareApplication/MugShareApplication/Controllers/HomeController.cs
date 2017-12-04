using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Ports;
using System.Security.Cryptography;
using MugShareApplication.Models;
using MugShareApplication.Repository;
using MugShareApplication.Repository.Home;

namespace MugShareApplication.Controllers
{
    public class HomeController : BaseController
    {
        /*--------------------------------------------------------------------------------------
         * MUG RETURN FUNCTIONS:
         * 
         * - MugReturn()
         * - ProcessReturn()
         * -------------------------------------------------------------------------------------*/
        /*
         Function: MugReturn

         Opens Mug Return view
       */
        public ActionResult MugReturn()
        {
            return View();
        }

        /*
           Function: ProcessReturn
          
           Processes mug return and alters database accordingly.

           Parameters:
           
                MugID - mug ID of mug we want to return
          
           Returns:
          
                model - object with user data
         */
        [HttpPost]
        public JsonResult ProcessReturn(string MugID)
        {
            bool returnStatus = mugreturn.UpdateMugStatusQuery(MugID);

            if (returnStatus == true)
            {
                string LastBorrowedBy = mugreturn.GetUBC_ID(MugID);
                returnStatus = mugreturn.UpdateUserStatusQuery(LastBorrowedBy);
            }

            IsTrueOrFalse model = new IsTrueOrFalse(returnStatus);
            return ResultJson(model);
        }


        /*--------------------------------------------------------------------------------------
         * ABOUT US FUNCTIONS:
         * 
         * - AboutUs()
         * -------------------------------------------------------------------------------------*/
        /*
         Function: AboutUs

         Opens AboutUs view
       */
        public ActionResult AboutUs()
        {
            return View();
        }

        /*--------------------------------------------------------------------------------------
         * HELP FUNCTIONS:
         * 
         * - Help()
         * -------------------------------------------------------------------------------------*/
        /*
         Function: Help

         Opens Help view
       */
        public ActionResult Help()
        {
            return View();
        }
        /*--------------------------------------------------------------------------------------
        * HELP FUNCTIONS:
        * 
        * - SignUp()
        * -------------------------------------------------------------------------------------*/
        /*
         Function: Help

         Opens Sign Up view
       */
        public ActionResult SignUp()
        {
            return View();
        }


        /*--------------------------------------------------------------------------------------
        * STATISTICS FUNCTIONS:
        * 
        * - MonthlyChartData()
        * - YearlyChartData()
        * -------------------------------------------------------------------------------------*/
        /*
           Function: MonthlyChartData
          
           Retrieves data to fill in Monthly Statistics chart.
          
           Returns:
          
                model - object used to fill the Monthly Statistics chart
         */
        [HttpGet]
        public JsonResult MonthlyChartData()
        {
            MonthlyDataModel model = mugreturn.GetMonthlyChartData();
            return ResultJson(model);
        }

        /*
           Function: YearlyChartData
          
           Retrieves data used to fill the Yearly Statistics chart.
          
           Returns:
          
                list - list of objects used to fill the Yearly Statistics chart
         */
        [HttpGet]
        public JsonResult YearlyChartData()
        {
            List<YearlyDataModel> list = mugreturn.GetYearlyChartData();
            return ResultJson(list);
        }

        /*--------------------------------------------------------------------------------------
         * LOGIN FUNCTIONS:
         * 
         * - LogIn()
         * - ProcessLogIn()
         * - GetRFID()
         * -------------------------------------------------------------------------------------*/
        /*
         Function: LogIn

         Opens Login view
       */
        public ActionResult LogIn()
        {
            if (SessionStorage.Username != null)
            {
                SessionStorage.Username = null;
                SessionStorage.AdminPermission = null;
            }
            return View();
        }

        /*
           Function: ProcessLogIn
          
           Processes user log in

           Parameters:
           
                Username - username of the user
          
           Returns:
          
                model - object with user data
         */
        [HttpPost]
        public JsonResult ProcessLogIn(string LogInUsername, string LogInPassword)
        {
            bool logInStatus = login.GetAdminPermission(LogInUsername, LogInPassword);
            IsTrueOrFalse model = new IsTrueOrFalse(logInStatus);
            return ResultJson(model);
        }

        /*
           Function: GetRFID

           Gets the RFID of the mug to be returned

           Returns:

                model - string containing the RFID
         */
        [HttpGet]
        public String GetRFID()
        {
            String mugRFID = ""; ;
            SerialPort comPort = null;
            try
            {
                string[] availablePorts = SerialPort.GetPortNames();

                // Setup and open COMPort
                comPort = new SerialPort(availablePorts[0], 9600, Parity.None, 8, StopBits.One);
                comPort.ReadTimeout = 5000; // Wait for 5 seconds to read from serial port
                comPort.Open();

                // Read COMPort
                mugRFID = comPort.ReadLine();
                if (String.Compare(mugRFID, "") != 0)
                {
                    // Check that it is a valid RFID string
                    // If it is not a valid RFID string, proceed to next available port

                    // Convert RFID to valid value
                    mugRFID = mugRFID.Substring(1, 12); // RFID starts from index 1, with length of 12
                }

                // Close COMPort
                comPort.Close();
            }
            catch (Exception)
            {
                if (comPort != null)
                {
                    comPort.Close();
                }
            }

            return mugRFID;
        }

        /*--------------------------------------------------------------------------------------
         * CHANGE PASSWORD FUNCTIONS:
         * 
         * - ChangePassword()
         * - ProcessChangePassword()
         * - ProcessForgotPassword()
         * -------------------------------------------------------------------------------------*/
        /*
         Function: ChangePassword

         Opens ChangePassword view
       */
        public ActionResult ChangePassword()
        {
            return View();
        }

        /*
           Function: ProcessChangePassword
          
           Processes change in user password

           Parameters:
           
                CurrentPassword - current password of the user
                NewPassword - new password of the user
                RetypeNewPassword - retyped new password of the user
          
           Returns:
          
                model - object with password change status
         */
        [HttpPost]
        public JsonResult ProcessChangePassword(string CurrentPassword, string NewPassword, string RetypeNewPassword)
        {
            bool changePasswordStatus = true;
            string Username = SessionStorage.Username;
            string Email = SessionStorage.Email;

            // Check that username and current password matches
            //string AdminPermission = login.GetAdminPermission(Username, StringCipher.Encrypt(CurrentPassword));
            //login.GetAdminPermission(Username, CurrentPassword);
            if (!(login.GetAdminPermission(Username, CurrentPassword)))
            {
                // Username and current password do not match
                changePasswordStatus = false;
            }
            else
            {
                // Check that new password and retyped new password matches
                if (!(string.Compare(NewPassword, RetypeNewPassword) == 0))
                {
                    // New password and retyped new password does not match
                    changePasswordStatus = false;
                }
                else
                {
                    // Change current password to the new password
                    changepassword.UpdatePasswordQuery(Email, StringCipher.Encrypt(NewPassword));
                }
            }

            IsTrueOrFalse model = new IsTrueOrFalse(changePasswordStatus);
            return ResultJson(model);
        }

        /*
           Function: ProcessForgotPassword

           Processes user's forgot password request

           Parameters:

                ForgotPassword_Email - email of the user

           Returns:

                model - object with user data
        */
        [HttpPost]
        public JsonResult ProcessForgotPassword(string ForgotPassword_Email)
        {
            bool ForgotPasswordStatus = false;

            if (login.CheckEmailAddress(ForgotPassword_Email))
            {
                string temp_password = null;

                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                {
                    Byte[] bytes = new Byte[8];
                    rng.GetBytes(bytes);
                    temp_password = Convert.ToBase64String(bytes);
                    changepassword.UpdatePasswordQuery(ForgotPassword_Email, StringCipher.Encrypt(temp_password));
                }

                string email_body = "Hi user!\r\n\r\nYour temporary password is " + temp_password + ". Please remember to reset your password!";
                string email_subject = "Mug Share Web App Reset Password Request";
                string email_recipients = ForgotPassword_Email;
                ForgotPasswordStatus = emailservice.SendEmail(email_recipients, email_subject, email_body);
            }

            IsTrueOrFalse model = new IsTrueOrFalse(ForgotPasswordStatus);
            return ResultJson(model);
        }

        /*--------------------------------------------------------------------------------------
        * EMAIL SERVICE FUNCTIONS:
        * 
        * - EmailService()
        * - SendEmailService()
        * - GetContactList()
        * -------------------------------------------------------------------------------------*/
        /*
         Function: EmailService

         Opens EmailService view
       */
        public ActionResult EmailService()
        {
            return View();
        }

        /*
           Function: SendEmailService
          
           Send Email

           Parameters:
                EmailSubject - Subject of the email to be sent
                EmailBody - Body of the email to be sent
                EmailRecipients - Recipients of the email separated by semicolon
          
           Returns:
          
                model - object with send email status
         */
        [HttpPost]
        public JsonResult SendEmailService(string EmailSubject, string EmailBody, string EmailRecipients)
        {
            bool sendEmailStatus = true;

            // Send Email
            sendEmailStatus = emailservice.SendEmail(EmailRecipients, EmailSubject, EmailBody);

            IsTrueOrFalse model = new IsTrueOrFalse(sendEmailStatus);
            return ResultJson(model);
        }

        /*
           Function: GetContactList
          
           Gets the contact list of the specified group

           Parameters:
                ContactListGroup - Subject of the email to be sent
                MugShareUsersType - Mug-Share users with and/or without mugs
          
           Returns:
                ContactList - Email addresses of the specified group
         */
        [HttpPost]
        public JsonResult GetContactList(string ContactListGroup, string MugShareUsersType)
        {
            // Get contact list
            string ContactList = null;
            ContactList = emailservice.GetContactList(ContactListGroup, MugShareUsersType);

            return ResultJson(ContactList);
        }
    }
}