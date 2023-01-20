function newEntity() {
    window.location.href = `/SmartOncology/CreateNewSchema`;
}

function editEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/EditSchema/${id}`;
}

function viewEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/PreviewSchema/${id}`;
}

function deleteEntity(event) {
    event.preventDefault();
    event.stopPropagation();

    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-id')

    $.ajax({
        type: "DELETE",
        url: `/SmartOncology/DeleteSchema/${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
            $('#deleteModal').modal('hide');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function mainFilter() {
    $('#indication').val($('#indicationTemp').val());
    $('#stage').val($('#stageTemp').val());
    $('#clinicalConstelation').val($('#clinicalConstelationTemp').val());
    filterData();
}

function advanceFilter() {
    $('#indicationTemp').val($('#indication').val());
    $('#stageTemp').val($('#stage').val());
    $('#clinicalConstelationTemp').val($('#clinicalConstelation').val());
    filterData();
}

function reloadTable() {
    $('#advancedFilterModal').modal('hide');
    setFilterTagsFromUrl();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setAdvancedFilterBtnStyle(requestObject, ['Indication', 'Stage', 'ClinicalConstelation', 'page', 'pageSize']);
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
        url: '/SmartOncology/ReloadSchemas',
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
    let result = {};
    var indication = $("#indication").val();
    var stage = $("#stage").val();
    var clinicalConstelation = $("#clinicalConstelation").val();
    var name = $("#name").val();

    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'Indication', indication);
        addPropertyToObject(result, 'Stage', stage);
        addPropertyToObject(result, 'ClinicalConstelation', clinicalConstelation);
        addPropertyToObject(result, 'Name', name);
    }

    return result;
}