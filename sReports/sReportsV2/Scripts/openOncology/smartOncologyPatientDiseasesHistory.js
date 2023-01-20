function newEntity() {
    window.location.href = `/SmartOncology/ProgressNote`;
}

function editEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/ProgressNote?schemaInstanceId=${id}`;
}

function viewEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/ProgressNote?schemaInstanceId=${id}`;
}

function deleteEntity(event) {
    event.preventDefault();
    event.stopPropagation();

    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-id')

    $.ajax({
        type: "DELETE",
        url: `/SmartOncology/DeleteSchemaInstance/${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function mainFilter() {
    $('#indication').val($('#indicationTemp').val());
    $('#stage').val($('#stageTemp').val());
    $('#name').val($('#nameTemp').val());
    filterData();
}

function advanceFilter() {
    $('#indicationTemp').val($('#indication').val());
    $('#stageTemp').val($('#stage').val());
    $('#nameTemp').val($('#name').val());
    filterData();
}

function reloadTable() {
    $('#advancedFilterModal').modal('hide');
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setFilterTagsFromObj(requestObject);
    setAdvancedFilterBtnStyle(requestObject, ['Indication', 'Stage', 'Name', 'patientId', 'page', 'pageSize']);
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    $.ajax({
        type: 'GET',
        url: '/SmartOncology/ReloadSchemaInstances',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    var state = $("#stage").val();
    var clinicalConstelation = $("#clinicalConstelation").val();
    var name = $("#name").val();
    var patient = $("#patient").val();
    var createdBy = $("#createdBy").val();

    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'State', state);
        addPropertyToObject(result, 'ClinicalConstelation', clinicalConstelation);
        addPropertyToObject(result, 'Name', name);
        addPropertyToObject(result, 'patientId', patient);
        addPropertyToObject(result, 'CreatedBy', createdBy);
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    delete filterObject.patientId;

    return filterObject;
}