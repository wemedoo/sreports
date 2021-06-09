$(document).on('focus', '.user-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    var user = document.getElementById("userIcon");
    user.style.borderRight = "solid 1px #4dbbc8";
})

$(document).on('blur', '.user-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    var user = document.getElementById("userIcon");
    user.style.borderRight = "solid 1px #e5e5e5";
})

$(document).on('focus', '.password-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    var password = document.getElementById("passwordIcon");
    password.style.borderRight = "solid 1px #4dbbc8";
})

$(document).on('blur', '.password-input', function (e) {
    e.stopPropagation();
    e.preventDefault();
    var password = document.getElementById("passwordIcon");
    password.style.borderRight = "solid 1px #e5e5e5";
})

function forgotPassword() {
    window.location.href = '/User/ForgotPassword';
}

function generatePassword(e, email, goToLogin) {
    e.preventDefault();
    e.stopPropagation();
    var userEmail = validateEmail(email);
    if (userEmail == true) {
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
    else
        $("#userEmail").parent().after("<div class='validation' style='color:red;margin-bottom: 20px;'>Please enter valid email address</div>");
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

$(document).ready(function () {
    /*let timeZone = -(new Date().getTimezoneOffset() / 60);
    console.log(timeZone);*/

    $("#timeZone").val(Intl.DateTimeFormat().resolvedOptions().timeZone);
});

function signInMicrosoft() {
    location.href = "/User/SignIn"
}