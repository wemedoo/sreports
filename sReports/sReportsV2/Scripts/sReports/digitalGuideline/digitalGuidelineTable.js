function reloadTable() {
    $('#advancedFilterModal').modal('hide');
    setFilterElements();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    requestObject.Page = getPageNum();
    requestObject.PageSize = getPageSize();

    $.ajax({
        type: 'GET',
        url: '/DigitalGuideline/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}



function redirectToCreate() {
    window.location.href = `/DigitalGuideline/Create`;
}


function editEntity(event, id) {
    window.location.href = `/DigitalGuideline/Edit?id=${id}`;
    event.preventDefault();
}

function removeEntry(event, id, lastUpdate) {
    event.stopPropagation();
    event.preventDefault();
    $.ajax({
        type: "DELETE",
        url: `/DigitalGuideline/Delete?id=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function advanceFilter() {
    $('#TitleTemp').val($('#title').val());

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');
    filterData();
    //clearFilters();
}

function mainFilter() {
    $('#title').val($('#TitleTemp').val());

    $('#advancedId').children('div:first').removeClass('btn-advanced');
    $('#advancedId').find('button:first').addClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'none');

    filterData();
    //clearFilters();
}

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Title', $('#title').val());
        addPropertyToObject(requestObject, 'Major', $('#major').val());
        addPropertyToObject(requestObject, 'Minor', $('#minor').val());
        addPropertyToObject(requestObject, 'DateTimeTo', $('#dateTimeTo').val());
        addPropertyToObject(requestObject, 'DateTimeFrom', $('#dateTimeFrom').val());

    }

    return requestObject;
}