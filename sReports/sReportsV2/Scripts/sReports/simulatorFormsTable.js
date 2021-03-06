function reloadSecondaryTable(url, container, pageNumIdentifier) {
    //setFilterFromUrl();
    let requestObject = {};
    checkUrlPageParams();
    checkSecondaryPage();
    requestObject.Page = window[pageNumIdentifier];
    requestObject.PageSize = getPageSize();
    if ($('#title').val()) {
        requestObject.Title = $('#title').val();
    }

    $.ajax({
        type: 'GET',
        url: `${url}`,
        data: requestObject,
        success: function (data) {
            $(`#${container}`).html(data);
            $(`#${container}`).show();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
            $(`#${container}`).hide();
        }
    });
}

function changePageSecondary(num, e, url, container, pageNumIdentifier, preventPushHistoryState) {
    e.preventDefault();
    formsCurrentPage = num;
    if (!preventPushHistoryState) {
        history.pushState({}, '', `?page=${currentPage}&pageSize=${getPageSize()}&secondaryPage=${num}`);
    }
    reloadSecondaryTable(url, container, pageNumIdentifier);
}

function redirectToDistributionParams(event, thesaurusId, versionId) {
    event.preventDefault();
    event.stopPropagation();
    window.location.href = `/FormDistribution/GetByThesaurusId?thesaurusId=${thesaurusId}&VersionId=${versionId}`;
}

function filter() {
    reloadSecondaryTable("/FormDistribution/ReloadForms", "formsTableContainer", "formsCurrentPage")
}
