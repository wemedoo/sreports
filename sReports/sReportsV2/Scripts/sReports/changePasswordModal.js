
function showChangePasswordModal(e, id) {
    e.stopPropagation();
    e.preventDefault();

    $('#buttonSubmitPassword').attr('data-userid', id);
    $('#passwordModal').modal('show');
}

function generateNewPassword() {
    var request = {};
    request['userId'] = $('#buttonSubmitPassword').data('userid');
    request['oldPassword'] = $("#oldPassword").val();
    request['newPassword'] = $("#newPassword").val();
    request['confirmPassword'] = $("#confirmPassword").val();
    $("#oldPasswordId").html("");
    $("#confirmPasswordId").html("");

    $.ajax({
        type: "GET",
        url: "/UserAdministration/ChangePassword",
        data: request,
        success: function (data) {
            toastr.options = {
                timeOut: 3000
            }
            $('#passwordModal').modal('hide');
            toastr.options.onHidden = function () { window.location.href = `/User/Logout`; }
            toastr.success(`Password successfully changed. Please log in again with new password.`);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            if (thrownError == "Bad Request") 
                $("#oldPasswordId").html("The old password you have entered is incorrect!");
            else if (thrownError == "Forbidden")
                $("#confirmPasswordId").html("Confirm password and new password doesn't match!");
            else
                toastr.error(`${thrownError} `);
        }
    });

    return false;
}