
function showGenerateModal(e, id) {
    e.stopPropagation();
    e.preventDefault();
    $('#language').val('');

    $('#buttonSubmitGenerate').attr('data-formid', id);
    $('#generateModal').modal('show');
}

function generateNewLanguage() {
    var request = {
        formId: $('#buttonSubmitGenerate').data('formid'),
        language: $('#language').val()
    };

    $.ajax({
        type: "GET",
        url: "/Form/GenerateNewLanguage",
        data: request,
        success: function (data) {
            toastr.success(`Success. Please change your language to see the changes.`);
            $('#generateModal').modal('hide');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${xhr.status} ${thrownError}`);
        }
    });

    return false;
}