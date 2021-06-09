function reloadTable() {
  
    let requestObject = {};
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    $.ajax({
        type: 'GET',
        url: '/UserAdministration/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

function createUserEntry() {
    window.location.href = "/UserAdministration/Create";
}

function editEntity(event, id) {
    window.location.href = `/UserAdministration/Edit?userId=${id}`;
    event.preventDefault();
}

function removeUserEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/UserAdministration/Delete?userId=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function setUserState(event, id, state, lastUpdate) {
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
            toastr.error(`${thrownError} `);
        }
    });
}