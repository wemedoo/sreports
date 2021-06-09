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
    $('#advancedFilterModal').modal('hide');
    setFilterElements();
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
        if ($('#BirthDateTemp').val()) {
            result['BirthDate'] = new Date($('#BirthDateTemp').val().trim()).toLocaleDateString();
        }
        if ($('#GivenTemp').val()) {
            result['Given'] = $('#GivenTemp').val().trim();
        }
        if ($('#FamilyTemp').val()) {
            result['Family'] = $('#FamilyTemp').val().trim();
        }
        if ($('#postalCode').val()) {
            result['PostalCode'] = $('#postalCode').val().trim();
        }

    }
    
    return result;
}

function advanceFilter() {
    $('#FamilyTemp').val($('#family').val());
    $('#GivenTemp').val($('#given').val());
    $('#BirthDateTemp').val($('#birthDate').val());

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');

    filterData();
    //clearFilters();
    
}
function mainFilter() {
    $('#family').val($('#FamilyTemp').val());
    $('#given').val($('#GivenTemp').val());
    $('#birthDate').val(new Date($("#BirthDateTemp").val()).toLocaleDateString());

    $('#advancedId').children('div:first').removeClass('btn-advanced');
    $('#advancedId').find('button:first').addClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'none');

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#family').val('');
    $('#given').val('');
    $('#birthDate').val('');
    $('#identifierType').val('');
    $('#identifierValue').val('');
    $('#city').val('');
    $('#country').val('');
    $('#postalCode').val('');
    $('#FamilyTemp').val('');
    $('#GivenTemp').val('');
    $('#BirthDateTemp').val('');
}

$('#mainFilterBirthCalendar').click(function () {
    $("#BirthDateTemp").datepicker({
        dateFormat: df
    }).focus();
});

$('#advancedFilterBirthCalendar').click(function () {
    $("#birthDate").datepicker({
        dateFormat: df
    }).focus();
});