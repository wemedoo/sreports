var params;

function createFormInstance(id, language) {
    if (simplifiedApp) {
        window.location.href = `/crf/create?id=${id}&language=${language}`;
    } else {
        window.location.href = extendDefaultUrlWithParameters(`/FormInstance/Create?VersionId=${filter.VersionId}&ThesaurusId=${filter.ThesaurusId}`, filter);
    }
}

function createPdfFormInstance(event, formId) {
    event.stopPropagation();
    event.preventDefault();
    
    $(window).unbind('beforeunload');
    window.location.href = `/Pdf/GetPdfForFormId?formId=${formId}`;

    $(window).on('beforeunload', function (event) {
        $("#loaderOverlay").show(100);
    });
}

function showUploadModal(e) {
 
    e.preventDefault();
    e.stopPropagation();

    $('#uploadModal').modal('show');
}

function upload() {

    var fd = new FormData(),
        myFile = document.getElementById("file").files[0];

    fd.append('file', myFile);

    $.ajax({
        url: `/Pdf/Upload`,
        data: fd,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
            $("#uploadModal").modal('toggle');
            toastr.success(`Success`);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#uploadModal").modal('toggle');
            handleResponseError(xhr, thrownError);
        }
    });
    return false;
}

function editFormDefinition(id) {
    window.location.href = `/Form/Edit?formId=${id}`;
}

function downloadTxt(event) {
    event.stopPropagation();
    event.preventDefault();
    var chkArray = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        chkArray.push(element);
    });

    var numOfSelectedDocuments = chkArray.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document for export.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        exportToTxt(formId, formTitle, i === numOfSelectedDocuments - 1);
    }
}

function exportToTxt(id, formTitle, lastFile = true) {
    event.stopPropagation();
    $.ajax({
        url: `/FormInstance/ExportToTxt?FormInstanceId=${id}`,
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
            a.download = formTitle + '.txt';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        }
    });
}

function editEntity(event, id) {
    event.preventDefault();
    if (simplifiedApp) {
        let language = $('.dropdown-menu').find('.language.active')[0];
        url = `${simplifiedApp}?FormInstanceId=${id}&language=${$(language).data('value')}`;
    } else {
        url = extendDefaultUrlWithParameters(`/FormInstance/Edit?VersionId=${filter.VersionId}&FormInstanceId=${id}`, filter);
    }
    window.location.href = url;
}

function removeFormInstance(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/FormInstance/Delete?formInstanceId=${id}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success('Removed');
            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}


function reloadTable(initLoad) {
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    setFilterTagsFromObj(requestObject);

    $.ajax({
        type: 'GET',
        url: '/FormInstance/ReloadByFormThesaurusTable',
        data: requestObject,
        traditional: true, // Explanation: https://stackoverflow.com/questions/31152130/is-it-good-to-use-jquery-ajax-with-traditional-true/31152304#31152304
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};

    if (filter) {
        requestObject = filter;
        defaultFilter = null;
    } else {
        requestObject.Content = $('#content').val();
        //requestObject.UserIds = setUser();
        //requestObject.PatientIds = setPatient();
        requestObject.VersionId = $('#VersioId').val();
        requestObject.ThesaurusId = $('#thesaurusId').val();
        requestObject.Classes = $('#DocumentClass').val();
        requestObject.GeneralPurpose = $('#GeneralPurpose').val();
        requestObject.ExplicitPurpose = $('#ExplicitPurpose').val();
        requestObject.ScopeOfValidity = $('#ScopeOfValidity').val();
        requestObject.ClinicalDomain = $('#ClinicalDomain').val();
        requestObject.ClinicalContext = $('#ClinicalContext').val();
        requestObject.AdministrativeContext = $('#AdministrativeContext').val();
    }

    return requestObject;
}

document.getElementById("file").onchange = function () {
    document.getElementById("uploadFile").value = this.value.replace("C:\\fakepath\\", "");
};

function downloadJsons(event) {
    event.preventDefault();
    event.stopPropagation();
    var chkArray = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        chkArray.push(element);
    });

    var numOfSelectedDocuments = chkArray.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document for export.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        getJson(formId, formTitle, i === numOfSelectedDocuments - 1);
    }
}

function getJson(formId, formTitle, lastFile = true) {
    $.ajax({
        url: `/Patholink/Export?formInstanceId=${formId}`,
        beforeSend: function (request) {
            request.setRequestHeader("LastFile", lastFile);
        },
        success: function (data) {
            var jsonse = JSON.stringify(data, null, 2);
            var blob = new Blob([jsonse], { type: "application/json" });
            var url = window.URL.createObjectURL(blob);
            //const url = window.URL.createObjectURL(data);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = formTitle;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function redirectToDistributionParams(thesaurusId) {
    window.location.href = `/FormDistribution/GetByThesaurusId?thesaurusId=${thesaurusId}`;
}

function clickedRow(e, id) {
    if (!$(e.target).hasClass('dropdown-button')
        && !$(e.target).hasClass('fa-bars')
        && !$(e.target).hasClass('dropdown-item')
        && !$(e.target).hasClass('dots')
        && !$(e.target).hasClass('form-checkbox-button')
        && !$(e.target).hasClass('form-checkbox-field')
        && !$(e.target).find('.form-checkbox-button').length > 0) {
        editEntity(e,id);
        e.stopPropagation();
    }
}

function extendDefaultUrlWithParameters(url, filter) {
    if (filter.DocumentClass) {
        url = `${url}&DocumentClass=${filter.DocumentClass}`
    }
    if (filter.GeneralPurpose) {
        url = `${url}&GeneralPurpose=${filter.GeneralPurpose}`
    }
    if (filter.ExplicitPurpose) {
        url = `${url}&ExplicitPurpose=${filter.ExplicitPurpose}`
    }
    if (filter.ScopeOfValidity) {
        url = `${url}&ScopeOfValidity=${filter.ScopeOfValidity}`;
    }
    if (filter.ClinicalDomain) {
        url = `${url}&ClinicalDomain=${filter.ClinicalDomain}`;
    }
    if (filter.ClinicalContext) {
        url = `${url}&ClinicalContext=${filter.ClinicalContext}`;
    }
    if (filter.AdministrativeContext) {
        url = `${url}&AdministrativeContext=${filter.AdministrativeContext}`;
    }
    if (filter.ClassesOtherValue) {
        url = `${url}&ClassesOtherValue=${filter.ClassesOtherValue}`;
    }
    if (filter.FollowUp) {
        url = `${url}&FollowUp=${filter.FollowUp}`;
    }

    return url;
}

$(document).on('change', '#selectAllCheckboxes', function () {
    var c = this.checked;
    $(':checkbox').prop('checked', c);
});

function singleDocumentFilter() {
    $('#content').val($('#ContentTemp').val());

    if (filter) {
        filter['Content'] = $('#ContentTemp').val();
        //filter['UserIds'] = setUser();
        //filter['PatientIds'] = setPatient();
    }

    filterData();
    //clearSingleDocumentFilters();
}

function clearSingleDocumentFilters() {
    $('#ContentTemp').val(' ');
}

function advanceFilter() {

    $('#ContentTemp').val($('#content').val());

    singleDocumentFilter();
    //clearFilters();
}

function getFilterParametersObjectForDisplay(requestObject) {
    let filterObject = {};
    filterObject['Content'] = requestObject['Content'];

    return filterObject;
}