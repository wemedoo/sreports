function reloadTable(initLoad) {
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
        addPropertyToObject(requestObject, 'Classes', $('#documentClass').val());
        addPropertyToObject(requestObject, 'ClassesOtherValue', $('#documentClassOtherInput').val());
        addPropertyToObject(requestObject, 'GeneralPurpose', $('#generalPurpose').val());
        addPropertyToObject(requestObject, 'ContextDependent', $('#contextDependent').val());
        addPropertyToObject(requestObject, 'ExplicitPurpose', $('#explicitPurpose').val());
        addPropertyToObject(requestObject, 'ScopeOfValidity', $('#scopeOfValidity').val());
        addPropertyToObject(requestObject, 'ClinicalDomain', $('#clinicalDomain').val());
        addPropertyToObject(requestObject, 'ClinicalContext', $('#clinicalContext').val());
        addPropertyToObject(requestObject, 'FollowUp', $('#documentFollowUpSelect').val());
        addPropertyToObject(requestObject, 'AdministrativeContext', $('#administrativeContext').val());
    }

    return requestObject;
}

function loadFormInstances(event, formId, thesaurusId, title,versionId) {
    var documentClass = $('#documentClass').val();
    var generalPurpose = $('#generalPurpose').val();
    var explicitPurpose = $('#explicitPurpose').val();
    var scopeOfValidity = $('#scopeOfValidity').val();
    var clinicalDomain = $('#clinicalDomain').val();
    var clinicalContext = $('#clinicalContext').val();
    var administrativeContext = $('#administrativeContext').val();
    var documentClassOtherInput = $('#documentClassOtherInput').val();
    var documentFollowUpSelect = $('#documentFollowUpSelect').val();


    window.location.href = `/FormInstance/GetAllByFormThesaurus?VersionId=${versionId}&FormId=${formId}&ThesaurusId=${thesaurusId}&Title=${title}&DocumentClass=${documentClass}&ClassesOtherValue=${documentClassOtherInput}&FollowUp=${documentFollowUpSelect}&GeneralPurpose=${generalPurpose}&ExplicitPurpose=${explicitPurpose}&ScopeOfValidity=${scopeOfValidity}&ClinicalDomain=${clinicalDomain}&ClinicalContext=${clinicalContext}&AdministrativeContext=${administrativeContext}`;
}

function downloadPdfs() {
    var chkArray = getCheckedRows();

    if (chkArray.length === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }

    downloadDocuments(`/Pdf/GetPdfForFormId`, chkArray);
}

function downloadCSVs() {
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
            a.download = title;
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
