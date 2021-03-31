function createOrganizationEntry(){
    window.location.href = "/Organization/Create";
}

function editEntity(event,id) {
    window.location.href = `/Organization/Edit?organizationId=${id}`;
    event.preventDefault();
}

function removeOrganizationEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/Organization/Delete?organizationId=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();

            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

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
        url: '/Organization/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

//function getFilterParametersObject() {
//    let result = {};
//    if (defaultFilter) {
//        result = getDefaultFilter();
//        defaultFilter = null;
//    }
//    else {
//        result['Page'] = currentPage;
//        result['PageSize'] = getPageSize();
        
//    }

//    return result;
//}