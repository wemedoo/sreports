function submitForm(form,e) {
 
    $(form).validate({
        ignore: []
    }); 

    if ($(form).valid()) {

        var request = {};

        request['Id'] = $("#userId").val();
        request['LastUpdate'] = $("#lastUpdate").val();
        request['Username'] = $("#username").val();
        request['FirstName'] = $("#firstName").val();
        request['LastName'] = $("#lastName").val();
        var email = validateEmail($("#email").val());
        if (email == true)
            request['Email'] = $("#email").val();
        else
            $(error).closest('.email').collapse("show");

        request['ContactPhone'] = $("#contactPhone").val();
        request['Organizations'] = getSelectedOrganizations();
        request['Roles'] = getSelectedRoles();

        $.ajax({
            type: "POST",
            url: "/User/Create",
            data: request,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                toastr.options.onHidden = function () { window.location.href = `/User/GetAll`; }
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });

    }
    var errors = $('.error').get();
    if (errors.length !== 0) {
        $.each(errors, function (index, error) {
            $(error).closest('.collapse').collapse("show");
        });
    };

    return false;
}

function cancelUserEdit() {
    window.location.href = '/User/GetAll';
}

function getSelectedOrganizations() {
    var chkArray = [];

    $(".chk:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function getSelectedRoles() {
    var chkArray = [];

    $(".chk2:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}