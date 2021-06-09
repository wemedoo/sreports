function createOrganizationEntry(){
    window.location.href = "/Organization/Create";
}

function editEntity(event,id) {
    window.location.href = `/Organization/Edit?organizationId=${id}`;
    event.preventDefault();
}

function removeOrganizationEntry(event,id, rowVersion) {
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
            toastr.error(`${thrownError} `);
        }
    });
}

function reloadTable() {
    $('#advancedFilterModal').modal('hide');
    let requestObject = getFilterParametersObject();
    setFilterElements();
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    $.ajax({
        type: 'GET',
        url: '/Organization/ReloadTable',
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
    var name = $("#organizationName").val();
    var type = $("#organizationType").val();
    var clinicalDomain = $("#organizationClinicalDomain").val();
    var alias = $("#alias").val();
    var activity = $("#activity").val();
    var identifierType = $("#identifierType").val();
    var identifierValue = $("#identifierValue").val();
    var state = $("#state").val();
    var country = $("#country").val();
    var postalCode = $("#postalCode").val();
    var street = $("#street").val();
    var city = $("#city").val();
    var parentId = $("#parentId").val();
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
        addPropertyToObject(result, 'Country', country);
        addPropertyToObject(result, 'PostalCode', postalCode);
        addPropertyToObject(result, 'Street', street);
        addPropertyToObject(result, 'ClinicalDomain', clinicalDomain);
        addPropertyToObject(result, 'ParentId', parentId);
    }

    return result;
}

function mainFilter() {
    $('#name').val($('#organizationName').val());
    $('#checkBoxGroup').val($('#organizationType').val());
    $('#clinicalDomain').val($('#organizationClinicalDomain').val());

    $('#advancedId').children('div:first').removeClass('btn-advanced');
    $('#advancedId').find('button:first').addClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'none');

    filterData();
    //clearFilters();
}

function advanceFilter() {
    $('#organizationName').val($('#name').val());
    $('#organizationType').val($('#checkBoxGroup').val());
    $('#organizationClinicalDomain').val($('#clinicalDomain').val());

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');

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