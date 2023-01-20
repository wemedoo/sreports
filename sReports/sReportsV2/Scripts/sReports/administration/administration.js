function getPredefinedType() {
    // empty default filter
    getFilterParametersObject();
    changeBtnIfCodeSystemSelected();
    filterTableData();
}

function cancelAddingNewType(predefinedType) {
    window.location.href = `/Administration/GetAll?PredefinedType=${predefinedType}`;
}

function createNewType() {
    var predefinedType = $("#predefinedType").val();
    var preferredTerm = $("#preferredTermSearch").val();
    var page = currentPage;
    var pageSize = getPageSize();
    if (predefinedType != "")
        window.location.href = `/Administration/Create?page=${page}&pageSize=${pageSize}&PredefinedType=${predefinedType}&PreferredTerm=${preferredTerm}`;
    else
        toastr.error(`Please choose predefined type`);
}

function changeBtnIfCodeSystemSelected() {
    var selectedPrefType = $("#predefinedType").val();
    var clickHandlerName = '';
    if (selectedPrefType == 'CodeSystem') {
        clickHandlerName = 'createNewCodeSystem()';
    } else {
        clickHandlerName = 'createNewType()';
    }
    $("#createPredefinedTypeBtn").attr("onclick", clickHandlerName);
}

function submitFormPredefined(e, predefinedType, id) {
    var request = {};
    request['Label'] = $("#preferredTerm").val();
    request['Type'] = predefinedType;
    request['ThesaurusEntryId'] = id;

    $.ajax({
        type: "POST",
        url: `/Administration/Create`,
        data: request,
        success: function (data) {
            toastr.options = {
                timeOut: 100
            }
            toastr.options.onHidden = function () { window.location.href = `/Administration/GetAll?PredefinedType=${predefinedType}`; }
            toastr.success("Success");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });

    return false;
}

function setPreferedTermFromUrl() {
    var item = null;
    let url = new URL(window.location.href);
    let entries = url.searchParams.entries();
    let params = paramsToObject(entries);
    item = Object.values(params)[3];

    return item;
}

$(document).on('click', '#predefinedTypeSelect', function (e) {
    $("#predefinedType").show().focus().click();
});

function createNewCodeSystem() {
    resetCodeForm();
    showCodeModalTitle("addCodeSystemTitle");
    $('#codeSystemModal').modal('show');
}

function editCodeSystem(event) {
    event.preventDefault();
    event.stopPropagation();

    resetCodeForm();
    setCodeSystemFormValues($(event.target).closest("tr"));
    showCodeModalTitle("editCodeSystemTitle");
    $('#codeSystemModal').modal('show');
}

function addCodeSystem(e) {
    e.preventDefault();
    e.stopPropagation();

    if ($('#codeSystemForm').valid()) {
        let codingObject = getNewCodeSystem();
        $('#codeSystemModal').modal('hide');

        $.ajax({
            type: "post",
            data: codingObject,
            url: `/Administration/CreateCodeSystem`,
            success: function (data, textStatus, jqXHR) {
                filterTableData();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function getNewCodeSystem() {
    return {
        "Id": $('#codeSystemId').val(),
        "Label": $('#codeSystemLabel').val(),
        "Value": $('#codeSystemValue').val(),
        "SAB": $('#codeSystemSAB').val()
    }
}

function setCodeSystemFormValues(codeSystemRow) {
    $(codeSystemRow).find("td[data-property]").each(function () {
        $(`#${$(this).data('property')}`).val($(this).data('value'));
    });
}

function resetCodeForm() {
    $('#codeSystemId').val('');
    $('#codeSystemValue').val('');
    $('#codeSystemLabel').val('');
    $('#codeSystemSAB').val('');

    resetValidationColor();
}

function resetValidationColor() {
    $('#codeSystemValue').removeClass('error');
    $('#codeSystemLabel').removeClass('error');
    $('#codeSystemSAB').removeClass('error');
}

function showCodeModalTitle(titleId) {
    $(".code-system-form-title").hide();
    $(`#${titleId}`).show();
}