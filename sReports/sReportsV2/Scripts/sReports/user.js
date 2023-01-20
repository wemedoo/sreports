$(document).on('focus', '.user-input, .password-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this)
        .removeClass("error")
        .siblings(".login-input-icon").css("border-right", "solid 1px #4dbbc8");
    $(this).siblings(".login-error").remove();
})

$(document).on('blur', '.user-input, .password-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(e.target).siblings(".login-input-icon").css("border-right", "solid 1px #e5e5e5");
})

function forgotPassword() {
    window.location.href = '/User/ForgotPassword';
}

function generatePassword(e, email, goToLogin) {
    e.preventDefault();
    e.stopPropagation();
    var valid = validateEmail(email);
    if (valid) {
        $.ajax({
            type: "POST",
            url: `/User/GeneratePassword?Email=${email}`,
            success: function () {
                toastr.options = {
                    timeOut: 500
                }
                if (goToLogin) {
                    toastr.options.onHidden = function () { window.location.href = `/User/Login?ReturnUrl=%2f`; };
                }
                toastr.success("Successfully changed password");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`Email address not found!`);
            }
        });
    }
    else {
        showGeneratePasswordErrors();
    }
}

function showGeneratePasswordErrors() {
    $(".login-error").html("Please enter valid email address");
    $(".login-input").addClass("error");
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

$(document).on('focus', '.forgot-password-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this).removeClass("error");
    $(this).siblings(".login-error").html("");
})

$(document).ready(function () {
    $("#timeZone").val(Intl.DateTimeFormat().resolvedOptions().timeZone);
});

function signInMicrosoft() {
    location.href = "/User/SignIn"
}