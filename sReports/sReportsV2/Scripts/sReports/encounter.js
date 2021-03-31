function submitForm(form) {
    //$('#idEpisodeOfCare').validate();
    if ($(form).valid()) {
        var request = {};

        request['Id'] = $("#id").val();
        request['EpisodeOfCareId'] = $("#eocId").val();
        request['Status'] = $("#status").val();
        request['Class'] = $("#classification").val();
        request['Type'] = $("#type").val();
        request['ServiceType'] = $("#servicetype").val();
        request['EpisodeOfCareId'] = $("#eocId").val();
        request['LastUpdate'] = $("#lastUpdate").val();
        var eocId = $("#eocId").val();

        $.ajax({
            type: "POST",
            url: "/Encounter/Create",
            data: request,
            success: function (data) {
                //toastr.options = {
                //    timeOut: 100
                //}
                //toastr.options.onHidden = function () { window.location.href = `/Encounter/GetAllEncounter?eocId=${eocId}`; }
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
    let requestObject = {};
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    requestObject.EpisodeOfCareId = $("#eocID").val();

    $.ajax({
        type: 'GET',
        url: '/Encounter/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });

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

function createEncounterEntry() {
    var eocId = $('#eocID').val();
    window.location.href = `/Encounter/Create?eocId=${eocId}`;
}

function editEntity(event,id) {
    window.location.href = `/Encounter/Edit?encounterId=${id}`;
    event.preventDefault();
}

function cancelEncounterEdit(id) {
    window.location.href = `/Encounter/GetAllEncounter?eocId=${id}`;
}

