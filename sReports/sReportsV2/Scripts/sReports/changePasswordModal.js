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
    $("#oldPassword").removeClass('error');
    $("#newPassword").removeClass('error');
    $("#confirmPassword").removeClass('error');

    $.ajax({
        type: "GET",
        url: "/UserAdministration/ChangePassword",
        data: request,
        success: function (data) {
            $('#passwordModal').modal('hide');
            $("body").css("pointer-events", "none");
            toastr.options = {
                timeOut: 3000
            }
            toastr.options.onHidden = function () { window.location.href = `/User/Logout`; }
            toastr.success(`Password successfully changed. Please log in again with new password.`);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            let formattedError = formatErrorMessages(thrownError);
            if (xhr.status == 400) {
                $("#oldPasswordId").html(formattedError);
                $("#oldPassword").addClass('error');
            } else if (xhr.status == 409) {
                $("#confirmPasswordId").html(formattedError);
                $("#confirmPassword").addClass('error');
                $("#newPassword").addClass('error');
            } else {
                handleResponseError(xhr, thrownError);
                $("#oldPassword").addClass('error');
                $("#newPassword").addClass('error');
                $("#confirmPassword").addClass('error');
            }
        }
    });

    return false;
}

function formatErrorMessages(thrownError) {
    let ul = $("<div></div>");
    for (let errorMessage of thrownError.split('|')) {
        if (errorMessage) {
            let li = $("<div></div>").text(errorMessage).css({ fontSize: 12, marginLeft: 5 });
            ul.append(li);
        }
    }
    return ul;
}