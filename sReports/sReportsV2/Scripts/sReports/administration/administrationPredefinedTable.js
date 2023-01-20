var thesaurusId = "";

function reloadTable() {
    checkUrlPageParams();
    let requestObject = {};
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    requestObject.PredefinedType = $("#predefinedType").val();
    requestObject.PreferredTerm = $("#preferredTermSearch").val();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    $.ajax({
        type: 'GET',
        url: `/Administration/ReloadTable`,
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

function filterTableData() {
    let urlPageParams = `?page=1&pageSize=${getPageSize()}`;
    let filter = getFilterParametersObject();
    let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

    history.pushState({}, '', fullUrlParams);
    currentPage = 1;
    reloadTable();
}

function getFilterParametersObject() {
    var predefinedType = $("#predefinedType").val();

    let requestObject = {};
    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'PredefinedType', predefinedType);
    }

    return requestObject;
}

function removePredefinedType(e) {
    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-id')   
    $.ajax({
        type: "DELETE",
        url: `/Administration/Delete?id=${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $('#deleteModal').modal('hide');
            $(`#row-${id}`).remove();
            e.stopPropagation();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
    reloadTable();
}



function hideDeleteModal(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#deleteModal').modal('hide');
}

$(document).on('click', '.remove-predefined', function (e) {
    $(this).closest(".tr").remove();
});

$("#preferredTermSearch").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#filterTablePredefined").click();
    }
});