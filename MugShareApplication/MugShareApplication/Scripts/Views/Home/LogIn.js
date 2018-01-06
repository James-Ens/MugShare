// Initializes a model of knockout observables used in the html
/*
var logIn_koModel = {
    Admin: ko.observable(true),

    Username: ko.observable(null),

    Password: ko.observable(null),

    ForgotPassword_Email: ko.observable(null)
}

$(document).ready(function () {
    ko.applyBindings(logIn_koModel, document.getElementById("LogIn"));
});
*/

/*--------------------------------------------------------------------------------------
    LOG IN FUNCTIONS:

        -logIn_submit()
        -logIn_enter_event()

----------------------------------------------------------------------------------------*/
// triggers a log in attempt
function logIn_submit() {
    logInAjax();
}

// if the enter button is pressed in the password field, run logIn_submit()
function logIn_enter_event(e) {
    var key;

    if (window.event) {
        key = e.keyCode;
    }
    else if (e.which) {
        key = e.which;
    }

    if (key == 13) {
        logIn_submit();
    }
}

/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -logIn_clear()

----------------------------------------------------------------------------------------*/
/*
    Function: logIn_clear

    Clears Password in knockout model
*/
function logIn_clear() {
    $("#Password").val("");
    //logIn_koModel.Password(null);
}

/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -logInAjax()

----------------------------------------------------------------------------------------*/
/*
    Function: logInAjax

    Ajax call for processing the user log in.
    If log in is successful, redirects user to /Home.
    If log in is unsuccessful, clear Password in knockout model so that the Password field is empty
*/
function logInAjax() {
    var u = $("#Username").val();

    var p = $("#Password").val();
    var jsonString = { "LogInUsername": u, "LogInPassword": p };
    $.ajax({
        url: ProcessLogIn_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.boolVal == true) {
                toastr.success("Logged in successfully!");
                window.location.href = "/Home";
            }
            else {
                toastr.error("An error occurred while logging in. Please make sure all fields are filled out correctly!");
                logIn_clear();
                return;
            }
        }
    });
}

/*--------------------------------------------------------------------------------------
    FORGOT PASSWORD FUNCTIONS:

        -forgotPassword_submit()

----------------------------------------------------------------------------------------*/

function forgotPassword_submit() {
    forgotPasswordAjax();
}

/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -forgotPassword_clear()

----------------------------------------------------------------------------------------*/
/*
    Function: forgotPassword_clear

    Clears ForgotPassword_Email in knockout model
*/
function forgotPassword_clear() {
    logIn_koModel.ForgotPassword_Email(null);
}

/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -forgotPasswordAjax()

----------------------------------------------------------------------------------------*/
/*
    Function: forgotPasswordAjax

    Ajax call for processing forgot password.
    Clear ForgotPassword_Username and ForgotPassword_Email in knockout model so that the fields are empty are the process
*/
function forgotPasswordAjax() {
    
    var jsonString = {"ForgotPassword_Email" : $("#Email").val()};
    $.ajax({
        url: ProcessForgotPassword_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            if (data.boolVal == true) {
                toastr.success("An email with a temporary password will be sent to your email!");
                window.location.href = "/Home";
            }
            else {
                toastr.error("An error occurred while processing the request. Please make sure all fields are filled out correctly!");
                forgotPassword_clear();
                return;
            }
        }
    });
}

function SignUp(url) {
    window.location.href = url;
}