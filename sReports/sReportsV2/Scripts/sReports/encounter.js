function submitEncounterForm(event, form) {
    event.preventDefault();
    event.stopPropagation();
    //$('#idEpisodeOfCare').validate();
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
        request['LastUpdate'] = $("#lastUpdate").val();
        console.log(new Date().toISOString());
        request['Period'] = {
            StartDate: new Date($('#start').val()).toISOString() ,
            EndDate: $('#end').val() ? new Date($('#end').val()).toISOString() : null
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
                toastr.error(`${thrownError} `);
            }
        });

    }
    return false;

}



function deleteEncounter(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/Encounter/Delete?id=${id}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);

            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

