var params;

function createFormInstance(id, language) {
    if (simplifiedApp) {
        window.location.href = `/crf/create?id=${id}&language=${language}`;
    } else {
        window.location.href = `/FormInstance/Create?VersionId=${filter.VersionId}&ThesaurusId=${filter.ThesaurusId}&DocumentClass=${filter.DocumentClass}&ClassesOtherValue=${filter.ClassesOtherValue}&FollowUp=${filter.FollowUp}&GeneralPurpose=${filter.GeneralPurpose}&ExplicitPurpose=${filter.ExplicitPurpose}&ScopeOfValidity=${filter.ScopeOfValidity}&ClinicalDomain=${filter.ClinicalDomain}&ClinicalContext=${filter.ClinicalContext}&AdministrativeContext=${filter.AdministrativeContext}`;
    }
}

function createPdfFormInstance(formId) {
    $(window).unbind('beforeunload');
    window.location.href = `/Pdf/GetPdfForFormId?formId=${formId}`;

    $(window).on('beforeunload', function (event) {
        $("#loaderOverlay").show(100);
    });
}

function showUploadModal(e) {
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
            toastr.error(` Error: ${xhr.status} ${thrownError}`);
        }
    });
    return false;
}

function editFormDefinition(id) {
    window.location.href = `/Form/Edit?formId=${id}`;
}

function downloadTxt() {
    var chkArray = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        chkArray.push(element);
    });

    if (chkArray.length === 0) {
        toastr.warning("Please select at least one document for export.");
        return;
    }
    for (var i = 0; i < chkArray.length; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        exportToTxt(formId, formTitle);
    }
}

function exportToTxt(id, formTitle) {
    event.stopPropagation();
    $.ajax({
        url: `/FormInstance/ExportToTxt?FormInstanceId=${id}`,
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
        url = `/FormInstance/Edit?VersionId=${filter.VersionId}&FormInstanceId=${id}&DocumentClass=${filter.DocumentClass}&ClassesOtherValue=${filter.ClassesOtherValue}&FollowUp=${filter.FollowUp}&GeneralPurpose=${filter.GeneralPurpose}&ExplicitPurpose=${filter.ExplicitPurpose}&ScopeOfValidity=${filter.ScopeOfValidity}&ClinicalDomain=${filter.ClinicalDomain}&ClinicalContext=${filter.ClinicalContext}&AdministrativeContext=${filter.AdministrativeContext}`;
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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`${errorThrown}`);
        }
    });
}


function reloadTable(initLoad) {
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();

    $.ajax({
        type: 'GET',
        url: '/FormInstance/ReloadByFormThesaurusTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Status: ${textStatus}; Error: ${errorThrown}`);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};

    if (filter) {
        requestObject = filter;
        defaultFilter = null;
    } else {
        requestObject.VersionId = $('#VersioId').html();
        requestObject.ThesaurusId = $('#thesaurusId').html();
        requestObject.Classes = $('#DocumentClass').html();
        requestObject.GeneralPurpose = $('#GeneralPurpose').html();
        requestObject.ExplicitPurpose = $('#ExplicitPurpose').html();
        requestObject.ScopeOfValidity = $('#ScopeOfValidity').html();
        requestObject.ClinicalDomain = $('#ClinicalDomain').html();
        requestObject.ClinicalContext = $('#ClinicalContext').html();
        requestObject.AdministrativeContext = $('#AdministrativeContext').html();
    }

    return requestObject;
}

document.getElementById("file").onchange = function () {
    document.getElementById("uploadFile").value = this.value.replace("C:\\fakepath\\", "");
};

function downloadJsons() {
    var chkArray = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        chkArray.push(element);
    });

    if (chkArray.length === 0) {
        toastr.warning("Please select at least one document for export.");
        return;
    }
    for (var i = 0; i < chkArray.length; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        getJson(formId, formTitle);
    }
}

function getJson(formId, formTitle) {

    $.ajax({
        url: `/Patholink/Export?formInstanceId=${formId}`,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'application/json';
            return xhr;
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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Status: ${textStatus}; Error: ${errorThrown}`);
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
        && !$(e.target).hasClass('form-checkbox-button')
        && !$(e.target).hasClass('form-checkbox-field')
        && !$(e.target).find('.form-checkbox-button').length > 0) {
        editEntity(e,id);
        e.stopPropagation();
    }
}

$(document).on('change', '#selectAllCheckboxes', function () {
    var c = this.checked;
    $(':checkbox').prop('checked', c);
});



