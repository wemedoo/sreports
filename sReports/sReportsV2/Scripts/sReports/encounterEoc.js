function search() {
    history.pushState({}, '', `?page=${currentPage}&pageSize=${getPageSize()}&identifierType=${$('#identifierType').val()}&identifierValue=${$('#identifierValue').val()}`);
    reloadTable();
}

function reloadTable() {
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.IdentifierType = $('#identifierType').val();
    requestObject.IdentifierValue = $('#identifierValue').val();
    requestObject.PageSize = getPageSize();

    if ($('#identifierType').val() && $('#identifierValue').val()) {
        $.ajax({
            type: 'GET',
            url: '/Encounter/ReloadEocTable',
            data: requestObject,
            success: function (data) {
                $("#tableContainer").html(data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
            }
        });
    }
}

function getAllEncounter(id) {
    window.location.href = `/Encounter/GetAllEncounter?eocId=${id}`;
}

function getFilterParametersObject() {
    let result = {};

    if (defaultFilter) {
        result = defaultFilter;
        defaultFilter = null;

    } else {
        if ($('#identifierType').val()) {
            result['IdentifierType'] = $('#identifierType').val().trim();
        }
        if ($('#identifierValue').val()) {
            result['IdentifierValue'] = $('#identifierValue').val().trim();
        }
    }
    
    
    return result;
}

