function createOrganizationEntry(){
    window.location.href = "/Organization/Create";
}

function editEntity(event,id) {
    window.location.href = `/Organization/Edit?organizationId=${id}`;
    event.preventDefault();
}

function removeOrganizationEntry(event, id, rowVersion) {
    event.preventDefault();
    event.stopPropagation();
    let data = {
        id: id,
        rowVersion: rowVersion
    };
    $.ajax({
        type: "POST",
        url: `/Organization/Delete`,
        data: data,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
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
    setAdvancedFilterBtnStyle(requestObject, ['Name', 'ClinicalDomain', 'Type', 'Page', 'PageSize']);
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
        url: '/Organization/ReloadTable',
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
    var name = $("#organizationName").val();
    var type = $("#organizationType").val();
    var clinicalDomain = $("#organizationClinicalDomain").val();
    var alias = $("#alias").val();
    var activity = $("#activity").val();
    var identifierType = $("#identifierType").val();
    var identifierValue = $("#identifierValue").val();
    var state = $("#state").val();
    var countryId = $("#countryId").val();
    var postalCode = $("#postalCode").val();
    var street = $("#street").val();
    var city = $("#city").val();
    var parentId = $("#parent").val();
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'Name', name);
        addPropertyToObject(result, 'City', city);
        addPropertyToObject(result, 'Type', type);
        addPropertyToObject(result, 'Alias', alias);
        addPropertyToObject(result, 'Activity', activity);
        addPropertyToObject(result, 'IdentifierType', identifierType);
        addPropertyToObject(result, 'IdentifierValue', identifierValue);
        addPropertyToObject(result, 'State', state);
        addPropertyToObject(result, 'CountryId', countryId);
        addPropertyToObject(result, 'PostalCode', postalCode);
        addPropertyToObject(result, 'Street', street);
        addPropertyToObject(result, 'ClinicalDomain', clinicalDomain);
        addPropertyToObject(result, 'Parent', parentId);
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'IdentifierType');
    getFilterParameterObjectForDisplay(filterObject, 'Type');

    if (filterObject.hasOwnProperty('Parent')) {
        let parentDisplay = $('#select2-parent-container').attr('title');
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'Parent', parentDisplay);
        }
    }

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

function mainFilter() {
    $('#name').val($('#organizationName').val());
    $('#type').val($('#organizationType').val());
    $('#clinicalDomain').val($('#organizationClinicalDomain').val());

    filterData();
    //clearFilters();
}

function advanceFilter() {
    $('#organizationName').val($('#name').val());
    $('#organizationType').val($('#type').val());
    $('#organizationClinicalDomain').val($('#clinicalDomain').val());

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#name').val('');
    $('#city').val('');
    $('#checkBoxGroup').val('');
    $('#clinicalDomain').val('');
    $('#alias').val('');
    $('#activity').val('');
    $('#identifierType').val('');
    $('#identifierValue').val('');
    $('#state').val('');
    $('#country').val('');
    $('#postalCode').val('');
    $('#street').val('');
    $('#parentId').val('');
}