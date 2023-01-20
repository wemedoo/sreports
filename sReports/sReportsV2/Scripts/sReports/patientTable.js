function editEntity(event,id) {
    window.location.href = `/Patient/Edit?patientId=${id}`;
    event.preventDefault();
}

function createPatientEntry() {
    window.location.href = "/Patient/Create";
}

function removePatientEntry(event, id) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/Patient/Delete?PatientId=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);

            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function reloadTable() {
    $('#advancedFilterModal').modal('hide');
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setFilterTagsFromObj(requestObject);
    setAdvancedFilterBtnStyle(requestObject, ['Given', 'Family', 'BirthDate', 'page', 'pageSize']);
    requestObject.Page = getPageNum();
    requestObject.PageSize = getPageSize();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;

    $.ajax({
        type: 'GET',
        url: '/Patient/ReloadTable',
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
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        if ($('#identifierType').val()) {
            addPropertyToObject(result, 'IdentifierType', $('#identifierType').val());
        }
        if ($('#identifierValue').val()) {
            addPropertyToObject(result, 'IdentifierValue', $('#identifierValue').val());
        }
        if ($('#countryId').val()) {
            addPropertyToObject(result, 'CountryId', $('#countryId').val());
        }
        if ($('#city').val()) {
            addPropertyToObject(result, 'City', $('#city').val().trim());
        }
        if ($('#BirthDateTemp').val()) {
            addPropertyToObject(result, 'BirthDate', $('#birthDateDefault').val());
        }
        if ($('#GivenTemp').val()) {
            addPropertyToObject(result, 'Given', $('#GivenTemp').val().trim());
        }
        if ($('#FamilyTemp').val()) {
            addPropertyToObject(result, 'Family', $('#FamilyTemp').val().trim());
        }
        if ($('#postalCode').val()) {
            addPropertyToObject(result, 'PostalCode', $('#postalCode').val().trim());
        }
    }
    
    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'IdentifierType');

    if (filterObject.hasOwnProperty('CountryId')) {
        let countryNameByHidden = $('#countryName').val();
        if (countryNameByHidden) {
            addPropertyToObject(filterObject, 'CountryId', countryNameByHidden);
        }
        let countryNameBySelect2 = $('#select2-countryId-container').attr('title');
        if (countryNameBySelect2) {
            addPropertyToObject(filterObject, 'CountryId', countryNameBySelect2);
        }
    }

    return filterObject;
}

function advanceFilter() {
    $('#FamilyTemp').val($('#family').val());
    $('#GivenTemp').val($('#given').val());
    $('#BirthDateTemp').val($('#birthDate').val());
    copyDateToHiddenField($("#BirthDateTemp").val(), "birthDateDefault");

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');

    filterData();
    //clearFilters();
    
}
function mainFilter() {
    $('#family').val($('#FamilyTemp').val());
    $('#given').val($('#GivenTemp').val());
    $('#birthDate').val($("#BirthDateTemp").val());
    copyDateToHiddenField($("#birthDate").val(), "birthDateDefault");

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
    $('#birthDateDefault').val('');
}