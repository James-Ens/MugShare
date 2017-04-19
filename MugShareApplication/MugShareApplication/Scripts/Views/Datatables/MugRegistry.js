// Initializes a model of knockout observables used in the html
var MR_koModel = {
    Admin: ko.observable(true),

    MugKey: ko.observable(null),
    MugID: ko.observable(null),
    LastBorrowedBy: ko.observable(null),
    CurrentlyInUse: ko.observable(false),
    Notes: ko.observable(null)
}

// Initializes a model that will later be used to check if changes
// have occurred in the knockout model
var MR_Initial = {
    MugID: null,
    Notes: null
}

// Variable that refers to the displayed data table
var MR_table = null;

/*--------------------------------------------------------------------------------------
    Document ready function:
    
        -Checks security permissions
        -Applys the knockout bindings to html section in MugRegistry.cshtml
        -References an ajax call to build the data table displayed in the view
        -Resizes it to fit the size of the window
----------------------------------------------------------------------------------------*/
$(document).ready(function () {
    ko.applyBindings(MR_koModel, document.getElementById("MugRegistry"));
    MR_Table_Data_Ajax();

    //$(window).resize(function () {
    //    MR_RedrawTable(220);
    //});
});

function MR_DataTable(data) {
    MR_table = $('#MR_Table').DataTable({
        "data": data,
        "pageLength": 10,
        "lengthChange": false,
        "pageType": "full_numbers",
        "columns": [
            { "title": "Mug ID", "data": "MugID", "classname": "alignleft", "width": "33%" },
            { "title": "Currently In Use", "data": "CurrentlyInUse", "classname": "alignleft", "width": "32%" },
            { "title": "", "data": "buttons", "classname": "center-block", "searchable": false, "orderable": false, "width": "35%" }
        ]
    });
}


/*--------------------------------------------------------------------------------------
    CREATE MODAL FUNCTIONS:

        -MR_create()
        -MR_create_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MR_create

    Opens the create mug modal, and clears the knockout model so that
    all the fields are empty
*/
function MR_create() {
    MR_clear_model();
    $('#MR_create').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });
}

/*
    Function: MR_create_submit

    Submits new record to the database
*/
function MR_create_submit() {
    toastr.clear();
    MR_create_field_check();
}

/*--------------------------------------------------------------------------------------
    READ MODAL FUNCTIONS:

        -MR_read()

----------------------------------------------------------------------------------------*/
/*
    Function: MR_read

    Opens the read mug modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        MugKey - The primary key of the record selected
*/
function MR_read(MugKey) {
    MR_clear_model();
    $('#MR_read').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    MR_GetRecord(MugKey);
}

/*--------------------------------------------------------------------------------------
    EDIT MODAL FUNCTIONS:

        -MR_edit()
        -MR_edit_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MR_edit

    Opens the edit mug modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        MugKey - The primary key of the record selected
*/
function MR_edit(MugKey) {
    MR_clear_model();
    $('#MR_edit').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    MR_GetRecord(MugKey);
}

/*
    Function: MR_edit_submit

    Submits updated record to the database
*/
function MR_edit_submit() {
    toastr.clear();
    MR_edit_field_check();
}

/*--------------------------------------------------------------------------------------
    DELETE MODAL FUNCTIONS:

        -MR_delete()
        -MR_delete_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: MR_delete

    Opens the delete mug modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        MugKey - The primary key of the record selected
*/
function MR_delete(MugKey) {
    MR_clear_model();
    $('#MR_delete').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    MR_GetRecord(MugKey);
}

/*
    Function: MR_delete_submit

    Submits command to the database to delete the selected record
*/
function MR_delete_submit() {
    MR_delete_ajax();
    MR_Table_Data_Ajax();
}


/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -initialize_MR_Initial()
        -MR_clear_model()
        -MR_create_field_check()
        -MR_edit_field_check()
        -MR_emptyTextField()

----------------------------------------------------------------------------------------*/
/*
    Function: initialize_MR_Initial

    Copies the initial values of the knockout model to the MR_Initial model
    so that it can be compared to later to check for changes.
*/
function initialize_MR_Initial() {
    MR_Initial.MugID = MR_koModel.MugID();
    MR_Initial.Notes = MR_koModel.Notes();
}

/*
    Function: MR_clear_model

    Clears both the knockout model and the MR_Initial model
*/
function MR_clear_model() {
    MR_koModel.MugKey(null),
    MR_koModel.MugID(null),
    MR_koModel.LastBorrowedBy(null),
    MR_koModel.CurrentlyInUse(false),
    MR_koModel.Notes(null),

    MR_Initial = {
        MugID: null,
        Notes: null,
    };

    toastr.clear(); // Clear all open toastr alerts when opening a modal
}

/*
    Function: MR_create_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before creating the new record with this data in the database
*/
function MR_create_field_check() {
    var validationSuccessful = true;

    // Mug ID validation step
    if (MR_emptyTextField(MR_koModel.MugID()) || MR_koModel.MugID().length != 12) {
        toastr.error("Invalid entry for 'Mug ID'.");
        validationSuccessful = false;
    }
    else if (!MR_MugIDValidator()) {
        validationSuccessful = false;
    }

    if (validationSuccessful) {
        MR_create_ajax();
        MR_Table_Data_Ajax();
    }
}

/*
    Function: MR_edit_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before updating the record with this data in the database. Also checks to
    make sure that changes have actually occurred to save server side processing
    time
*/
function MR_edit_field_check() {
    var validationSuccessful = true;
    var changeDetected = false;

    // Mug ID change/validation step
    if (MR_koModel.MugID() != MR_Initial.MugID) {
        changeDetected = true;

        if (MR_emptyTextField(MR_koModel.MugID()) || MR_koModel.MugID().length != 12) {
            toastr.error("Invalid entry for 'Mug ID'.");
            validationSuccessful = false;
        }
        else if (!MR_MugIDValidator()) {
            validationSuccessful = false;
        }
    }

    // Notes change check
    if (MR_koModel.Notes() != MR_koModel.Notes) {
        changeDetected = true;
    }

    if (changeDetected && validationSuccessful) {
        MR_edit_ajax();
        MR_Table_Data_Ajax();
    }
    else if (!changeDetected) {
        toastr.error("No changes detected.");
    }
}

/*
    Function: MR_emptyTextField

    Checks to see if knockout value is null or ""

    Parameters:

        fieldName - string from knockout model to be checked for null or "" value

    Returns:

        true - if the value in the text field is null or ""
        false - if the text field contains a value other than null or ""
*/
function MR_emptyTextField(fieldName) {
    if (fieldName == null || fieldName == "")
        return true;
    return false;
}


/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -MR_Table_Data_Ajax()
        -MR_GetRecord()
        -MR_create_ajax()
        -MR_edit_ajax()
        -MR_delete_ajax()
        -MR_MugIDValidator()

----------------------------------------------------------------------------------------*/
/*
    Function: MR_Table_Data_Ajax

    Ajax call that gathers the data needed to fill the data table then calls functions
    to build the table or destroy and rebuild the table.
*/
function MR_Table_Data_Ajax() {
    $.ajax({
        url: MR_Table_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            if (MR_table !== null && typeof MR_table !== 'undefined') {
                MR_table.destroy();
                MR_table = null;
                $('#MR_table').replaceWith('<table id="MR_Table" class="table table-striped table-bordered" cellpadding="0" cellspacing="0" border="0"></table>');
            };
            MR_DataTable(data);
        }
    });
}

/*
    Function: MR_GetRecord

    Ajax call that retrieves all the data related to the record with specified
    MugKey, then copies the data to the knockout model, as well as the
    MR_Initial model.

    Parameters:

        MugKey - The primary key of a selected record.
*/
function MR_GetRecord(MugKey) {
    var jsonString = { "MugKey": MugKey };

    $.ajax({
        url: MR_GetRecord_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            MR_koModel.MugKey(data.MugKey);
            MR_koModel.MugID(data.MugID);
            MR_koModel.LastBorrowedBy(data.LastBorrowedBy);
            MR_koModel.CurrentlyInUse(data.CurrentlyInUse);
            MR_koModel.Notes(data.Notes);

            initialize_MR_Initial();
        }
    });
}

/*
    Function: MR_create_ajax

    Ajax call that passes all the knockout data to the server side to add a new
    mug and save it to the server.
*/
function MR_create_ajax() {
    var jsonString = { "MugID": MR_koModel.MugID() };

    $.ajax({
        url: MR_CreateRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                MR_koModel.MugID(null);
                toastr.success("New mug was added successfully!");
            }
            else {
                toastr.error("An error occurred while adding the new mug. Please make sure the field is filled out correctly.");
            }
        }
    });
}

/*
    Function: MR_edit_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    mug record and save it to the server.
*/
function MR_edit_ajax() {
    var jsonString = {
        "MugKey": MR_koModel.MugKey(),
        "MugID": MR_koModel.MugID(),
        "CurrentlyInUse": MR_koModel.CurrentlyInUse(),
        "Notes": MR_koModel.Notes()
    };

    $.ajax({
        url: MR_EditRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#MR_edit').modal("hide");
                toastr.success("Mug record was updated successfully!");
            }
            else {
                toastr.error("An error occurred while updating the mug record. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: MR_delete_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    mug record and save it to the server.
*/
function MR_delete_ajax() {
    var jsonString = { "MugKey": MR_koModel.MugKey() };

    $.ajax({
        url: MR_DeleteRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#MR_delete').modal("hide");
                toastr.success("Mug record was deleted successfully!");
            }
            else {
                toastr.error("An error occurred while deleting the mug record. Please try again.");
            }
        }
    });
}

/*
    Function: MR_MugIDValidator

    Ajax call to validate that the mug ID is unique.
*/
function MR_MugIDValidator() {
    var boolVal = false;
    var jsonString = { "MugID": MR_koModel.MugID() };

    $.ajax({
        url: MR_MugID_URL,
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
                toastr.error("Mug ID already exists.");
                MR_koModel.MugID(null);
            }
        }
    });

    return boolVal;
}

/*--------------------------------------------------------------------------------------
        RFID FUNCTIONS:

            -MR_getRFID()

----------------------------------------------------------------------------------------*/
/*
    Function: MR_getRFID

    Get the RFID of the mug to be registered
*/
function MR_getRFID() {
    $.ajax({
        url: MR_GetRFID_URL,
        data: "",
        cache: false,
        type: "GET",
        async: false,
        dataType: "text",
        success: function (data) {
            if (data != "") {
                MR_koModel.MugID(data);
            }
            else {
                toastr.error("RFID scanner timed out.");
                MR_koModel.MugID(null);
                return;
            }
        }
    });
}