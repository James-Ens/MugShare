// Initializes a model of knockout observables used in the html
var S_koModel = {
    Admin: ko.observable(true),

    SecurityKey: ko.observable(null),
    FirstName: ko.observable(null),
    LastName: ko.observable(null),
    StaffCardID: ko.observable(null),
    Username: ko.observable(null),
    Password: ko.observable(null),
    ReTypePassword: ko.observable(null),
    Email: ko.observable(null),
    AdminPermission: ko.observable(false)
}

// Initializes a model that will later be used to check if changes
// have occurred in the knockout model
var S_Initial = {
    FirstName: null,
    LastName: null,
    StaffCardID: null,
    Username: null,
    Email: null,
    AdminPermission: false
}

// Variable that refers to the displayed data table
var S_table = null;

/*--------------------------------------------------------------------------------------
    Document ready function:
    
        -Checks security permissions
        -Applys the knockout bindings to html section in MugShareUsers.cshtml
        -References an ajax call to build the data table displayed in the view
        -Resizes it to fit the size of the window
----------------------------------------------------------------------------------------*/
$(document).ready(function () {
    ko.applyBindings(S_koModel, document.getElementById("Security"));
    S_Table_Data_Ajax();

    //$(window).resize(function () {
    //    S_RedrawTable(220);
    //});
});

function S_DataTable(data) {
    S_table = $('#S_Table').DataTable({
        "data": data,
        "pageLength": 10,
        "lengthChange": false,
        "pageType": "full_numbers",
        "columns": [
            { "title": "Username", "data": "Username", "width": "20%" },
            { "title": "Email", "data": "Email", "width": "35%" },
            { "title": "Admin Permission", "data": "AdminPermission", "width": "25%" },
            { "title": "", "data": "buttons", "searchable": false, "orderable": false, "width": "20%" }
        ]
    });
}


/*--------------------------------------------------------------------------------------
    CREATE MODAL FUNCTIONS:

        -S_create()
        -S_create_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: S_create

    Opens the create staff user modal, and clears the knockout model so that
    all the fields are empty
*/
function S_create() {
    S_clear_model();
    S_createDefaultTextFields();
    $('#S_create').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });
}

/*
    Function: S_create_submit

    Submits new record to the database
*/
function S_create_submit() {
    toastr.clear();
    S_stringCorrections();
    S_create_field_check();
}

/*--------------------------------------------------------------------------------------
    READ MODAL FUNCTIONS:

        -S_read()

----------------------------------------------------------------------------------------*/
/*
    Function: S_read

    Opens the read staff user modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        SecurityKey - The primary key of the record selected
*/
function S_read(SecurityKey) {
    S_clear_model();
    $('#S_read').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    S_GetRecord(SecurityKey);
}

/*--------------------------------------------------------------------------------------
    EDIT MODAL FUNCTIONS:

        -S_edit()
        -S_edit_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: S_edit

    Opens the edit staff user modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        SecurityKey - The primary key of the record selected
*/
function S_edit(SecurityKey) {
    S_clear_model();
    S_editDefaultTextFields();
    $('#S_edit').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    S_GetRecord(SecurityKey);
}

/*
    Function: S_edit_submit

    Submits updated record to the database
*/
function S_edit_submit() {
    toastr.clear();
    S_stringCorrections();
    S_editDefaultTextFields();
    S_edit_field_check();
}

/*--------------------------------------------------------------------------------------
    DELETE MODAL FUNCTIONS:

        -S_delete()
        -S_delete_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: S_delete

    Opens the delete staff user modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        SecurityKey - The primary key of the record selected
*/
function S_delete(SecurityKey) {
    S_clear_model();
    $('#S_delete').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    S_GetRecord(SecurityKey);
}

/*
    Function: S_delete_submit

    Submits command to the database to delete the selected record
*/
function S_delete_submit() {
    S_delete_ajax();
    S_Table_Data_Ajax();
}


/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -initialize_S_Initial()
        -S_clear_model()
        -S_stringCorrections()
        -S_createDefaultTextFields()
        -S_editDefaultTextFields()
        -S_create_field_check()
        -S_edit_field_check()
        -S_emptyTextField()

----------------------------------------------------------------------------------------*/
/*
    Function: initialize_S_Initial

    Copies the initial values of the knockout model to the S_Initial model
    so that it can be compared to later to check for changes.
*/
function initialize_S_Initial() {
    S_Initial.FirstName = S_koModel.FirstName();
    S_Initial.LastName = S_koModel.LastName();
    S_Initial.StaffCardID = S_koModel.StaffCardID();
    S_Initial.Username = S_koModel.Username();
    S_Initial.Email = S_koModel.Email();
    S_Initial.AdminPermission = S_koModel.AdminPermission();
}

/*
    Function: S_clear_model

    Clears both the knockout model and the S_Initial model
*/
function S_clear_model() {
    S_koModel.SecurityKey(null),
    S_koModel.FirstName(null),
    S_koModel.LastName(null),
    S_koModel.StaffCardID(null),
    S_koModel.Username(null),
    S_koModel.Password(null),
    S_koModel.ReTypePassword(null),
    S_koModel.Email(null),
    S_koModel.AdminPermission(false),

    S_Initial = {
        FirstName: null,
        LastName: null,
        StaffCardID: null,
        Username: null,
        Email: null,
        AdminPermission: false
    };

    toastr.clear(); // Clear all open toastr alerts when opening a modal
}

/*
    Function: S_stringCorrections

    Trims unnecessary spaces from beginning and end of first and last name entries
*/
function S_stringCorrections() {
    if (S_koModel.FirstName() != null) { S_koModel.FirstName(S_koModel.FirstName().trim()); }
    if (S_koModel.LastName() != null) { S_koModel.LastName(S_koModel.LastName().trim()); }
}

/*
    Function: S_createDefaultTextFields

    Sets the border color of the create modal text fields to the default color
*/
function S_createDefaultTextFields() {
    document.getElementById("C_FirstName").className = "col-sm-6";
    document.getElementById("C_LastName").className = "col-sm-6";
    document.getElementById("C_StaffCardID").className = "col-sm-6";
    document.getElementById("C_Username").className = "col-sm-6";
    document.getElementById("C_Password").className = "col-sm-6";
    document.getElementById("C_ReTypePassword").className = "col-sm-6";
    document.getElementById("C_Email").className = "col-sm-6";
}

/*
    Function: S_editDefaultTextFields

    Sets the border color of the edit modal text fields to the default color
*/
function S_editDefaultTextFields() {
    document.getElementById("E_FirstName").className = "col-sm-6";
    document.getElementById("E_LastName").className = "col-sm-6";
    document.getElementById("E_StaffCardID").className = "col-sm-6";
    document.getElementById("E_Username").className = "col-sm-6";
    document.getElementById("E_Email").className = "col-sm-6";
}

/*
    Function: S_create_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before creating the new record with this data in the database
*/
function S_create_field_check() {
    var validationSuccessful = true;

    // First Name validation step
    if (S_emptyTextField(S_koModel.FirstName())) {
        document.getElementById("C_FirstName").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'First Name'.");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_FirstName").className = "col-sm-6 has-success";
    }

    // Last Name validation step
    if (S_emptyTextField(S_koModel.LastName())) {
        document.getElementById("C_LastName").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Last Name'.");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_LastName").className = "col-sm-6 has-success";
    }

    // Staff Card ID validation step
    if (S_emptyTextField(S_koModel.StaffCardID()) || S_koModel.StaffCardID().length < 7 || S_koModel.StaffCardID().length > 8) {
        document.getElementById("C_StaffCardID").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Staff Card ID'.");
        validationSuccessful = false;
    }
    else if (!S_StaffCardIDValidator()) {
        document.getElementById("C_StaffCardID").className = "col-sm-6 has-error";
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_StaffCardID").className = "col-sm-6 has-success";
    }

    // Username validation step
    if (S_emptyTextField(S_koModel.Username())) {
        document.getElementById("C_Username").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Username'.");
        validationSuccessful = false;
    }
    else if (!S_UsernameValidator()) {
        document.getElementById("C_Username").className = "col-sm-6 has-error";
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_Username").className = "col-sm-6 has-success";
    }

    // Password validation step
    if (S_emptyTextField(S_koModel.Password())) {
        document.getElementById("C_Password").className = "col-sm-6 has-error";
        document.getElementById("C_ReTypePassword").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Password'.");
        validationSuccessful = false;
    }
    else if (S_koModel.Password() != S_koModel.ReTypePassword()) {
        document.getElementById("C_Password").className = "col-sm-6 has-error";
        document.getElementById("C_ReTypePassword").className = "col-sm-6 has-error";
        toastr.error("'Password' and 'Re-Type Password' fields don't match.");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_Password").className = "col-sm-6 has-success";
        document.getElementById("C_ReTypePassword").className = "col-sm-6 has-success";
    }

    // Email validation step
    if (S_emptyTextField(S_koModel.Email())) {
        document.getElementById("C_Email").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Email'.");
        validationSuccessful = false;
    }
    else if (!S_EmailValidator()) {
        document.getElementById("C_Email").className = "col-sm-6 has-error";
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_Email").className = "col-sm-6 has-success";
    }

    if (validationSuccessful) {
        S_create_ajax();
        S_Table_Data_Ajax();
    }
}

/*
    Function: S_edit_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before updating the record with this data in the database. Also checks to
    make sure that changes have actually occurred to save server side processing
    time
*/
function S_edit_field_check() {
    var validationSuccessful = true;
    var changeDetected = false;

    // First Name change/validation step
    if (S_koModel.FirstName() != S_Initial.FirstName) {
        changeDetected = true;

        if (S_emptyTextField(S_koModel.FirstName())) {
            document.getElementById("E_FirstName").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'First Name'.");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_FirstName").className = "col-sm-6 has-success";
        }
    }

    // Last Name change/validation step
    if (S_koModel.LastName() != S_Initial.LastName) {
        changeDetected = true;

        if (S_emptyTextField(S_koModel.LastName())) {
            document.getElementById("E_LastName").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Last Name'.");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_LastName").className = "col-sm-6 has-success";
        }
    }

    // Staff Card ID change/validation step
    if (S_koModel.StaffCardID() != S_Initial.StaffCardID) {
        changeDetected = true;

        if (S_emptyTextField(S_koModel.StaffCardID()) || S_koModel.StaffCardID().length < 7 || S_koModel.StaffCardID().length > 8) {
            document.getElementById("E_StaffCardID").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Staff Card ID'.");
            validationSuccessful = false;
        }
        else if (!S_StaffCardIDValidator()) {
            document.getElementById("E_StaffCardID").className = "col-sm-6 has-error";
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_StaffCardID").className = "col-sm-6 has-success";
        }
    }

    // Username change/validation step
    if (S_koModel.Username() != S_Initial.Username) {
        changeDetected = true;

        if (S_emptyTextField(S_koModel.Username())) {
            document.getElementById("E_Username").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Username'.");
            validationSuccessful = false;
        }
        else if (!S_UsernameValidator()) {
            document.getElementById("E_Username").className = "col-sm-6 has-error";
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_Username").className = "col-sm-6 has-success";
        }
    }

    // Email change/validation step
    if (S_koModel.Email() != S_Initial.Email) {
        changeDetected = true;

        if (S_emptyTextField(S_koModel.Email())) {
            document.getElementById("E_Email").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Email'.");
            validationSuccessful = false;
        }
        else if (!S_EmailValidator()) {
            document.getElementById("E_Email").className = "col-sm-6 has-error";
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_Email").className = "col-sm-6 has-success";
        }
    }

    // Admin Permission change/validation step
    if (S_koModel.AdminPermission() != S_Initial.AdminPermission) {
        changeDetected = true;
    }

    if (changeDetected && validationSuccessful) {
        S_edit_ajax();
        S_Table_Data_Ajax();
    }
    else if (!changeDetected) {
        toastr.error("No changes detected.");
    }
}

/*
    Function: S_emptyTextField

    Checks to see if knockout value is null or ""

    Parameters:

        fieldName - string from knockout model to be checked for null or "" value

    Returns:

        true - if the value in the text field is null or ""
        false - if the text field contains a value other than null or ""
*/
function S_emptyTextField(fieldName) {
    if (fieldName == null || fieldName == "")
        return true;
    return false;
}


/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -S_Table_Data_Ajax()
        -S_GetRecord()
        -S_create_ajax()
        -S_edit_ajax()
        -S_delete_ajax()
        -S_UsernameValidator()
        -S_EmailValidator()

----------------------------------------------------------------------------------------*/
/*
    Function: S_Table_Data_Ajax

    Ajax call that gathers the data needed to fill the data table then calls functions
    to build the table or destroy and rebuild the table.
*/
function S_Table_Data_Ajax() {
    $.ajax({
        url: S_Table_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            if (S_table !== null && typeof S_table !== 'undefined') {
                S_table.destroy();
                S_table = null;
                $('#S_table').replaceWith('<table id="S_Table" class="table table-striped table-bordered" cellpadding="0" cellspacing="0" border="0"></table>');
            };
            S_DataTable(data);
        }
    });
}

/*
    Function: S_GetRecord

    Ajax call that retrieves all the data related to the record with specified
    SecurityKey, then copies the data to the knockout model, as well as the
    S_Initial model.

    Parameters:

        SecurityKey - The primary key of a selected record.
*/
function S_GetRecord(SecurityKey) {
    var jsonString = { "SecurityKey": SecurityKey };

    $.ajax({
        url: S_GetRecord_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            S_koModel.SecurityKey(data.SecurityKey);
            S_koModel.FirstName(data.FirstName);
            S_koModel.LastName(data.LastName);
            S_koModel.StaffCardID(data.StaffCardID);
            S_koModel.Username(data.Username);
            S_koModel.Email(data.Email);
            S_koModel.AdminPermission(data.AdminPermission);

            initialize_S_Initial();
        }
    });
}

/*
    Function: S_create_ajax

    Ajax call that passes all the knockout data to the server side to create a new
    staff user and save it to the server.
*/
function S_create_ajax() {
    var jsonString = {
        "FirstName": S_koModel.FirstName(),
        "LastName": S_koModel.LastName(),
        "StaffCardID": S_koModel.StaffCardID(),
        "Username": S_koModel.Username(),
        "Password": S_koModel.Password(),
        "Email": S_koModel.Email(),
        "AdminPermission": S_koModel.AdminPermission()
    };

    $.ajax({
        url: S_CreateRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#S_create').modal("hide");
                toastr.success("New staff user was successfully created!");
            }
            else {
                toastr.error("An error occurred while creating the new staff user. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: S_edit_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    staff user and save it to the server.
*/
function S_edit_ajax() {
    var jsonString = {
        "SecurityKey": S_koModel.SecurityKey(),
        "FirstName": S_koModel.FirstName(),
        "LastName": S_koModel.LastName(),
        "StaffCardID": S_koModel.StaffCardID(),
        "Username": S_koModel.Username(),
        "Email": S_koModel.Email(),
        "AdminPermission": S_koModel.AdminPermission()
    };

    $.ajax({
        url: S_EditRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#S_edit').modal("hide");
                toastr.success("Staff user record was updated successfully!");
            }
            else {
                toastr.error("An error occurred while updating the staff user record. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: S_delete_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    staff user and save it to the server.
*/
function S_delete_ajax() {
    var jsonString = { "SecurityKey": S_koModel.SecurityKey() };

    $.ajax({
        url: S_DeleteRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#S_delete').modal("hide");
                toastr.success("Staff user record was deleted successfully!");
            }
            else {
                toastr.error("An error occurred while deleting the staff user record. Please try again.");
            }
        }
    });
}

/*
    Function: S_StaffCardIDValidator

    Ajax call to validate that the staff card ID is unique.
*/
function S_StaffCardIDValidator() {
    var boolVal = false;
    var jsonString = { "StaffCardID": S_koModel.StaffCardID() };

    $.ajax({
        url: S_StaffCardID_URL,
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
                toastr.error("Staff card ID already exists.");
            }
        }
    });

    return boolVal;
}

/*
    Function: S_UsernameValidator

    Ajax call to validate that the username is unique.
*/
function S_UsernameValidator() {
    var boolVal = false;
    var jsonString = { "Username": S_koModel.Username() };

    $.ajax({
        url: S_Username_URL,
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
                toastr.error("Username already exists.");
            }
        }
    });

    return boolVal;
}

/*
    Function: S_EmailValidator

    Ajax call to validate that the email is unique.
*/
function S_EmailValidator() {
    var boolVal = false;
    var jsonString = { "Email": S_koModel.Email() };

    $.ajax({
        url: S_Email_URL,
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