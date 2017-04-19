// Initializes a model of knockout observables used in the html
var emailService_koModel = {
    Admin: ko.observable(true),

    EmailContactListGroups: ko.observableArray(['Admin', 'Staff', 'Mug-Share Users']),

    EmailContactListGroup: ko.observable(null),

    EmailContactList: ko.observable(null),

    EmailContactListGroupChanged: function (event) {
        displayOptions();
        getContactList();
    },

    MugShareUsersType: ko.observableArray([])
}

$(document).ready(function () {
    //return_koModel.Admin(viewModel.Admin());

    ko.applyBindings(emailService_koModel, document.getElementById("EmailService"));

    //$(".Admin").show();
});


function MugShareUsersOptionsChanged() {
    getContactList();
}

function displayOptions() {
    if (emailService_koModel.EmailContactListGroup() == "Mug-Share Users") {
        $('.MugShareUsersOptions').show();
    } else {
        $('.MugShareUsersOptions').hide();
    }
}

/*--------------------------------------------------------------------------------------
    CONTACT LIST FUNCTIONS:

        -getContactList()

----------------------------------------------------------------------------------------*/
/*
    Function: getContactList
*/
function getContactList() {
    emailService_koModel.EmailContactList(null);
    getContactListAjax();
}

/*--------------------------------------------------------------------------------------
    AJAX FUNCTIONS:

        -getContactListAjax()

----------------------------------------------------------------------------------------*/
/*
    Function: getContactListAjax

    Ajax call for getting the contact list
*/
function getContactListAjax() {
    var jsonString = {
        "ContactListGroup": emailService_koModel.EmailContactListGroup, "MugShareUsersType": emailService_koModel.MugShareUsersType
    };
    $.ajax({
        url: GetContactList_URL,
        data: jsonString,
        cache: false,
        type: "POST",
        async: false,
        dataType: "json",
        success: function (data) {
            emailService_koModel.EmailContactList(data);
            if (data == null) {
                toastr.error("An error occurred while getting the contact list.");
                return;
            }
        }
    });
}


