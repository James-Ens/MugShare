// Initializes a model of knockout observables used in the html
var MSU_koModel = {
    ExcelPath: ko.observable(null),

    UserKey: ko.observable(null),
    StudentNumber: ko.observable(null),
    FirstName: ko.observable(null),
    LastName: ko.observable(null),
    Email: ko.observable(null),
    MugInUse: ko.observable(false),
    DateOfRental: ko.observable(null),
    TotalMugsBorrowed: ko.observable(null),
    Notes: ko.observable(null)
}

// Initializes a model that will later be used to check if changes
// have occurred in the knockout model
var MSU_Initial = {
    StudentNumber: null,
    FirstName: null,
    LastName: null,
    Email: null,
    MugInUse: false,
    DateOfRental: null,
    Notes: null
}

// Variable that refers to the displayed data table
var MSU_table = null;

/*--------------------------------------------------------------------------------------
    Document ready function:
    
        -Checks security permissions
        -Applys the knockout bindings to html section in MugShareUsers.cshtml
        -References an ajax call to build the data table displayed in the view
        -Resizes it to fit the size of the window
----------------------------------------------------------------------------------------*/
$(document).ready(function () {
    ko.applyBindings(MSU_koModel, document.getElementById("MugShareUsers"));
    MSU_Table_Data_Ajax();

    //$(window).resize(function () {
    //    MSU_RedrawTable(220);
    //});
});

function MSU_DataTable(data) {
    MSU_table = $('#MSU_Table').DataTable({
        "data": data,
        "pageLength": 10,
        "lengthChange": false,
        "pageType": "full_numbers",
        "columns": [
            { "title": "UBC ID", "data": "StudentNumber", "classname": "alignleft", "width": "15%" },
            { "title": "First Name", "data": "FirstName", "classname": "alignleft", "width": "18%" },
            { "title": "Last Name", "data": "LastName", "classname": "alignleft", "width": "22%" },
            { "title": "Total Mugs Borrowed", "data": "TotalMugsBorrowed", "classname": "alignleft", "width": "25%" },
            { "title": "", "data": "buttons", "classname": "center-block", "searchable": false, "orderable": false, "width": "20%" }
        ]
    });
}


/*--------------------------------------------------------------------------------------
    CREATE MODAL FUNCTIONS:

        -MSU_create()
        -MSU_create_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MSU_create

    Opens the create user modal, and clears the knockout model so that
    all the fields are empty
*/
function MSU_create() {
    MSU_clear_model();
    MSU_createDefaultTextFields();
    $('#MSU_create').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });
}

/*
    Function: MSU_create_submit

    Submits new record to the database
*/
function MSU_create_submit() {
    toastr.clear();
    MSU_stringCorrections();
    MSU_create_field_check();
}

/*--------------------------------------------------------------------------------------
    UPLOAD MODAL FUNCTIONS:

        -MSU_upload()
        -MSU_upload_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MSU_upload

    Opens the excel file upload modal
*/
function MSU_upload() {
    MSU_clear_model();
    $('#MSU_upload').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });
}

/*
    Function: MSU_upload_submit

    Submits list of records imported from excel document to the database
    which replaces all existing data (ie. old data is deleted)
*/
function MSU_upload_submit() {
    toastr.clear();
    MSU_excel_upload_ajax();
}

/*--------------------------------------------------------------------------------------
    READ MODAL FUNCTIONS:

        -MSU_read()

----------------------------------------------------------------------------------------*/
/*
    Function: MSU_read

    Opens the read user modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        UserKey - The primary key of the record selected
*/
function MSU_read(UserKey) {
    MSU_clear_model();
    $('#MSU_read').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    MSU_GetRecord(UserKey);
}

/*--------------------------------------------------------------------------------------
    EDIT MODAL FUNCTIONS:

        -MSU_edit()
        -MSU_edit_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MSU_edit

    Opens the edit user modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        UserKey - The primary key of the record selected
*/
function MSU_edit(UserKey) {
    MSU_clear_model();
    MSU_editDefaultTextFields();
    $('#MSU_edit').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    MSU_GetRecord(UserKey);
}

/*
    Function: MSU_edit_submit

    Submits updated record to the database
*/
function MSU_edit_submit() {
    toastr.clear();
    MSU_stringCorrections();
    MSU_editDefaultTextFields();
    MSU_edit_field_check();
}

/*--------------------------------------------------------------------------------------
    DELETE MODAL FUNCTIONS:

        -MSU_delete()
        -MSU_delete_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MSU_delete

    Opens the delete user modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        UserKey - The primary key of the record selected
*/
function MSU_delete(UserKey) {
    MSU_clear_model();
    $('#MSU_delete').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    MSU_GetRecord(UserKey);
}

/*
    Function: MSU_delete_submit

    Submits command to the database to delete the selected record
*/
function MSU_delete_submit() {
    MSU_delete_ajax();
    MSU_Table_Data_Ajax();
}


/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -initialize_MSU_Initial()
        -MSU_clear_model()
        -MSU_stringCorrections()
        -MSU_createDefaultTextFields()
        -MSU_editDefaultTextFields()
        -MSU_create_field_check()
        -MSU_edit_field_check()
        -MSU_emptyTextField()

----------------------------------------------------------------------------------------*/
/*
    Function: initialize_MSU_Initial

    Copies the initial values of the knockout model to the MSU_Initial model
    so that it can be compared to later to check for changes.
*/
function initialize_MSU_Initial() {
    MSU_Initial.StudentNumber = MSU_koModel.StudentNumber();
    MSU_Initial.FirstName = MSU_koModel.FirstName();
    MSU_Initial.LastName = MSU_koModel.LastName();
    MSU_Initial.Email = MSU_koModel.Email();
    MSU_Initial.MugInUse = MSU_koModel.MugInUse();
    MSU_Initial.DateOfRental = MSU_koModel.DateOfRental();
    MSU_Initial.Notes = MSU_koModel.Notes();
}

/*
    Function: MSU_clear_model

    Clears both the knockout model and the MSU_Initial model
*/
function MSU_clear_model() {
    ExcelPath: ko.observable(null),
    MSU_koModel.UserKey(null),
    MSU_koModel.StudentNumber(null),
    MSU_koModel.FirstName(null),
    MSU_koModel.LastName(null),
    MSU_koModel.Email(null),
    MSU_koModel.MugInUse(false),
    MSU_koModel.DateOfRental(null),
    MSU_koModel.TotalMugsBorrowed(null),
    MSU_koModel.Notes(null),

    MSU_Initial = {
        StudentNumber: null,
        FirstName: null,
        LastName: null,
        Email: null,
        MugInUse: false,
        DateOfRental: null,
        Notes: null
    };

    toastr.clear(); // Clear all open toastr alerts when opening a modal
}

/*
    Function: MSU_stringCorrections

    Trims unnecessary spaces from beginning and end of first and last name entries
*/
function MSU_stringCorrections() {
    if (MSU_koModel.FirstName() != null) { MSU_koModel.FirstName(MSU_koModel.FirstName().trim()); }
    if (MSU_koModel.LastName() != null) { MSU_koModel.LastName(MSU_koModel.LastName().trim()); }
}

/*
    Function: MSU_createDefaultTextFields

    Sets the border color of the create modal text fields to the default color
*/
function MSU_createDefaultTextFields() {
    document.getElementById("C_StudentNumber").className = "col-sm-6";
    document.getElementById("C_FirstName").className = "col-sm-6";
    document.getElementById("C_LastName").className = "col-sm-6";
    document.getElementById("C_Email").className = "col-sm-6";
}

/*
    Function: MSU_editDefaultTextFields

    Sets the border color of the edit modal text fields to the default color
*/
function MSU_editDefaultTextFields() {
    document.getElementById("E_StudentNumber").className = "col-sm-6";
    document.getElementById("E_FirstName").className = "col-sm-6";
    document.getElementById("E_LastName").className = "col-sm-6";
    document.getElementById("E_Email").className = "col-sm-6";
    document.getElementById("E_DateOfRental").className = "col-sm-6";
}

/*
    Function: MSU_create_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before creating the new record with this data in the database
*/
function MSU_create_field_check() {
    var validationSuccessful = true;

    // Student Number validation step
    if (MSU_emptyTextField(MSU_koModel.StudentNumber()) || MSU_koModel.StudentNumber().length < 7 || MSU_koModel.StudentNumber().length > 8) {
        document.getElementById("C_StudentNumber").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Student Number'.");
        validationSuccessful = false;
    }
    else if (!MSU_StudentNumberValidator()) {
        document.getElementById("C_StudentNumber").className = "col-sm-6 has-error";
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_StudentNumber").className = "col-sm-6 has-success";
    }

    // First Name validation step
    if (MSU_emptyTextField(MSU_koModel.FirstName())) {
        document.getElementById("C_FirstName").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'First Name'.");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_FirstName").className = "col-sm-6 has-success";
    }

    // Last Name validation step
    if (MSU_emptyTextField(MSU_koModel.LastName())) {
        document.getElementById("C_LastName").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Last Name'.");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_LastName").className = "col-sm-6 has-success";
    }

    // Email validation step
    if (MSU_emptyTextField(MSU_koModel.Email())) {
        document.getElementById("C_Email").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Email'.");
        validationSuccessful = false;
    }
    else if (!MSU_EmailValidator()) {
        document.getElementById("C_Email").className = "col-sm-6 has-error";
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_Email").className = "col-sm-6 has-success";
    }

    if (validationSuccessful) {
        MSU_create_ajax();
        MSU_Table_Data_Ajax();
    }
}

/*
    Function: MSU_edit_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before updating the record with this data in the database. Also checks to
    make sure that changes have actually occurred to save server side processing
    time
*/
function MSU_edit_field_check() {
    var validationSuccessful = true;
    var changeDetected = false;

    // Student Number change/validation step
    if (MSU_koModel.StudentNumber() != MSU_Initial.StudentNumber) {
        changeDetected = true;

        if (MSU_emptyTextField(MSU_koModel.StudentNumber()) || MSU_koModel.StudentNumber().length < 7 || MSU_koModel.StudentNumber().length > 8) {
            document.getElementById("E_StudentNumber").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Student Number'.");
            validationSuccessful = false;
        }
        else if (!MSU_StudentNumberValidator()) {
            document.getElementById("E_StudentNumber").className = "col-sm-6 has-error";
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_StudentNumber").className = "col-sm-6 has-success";
        }
    }

    // First Name change/validation step
    if (MSU_koModel.FirstName() != MSU_Initial.FirstName) {
        changeDetected = true;

        if (MSU_emptyTextField(MSU_koModel.FirstName())) {
            document.getElementById("E_FirstName").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'First Name'.");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_FirstName").className = "col-sm-6 has-success";
        }
    }

    // Last Name change/validation step
    if (MSU_koModel.LastName() != MSU_Initial.LastName) {
        changeDetected = true;

        if (MSU_emptyTextField(MSU_koModel.LastName())) {
            document.getElementById("E_LastName").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Last Name'.");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_LastName").className = "col-sm-6 has-success";
        }
    }

    // Email change/validation step
    if (MSU_koModel.Email() != MSU_Initial.Email) {
        changeDetected = true;

        if (MSU_emptyTextField(MSU_koModel.Email())) {
            document.getElementById("E_Email").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Email'.");
            validationSuccessful = false;
        }
        else if (!MSU_EmailValidator()) {
            document.getElementById("E_Email").className = "col-sm-6 has-error";
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_Email").className = "col-sm-6 has-success";
        }
    }

    // Mug In Use/Date of Rental change/validation step
    if (MSU_koModel.MugInUse() != MSU_Initial.MugInUse
        || MSU_koModel.DateOfRental() != MSU_Initial.DateOfRental) {
        changeDetected = true;

        if ((MSU_koModel.MugInUse() == true && (MSU_emptyTextField(MSU_koModel.DateOfRental())))) {
            document.getElementById("E_DateOfRental").className = "col-sm-6 has-error";
            toastr.error("If 'Mug In Use' is checked, then 'Date Of Rental' must be filled in.");
            validationSuccessful = false;
        }
        else if ((MSU_koModel.MugInUse() == false && !MSU_emptyTextField(MSU_koModel.DateOfRental()))) {
            document.getElementById("E_DateOfRental").className = "col-sm-6 has-error";
            toastr.error("If 'Mug In Use' is blank, then 'Date Of Rental' cannot be filled in.");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_DateOfRental").className = "col-sm-6 has-success";
        }
    }

    // Notes change check
    if (MSU_koModel.Notes() != MSU_Initial.Notes) {
        changeDetected = true;
    }

    if (changeDetected && validationSuccessful) {
        MSU_edit_ajax();
        MSU_Table_Data_Ajax();
    }
    else if (!changeDetected) {
        toastr.error("No changes detected.");
    }
}

/*
    Function: MSU_emptyTextField

    Checks to see if knockout value is null or ""

    Parameters:

        fieldName - string from knockout model to be checked for null or "" value

    Returns:

        true - if the value in the text field is null or ""
        false - if the text field contains a value other than null or ""
*/
function MSU_emptyTextField(fieldName) {
    if (fieldName == null || fieldName == "")
        return true;
    return false;
}


/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -MSU_Table_Data_Ajax()
        -MSU_excel_upload_ajax()
        -MSU_GetRecord()
        -MSU_create_ajax()
        -MSU_edit_ajax()
        -MSU_delete_ajax()
        -MSU_StudentNumberValidator()
        -MSU_EmailValidator()

----------------------------------------------------------------------------------------*/
/*
    Function: MSU_Table_Data_Ajax

    Ajax call that gathers the data needed to fill the data table then calls functions
    to build the table or destroy and rebuild the table.
*/
function MSU_Table_Data_Ajax() {
    $.ajax({
        url: MSU_Table_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            if (MSU_table !== null && typeof MSU_table !== 'undefined') {
                MSU_table.destroy();
                MSU_table = null;
                $('#MSU_table').replaceWith('<table id="MSU_Table" class="table table-striped table-bordered" cellpadding="0" cellspacing="0" border="0"></table>');
            };
            MSU_DataTable(data);
        }
    });
}

/*
    Function: MSU_excel_upload_ajax

    Ajax call that submits file path for excel document and starts the import process
    taking the excel data and inserting it into the MugShareUsers datatable in the
    database.
*/
function MSU_excel_upload_ajax() {
    var jsonString = { "ExcelPath": MSU_koModel.ExcelPath() };

    $.ajax({
        url: MSU_Excel_URL,
        cache: false,
        async: false,
        data: jsonString,
        dataType: "json",
        success: function (data) {
            if (data.boolVal == true) {
                $('#MSU_upload').modal("hide");
                toastr.success("Mug share users data has been successfully replaced with data from the selected excel document");
                MSU_Table_Data_Ajax();
            }
            else {
                toastr.error("An error occured when transfering the data. Please ensure that you have selected an excel document uses the required format.");
            }
        }
    });
}

/*
    Function: MSU_GetRecord

    Ajax call that retrieves all the data related to the record with specified
    UserKey, then copies the data to the knockout model, as well as the
    MSU_Initial model.

    Parameters:

        UserKey - The primary key of a selected record.
*/
function MSU_GetRecord(UserKey) {
    var jsonString = { "UserKey": UserKey };

    $.ajax({
        url: MSU_GetRecord_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            MSU_koModel.UserKey(data.UserKey);
            MSU_koModel.StudentNumber(data.StudentNumber);
            MSU_koModel.FirstName(data.FirstName);
            MSU_koModel.LastName(data.LastName);
            MSU_koModel.Email(data.Email);
            MSU_koModel.MugInUse(data.MugInUse);
            MSU_koModel.DateOfRental(data.DateOfRental);
            MSU_koModel.TotalMugsBorrowed(data.TotalMugsBorrowed);
            MSU_koModel.Notes(data.Notes);

            initialize_MSU_Initial();
        }
    });
}

/*
    Function: MSU_create_ajax

    Ajax call that passes all the knockout data to the server side to create a new
    user and save it to the server.
*/
function MSU_create_ajax() {
    var jsonString = {
        "StudentNumber": MSU_koModel.StudentNumber(),
        "FirstName": MSU_koModel.FirstName(),
        "LastName": MSU_koModel.LastName(),
        "Email": MSU_koModel.Email(),
        "Notes": MSU_koModel.Notes()
    };

    $.ajax({
        url: MSU_CreateRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#MSU_create').modal("hide");
                toastr.success("New user was successfully created!");
            }
            else {
                toastr.error("An error occurred while creating the new user. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: MSU_edit_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    user and save it to the server.
*/
function MSU_edit_ajax() {
    var jsonString = {
        "UserKey": MSU_koModel.UserKey(),
        "StudentNumber": MSU_koModel.StudentNumber(),
        "FirstName": MSU_koModel.FirstName(),
        "LastName": MSU_koModel.LastName(),
        "Email": MSU_koModel.Email(),
        "MugInUse": MSU_koModel.MugInUse(),
        "DateOfRental": MSU_koModel.DateOfRental(),
        "Notes": MSU_koModel.Notes()
    };

    $.ajax({
        url: MSU_EditRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#MSU_edit').modal("hide");
                toastr.success("User record was updated successfully!");
            }
            else {
                toastr.error("An error occurred while updating the user record. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: MSU_delete_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    user and save it to the server.
*/
function MSU_delete_ajax() {
    var jsonString = { "UserKey": MSU_koModel.UserKey() };

    $.ajax({
        url: MSU_DeleteRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#MSU_delete').modal("hide");
                toastr.success("User record was deleted successfully!");
            }
            else {
                toastr.error("An error occurred while deleting the user record. Please try again");
            }
        }
    });
}

/*
    Function: MSU_StudentNumberValidator

    Ajax call to validate that the student number is unique.
*/
function MSU_StudentNumberValidator() {
    var boolVal = false;
    var jsonString = { "StudentNumber": MSU_koModel.StudentNumber() };

    $.ajax({
        url: MSU_StudentNumber_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                boolVal = data.boolVal;
            }
            else {
                toastr.error("Student number already exists.");
            }
        }
    });

    return boolVal;
}

/*
    Function: MSU_EmailValidator

    Ajax call to validate that the email is unique.
*/
function MSU_EmailValidator() {
    var boolVal = false;
    var jsonString = { "Email": MSU_koModel.Email() };

    $.ajax({
        url: MSU_Email_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                boolVal = data.boolVal;
            }
            else {
                toastr.error("Email already exists.");
            }
        }
    });

    return boolVal;
}