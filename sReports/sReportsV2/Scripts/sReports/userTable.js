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
        url: '/User/ReloadTable',
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
    window.location.href = "/User/Create";
}

function editEntity(event, id) {
    window.location.href = `/User/Edit?userId=${id}`;
    event.preventDefault();
}

function removeUserEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/User/Delete?userId=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}