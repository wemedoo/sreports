function reloadTable() {
    let requestObject = {};
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    $.ajax({
        type: 'GET',
        url: '/RoleAdministration/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
            addSortArrows();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function createEntity() {
    window.location.href = "/RoleAdministration/Create";
}

function editEntity(event, id) {
    window.location.href = `/RoleAdministration/Edit?roleId=${id}`;
    event.preventDefault();
}

$(document).on('change', '.check-permissions-per-module', function () {
    checkUncheckPermissionsPerModule($(this));
});

function checkUncheckPermissionsPerModule($checkbox) {
    let moduleId = $checkbox.val();
    let shouldCheckPermission = false;
    if ($checkbox.is(":checked")) {
        shouldCheckPermission = true;
    }
    $(`.module-${moduleId}-permission`).prop("checked", shouldCheckPermission);
}

function submitRoleForm(form, event) {
    event.preventDefault();
    $('#roleForm').validate();
    if ($(form).valid()) {
        $.ajax({
            type: 'POST',
            url: '/RoleAdministration/Edit',
            data: getRole(),
            success: function (data) {
                toastr.success('Success');
                if (isNewRoleCreated()) {
                    editEntity(event, data.Id);
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function getRole() {
    let role = {};

    role['Id'] = $('#roleId').val();
    role['Name'] = $('#roleName').val();
    role['Description'] = $('#roleDescription').val();
    role['Permissions'] = getSelectedPermissions();

    return role;
}

function getSelectedPermissions() {
    let permissions = [];
    $('.module-permission').each(function (index, element) {
        let moduleId = $(element).data('id');
        $(`.module-${moduleId}-permission:checked`).each(function (index, element) {
            let permissionId = $(element).val();
            permissions.push({
                ModuleId: moduleId,
                PermissionId: permissionId
            });
        });
        
    });

    return permissions;
}

function cancelEditRole() {
    window.location.href = '/RoleAdministration/GetAll';
}

function isNewRoleCreated() {
    return !$('#roleId').val();
}