function advanceFilter() {
    $("#advancedFilterModal").modal("hide");
    //send to browse page
    sendFilterData();
}

$('#homeSearch').change(function (e) {
    $("#searchModal").val($(this).val());
});

$('#searchModal').change(function (e) {
    $("#homeSearch").val($(this).val());
});

function sendFilterData() {
    window.location.href = `/ThesaurusGlobal/Browse?term=${$('#searchModal').val()}&Language=${$('#language').val()}&Author=${$('#author').val()}&License=${$('#license').val()}&TermIndicator=${$('input[name="exact"]:checked').val()}`;
}

$('#homeSearchIcon').on("click", function (e) {
    window.location.href = `/ThesaurusGlobal/Browse?term=${$("#homeSearch").val()}`;
});


$('#homeSearch').keypress(function (e) {
    if (e.which == 13) {
        window.location.href = `/ThesaurusGlobal/Browse?term=${$(this).val()}`;
    }
});

function advancedFilterModal(event) {
    event.preventDefault();
    event.stopPropagation();
    $("#advancedFilterModal").modal("show");
}

function getTotalChartData() {
    $.ajax({
        url: '/ThesaurusGlobal/GetTotalChartData',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $('#thesaurusEntriesTotal').html(response.ThesaurusTotal);
            $('#documentsTotal').html(response.DocumentTotal);
            $('#datasetsTotal').html(response.DatasetTotal);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

$(document).ready(function () {
    getTotalChartData();
});

function manageUsers() {
    window.location.href = '/ThesaurusGlobal/Users';
}