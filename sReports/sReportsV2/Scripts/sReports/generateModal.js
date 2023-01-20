
function showGenerateModal(e, id) {
    e.stopPropagation();
    e.preventDefault();
    $('#language').val('');

    document.getElementById("buttonSubmitGenerate").setAttribute('data-formid', id);
    $('#generateModal').modal('show');
}

function generateNewLanguage() {
    var request = {
        formId: document.getElementById("buttonSubmitGenerate").getAttribute('data-formid'),
        language: $('#language').val()
    };

    $.ajax({
        type: "GET",
        url: "/Form/GenerateNewLanguage",
        data: request,
        success: function (data) {
            toastr.success(data);
            $('#generateModal').modal('hide');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });

    return false;
}