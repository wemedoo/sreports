$(document).ready(function () {
    setCommonValidatorMethods();
});

function submitEncounterForm(event, form) {
    event.preventDefault();
    event.stopPropagation();
    $('#idEpisodeOfCare').validate();
    if ($(form).valid()) {
        $('.encounter-submit-button').attr('disabled', 'disabled');
        var request = {};

        request['Id'] = $("#id").val();
        request['EpisodeOfCareId'] = $("#eocId").val();
        request['PatientId'] = $("#patientId").val();
        request['Status'] = $("#status").val();
        request['Class'] = $("#classification").val();
        request['Type'] = $("#type").val();
        request['ServiceType'] = $("#servicetype").val();
        request['EpisodeOfCareId'] = $("#eocId").val();
        request['Period'] = {
            StartDate: toLocaleDateStringIfValue($('#start').val()),
            EndDate: toLocaleDateStringIfValue($('#end').val())
        }
        $.ajax({
            type: "POST",
            url: "/Encounter/Create",
            data: request,
            success: function (data, textStatus, jqXHR) {
                reloadPatientTree(request['EpisodeOfCareId'], jqXHR.statusText, 'encounter');
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });

    }
    return false;

}



function deleteEncounter(event, id) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/Encounter/Delete?id=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);

            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

