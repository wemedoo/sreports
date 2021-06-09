function reloadTable(initLoad) {
    $('#advancedFilterModal').modal('hide');
    setFilterElements();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    //checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();

    $.ajax({
        type: 'GET',
        url: '/Form/ReloadByFormThesaurusTable',
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
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Title', $('#TitleTemp').val());
        addPropertyToObject(requestObject, 'ThesaurusId', $('#ThesaurusIdTemp').val());
        addPropertyToObject(requestObject, 'State', $('#StateTemp').val());
        addPropertyToObject(requestObject, 'Classes', $('#classes').val());
        addPropertyToObject(requestObject, 'ClassesOtherValue', $('#documentClassOtherInput').val());
        addPropertyToObject(requestObject, 'GeneralPurpose', $('#generalPurpose').val());
        addPropertyToObject(requestObject, 'ContextDependent', $('#contextDependent').val());
        addPropertyToObject(requestObject, 'ExplicitPurpose', $('#explicitPurpose').val());
        addPropertyToObject(requestObject, 'ScopeOfValidity', $('#scopeOfValidity').val());
        addPropertyToObject(requestObject, 'ClinicalDomain', $('#clinicalDomain').val());
        addPropertyToObject(requestObject, 'ClinicalContext', $('#clinicalContext').val());
        addPropertyToObject(requestObject, 'FollowUp', $('#documentFollowUpSelect').val());
        addPropertyToObject(requestObject, 'AdministrativeContext', $('#administrativeContext').val());
        addPropertyToObject(requestObject, 'DateTimeTo', $('#dateTimeTo').val());
        addPropertyToObject(requestObject, 'DateTimeFrom', $('#dateTimeFrom').val());
    }

    return requestObject;
}

function loadFormInstances(event, formId, thesaurusId, title, versionId) {
    var documentClass = $('i[name="Classes"]:first').attr('data-value');
    var generalPurpose = $('i[name="GeneralPurpose"]:first').attr('data-value');
    var explicitPurpose = $('i[name="ExplicitPurpose"]:first').attr('data-value');
    var scopeOfValidity = $('i[name="ScopeOfValidity"]:first').attr('data-value');
    var clinicalDomain = $('i[name="ClinicalDomain"]:first').attr('data-value');
    var clinicalContext = $('i[name="ClinicalContext"]:first').attr('data-value');
    var administrativeContext = $('i[name="AdministrativeContext"]:first').attr('data-value');
    var documentClassOtherInput = $('i[name="DocumentClassOtherInput"]:first').attr('data-value');
    var documentFollowUpSelect = $('i[name="DocumentFollowUpSelect"]:first').attr('data-value');


    window.location.href = `/FormInstance/GetAllByFormThesaurus?VersionId=${versionId}&FormId=${formId}&ThesaurusId=${thesaurusId}&Title=${encodeURIComponent(title)}&DocumentClass=${documentClass}&ClassesOtherValue=${documentClassOtherInput}&FollowUp=${documentFollowUpSelect}&GeneralPurpose=${generalPurpose}&ExplicitPurpose=${explicitPurpose}&ScopeOfValidity=${scopeOfValidity}&ClinicalDomain=${clinicalDomain}&ClinicalContext=${clinicalContext}&AdministrativeContext=${administrativeContext}`;
}


function downloadPdfs(event) {
    event.preventDefault();
    event.stopPropagation();
    var chkArray = getCheckedRows();

    if (chkArray.length === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }

    downloadDocuments(`/Pdf/GetPdfForFormId`, chkArray);
}

function downloadCSVs(event) {
    event.stopPropagation();
    event.preventDefault();
    let checkedRows = getCheckedRows();

    if (checkedRows.length === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }
    for (var i = 0; i < checkedRows.length; i++) {
        var formId = $(checkedRows[i]).val();
        var formTitle = $(checkedRows[i]).data('title');

        getDocument(`/FormInstance/ExportToCSV?formId=${formId}`, `${formTitle}.csv`);
    } 
}

function downloadDocuments(baseUrl, chkArray) {
    for (var i = 0; i < chkArray.length; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        getDocument(`${baseUrl}?formId=${formId}`, formTitle);
    } 
}

function getCheckedRows() {
    var result = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        result.push(element);
    });
    return result;
}

function getDocument(url, title) {
    console.log(title);
    $.ajax({
        url: url,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'blob';
            return xhr;
        },
        success: function (data) {
            const url = window.URL.createObjectURL(data);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = title + '.pdf';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        }
    });
}

$(document).on('change', '#selectAllCheckboxes', function () {
        var c = this.checked;
        $(':checkbox').prop('checked', c);
});

function advanceFilter() {
    $('#TitleTemp').val($('#title').val());
    $('#ThesaurusIdTemp').val($('#thesaurusId').val());
    $('#StateTemp').val($('#state').val()).change();

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');
    filterData();
    //clearFilters();
}

function mainFilter() {
    $('#title').val($('#TitleTemp').val());
    $('#thesaurusId').val($('#ThesaurusIdTemp').val());
    $('#state').val($('#StateTemp').val()).change();

    $('#advancedId').children('div:first').removeClass('btn-advanced');
    $('#advancedId').find('button:first').addClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'none');

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#title').val('');
    $('#thesaurusId').val('');
    $('#state').val('');
    $('#TitleTemp').val('');
    $('#ThesaurusIdTemp').val('');
    $('#StateTemp').val('');
    $('#classes').val('');
    $('#generalPurpose').val('');
    $('#documentClassOtherInput').val('');
    $('#contextDependent').val('');
    $('#explicitPurpose').val('');
    $('#scopeOfValidity').val('');
    $('#clinicalDomain').val('');
    $('#clinicalContext').val('');
    $('#administrativeContext').val('');
    $('#documentFollowUpSelect').val('');
}
