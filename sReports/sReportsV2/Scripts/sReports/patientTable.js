function editEntity(event,id) {
    window.location.href = `/Patient/Edit?patientId=${id}`;
    event.preventDefault();
}
function createPatientEntry() {
    window.location.href = "/Patient/Create";
}

function showEpisodeOfCares(id, event) {
    event.preventDefault();
    window.location.href = `/EpisodeOfCare/GetAll?IdentifierType=O4MtPatientId&IdentifierValue=${id}`;
}
function removePatientEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/Patient/Delete?PatientId=${id}&&LastUpdate=${lastUpdate}`,
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
function reloadTable() {
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    requestObject.Page = getPageNum();
    requestObject.PageSize = getPageSize();

    $.ajax({
        type: 'GET',
        url: '/Patient/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        if ($('#identifierType').val() && $('#identifierType').val().trim()) {
            result['IdentifierValue'] = $('#identifierValue').val().trim();
            result['IdentifierType'] = $('#identifierType').val();
        }
        if ($('#country').val()) {
            result['Country'] = $('#country').val().trim();
        }
        if ($('#city').val()) {
            result['City'] = $('#city').val().trim();
        }
        if ($('#birthDate').val()) {
            console.log(new Date($('#birthDate').val().trim()).toUTCString());
            result['BirthDate'] = new Date($('#birthDate').val().trim()).toUTCString();
        }
        if ($('#given').val()) {
            result['Given'] = $('#given').val().trim();
        }
        if ($('#family').val()) {
            result['Family'] = $('#family').val().trim();
        }
        if ($('#postalCode').val()) {
            result['PostalCode'] = $('#postalCode').val().trim();
        }

    }
    
    return result;
}