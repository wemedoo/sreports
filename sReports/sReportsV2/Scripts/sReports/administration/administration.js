function getPredefinedType() {
    getFilterParametersObject();
    filterTableData();
}

function cancelAddingNewType(predefinedType) {
    window.location.href = `/Administration/GetAll?PredefinedType=${predefinedType}`;
}

function createNewType() {
    var predefinedType = $("#predefinedType").val();
    var page = currentPage;
    var pageSize = getPageSize();
    if (predefinedType != "")
        window.location.href = `/Administration/Create?page=${page}&pageSize=${pageSize}&PredefinedType=${predefinedType}`;
    else
        toastr.error(`Please choose predefined type`);
}

function submitFormPredefined(e, predefinedType, id) {
    var request = {};
    request['Label'] = $("#preferredTerm").val();
    request['Type'] = predefinedType;
    request['ThesaurusId'] = id;

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
            toastr.error(`${thrownError} `);
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