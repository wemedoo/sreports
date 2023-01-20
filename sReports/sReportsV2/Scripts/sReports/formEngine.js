//NOTE: Be careful that $.validator.methods.requiredInCore or $.validator.methods.emailInCore are core methods and not overridden methods.
//Otherwise, It will be recursive functions!
$.validator.methods.requiredInCore = $.validator.methods.required;
$.validator.methods.required = function (value, element, param) {
    if (isSpecialValueSelected($(element))) {
        return true;
    } else {
        return $.validator.methods.requiredInCore.apply(this, [value, element, param]);
    }
}

$.validator.methods.emailInCore = $.validator.methods.email;
$.validator.methods.email = function (value, element, param) {
    if (isSpecialValueSelected($(element))) {
        return true;
    } else {
        return $.validator.methods.emailInCore.apply(this, [value, element, param]);
    }
}

function handleSuccessFormSubmit() {
    let versionId = $('input[name=VersionId]').val();
    let thesaurusId = $('input[name=thesaurusId]').val();
    let formDefinitionId = $('input[name=formDefinitionId]').val();
    window.location.href = `/FormInstance/GetAllByFormThesaurus?versionId=${versionId}&thesaurusId=${thesaurusId}&formDefinitionId=${formDefinitionId}`;
}

function switchFormInstanceViewMode(viewMode) {
    $(".form-instance-view-mode").toggle();

    $.ajax({
        type: "GET",
        url: `/FormInstance/GetFormInstanceContent`,
        data: getSwitchFormInstanceViewModeRequest(viewMode),
        success: function (data) {
            setFormInstanceContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getSwitchFormInstanceViewModeRequest(viewMode) {
    var request = {};
    request["viewMode"] = viewMode;
    request["formInstanceId"] = getFormInstanceId();
    return request;
}

function setFormInstanceContent(data) {
    $("#formInstanceContentContainer").html(data);
}

function downloadSynopticPdf(formTitle) {
    request = {};
    request['formInstanceId'] = $("input[name=formInstanceId]").val();

    $.ajax({
        type: 'GET',
        url: `/Pdf/GetSynopticPdf`,
        data: request,
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
            a.download = formTitle;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

//NOTE: Signing of form instances is only possible through Engine module!
function showSignDocumentModal(event, formInstanceNextState) {
    event.preventDefault();
    event.stopPropagation();

    $.ajax({
        type: "GET",
        url: '/FormInstance/GetSignDocumentModel',
        data: getSignDocumentModalRequest(formInstanceNextState),
        success: function (data) {
            showSignDocumentModalContent(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getSignDocumentModalRequest(formInstanceNextState) {
    var request = {};
    request['formInstanceNextState'] = formInstanceNextState;
    return request;
}

function showSignDocumentModalContent(data) {
    $("#signDocumentModalFormContainer").html(data);
    $("#signDocumentModal").modal("show");
}

$(document).on("keypress", "#signDocumentPassword", function (e) {
    if (e.which === 13) {
        signDocument(e);
    }
})

function signDocument(event) {
    event.preventDefault();
    event.stopPropagation();

    $.ajax({
        type: "POST",
        url: '/FormInstance/SignDocument',
        data: getSignDocumentRequest(),
        success: function (data) {
            if (isSignDocumentValid(data)) {
                toastr.success("Document is signed");
                $("#signDocumentModal").modal("hide");
                window.location.reload();
            } else {
                showSignDocumentModalContent(data);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getSignDocumentRequest() {
    var request = {};
    request["formInstanceId"] = getFormInstanceId();
    request['password'] = $("#signDocumentPassword").val();
    request['formInstanceNextState'] = $("#FormInstanceNextState").val();
    return request;
}

$(document).on("focus", "#signDocumentPassword", function () {
    if ($(this).hasClass("error")) {
        $(this).removeClass("error");
        $("#sign-password-error").html('');
    }
})

function isSignDocumentValid(data) {
    return $(data).find('input.error').length == 0;
}

function getFormInstanceId() {
    return $("input[name=formInstanceId]").val();
}