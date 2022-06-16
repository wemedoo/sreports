function submitEOCForm(form) {
    console.log('eoc submit');
    $(form).validate();

    if ($(form).valid()) {
        var period = {
            StartDate: new Date($("#periodStartDate").val()).toDateString(),
            EndDate: $("#periodEndDate").val() ? new Date($("#periodEndDate").val()).toDateString() : null

        };

        var request = {};
        request['Id'] = $("#id").val();
        request['Status'] = $("#status").val();
        request['Type'] = $("#type").val();
        request['DiagnosisCondition'] = $("#diagnosisCondition").val();
        request['DiagnosisRole'] = $("#diagnosisRole").val();
        request['DiagnosisRank'] = $("#diagnosisRank").val();
        request['Period'] = period;
        request['PatientId'] = $("#patId").val();
        request['Description'] = $("#description").val();
        request['PatientId'] = $("#patientId").val();
        request['LastUpdate'] = $("#lastUpdate").val();

        $.ajax({
            type: "POST",
            url: "/EpisodeOfCare/Create",
            data: request,
            success: function (data, textStatus, jqXHR) {
                reloadPatientTree(null, $('#id').val() || jqXHR.statusText, 'episodeofcare');
                toastr.success("Success");

            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }

        });

    }
    return false;
}

$('#startDateCalendar').click(function () {
    $("#periodStartDate").datepicker({
        dateFormat: df
    }).focus();
});

$('#endDateCalendar').click(function () {
    $("#periodEndDate").datepicker({
        dateFormat: df
    }).focus();
});