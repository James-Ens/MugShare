// Initializes a model of knockout observables used in the html
var return_koModel = {
    Admin: ko.observable(true),

    MugID: ko.observable(null),

    MugCountYearly: ko.observable(null)
}

// Monthly Statistics chart information
var monthlyChart_title = 'Monthly Mug Rental Statistics'
var monthlyChart_x_gridValues = ["January", "February", "March", "April", "May", "June",
                    "July", "August", "September", "October", "November", "December"]
var monthlyChart_x_label = 'Month'
var monthlyChart_y_label = 'Number of Mugs Rented'
var monthlyChart_colors = [
                    'rgba(29, 96, 189, 0.2)',
                    'rgba(255, 0, 204, 0.2)',
                    'rgba(227, 11, 92, 0.2)',
                    'rgba(255, 153, 102, 0.2)',
                    'rgba(102, 255, 102, 0.2)',
                    'rgba(255, 255, 102, 0.2)',
                    'rgba(149, 224, 232, 0.2)',
                    'rgba(255, 153, 51, 0.2)',
                    'rgba(51, 204, 153, 0.2)',
                    'rgba(253, 14, 53, 0.2)',
                    'rgba(111, 45, 168, 0.2)',
                    'rgba(120, 129, 147, 0.2)'
]

// Yearly Statistics chart information
var yearlyChart_title = 'Yearly Mug Rental Statistics'
var yearlyChart_y_label = 'Number of Mugs Rented'

$(document).ready(function () {
    if (!return_koModel.Admin()) {
        window.location.href = "/Home/LogIn";
    }

    ko.applyBindings(return_koModel, document.getElementById("MugReturn"));
     

    getMugCountYearly();
   // $(".Admin").show();
});

/*--------------------------------------------------------------------------------------
    RETURN MODAL FUNCTIONS:

        -returnModal()
        -returnModal_submit()

----------------------------------------------------------------------------------------*/
/*
    Function: returnModal

    Opens the create user modal, and clears the knockout model so that
    all the fields are empty
*/
function returnModal() {
    returnModal_clear();
    return_koModel.MugID(0);
    /*
    $('#MugReturnModal').modal({
        show: true,
        backdrop: false,
        keyboard: false
    });*/
}

function returnModal_submit() {
    returnAjax();
}

/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -returnModal_clear()

----------------------------------------------------------------------------------------*/
/*
    Function: returnModal_clear

    Clears the knockout model
*/
function returnModal_clear() {
    return_koModel.MugID(null)
}

/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -returnAjax()
        -MonthlyChartDataAjax()
        -YearlyChartDataAjax()

----------------------------------------------------------------------------------------*/
/*
    Function: returnAjax

    Ajax call for processing the return of a mug
*/
function returnAjax() {
    var jsonString = { "MugID": return_koModel.MugID() };
    $.ajax({
        url: ProcessReturn_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.boolVal == true) {
                toastr.success("Mug was returned successfully!");
                return_koModel.MugID(null);
            }
            else {
                toastr.error("An error occurred while returning the mug. Please make sure all fields are filled out correctly!");
            }
        }
    });
}

/*
    Function: MonthlyChartDataAjax

    Ajax call that gathers the data needed to fill the Monthly Statistics chart.
*/
function MonthlyChartDataAjax() {
    $.ajax({
        url: MonthlyChart_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            var monthlyChart_data = [data.January, data.February, data.March, data.April, data.May, data.June,
                                    data.July, data.August, data.September, data.October, data.November, data.December];

            DrawBarChart("MonthlyStatistics", monthlyChart_data, monthlyChart_title, monthlyChart_x_gridValues, monthlyChart_x_label, monthlyChart_y_label, monthlyChart_colors);
        },
        error: function () {
            toastr.error("Connection to database failed. Please check internet connection and try again.");
        }
    });
}

/*
    Function: YearlyChartDataAjax

    Ajax call that gathers the data needed to fill the Yearly Statistics chart.
*/
function YearlyChartDataAjax() {
    $.ajax({
        url: YearlyChart_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            var yearlyChart_data = [];
            var yearlyChart_x_label = [];

            for (var i = 0; i < data.length; i++) {
                yearlyChart_data.push(data[i].TotalMugsBorrowed);
                yearlyChart_x_label.push(data[i].Year);
            }

            DrawLineChart("YearlyStatistics", yearlyChart_data, yearlyChart_title, yearlyChart_x_label, yearlyChart_y_label);
        },
        error: function () {
            toastr.error("Connection to database failed. Please check internet connection and try again.");
        }
    });
}

/*--------------------------------------------------------------------------------------
        MUG COUNTER FUNCTIONS:

        -getMugCountYearly()

----------------------------------------------------------------------------------------*/
/*
    Function: getMugCountYearly

    Get the number of mugs borrowed for the current year
*/
function getMugCountYearly() {
    var date = new Date();
    var year = date.getFullYear();

    $.ajax({
        url: YearlyChart_Data_URL,
        cache: false,
        async: false,
        dataType: "json",
        success: function (data) {
            for(var i = 0; i < data.length; i++) {
                if(data[i].Year == year)
                {
                    $("#MugCountYearly").text(data[i].TotalMugsBorrowed);
                    //return_koModel.MugCountYearly(data[i].TotalMugsBorrowed);
                    break;
                }

                // If unable to obtain the total mugs borrowed for some reason, output 0
                $("#MugCountYearly").text(0);
                //return_koModel.MugCountYearly(0);
            }
        },
        error: function () {
            toastr.error("Connection to database failed. Please check internet connection and try again.");
        }
    });
}

/*--------------------------------------------------------------------------------------
        RFID FUNCTIONS:

        -getRFID()

----------------------------------------------------------------------------------------*/
/*
    Function: getRFID

    Get the RFID of the mug to be returned
*/
function getRFID() {
    $.ajax({
        url: GetRFID_URL,
        data: "",
        cache: false,
        type: "GET",
        async: false,
        dataType: "text",
        success: function (data) {
            if (data != "") {
                return_koModel.MugID(data);
            }
            else {
                toastr.error("An error occurred while scanning the RFID.");
                return;
            }
        }
    });
}

function SignUp(){
 window.location.href = SignUp_URL;
}