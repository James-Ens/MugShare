using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Configuration;
using MugShareApplication.Models;
using MugShareApplication.Repository.Datatables;
using System.Text;
using LinqToExcel;

namespace MugShareApplication.Controllers
{
    public class DatatablesController : BaseController
    {
        /*--------------------------------------------------------------------------------------
         * MUG SHARE USERS TABLE FUNCTIONS:
         * 
         * - MugShareUsers()
         * - MSU_Excel()
         * - MSU_TableData()
         * - MSU_GetRecord()
         * - MSU_Create()
         * - MSU_Edit()
         * - MSU_Delete()
         * - MSU_StudentNumberValidator()
         * - MSU_EmailValidator()
         * -------------------------------------------------------------------------------------*/
        /*
          Function: MugShareUsers

          Opens Mug-Share Users view
        */
        public ActionResult MugShareUsers()
        {
            return View();
        }

        /*
          Function: MSU_Excel

          Upload data imported form excel document and replace MugShareUsers data in the 
          database with the collected data.

          Parameters:

               ExcelPath - path to excel document on local computer

          Returns:

               model - true or false value in an object to verify succcess of query
        */
        [HttpGet]
        public JsonResult MSU_Excel(string ExcelPath)
        {
            //string connectionString = @"C:\Users\Logan\Downloads\StudentInfoExample.xlsx";
            string connectionString = ExcelPath;
            string sheetName = "Sheet1";
            bool createStatus = false;

            //tempString is the hardcoded working string, and connectionString is the same string passed through ajax
            //if(tempString != connectionString)
            //{
            //    IsTrueOrFalse temp = new IsTrueOrFalse(false);
            //    return ResultJson(temp);
            //}

            List<UsersModalModel> list = new List<UsersModalModel>();

            //connect to excel document and get row data
            var excelFile = new ExcelQueryFactory(connectionString);
            var studentRecords = from a in excelFile.Worksheet(sheetName) select a;

            //create UsersModalModel models containing the data from excel document
            foreach (var a in studentRecords)
            {
                UsersModalModel model = new UsersModalModel();
                model.StudentNumber = a["STUD_NO"];
                model.FirstName = a["GIVEN_NAME"];
                model.LastName = a["SURNAME"];
                model.Email = a["EMAIL_ADDRESS"];
                model.TotalMugsBorrowed = "0";
                list.Add(model);
            }

            //get ride of duplicates based on student numbers
            list = list.GroupBy(x => x.StudentNumber).Select(y => y.First()).ToList();

            //delete all records from the MugShareUsers table in database
            if (list.Count > 0)
            {
                string clearMSUTable = "DELETE FROM MugShareUsers";
                createStatus = mugshareusers.QueryProcessor(clearMSUTable);
            }

            //make query string to input excel data into database
            if (createStatus)
            {
                string dataEntryQuery = "INSERT INTO MugShareUsers (StudentNumber, FirstName, LastName, Email, MugInUse, TotalMugsBorrowed) VALUES ";

                foreach (UsersModalModel element in list)
                {
                    string recordValues = "(" + element.StudentNumber + ", '" + element.FirstName + "', '" + element.LastName + "', '" + element.Email + "', 'false', 0), ";
                    dataEntryQuery += recordValues;
                }

                dataEntryQuery = dataEntryQuery.Substring(0, dataEntryQuery.Length - 2);

                createStatus = mugshareusers.QueryProcessor(dataEntryQuery);
            }

            IsTrueOrFalse boolModel = new IsTrueOrFalse(createStatus);
            return ResultJson(boolModel);
        }

        /*
           Function: MSU_TableData
          
           Retrieves data used to fill the MugShareUsers display table.
          
           Returns:
          
                list - list of objects used to fill MugShareUsers display table
         */
        [HttpGet]
        public JsonResult MSU_TableData()
        {
            List<UsersDatatableModel> list = mugshareusers.GetList();
            return ResultJson(list);
        }

        /*
           Function: MSU_GetRecord
          
           Retrieves data used to fill the MugShareUsers read/edit/delete modals.

           Parameters:
           
                UserKey - primary key of user record we want to get
          
           Returns:
          
                model - object with user data
         */
        [HttpPost]
        public JsonResult MSU_GetRecord(string UserKey)
        {
            UsersModalModel model = mugshareusers.GetRecord(UserKey);
            return ResultJson(model);
        }

        /*
           Function: MSU_Create
          
           Creates a new user record in the MugShareUsers datatable.

           Parameters:
           
                jsonString - object with new user data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MSU_Create(UsersModalModel jsonString)
        {
            string StudentNumber = jsonString.StudentNumber;
            string FirstName = jsonString.FirstName;
            string LastName = jsonString.LastName;
            string Email = jsonString.Email;
            string Notes = jsonString.Notes;

            //Notes field is allowed to be null, therfore account for string variation
            if (Notes != null) { Notes = "'" + Notes + "'"; }
            else { Notes = "NULL"; }

            bool createStatus = mugshareusers.CreateQuery(StudentNumber, FirstName, LastName, Email, Notes);
            IsTrueOrFalse model = new IsTrueOrFalse(createStatus);
            return ResultJson(model);
        }

        /*
           Function: MSU_Edit
          
           Updates an existing user record in the MugShareUsers datatable.

           Parameters:
           
                jsonString - object with updated user data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MSU_Edit(UsersModalModel jsonString)
        {
            string UserKey = jsonString.UserKey;
            string StudentNumber = jsonString.StudentNumber;
            string FirstName = jsonString.FirstName;
            string LastName = jsonString.LastName;
            string Email = jsonString.Email;
            bool MugInUse = jsonString.MugInUse;
            string DateOfRental = jsonString.DateOfRental;
            string Notes = jsonString.Notes;

            //Date of rental field is allowed to be null, therfore account for string variation
            if (DateOfRental != null) { DateOfRental = "'" + DateOfRental + "'"; }
            else { DateOfRental = "NULL"; }

            //Notes field is allowed to be null, therfore account for string variation
            if (Notes != null) { Notes = "'" + Notes + "'"; }
            else { Notes = "NULL"; }

            bool editStatus = mugshareusers.EditQuery(UserKey, StudentNumber, FirstName, LastName, Email, MugInUse, DateOfRental, Notes);
            IsTrueOrFalse model = new IsTrueOrFalse(editStatus);
            return ResultJson(model);
        }

        /*
           Function: MSU_Delete
          
           Deletes an existing user record from the MugShareUsers datatable.

           Parameters:
           
                UserKey - primary key of user record to be deleted
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MSU_Delete(string UserKey)
        {
            bool deleteStatus = mugshareusers.DeleteQuery(UserKey);
            IsTrueOrFalse model = new IsTrueOrFalse(deleteStatus);
            return ResultJson(model);
        }

        /*
           Function: MSU_StudentNumberValidator
          
           Checks to make sure the student number entered is unique.

           Parameters:
           
                StudentNumber - student number that will be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MSU_StudentNumberValidator(string StudentNumber)
        {
            bool studentNumberStatus = mugshareusers.StudentNumberValidator(StudentNumber);
            IsTrueOrFalse model = new IsTrueOrFalse(studentNumberStatus);
            return ResultJson(model);
        }

        /*
           Function: MSU_EmailValidator
          
           Checks to make sure the email entered is unique.

           Parameters:
           
                Email - email that will be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MSU_EmailValidator(string Email)
        {
            bool emailStatus = mugshareusers.EmailValidator(Email);
            IsTrueOrFalse model = new IsTrueOrFalse(emailStatus);
            return ResultJson(model);
        }


        /*--------------------------------------------------------------------------------------
         * LOCATION SUPPLY TABLE FUNCTIONS:
         * 
         * - LocationSupply()
         * - LS_TableData()
         * - LS_GetRecord()
         * - LS_HourlyChartData()
         * - LS_Create()
         * - LS_Edit()
         * - LS_Delete()
         * - LS_MachineIDValidator()
         * -------------------------------------------------------------------------------------*/
        /*
          Function: LocationSupply

          Opens Location Supply view
        */
        public ActionResult LocationSupply()
        {
            return View();
        }

        /*
           Function: LS_TableData
          
           Retrieves data used to fill the LocationSupply display table.
          
           Returns:
          
                list - list of objects used to fill LocationSupply display table
         */
        [HttpGet]
        public JsonResult LS_TableData()
        {
            List<LocationDatatableModel> list = locationsupply.GetList();
            return ResultJson(list);
        }

        /*
           Function: LS_GetRecord
          
           Retrieves data used to fill the LocationSupply read/edit/delete modals.

           Parameters:
           
                MachineKey - primary key of machine record we want to get
          
           Returns:
          
                model - object with machine data
         */
        [HttpPost]
        public JsonResult LS_GetRecord(string MachineKey)
        {
            LocationModalModel model = locationsupply.GetRecord(MachineKey);
            return ResultJson(model);
        }

        /*
           Function: LS_HourlyChartData
          
           Retrieves data to fill in Hourly Statistics chart of specified dispensing machine.
          
           Returns:
          
                model - object used to fill the Hourly Statistics chart of specified dispensing machine
         */
        [HttpPost]
        public JsonResult LS_HourlyChartData(LocationModalModel jsonString)
        {
            string MachineID = jsonString.MachineID;
            int openingHour = jsonString.OpeningHour;
            int closingHour = jsonString.ClosingHour;

            List<HourlyDataModel> list = locationsupply.GetHourlyChartData(MachineID, openingHour, closingHour);
            return ResultJson(list);
        }

        /*
           Function: LS_Create
          
           Creates a new mug record in the LocationSupply datatable.

           Parameters:
           
                jsonString - object with new machine data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult LS_Create(LocationModalModel jsonString)
        {
            string MachineID = jsonString.MachineID;
            string MachineLocation = jsonString.MachineLocation;
            int OpeningHour = jsonString.OpeningHour;
            int ClosingHour = jsonString.ClosingHour;
            string TotalCapacity = jsonString.TotalCapacity;
            bool OutOfOrder = jsonString.OutOfOrder;
            string Notes = jsonString.Notes;

            //Notes field is allowed to be null, therfore account for string variation
            if (Notes != null) { Notes = "'" + Notes + "'"; }
            else { Notes = "NULL"; }

            bool createStatus = locationsupply.CreateQuery(MachineID, MachineLocation, OpeningHour, ClosingHour, TotalCapacity, OutOfOrder, Notes);
            IsTrueOrFalse model = new IsTrueOrFalse(createStatus);
            return ResultJson(model);
        }

        /*
           Function: LS_Edit
          
           Updates an existing mug record in the LocationSupply datatable.

           Parameters:
           
                jsonString - object with updated machine data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult LS_Edit(LocationModalModel jsonString)
        {
            string MachineKey = jsonString.MachineKey;
            string MachineID = jsonString.MachineID;
            string MachineLocation = jsonString.MachineLocation;
            int OpeningHour = jsonString.OpeningHour;
            int ClosingHour = jsonString.ClosingHour;
            string TotalCapacity = jsonString.TotalCapacity;
            bool OutOfOrder = jsonString.OutOfOrder;
            string Notes = jsonString.Notes;

            //Notes field is allowed to be null, therfore account for string variation
            if (Notes != null) { Notes = "'" + Notes + "'"; }
            else { Notes = "NULL"; }

            bool editStatus = locationsupply.EditQuery(MachineKey, MachineID, MachineLocation, OpeningHour, ClosingHour, TotalCapacity, OutOfOrder, Notes);
            IsTrueOrFalse model = new IsTrueOrFalse(editStatus);
            return ResultJson(model);
        }

        /*
           Function: LS_Delete
          
           Deletes an existing mug record from the LocationSupply datatable.

           Parameters:
           
                MachineKey - primary key of machine record to be deleted
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult LS_Delete(string MachineID)
        {
            bool deleteStatus = locationsupply.DeleteQuery(MachineID);
            IsTrueOrFalse model = new IsTrueOrFalse(deleteStatus);
            return ResultJson(model);
        }

        /*
           Function: LS_MachineIDValidator
          
           Checks to make sure the machine ID is unique.

           Parameters:
           
                MachineID - machine ID to be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult LS_MachineIDValidator(string MachineID)
        {
            bool machineIDStatus = locationsupply.MachineIDValidator(MachineID);
            IsTrueOrFalse model = new IsTrueOrFalse(machineIDStatus);
            return ResultJson(model);
        }


        /*--------------------------------------------------------------------------------------
         * MUG REGISTRY TABLE FUNCTIONS:
         * 
         * - MugRegistry()
         * - MR_TableData()
         * - MR_GetRecord()
         * - MR_Create()
         * - MR_Edit()
         * - MR_Delete()
         * - MR_MugIDValidator()
         * -------------------------------------------------------------------------------------*/
        /*
          Function: MugRegistry

          Opens Mug Registry view
        */
        public ActionResult MugRegistry()
        {
            return View();
        }

        /*
           Function: MR_TableData
          
           Retrieves data used to fill the MugRegistry display table.
          
           Returns:
          
                list - list of objects used to fill MugRegistry display table
         */
        [HttpGet]
        public JsonResult MR_TableData()
        {
            List<MugsDatatableModel> list = mugregistry.GetList();
            return ResultJson(list);
        }

        /*
           Function: MR_GetRecord
          
           Retrieves data used to fill the MugRegistry read/edit/delete modals.

           Parameters:
           
                MugKey - primary key of mug record we want to get
          
           Returns:
          
                model - object with mug data
         */
        [HttpPost]
        public JsonResult MR_GetRecord(string MugKey)
        {
            MugsModalModel model = mugregistry.GetRecord(MugKey);
            return ResultJson(model);
        }

        /*
           Function: MR_Create
          
           Creates a new mug record in the MugRegistry datatable.

           Parameters:
           
                MugID - new mug id to be added to the MugRegistry datatable
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MR_Create(string MugID)
        {
            bool CurrentlyInUse = false;

            bool createStatus = mugregistry.CreateQuery(MugID, CurrentlyInUse);
            IsTrueOrFalse model = new IsTrueOrFalse(createStatus);
            return ResultJson(model);
        }

        /*
           Function: MR_Edit
          
           Updates an existing mug record in the MugRegistry datatable.

           Parameters:
           
                jsonString - object with updated mug data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MR_Edit(MugsModalModel jsonString)
        {
            string MugKey = jsonString.MugKey;
            string MugID = jsonString.MugID;
            bool CurrentlyInUse = jsonString.CurrentlyInUse;
            string Notes = jsonString.Notes;

            //Notes field is allowed to be null, therfore account for string variation
            if (Notes != null) { Notes = "'" + Notes + "'"; }
            else { Notes = "NULL"; }

            bool editStatus = mugregistry.EditQuery(MugKey, MugID, CurrentlyInUse, Notes);
            IsTrueOrFalse model = new IsTrueOrFalse(editStatus);
            return ResultJson(model);
        }

        /*
           Function: MR_Delete
          
           Deletes an existing mug record from the MugRegistry datatable.

           Parameters:
           
                MugKey - primary key of mug record to be deleted
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MR_Delete(string MugKey)
        {
            bool deleteStatus = mugregistry.DeleteQuery(MugKey);
            IsTrueOrFalse model = new IsTrueOrFalse(deleteStatus);
            return ResultJson(model);
        }

        /*
           Function: MR_MugIDValidator
          
           Checks to make sure the mug ID is unique.

           Parameters:
           
                MugID - mug ID to be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult MR_MugIDValidator(string MugID)
        {
            bool mugIDStatus = mugregistry.MugIDValidator(MugID);
            IsTrueOrFalse model = new IsTrueOrFalse(mugIDStatus);
            return ResultJson(model);
        }


        /*--------------------------------------------------------------------------------------
         * SECURITY TABLE FUNCTIONS:
         * 
         * - Security()
         * - S_TableData()
         * - S_GetRecord()
         * - S_Create()
         * - S_Edit()
         * - S_Delete()
         * - S_StaffCardIDValidator()
         * - S_UsernameValidator()
         * - S_EmailValidator()
         * -------------------------------------------------------------------------------------*/
        /*
         Function: Security

         Opens Security view
        */
        public ActionResult Security()
        {
            return View();
        }

        /*
           Function: S_TableData
          
           Retrieves data used to fill the Security display table.
          
           Returns:
          
                list - list of objects used to fill Security display table
         */
        [HttpGet]
        public JsonResult S_TableData()
        {
            List<SecurityDatatableModel> list = security.GetList();
            return ResultJson(list);
        }

        /*
           Function: MSU_GetRecord
          
           Retrieves data used to fill the Security read/edit/delete modals.

           Parameters:
           
                SecurityKey - primary key of security record we want to get
          
           Returns:
          
                model - object with security data
         */
        [HttpPost]
        public JsonResult S_GetRecord(string SecurityKey)
        {
            SecurityModalModel model = security.GetRecord(SecurityKey);
            return ResultJson(model);
        }

        /*
           Function: S_Create
          
           Creates a new security record in the Security datatable.

           Parameters:
           
                jsonString - object with new security data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult S_Create(SecurityModalModel jsonString)
        {
            string FirstName = jsonString.FirstName;
            string LastName = jsonString.LastName;
            string StaffCardID = jsonString.StaffCardID;
            string Username = StringCipher.Encrypt(jsonString.Username);
            string Password = StringCipher.Encrypt(jsonString.Password);
            string Email = jsonString.Email;
            bool AdminPermission = jsonString.AdminPermission;

            bool createStatus = security.CreateQuery(FirstName, LastName, StaffCardID, Username, Password, Email, AdminPermission);
            IsTrueOrFalse model = new IsTrueOrFalse(createStatus);
            return ResultJson(model);
        }

        /*
           Function: S_Edit
          
           Updates an existing security record in the Security datatable.

           Parameters:
           
                jsonString - object with updated security data
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult S_Edit(SecurityModalModel jsonString)
        {
            string SecurityKey = jsonString.SecurityKey;
            string FirstName = jsonString.FirstName;
            string LastName = jsonString.LastName;
            string StaffCardID = jsonString.StaffCardID;
            string UserName = StringCipher.Encrypt(jsonString.Username);
            string Email = jsonString.Email;
            bool AdminPermission = jsonString.AdminPermission;

            bool editStatus = security.EditQuery(SecurityKey, FirstName, LastName, StaffCardID, UserName, Email, AdminPermission);
            IsTrueOrFalse model = new IsTrueOrFalse(editStatus);
            return ResultJson(model);
        }

        /*
           Function: S_Delete
          
           Deletes an existing security record from the Security datatable.

           Parameters:
           
                SecurityKey - primary key of security record to be deleted
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult S_Delete(string SecurityKey)
        {
            bool deleteStatus = security.DeleteQuery(SecurityKey);
            IsTrueOrFalse model = new IsTrueOrFalse(deleteStatus);
            return ResultJson(model);
        }

        /*
           Function: S_StaffCardIDValidator
          
           Checks to make sure the staff card ID entered is unique.

           Parameters:
           
                StaffCardID - staff card ID that will be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult S_StaffCardIDValidator(string StaffCardID)
        {
            bool staffCardIDStatus = security.StaffCardIDValidator(StaffCardID);
            IsTrueOrFalse model = new IsTrueOrFalse(staffCardIDStatus);
            return ResultJson(model);
        }

        /*
           Function: S_UsernameValidator
          
           Checks to make sure the username entered is unique.

           Parameters:
           
                Username - username that will be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult S_UsernameValidator(string Username)
        {
            bool usernameStatus = security.UsernameValidator(Username);
            IsTrueOrFalse model = new IsTrueOrFalse(usernameStatus);
            return ResultJson(model);
        }

        /*
           Function: S_EmailValidator
          
           Checks to make sure the email entered is unique.

           Parameters:
           
                Email - email that will be checked to make sure it is unique
          
           Returns:
          
                model - true or false value in an object to verify succcess of query
         */
        [HttpPost]
        public JsonResult S_EmailValidator(string Email)
        {
            bool emailStatus = security.EmailValidator(Email);
            IsTrueOrFalse model = new IsTrueOrFalse(emailStatus);
            return ResultJson(model);
        }
    }
}
 