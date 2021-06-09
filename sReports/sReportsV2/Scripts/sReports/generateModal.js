
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
            toastr.success(`Success. Please change your language to see the changes.`);
            $('#generateModal').modal('hide');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${xhr.status} ${thrownError}`);
        }
    });

    return false;
}