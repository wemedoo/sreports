function loginUser(form, e) {
    e.preventDefault();
    e.stopPropagation();
    $(form).validate();
    if ($(form).valid()) {
       //
        let user = {};
        user["Username"] = $("#username").val();
        user["Password"] = $("#loginPartial").find("#password").val();
        $.ajax({
            type: "POST",
            data: user,
            url: `/ThesaurusGlobal/Login`,
            success: function (data, textStatus, jqXHR) {
                window.location.href = "/ThesaurusGlobal/Index"
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });
    }
}

function goToSignIn() {
    $("#loginPartial").show();
    $("#registrationPartial").hide();

}
function goToRegister() {
    $("#loginPartial").hide();
    $("#registrationPartial").show();
}

function registerUser(form, e) {
    e.preventDefault();
    e.stopPropagation();
    $(form).validate();
    if ($(form).valid()){
        if ($("#password").val() == $("#confirmPassword").val()) {
            submitUser();
        } else {
            toastr.error("Please confirm your password!");
        }
        
    }
}

function submitUser() {
    let user = getUser();
    $.ajax({
        type: "POST",
        url: "/ThesaurusGlobal/RegisterUser",
        data: user,
        success: function (data, textStatus, jqXHR) {
            goToLogin();
            toastr.success("Success");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });

}

function getUser() {
    let user = {};

    user["FirstName"] = $("#firstName").val();
    user["LastName"] = $("#secondName").val();
    user["Email"] = $("#email").val();
    user["Affiliation"] = $("#affiliation").val();
    user["Country"] = $("#country").val();
    user["Phone"] = $("#phone").val();
    user["Password"] = $("#password").val();

    return user;
}