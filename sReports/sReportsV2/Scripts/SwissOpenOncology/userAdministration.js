function setUserState(event, id, state) {
    event.stopPropagation();
    event.preventDefault();

    $.ajax({
        type: "PUT",
        url: `/ThesaurusGlobal/SetUserStatus?userId=${id}&newStatus=${state}`,
        success: function (data) {
            toastr.success(`Success`);
            reloadTable();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function reloadTable() {
    window.location.href = '/ThesaurusGlobal/Users';
}

function setUserRoles(event, id) {
    event.stopPropagation();
    event.preventDefault();

    $('.dropdown-menu').dropdown()
}

function updateRoles(event, form) {
    event.stopPropagation();
    event.preventDefault();

    $.ajax({
        type: "PUT",
        url: '/ThesaurusGlobal/UpdateUserRoles',
        data: getRoleObject(form),
        success: function (data) {
            $(form).closest('.dropdown-menu').toggleClass("show");
            toastr.options = {
                timeOut: 2000
            }
            toastr.options.onHidden = function () {
                reloadTable();
            }
            toastr.success('Roles are successfully updated');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getRoleObject(form) {
    var object = {};

    var roles = [];
    $(form).find(".user-role:checked").each(function () {
        roles.push($(this).val());
    });

    object['Roles'] = roles;
    object['Id'] = $(form).data('id');

    return object;
}