function reloadTable() {
  
    let requestObject = getFilterParametersObject();
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
        url: '/UserAdministration/ReloadTable',
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

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'ShowUnassignedUsers', $('#showUnassignedUsers').is(":checked"));
    }
  
    return requestObject;
}

function createUserEntry() {
    window.location.href = "/UserAdministration/Create";
}

function editEntity(event, id) {
    window.location.href = `/UserAdministration/Edit?userId=${id}`;
    event.preventDefault();
}

function removeUserEntry(event, id) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/UserAdministration/Delete?userId=${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function setUserState(event, id, state) {
    event.stopPropagation();
    event.preventDefault();

    $.ajax({
        type: "PUT",
        url: `/UserAdministration/SetUserState?userId=${id}&newState=${state}`,
        success: function (data) {
            toastr.success(`Success`);
            reloadTable();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

$(document).on("change", "#showUnassignedUsers", function () {
    filterData();
});