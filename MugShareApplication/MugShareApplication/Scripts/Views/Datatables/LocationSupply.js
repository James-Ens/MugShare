var Hour = function (text, value) {
    this.hourText = text;
    this.hourValue = value;
};

// Initializes a model of knockout observables used in the html
var LS_koModel = {
    Admin: ko.observable(true),

    MachineKey: ko.observable(null),
    MachineID: ko.observable(null),
    MachineLocation: ko.observable(null),
    OpeningHourList: ko.observableArray([new Hour("12am", 0), new Hour("1am", 1), new Hour("2am", 2), new Hour("3am", 3), new Hour("4am", 4), new Hour("5am", 5), new Hour("6am", 6),
        new Hour("7am", 7), new Hour("8am", 8), new Hour("9am", 9), new Hour("10am", 10), new Hour("11am", 11), new Hour("12pm", 12), new Hour("1pm", 13), new Hour("2pm", 14), new Hour("3pm", 15),
        new Hour("4pm", 16), new Hour("5pm", 17), new Hour("6pm", 18), new Hour("7pm", 19), new Hour("8pm", 20), new Hour("9pm", 21), new Hour("10pm", 22), new Hour("11pm", 23)]),
    OpeningHour: ko.observable(null),
    ClosingHourList: ko.observableArray([new Hour("12am", 0), new Hour("1am", 1), new Hour("2am", 2), new Hour("3am", 3), new Hour("4am", 4), new Hour("5am", 5), new Hour("6am", 6),
        new Hour("7am", 7), new Hour("8am", 8), new Hour("9am", 9), new Hour("10am", 10), new Hour("11am", 11), new Hour("12pm", 12), new Hour("1pm", 13), new Hour("2pm", 14), new Hour("3pm", 15),
        new Hour("4pm", 16), new Hour("5pm", 17), new Hour("6pm", 18), new Hour("7pm", 19), new Hour("8pm", 20), new Hour("9pm", 21), new Hour("10pm", 22), new Hour("11pm", 23)]),
    ClosingHour: ko.observable(null),
    CurrentSupply: ko.observable(null),
    TotalCapacity: ko.observable(null),
    TotalMugsDispensed: ko.observable(null),
    OutOfOrder: ko.observable(false),
    Notes: ko.observable(null)
}

// Initializes a model that will later be used to check if changes
// have occurred in the knockout model
var LS_Initial = {
    MachineID: null,
    MachineLocation: null,
    OpeningHour: null,
    ClosingHour: null,
    CurrentSupply: null,
    TotalCapacity: null,
    OutOfOrder: false,
    Notes: null
}

// Variable that refers to the displayed data table
var LS_table = null;

// Hourly Statistics chart information
var hourlyChart_x_label = 'Hour (24-hour)'
var hourlyChart_y_label = 'Number of Mugs Rented'

/*--------------------------------------------------------------------------------------
    Document ready function:
    
        -Checks security permissions
        -Applys the knockout bindings to html section in LocationSupply.cshtml
        -References an ajax call to build the data table displayed in the view
        -Resizes it to fit the size of the window
----------------------------------------------------------------------------------------*/
$(document).ready(function () {
    ko.applyBindings(LS_koModel, document.getElementById("LocationSupply"));
    LS_Table_Data_Ajax();

    //$(window).resize(function () {
    //    LS_RedrawTable(220);
    //});
});

function LS_DataTable(data) {
    LS_table = $('#LS_Table').DataTable({
        "data": data,
        "pageLength": 10,
        "lengthChange": false,
        "pageType": "full_numbers",
        "columns": [
            { "title": "Machine ID", "data": "MachineID", "width": "10%" },
            { "title": "Machine Location", "data": "MachineLocation", "width": "30%" },
            { "title": "Current Supply", "data": "CurrentSupplyPercentage", "width": "15%" },
            { "title": "In Service", "data": "OutOfOrder", "width": "15%" },
            { "title": "", "data": "buttons", "searchable": false, "orderable": false, "width": "30%" }
        ]
    });
}


/*--------------------------------------------------------------------------------------
    CREATE MODAL FUNCTIONS:

        -LS_create()
        -LS_create_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: LS_create

    Opens the create machine modal, and clears the knockout model so that
    all the fields are empty
*/
function LS_create() {
    LS_clear_model();
    LS_createDefaultTextFields();
    $('#LS_create').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });
}

/*
    Function: LS_create_submit

    Submits new record to the database
*/
function LS_create_submit() {
    toastr.clear();
    LS_stringCorrections();
    LS_create_field_check();
}

/*--------------------------------------------------------------------------------------
    READ MODAL FUNCTIONS:

        -LS_read()

----------------------------------------------------------------------------------------*/
/*
    Function: LS_read

    Opens the read machine modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        MachineKey - The primary key of the record selected
*/
function LS_read(MachineKey) {
    LS_clear_model();
    $('#LS_read').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    LS_GetRecord(MachineKey);
}

/*--------------------------------------------------------------------------------------
    CHART MODAL FUNCTIONS:

        -LS_chart()

----------------------------------------------------------------------------------------*/
/*
    Function: LS_chart

    Opens the machine chart modal and draws a bar chart with collected data.

    Parameters:

        MachineID - The machine ID of the record selected
*/
function LS_chart(MachineID, OpeningHour, ClosingHour) {
    $('#LS_chart').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    LS_HourlyChartDataAjax(MachineID, OpeningHour, ClosingHour);
}

/*--------------------------------------------------------------------------------------
    EDIT MODAL FUNCTIONS:

        -LS_edit()
        -LS_edit_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: LS_edit

    Opens the edit machine modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        MachineKey - The primary key of the record selected
*/
function LS_edit(MachineKey) {
    LS_clear_model();
    LS_editDefaultTextFields();
    $('#LS_edit').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    LS_GetRecord(MachineKey);
}

/*
    Function: LS_edit_submit

    Submits updated record to the database
*/
function LS_edit_submit() {
    toastr.clear();
    LS_stringCorrections();
    LS_editDefaultTextFields();
    LS_edit_field_check();
}

/*--------------------------------------------------------------------------------------
    DELETE MODAL FUNCTIONS:

        -LS_delete()
        -LS_delete_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: LS_delete

    Opens the delete machine modal and fills the knockout model with the values
    in the selected record, which are then displayed in their respective fields.

    Parameters:

        MachineKey - The primary key of the record selected
*/
function LS_delete(MachineKey) {
    LS_clear_model();
    $('#LS_delete').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });;
    LS_GetRecord(MachineKey);
}

/*
    Function: LS_delete_submit

    Submits command to the database to delete the selected record
*/
function LS_delete_submit() {
    LS_delete_ajax();
    LS_Table_Data_Ajax();
}


/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -initialize_LS_Initial()
        -LS_clear_model()
        -LS_stringCorrections()
        -LS_createDefaultTextFields()
        -LS_editDefaultTextFields()
        -LS_create_field_check()
        -LS_edit_field_check()
        -LS_emptyTextField()

----------------------------------------------------------------------------------------*/
/*
    Function: initialize_LS_Initial

    Copies the initial values of the knockout model to the LS_Initial model
    so that it can be compared to later to check for changes.
*/
function initialize_LS_Initial() {
    LS_Initial.MachineID = LS_koModel.MachineID();
    LS_Initial.MachineLocation = LS_koModel.MachineLocation();
    LS_Initial.OpeningHour = LS_koModel.OpeningHour();
    LS_Initial.ClosingHour = LS_koModel.ClosingHour();
    LS_Initial.CurrentSupply = LS_koModel.CurrentSupply();
    LS_Initial.TotalCapacity = LS_koModel.TotalCapacity();
    LS_Initial.OutOfOrder = LS_koModel.OutOfOrder();
    LS_Initial.Notes = LS_koModel.Notes();
}

/*
    Function: LS_clear_model

    Clears both the knockout model and the LS_Initial model
*/
function LS_clear_model() {
    LS_koModel.MachineKey(null),
    LS_koModel.MachineID(null),
    LS_koModel.MachineLocation(null),
    LS_koModel.OpeningHour(null),
    LS_koModel.ClosingHour(null),
    LS_koModel.CurrentSupply(null),
    LS_koModel.TotalCapacity(null),
    LS_koModel.TotalMugsDispensed(null),
    LS_koModel.OutOfOrder(false),
    LS_koModel.Notes(null),

    LS_Initial = {
        MachineID: null,
        MachineLocation: null,
        OpeningHour: null,
        ClosingHour: null,
        CurrentSupply: null,
        TotalCapacity: null,
        OutOfOrder: false,
        Notes: null
    };

    toastr.clear(); // Clear all open toastr alerts when opening a modal
}

/*
    Function: LS_stringCorrections

    Trims unnecessary spaces from beginning and end of first and last name entries
*/
function LS_stringCorrections() {
    if (LS_koModel.MachineLocation() != null) { LS_koModel.MachineLocation(LS_koModel.MachineLocation().trim()); }
}

/*
    Function: LS_createDefaultTextFields

    Sets the border color of the create modal text fields to the default color
*/
function LS_createDefaultTextFields() {
    document.getElementById("C_MachineID").className = "col-sm-6";
    document.getElementById("C_MachineLocation").className = "col-sm-6";
    document.getElementById("C_OpeningHour").className = "col-sm-6";
    document.getElementById("C_ClosingHour").className = "col-sm-6";
    document.getElementById("C_TotalCapacity").className = "col-sm-6";
}

/*
    Function: LS_editDefaultTextFields

    Sets the border color of the edit modal text fields to the default color
*/
function LS_editDefaultTextFields() {
    document.getElementById("E_MachineID").className = "col-sm-6";
    document.getElementById("E_MachineLocation").className = "col-sm-6";
    document.getElementById("E_OpeningHour").className = "col-sm-6";
    document.getElementById("E_ClosingHour").className = "col-sm-6";
    document.getElementById("E_TotalCapacity").className = "col-sm-6";
}

/*
    Function: LS_create_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before creating the new record with this data in the database
*/
function LS_create_field_check() {
    var validationSuccessful = true;

    // Machine ID validation step
    if (LS_emptyTextField(LS_koModel.MachineID()) || LS_koModel.MachineID().length != 6) {
        document.getElementById("C_MachineID").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Machine ID'.");
        validationSuccessful = false;
    }
    else if (!LS_MachineIDValidator()) {
        document.getElementById("C_MachineID").className = "col-sm-6 has-error";
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_MachineID").className = "col-sm-6 has-success";
    }

    // Machine Location validation step
    if (LS_emptyTextField(LS_koModel.MachineLocation())) {
        document.getElementById("C_MachineLocation").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Machine Location'.");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_MachineLocation").className = "col-sm-6 has-success";
    }

    // Open Hours validation step
    if (LS_koModel.OpeningHour() != null && LS_koModel.ClosingHour() != null) {
        document.getElementById("C_OpeningHour").className = "col-sm-6 has-success";
        document.getElementById("C_ClosingHour").className = "col-sm-6 has-success";
    }
    else {
        document.getElementById("C_OpeningHour").className = "col-sm-6 has-error";
        document.getElementById("C_ClosingHour").className = "col-sm-6 has-error";
        toastr.error("Opening and closing hours are required values, please ensure you have selected values for both fields.");
        validationSuccessful = false;
    }

    // Total Capacity validation step
    if (LS_emptyTextField(LS_koModel.TotalCapacity()) || LS_koModel.TotalCapacity() == "0") {
        document.getElementById("C_TotalCapacity").className = "col-sm-6 has-error";
        toastr.error("Invalid entry for 'Total Capacity'. (Note: Capacity cannot be '0')");
        validationSuccessful = false;
    }
    else {
        document.getElementById("C_TotalCapacity").className = "col-sm-6 has-success";
    }

    if (validationSuccessful) {
        LS_create_ajax();
        LS_Table_Data_Ajax();
    }
}

/*
    Function: LS_edit_field_check

    Validates each entry in their respective text box to ensure correct inputs
    before updating the record with this data in the database. Also checks to
    make sure that changes have actually occurred to save server side processing
    time
*/
function LS_edit_field_check() {
    var validationSuccessful = true;
    var changeDetected = false;

    // Machine ID change/validation step
    if (LS_koModel.MachineID() != LS_Initial.MachineID) {
        changeDetected = true;

        if (LS_emptyTextField(LS_koModel.MachineID()) || LS_koModel.MachineID().length != 6) {
            document.getElementById("E_MachineID").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Machine ID'.");
            validationSuccessful = false;
        }
        else if (!LS_MachineIDValidator()) {
            document.getElementById("E_MachineID").className = "col-sm-6 has-error";
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_MachineID").className = "col-sm-6 has-success";
        }
    }
    
    // Machine Location change/validation step
    if (LS_koModel.MachineLocation() != LS_Initial.MachineLocation) {
        changeDetected = true;

        if (LS_emptyTextField(LS_koModel.MachineLocation())) {
            document.getElementById("E_MachineLocation").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Machine Location'.");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_MachineLocation").className = "col-sm-6 has-success";
        }
    }

    // Open Hours validation step
    if (LS_koModel.OpeningHour() != LS_Initial.OpeningHour || LS_koModel.ClosingHour() != LS_Initial.ClosingHour) {
        changeDetected = true;

        if (LS_koModel.OpeningHour() != null && LS_koModel.ClosingHour() != null) {
            document.getElementById("C_OpeningHour").className = "col-sm-6 has-success";
            document.getElementById("C_ClosingHour").className = "col-sm-6 has-success";
        }
        else {
            document.getElementById("C_OpeningHour").className = "col-sm-6 has-error";
            document.getElementById("C_ClosingHour").className = "col-sm-6 has-error";
            toastr.error("Opening and closing hours are required values, please ensure you have selected values for both fields.");
            validationSuccessful = false;
        }
    }

    // Total Capacity change/validation step
    if (LS_koModel.TotalCapacity() != LS_Initial.TotalCapacity) {
        changeDetected = true;

        if (LS_emptyTextField(LS_koModel.TotalCapacity()) || LS_koModel.TotalCapacity() == "0") {
            document.getElementById("E_TotalCapacity").className = "col-sm-6 has-error";
            toastr.error("Invalid entry for 'Total Capacity'. (Note: total capacity cannot be '0')");
            validationSuccessful = false;
        }
        else {
            document.getElementById("E_TotalCapacity").className = "col-sm-6 has-success";
        }
    }

    // Out Of Order change/validation step
    if (LS_koModel.OutOfOrder() != LS_Initial.OutOfOrder) {
        changeDetected = true;
    }

    // Notes change check
    if (LS_koModel.Notes() != LS_Initial.Notes) {
        changeDetected = true;
    }

    if (changeDetected && validationSuccessful) {
        LS_edit_ajax();
        LS_Table_Data_Ajax();
    }
    else if (!changeDetected) {
        toastr.error("No changes detected.");
    }
}

/*
    Function: LS_emptyTextField

    Checks to see if knockout value is null or ""

    Parameters:

        fieldName - string from knockout model to be checked for null or "" value

    Returns:

        true - if the value in the text field is null or ""
        false - if the text field contains a value other than null or ""
*/
function LS_emptyTextField(fieldName) {
    if (fieldName == null || fieldName == "")
        return true;
    return false;
}


/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -LS_Table_Data_Ajax()
        -LS_GetRecord()
        -LS_create_ajax()
        -LS_edit_ajax()
        -LS_delete_ajax()
        -LS_MachineIDValidator()

----------------------------------------------------------------------------------------*/
/*
    Function: LS_Table_Data_Ajax

    Ajax call that gathers the data needed to fill the data table then calls functions
    to build the table or destroy and rebuild the table.
*/
function LS_Table_Data_Ajax() {
    $.ajax({
        url: LS_Table_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            if (LS_table !== null && typeof LS_table !== 'undefined') {
                LS_table.destroy();
                LS_table = null;
                $('#LS_table').replaceWith('<table id="LS_Table" class="table table-striped table-bordered" cellpadding="0" cellspacing="0" border="0"></table>');
            };
            LS_DataTable(data);
        }
    });
}

/*
    Function: LS_GetRecord

    Ajax call that retrieves all the data related to the record with specified
    MachineKey, then copies the data to the knockout model, as well as the
    LS_Initial model.

    Parameters:

        MachineKey - The primary key of a selected record.
*/
function LS_GetRecord(MachineKey) {
    var jsonString = { "MachineKey": MachineKey };

    $.ajax({
        url: LS_GetRecord_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            LS_koModel.MachineKey(data.MachineKey);
            LS_koModel.MachineID(data.MachineID);
            LS_koModel.MachineLocation(data.MachineLocation);
            LS_koModel.OpeningHour(LS_koModel.OpeningHourList._latestValue[data.OpeningHour]);
            LS_koModel.ClosingHour(LS_koModel.ClosingHourList._latestValue[data.ClosingHour]);
            LS_koModel.CurrentSupply(data.CurrentSupply);
            LS_koModel.TotalCapacity(data.TotalCapacity);
            LS_koModel.TotalMugsDispensed(data.TotalMugsDispensed);
            LS_koModel.OutOfOrder(data.OutOfOrder);
            LS_koModel.Notes(data.Notes);

            initialize_LS_Initial();
        }
    });
}

/*
    Function: LS_create_ajax

    Ajax call that passes all the knockout data to the server side to create a new
    user and save it to the server.
*/
function LS_create_ajax() {
    var jsonString = {
        "MachineID": LS_koModel.MachineID(),
        "MachineLocation": LS_koModel.MachineLocation(),
        "OpeningHour": LS_koModel.OpeningHour().hourValue,
        "ClosingHour": LS_koModel.ClosingHour().hourValue,
        "TotalCapacity": LS_koModel.TotalCapacity(),
        "OutOfOrder": LS_koModel.OutOfOrder(),
        "Notes": LS_koModel.Notes()
    };

    $.ajax({
        url: LS_CreateRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#LS_create').modal("hide");
                toastr.success("New machine profile was successfully created!");
            }
            else {
                toastr.error("An error occurred while creating the new machine profile. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: LS_edit_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    machine profile and save it to the server.
*/
function LS_edit_ajax() {
    var jsonString = {
        "MachineKey": LS_koModel.MachineKey(),
        "MachineID": LS_koModel.MachineID(),
        "MachineLocation": LS_koModel.MachineLocation(),
        "OpeningHour": LS_koModel.OpeningHour().hourValue,
        "ClosingHour": LS_koModel.ClosingHour().hourValue,
        "TotalCapacity": LS_koModel.TotalCapacity(),
        "OutOfOrder": LS_koModel.OutOfOrder(),
        "Notes": LS_koModel.Notes()
    };

    $.ajax({
        url: LS_EditRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#LS_edit').modal("hide");
                toastr.success("Machine profile was updated successfully!");
            }
            else {
                toastr.error("An error occurred while updating the machine profile. Please make sure all fields are filled out correctly.");
            }
        }
    });
}

/*
    Function: LS_delete_ajax

    Ajax call that passes all the knockout data to the server side to edit an existing
    user and save it to the server.
*/
function LS_delete_ajax() {
    var jsonString = { "MachineID": LS_koModel.MachineID() };

    $.ajax({
        url: LS_DeleteRecord_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            if (data.boolVal == true) {
                $('#LS_delete').modal("hide");
                toastr.success("Machine profile was deleted successfully!");
            }
            else {
                toastr.error("An error occurred while deleting the machine profile. Please try again.");
            }
        }
    });
}

/*
    Function: LS_MachineIDValidator

    Ajax call to validate that the machine ID is unique.
*/
function LS_MachineIDValidator() {
    var boolVal = false;
    var jsonString = { "MachineID": LS_koModel.MachineID() };

    $.ajax({
        url: LS_MachineID_URL,
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
                toastr.error("Machine ID already exists.");
            }
        }
    });

    return boolVal;
}

/*
    Function: LS_HourlyChartDataAjax

    Ajax call that gathers the data needed to fill the Hourly Statistics chart for given machine ID.
*/
function LS_HourlyChartDataAjax(MachineID, OpeningHour, ClosingHour) {
    var jsonString = {
        "MachineID": MachineID,
        "OpeningHour": OpeningHour,
        "ClosingHour": ClosingHour
    };

    $.ajax({
        url: LS_HourlyChartData_URL,
        data: jsonString,
        dataType: 'json',
        type: 'POST',
        cache: false,
        async: false,
        success: function (data) {
            var hourlyChart_data = [];
            var hourlyChart_x_gridValues = [];
            var hourlyChart_title = 'Hourly Mug Rental Statistics for Machine: ' + MachineID;

            for (var i = 0; i < data.length; i++) {
                hourlyChart_data.push(data[i].TotalMugsBorrowed);
                hourlyChart_x_gridValues.push(data[i].Hour);
            }

            DrawBarChart("LS_HourlyStatistics", hourlyChart_data, hourlyChart_title, hourlyChart_x_gridValues, hourlyChart_x_label, hourlyChart_y_label, 'rgba(0, 0, 255, 0.5)');
        },
        error: function () {
            toastr.error("Connection to database failed. Please check internet connection and try again.");
        }
    });
}