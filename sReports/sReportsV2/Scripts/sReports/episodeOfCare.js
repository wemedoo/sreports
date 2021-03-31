var validator;
var episodeOfCareId;
function newEpisodeOfCare() {
    if ($('#identifierType').val() && $('#identifierValue').val()) {
        window.location.href = `/EpisodeOfCare/Create?System=${$('#identifierType').val()}&Value=${$('#identifierValue').val()}`;
    }
}

function getAllEncountersForEOC(id) {
    window.location.href = `/Encounter/GetAllByEocId?EpisodeOfCareId=${id}`;
}

function goToPatient(id) {
    let episodeOfCareId = $('#episodeOfCareId').val();
    if (episodeOfCareId) {
        history.pushState({}, '', `?EpisodeOfCareId=${episodeOfCareId}`);
    }
    window.location.href = `/Patient/Edit?patientId=${id}`;
} 

function goToEpisodeOfCare(id) {
    window.location.href = `/EpisodeOfCare/Edit?EpisodeOfCareId=${id}`;
} 

function cancelEpisodeOfCareEdit() {
    window.location.href = `/EpisodeOfCare/GetAll`;
}


function submitEOCForm(form) {
    $(form).validate();
    
    if($(form).valid())
    {
        var period = {
            StartDate: new Date($("#periodStartDate").val()).toUTCString(),
            EndDate: $("#periodEndDate").val() ? new Date($("#periodEndDate").val()).toUTCString() : null
           
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
            success: function (data) {
                //toastr.options = {
                //    timeOut: 100
                //}
               // toastr.options.onHidden = function () { window.location.href = `/EpisodeOfCare/GetAll`; }
                reloadPatientTree();

                toastr.success("Success");
                
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }

        });

    }
    return false;

}

function reloadTable() {
    if ($('#id').val()) {
        $('#diagnosticReports').show();
        let requestObject = {};
        requestObject.EpisodeOfCareId = $('#id').val();
        checkUrlPageParams();
        requestObject.Page = currentPage;
        requestObject.PageSize = getPageSize();

        $.ajax({
            type: 'GET',
            url: '/DiagnosticReport/ReloadTable',
            data: requestObject,
            success: function (data) {
                $("#diagnosticReportsContainer").html(data);

                $('#diagnosticReportsContainer').collapse('show');

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
            }
        });
    }
}

function clickedDiagnosticReportRow(e, formId, reportId) {
    if (!$(e.target).hasClass('dropdown-button')
        && !$(e.target).hasClass('fa-bars')
        && !$(e.target).hasClass('dropdown-item')
        && !$(e.target).hasClass('form-checkbox-button')
        && !$(e.target).hasClass('form-checkbox-field')
        && !$(e.target).find('.form-checkbox-button').length > 0) {
        editDiagnosticReport(e, formId, reportId);
    }
}

function editDiagnosticReport(e, episodeOfCareId, formInstanceId) {
    window.location.href = `/DiagnosticReport/Edit?episodeOfCareId=${episodeOfCareId}&diagnosticReportId=${formInstanceId}`;
    e.preventDefault();
}

function showAvailableForms(e, episodeOfCareId) {
    history.pushState({}, '', `?EpisodeOfCareId=${episodeOfCareId}`);
    window.location.href = `/DiagnosticReport/ListForms?episodeOfCareId=${episodeOfCareId}`;
}

function showAvailableFormsWithReferral(e, episodeOfCareId) {
    history.pushState({}, '', `?EpisodeOfCareId=${episodeOfCareId}`);

    let referralParams = [];
    $('[name="referral"]:checked').each(function (index, element) {
        if ($(element).val()) {
            referralParams.push(`referrals=${$(element).val()}`);
        }
    });
    window.location.href = `/DiagnosticReport/ListForms?episodeOfCareId=${episodeOfCareId}&${referralParams.join('&')}`;
}


function createDiagnosticReport(e, episodeOfCareId, formId, encounterId) {
    let referralParams = [];
    if (referrals) {
        referrals.forEach(x => {
            if (x) {
                referralParams.push(`referrals=${x}`);
            }
        });
    }

    window.location.href = `/DiagnosticReport/Create?encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}&formId=${formId}&${referralParams.join('&')}`;
} 

function formatArrayParameter(data) {
    let referralParams = [];
    $(data).each(function (index, element) {
        referralParams.push(`referrals=${$(element).val()}`);
    });

    return referralParams.join();
}

function deleteDiagnosticReport(e, episodeOfCareId, formInstanceId,lastUpdate ) {
    e.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/DiagnosticReport/Delete?episodeOfCareId=${episodeOfCareId}&diagnosticReportId=${formInstanceId}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${formInstanceId}`).remove();
            toastr.success(`Success`);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}


$(document).ready(function () {
    let url = new URL(window.location.href);
    let currentEpisodeOfCareId = url.searchParams.get("EpisodeOfCareId");

    if (currentEpisodeOfCareId) {
        episodeOfCareId = currentEpisodeOfCareId;
    } else {
        episodeOfCareId = null;
    }
});

function pushStateWithoutFilter(num) {
    if (episodeOfCareId) {
        history.pushState({}, '', `?EpisodeOfCareId=${episodeOfCareId}&page=${num}&pageSize=${getPageSize()}`);
    } else {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}