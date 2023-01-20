function reloadTable(initLoad) {
    $('#advancedFilterModal').modal('hide');
    setFilterTagsFromUrl();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setAdvancedFilterBtnStyle(requestObject, ['Title', 'State', 'ThesaurusId', 'page', 'pageSize']);
    //checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;

    $.ajax({
        type: 'GET',
        url: '/Form/ReloadByFormThesaurusTable',
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
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Content', $('#ContentTemp').val());
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
        addPropertyToObject(requestObject, 'DateTimeTo', toLocaleDateStringIfValue($('#dateTimeTo').val()));
        addPropertyToObject(requestObject, 'DateTimeFrom', toLocaleDateStringIfValue($('#dateTimeFrom').val()));
    }
    if (requestObject['DateTimeFrom']) {
        addPropertyToObject(requestObject, 'DateTimeFrom', toValidTimezoneFormat(requestObject['DateTimeFrom']));
    }
    if (requestObject['DateTimeTo']) {
        addPropertyToObject(requestObject, 'DateTimeTo', toValidTimezoneFormat(requestObject['DateTimeTo']));
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

    var url = `/FormInstance/GetAllByFormThesaurus?VersionId=${versionId}&FormId=${formId}&ThesaurusId=${thesaurusId}&Title=${encodeURIComponent(decodeLocalizedString(title))}`;
    if (documentClass) {
        url = `${url}&DocumentClass=${documentClass}`
    }
    if (generalPurpose) {
        url = `${url}&GeneralPurpose=${generalPurpose}`
    }
    if (explicitPurpose) {
        url = `${url}&ExplicitPurpose=${explicitPurpose}`
    }
    if (scopeOfValidity) {
        url = `${url}&ScopeOfValidity=${scopeOfValidity}`;
    }
    if (clinicalDomain) {
        url = `${url}&ClinicalDomain=${clinicalDomain}`;
    }
    if (clinicalContext) {
        url = `${url}&ClinicalContext=${clinicalContext}`;
    }
    if (administrativeContext) {
        url = `${url}&AdministrativeContext=${administrativeContext}`;
    }
    if (documentClassOtherInput) {
        url = `${url}&ClassesOtherValue=${documentClassOtherInput}`;
    }
    if (documentFollowUpSelect) {
        url = `${url}&FollowUp=${documentFollowUpSelect}`;
    }

    window.location.href = url;
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

    var numOfSelectedDocuments = checkedRows.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(checkedRows[i]).val();
        var formTitle = $(checkedRows[i]).data('title');

        getDocument(`/FormInstance/ExportToCSV?formId=${formId}`, `${formTitle}.csv`, i === numOfSelectedDocuments - 1);
    } 
}

function downloadDocuments(baseUrl, chkArray) {
    var numOfSelectedDocuments = chkArray.length;
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        getDocument(`${baseUrl}?formId=${formId}`, formTitle, i === numOfSelectedDocuments - 1);
    } 
}

function getCheckedRows() {
    var result = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        result.push(element);
    });
    return result;
}

function getDocument(url, title, lastFile = true) {
    console.log(title);
    $.ajax({
        url: url,
        beforeSend: function (request) {
            request.setRequestHeader("LastFile", lastFile);
        },
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
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
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

    filterData();
    //clearFilters();
}

function mainFilter() {
    $('#content').val($('#ContentTemp').val());
    $('#title').val($('#TitleTemp').val());
    $('#thesaurusId').val($('#ThesaurusIdTemp').val());
    $('#state').val($('#StateTemp').val()).change();

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#content').val('');
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
