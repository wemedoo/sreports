function reloadTable() {
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    

    $.ajax({
        type: 'GET',
        url: '/EpisodeOfCare/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
            $('#createEoc').show();
            $('#tableContainer').show();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
            $('#createEoc').hide();
            $('#tableContainer').hide();
        }
    });
}

function editEntity(event,id) {
    //history.pushState(null, '', `?page=${currentPage}&pageSize=${getPageSize()}`);
    window.location.href = `/EpisodeOfCare/Edit?EpisodeOfCareId=${id}`;
    event.preventDefault();
}

function removeEOCEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/EpisodeOfCare/DeleteEOC?eocId=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    } else {
        if ($('#identifierType').val() && $('#identifierType').val().trim()) {
            result['IdentifierValue'] = $('#identifierValue').val().trim();
            result['IdentifierType'] = $('#identifierType').val();
        }
        if ($('#description').val()) {
            result['Description'] = $('#description').val().trim();
        }
        if ($('#type').val()) {
            result['Type'] = $('#type').val().trim();
        }
        if ($('#periodStartDate').val()) {
            result['PeriodStartDate'] = new Date($('#periodStartDate').val().trim()).toUTCString();
        }
        if ($('#periodEndDate').val()) {
            result['PeriodEndDate'] = new Date($('#periodEndDate').val().trim()).toUTCString();
        }
        if ($('#status').val()) {
            result['Status'] = $('#status').val().trim();
        }
    }

    return result;
}




