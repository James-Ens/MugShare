// Initializes a model of knockout observables used in the html
var changePassword_koModel = {
    Admin: ko.observable(true),

    CurrentPassword: ko.observable(null),

    NewPassword: ko.observable(null),

    RetypeNewPassword: ko.observable(null)
}

$(document).ready(function () {
    //return_koModel.Admin(viewModel.Admin());

    ko.applyBindings(changePassword_koModel, document.getElementById("ChangePassword"));

    //$(".Admin").show();
});

/*--------------------------------------------------------------------------------------
    CHANGE PASSWORD FUNCTIONS:

        -changePassword_submit()

----------------------------------------------------------------------------------------*/

function changePassword_submit() {
    changePasswordAjax();
}

/*--------------------------------------------------------------------------------------
    CHECK/VALIDATION FUNCTIONS:

        -changePassword_clear()

----------------------------------------------------------------------------------------*/
/*
    Function: changePassword_clear

    Clears CurrentPassword, NewPassword and RetypeNewPassword in knockout model
*/
function changePassword_clear() {
    changePassword_koModel.CurrentPassword(null);
    changePassword_koModel.NewPassword(null);
    changePassword_koModel.RetypeNewPassword(null);
}

/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -changePasswordAjax()

----------------------------------------------------------------------------------------*/
/*
    Function: changePassword

    Ajax call for processing the password change.
    Clear CurrentPassword, NewPassword and RetypeNewPassword in knockout model so that the fields are empty after the process
*/
function changePasswordAjax() {
    var jsonString = {
        "CurrentPassword": changePassword_koModel.CurrentPassword(), "NewPassword": changePassword_koModel.NewPassword(),
        "RetypeNewPassword" : changePassword_koModel.RetypeNewPassword()};
    $.ajax({
        url: ProcessChangePassword_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            changePassword_clear();
            if (data.boolVal == true) {
                toastr.success("Password has been changed successfully!");
                window.location.href = "/Home";
            }
            else {
                toastr.error("An error occurred while changing password. Please make sure all fields are filled out correctly!");
                return;
            }
        }
    });
}